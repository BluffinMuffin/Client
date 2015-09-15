using System;
using System.Linq;
using System.Collections.Generic;
using BluffinMuffin.Client.DataTypes;
using BluffinMuffin.Client.DataTypes.EventHandling;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.Game;
using BluffinMuffin.Protocol.Lobby;
using Com.Ericmas001.Games;
using Com.Ericmas001.Net.Protocol;
using Com.Ericmas001.Util;
using AbstractCommand = BluffinMuffin.Protocol.AbstractCommand;

namespace BluffinMuffin.Client.Protocol
{
    public class GameTcpClient : CommandQueueCommunicator<GameObserver>, IPokerGame
    {
        #region Fields
        private readonly DummyTable m_PokerTable = new DummyTable();
        private int m_TablePosition;
        private readonly string m_PlayerName;
        private readonly int m_NoPort;

        #endregion Fields

        #region Properties
        public PokerGameObserver Observer { get; private set; }
        public TableInfo Table { get { return m_PokerTable; } }
        #endregion Properties

        #region Ctors & Init
        public GameTcpClient(string name, int port)
        {
            Observer = new PokerGameObserver(this);
            m_TablePosition = -1;
            m_PlayerName = name;
            m_NoPort = port;
        }

        protected override void InitializeCommandObserver()
        {
            m_CommandObserver.CommandReceived += OnCommandReceived;
            SendedSomething += OnCommandSended;
            m_CommandObserver.BetTurnEndedCommandReceived += OnBetTurnEndedCommandReceived;
            m_CommandObserver.BetTurnStartedCommandReceived += OnBetTurnStartedCommandReceived;
            m_CommandObserver.GameEndedCommandReceived += OnGameEndedCommandReceived;
            m_CommandObserver.GameStartedCommandReceived += OnGameStartedCommandReceived;
            m_CommandObserver.PlayerHoleCardsChangedCommandReceived += OnPlayerHoleCardsChangedCommandReceived;
            m_CommandObserver.PlayerJoinedCommandReceived += OnPlayerJoinedCommandReceived;
            m_CommandObserver.SeatUpdatedCommandReceived += OnSeatUpdatedCommandReceived;
            m_CommandObserver.PlayerTurnBeganCommandReceived += OnPlayerTurnBeganCommandReceived;
            m_CommandObserver.PlayerTurnEndedCommandReceived += OnPlayerTurnEndedCommandReceived;
            m_CommandObserver.PlayerWonPotCommandReceived += OnPlayerWonPotCommandReceived;
            m_CommandObserver.TableClosedCommandReceived += OnTableClosedCommandReceived;
            m_CommandObserver.DiscardRoundStartedCommandReceived += OnDiscardRoundStartedCommandReceived;

            m_CommandObserver.PlayerSitInResponseReceived += OnPlayerSitInResponseReceived;
            m_CommandObserver.PlayerSitOutResponseReceived += OnPlayerSitOutResponseReceived;
        }

        void OnDiscardRoundStartedCommandReceived(object sender, CommandEventArgs<DiscardRoundStartedCommand> e)
        {
            lock (m_PokerTable)
            {
                Observer.RaiseDiscardActionNeeded(m_PokerTable.Seats[m_TablePosition].Player, e.Command.MinimumCardsToDiscard,e.Command.MaximumCardsToDiscard);
            }
        }

        private void OnCommandSended(object sender, KeyEventArgs<string> e)
        {
            LogManager.Log(LogLevel.MessageLow, "GameClient.m_CommandObserver_CommandReceived", "{0} SENT -={1}=-", m_PlayerName, e.Key);
        }

        #endregion Ctors & Init

        #region CommandObserver Event Handling

        void OnPlayerSitOutResponseReceived(object sender, CommandEventArgs<PlayerSitOutResponse> e)
        {
            lock (m_PokerTable)
            {
                Observer.RaiseSitOutResponseReceived(e.Command.Success);
            }
            OnSeatUpdatedCommandReceived(sender, new CommandEventArgs<SeatUpdatedCommand>(new SeatUpdatedCommand()
            {
                TableId = e.Command.TableId,
                Seat = new SeatInfo()
                {
                    NoSeat = m_TablePosition,
                    Player = null
                }
            }));
        }

        void OnPlayerSitInResponseReceived(object sender, CommandEventArgs<PlayerSitInResponse> e)
        {
            lock (m_PokerTable)
            {
                Observer.RaiseSitInResponseReceived(e.Command.NoSeat);
            }
            OnSeatUpdatedCommandReceived(sender, new CommandEventArgs<SeatUpdatedCommand>(new SeatUpdatedCommand()
            {
                TableId = e.Command.TableId,
                Seat = new SeatInfo()
                {
                    NoSeat = e.Command.NoSeat,
                    Player = new PlayerInfo(m_PlayerName, 0)
                }
            }));
        }
        void OnCommandReceived(object sender, StringEventArgs e)
        {
            lock (m_PokerTable)
            {
                LogManager.Log(LogLevel.MessageLow, "GameClient.m_CommandObserver_CommandReceived", "{0} RECV -={1}=-", m_PlayerName, e.Str);
            }
        }

        void OnBetTurnEndedCommandReceived(object sender, CommandEventArgs<BetTurnEndedCommand> e)
        {
            lock (m_PokerTable)
            {
                var cmd = e.Command;

                InitPotAmounts(cmd.PotsAmounts, 0);
                m_PokerTable.HigherBet = 0;
                m_PokerTable.Players.ForEach(p => p.MoneyBetAmnt = 0);

                Observer.RaiseGameBettingRoundEnded();
            }
        }

        void OnBetTurnStartedCommandReceived(object sender, CommandEventArgs<BetTurnStartedCommand> e)
        {
            lock (m_PokerTable)
            {
                var cmd = e.Command;
                SetCards(cmd.Cards);
                foreach (var seat in e.Command.Seats)
                    m_PokerTable.Seats[seat.NoSeat].SeatAttributes = seat.SeatAttributes;

                Observer.RaiseGameBettingRoundStarted(cmd.BettingRoundId);
                Observer.RaiseGameGenerallyUpdated();
            }
        }

        void OnGameEndedCommandReceived(object sender, CommandEventArgs<GameEndedCommand> e)
        {
            lock (m_PokerTable)
            {
                m_PokerTable.TotalPotAmnt = 0;

                Observer.RaiseGameEnded();
            }
        }

        void OnGameStartedCommandReceived(object sender, CommandEventArgs<GameStartedCommand> e)
        {
            lock (m_PokerTable)
            {
                var cmd = e.Command;
                m_PokerTable.People.Clear();
                m_PokerTable.SetCards();
                for (var i = 0; i < m_PokerTable.Params.MaxPlayers; ++i)
                {
                    m_PokerTable.SetSeat(cmd.Seats[i]);
                    m_PokerTable.People.Add(m_PokerTable.Seats[i].Player);

                }

                if (m_TablePosition >= 0 && cmd.NeededBlindAmount > 0)
                    Send(new PlayerPlayMoneyCommand() { AmountPlayed = cmd.NeededBlindAmount });

                Observer.RaiseGameGenerallyUpdated();
                Observer.RaiseGameBlindNeeded();
            }
        }

        void OnPlayerHoleCardsChangedCommandReceived(object sender, CommandEventArgs<PlayerHoleCardsChangedCommand> e)
        {
            lock (m_PokerTable)
            {
                var cmd = e.Command;
                var p = m_PokerTable.Seats[cmd.NoSeat].Player;

                if (p != null)
                {
                    SetPlayerVisibility(p, cmd.PlayerState, cmd.FaceUpCards.Select(ConvertToGameCard), cmd.FaceDownCards.Select(ConvertToGameCard));

                    Observer.RaisePlayerHoleCardsChanged(p);
                }
            }
        }


        void OnPlayerJoinedCommandReceived(object sender, CommandEventArgs<PlayerJoinedCommand> e)
        {
            lock (m_PokerTable)
            {
                var cmd = e.Command;
                var p = new PlayerInfo() { Name = cmd.PlayerName };

                m_PokerTable.JoinTable(p);

                Observer.RaisePlayerJoined(p);
            }
        }

        void OnSeatUpdatedCommandReceived(object sender, CommandEventArgs<SeatUpdatedCommand> e)
        {
            lock (m_PokerTable)
            {
                var s = e.Command.Seat;
                if (!s.IsEmpty)
                {
                    m_PokerTable.SitInToTable(s.Player, s.NoSeat);
                    if (m_PlayerName == s.Player.Name)
                        m_TablePosition = s.NoSeat;
                }
                else
                    m_PokerTable.ClearSeat(s.NoSeat);

                Observer.RaiseSeatUpdated(s);
            }

        }

        void OnPlayerTurnBeganCommandReceived(object sender, CommandEventArgs<PlayerTurnBeganCommand> e)
        {
            lock (m_PokerTable)
            {
                var cmd = e.Command;
                var ps = m_PokerTable.Seats[cmd.NoSeat];

                if (!ps.IsEmpty)
                {
                    m_PokerTable.ChangeCurrentPlayerTo(ps);
                    m_PokerTable.MinimumRaiseAmount = cmd.MinimumRaiseAmount;

                    Observer.RaisePlayerActionNeeded(ps.Player);
                }
            }
        }

        void OnPlayerTurnEndedCommandReceived(object sender, CommandEventArgs<PlayerTurnEndedCommand> e)
        {
            lock (m_PokerTable)
            {
                var cmd = e.Command;
                var p = m_PokerTable.Seats[cmd.NoSeat].Player;

                m_PokerTable.HigherBet = Math.Max(m_PokerTable.HigherBet, cmd.TotalPlayedMoneyAmount);
                m_PokerTable.TotalPotAmnt = cmd.TotalPot;

                if (p != null)
                {
                    p.MoneyBetAmnt = cmd.TotalPlayedMoneyAmount;
                    p.MoneySafeAmnt = cmd.TotalSafeMoneyAmount;
                    p.State = cmd.PlayerState;

                    Observer.RaisePlayerActionTaken(p, cmd.ActionTakenType, cmd.ActionTakenAmount);
                }
            }
        }

        void OnPlayerWonPotCommandReceived(object sender, CommandEventArgs<PlayerWonPotCommand> e)
        {
            lock (m_PokerTable)
            {
                var cmd = e.Command;
                var p = m_PokerTable.Seats[cmd.NoSeat].Player;

                if (p != null)
                {
                    p.MoneySafeAmnt = cmd.TotalPlayerMoney;

                    Observer.RaisePlayerWonPot(p, cmd.PotId, cmd.WonAmount, cmd.WinningHand, cmd.WinningCards);
                }
            }
        }

        void OnTableClosedCommandReceived(object sender, CommandEventArgs<TableClosedCommand> e)
        {
            lock (m_PokerTable)
            {
                Observer.RaiseEverythingEnded();
            }
        }

        public void FirstTableInfoReceived(JoinTableResponse cmd)
        {
            lock (m_PokerTable)
            {
                IsPlaying = cmd.GameHasStarted;
                InitPotAmounts(cmd.PotsAmount, cmd.TotalPotAmount);
                SetCards(cmd.BoardCards);
                m_PokerTable.Params = cmd.Params;
                m_PokerTable.People.Clear();

                for (var i = 0; i < cmd.Params.MaxPlayers; ++i)
                {
                    m_PokerTable.SetSeat(cmd.Seats[i]);
                    m_PokerTable.People.Add(m_PokerTable.Seats[i].Player);

                }
                Observer.RaiseGameGenerallyUpdated();
            }
        }
        #endregion CommandObserver Event Handling

        #region Public Methods
        public void Disconnect()
        {
            if (IsConnected)
            {
                Send(new DisconnectCommand());
            }
        }

        public bool PlayMoney(PlayerInfo p, int amnt)
        {
            Send(new PlayerPlayMoneyCommand() { TableId=m_NoPort, AmountPlayed = amnt });
            return true;
        }

        public int AfterPlayerSat(PlayerInfo p, int noSeat = -1, int moneyAmount = 1500)
        {
            Send(new PlayerSitInCommand()
            {
                TableId = m_NoPort, 
                MoneyAmount = moneyAmount,
                NoSeat = noSeat
            });
            return -1;
        }

        public bool SitOut(PlayerInfo p)
        {
            Send(new PlayerSitOutCommand() { TableId = m_NoPort, });
            return true;
        }

        public void Discard(string[] cards)
        {
            Send(new PlayerDiscardActionCommand() {TableId = m_NoPort, CardsDiscarded = cards});
        }

        #endregion Public Methods

        #region Private Methods
        private void InitPotAmounts(List<int> amounts, int totalPotAmnt)
        {
            m_PokerTable.Pots.Clear();
            m_PokerTable.TotalPotAmnt = totalPotAmnt;

            for (var i = 0; i < amounts.Count && amounts[i] > 0; ++i)
            {
                m_PokerTable.Pots.Add(new MoneyPot(i, amounts[i]));
                m_PokerTable.TotalPotAmnt += amounts[i];
            }
        }

        private GameCard ConvertToGameCard(string c)
        {
            return new GameCard(string.IsNullOrEmpty(c) ? "--" : c.Replace("10", "T"));
        }

        private void SetCards(IEnumerable<string> cardsId)
        {
            m_PokerTable.SetCards(cardsId.Select(ConvertToGameCard).ToArray());
        }

        private void SetPlayerVisibility(PlayerInfo p, PlayerStateEnum pState, IEnumerable<GameCard> faceUpCards, IEnumerable<GameCard> faceDownCards)
        {
            p.State = pState;
            p.FaceUpCards = faceUpCards.Select(x => x.ToString()).ToArray();
            p.FaceDownCards = faceDownCards.Select(x => x.ToString()).ToArray();
        }
        #endregion Private Methods


        public bool IsPlaying { get; private set; }

        public override void Send(AbstractCommand command)
        {
            var gameCommand = command as AbstractGameCommand;
            if (gameCommand != null)
            {
                gameCommand.TableId = m_NoPort;
            }
            base.Send(command);
        }
    }
}

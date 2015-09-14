using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BluffinMuffin.Client.Windows.Forms.Properties;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Client.Windows.Forms.Lobby
{
    public partial class CreateTableTabControl : UserControl
    {
        readonly LobbyTypeEnum m_LobbyType;
        readonly GameInfo m_Game;
        string CurrentVariant { get { return lstVariant.SelectedItem.ToString(); } }
        //RuleInfo CurrentRule { get { return m_Rules.First(r => r.Name == CurrentVariant); } }
        public CreateTableTabControl(string playerName, LobbyTypeEnum lobby, GameInfo game)
        {
            m_LobbyType = lobby;
            m_Game = game;
            InitializeComponent();
            txtTableName.Text = playerName + Resources.CreateTableTabControl_CreateTableTabControl__Table;
            InitVariants();
            RefreshNumbers();
            grpQuickMode.Visible = lobby == LobbyTypeEnum.QuickMode;
        }

        private void InitVariants()
        {
            object[] names = m_Game.AvailableVariants.OrderBy(x => (int)x).Select(EnumFactory<GameSubTypeEnum>.ToString).ToArray();
            lstVariant.Items.AddRange(names);
            lstVariant.SelectedItem = names[0];
            VariantChoosen();
        }

        private void lstVariant_SelectedIndexChanged(object sender, EventArgs e)
        {
            VariantChoosen();
        }

        private void VariantChoosen()
        {
            SetMinMax();
            SetBetLimits();
            SetBlindTypes();
            SetWaitingTimes();
        }

        private void SetMinMax()
        {

            nudNbPlayersMin.Minimum = m_Game.MinPlayers;
            nudNbPlayersMin.Maximum = m_Game.MaxPlayers;
            nudNbPlayersMin.Value = m_Game.MinPlayers;

            nudNbPlayersMax.Minimum = m_Game.MinPlayers;
            nudNbPlayersMax.Maximum = m_Game.MaxPlayers;
            nudNbPlayersMax.Value = m_Game.MaxPlayers;
        }

        private void SetBetLimits()
        {
            lstBetLimit.Items.Clear();
            lstBetLimit.Items.AddRange(m_Game.AvailableLimits.Select(EnumFactory<LimitTypeEnum>.ToString).ToArray());
            lstBetLimit.SelectedIndex = 0;
        }

        private void SetBlindTypes()
        {
            lstBlinds.Items.Clear();
            lstBlinds.Items.AddRange(m_Game.AvailableBlinds.Select(EnumFactory<BlindTypeEnum>.ToString).ToArray());
            lstBlinds.SelectedIndex = 0;
            SetBlindRules();
        }

        private void SetBlindRules()
        {
            var blind = EnumFactory<BlindTypeEnum>.Parse((string)lstBlinds.SelectedItem);
            ucAnte.Visible = blind == BlindTypeEnum.Antes;
            ucBlinds.Visible = blind == BlindTypeEnum.Blinds;
        }

        private void SetWaitingTimes()
        {
            nudWTAPlayerAction.Value = 500;
            nudWTABoardDealed.Value = 500;
            nudWTAPotWon.Value = 2500;
            grpTimes.Enabled = true;
        }

        private void nudNbPlayersMin_ValueChanged(object sender, EventArgs e)
        {
            nudNbPlayersMax.Minimum = nudNbPlayersMin.Value;
        }

        private void nudNbPlayersMax_ValueChanged(object sender, EventArgs e)
        {
            nudNbPlayersMin.Maximum = nudNbPlayersMax.Value;
        }

        private void lstBlinds_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBlindRules();
        }

        public TableParams Params
        {
            get
            {
                var moneyUnit = (int)nudMoneyUnit.Value;
                LobbyOptions lobby = null;
                switch (m_LobbyType)
                {
                    case LobbyTypeEnum.QuickMode:
                        lobby = new LobbyOptionsQuickMode()
                        {
                            StartingAmount = (int)nudStartingAmount.Value,
                        };
                        break;

                    case LobbyTypeEnum.RegisteredMode:
                        lobby = new LobbyOptionsRegisteredMode()
                        {
                            IsMaximumBuyInLimited = rdBuyInLimited.Checked,
                        };
                        break;
                }
                GameTypeOptions options = null;
                switch (m_Game.GameType)
                {
                    case GameTypeEnum.CommunityCardsPoker:
                        options = new GameTypeOptionsCommunity();
                        break;
                    case GameTypeEnum.StudPoker:
                        options = new GameTypeOptionsStud();
                        break;
                    case GameTypeEnum.DrawPoker:
                        options = new GameTypeOptionsDraw();
                        break;
                }
                BlindTypeEnum blind = EnumFactory<BlindTypeEnum>.Parse((string) lstBlinds.SelectedItem);
                LimitTypeEnum limit = EnumFactory<LimitTypeEnum>.Parse((string)lstBetLimit.SelectedItem);
                return new TableParams()
                {
                    TableName = txtTableName.Text,
                    Options = options,
                    Variant = EnumFactory<GameSubTypeEnum>.Parse((string)lstVariant.SelectedItem),
                    MinPlayersToStart = (int)nudNbPlayersMin.Value,
                    MaxPlayers = (int)nudNbPlayersMax.Value,
                    WaitingTimes = new ConfigurableWaitingTimes()
                    {
                        AfterPlayerAction = (int)nudWTAPlayerAction.Value,
                        AfterBoardDealed = (int)nudWTABoardDealed.Value,
                        AfterPotWon = (int)nudWTAPotWon.Value,
                    },
                    Lobby = lobby,
                    Blind = blind,
                    Limit = limit,
                    GameSize = moneyUnit,
                };
            }
        }

        private void NeedToRefreshNumbers(object sender, EventArgs e)
        {
            RefreshNumbers();
        }
        private void RefreshNumbers()
        {
            var moneyUnit = (int)nudMoneyUnit.Value;
            var minBuyIn = moneyUnit * 20;
            var maxBuyIn = moneyUnit * 100;
            lblGameSize.Text = string.Format("${0} / ${1}", moneyUnit, moneyUnit * 2);
            lblMinimumBuyIn.Text = string.Format("${0}", minBuyIn);
            lblMaximumBuyIn.Text = string.Format("(${0})", maxBuyIn);
            ucAnte.SetAnte(moneyUnit);
            ucBlinds.SetBlinds(moneyUnit);
            nudStartingAmount.Minimum = minBuyIn;
            nudStartingAmount.Maximum = rdBuyInLimited.Checked ? maxBuyIn : int.MaxValue;
            nudStartingAmount.Increment = moneyUnit;
        }
    }
}

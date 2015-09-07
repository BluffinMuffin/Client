using System;
using BluffinMuffin.Protocol.Game;
using Com.Ericmas001.Net.Protocol.JSON;

namespace BluffinMuffin.Client.Protocol
{
    public class GameObserver : JsonCommandObserver
    {
        
        public event EventHandler<CommandEventArgs<BetTurnEndedCommand>> BetTurnEndedCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<BetTurnStartedCommand>> BetTurnStartedCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<GameEndedCommand>> GameEndedCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<GameStartedCommand>> GameStartedCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<PlayerHoleCardsChangedCommand>> PlayerHoleCardsChangedCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<PlayerJoinedCommand>> PlayerJoinedCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<SeatUpdatedCommand>> SeatUpdatedCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<PlayerTurnBeganCommand>> PlayerTurnBeganCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<PlayerTurnEndedCommand>> PlayerTurnEndedCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<PlayerWonPotCommand>> PlayerWonPotCommandReceived = delegate { };

        
        public event EventHandler<CommandEventArgs<TableClosedCommand>> TableClosedCommandReceived = delegate { };


        public event EventHandler<CommandEventArgs<PlayerSitInResponse>> PlayerSitInResponseReceived = delegate { };


        public event EventHandler<CommandEventArgs<PlayerSitOutResponse>> PlayerSitOutResponseReceived = delegate { };


        public event EventHandler<CommandEventArgs<DiscardRoundStartedCommand>> DiscardRoundStartedCommandReceived = delegate { };
    }
}

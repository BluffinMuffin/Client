using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Client.DataTypes.EventHandling;
using BluffinMuffin.Protocol.DataTypes;

namespace BluffinMuffin.Client.DataTypes
{
    public interface IPokerGame
    {
        PokerGameObserver Observer { get; }

        TableInfo Table { get; }

        bool PlayMoney(PlayerInfo p, int amnt);
        int AfterPlayerSat(PlayerInfo p, int noSeat = -1, int moneyAmount = 1500);
        bool SitOut(PlayerInfo p);

        bool IsPlaying { get; }
    }
}

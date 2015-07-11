using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.EventHandling;

namespace BluffinMuffin.Client.DataTypes.EventHandling
{
    public class MinMaxEventArgs : PlayerInfoEventArgs
    {
        public int Minimum { get; private set; }
        public int Maximum { get; private set; }

        public MinMaxEventArgs(PlayerInfo player, int min, int max) : base(player)
        {
            Minimum = min;
            Maximum = max;
        }
    }
}

using System;
using BluffinMuffin.Protocol.DataTypes.Options;

namespace BluffinMuffin.Client.DataTypes.EventHandling
{
    public class GameMessageOptionEventArgs : EventArgs
    {
        public GameMessageOption Info { get; }

        public GameMessageOptionEventArgs(GameMessageOption o)
        {
            Info = o;
        }
    }
}

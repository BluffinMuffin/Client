using System;

namespace BluffinMuffin.Client.DataTypes.EventHandling
{
    public class RoundEventArgs : EventArgs
    {
        private readonly int m_Round;
        public int Round { get { return m_Round; } }

        public RoundEventArgs(int r)
        {
            m_Round = r;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluffinMuffin.Client.DataTypes.EventHandling
{
    public class MinMaxEventArgs : EventArgs
    {
        public int Minimum { get; private set; }
        public int Maximum { get; private set; }

        public MinMaxEventArgs(int min, int max)
        {
            Minimum = min;
            Maximum = max;
        }
    }
}

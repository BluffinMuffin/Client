using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluffinMuffin.Client.DataTypes
{
    public interface IPokerViewer
    {
        void SetGame(IPokerGame c);
        void Start();
    }
}

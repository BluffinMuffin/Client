using BluffinMuffin.Client.Windows.Forms.Game;
using BluffinMuffin.Protocol.DataTypes.Options;

namespace BluffinMuffin.Client.Game
{
    public partial class QuickModeTableForm : TableForm
    {
        public QuickModeTableForm()
        {
            InitializeComponent();
        }

        protected override int GetSitInMoneyAmount()
        {
            return ((LobbyOptionsQuickMode)m_Game.Table.Params.Lobby).StartingAmount;
        }
    }
}

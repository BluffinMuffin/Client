using BluffinMuffin.Client.Properties;
using BluffinMuffin.DataTypes;
using System;
using System.Windows.Forms;
using BluffinMuffin.Client.Extensions;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;

namespace BluffinMuffin.Client.Game
{
    public partial class BuyInForm : Form
    {
        public bool Ok { get; private set; }
        public int BuyIn { get; private set; }

        public BuyInForm(UserInfo user, TableParams parms)
        {
            InitializeComponent();
            lblAccountMoney.Text = Resources.BuyInForm_BuyInForm_Dollar + user.TotalMoney;
            lblMoneyUnit.Text = Resources.BuyInForm_BuyInForm_Dollar + parms.GameSize;
            lblMin.Text = Resources.BuyInForm_BuyInForm_Dollar + parms.Lobby.MinimumBuyInAmount(parms.GameSize);
            lblMax.Text = Resources.BuyInForm_BuyInForm_Dollar + Math.Min(parms.Lobby.MaximumBuyInAmount(parms.GameSize), user.TotalMoney);
            nudBuyIn.Minimum = parms.Lobby.MinimumBuyInAmount(parms.GameSize);
            nudBuyIn.Maximum = (decimal)Math.Min(parms.Lobby.MaximumBuyInAmount(parms.GameSize), user.TotalMoney);
            nudBuyIn.Increment = parms.GameSize;
            nudBuyIn.Value = parms.Lobby.MinimumBuyInAmount(parms.GameSize);
        }

        private void btnSitIn_Click(object sender, EventArgs e)
        {
            BuyIn = (int)nudBuyIn.Value;
            Ok = true;
            Close();
        }
    }
}

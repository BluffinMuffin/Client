using BluffinMuffin.Client.Properties;
using BluffinMuffin.DataTypes;
using System;
using System.Windows.Forms;
using BluffinMuffin.Protocol.DataTypes;

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
            lblMin.Text = Resources.BuyInForm_BuyInForm_Dollar + parms.MinimumBuyInAmount;
            lblMax.Text = Resources.BuyInForm_BuyInForm_Dollar + Math.Min(parms.MaximumBuyInAmount, user.TotalMoney);
            nudBuyIn.Minimum = parms.MinimumBuyInAmount;
            nudBuyIn.Maximum = (decimal)Math.Min(parms.MaximumBuyInAmount, user.TotalMoney);
            nudBuyIn.Increment = parms.GameSize;
            nudBuyIn.Value = parms.MinimumBuyInAmount;
        }

        private void btnSitIn_Click(object sender, EventArgs e)
        {
            BuyIn = (int)nudBuyIn.Value;
            Ok = true;
            Close();
        }
    }
}

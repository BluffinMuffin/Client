using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BluffinMuffin.Client.DataTypes;
using BluffinMuffin.Client.DataTypes.EventHandling;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.EventHandling;
using Com.Ericmas001.Util;

namespace BluffinMuffin.Client.Windows.Forms.Game
{
    public partial class TableForm : TableViewerForm
    {
        private MinMaxEventArgs m_DiscardInfo;

        protected virtual int GetSitInMoneyAmount() { return 1500; }

        protected TableForm()
        {
            InitializeComponent();
            DisableButtons();
            DisableButton(btnSitOut);
        }

        private void btnFold_Click(object sender, EventArgs e)
        {
            DisableButtons();
            var table = m_Game.Table;
            var p = table.Seats[m_NoSeat].Player;
            m_Game.PlayMoney(p, -1);
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            DisableButtons();
            var table = m_Game.Table;
            var p = table.Seats[m_NoSeat].Player;
            m_Game.PlayMoney(p, table.CallAmnt(p));
        }

        private void btnRaise_Click(object sender, EventArgs e)
        {
            DisableButtons();
            var table = m_Game.Table;
            var p = table.Seats[m_NoSeat].Player;
            m_Game.PlayMoney(p, (int)nudRaise.Value - p.MoneyBetAmnt);
        }

        private void DisableButtons()
        {
            DisableButton(btnCall);
            DisableButton(btnRaise);
            DisableButton(btnFold);
            nudRaise.Enabled = false;
        }

        private void DisableButton(Button btn)
        {
            if (btn.Enabled)
            {
                btn.Enabled = false;
                btn.Tag = btn.BackColor;
                btn.BackColor = Color.DimGray;
            }
        }

        private void EnableButton(Button btn)
        {
            if (!btn.Enabled)
            {
                btn.Enabled = true;
                btn.BackColor = (Color)btn.Tag;
            }
        }
        public override void SetGame(IPokerGame c)
        {
            base.SetGame(c);
            m_Game.Observer.PlayerActionNeeded += OnPlayerActionNeeded;
            m_Game.Observer.SitInResponseReceived += OnSitInResponseReceived;
            m_Game.Observer.SitOutResponseReceived += OnSitOutResponseReceived;
            m_Game.Observer.SeatUpdated += OnGameGenerallyUpdated;
            m_Game.Observer.GameGenerallyUpdated += OnGameGenerallyUpdated;
            m_Game.Observer.DiscardActionNeeded += OnDiscardActionNeeded;
        }

        void OnDiscardActionNeeded(object sender, MinMaxEventArgs e)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new EventHandler<MinMaxEventArgs>(OnDiscardActionNeeded), new[] { sender, e });
                return;
            }
            m_DiscardInfo = e;
            string[] cards = e.Player.HoleCards.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            chkC1.Checked = false;
            chkC2.Checked = false;
            chkC3.Checked = false;
            chkC4.Checked = false;
            chkC5.Checked = false;
            chkC1.Visible = false;
            chkC2.Visible = false;
            chkC3.Visible = false;
            chkC4.Visible = false;
            chkC5.Visible = false;
            lblC1.Visible = false;
            lblC2.Visible = false;
            lblC3.Visible = false;
            lblC4.Visible = false;
            lblC5.Visible = false;
            switch (cards.Length)
            {
                case 1:
                    chkC3.Visible = true;
                    lblC3.Visible = true;
                    lblC3.Text = cards[0];
                    break;
                case 2:
                    chkC2.Visible = true;
                    lblC2.Visible = true;
                    lblC2.Text = cards[0];
                    chkC4.Visible = true;
                    lblC4.Visible = true;
                    lblC4.Text = cards[1];
                    break;
                case 3:
                    chkC2.Visible = true;
                    lblC2.Visible = true;
                    lblC2.Text = cards[0];
                    chkC3.Visible = true;
                    lblC3.Visible = true;
                    lblC3.Text = cards[1];
                    chkC4.Visible = true;
                    lblC4.Visible = true;
                    lblC4.Text = cards[2];
                    break;
                case 4:
                    chkC1.Visible = true;
                    lblC1.Visible = true;
                    lblC1.Text = cards[0];
                    chkC2.Visible = true;
                    lblC2.Visible = true;
                    lblC2.Text = cards[1];
                    chkC3.Visible = true;
                    lblC3.Visible = true;
                    lblC3.Text = cards[2];
                    chkC4.Visible = true;
                    lblC4.Visible = true;
                    lblC4.Text = cards[3];
                    break;
                case 5:
                    chkC1.Visible = true;
                    lblC1.Visible = true;
                    lblC1.Text = cards[0];
                    chkC2.Visible = true;
                    lblC2.Visible = true;
                    lblC2.Text = cards[1];
                    chkC3.Visible = true;
                    lblC3.Visible = true;
                    lblC3.Text = cards[2];
                    chkC4.Visible = true;
                    lblC4.Visible = true;
                    lblC4.Text = cards[3];
                    chkC5.Visible = true;
                    lblC5.Visible = true;
                    lblC5.Text = cards[4];
                    break;
            }
            btnDiscard.Enabled = e.Minimum == 0;
            grpDiscard.Visible = true;
        }

        void OnSitOutResponseReceived(bool success)
        {
            SitOutEnabled(!success);
            if (success)
            {
                m_NoSeat = -1;
                SitInButtonsShowing(true);
            }
        }

        void OnGameGenerallyUpdated(object sender, EventArgs e)
        {
            SitInButtonsShowing(m_NoSeat == -1);
        }

        void OnPlayerActionNeeded(object sender, PlayerInfoEventArgs e)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new EventHandler<PlayerInfoEventArgs>(OnPlayerActionNeeded), new[] { sender, e });
                return;
            }
            var p = e.Player;
            var table = m_Game.Table;
            if (p.NoSeat == m_NoSeat)
            {
                EnableButton(btnFold);
                SetCallButtonName(p);
                EnableButton(btnCall);
                if (table.HigherBet < p.MoneyAmnt)
                {
                    var min = table.MinRaiseAmnt(p) + p.MoneyBetAmnt;
                    EnableButton(btnRaise);
                    nudRaise.Enabled = true;
                    nudRaise.Minimum = min;
                    nudRaise.Maximum = p.MoneyAmnt;
                    nudRaise.Value = min;
                    nudRaise.Increment = min;
                }
            }
        }

        private void SetCallButtonName(PlayerInfo p)
        {
            var table = m_Game.Table;
            var s = "CALL";
            if (table.CanCheck(p))
                s = "CHECK";
            else if (table.HigherBet >= p.MoneyAmnt)
                s = "ALL-IN";
            btnCall.Text = s;
        }

        private void btnSitIn_Click(object sender, EventArgs e)
        {
            var money = GetSitInMoneyAmount();
            if (money >= 0)
            {
                SitInButtonsShowing(false);
                var name = ((Control)sender).Name;
                var seatWanted = int.Parse(name.Substring(name.Length - 1));
                m_Game.AfterPlayerSat(null, seatWanted, money);
            }
        }

        void OnSitInResponseReceived(int noSeat)
        {
            m_NoSeat = noSeat;
            SitOutEnabled(noSeat != -1);
            SitInButtonsShowing(noSeat == -1);
        }

        private void SitInButtonsShowing(bool visible)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new BooleanHandler(SitInButtonsShowing), visible);
                return;
            }
            SuspendLayout();
            for (var i = 0; i < 10; ++i )
            {
                var btnSitIn = Controls["btnSitIn" + i] as Button;
                if (i < m_Game.Table.Seats.Count && m_Game.Table.Seats[i].IsEmpty)
                {
                    if (btnSitIn != null) btnSitIn.Visible = visible;
                }
                else if (btnSitIn != null) btnSitIn.Visible = false;
            }
            ResumeLayout();
        }

        private void SitOutEnabled(bool enable)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new BooleanHandler(SitOutEnabled), enable);
                return;
            }
            SuspendLayout();
            if(enable)
                EnableButton(btnSitOut);
            else
                DisableButton(btnSitOut);
            ResumeLayout();
        }

        private void btnSitOut_Click(object sender, EventArgs e)
        {
            DisableButtons();
            SitOutEnabled(false);
            m_Game.SitOut(null);
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            var chks = new[] { new KeyValuePair<CheckBox, Label>(chkC1, lblC1), new KeyValuePair<CheckBox, Label>(chkC2, lblC2), new KeyValuePair<CheckBox, Label>(chkC3, lblC3), new KeyValuePair<CheckBox, Label>(chkC4, lblC4), new KeyValuePair<CheckBox, Label>(chkC5, lblC5) };
            string[] cardsToDiscard = chks.Where(x => x.Key.Checked).Select(x => x.Value.Text).ToArray();
            grpDiscard.Visible = false;
            m_Game.Discard(cardsToDiscard);
        }

        private void chkDiscard_CheckedChanged(object sender, EventArgs e)
        {
            var chks = new[] {chkC1, chkC2, chkC3, chkC4, chkC5};
            btnDiscard.Enabled = chks.Count(x => x.Checked) >= m_DiscardInfo.Minimum && chks.Count(x => x.Checked) <= m_DiscardInfo.Maximum;
        }
    }
}

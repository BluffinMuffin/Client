using System;
using System.Windows.Forms;

namespace BluffinMuffin.Client.Windows.Forms.Lobby
{
    public partial class BlindUcBlinds : UserControl
    {
        public BlindUcBlinds()
        {
            InitializeComponent();
        }

        public void SetBlinds( int bigblind )
        {
            lblSmallBlind.Text = string.Format("${0}", bigblind / 2);
            lblBigBlind.Text = string.Format("${0}", bigblind);
        }
    }
}

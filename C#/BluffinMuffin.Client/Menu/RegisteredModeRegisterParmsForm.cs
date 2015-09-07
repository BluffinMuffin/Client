using BluffinMuffin.Client.Splash;
using Com.Ericmas001.Windows.Forms;
using System;
using System.Windows.Forms;

namespace BluffinMuffin.Client.Menu
{
    public partial class RegisteredModeRegisterParmsForm : Form
    {
        private readonly string m_ServerAdress;
        private readonly int m_ServerPort;

        public RegisteredModeRegisterParmsForm(string serverAddress, int serverPort)
        {
            InitializeComponent();
            m_ServerAdress = serverAddress;
            m_ServerPort = serverPort;
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            btnRegister.Enabled = !string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtPasswordConfirm.Text) && !string.IsNullOrEmpty(txtEmail.Text) && !string.IsNullOrEmpty(txtEmailConfirm.Text) && !string.IsNullOrEmpty(txtDisplayName.Text);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Hide();
            var info = new RegisteredModeRegisterSplashInfo(m_ServerAdress, m_ServerPort, txtUsername.Text, txtPassword.Text, txtEmail.Text, txtDisplayName.Text);
            if (new StepSplashForm(info).ShowDialog() == DialogResult.OK)
            {
                new LobbyRegisteredModeForm(info.Server).ShowDialog();
                Close();
            }
            else
                Show();

        }
    }
}

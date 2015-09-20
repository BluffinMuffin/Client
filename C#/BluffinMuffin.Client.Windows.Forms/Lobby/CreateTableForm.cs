using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;

namespace BluffinMuffin.Client.Windows.Forms.Lobby
{
    public partial class CreateTableForm : Form
    {
        public TableParams Params { get; private set; }
        public CreateTableForm(string playerName, LobbyTypeEnum lobby, IEnumerable<GameInfo> games)
        {
            Params = null;
            InitializeComponent();

            var infos = games as GameInfo[] ?? games.ToArray();
            foreach (var type in infos.Select(r => r.GameType).Distinct())
            {
                var t = type.ToString();
                var tp = new TabPage(t) { Name = "tab" + t, BackColor = Color.White };
                tp.Controls.Add(new CreateTableTabControl(playerName, lobby, infos.First(r => r.GameType.ToString() == t)) { Dock = DockStyle.Fill });
                tabControl1.TabPages.Add(tp);
            }
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {
            Params = tabControl1.SelectedTab.Controls.OfType<CreateTableTabControl>().First().Params;
            Close();
        }
    }
}

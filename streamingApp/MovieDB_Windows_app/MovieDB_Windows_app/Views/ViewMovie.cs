using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieDB_Windows_app.Views
{
    public partial class ViewMovie : Form
    {
        private static string mguid;
        public ViewMovie(string guid)
        {
            InitializeComponent();
            mguid = guid;
        }

        private void ViewMovieSettings_Load(object sender, EventArgs e)
        {
            ipTextBox.Text = Properties.Settings.Default.WEBip;
            portTextBox.Text = Properties.Settings.Default.WEBport;
            dirTextBox.Text = Properties.Settings.Default.WEBDir;
        }

        private void playMovieButton_Click(object sender, EventArgs e)
        {
            if(mguid != "" || mguid.Length > 0)
            {
                string url = "";
                if (ipTextBox.TextLength < 0 || ipTextBox.TextLength > 16)
                {
                    MessageBox.Show("IP is incorrect => ip length is either too long or is empty");
                }
                else
                {
                    url += ipTextBox.Text;
                    if (portTextBox.Text != "" || portTextBox.TextLength < 0) url += ":" + portTextBox.Text;
                    if (dirTextBox.Text != "" || dirTextBox.TextLength < 0) url += "/" + dirTextBox.Text;
                    System.Diagnostics.Process.Start("http://" + url + "/play/index.php?id=" + mguid);
                }
            }
        }
    }
}

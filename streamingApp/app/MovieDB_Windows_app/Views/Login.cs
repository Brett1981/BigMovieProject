using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using appDllv1;
using System.Net;

namespace MovieDB_Windows_app.Views
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var prevButtonText = loginButton.Text;
            loginButton.Enabled = false;
            loginButton.Text = "Wait";
            new API(Properties.Settings.Default.APIip, Properties.Settings.Default.APIport);

            var login = await API.Communication.Get.Login(new API.Auth.Login()
                {
                    username = usernameTextBox.Text,
                    password = Convert.ToBase64String(
                               Encoding.ASCII.GetBytes(passwordTextBox.Text))
                }
            );
            if (login != null && login.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var user = JsonConvert.DeserializeObject<User.Info>(await login.Content.ReadAsStringAsync());
                if (user.unique_id != null)
                {
                    Main m = new Main(user);
                    this.Hide();
                    m.Show();
                }
            }
            else
            {
                if(login != null) { 
                    if (login.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    || login.StatusCode == System.Net.HttpStatusCode.NotFound)
                        MessageBox.Show("Username or password is incorrect!");

                    loginButton.Text = prevButtonText;
                    loginButton.Enabled = true;
                }
                
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}

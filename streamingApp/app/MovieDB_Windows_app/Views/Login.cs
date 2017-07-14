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
            if (login != null)
            {
                if(login.StatusCode == System.Net.HttpStatusCode.OK)
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
                    var error = (login.StatusCode == System.Net.HttpStatusCode.Unauthorized) ? "You are not authorized to access this application!" : "";
                    error = (login.StatusCode == System.Net.HttpStatusCode.NotFound && error == "") ? "Username or password is incorrect!" : error;
                    error = (error == "") ? "Something went wrong!" : error;
                    MessageBox.Show(error);
                }

                loginButton.Text = prevButtonText;
                loginButton.Enabled = true;

            }
            
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}

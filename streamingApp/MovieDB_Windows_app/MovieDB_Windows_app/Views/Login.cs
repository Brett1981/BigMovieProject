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
using MovieDB_Windows_app.Resources;

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

            var login = await API.Communication.Login(new API.Auth.Login()
                {
                    username = usernameTextBox.Text,
                    password = Convert.ToBase64String(
                               Encoding.ASCII.GetBytes(passwordTextBox.Text))
                }
            );
            if(login.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var c = await login.Content.ReadAsStringAsync();
                if (c != null)
                {
                    var user = JsonConvert.DeserializeObject<User.Info>(c);
                    if (user.unique_id != null)
                    {
                        Main m = new Main(user);
                        this.Hide();
                        m.Show();
                    }
                   
                }
            }
            else
            {
                if(login.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Incorect user credentials!");
                }
                else if(login.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Username or password is incorrect!");
                }
            }
        }
    }
}

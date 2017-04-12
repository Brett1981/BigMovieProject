using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MovieDB_Windows_app.Resources;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace MovieDB_Windows_app.Views
{
    public partial class NewUser : Form
    {
        private User.Info user;
        public NewUser()
        {
            InitializeComponent();
        }

        private void NewUser_Load(object sender, EventArgs e)
        {
            Dictionary<string,User.Groups> groupDict = new Dictionary<string, User.Groups>();

            foreach (var g in GlobalVar.GlobalData.users.groups)
            {
                groupDict.Add(g.type, g);
            }
            userGroupComboBox.DataSource = new BindingSource(groupDict, null);
            userGroupComboBox.ValueMember = "Value";
            userGroupComboBox.DisplayMember = "Key";
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (user == null) user = new User.Info();
            if(sender is TextBox)
            {
                var item = (TextBox)sender;
                CheckTextBoxDataConsistency(item, item.Text);
            }
        }

        private void CheckTextBoxDataConsistency(TextBox control, string item)
        {
            bool status = false;
            var currentProperty = control.Name.Split('_')[0];
            switch (currentProperty)
            {
                case "username":
                    status = IsValidInput(control);
                    break;
                case "password":
                    status = IsValidInput(control); 
                    break;
                case "displayName":
                    status = IsValidInput(control); 
                    break;
                case "email":
                    status = IsValidInput(control);
                    break;
            }

            if (status)
            {
                if(currentProperty == "displayName") currentProperty = "display_name";

                var prop = user.GetType().GetProperty(currentProperty);
                if(prop != null)
                {
                    prop.SetValue(user, control.Text);
                }
                
            }
        }

        private bool IsValidInput(Control item)
        {
            
            if (item.Name.Contains("email"))
            {
                try
                {
                    MailAddress m = new MailAddress(item.Text);
                }
                catch (FormatException)
                {
                    SetError(item,"Email address is incorrect!");
                    return false;
                }
            }
            else
            {
                Regex regex = new Regex(@"[a-zA-Z0-9_-]+$");
                if (regex.IsMatch(item.Text, 0) == false)
                {
                    SetError(item);
                    return false;
                }
            }

            ClearError(item);
            return true;
        }
        private void ProccessingNewUserObject(User.Info data)
        {

        }

        private void SetError(Control item, string error = "Data contains invalid characters")
        {
            errorProvider1.SetError(item, error);
        }

        private void ClearError(Control item)
        {
            errorProvider1.SetError(item, "");
        }

        private async void createUserButton_Click(object sender, EventArgs e)
        {
            
            
            if (errorProvider1.Container.Components.Count > 0)
            {
                bool IsErrorSet = false;
                foreach (Control c in errorProvider1.ContainerControl.Controls)
                {
                    if (errorProvider1.GetError(c) != "") { IsErrorSet = true; break; }
                }
                    
                if (IsErrorSet)
                    MessageBox.Show("Check form for errors and re-submit your data", "Check data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    await CreateNewUser();
            }
            else
            {
                await CreateNewUser();
            }
        }

        private async Task CreateNewUser()
        {
            try
            {
                user.birthday = dateTimePicker1.Value;

                //send data to API for new user
                var status = await API.Communication.Create.User(
                        user, 
                        ((KeyValuePair<string, User.Groups>)userGroupComboBox.SelectedItem).Value
                    );
                if (status.IsSuccessStatusCode)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
    }
}

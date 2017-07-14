using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using appDllv1;
using System.Reflection;
using System.Diagnostics;

namespace MovieDB_Windows_app.Views
{
    public partial class Users : Form
    {
        private Objects.Communication.Users UsersData = new Objects.Communication.Users();
        private string[] ignoreUserItem = new string[] { "User_Group", "group" };
        private List<DataGridViewRow> UserDataGridRows = new List<DataGridViewRow>();

        private Dictionary<string, User.Groups> groupDict;
        private bool Init = false;
        public Users()
        {
            InitializeComponent();
            UsersData = GlobalVar.GlobalData.users;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Users_Load(object sender, EventArgs e)
        {
            CreateDataGridRows();
        }

        private void CreateDataGridRows()
        {
            Init = true;
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows.Clear();
            }
            foreach (var item in UsersData.users)
            {
                if (item != null)
                {
                    if (dataGridView1.Columns.Count == 0)
                    {
                        foreach (var p in item.GetType().GetProperties())
                        {
                            if (!Array.Exists(ignoreUserItem, element => element.Equals(p.Name)))
                            {
                                dataGridView1.Columns.Add(p.Name, p.Name);
                            }
                            else
                            {

                                if (p.Name == ignoreUserItem[0] || p.Name == ignoreUserItem[1])
                                {

                                    DataGridViewComboBoxColumn group = new DataGridViewComboBoxColumn()
                                    {
                                        HeaderText = "Select Group",
                                        Name = "Access"
                                    };

                                    groupDict = new Dictionary<string, User.Groups>();

                                    foreach (var g in UsersData.groups)
                                    {
                                        groupDict.Add(g.type, g);
                                    }
                                    group.DataSource = new BindingSource(groupDict, null);
                                    group.ValueMember = "Value";
                                    group.DisplayMember = "Key";
                                    dataGridView1.Columns.Add(group);
                                }
                            }
                        }
                        DataGridViewButtonColumn bttn = new DataGridViewButtonColumn()
                        {
                            HeaderText = "Save",
                            Name = "Save",
                            Text = "Update"
                        };
                        dataGridView1.Columns.Add(bttn);
                        dataGridView1.CellClick += DataGridView1_CellClick;
                        dataGridView1.CellValueChanged +=
                            new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
                        dataGridView1.CurrentCellDirtyStateChanged +=
                            new EventHandler(dataGridView1_CurrentCellDirtyStateChanged);
                    }
                    SetDataGridUserRow(item);
                }
            }
            Init = false;
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dataGridView1.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell && !Init)
            {
                DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                var group = (User.Groups)cb.Value;
                if (group != null)
                {
                    var user = UsersData.users.Where(x => x.unique_id == (string)dataGridView1.Rows[e.RowIndex].Cells["unique_id"].Value).FirstOrDefault();
                    // do stuff
                    if(user != null)
                    {
                        //send to API
                        API.Communication.Edit.User(user, group);
                    }
                    dataGridView1.Invalidate();
                }
            }
            
        }

        private void SetDataGridUserRow(User.Info user)
        {
            int currentRow = dataGridView1.Rows.Count;
            if(user != null)
            {
                try
                {
                    dataGridView1.Rows.Add(
                        user.Id.ToString(),
                        user.unique_id,
                        user.username,
                        user.password,
                        (user.profile_image != null) ? user.profile_image : "Not set",
                        (user.display_name != null) ? user.display_name : "",
                        (user.profile_created.ToString() != null) ? user.profile_created.ToString() : "",
                        (user.last_logon.ToString() != null) ? user.last_logon.ToString() : "",
                        (user.birthday.ToString() != null) ? user.birthday.ToString() : "",
                        user.email.ToString()
                        );
                    dataGridView1.Rows[currentRow]
                        .Cells["Access"]
                        .Value = FindUserGroup(
                            user,
                            currentRow
                            );

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Exception: " + ex.Message);
                }
                
            }
        }

        private User.Groups FindUserGroup(User.Info user,int row)
        {
            if(row >= 0)
            {

                var x = (((DataGridViewComboBoxCell)dataGridView1.Rows[row].Cells["Access"])
                   .Items
                   .Cast<KeyValuePair<string, User.Groups>>()
                   .ToList());

                return x.Where(y => y.Value.Id == user.group)
                    .FirstOrDefault()
                    .Value;
            }
            return null;
            
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 0 && e.RowIndex >= 0)
                {
                    var column = dataGridView1.Columns[e.ColumnIndex].HeaderText;
                    var item = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    switch (column.ToLower())
                    {
                        case "display_name":
                            ResetStatusLabel();
                            //implementacija za spreminjanje display nama
                            break;
                        case "password":
                            ResetStatusLabel();
                            //implementacija za spremembo gesla
                            break;
                        case "select group":
                            //implementacija spremembe groupe uporabniku
                            ChangeUserGroup(column,item);
                            break;
                        default:
                            dataGridStatusLabel.Text = "Caution: ";
                            dataGridDescriptionLabel.Text = "The selected item cannot be edited for security reasons!";
                            ; break;
                    }
                    //Debug.WriteLine("Not implemented cellclick");
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }

        private void ResetStatusLabel()
        {
            dataGridDescriptionLabel.Text = "";
            dataGridStatusLabel.Text = "";
        }

        private void ChangeUserGroup(string cell, DataGridViewCell data)
        {
            var x = cell;
            var y = data;
        }

        private async void addUser_Click(object sender, EventArgs e)
        {
            Views.NewUser nUser = new NewUser();
            if(nUser.ShowDialog() == DialogResult.OK)
            {
                UsersData = await API.Communication.Get.AllUsers();
                CreateDataGridRows();
            }
            
        }

        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            User.Info user = new User.Info();
            if(dataGridView1.SelectedRows.Count == 1)
            {
                var item = ((dataGridView1.SelectedRows.Cast<DataGridViewRow>().ToList()[0].Cells));
                foreach(var i in item)
                {
                    if(i is DataGridViewTextBoxCell)
                    {
                        var textBox = (DataGridViewTextBoxCell)i;
                        switch (dataGridView1.Columns[textBox.ColumnIndex].Name)
                        {
                            case "unique_id":
                                user.unique_id = textBox.Value.ToString();
                                break;
                            case "username":
                                user.username = textBox.Value.ToString();
                                break;
                        }
                    }
                }
                if(user.unique_id != null && user.username != null)
                {
                    if (MessageBox.Show("Are you sure you want to remove user: "+ user.username,"Delete user",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //call api to remove user
                        var response = await API.Communication.Remove.User(user);
                        if (response.IsSuccessStatusCode)
                        {
                            UsersData = await API.Communication.Get.AllUsers();
                            CreateDataGridRows();
                        }
                    }
                }
                
            }
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            UsersData = await API.Communication.Get.AllUsers();
            CreateDataGridRows();
        }
    }
}
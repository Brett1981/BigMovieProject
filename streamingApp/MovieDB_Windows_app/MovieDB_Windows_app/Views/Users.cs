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
using System.Reflection;
using System.Diagnostics;

namespace MovieDB_Windows_app.Views
{
    public partial class Users : Form
    {
        private APIObjects.Users UsersData = new APIObjects.Users();
        private string[] ignoreUserItem = new string[] { "User_Group", "groupId" };
        private List<DataGridViewRow> UserDataGridRows = new List<DataGridViewRow>();

        private Dictionary<string, User.Groups> groupDict;

        public Users()
        {
            InitializeComponent();
            UsersData = GlobalVar.GlobalData.users;
        }

        private void Users_Load(object sender, EventArgs e)
        {
            foreach(var item in UsersData.users)
            {
                if(item != null)
                {
                    if(dataGridView1.Columns.Count == 0)
                    {
                        foreach(var p in item.GetType().GetProperties())
                        {
                            if(!Array.Exists(ignoreUserItem,element => element.Equals(p.Name)))
                            {
                                dataGridView1.Columns.Add(p.Name, p.Name);
                            }
                            else {
                                
                                if(p.Name == ignoreUserItem[0])
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
                                    group.DataSource = new BindingSource(groupDict,null);
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
                    }
                    SetDataGridUserRow(item);
                }
            }
            this.Width = dataGridView1.Width+300;
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
                    user.profile_image.ToString() ?? "",
                    user.display_name.ToString(),
                    user.profile_created.ToString(),
                    user.last_logon.ToString() ?? "",
                    user.birthday.ToString(),
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

                return x.Where(y => y.Value.Id == user.groupId)
                    .FirstOrDefault()
                    .Value;
            }
            return null;
            
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var column = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            var item = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            switch (column.ToLower())
            {
                case "display_name": ResetStatusLabel();
                    //implementacija za spreminjanje display nama
                    break;
                case "password": ResetStatusLabel();
                    //implementacija za spremembo gesla
                    break;
                case "select group":
                    //implementacija spremembe groupe uporabniku
                    ;
                    break;
                default: dataGridStatusLabel.Text = "Caution: ";
                    dataGridDescriptionLabel.Text = "The selected item cannot be edited for security reasons!";
                        ;break;
            }
            Debug.WriteLine("Not implemented cellclick");
        }

        private void ResetStatusLabel()
        {
            dataGridDescriptionLabel.Text = "";
            dataGridStatusLabel.Text = "";
        }

        private void addUser_Click(object sender, EventArgs e)
        {
            Views.NewUser nUser = new NewUser();
            nUser.Show();
        }
    }
}
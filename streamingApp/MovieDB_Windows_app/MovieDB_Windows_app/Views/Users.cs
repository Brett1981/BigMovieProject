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

namespace MovieDB_Windows_app.Views
{
    public partial class Users : Form
    {
        private APIObjects.Users UsersData = new APIObjects.Users();
        private string[] ignoreUserItem = new string[] { "User_Group", "groupId" };
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
                                    DataGridViewComboBoxColumn select = new DataGridViewComboBoxColumn()
                                    {
                                        HeaderText = "Select Group",
                                        Name = "Access"
                                    };
                                    foreach (var g in UsersData.groups)
                                    {
                                        select.Items.Add(g.type);
                                    }
                                    dataGridView1.Columns.Add(select);
                                }
                            }
                            DataGridViewButtonColumn bttn = new DataGridViewButtonColumn()
                            {
                                HeaderText = "Save",
                                Name = "Save",
                            };
                            dataGridView1.Columns.Add(bttn);
                            dataGridView1.CellClick += DataGridView1_CellClick;
                            SetUserToView(item);
                        }
                        
                    }
                    else
                    {
                        SetUserToView(item);
                    }
                }
            }
        }

        private  void SetUserToView(User.Info user)
        {
            if(user != null)
            {
                dataGridView1.Rows.Add(user);
                /*DataGridViewRow row = new DataGridViewRow();
                //row.Cells["Id"].Value               = user.Id.ToString();
                row.Cells["unique_Id"].Value        = user.unique_id;
                row.Cells["username"].Value         = user.username;
                row.Cells["password"].Value         = user.password;
                row.Cells["profile_image"].Value    = user.profile_image.ToString();
                row.Cells["display_name"].Value     = user.display_name.ToString();
                row.Cells["profile_create"].Value   = user.profile_created.ToString();
                row.Cells["last_logon"].Value       = user.last_logon.ToString();
                row.Cells["birthday"].Value         = user.birthday.ToString();
                row.Cells["email"].Value            = user.Id.ToString();
                row.Cells["Id"].Value               = user.Id.ToString();
                dataGridView1.Rows.Add(row);*/
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}


/*
 DataGridViewColumn[] dataGridObjects = new DataGridViewColumn[item.GetType().GetProperties().Count() - 2 + 2];
                        int countItems = 0;
                        foreach(var p in item.GetType().GetProperties())
                        {
                            if (!Array.Exists(ignoreUserItem, element => element.Equals(p.Name)))
                            {
                                dataGridObjects[countItems++] = new DataGridViewColumn()
                                {
                                    Name = p.Name,
                                    HeaderText = p.Name
                                };
                            }
                            else {
                                if(p.Name == ignoreUserItem[0])
                                {
                                    DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn()
                                    {
                                        HeaderText = "Select Group",
                                        Name = "Access"
                                    };
                                    foreach (var g in UsersData.groups)
                                    {
                                        cmb.Items.Add(g.type);
                                    }
                                    dataGridObjects[countItems++] = cmb;
                                }
                            }
                        }

                        dataGridObjects[countItems++] = new DataGridViewButtonColumn()
                        {
                            HeaderText = "Save",
                            Name = "Save",
                        };
                        dataGridView1.Columns.AddRange(dataGridObjects);
                        dataGridView1.CellClick += DataGridView1_CellClick;
                        SetUserToView(item);

    */

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
        public Users()
        {
            InitializeComponent();
            UsersData = GlobalVar.GlobalData.users;
        }

        private void Users_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = UsersData.users;
            var last = dataGridView1.Columns[dataGridView1.Columns.Count - 1];
            /*foreach (var item in UsersData.users)
            {
                Type myType = item.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                foreach(var p in props)
                {
                    
                }
            }*/
        }
    }
}

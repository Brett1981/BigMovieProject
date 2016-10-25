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

namespace MovieDB_Windows_app
{
    public partial class Video : Form
    {
        public Video()
        {
            InitializeComponent();
        }

        private void Video_Load(object sender, EventArgs e)
        {
            var data = (GlobalVar.GlobalMovieData.data.Where(a => a.DBid == GlobalVar.GlobalMovieId).First());
            string serverUrl = GlobalVar.GlobalServerUrl + data.ServerLocation;
            
        }

        
    }
}

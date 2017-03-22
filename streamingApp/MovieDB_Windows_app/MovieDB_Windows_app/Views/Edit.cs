using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MovieDB_Windows_app.Resources;

namespace MovieDB_Windows_app.Views
{
    public partial class Edit : Form
    {
        private static Movie.Data movie = new Movie.Data();
        private static Button movieButton = new Button();
        private static API api = new API();
        private bool Edited = false;
        public Edit(Movie.Data data,Button mbutton)
        {
            InitializeComponent();
            movie = data;
            movieButton = mbutton;
            this.Text += "-> " + movie.Movie_Info.title;
            foreach (Control cont in this.Controls)
            {
                if (cont.Name.Contains("TextBox") || cont.Name.Contains("CheckedBox") || cont.Name.Contains("Image"))
                {
                    GetMovieInfoToObject(cont.Name);
                }
            }
        }

        private void Edit_Load(object sender, EventArgs e)
        {
            
        }

        private void GetMovieInfoToObject(string item)
        {
            Movie.Data m = new Movie.Data() {
                Movie_Info = new Movie.Info()
            };
            var sitem = item.Split(new string[] { "TextBox", "CheckedBox", "Image" }, StringSplitOptions.None);
            if (sitem != null && sitem[0] != "poster" )
            {
                var prop = m.GetType().GetProperty(sitem[0]);
                
                if(prop == null)
                {
                    //property is maybe in movie info
                    var propinfo = m.Movie_Info.GetType().GetProperty(sitem[0]);
                    if(propinfo != null)
                    {
                        var c = Controls.Find(item, true).First();
                        SetObject(sitem[0], "info", c);
                    }
                }
                else
                {
                    //property is in movie data
                    var c = Controls.Find(item, true).First();
                    SetObject(sitem[0], "movie", c);
                }
            }
            else
            {
                //item is poster
                posterImage.BackgroundImage = movieButton.BackgroundImage;
                posterImage.BackgroundImageLayout = ImageLayout.Stretch;

            }
        }
        private void SetObject(string item,string type,Control c)
        {
            if (c is TextBox || c is CheckBox)
            {
                string p = "";
                if(type == "movie")
                    p = GetMoviePropertie(item).ToString();
                else 
                    p = GetMovieInfoPropertie(item).ToString();
                if (c is CheckBox)
                    ((CheckBox)c).Checked = (p.ToLower() == "true") ? true : false;
                else
                    c.Text = p;

            }
            
        }
        private object GetMoviePropertie(string item)
        {
            Type myType = movie.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            return props.Where(y => y.Name.Contains(item)).First().GetValue(movie, null);
        }
        private object GetMovieInfoPropertie(string item)
        {
            Type myType = movie.Movie_Info.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            return props.Where(y => y.Name.Contains(item)).First().GetValue(movie.Movie_Info, null);
        }

        private void watchButton_Click(object sender, EventArgs e)
        {
            Views.ViewMovie vm = new Views.ViewMovie(movie.guid);
            vm.ShowDialog();
        }

        private async void enabledCheckedBox_Click(object sender, EventArgs e)
        {
            Movie.Data temp = movie;
            string desc = "";

            if (enabledCheckedBox.Checked) { desc = "Are you sure you want to DISABLE this movie!"; }
            else { desc = "Are you sure you want to ENABLE this movie!"; }

            if (MessageBox.Show(desc, temp.Movie_Info.title, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                temp.enabled = !enabledCheckedBox.Checked;
                Debug.WriteLine(await API.Communication.ChangeMovieStatus(temp));
                var m = await API.Communication.GetMovie(temp.guid);

                if (m.enabled == temp.enabled)
                {
                    enabledCheckedBox.Checked = (m.enabled == true) ? true : false;
                    Edited = true;
                }
            }
            
        }

        private void enabledCheckedBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Edit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Edited)
            {
                this.DialogResult = DialogResult.OK;
            }
            else this.DialogResult = DialogResult.Cancel;
        }
    }
}

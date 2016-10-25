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
using System.IO;

namespace MovieDB_Windows_app
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        public bool isPersistantStoragePrimary = false;
        private async void Form1_Load(object sender, EventArgs e)
        {
            GlobalVar.GlobalServerUrl = "http://213.143.88.177:1515";
            API api = new API();
            var data = await api.RetrieveData();
            
            //item1 = genres, item2 = movies
            GlobalVar.GlobalGenresData = data.Item1;
            GlobalVar.GlobalMovieData = data.Item2;
            genresToCheckedListBox(data.Item1);
            DisplayingMovieData(data.Item2);
            
        }
        private void genresToCheckedListBox(jsonGenresClass data)
        {
            for (int i = 0;i<data.genres.Count;i++)
            {
                var name = data.genres[i].name + " | " + data.genres[i].id;
                checkedListBox1.Items.Add(name);
            }
        }
        private async void DisplayingMovieData(jsonMovieClass data)
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = data.data.Count;
            
            for(int i = 0; i < data.data.Count;i++)
            {
                toolStripProgressBar1.Value++;
                toolStripStatusLabel2.Text = "Retrieving movie data...";
                Button bttn = new Button();

                API api = new API();
                bttn.Height = 300;
                bttn.Width = 200;
                bttn.Visible = true;
                bttn.BackgroundImage = await api.ImageDownload("https://image.tmdb.org/t/p/w300/" + data.data[i].DBposter);
                bttn.BackgroundImageLayout = ImageLayout.Stretch;
                bttn.Click += new EventHandler(button_movie_Click);
                bttn.Text = data.data[i].DBid;
               // genre.Text = api.RetrieveGenres(data.data[i].DBgenres);
                flowLayoutPanel1.Controls.Add(bttn);
                
            }
            toolStripStatusLabel2.Text = "Completed";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(var i = 0; i < checkedListBox1.Items.Count;i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }
        
        private void button_movie_Click(object sender, EventArgs e)
        {
            var item = (Button)sender;
            GlobalVar.GlobalMovieId = item.AccessibilityObject.Name;
            //MessageBox.Show(sender.ToString());
            //this.hide();
            Video video = new Video();
            video.Show();
        }
    }
}

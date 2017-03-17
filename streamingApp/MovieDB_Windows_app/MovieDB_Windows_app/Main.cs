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
using System.Diagnostics;

namespace MovieDB_Windows_app
{
    public partial class Main : Form
    {
        public static Movie_Data movie_click_data { get; set; }
        public bool isPersistantStoragePrimary = false;

        public Main()
        {
            InitializeComponent();
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(dir + @"\Data") )
            {
                Directory.CreateDirectory(dir + @"\Data");
                
            }
            if (!Directory.Exists(dir + @"\Data\Images"))
            {
                Directory.CreateDirectory(dir + @"\Data\Images");
            }

            if(Properties.Settings.Default.DataPath == null || Properties.Settings.Default.DataPath == "")
            {
                Properties.Settings.Default.DataPath = dir + @"Data";
            }
            if(Properties.Settings.Default.ImagePath == null || Properties.Settings.Default.ImagePath == "")
            {
                Properties.Settings.Default.ImagePath = dir + @"Data\Images";
            }
            Properties.Settings.Default.Save();
        }

        

        private async void Form1_Load(object sender, EventArgs e)
        {
            SetMovieList();
        }
        public async void SetMovieList()
        {
            API api = new API();
            GlobalVar.GlobalMovieData = await api.getMovieData();
            if (GlobalVar.GlobalMovieData == null || GlobalVar.GlobalMovieData.Count < 0) MessageBox.Show("No connection could be made to the server!");
            else
            {
                GlobalVar.GlobalUserInfo = await api.getUsersData();
                DisplayMovieData(GlobalVar.GlobalMovieData);
            }
        }
        private async void DisplayMovieData(List<Movie_Data> data)
        {
            if (flowLayoutPanel1.Controls.Count > 0) flowLayoutPanel1.Controls.Clear();
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = data.Count();
            List<Button> bttns = new List<Button>();
            for (int i = 0; i < data.Count();i++)
            {
                toolStripProgressBar1.Value++;
                toolStripStatusLabel2.Text = "Retrieving movie data... " + i +" of "+ toolStripProgressBar1.Maximum;
                
                Button bttn = new Button() {
                    Height  = 240,
                    Width   = 160,
                    Visible = true,
                    BackgroundImageLayout   = ImageLayout.Stretch,
                    Text    = data[i].Movie_Info.id.ToString(),
                };
                bttn.BackgroundImage = await GetImage(data[i].Movie_Info.poster_path);
                bttn.Click += new EventHandler(button_movie_Click);

                bttns.Add(bttn);
                flowLayoutPanel1.Controls.Add(bttn);
            }
            GlobalVar.GlobalMovieButtonList = new List<Button>(bttns);
            toolStripStatusLabel2.Text = "Completed";
            bttns.Clear();
        }

        private async Task<Image> GetImage(string image)
        {
            var path = Properties.Settings.Default.ImagePath;
            image = image.Substring(1, image.Length - 1);
            if(File.Exists(path + "\\" +image))
            {
                var i = Image.FromFile(path + "\\" + image);
                if (i == null) return null; 
                return i;
            }
            var x = await API.Downloader.ImageDownload("https://image.tmdb.org/t/p/w160/" + image);
            try
            {
                x.Save(path + "\\" + image, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return x;
        } 

        private void button_movie_Click(object sender, EventArgs e)
        {
            var item = (Button)sender;
            GlobalVar.GlobalMovieId = item.AccessibilityObject.Name;

            movie_click_data = GlobalVar.GlobalMovieData
                .Where(x => x.Movie_Info.id.ToString() == GlobalVar.GlobalMovieId)
                .First();

            if(movie_click_data == null)
            {
                MessageBox.Show("There seems to be no movie selected!");
                
            }
            else
            {
                Views.Edit ed = new Views.Edit(movie_click_data, item);
                if (ed.ShowDialog() == DialogResult.OK)
                {
                   SetMovieList();
                }
               
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            if (searchTextBox.Text.Length > 0)
            {
                
                var search = GlobalVar.GlobalMovieData
                    .Where(x => x.Movie_Info.title.ToLower()
                    .Contains(searchTextBox.Text.ToLower()))
                    .ToList();

                if(search.Count > 0)
                {
                    List<Button> searchList = new List<Button>();
                    foreach(var item in search)
                    {
                        searchList.Add(GlobalVar.GlobalMovieButtonList
                            .Where(x => x.Text == item.Movie_Info.id.ToString())
                            .First());
                    }
                    if(searchList.Count > 0)
                    {
                        foreach(var item in searchList)
                        {
                            flowLayoutPanel1.Controls.Add(item);
                        }
                    }
                }
            }
            else
            {
                foreach(var item in GlobalVar.GlobalMovieButtonList)
                {
                    flowLayoutPanel1.Controls.Add(item);
                }
                
            }
        }
    }
}

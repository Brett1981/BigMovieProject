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
using static MovieDB_Windows_app.API;

namespace MovieDB_Windows_app
{
    public partial class Main : Form
    {
        public static Movie.Data movie_click_data { get; set; }
        public bool isPersistantStoragePrimary = false;
        public API api; 

        public Main(User.Info u = null)
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
            if(u != null)
            {
                GlobalVar.GlobalCurrentUserInfo = u;
                GlobalVar.GlobalAuthUser = new Auth.User()
                {
                    username = u.username,
                    unique_id = u.unique_id
                };
                userLogedIn.Text = u.username;
            }
            rightClickMovieContextMenu.ItemClicked += RightClickMovieContextMenu_ItemClicked;
            api = new API();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await SetMovieList();
            SetAPIHistoryDataGrid();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public async Task SetMovieList(List<Movie.Data> list = null , bool force = false)
        {
            if(GlobalVar.GlobalData == null || (GlobalVar.GlobalData.users == null ||
                GlobalVar.GlobalData.disks == null || 
                GlobalVar.GlobalData.movies == null) || force)
            {
                GlobalVar.GlobalData = await GlobalVar.client.InitAppData();
                
                
            }
            if (list == null && GlobalVar.GlobalData.movies != null)
                GlobalVar.GlobalData.movies = GlobalVar.GlobalData.movies;
            else
                GlobalVar.GlobalData.movies = list;

            if (GlobalVar.GlobalData.movies == null || GlobalVar.GlobalData.movies.Count < 0) MessageBox.Show("No connection could be made to the server!");
            else
            {
                DisplayMovieData(GlobalVar.GlobalData.movies);
            }
        }
        public void SetAPIHistoryDataGrid()
        {
            if (GlobalVar.GlobalData.apiHistory != null)
                dataGridView1.DataSource = GlobalVar.GlobalData.apiHistory;
        }

        private async void DisplayMovieData(List<Movie.Data> data)
        {
            if (flowLayoutPanel1.Controls.Count > 0) flowLayoutPanel1.Controls.Clear();
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = data.Count();
            List<Button> bttns = new List<Button>();
            for (int i = 0; i < data.Count();i++)
            {
                toolStripProgressBar1.Value++;
                StatusLabel.Text = "Retrieving movie data... " + i +" of "+ toolStripProgressBar1.Maximum;
                
                Button bttn = new Button() {
                    Height  = 240,
                    Width   = 160,
                    Visible = true,
                    BackgroundImageLayout   = ImageLayout.Stretch,
                    Text    = "",
                    Tag     = data[i]
                };
                bttn.BackgroundImage = await GetImage(data[i].Movie_Info.poster_path);
                bttn.MouseUp += new MouseEventHandler(button_mouse_Click);
                bttns.Add(bttn);
                flowLayoutPanel1.Controls.Add(bttn);
            }
            GlobalVar.GlobalMovieButtonList = new List<Button>(bttns);
            StatusLabel.Text = "Completed";
            bttns.Clear();
        }

        

        private async Task<Image> GetImage(string image)
        {
            var path = Properties.Settings.Default.ImagePath + image.Substring(1, image.Length - 1);

            if(File.Exists(path)) return Image.FromFile(path) ?? null; 

            var x = await Downloader.ImageDownload("https://image.tmdb.org/t/p/w160/" + image);
            try
            {
                x.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return x;
        } 

        private async void button_movie_Click(object sender, EventArgs e)
        {
            var item = (Button)sender;
            var m = (Movie.Data)item.Tag;
            if(m == null)
                MessageBox.Show("There seems to be no movie selected!");
            else
            {
                Views.Edit ed = new Views.Edit(m, item);
                if (ed.ShowDialog() == DialogResult.OK)
                    await SetMovieList();

            }
        }
        private void button_mouse_Click(object sender, MouseEventArgs e)
        {
            
            switch (e.Button)
            {
                case MouseButtons.Right:
                    SetContextMenu((Button)sender);
                    break;
                default:
                    button_movie_Click(sender, new EventArgs());
                    break;
            }
        }

        //Context menu
        private void SetContextMenu(Button b)
        {
            rightClickMovieContextMenu.Tag = b;
            rightClickMovieContextMenu.Show(Cursor.Position.X, Cursor.Position.Y);
        }
        //context menu item clicked
        private async void RightClickMovieContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var b = (Button)rightClickMovieContextMenu.Tag;
            var m = (Movie.Data)b.Tag;
            string desc = "";
            switch (e.ClickedItem.Text)
            {
                case "Enable / Disable":
                    {
                        if (m.enabled)
                        {
                            desc = "Are you sure you want to DISABLE this movie!";
                            m.enabled = false;
                        }
                        else
                        {
                            desc = "Are you sure you want to ENABLE this movie!";
                            m.enabled = true;
                        }
                        if (MessageBox.Show(desc, m.Movie_Info.title, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                        {
                            var r = await API.Communication.Set.MovieStatus(m);
                            var x = await r.Content.ReadAsStringAsync();
                            if (x.Contains("disabled") || x.Contains("enabled"))
                            {
                                var a = await API.Communication.Get.RefreshData(GlobalVar.GlobalAuthUser);
                                //await SetMovieList();
                            }
                                
                        }
                            

                    }
                    break;
                case "Edit":
                    {
                        Views.Edit edit = new Views.Edit(m,b);
                        edit.Show();
                    }
                    break;
                case "View":
                    {
                        Views.ViewMovie view = new Views.ViewMovie(m.guid);
                        view.Show();
                    }
                    break;
            }
        }

        //search box
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            if (searchTextBox.Text.Length > 0)
            {
                
                var search = GlobalVar.GlobalData.movies
                    .Where(x => x.Movie_Info.title.ToLower()
                    .Contains(searchTextBox.Text.ToLower()))
                    .ToList();

                if(search.Count > 0)
                {
                    List<Button> searchList = new List<Button>();
                    foreach(var item in search)
                    {
                        searchList.Add(GlobalVar.GlobalMovieButtonList
                            .Where(x => (Movie.Data)x.Tag == item)
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

        /// <summary>
        /// Refreshes movie list and creates new List in API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await SetMovieList(await API.Communication.Get.RefreshData(GlobalVar.GlobalAuthUser));
        }

        //clean temp folder
        private void cleanTempFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            foreach(FileInfo img in new DirectoryInfo(Properties.Settings.Default.ImagePath).GetFiles())
            {
                img.Delete();
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            GlobalVar.GlobalCurrentUserInfo = null;
            GlobalVar.GlobalMovieButtonList = null;
            GlobalVar.GlobalData = null;
            Application.Exit();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Views.Users u = new Views.Users();
            u.Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Views.Settings s = new Views.Settings();
            s.Show();
        }
    }
}

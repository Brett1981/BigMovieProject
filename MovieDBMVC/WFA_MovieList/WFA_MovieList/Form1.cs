using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;


namespace WFA_MovieList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static List<string> LocationList;
        public static Dictionary<string,List<string>> Files;
        public static Dictionary<string,string> LocationDictionary;
        public static List<string> Data = new List<string>();
        public API APICall = new API();
        public string Title = "MovieDataToJson";
        public string APICallTitle = "Processing ...";

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = Properties.Settings.Default.APIKey;
            if (LocationList == null)
            {
                LocationList = new List<string>();
            }
            if(LocationDictionary == null)
            {
                LocationDictionary = new Dictionary<string, string>();
            }
            if(Files == null)
            {
                Files = new Dictionary<string,List<string>>();
            }
        }

        private void setFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                if (!LocationList.Contains(folderBrowserDialog1.SelectedPath))
                {
                    LocationDictionary.Add(LocationDictionary.Count + 1.ToString(), folderBrowserDialog1.SelectedPath);
                    //LocationList.Add(folderBrowserDialog1.SelectedPath);
                }
            }
            if (!LocationList.Contains(folderBrowserDialog1.SelectedPath))
            {
                LocationList.Add(folderBrowserDialog1.SelectedPath);
                listBox2.DataSource = null;
            }
            listBox2.DataSource = LocationList;
            listBox2.Refresh();
        }

        private void updateFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = new List<string>();
            var movieList = new List<string>();
            foreach (string key in LocationDictionary.Keys)
            {
                var dir = LocationDictionary[key];
                
                list = Directory.GetDirectories(dir).ToList();
                foreach(var item in list)
                {
                    foreach(var dirFiles in Directory.GetFiles(item))
                    {
                        if (dirFiles.EndsWith(".mp4"))
                        {
                            movieList.Add(item);
                        }
                    }
                    
                }
                if(Files.ContainsKey(dir))
                {
                    Files.Remove(dir);
                }
                Files.Add(dir, movieList);
                
            }
            
            foreach (var key in Files.Keys)
            {
                var data_dir = Files[key].ToArray();
                for(int i = 0; i < data_dir.Length;i++)
                {
                    if (!Data.Contains(data_dir[i]))
                    {
                        Data.Add(data_dir[i]);
                    }
                    
                }
            }
            listBox1.DataSource = Data;
            progressBar1.Maximum = Data.Count;
            MessageBox.Show("Files found: " + Data.Count.ToString() , "Message");
        }
        private int countCall = 0;
        private async void button1_Click(object sender, EventArgs e)
        {
           
            
            var objectToSerialize = new JSON();
            objectToSerialize.data = new List<WFA_MovieList.Data>();
            for (int i = 0; i < Data.Count;i++)
            {
                progressBar1.Value = progressBar1.Value + 1;
                Data item = new WFA_MovieList.Data();
                item.Name = new DirectoryInfo(Data[i]).Name;
                item.ID = i;
                foreach (var key in Directory.GetFiles(Data[i]))
                {
                    //if (key.EndsWith("mp4", false, null) || key.EndsWith("avi", false, null) || key.EndsWith("mkv", false, null))
                    if (key.EndsWith("mp4", false, null))
                    {
                        var movieDataInDir = Path.GetFileName(key);
                        item.FileName = movieDataInDir;
                        break;
                    }
                }

                var driveLetter = Path.GetPathRoot(Data[i]);
                item.ServerLocation =  "/Movies_" + driveLetter[0] + "/"+item.Name + "/" + item.FileName;
                countCall++;
                if (countCall == 40)
                {
                    countCall = 1;
                    label2.Text = "Waiting 10s before calling API...";
                    await Task.Delay(10000);
                    

                }
                else
                {
                    this.Text = APICallTitle + item.Name;
                    label2.Text = "Retrieving Movie Info From API...";
                }
                var response = await APICall.RetrieveInfo(item.Name);
                
                if (response != null)
                {
                    item.DBid = response.id;
                    item.DBTitle = response.title;
                    item.DBgenres = response.genre_ids;
                    item.DBposter = response.poster_path;
                }
                else
                {
                    item.DBid = 000000;
                    item.DBTitle = "Unknown";
                    item.DBgenres = new List<int>();
                    item.DBposter = "";
                }
                
                
                
                objectToSerialize.data.Add(item);
            }
            await Task.Delay(0);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            var json = JsonConvert.SerializeObject(objectToSerialize);
            label2.Text = "Completed";
            textBox1.Text = json;
            this.Text = Title;
        }
        
        private void jSONExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                folderBrowserDialog1.ShowNewFolderButton = true;
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if(result == DialogResult.OK)
                {
                    File.WriteAllText(folderBrowserDialog1.SelectedPath + "\\data.json", textBox1.Text);
                    MessageBox.Show("File creation completed!");
                }
            }
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(textBox1.Text);
        }

        private async void getGenreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = true;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                var data = await APICall.RetriveGenres();
                if(data.genres != null)
                {
                    File.WriteAllText(folderBrowserDialog1.SelectedPath + "\\genres.json", JsonConvert.SerializeObject(data));
                    MessageBox.Show("File creation completed!");
                }
                
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.APIKey = textBox2.Text;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}

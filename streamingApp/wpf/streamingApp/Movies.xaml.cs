using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using streamingApp.Resources;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace streamingApp
{
    /// <summary>
    /// Interaction logic for Movies.xaml
    /// </summary>
    public partial class Movies : MetroWindow
    {
        private MetroNavigationWindow nav;
        public Movies()
        {
            InitializeComponent();
            GetMovies();
            nav = new MetroNavigationWindow();
            nav.Navigated += Nav_Navigated;
            nav.Navigating += Nav_Navigating;
        }

        private void Nav_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Nav_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task GetMovies()
        {
            HttpClient client = new HttpClient();
            try
            {
                var data = JsonConvert.DeserializeObject<List<MovieData>>(
                    await client.GetStringAsync("http://31.15.224.24:53851/api/video/allmovies")
                    );
                foreach(var item in data)
                {
                    item.MovieInfo.poster_path = "https://image.tmdb.org/t/p/w300/" + item.MovieInfo.poster_path;
                }
                MovieListView.ItemsSource = data;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void WrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var d = (WrapPanel)sender;
            var movie = (MovieData)d.DataContext;
            MainWindow w = new MainWindow();
            
            w.Show();
            this.Hide();
            //nav.Navigate(new MoviesPlay(), movie);
        }
    }
}

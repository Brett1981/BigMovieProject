using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using streamingApp.Models;
using Unosquare.FFmpegMediaElement;

namespace streamingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MediaEl.MouseDown += MediaEl_MouseDown;
            this.MediaEl.MouseWheel += MediaEl_MouseWheel;
            this.MediaEl.MediaEnded += (s, e) => { System.Diagnostics.Debug.WriteLine("MediaEnded Event Fired"); };
            this.MediaEl.MediaOpened += (s, e) => { System.Diagnostics.Debug.WriteLine("MediaOpened Event Fired"); };
            this.MediaEl.MediaFailed += (s, e) => { System.Diagnostics.Debug.WriteLine("MediaFailed Event Fired {0}", ((MediaErrorRoutedEventArgs)e).ErrorException); };
            this.MediaEl.MediaErrored += (s, e) => {
                var error = (e as MediaErrorRoutedEventArgs);
                var ex = error.ErrorException as MediaPlaybackException;
                if (ex == null)
                {
                    System.Diagnostics.Debug.WriteLine("MediaErrored Event Fired {0}", error.ErrorException);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("MediaErrored Event Fired {0} - {1}", ex.ErrorCode.ToString(), error.ErrorException);
                }
            };

            this.Closing += MainWindow_Closing;
        }
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.MediaEl.Close();
        }
        void MediaEl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var timeGap = 0.04M;
            var newPosition = Convert.ToDecimal(this.MediaEl.Position) + timeGap * (e.Delta > 0 ? MediaEl.SpeedRatio : -MediaEl.SpeedRatio);
            newPosition = SnapTo(0.04M, newPosition);
            MediaEl.Position = newPosition;
            if (!this.MediaEl.IsPlaying)
            {
                this.MediaEl.Play();
            }

        }

        void MediaEl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (this.MediaEl.IsPlaying)
                    this.MediaEl.Pause();
                else
                    this.MediaEl.Play();

                return;
            }

            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.MediaEl.Stop();
            }
        }
        private static decimal SnapTo(decimal gridLength, decimal number)
        {
            var result = number;
            var resultRemainder = result % gridLength;
            result = result - resultRemainder;
            return result;

        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://192.168.1.10:53851");
                var data = JsonConvert.DeserializeObject<MovieData>(await client.GetStringAsync("api/video/getmovie?id="+ SourceText.Text));
                if(data != null)
                {
                    this.Title = data.movie_name;
                    MediaEl.Source = new Uri("http://192.168.1.10:53851/api/video/play?id=" + data.movie_guid);
                }
            }
            catch
            {

            }
        }
    }
}

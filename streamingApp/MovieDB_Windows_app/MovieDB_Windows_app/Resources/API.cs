using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Media;
using System.Net.Http;
using MovieDB_Windows_app.Resources;
using System.Drawing;
using System.Diagnostics;

namespace MovieDB_Windows_app
{
    public class API
    {
        private static string conAddress
        {
            get
            {
                return "http://" + Properties.Settings.Default.APIip + ":" + Properties.Settings.Default.APIport;
            }
        }

        private static HttpClient client = new HttpClient();
        private List<Movie.Data> movieData = new List<Movie.Data>();
        //private jsonGenresClass genresData = new jsonGenresClass();
        
        public async Task<List<Movie.Data>> GetMovieData(bool force = false)
        {
            try
            {
                var responseMovie = await Communication.GetAllMovies();
                if(responseMovie != Properties.Settings.Default.MovieData || force)
                {
                    Properties.Settings.Default.MovieData = responseMovie;
                    Properties.Settings.Default.Save();
                    return JsonConvert.DeserializeObject<List<Movie.Data>>(responseMovie);
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<Movie.Data>>(Properties.Settings.Default.MovieData);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Movie.Data>();
            }
        }

        public async Task<List<User.Info>> GetUsersData()
        {
            try
            {

                var response = await Communication.GetAllUsers();
                if (response != String.Empty)
                {
                    return JsonConvert.DeserializeObject<List<User.Info>>(response);
                }
                else
                {

                    return new List<User.Info>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<User.Info>();
            }
        }

        public class Communication
        {
            private static StringContent CreateHttpContent<T>(object data )
            {
                return new StringContent(
                        JsonConvert.SerializeObject(data),
                        Encoding.UTF8,
                        "application/json");
            }

            //Get API
            public static async Task<string> GetAllMovies()
            {
                return await client.GetStringAsync(conAddress + "/api/video/allmovies");
            }

            public static async Task<string> GetAllUsers()
            {
                return await client.GetStringAsync(conAddress + "/api/user/getusers");
            }

            public static async Task<Movie.Data> GetMovie(string guid)
            {
                return JsonConvert.DeserializeObject<Movie.Data>(await client.GetStringAsync(conAddress + "/api/video/getmovie/" + guid));
            }

            /// <summary>
            /// Force refresh on API and returns new movie list
            /// </summary>
            /// <returns>List<Movie.Data></returns>
            public static async Task<List<Movie.Data>> Refresh()
            {
                return JsonConvert.DeserializeObject<List<Movie.Data>>(await client.GetStringAsync(conAddress + "/api/administration/refresh"));
            }
            
            /// <summary>
            /// Administration API
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            private static async Task<HttpResponseMessage> SetMovieStatus(StringContent content )
            {
                return await client.PostAsync(conAddress + "/api/administration/setmoviestatus", content);
            }

            private static async Task<HttpResponseMessage> Auth(StringContent content)
            {
                return await client.PostAsync(conAddress + "/api/administration/auth", content);
            } 

            //Enable/disable movie API
            public static async Task<HttpResponseMessage> ChangeMovieStatus(Movie.Data movie,User.Info user = null)
            {
                if(user == null)
                {
                    user = GlobalVar.GlobalCurrentUser;
                }
                return await SetMovieStatus(
                    CreateHttpContent<AuthMovieEdit>(
                        new AuthMovieEdit()
                        {
                            User = user,
                            Movie = movie
                        }
                    ));
            }

            public static async Task<HttpResponseMessage> Login(Auth.Login data)
            {
                return await Auth(CreateHttpContent<Auth.Login>(data));
            }

            /// <summary>
            /// Custom auth class for communication with API for movie edit
            /// </summary>
            public class AuthMovieEdit
            {
                public User.Info User { get; set; }
                public Movie.Data Movie { get; set; }
            }
        }

        public class Downloader
        {
            public static async Task<Image> ImageDownload(string url)
            {
                var response = await client.GetStreamAsync(url);
                return Image.FromStream(response);
            }
        }

        public class Auth
        {
            public class Login
            {
                public string username { get; set; }
                public string password { get; set; }
            }

            public class User
            {
                public int Id { get; set; }

                public string username { get; set; }
                public string password { get; set; }
                public string unique_id { get; set; }
                public string image_url { get; set; }
                public string display_name { get; set; }
                public Nullable<System.DateTime> profile_created { get; set; }
                public Nullable<System.DateTime> birthday { get; set; }
                public string email { get; set; }
            }

        }
  


    }

    
}

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

        public class Get
        {

        }


        public async Task<APIObjects.Data> InitalizeAppData()
        {
            try
            {
                return await Communication.InitApp(GlobalVar.GlobalAuthUser);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new APIObjects.Data();
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

            public static async Task<Movie.Data> GetMovie(string guid)
            {
                return JsonConvert.DeserializeObject<Movie.Data>(await client.GetStringAsync(conAddress + "/api/video/getmovie/" + guid));
            }

            
            
            /// <summary>
            /// Administration API
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            private static async Task<HttpResponseMessage> ChangeMovieStatus(StringContent content )
            {
                return await client.PostAsync($"{conAddress}/api/administration/changemoviestatus", content);
            }

            private static async Task<HttpResponseMessage> Auth(StringContent content)
            {
                return await client.PostAsync($"{conAddress}/api/administration/auth", content);
            }

            private static async Task<HttpResponseMessage> Refresh(StringContent content)
            {
                return await client.PostAsync($"{conAddress}/api/administration/refresh", content);
            }

            private static async Task<HttpResponseMessage> Init(StringContent content)
            {
                return await client.PostAsync($"{conAddress}/api/administration/init", content);
            }

            private static async Task<HttpResponseMessage> EditMovie(StringContent content)
            {
                return await client.PostAsync($"{conAddress}/api/administration/editmovie", content);
            }

            /// <summary>
            /// Enable or disable movie
            /// </summary>
            /// <param name="movie">Movie.Data</param>
            /// <param name="user">User.Info</param>
            /// <returns>HttpResponseMessage</returns>
            public static async Task<HttpResponseMessage> ChangeMovieStatus(Movie.Data movie,User.Info user = null)
            {
                if(user == null)
                {
                    user = GlobalVar.GlobalCurrentUserInfo;
                }
                return await ChangeMovieStatus(
                    CreateHttpContent<AuthMovieEdit>(
                        new AuthMovieEdit()
                        {
                            user = user,
                            movie = movie
                        }
                    ));
            }

            /// <summary>
            /// Login user and retrieve auth user parameters
            /// </summary>
            /// <param name="data">Auth.Login</param>
            /// <returns>HttpResponseMessage</returns>
            public static async Task<HttpResponseMessage> Login(Auth.Login data)
            {
                return await Auth(CreateHttpContent<Auth.Login>(data));
            }

            /// <summary>
            /// Refresh movies and retrieve new list
            /// </summary>
            /// <param name="data">Auth.User</param>
            /// <returns>List<Movie.Data></returns>
            public static async Task<List<Movie.Data>> RefreshData(Auth.User data)
            {
                 var response = await Refresh(
                        CreateHttpContent<Auth.User>(data)
                    );
                List<Movie.Data> m = new List<Movie.Data>();
                try
                {
                    m = JsonConvert.DeserializeObject<List<Movie.Data>>(await response.Content.ReadAsStringAsync());
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                return m;
            }

            /// <summary>
            /// Initialize the app and retrieve all data from API
            /// </summary>
            /// <param name="data">Auth.User</param>
            /// <returns>APIObjects.Data</returns>
            public static async Task<APIObjects.Data> InitApp(Auth.User data)
            {
                var response = await Init(CreateHttpContent<Auth.User>(data));
                APIObjects.Data init = new APIObjects.Data();
                try
                {
                    init = JsonConvert.DeserializeObject<APIObjects.Data>(await response.Content.ReadAsStringAsync());
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                return init;
            }

            /// <summary>
            /// Edited movie data send to API 
            /// </summary>
            /// <param name="user">Auth.User</param>
            /// <param name="data">Movie.Data</param>
            /// <returns>HttpResponseMessage</returns>
            public static async Task<HttpResponseMessage> EditMovie(Auth.User user, Movie.Data data) 
            {
                return await EditMovie(CreateHttpContent<APIObjects.Edit>(new APIObjects.Edit() { auth = user, movie = data }));
            }

            /// <summary>
            /// Custom auth class for communication with API for movie and users data
            /// </summary>
            ///
            public class AuthMovieEdit
            {
                public User.Info user { get; set; }
                public Movie.Data movie { get; set; }
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

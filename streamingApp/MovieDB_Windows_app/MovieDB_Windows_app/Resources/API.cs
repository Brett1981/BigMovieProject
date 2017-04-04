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
using System.Windows.Forms;

namespace MovieDB_Windows_app
{
    public class API
    {
        private static HttpClient client = new HttpClient() { Timeout = new TimeSpan(0,1,0)};

        private static string conAddress
        {
            get
            {
                return "http://" + Properties.Settings.Default.APIip + ":" + Properties.Settings.Default.APIport;
            }
        }

        

        public async Task<APIObjects.Data> InitAppData()
        {
            try
            {
                return await Communication.Get.InitApp(GlobalVar.GlobalAuthUser);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new APIObjects.Data();
            }
        }


        

        public class Communication
        {

            public class Get
            {
                //Get API
                public static async Task<string> AllMovies()
                {
                    try
                    {
                        return await client.GetStringAsync(conAddress + "/api/video/allmovies");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    
                }

                public static async Task<Movie.Data> MovieByGuid(string guid)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<Movie.Data>(await client.GetStringAsync(conAddress + "/api/video/getmovie/" + guid));
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    
                }

                /// <summary>
                /// Login user and retrieve auth user parameters
                /// </summary>
                /// <param name="data">Auth.Login</param>
                /// <returns>HttpResponseMessage</returns>
                public static async Task<HttpResponseMessage> Login(Auth.Login data)
                {
                    try
                    {
                        return await Administration.Auth(Create.HttpContent<Auth.Login>(data));
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    
                }

                /// <summary>
                /// Refresh movies and retrieve new list
                /// </summary>
                /// <param name="data">Auth.User</param>
                /// <returns>List<Movie.Data></returns>
                public static async Task<List<Movie.Data>> RefreshData(Auth.User data)
                {
                    var response = await Administration.Refresh(
                           Create.HttpContent<Auth.User>(data)
                       );
                    List<Movie.Data> m = new List<Movie.Data>();
                    try
                    {
                        m = JsonConvert.DeserializeObject<List<Movie.Data>>(await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    var response = await Administration.Init(Create.HttpContent<Auth.User>(data));
                    APIObjects.Data init = new APIObjects.Data();
                    try
                    {
                        init = JsonConvert.DeserializeObject<APIObjects.Data>(await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "An error occured",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    return init;
                }

            }

            public class Administration
            {
                /// <summary>
                /// Administration API
                /// </summary>
                /// <param name="content"></param>
                /// <returns>HttpResponseMessage</returns>
                internal static async Task<HttpResponseMessage> ChangeMovieStatus(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/changemoviestatus", content);
                }

                internal static async Task<HttpResponseMessage> Auth(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/auth", content);
                }

                internal static async Task<HttpResponseMessage> Refresh(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/refresh", content);
                }

                internal static async Task<HttpResponseMessage> Init(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/init", content);
                }

                internal static async Task<HttpResponseMessage> Edit(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/edit", content);
                }
            }
            
            public class Create
            {
                public static StringContent HttpContent<T>(object data)
                {
                    return new StringContent(
                            JsonConvert.SerializeObject(data),
                            Encoding.UTF8,
                            "application/json");
                }
            }
            
            public class Set
            {
                /// <summary>
                /// Enable or disable movie
                /// </summary>
                /// <param name="movie">Movie.Data</param>
                /// <param name="user">User.Info</param>
                /// <returns>HttpResponseMessage</returns>
                public static async Task<HttpResponseMessage> MovieStatus(Movie.Data movie, User.Info user = null)
                {
                    if (user == null)
                    {
                        user = GlobalVar.GlobalCurrentUserInfo;
                    }
                    return await Administration.ChangeMovieStatus(
                        Create.HttpContent<AuthMovieEdit>(
                            new AuthMovieEdit()
                            {
                                user = user,
                                movie = movie
                            }
                        ));
                }
            }

            public class Edit
            {
                /// <summary>
                /// Edited movie data send to API 
                /// </summary>
                /// <param name="user">Auth.User</param>
                /// <param name="data">Movie.Data</param>
                /// <returns>HttpResponseMessage</returns>
                public static async Task<HttpResponseMessage> Movie(Auth.User user, Movie.Data data)
                {
                    return await Administration.Edit(Create.HttpContent<APIObjects.Edit>(new APIObjects.Edit() { auth = user, movie = data }));
                }

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

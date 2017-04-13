using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Drawing;

namespace appDllv1
{
    public class API
    {
        private static HttpClient client;
        private static IPEndPoint _apiIp;

        private static string conAddress
        {
            get
            {
                if (APIip.Address != null && APIip.Port > 0)
                {
                    return "http://" + APIip.Address + ":" + APIip.Port;
                }
                else
                {
                    MessageBox.Show("No API address was set and cannot connect to external API...!");
                    return null;
                }

            }
        }

        public static IPAddress HostIp { get; set; }
        public static int HostPort { get; set; }
        public static IPEndPoint APIip
        {
            get
            {
                return _apiIp;
            }
            set
            {
                _apiIp = value;
            }
        }

        public API(IPAddress ip = null, int port = 0)
        {
            try
            {
                if(ip != null && port > 0)
                {
                    HostIp = ip; HostPort = port;
                    APIip = new IPEndPoint(ip,port);
                    client = new HttpClient() { Timeout = new TimeSpan(0, 1, 0) };
                }
                else
                {
                    MessageBox.Show("No API address or port was set, application cannot start. Contact the developer!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public API(string ip, string port)
        {
            try
            {
                if (ip != "" && port != "")
                {
                    HostIp = IPAddress.Parse(ip);
                    HostPort = Convert.ToInt32(port);
                    APIip = new IPEndPoint(HostIp, HostPort);
                    client = new HttpClient() { Timeout = new TimeSpan(0, 1, 0) };
                }
                else
                {
                    MessageBox.Show("No API address or port was set, application cannot start. Contact the developer!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        public static async Task<Objects.Communication.Data> InitAppData()
        {
            try
            {
                return await Communication.Get.InitApp(GlobalVar.GlobalAuthUser);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new Objects.Communication.Data();
            }
        }


        public class Communication
        {

            public class Get
            {
                //Get API

                /// <summary>
                /// Retrieve all movies from API by retrieving JSON string
                /// </summary>
                /// <returns>string</returns>
                public static async Task<string> AllMovies()
                {
                    try
                    {
                        return await client.GetStringAsync(conAddress + "/api/video/allmovies");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }

                }

                /// <summary>
                /// Retrieve a movie by sending a guid
                /// </summary>
                /// <param name="guid">string</param>
                /// <returns>Movie.Data</returns>
                public static async Task<Movie.Data> MovieByGuid(string guid)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<Movie.Data>(await client.GetStringAsync(conAddress + "/api/video/getmovie/" + guid));
                    }
                    catch (Exception ex)
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
                    catch (Exception ex)
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
                /// <returns>APIData</returns>
                public static async Task<Objects.Communication.Data> InitApp(Auth.User data)
                {
                    var response = await Administration.Init(Create.HttpContent<Auth.User>(data));
                    Objects.Communication.Data init = new Objects.Communication.Data();
                    try
                    {
                        init = JsonConvert.DeserializeObject<Objects.Communication.Data>(await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return init;
                }

                //Retrieve new user's list from api
                public static async Task<Objects.Communication.Users> AllUsers()
                {
                    return JsonConvert.DeserializeObject<Objects.Communication.Users>(
                        await (await Administration.GetAllUsers(
                        Create.HttpContent<Auth.User>(GlobalVar.GlobalAuthUser)
                            )
                        )
                        .Content.ReadAsStringAsync());
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
                    return await client.PostAsync($"{conAddress}/api/administration/ChangeMovieStatus", content);
                }

                internal static async Task<HttpResponseMessage> Auth(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/Auth", content);
                }

                internal static async Task<HttpResponseMessage> Refresh(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/Refresh", content);
                }

                internal static async Task<HttpResponseMessage> Init(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/Init", content);
                }

                internal static async Task<HttpResponseMessage> Edit(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/administration/Edit", content);
                }

                internal static async Task<HttpResponseMessage> NewUser(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/Administration/NewUser", content);
                }

                internal static async Task<HttpResponseMessage> RemoveUser(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/Administration/RemoveUser", content);
                }

                internal static async Task<HttpResponseMessage> GetAllUsers(StringContent content)
                {
                    return await client.PostAsync($"{conAddress}/api/Administration/GetAllUsers", content);
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

                public static async Task<HttpResponseMessage> User(User.Info user, User.Groups group)
                {
                    return await Administration.NewUser(
                        HttpContent<Objects.Communication.Edit>(
                            new Objects.Communication.Edit()
                            {
                                user = user,
                                groups = group,
                                auth = GlobalVar.GlobalAuthUser
                            }));
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
                public static async Task<HttpResponseMessage> Movie(Movie.Data data)
                {
                    return await Administration.Edit(Create.HttpContent<Objects.Communication.Edit>(new Objects.Communication.Edit() { auth = GlobalVar.GlobalAuthUser, movie = data }));
                }

            }

            public class Remove
            {
                public static async Task<HttpResponseMessage> User(User.Info user)
                {
                    return await Administration.RemoveUser(
                        Create.HttpContent<Objects.Communication.Edit>(
                            new Objects.Communication.Edit()
                            {
                                auth = GlobalVar.GlobalAuthUser,
                                user = user
                            }));
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
                return Image.FromStream(
                    await client.GetStreamAsync(url)
                    );
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
                public DateTime? profile_created { get; set; }
                public DateTime? birthday { get; set; }
                public string email { get; set; }
            }

        }

        public class Functions
        {
            public static long StringToLong(string data)
            {
                try
                {
                    IPAddress ip;

                    if (IPAddress.TryParse(data, out ip))
                    {
                        byte[] bytes = ip.GetAddressBytes();

                        return (long)
                            (
                            16777216 * (long)bytes[0] +
                            65536 * (long)bytes[1] +
                            256 * (long)bytes[2] +
                            (long)bytes[3]
                            )
                            ;
                    }
                    
                        
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return 0;
            }

            public static int StringToInt32(string data)
            {
                try
                {
                    return Convert.ToInt32(data);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return 0;
            }
        }

    }


}

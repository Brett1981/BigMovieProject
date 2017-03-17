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
        private List<Movie_Data> movieData = new List<Movie_Data>();
        //private jsonGenresClass genresData = new jsonGenresClass();
        
        public async Task<List<Movie_Data>> getMovieData(bool force = false)
        {
            try
            {
                var responseMovie = await Communication.GetAllMovies();
                if(responseMovie != Properties.Settings.Default.MovieData || force)
                {
                    Properties.Settings.Default.MovieData = responseMovie;
                    Properties.Settings.Default.Save();
                    return JsonConvert.DeserializeObject<List<Movie_Data>>(responseMovie);
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<Movie_Data>>(Properties.Settings.Default.MovieData);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Movie_Data>();
            }
        }

        public async Task<List<User_Info>> getUsersData()
        {
            try
            {

                var response = await Communication.GetAllUsers();
                if (response != String.Empty)
                {
                    return JsonConvert.DeserializeObject<List<User_Info>>(response);
                }
                else
                {

                    return new List<User_Info>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<User_Info>();
            }
        }

        public class Communication
        {
            //Get API
             
            public static async Task<string> GetAllMovies()
            {
                return await client.GetStringAsync(conAddress + "/api/video/allmovies");
            }

            public static async Task<string> GetAllUsers()
            {
                return await client.GetStringAsync(conAddress + "/api/user/getusers");
            }
            
            //Edit API
            public static async Task<string> UpdateMovieData()
            {
                return await client.GetStringAsync(conAddress + "/api/video/edit");
            }

            //Enable/disable movie API
            public static async Task<System.Net.HttpStatusCode> ChangeMovieStatus(string guid,string status)
            {
                List<values> v = new List<values>();
                v.Add(new values() { id = 0, name = Convert.ToBase64String(Encoding.ASCII.GetBytes(guid)) });
                v.Add(new values() { id = 1, name = status });
                string jsonData = string.Format("={0}", JsonConvert.SerializeObject(v));
                var content = new StringContent(
                        jsonData,
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.PostAsync(conAddress + "/api/video/setmoviestatus", content);
                Debug.WriteLine(response.StatusCode);
                return response.StatusCode;
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


    }

    
}

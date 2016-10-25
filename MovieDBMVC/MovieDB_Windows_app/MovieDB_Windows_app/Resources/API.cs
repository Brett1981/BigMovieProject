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

namespace MovieDB_Windows_app
{
    public class API
    {
        private  HttpClient client = new HttpClient();
        private jsonMovieClass movieData = new jsonMovieClass();
        private jsonGenresClass genresData = new jsonGenresClass();
        public async Task<Tuple<jsonGenresClass,jsonMovieClass>> RetrieveData()
        {
            try
            {

                var responseMovie = await client.GetStringAsync("http://213.143.88.177:8080/my-site/iss/json/data.json");
                if(responseMovie != Properties.Settings.Default.MovieData)
                {
                    Properties.Settings.Default.MovieData = responseMovie;
                    movieData = JsonConvert.DeserializeObject<jsonMovieClass>(responseMovie);
                }
                else
                {
                    
                    movieData = JsonConvert.DeserializeObject<jsonMovieClass>(Properties.Settings.Default.MovieData);
                }
                var responseGenre = await client.GetStringAsync("http://213.143.88.177:8080/my-site/iss/json/genres.json");
                if(responseGenre != Properties.Settings.Default.GenreData)
                {
                    Properties.Settings.Default.GenreData = responseGenre;
                    genresData = JsonConvert.DeserializeObject<jsonGenresClass>(responseGenre);
                }
                else
                {
                    genresData = JsonConvert.DeserializeObject<jsonGenresClass>(Properties.Settings.Default.GenreData);
                }
                return new Tuple<jsonGenresClass, jsonMovieClass>(genresData, movieData);
            }
            catch(Exception e)
            {
                throw;
            }
        }
        public async Task<Image> ImageDownload(string url)
        {
            var response = await client.GetStreamAsync(url);
            return Image.FromStream(response);
        }

        public string RetrieveGenres(List<int> data)
        {
            List<string> items = new List<string>();
            string tekst = "";
            for (int i=0;i <data.Count;i++)
            {
                if(i <= 2)
                {
                    var item = (GlobalVar.GlobalGenresData.genres.Where(x => x.id == data[i]).First().name);
                    if (i < 1)
                    {
                        tekst += item + "/";
                    }
                    else
                    {
                        tekst += item;
                    }
                }
                else
                {
                    break;
                }
                
            }
            return tekst;
            
        }
    }
}

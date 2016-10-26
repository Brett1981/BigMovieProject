using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.Resources
{
    public class MoviesAPI
    {
        private static string movieSearchURL = "http://api.themoviedb.org/3/search/movie?api_key=";
        //private static string GenreURL = "http://api.themoviedb.org/3/genre/movie/list?api_key=";
        public static int countAPICalls = 0;
        public static async Task<MovieInfo> getMovieInfo(string data)
        {
            try
            {
                var date = new DateTime(int.Parse("0001"), 1, 1);
                string Searcheditem = "";
                HttpClient client = new HttpClient();

                if (data.Contains(".")) { Searcheditem = data.Replace('.', ' '); }
                else { Searcheditem = data; }

                string[] dates = new string[] {
                    "2010","2011", "2012", "2013", "2014", "2015", "2016", "2017",
                    "(2010)","(2011)","(2012)","(2013)","(2014)","(2015)","(2016)","(2017)",
                    "2000","2001","2002","2003","2004","2005","2006","2007","2008","2009",
                    "(2000)","(2001)","(2002)","(2003)","(2004)","(2005)","(2006)","(2007)","(2008)","(2009)",
                    "1990","1991","1992","1993","1994","1995","1996","1997","1998","1999",
                    "(1990)","(1991)","(1992)","(1993)","(1994)","(1995)","(1996)","(1997)","(1998)","(1999)",
                    "1980","1981","1982","1983","1984","1985","1986","1987","1988","1989",
                    "(1980)","(1981)","(1982)","(1983)","(1984)","(1985)","(1986)","(1987)","(1988)","(1989)",
                };

                string[] specialStrings = new string[] { "SLOSubs", "COMPLETE" };
                foreach (var item in dates)
                {
                    if (Searcheditem.Contains(item))
                    {
                        Searcheditem = Searcheditem.Remove(Searcheditem.IndexOf(item));
                        Searcheditem = Searcheditem.TrimEnd();
                        var datum = item;
                        if (item.Contains("(") || item.Contains(")"))
                        {
                            datum = datum.Replace('(', ' '); datum = datum.Replace(')', ' ');
                            datum.Trim();
                            date = new DateTime(int.Parse(datum), 1, 1);
                        }
                        else { date = new DateTime(int.Parse(datum), 1, 1); }
                        break;
                    }
                }
                foreach (var item in specialStrings)
                {
                    if (Searcheditem.Contains(item))
                    {
                        var position = Searcheditem.IndexOf(item);
                        var editedInfo = Searcheditem.Remove(position);
                        Searcheditem = editedInfo.TrimEnd();
                    }
                }
                //Building api url with parameters - apikey + item to search for
                string url_API = "";
                var apikey = ConfigurationManager.AppSettings["APIkey"];

                if(apikey == null) { throw new Exception("API key was null or not defined! Check your Web.config to include value with key!"); }

                if (apikey != null && apikey.Length != 0)
                {
                    url_API = movieSearchURL + apikey + "&query=" + Searcheditem;
                    try
                    {
                        //GlobalVar.GlobalApiCall.Counter++;
                        if(countAPICalls > 30) { await Task.Delay(5000); countAPICalls = 0; }

                        var response = await client.GetStringAsync(url_API);
                        countAPICalls++;
                        var jsonData = JsonConvert.DeserializeObject<DataAPI>(response);

                        results apiResult = new results();

                        for (int i = 0; i < jsonData.results.Count; i++)
                        {
                            DateTime jsonDate = Convert.ToDateTime(jsonData.results[i].release_date);
                            if (date.Year != 1 && date != null)
                            {
                                if (jsonDate >= date || jsonData.results[i].title == Searcheditem)
                                {
                                    apiResult = new results()
                                    {
                                        id = jsonData.results[i].id,
                                        title = jsonData.results[i].title,
                                        genre_ids = jsonData.results[i].genre_ids,
                                        poster_path = jsonData.results[i].poster_path
                                    };
                                    break;
                                }
                            }
                        }
                        if (apiResult == null) { return new MovieInfo(); }

                        //GlobalVar.GlobalApiCall.Counter++;
                        if (countAPICalls > 30) { await Task.Delay(5000); countAPICalls = 0; }

                        var movieInfoJsonObject = JsonConvert.DeserializeObject<MovieInfo>(
                            await client.GetStringAsync(
                                "http://api.themoviedb.org/3/movie/" + apiResult.id + "?api_key=" + apikey)
                            );
                        countAPICalls++;


                        return movieInfoJsonObject;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString() + " | " + e.Message);
                    }
                }
                else
                {
                    return new MovieInfo();

                }
                return new MovieInfo();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new MovieInfo();
            }
        }

    }

    public class DataAPI
    {
         public List<results> results { get; set; }
    }
    public class results
    {
        public int id { get; set; }
        public string title { get; set; }
        public string release_date { get; set; }
        public List<int> genre_ids { get; set; }
        public string poster_path { get; set; }
    }

    public class DataGenre
    {
        public List<GenresList> genres { get; set; }
    }
    public class GenresList
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
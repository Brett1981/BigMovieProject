using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace api.Resources
{
    public class MoviesAPI
    {
        private static Uri movieSearchURL = new Uri("http://api.themoviedb.org/3/search/movie");
        //private static string GenreURL = "http://api.themoviedb.org/3/genre/movie/list?api_key=";
        public static int countAPICalls = 0;

        public static class Get
        {
            public static async Task<Movie_Info> Info(Match data, int id)
            {
                try
                {
                    DateTime date = new DateTime(int.Parse(data.Groups["year"].Value), 1, 1);
                    //var movie = data.Split('|');

                    //string Searcheditem = "";
                    HttpClient client = new HttpClient();

                    //Building api url with parameters - apikey + item to search for
                    Uri searchMovieAPI;
                    var apikey = Global.Global.GlobalServerSettings.Where(x => x.name == "APIKey").First().value;

                    if (apikey == null)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "API key was null or not defined! Check your Web.config to include value with key!",
                            api_type = "Exception -> MoviesAPI.Get.MovieInfo() -> No API Key",
                            api_datetime = DateTime.Now
                        });
                        throw new Exception("API key was null or not defined! Check your Web.config to include value with key!");
                    }

                    if (apikey != null && apikey.Length != 0)
                    {
                        searchMovieAPI = new Uri(movieSearchURL, "?api_key=" + apikey + "&query=" + data.Groups["title"].Value.Replace('.', ' '));
                        try
                        {
                            //GlobalVar.GlobalApiCall.Counter++;
                            if (countAPICalls > 30) { await Task.Delay(5000); countAPICalls = 0; }
                            HttpResponseMessage response;
                            try
                            {
                                response = await client.GetAsync(searchMovieAPI, HttpCompletionOption.ResponseContentRead);
                                if (response.IsSuccessStatusCode)
                                {
                                    countAPICalls++;

                                    var jsonData = JsonConvert.DeserializeObject<CustomClasses.Random.APIResults>(await response.Content.ReadAsStringAsync());
                                    if (jsonData == null) { return new Movie_Info(); }

                                    var apiResult = new CustomClasses.Random.results();

                                    for (int i = 0; i < jsonData.results.Count; i++)
                                    {
                                        DateTime jsonDate = Convert.ToDateTime(jsonData.results[i].release_date);
                                        if (date.Year != 1 && date != null)
                                        {
                                            if (jsonDate.Year == date.Year && jsonData.results[i].title.Contains(data.Groups["title"].Value.Replace('.', ' ')))
                                            {
                                                apiResult = new CustomClasses.Random.results()
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
                                    if (apiResult == null) { return new Movie_Info(); }

                                    //GlobalVar.GlobalApiCall.Counter++;
                                    if (countAPICalls > 30) { await Task.Delay(5000); countAPICalls = 0; }
                                    string info = "";
                                    if (apiResult.id != 0) { info = await client.GetStringAsync("http://api.themoviedb.org/3/movie/" + apiResult.id + "?api_key=" + apikey); }
                                    else { return new Movie_Info(); }
                                    countAPICalls++;
                                    return Create.MovieInfo(JsonConvert.DeserializeObject<CustomClasses.JSONToMovieInfo>(info), id);
                                }
                                else
                                {
                                    return new Movie_Info();
                                }
                            }
                            catch (Exception ex)
                            {
                                await History.Create(History.Type.API, new History_API()
                                {
                                    api_action = "Exception caught - first | Message " + ex.Message ,
                                    api_type = "Exception -> MoviesAPI.Get.MovieInfo()",
                                    api_datetime = DateTime.Now
                                });
                                return new Movie_Info();
                            }
                        }
                        catch (Exception e)
                        {
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_action = "Exception caught - second | Message " + e.Message,
                                api_type = "Exception -> MoviesAPI.Get.MovieInfo()",
                                api_datetime = DateTime.Now
                            });
                            
                        }
                    }
                    else
                    {
                        return new Movie_Info();

                    }
                    return new Movie_Info();
                }
                catch (Exception e)
                {
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_action = "Exception caught - third | Message " + e.Message,
                        api_type = "Exception -> MoviesAPI.Get.MovieInfo()",
                        api_datetime = DateTime.Now
                    });
                    return new Movie_Info();
                }
            }
        }

        public static class Create
        {
            public static Movie_Info MovieInfo(CustomClasses.JSONToMovieInfo data, int id)
            {
                return new Movie_Info()
                {
                    id = data.id,
                    adult = data.adult,
                    backdrop_path = data.backdrop_path,
                    budget = data.budget,
                    homepage = data.homepage,
                    imdb_id = data.imdb_id,
                    original_title = data.original_title,
                    overview = data.overview,
                    popularity = data.popularity,
                    poster_path = data.poster_path,
                    release_date = Convert.ToDateTime(data.release_date),
                    revenue = data.revenue,
                    status = data.status,
                    tagline = data.tagline,
                    title = data.title,
                    vote_average = data.vote_average,
                    vote_count = data.vote_count,
                    genres = Functions.ListToString(data.genres),
                    production_companies = Functions.ListToString(data.production_companies),
                    production_countries = Functions.ListToString(data.production_countries),
                    spoken_languages = Functions.ListToString(data.spoken_languages)
                };

            }
        }

        public static class Functions
        {
            public static string ListToString(List<CustomClasses.Random.values> data)
            {
                string x = "";
                for (int i = 0; i < data.Count; i++)
                {
                    x += data[i].id + ":" + data[i].name;
                    if (data.Count - 1 != i) { x += "|"; }
                }
                return x += "";
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using api.Controllers;
using api.Models;
using System.Data.Entity.Validation;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Threading;
using Microsoft.Owin;
using System.Net.Http;
using System.Net;
using System.Web.Http;

namespace api.Resources
{
    public class Database
    {

        #region Movie database methods
        public static MovieData movie;
        private static MDBSQLEntities db = new MDBSQLEntities();
        private static int createListCount = 0;
        private static int checkDbCount = 0;

        /// <summary>
        /// ProjectDebug is used for only creating a list of movies from database if false reads directories specified
        /// and checks db to add movie to it.
        /// </summary>
        private static bool projectDebug = false;

        private static int databaseMovieCount = 0;

        private static List<MovieData> _movies;
        
        /// <summary>
        /// Movie Database  public / private items
        /// </summary>
        public static List<MovieData> allMovies
        {
            get { if (_movies != null) { return _movies; } else return new List<MovieData>(); }
            set { _movies = value; }
        }
        private static DateTime time;
        private static DateTime CreateListTime;

        /// <summary>
        /// Edit movie from MVC view
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        internal static async Task<MovieData> Edit(string guid, System.Web.Mvc.FormCollection collection)
        {
            try
            {
                var movie = await Database.Get(guid);
                foreach (var item in collection.AllKeys)
                {
                    var i = collection.GetValue(item);
                    switch (item)
                    {
                        case "movie_name": {movie.movie_name = i.AttemptedValue; } break;
                        case "movie_ext": {movie.movie_ext = i.AttemptedValue; } break;
                        case "movie_folder": {movie.movie_folder = i.AttemptedValue; } break;
                        case "movie_guid": {movie.movie_guid = i.AttemptedValue; } break;
                        case "MovieInfo.adult": { if (i.AttemptedValue != "") { movie.MovieInfo.adult = Convert.ToBoolean(i.AttemptedValue); } } break;
                        case "MovieInfo.backdrop_path": {movie.MovieInfo.backdrop_path = i.AttemptedValue; } break;
                        case "MovieInfo.budget": {movie.MovieInfo.budget = i.AttemptedValue; } break;
                        case "MovieInfo.genres": {movie.MovieInfo.genres = i.AttemptedValue; } break;
                        case "MovieInfo.homepage": {movie.MovieInfo.homepage = i.AttemptedValue; } break;
                        case "MovieInfo.id": {movie.MovieInfo.id = Convert.ToInt32(i.AttemptedValue); } break;
                        case "MovieInfo.id_movie": {movie.MovieInfo.id_movie = movie.Id; } break;
                        case "MovieInfo.imdb_id": {movie.MovieInfo.imdb_id = i.AttemptedValue; } break;
                        case "MovieInfo.original_title": {movie.MovieInfo.original_title = i.AttemptedValue; } break;
                        case "MovieInfo.overview": {movie.MovieInfo.overview = i.AttemptedValue; } break;
                        case "MovieInfo.popularity": {movie.MovieInfo.popularity = i.AttemptedValue; } break;
                        case "MovieInfo.poster_path": {movie.MovieInfo.poster_path = i.AttemptedValue; } break;
                        case "MovieInfo.production_companies": {movie.MovieInfo.production_companies = i.AttemptedValue; } break;
                        case "MovieInfo.production_countries": {movie.MovieInfo.production_countries = i.AttemptedValue; } break;
                        case "MovieInfo.release_data": {movie.MovieInfo.release_date = Convert.ToDateTime(i.AttemptedValue); } break;
                        case "MovieInfo.revenue": {movie.MovieInfo.revenue = i.AttemptedValue; } break;
                        case "MovieInfo.spoken_language": {movie.MovieInfo.spoken_languages = i.AttemptedValue; } break;
                        case "MovieInfo.status": {movie.MovieInfo.status = i.AttemptedValue; } break;
                        case "MovieInfo.tagline": {movie.MovieInfo.tagline = i.AttemptedValue; } break;
                        case "MovieInfo.title": {movie.MovieInfo.title = i.AttemptedValue; } break;
                        case "MovieInfo.vote_average": {movie.MovieInfo.vote_average = i.AttemptedValue; } break;
                        case "MovieInfo.vote_count": {movie.MovieInfo.vote_count = i.AttemptedValue; } break;
                    }
                }
                await db.SaveChangesAsync();
                return movie;
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception Database.Edit --> " + e.Message);
                return new MovieData();
            }
        }
        
        /// <summary>
        /// Get movie data from local db
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static async Task<MovieData> Get(string guid)
        {
            try
            {
                var item = await db.MovieDatas.Where(x => x.movie_guid == guid).FirstOrDefaultAsync();
                if (item != null)
                {
                    return item;
                }
                return new MovieData();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new MovieData();
            }
        }

        /// <summary>
        /// Retrieve movie data from db using a guid string as reference and increase the view counter if movie found
        /// </summary>
        /// <param name="guid"></param>
        /// <returns name="MovieData"></returns>
        public static async Task<MovieData> GetMovie(string guid)
        {
            var movie = await db.MovieDatas.Where(x => x.movie_guid == guid).FirstOrDefaultAsync();
            if(movie != null) {
                if (movie.movie_views == null){ movie.movie_views = 1; }
                else { movie.movie_views += 1; }
                try
                {
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    Debug.WriteLine("Exception: GetMovie(string) --> " + e.Message);
                }
                finally
                {
                    movie = await db.MovieDatas.Where(x => x.movie_guid == guid).FirstOrDefaultAsync();
                }
                if(movie == null)
                {
                    return new MovieData();
                }
                return movie;
            }
            return new MovieData();
            
        }
        
        /// <summary>
        /// Retrieve movie from db and increase the view counter
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<MovieData> GetMovie(DatabaseUserModels data)
        {
            var user = await db.Users.Where(x => x.unique_id == data.user_id).FirstOrDefaultAsync();
            if (user != null)
            {
                Debug.WriteLine("User " + user.username + " with : guid '" + user.unique_id + "' is trying to view content :" + data.movie_id);
                var movie = await db.MovieDatas.Where(x => x.movie_guid == data.movie_id).FirstOrDefaultAsync();
                if (movie.movie_views == null) { movie.movie_views = 1; }
                else { movie.movie_views += 1; }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception: GetMovie (object) --> " + e.Message);
                }
                finally
                {
                    movie = await db.MovieDatas.Where(x => x.movie_guid == data.movie_id).FirstOrDefaultAsync();
                }
                if (movie == null)
                {
                    return new MovieData();
                }
                return movie;
            }
            return new MovieData();
        }
        
        /// <summary>
        /// Remove movie from db
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<int> Remove(MovieData item)
        {
            try
            {
                db.MovieDatas.Remove(item);
                return await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }
            
        }

        /// <summary>
        /// Force creating the movie list when an element is deleted from db or added
        /// </summary>
        public static void ForceMovieList()
        {
            allMovies = db.MovieDatas.Select(x => x).ToList();
            OrganizeListByDate();
        }

        /// <summary>
        /// Create movie list every x minutes or hours, depends how it is set up
        /// </summary>
        /// <returns></returns>
        public static async Task CreateList()
        {
            try
            {
                bool edited = false;
                while (true)
                {
                    if (createListCount == 0) { Debug.WriteLine("Movie list --> Creating new list."); allMovies = db.MovieDatas.Select(x => x).ToList(); createListCount++; edited = true; }
                    if(DateTime.Now > CreateListTime.AddMinutes(5)) { allMovies = await db.MovieDatas.Select(x => x).ToListAsync(); createListCount++; CreateListTime = DateTime.Now; edited = true; }
                    Debug.WriteLine("Movie list --> waiting ...");
                    if (edited || createListCount == 1){ OrganizeListByDate(); }
                    await Task.Delay(new TimeSpan(0, 1, 0));
                    edited = false;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static void OrganizeListByDate()
        {
            allMovies.Sort((x, y) => y.MovieInfo.release_date.Value.CompareTo(x.MovieInfo.release_date.Value));
        }

        /// <summary>
        /// Check directories if new movie was found that is not in the local db
        /// </summary>
        /// <returns></returns>
        public static async Task CheckDB()
        {
            try
            {
                
                while (true)
                {
                    if (!projectDebug && checkDbCount == 0)
                    {
                        if (checkDbCount == 0) {
                            
                            await databaseMovieCheck();
                            time = DateTime.Now;
                            Thread t2 = new Thread(async () => await Database.CreateList());
                            t2.Priority = ThreadPriority.Normal;
                            t2.Start();
                        }
                        if (checkDbCount != 0 && DateTime.Now > time.AddMinutes(10)) { await databaseMovieCheck(); time = DateTime.Now; }
                    }
                    else if(projectDebug)
                    {
                        Thread t2 = new Thread(async () => await Database.CreateList());
                        t2.Priority = ThreadPriority.Normal;
                        t2.Start();
                    }
                    Debug.WriteLine("Done checking / creating , waiting 1 minute/s.");
                    await Task.Delay(new TimeSpan(0, 1, 0));
                    checkDbCount++;
                }
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Method that checks directories
        /// </summary>
        /// <returns></returns>
        private static async Task<int> databaseMovieCheck(){
            try
            {
                Debug.WriteLine("Checking database for new entries!");
                List<MovieData> temp = new List<MovieData>();
                foreach (var childDirs in VideoController.movieDir)
                {
                    var dirs = Directory.GetDirectories(childDirs);
                    // var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";

                    var dbCount = db.MovieDatas.Count();
                    if (dbCount == 0) { databaseMovieCount = 0; } else { databaseMovieCount = db.MovieDatas.Count(); }

                    foreach (var d in dirs)
                    {
                        var files = Directory.GetFiles(d);
                        foreach (var f in files)
                        {
                            var item = new FileInfo(f);
                            int idx = item.Name.LastIndexOf('.');
                            var name = item.Name.Substring(0, idx);
                            var ext = item.Name.Substring(idx + 1);
                            if (ext == "mp4" || ext == "webm")
                            {
                                var m = await db.MovieDatas.Where(x => x.movie_name == name).FirstOrDefaultAsync();
                                if (m == null)
                                {
                                    //get movieinfo from api 
                                    if (MoviesAPI.countAPICalls > 30)
                                    {
                                        await Task.Delay(5000); MoviesAPI.countAPICalls = 0;
                                    }
                                    MovieInfo mInfo = await MoviesAPI.getMovieInfo(name, databaseMovieCount);//editMovieInfo
                                    if(mInfo.id != null)
                                    {
                                        if (mInfo.tagline.Length > 128) { mInfo.tagline = mInfo.tagline.Substring(0, 127); }
                                        MovieData mData = new MovieData()
                                        {
                                            movie_name = name,
                                            movie_ext = ext,
                                            movie_guid = CreateGuid(name).ToString(),
                                            movie_folder = item.Directory.Name.ToString(),
                                            MovieInfo = mInfo
                                        };
                                        try
                                        {
                                            db.MovieDatas.Add(mData);
                                            await db.SaveChangesAsync();
                                            databaseMovieCount++;
                                            //temp.Add(mData);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(db.Database.Log);
                                            Debug.WriteLine("Exception : Inserting movie to Database --> " + ex.Message);
                                        }
                                        finally
                                        {

                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Movie " + name + " was not added as there was a problem!");
                                    }
                                }
                            }

                        }

                    }
                }
                int retValue = 0;
                if (temp.Count != 0)
                {
                    try
                    {
                        var dbSaveInt = await db.SaveChangesAsync();
                        /*var writer = new XmlSerializer(typeof(List<MovieData>));
                        if (File.Exists(path))
                        {
                            FileStream file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                            writer.Serialize(file, temp);
                            file.Close();
                        }
                        else
                        {
                            var file = File.Create(path);
                            writer.Serialize(file, temp);
                            file.Close();
                        }*/
                        //retValue = dbSaveInt;
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Debug.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                        retValue = -99999;
                    }
                }
                return retValue;
                
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception --> {0} -- {1}", ex.Message, ex.InnerException.InnerException);
                return ex.HResult;

            }
        }

        /// <summary>
        /// GUID method that is called when a new movie is about to be added to local db. 
        /// This GUID is ussed for references when a movie is searched for
        /// </summary>
        /// <param name="movieName"></param>
        /// <returns></returns>
        public static Guid CreateGuid(string movieName)
        {
            using (MD5 md5 = MD5.Create())
            {
                return new Guid(md5.ComputeHash(Encoding.Default.GetBytes(movieName)));
            }
        }

        /// <summary>
        /// Retrieve movies from db where genre is the same as searched
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        public static List<MovieData> GetByGenre(string genre)
        {
            List<MovieData> searchedMovies = new List<MovieData>();
            foreach(var item in allMovies)
            {
                if(item.MovieInfo.genres != "")
                {
                    var g = item.MovieInfo.genres;
                    if (g.Contains("|"))
                    {
                        var h = g.Split('|');
                        foreach(var gnr in h)
                        {
                            var y = gnr.Split(':');
                            if(y[1].ToLower() == genre.ToLower())
                            {
                                searchedMovies.Add(item);
                                break;
                            }
                        }

                    }
                    else
                    {
                        if (g.Contains(":"))
                        {
                            var h = g.Split(':');
                            if(h[1].ToLower() == genre.ToLower())
                            {
                                searchedMovies.Add(item);
                            }
                        }
                    }
                }
            }
            return searchedMovies;
        }

        #endregion

        #region User database methods

        /// <summary>
        /// Changes user profile picture when user prompts to change it
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<string>ChangeUserPicture(CustomUserModel user)
        {

            var uData = await db.Users.Where(x => x.unique_id == user.unique_id).FirstOrDefaultAsync();
            if (uData != null)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    uData.profile_image = await client.GetByteArrayAsync(user.image_url);
                    db.Entry(uData).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return "OK";
                }
                catch (HttpException ex)
                {
                    Debug.WriteLine("HttpException --> {0} -- {1}", ex.Message, ex.InnerException.InnerException);
                    return "Exception";
                }
            }
            return "NotAuthorized";
        }
        #endregion
        
    }
}
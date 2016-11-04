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

        private static bool projectDebug = true;

        private static int databaseMovieCount = 0;

        private static MovieData[] _movies;
        
        /// <summary>
        /// Movie Database methods
        /// </summary>
        public static MovieData[] allMovies
        {
            get { if (_movies != null) { return _movies; } else return new MovieData[] { }; }
            set { _movies = value; }
        }
        private static DateTime time;
        private static DateTime CreateListTime;

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
                        case "MovieInfo.release_data": {movie.MovieInfo.release_date = i.AttemptedValue; } break;
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
                return new MovieData();
            }
        }

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

        public static async Task<MovieData> GetMovie(AuthorizationUserModels data)
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

        public static void ForceCreateList()
        {
            allMovies = db.MovieDatas.Select(x => x).ToArray();
        }
        public static async Task CreateList()
        {
            try
            {
                while (true)
                {
                    if (createListCount == 0) { Debug.WriteLine("Movie list --> Creating new list."); allMovies = db.MovieDatas.Select(x => x).ToArray(); createListCount++;  }
                    if(DateTime.Now > CreateListTime.AddMinutes(5)) { allMovies = await db.MovieDatas.Select(x => x).ToArrayAsync(); createListCount++; CreateListTime = DateTime.Now; }
                    Debug.WriteLine("Movie list --> waiting ...");
                    await Task.Delay(new TimeSpan(0, 1, 0));
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

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
                                    MovieInfo mInfo = await MoviesAPI.getMovieInfo(name, databaseMovieCount);//editMovieInfo(

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
                                        databaseMovieCount++;
                                        temp.Add(mData);
                                    }
                                    catch(Exception ex)
                                    {
                                        Debug.WriteLine("Exception : Inserting movie to Database --> " + ex.Message);
                                    }
                                    finally
                                    {

                                    }
                                    //var dbSaveInt = await db.SaveChangesAsync();
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
        public static Guid CreateGuid(string movieName)
        {
            using (MD5 md5 = MD5.Create())
            {
                return new Guid(md5.ComputeHash(Encoding.Default.GetBytes(movieName)));
            }
        }

        private static MovieInfo editMovieInfo(MovieInfo data, int id)
        {
            data.id_movie = id;
            return data;
        }

        #endregion

        #region User database methods
        public static async Task<User> GetUser(string username, string password)
        {
            return await db.Users.Where(x => (x.username == username && x.password == password)).FirstOrDefaultAsync();
        }

        public static async Task<User> CreateUser(UserLibrary data)
        {
            User u = new User();
            if(data.username != "" && data.password != ""){
                u.username = data.username;
                u.password = data.password;
                if (data.image_url != null)
                {
                    try
                    {
                        HttpClient client = new HttpClient();
                        u.profile_image = await client.GetByteArrayAsync(data.image_url);
                    }
                    catch (HttpException ex)
                    {
                        Debug.WriteLine("HttpException --> {0} -- {1}", ex.Message, ex.InnerException.InnerException);
                    }
                }
            }
            return u;
        }

        public static async Task<string>ChangeUserPicture(UserLibrary user)
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
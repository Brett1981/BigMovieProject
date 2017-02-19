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
using System.Drawing;
using System.Text.RegularExpressions;

namespace api.Resources
{
    public class Database
    {

        #region Movie database methods
        public static Movie_Data movie;
        private static MDBSQLEntities db = new MDBSQLEntities();
        private static int createListCount = 0;
        private static int checkDbCount = 0;

        /// <summary>
        /// ProjectDebug is used for only creating a list of movies from database if false reads directories specified
        /// and checks db to add movie to it.
        /// </summary>
        private static bool projectDebug = false;

        private static int databaseMovieCount = 0;

        private static List<Movie_Data> _movies;
        
        /// <summary>
        /// Movie Database  public / private items
        /// </summary>
        public static List<Movie_Data> AllMovies
        {
            get
            {
                if (_movies != null) { return _movies; }
                else
                {
                    try { ForceMovieList(); }
                    catch (Exception e) { Debug.WriteLine("Exc -> ForceMovieList | " + e.Message); }
                    finally
                    {
                        if (_movies == null || _movies.Count == 0) { _movies = new List<Movie_Data>(); }
                    }
                    return _movies;
                }
            }
            set { _movies = value; }
        }
        private static DateTime time;
        private static DateTime CreateListTime;

        /// <summary>
        /// Edit movie from MVC view
        /// </summary>
        /// <param name="guid">string</param>
        /// <param name="collection">System.Web.Mvc.FormCollection</param>
        /// <returns>MovieData</returns>
        internal static async Task<Movie_Data> Edit(string guid, System.Web.Mvc.FormCollection collection)
        {
            try
            {
                var movie = await Database.Get(guid);
                foreach (var item in collection.AllKeys)
                {
                    var i = collection.GetValue(item);
                    switch (item)
                    {
                        case "movie_name": {movie.name = i.AttemptedValue; } break;
                        case "movie_ext": {movie.ext = i.AttemptedValue; } break;
                        case "movie_folder": {movie.folder = i.AttemptedValue; } break;
                        case "movie_guid": {movie.guid = i.AttemptedValue; } break;
                        case "movie_dir": { movie.dir = i.AttemptedValue; }break;
                        case "MovieInfo.adult": { if (i.AttemptedValue != "") { movie.Movie_Info.adult = Convert.ToBoolean(i.AttemptedValue); } } break;
                        case "MovieInfo.backdrop_path": { movie.Movie_Info.backdrop_path = i.AttemptedValue; } break;
                        case "MovieInfo.budget": { movie.Movie_Info.budget = i.AttemptedValue; } break;
                        case "MovieInfo.genres": { movie.Movie_Info.genres = i.AttemptedValue; } break;
                        case "MovieInfo.homepage": { movie.Movie_Info.homepage = i.AttemptedValue; } break;
                        case "MovieInfo.id": { movie.Movie_Info.id = Convert.ToInt32(i.AttemptedValue); } break;
                        case "MovieInfo.id_movie": { movie.Movie_Info.id_movie = movie.Id; } break;
                        case "MovieInfo.imdb_id": { movie.Movie_Info.imdb_id = i.AttemptedValue; } break;
                        case "MovieInfo.original_title": { movie.Movie_Info.original_title = i.AttemptedValue; } break;
                        case "MovieInfo.overview": { movie.Movie_Info.overview = i.AttemptedValue; } break;
                        case "MovieInfo.popularity": { movie.Movie_Info.popularity = i.AttemptedValue; } break;
                        case "MovieInfo.poster_path": { movie.Movie_Info.poster_path = i.AttemptedValue; } break;
                        case "MovieInfo.production_companies": { movie.Movie_Info.production_companies = i.AttemptedValue; } break;
                        case "MovieInfo.production_countries": { movie.Movie_Info.production_countries = i.AttemptedValue; } break;
                        case "MovieInfo.release_data": { movie.Movie_Info.release_date = Convert.ToDateTime(i.AttemptedValue); } break;
                        case "MovieInfo.revenue": { movie.Movie_Info.revenue = i.AttemptedValue; } break;
                        case "MovieInfo.spoken_language": { movie.Movie_Info.spoken_languages = i.AttemptedValue; } break;
                        case "MovieInfo.status": { movie.Movie_Info.status = i.AttemptedValue; } break;
                        case "MovieInfo.tagline": { movie.Movie_Info.tagline = i.AttemptedValue; } break;
                        case "MovieInfo.title": { movie.Movie_Info.title = i.AttemptedValue; } break;
                        case "MovieInfo.vote_average": { movie.Movie_Info.vote_average = i.AttemptedValue; } break;
                        case "MovieInfo.vote_count": { movie.Movie_Info.vote_count = i.AttemptedValue; } break;
                    }
                }
                await db.SaveChangesAsync();
                return movie;
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception Database.Edit --> " + e.Message);
                return new Movie_Data();
            }
        }
        
        /// <summary>
        /// Get movie data from local db
        /// </summary>
        /// <param name="guid">string</param>
        /// <returns>MovieData</returns>
        public static async Task<Movie_Data> Get(string guid)
        {
            try
            {
                var item = await db.Movie_Data.Where(x => x.guid == guid).FirstOrDefaultAsync();
                if (item != null)
                    return item;
                return new Movie_Data();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new Movie_Data();
            }
        }

        /// <summary>
        /// Retrieve movie data from db using a guid string as reference and increase the view counter if movie found
        /// </summary>
        /// <param name="guid">string</param>
        /// <returns name="MovieData">MovieData</returns>
        public static async Task<Movie_Data> GetMovie(string guid)
        {
            var movie = await db.Movie_Data.Where(x => x.guid == guid).FirstOrDefaultAsync();
            if(movie != null) {
                if (movie.views == null){ movie.views = 1; }
                else { movie.views += 1; }
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
                    movie = await db.Movie_Data.Where(x => x.guid == guid).FirstOrDefaultAsync();
                }
                if(movie == null)
                {
                    return new Movie_Data();
                }
                return movie;
            }
            return new Movie_Data();
            
        }

        /// <summary>
        /// Retrieve movie from db and increase the view counter
        /// </summary>
        /// <param name="data">DatabaseUserModels</param>
        /// <returns>MovieData</returns>
        public static async Task<Movie_Data> GetMovie(DatabaseUserModels data)
        {
            var user = await db.User_Info.Where(x => x.unique_id == data.user_id).FirstOrDefaultAsync();
            if (user != null)
            {
                Debug.WriteLine("User " + user.username + " with : guid '" + user.unique_id + "' is trying to view content :" + data.movie_id);
                var movie = await db.Movie_Data.Where(x => x.guid == data.movie_id).FirstOrDefaultAsync();
                if (movie.views == null) { movie.views = 1; }
                else { movie.views += 1; }
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
                    movie = await db.Movie_Data.Where(x => x.guid == data.movie_id).FirstOrDefaultAsync();
                }
                if (movie == null)
                {
                    return new Movie_Data();
                }
                return movie;
            }
            else
            {
                if(data.user_id.Length < 20)
                {
                    //it is a guest
                    var movie = await db.Movie_Data.Where(x => x.guid == data.movie_id).FirstOrDefaultAsync();
                    if(movie != null)
                    {
                        if (movie.views == null) { movie.views = 1; }
                        else { movie.views++; }
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
                            movie = await db.Movie_Data.Where(x => x.guid == data.movie_id).FirstOrDefaultAsync();
                        }
                        return movie;
                    }
                    return new Movie_Data(); 
                }
            }
            return new Movie_Data();
        }
        
        /// <summary>
        /// Remove movie from db
        /// </summary>
        /// <param name="item">MovieData</param>
        /// <returns>int</returns>
        public static async Task<int> Remove(Movie_Data item)
        {
            try
            {
                db.Movie_Data.Remove(item);
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
            AllMovies = db.Movie_Data.Select(x => x).ToList();
            OrganizeListByDate();
        }

        /// <summary>
        /// Create movie list every x minutes or hours, depends how it is set up
        /// </summary>
        /// <returns>null</returns>
        public static async Task CreateList()
        {
            try
            {
                bool edited = false;
                while (true)
                {
                    if (createListCount == 0) { Debug.WriteLine("Movie list --> Creating new list."); AllMovies = await db.Movie_Data.Select(x => x).ToListAsync(); createListCount++; edited = true; }
                    if(DateTime.Now > CreateListTime.AddMinutes(5)) { AllMovies = await db.Movie_Data.Select(x => x).ToListAsync(); createListCount++; CreateListTime = DateTime.Now; edited = true; }
                    Debug.WriteLine("Movie list --> waiting ...");
                    if (edited){ OrganizeListByDate(); }
                    await Task.Delay(new TimeSpan(0, 1, 0));
                    edited = false;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Organize list of movies by their release date
        /// </summary>
        private static void OrganizeListByDate()
        {
            var d = AllMovies;
            if(AllMovies.Count > 0)
            {
                AllMovies.Sort((x, y) => y.Movie_Info.release_date.Value.CompareTo(x.Movie_Info.release_date.Value));
            }
            
        }

        /// <summary>
        /// Check directories if new movie was found that is not in the local db
        /// </summary>
        /// <returns>null</returns>
        public static async Task CheckDB()
        {
            try
            {
                while (true)
                {
                    if (!projectDebug && checkDbCount == 0)
                    {
                        await databaseMovieCheck();
                        await insertMoviesToDb();
                        time = DateTime.Now;
                        Thread t2 = new Thread(async () => await Database.CreateList());
                        t2.Priority = ThreadPriority.Normal;
                        t2.Start();
                    }
                    else if (!projectDebug && checkDbCount > 0) //&& DateTime.Now > time.AddMinutes(10)
                    {
                        await databaseMovieCheck();
                        await insertMoviesToDb();
                        await databaseRemoveDeletedFolderFromDb();
                        time = DateTime.Now;
                    }
                    else if(projectDebug)
                    {
                        Thread t2 = new Thread(async () => await Database.CreateList());
                        t2.Priority = ThreadPriority.Normal;
                        t2.Start();
                    }
                    
                    Debug.WriteLine("Done checking / creating , waiting 1 minute/s.");
                    await Task.Delay(new TimeSpan(0, 30, 0));
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
        /// <returns>null</returns>
        private static async Task databaseMovieCheck(){
            try
            {
                Debug.WriteLine("Checking database for new entries!");
                movieListToAdd = new List<Tuple<Movie_Data, Match>>();
                foreach (var childDirs in VideoController.movieDir)
                {
                    var dirs = Directory.GetDirectories(childDirs);
                    //var dbCount = db.MovieDatas.Count();
                    //if (dbCount == 0) { databaseMovieCount = 0; } else { databaseMovieCount = db.MovieDatas.Count(); }

                    foreach (var d in dirs)
                    {
                        //DirectoryInfo v = new DirectoryInfo(d);
                        var files = Directory.GetFiles(d);
                        if (files.Any(s => s.Contains(".mp4") || s.Contains(".webm")))
                        {
                            var mName = files.Where(s => s.Contains(".mp4") || s.Contains(".webm")).ToArray();
                            
                            if(mName.Count() > 0 )
                            {
                                //create a string from folder name so that it can retrieve movie information from external API
                                var item = new FileInfo(mName[0]);
                                int idx = item.Name.LastIndexOf('.');
                                var name = item.Name.Substring(0, idx);
                                //regex to get movie info
                                var movie = GetMovieName(item.Name);
                                var movieName = movie.Groups["title"].Value.Replace('.', ' ');
                                var ext = movie.Groups["extension"].Value;
                                if (ext == "mp4" || ext == "webm")
                                {
                                    //check if movie exists in current server db
                                    var m = await db.Movie_Data.Where(x => x.name == name).FirstOrDefaultAsync();
                                    if (m == null)
                                    {
                                        //creating a list of movies to be searched on the selected API 
                                        movieListToAdd.Add(new Tuple<Movie_Data, Match>(
                                            new Movie_Data() //movie data to be written to db
                                            {
                                                name = name,
                                                ext = ext,
                                                guid = CreateGuid(name).ToString(),
                                                folder = item.Directory.Name.ToString(),
                                                dir = item.Directory.FullName,
                                                added = DateTime.Now,
                                                enabled = true,
                                                views = 0
                                            },
                                            movie //movie regex
                                        ));
                                    }
                                }
                            }
                        }
                    }
                }
                if(movieListToAdd.Count > 0)
                {
                    Debug.WriteLine("Movie list contains one or more objects to be added to DB!");
                    Debug.WriteLine("Adding movies to local DB ...");
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception --> {0} -- {1}", ex.Message);
            }
        }
        private static List<Tuple<Movie_Data, Match>> movieListToAdd;

        private static async Task insertMoviesToDb()
        {
            Debug.WriteLine("Starting to add movies to local DB ...");
            var list = new List<int>();
            //item1 = MovieData, item2 = Match
            foreach (var item in movieListToAdd)
            {
                try
                {
                    //get movieinfo from api 
                    if (MoviesAPI.countAPICalls > 30) { await Task.Delay(5000); MoviesAPI.countAPICalls = 0; }
                    //editMovieInfo, movie[0] is array from method GetMovieName
                    Movie_Info mInfo = await MoviesAPI.getMovieInfo(item.Item2, databaseMovieCount);

                    if (mInfo.id != null)
                    {
                        if (mInfo.tagline.Length > 128) { mInfo.tagline = mInfo.tagline.Substring(0, 127); }
                        item.Item1.Movie_Info = mInfo;
                        try
                        {
                            db.Movie_Data.Add(item.Item1);
                            await db.SaveChangesAsync();
                            //databaseMovieCount++;
                            //temp.Add(mData);
                            var movie = db.Movie_Data.Where(x => x.name == item.Item1.name).First();
                            Debug.WriteLine("Movie " + movie.Movie_Info.title + " was added to the database as id " + movie.Id + "!");
                            list.Add(movie.Id);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(db.Database.Log);
                            Debug.WriteLine("Exception : Inserting movie to Database --> " + ex.Message);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Movie " + item.Item2.Groups["title"].ToString() + " was not added as there was a problem!");
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("Error -> An error occured : " + ex.Message);
                }
                
            }
            Debug.WriteLine("End of import of movies.");
            if (list.Count == movieListToAdd.Count)
            {
                Debug.WriteLine("Info -> All movies added");
                Debug.WriteLine("Movies added " + list.Count);
            }
            else if (list.Count < movieListToAdd.Count)
            {
                Debug.WriteLine("Error -> Less movies added than found on local storage!");
                Debug.WriteLine("Movies added " + list.Count);
            }
            else
            {
                Debug.WriteLine("Error -> Something went wrong with importing data to DB!");
                Debug.WriteLine("Movies added " + list.Count);
            }
        }
        
        /// <summary>
        /// Remove movie database entries if directory does not exist
        /// </summary>
        /// <returns>string</returns>
        private static async Task<string> databaseRemoveDeletedFolderFromDb()
        {
            try
            {
                List<Movie_Data> toDelete = new List<Movie_Data>();
                if(AllMovies.Count > 0)
                {
                    foreach(var item in AllMovies)
                    {
                        if(!Directory.Exists(item.dir)){ toDelete.Add(item); }
                    }
                    if(toDelete.Count > 0)
                    {
                        db.Movie_Data.RemoveRange(toDelete); //removing entries in database
                        await db.SaveChangesAsync();
                        return "Ok";
                    }
                }
                return "No entries in database ...";
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception --> {0} -- {1}", ex.Message, ex.InnerException.InnerException);
                return ex.Message;
            }
        }

        /// <summary>
        /// Retrieve a movie name from its folder and return an array of strings
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>Match</returns>
        private static Match GetMovieName(string value)
        {
            Match r = null;
            string pattern = @"(?'title'.*)(?=\.[\d]{4})\.(?'year'[\d]{4})\.(?'pixelsize'[\d]{4}p)\.(?'format'[\w]+)\.(?'formatsize'[\w]+)-\[(?'group'.*)\]\.(?'extension'[\w]+)$";
            string pattern2 = @"(?'title'.*)\.(?'year'[^\.]+)\.(?'pixelsize'[^\.]+)\.(?'format'[^\.]+)\.(?'formatsize'[^\.]+)\.(?'filename'[^\.]+)\.(?'extension'[^\.]+)";

            if (value.Contains("[")) r = Regex.Match(value, pattern);
            else r = Regex.Match(value, pattern2); 
            return r;
        }

        /// <summary>
        /// GUID method that is called when a new movie is about to be added to local db. 
        /// This GUID is ussed for references when a movie is searched for
        /// </summary>
        /// <param name="movieName">string</param>
        /// <returns>Guid</returns>
        public static Guid CreateGuid(string value)
        {
            using (MD5 md5 = MD5.Create())
            {
                return new Guid(md5.ComputeHash(Encoding.Default.GetBytes(value)));
            }
        }

        /// <summary>
        /// Retrieve movies from db where genre is the same as searched
        /// </summary>
        /// <param name="genre">string</param>
        /// <returns>MovieData</returns>
        public static List<Movie_Data> GetByGenre(string genre)
        {
            List<Movie_Data> searchedMovies = new List<Movie_Data>();
            foreach(var item in AllMovies)
            {
                if(item.Movie_Info.genres != "")
                {
                    var g = item.Movie_Info.genres;
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

        /// <summary>
        /// Returns last 10 movies added to database
        /// </summary>
        /// <returns>MovieData</returns>
        public static async Task<List<Movie_Data>> GetLast10()
        {
            return await db.Movie_Data.OrderByDescending(x => x.added).Take(10).ToListAsync();
        }
        /// <summary>
        /// Returns the top 10 most played movies 
        /// </summary>
        /// <returns>MovieData</returns>
        public static async Task<List< Movie_Data>> GetTop10()
        {
            return await db.Movie_Data.Where(x => x.views > 0).Take(5).ToListAsync();
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
            var uData = await db.User_Info.Where(x => x.unique_id == user.unique_id).FirstOrDefaultAsync();
            if (uData != null)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    var img = Image.FromStream(await client.GetStreamAsync(user.image_url));

                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\users\", uData.unique_id);

                    if(!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                    var i = Path.Combine(path, "profile.jpg");
                    if (!File.Exists(i)){
                        img.Save(i);
                    }
                    else{
                        
                        File.Delete(i);
                        img.Save(i);
                    }
                    img.Dispose();
                    uData.profile_image = uData.unique_id+"/profile.jpg";
                    db.Entry(uData).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return "OK";
                }
                catch (HttpException ex)
                {
                    Debug.WriteLine("HttpException at ChangeUserPicture --> {0}", ex.Message);
                    return "Exception";
                }
            }
            return "NotAuthorized";
        }

        public static byte[] GetUserImage(string path)
        {
            var imgdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"images\users\", path);
            if (File.Exists(imgdir))
            {
                try
                {
                    ImageConverter converter = new ImageConverter();
                    var i = Image.FromFile(imgdir);
                    var b =  (byte[])converter.ConvertTo(i, typeof(byte[]));
                    i.Dispose();
                    return b;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("Exception at GetUserImage --> " + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        } 
        /// <summary>
        /// Retrieve the user information from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<User_Info> FindUser(string id)
        {
            var u = await db.User_Info.Where(x => x.unique_id == id).FirstOrDefaultAsync();
            if(u == null) { return null; }
            return u;
        }
        #endregion

        #region SessionPlay Methods
        /// <summary>
        /// Creating a session guid that allows users to play content
        /// </summary>
        /// <param name="data">DatabaseUserModels</param>
        /// <returns>string</returns>
        public static async Task<Object> CreateSession<T>(DatabaseUserModels data)
        {
            bool isGuest = false;
            Guid g1;
            if (data.user_id.Length > 30)
            {
                var s0 = await db.Session_Play.Where(x => x.movie_id == data.movie_id && x.user_id == data.user_id).FirstOrDefaultAsync();
                if (s0 != null)
                {
                    if (s0.movie_id != "" && s0.session_id != "" && s0.session_date < DateTime.Now && s0.user_id != "")
                    {
                        db.Session_Play.Remove(s0);
                        await db.SaveChangesAsync();
                    }
                }
                isGuest = false;
                g1 = new Guid(data.user_id);
                
            }
            else
            {
                isGuest = true;
                MD5 md5Hasher = MD5.Create();
                g1 = new Guid(md5Hasher.ComputeHash(Encoding.Default.GetBytes(DateTime.Now.ToString())));
            }
            Guid g2 = new Guid(data.movie_id);

            var result = g1.GetHashCode() ^ g2.GetHashCode() + DateTime.Now.GetHashCode();
            List<CustomClasses.values> v = new List<CustomClasses.values>();
            if (isGuest)
            {
                var s = new Session_Guest()
                {
                    session_date = DateTime.Now,
                    session_id = CreateGuid(result.ToString()).ToString(),
                    movie_id = data.movie_id,
                };
                db.Session_Guest.Add(s);
                await db.SaveChangesAsync();
                return s;
            }
            else
            {
                var s = new Session_Play()
                {
                    session_date = DateTime.Now,
                    session_id = CreateGuid(result.ToString()).ToString(),
                    movie_id = data.movie_id,
                    user_id = data.user_id
                };
                db.Session_Play.Add(s);
                await db.SaveChangesAsync();
                return s;
            }
            

        }

        /// <summary>
        /// Retrieve a session from database 
        /// </summary>
        /// <param name="data">DatabaseUserModels</param>
        /// <returns>SessionPlay</returns>
        public static async Task<Session_Play> GetSession(DatabaseUserModels data)
        {
           return await db.Session_Play.Where(x => x.movie_id == data.movie_id && x.user_id == data.user_id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieve data from db by providing session guid that was generated by requesting a movie
        /// </summary>
        /// <param name="session">string</param>
        /// <returns>SessionGuest</returns>
        public static async Task<Session_Guest> GetBySession(string session)
        {
            return await db.Session_Guest.Where(x => x.session_id == session).FirstOrDefaultAsync();
        }
        #endregion

    }
}
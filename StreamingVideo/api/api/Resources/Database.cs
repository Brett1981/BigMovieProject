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
using System.Data.Entity.Validation;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Threading;

namespace api.Resources
{
    public class Database
    {
        public static MovieData movie;
        private static MDBSQLEntities db = new MDBSQLEntities();
        private static string moviesPath = VideoController.movieDir;
        private static int createListCount = 0;
        private static int checkDbCount = 0;

        private static int databaseMovieCount = 0;

        private static MovieData[] _movies;

        public static MovieData[] allMovies
        {
            get { return _movies; }
            set { _movies = value; }
        }
        private static DateTime time;
        private static DateTime CreateListTime;

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

        public static async Task CreateList()
        {
            try
            {
                while (true)
                {
                    if (createListCount == 0) { allMovies = db.MovieDatas.Select(x => x).ToArray(); createListCount++; }
                    if(DateTime.Now > CreateListTime.AddMinutes(5)) { allMovies = await db.MovieDatas.Select(x => x).ToArrayAsync(); createListCount++; CreateListTime = DateTime.Now; }
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
                    if (checkDbCount == 0) {
                        await databaseMovieCheck();
                        time = DateTime.Now;
                        Thread t2 = new Thread(async () => await Database.CreateList());
                        t2.Priority = ThreadPriority.Normal;
                        t2.Start();
                    }
                    if (checkDbCount != 0 && DateTime.Now > time.AddMinutes(10)) { await databaseMovieCheck(); time = DateTime.Now; }
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
                var dirs = Directory.GetDirectories(moviesPath);
                List<MovieData> temp = new List<MovieData>();
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";

                var dbCount = db.MovieDatas.Count();
                if (dbCount == 0) { databaseMovieCount = 0; } else { databaseMovieCount = db.MovieDatas.Count(); }

                /*if (File.Exists(path) && new FileInfo(path).Length == 0)
                {
                    FileStream f = File.Open(path, FileMode.Open, FileAccess.Read);
                    var reader = XmlReader.Create(f);
                    XElement doc = XElement.Load(reader);
                    //doc.Descendants("MovieData").Where(x => x.Descendants("") )
                    foreach(var item in doc.Descendants("MovieData"))
                    {

                    }
                    return 1;
                }
                else{*/
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

                                MovieData mData = new MovieData() {
                                    movie_name = name,
                                    movie_ext = ext,
                                    movie_guid = CreateGuid(name).ToString(),
                                    movie_folder = item.Directory.Name.ToString(),
                                    MovieInfo = mInfo
                                };
                                db.MovieDatas.Add(mData);
                                var dbSaveInt = await db.SaveChangesAsync();
                                databaseMovieCount++;
                                temp.Add(mData);
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
                        var writer = new XmlSerializer(typeof(List<MovieData>));
                        if (File.Exists(path))
                        {
                            FileStream file = new FileStream(path, FileMode.Open,FileAccess.ReadWrite);
                            writer.Serialize(file, temp);
                            file.Close();
                        }
                        else
                        {
                            var file = File.Create(path);
                            writer.Serialize(file, temp);
                            file.Close();
                        }
                        retValue = dbSaveInt;
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
                //}
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return ex.HResult;

            }
        }
        private static Guid CreateGuid(string movieName)
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
    }
}
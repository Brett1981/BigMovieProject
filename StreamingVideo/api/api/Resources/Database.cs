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

namespace api.Resources
{
    public class Database
    {
        public static MovieData movie;
        private static MDBSQLEntities db = new MDBSQLEntities();
        private static string moviesPath = VideoController.movieDir;
        private static int createListCount = 0;
        private static int checkDbCount = 0;

        private static List<MovieData> _movies;

        public static List<MovieData> allMovies
        {
            get { return _movies; }
            set { _movies = value; }
        }
        private static DateTime time;

        public static async Task<MovieData> Get(string guid)
        {
            try
            {
                var item = await db.MovieDatas.Where(x => x.movie_guid == guid).FirstOrDefaultAsync();
                if (item != null)
                {
                    return item;
                    /*if(type == 0) {
                        return item;
                    }
                    else if(type == 1){
                        

                    }*/
                    
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
                    if (createListCount == 0) { allMovies = await db.MovieDatas.Select(x => x).ToListAsync(); createListCount++; }
                    if(DateTime.Now > time.AddSeconds(20)) { allMovies = await db.MovieDatas.Select(x => x).ToListAsync(); createListCount++; }
                    await Task.Delay(new TimeSpan(0, 0, 10));
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
                time = DateTime.Now;
                while (true)
                {
                    if (checkDbCount == 0) { await checkDatabase(); checkDbCount++; }
                    if (DateTime.Now > time.AddMinutes(10)) { await checkDatabase(); checkDbCount++; }
                    Debug.WriteLine("Done checking / creating , waiting 1 minute/s.");
                    await Task.Delay(new TimeSpan(0, 1, 0));

                }
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static async Task<int> checkDatabase(){
            try
            {
                Debug.WriteLine("Checking database for new entries!");
                var dirs = Directory.GetDirectories(moviesPath);
                List<MovieData> temp = new List<MovieData>();
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
                                if (MoviesAPI.countAPICalls > 30) {
                                    await Task.Delay(5000); MoviesAPI.countAPICalls = 0;
                                }
                                MovieInfo mInfo = await MoviesAPI.getMovieInfo(name);
                                MovieData mData = new MovieData() { movie_name = name, movie_ext = ext, movie_guid = CreateGuid(name).ToString(), MovieInfo = mInfo };
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
                        XmlDocument doc = new XmlDocument();
                        XPathNavigator nav = doc.CreateNavigator();
                        using (XmlWriter w = nav.AppendChild())
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(List<MovieData>));
                            ser.Serialize(w, temp);
                            doc.Save(@"C:\VisualStudioProjekti\BigMovieProject\StreamingVideo\movies.xml");
                        }

                        db.MovieDatas.AddRange(temp);
                        retValue = await db.SaveChangesAsync();
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
    }
}
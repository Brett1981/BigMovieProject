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

namespace api.Resources
{
    public class Database
    {
        public static MovieData movie;
        private static MDBSQLEntities db = new MDBSQLEntities();
        private static string moviesPath = VideoController.movieDir;

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
                    if(DateTime.Now > time.AddMinutes(5)) { allMovies = await db.MovieDatas.Select(x => x).ToListAsync();  }
                    await Task.Delay(new TimeSpan(0, 2, 0));
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
                    
                    if (DateTime.Now > time.AddMinutes(2))
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
                                if(ext == "mp4")
                                {
                                    var m = db.MovieDatas.Where(x => x.movie_name == name).FirstOrDefault();
                                    if (m == null)
                                    {
                                        MovieData mData = new MovieData() { movie_name = name, movie_ext = ext, movie_guid = CreateGuid(name).ToString() };
                                        temp.Add(mData);
                                    }
                                }
                            }
                           
                        }
                        if(temp.Count != 0)
                        {
                            db.MovieDatas.AddRange(temp);
                            await db.SaveChangesAsync();
                        }
                        
                    }
                    
                    Debug.WriteLine("Done checking / creating , waiting 10 minutes.");
                    await Task.Delay(new TimeSpan(0, 0, 10));

                }
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.Message);
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
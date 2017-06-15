using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Resources;
using MediaToolkit;
using MediaToolkit.Model;

namespace api.Resources
{
    public class Processing
    {
        public static List<Tuple<Movie_Data, Match>> NewMovieEntry { get; private set; }
        private static MovieDatabaseEntities Db = new MovieDatabaseEntities();
        private static int DbThreadCount;
        /// <summary>
        /// Debug is used for only creating a list of movies from database if false reads directories specified
        /// and checks db to add movie to it.
        /// </summary>
        private static bool DebugTest = false;
        private static int DatabaseMovieCount;

        public static async Task Start()
        {
            await Check.DatabaseThread();
        }

        private static class Get
        {
            /// <summary>
            /// Retrieve a movie name from its folder and return an array of strings
            /// </summary>
            /// <param name="value">string</param>
            /// <returns>Match</returns>
            public static Match ByMovieFolderName(string value)
            {
                Match r = null;
                string pattern = @"(?'title'.*)(?=\.[\d]{4})\.(?'year'[\d]{4})\.(?'pixelsize'[\d]{4}p)\.(?'format'[\w]+)\.(?'formatsize'[\w]+)-\[(?'group'.*)\]\.(?'extension'[\w]+)$";
                string pattern2 = @"(?'title'.*)\.(?'year'[^\.]+)\.(?'pixelsize'[^\.]+)\.(?'format'[^\.]+)\.(?'formatsize'[^\.]+)\.(?'filename'[^\.]+)\.(?'extension'[^\.]+)";

                if (value.Contains("[")) r = Regex.Match(value, pattern);
                else r = Regex.Match(value, pattern2);
                return r;
            }
        }

        private static class Remove
        {
            /// <summary>
            /// Remove movie database entries if directory does not exist
            /// </summary>
            /// <returns>string</returns>
            public static async Task<bool> DeletedMovies()
            {
                try
                {
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_type = "Task start -> DatabaseRemoveDeletedFolderFromDb",
                        api_action = "Starting task -> Remove deleted Movies from Database",
                        api_datetime = DateTime.Now
                    });
                    List<Movie_Data> toDelete = new List<Movie_Data>();
                    if (Database.AllMovies.Count > 0)
                    {
                        foreach (var item in Database.AllMovies)
                        {
                            if (!Directory.Exists(item.dir)) { toDelete.Add(item); }
                        }
                        if (toDelete.Count > 0)
                        {
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_type = "Task action -> DatabaseRemoveDeletedFolderFromDb",
                                api_action = "Task action -> Found " + toDelete.Count + " movies to delete",
                                api_datetime = DateTime.Now
                            });
                            Db.Movie_Data.RemoveRange(toDelete); //removing entries in database
                            await Db.SaveChangesAsync();
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_type = "Task status -> DatabaseRemoveDeletedFolderFromDb",
                                api_action = "Task status -> removed " + toDelete.Count + " movies with success",
                                api_datetime = DateTime.Now
                            });
                            return true;
                        }
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_type = "Task action -> DatabaseRemoveDeletedFolderFromDb",
                            api_action = "Task action -> No movies found for deletion",
                            api_datetime = DateTime.Now
                        });
                    }
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_type = "Task action -> DatabaseRemoveDeletedFolderFromDb",
                        api_action = "Task action -> No entries in database ...",
                        api_datetime = DateTime.Now
                    });
                }
                catch (Exception ex)
                {
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_type = "Exception thrown -> DatabaseRemoveDeletedFolderFromDb",
                        api_action = "Exception --> " + ex.Message + " -- " + ex.InnerException.InnerException,
                        api_datetime = DateTime.Now
                    });
                    
                }
                return false;
            }
        }

        private static class Check
        {
            

            /// <summary>
            /// Check directories if new movie was found that is not in the local Db
            /// </summary>
            /// <returns>null</returns>
            public static async Task DatabaseThread()
            {
                try
                {
                    DateTime Time = DateTime.Now;
                    DbThreadCount = 0;
                    while (true)
                    {
                        if (!DebugTest && DbThreadCount == 0)
                        {
                            await NewEntriesAndDeleted();
                            Time = DateTime.Now;
                            await Database.Movie.Create.List();
                        }
                        else if (!DebugTest && DbThreadCount > 0 && DateTime.Now > Time.AddMinutes(10))
                        {
                            await NewEntriesAndDeleted();
                            Time = DateTime.Now;
                        }
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action      = "Completed Task -> check/create/remove movies. | DatabaseThread()",
                            api_type        = "Task -> status ",
                            api_datetime    = DateTime.Now
                        });
                        await Task.Delay(new TimeSpan(0, 10, 0));
                        DbThreadCount++;
                    }
                }
                catch (InvalidOperationException e)
                {
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_action      = "Exception caught | Message " + e.Message,
                        api_type        = "Exception -> DatabaseThread()",
                        api_datetime    = DateTime.Now
                    });
                }
            }

            private static async Task NewEntriesAndDeleted()
            {
                await DirectoriesForNewEntries();
                await Insert.MoviesToDatabase();
                await Remove.DeletedMovies();
            }

            /// <summary>
            /// Method that checks directories
            /// </summary>
            /// <returns>null</returns>
            private static async Task DirectoriesForNewEntries()
            {
                try
                {
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_action      = "Checking directories for new entries!",
                        api_type        = "Status check",
                        api_datetime    = DateTime.Now
                    });
                    NewMovieEntry = new List<Tuple<Movie_Data, Match>>();

                    foreach (var childDirs in Global.Global.GlobalMovieDisksList)
                    {
                        var dirs = Directory.GetDirectories(childDirs.value);
                        //var DbCount = Db.MovieDatas.Count();
                        //if (DbCount == 0) { databaseMovieCount = 0; } else { databaseMovieCount = Db.MovieDatas.Count(); }

                        foreach (var d in dirs)
                        {
                            //DirectoryInfo v = new DirectoryInfo(d);
                            var files = Directory.GetFiles(d);
                            if (files.Any(s => s.Contains(".mp4") || s.Contains(".webm")))
                            {
                                var mName = files.Where(s => s.Contains(".mp4") || s.Contains(".webm")).ToArray();

                                if (mName.Count() > 0)
                                {
                                    //create a string from folder name so that it can retrieve movie information from external API
                                    var item        = new FileInfo(mName[0]);
                                    int idx         = item.Name.LastIndexOf('.');
                                    var name        = item.Name.Substring(0, idx);
                                    //regex to get movie info
                                    var movie       = Get.ByMovieFolderName(item.Name);
                                    var movieName   = movie.Groups["title"].Value.Replace('.', ' ');
                                    var ext         = movie.Groups["extension"].Value;
                                    if (ext == "mp4" || ext == "webm")
                                    {
                                        //check if movie exists in current server Db
                                        var m = await Db.Movie_Data.Where(x => x.name == name).FirstOrDefaultAsync();
                                        if (m == null)
                                        {
                                            //creating a list of movies to be searched on the selected API 
                                            NewMovieEntry.Add(new Tuple<Movie_Data, Match>(
                                                new Movie_Data() //movie data to be written to Db
                                                {
                                                    name    = name,
                                                    ext     = ext,
                                                    guid    = Database.Movie.Create.Guid(name).ToString(),
                                                    folder  = item.Directory.Name.ToString(),
                                                    dir     = item.Directory.FullName,
                                                    added   = DateTime.Now,
                                                    enabled = true,
                                                    views   = 0,
                                                    FileCreationDate = item.LastWriteTime.ToLocalTime()
                                                    
                                                },
                                                movie //movie regex
                                            ));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (NewMovieEntry.Count > 0)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action      = "Movie list contains one or more objects to be added to Db!",
                            api_type        = "Task -> new movies found",
                            api_datetime    = DateTime.Now
                        });
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action      = "Adding movies to local Db ...",
                            api_type        = "Task -> adding to local Db",
                            api_datetime    = DateTime.Now
                        });
                    }
                }
                catch (Exception ex)
                {
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_action      = "Exception --> " + ex.Message,
                        api_type        = "Exception thrown -> DatabaseMovieCheck",
                        api_datetime    = DateTime.Now
                    });
                }
            }
        }

        private static class Insert
        {

            /// <summary>
            /// Insert new movies to database
            /// </summary>
            /// <returns></returns>
            public static async Task MoviesToDatabase()
            {
                await History.Create(History.Type.API, new History_API()
                {
                    api_action = "Starting task -> Add movies to local Database",
                    api_datetime = DateTime.Now,
                    api_type = "Task -> add movies to Db",
                });
                var list = new List<int>();
                //item1 = MovieData, item2 = Match
                foreach (var item in NewMovieEntry)
                {
                    try
                    {
                        //check if movie is already in database with this title and update values only for data not info!!
                        var Db_Movie = await Db.Movie_Data.Where(x => x.name == item.Item1.name).FirstOrDefaultAsync();
                        if (Db_Movie == null)
                        {
                            //get movieinfo from api 
                            if (MoviesAPI.countAPICalls > 30) { await Task.Delay(5000); MoviesAPI.countAPICalls = 0; }
                            //editMovieInfo, movie[0] is array from method GetMovieName
                            Movie_Info mInfo = await MoviesAPI.Get.Info(item.Item2, DatabaseMovieCount);

                            if (mInfo.id != null)
                            {
                                //tagline error in database has max length of 128 char in SQL
                                if (mInfo.tagline.Length > 128) { mInfo.tagline = mInfo.tagline.Substring(0, 127); }
                                item.Item1.Movie_Info = mInfo;

                                //set tick for movie length
                                item.Item1.Movie_Info.length = Media.Length(item.Item1);

                                try
                                {
                                    Db.Movie_Data.Add(item.Item1);
                                    await Db.SaveChangesAsync();
                                    //databaseMovieCount++;
                                    //temp.Add(mData);
                                    Movie_Data movie = Db.Movie_Data.Where(x => x.name == item.Item1.name).First();
                                    await History.Create(History.Type.API, new History_API()
                                    {
                                        api_action = "Movie " + movie.Movie_Info.title + " was added to the database as id " + movie.Id + "!",
                                        api_datetime = DateTime.Now,
                                        api_type = "Movie added to database",
                                    });
                                    list.Add(movie.Id);
                                }
                                catch (Exception ex)
                                {
                                    await History.Create(History.Type.API, new History_API()
                                    {
                                        api_action = "Exception : Inserting movie to Database --> " + ex.Message,
                                        api_datetime = DateTime.Now,
                                        api_type = "Exception thrown InsertMoviesToDb",
                                    });
                                }
                            }
                            else
                            {
                                await History.Create(History.Type.API, new History_API()
                                {
                                    api_action = "Movie " + item.Item2.Groups["title"].ToString() + " was not added as there was a problem!",
                                    api_datetime = DateTime.Now,
                                    api_type = "Error on movie addition",
                                });
                            }
                        }
                        else
                        {
                            //update movie info but only directory at which the movie is located
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_action = "Movie update -> Updating movie: " + Db_Movie.name + " directory!",
                                api_datetime = DateTime.Now,
                                api_type = "Task -> update movie data / info in SQL...",
                            });
                            if (await Database.Movie.Update.Directory(Db_Movie, item.Item1))
                            {
                                await History.Create(History.Type.API, new History_API()
                                {
                                    api_action = "Movie update -> Succesfully updated movie: " + Db_Movie.name + " directory!",
                                    api_datetime = DateTime.Now,
                                    api_type = "Task -> Success",
                                });
                            }
                            else
                            {
                                await History.Create(History.Type.API, new History_API()
                                {
                                    api_action = "Movie update -> Failed to update movie: " + Db_Movie.name + " directory!",
                                    api_datetime = DateTime.Now,
                                    api_type = "Task -> Failed",
                                });
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Error -> An error occured : " + ex.Message,
                            api_datetime = DateTime.Now,
                            api_type = "Error occured on InsertMoviesToDb",
                        });
                    }

                }
                await History.Create(History.Type.API, new History_API()
                {
                    api_action = "End of import of movies.",
                    api_datetime = DateTime.Now,
                    api_type = "Status -> InsertMoviesToDb",
                });
                History_API hapi;
                if (list.Count == NewMovieEntry.Count)
                {
                    hapi = new History_API()
                    {
                        api_type = "Movie to Db status",
                        api_datetime = DateTime.Now,
                        api_action = "Info -> All movies added (" + list.Count + " - ADDED)",
                    };
                }
                else if (list.Count < NewMovieEntry.Count)
                {
                    hapi = new History_API()
                    {
                        api_type = "Movie to Db status",
                        api_datetime = DateTime.Now,
                        api_action = "Error -> Less movies added than found on local storage! Movies added " + list.Count,
                    };
                }
                else
                {
                    hapi = new History_API()
                    {
                        api_type = "Movie to Db status",
                        api_datetime = DateTime.Now,
                        api_action = "Error -> Something went wrong with importing data to Db! Movies added " + list.Count,
                    };
                }
                if (hapi != null)
                {
                    await History.Create(History.Type.API, hapi);
                }
            }
        }

        private static class Media
        {
            /// <summary>
            /// Return the media's (movies) length as ticks
            /// </summary>
            /// <param name="data">Movie_Data</param>
            /// <returns>long</returns>
            public static long Length(Movie_Data data)
            {

                var movie = (Directory.EnumerateFiles(data.dir))
                            .Where(y => y.Contains(data.ext) || y.Contains(data.name))
                            .FirstOrDefault();
                
                var inputFile = new MediaFile { Filename = movie };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                }

                return inputFile.Metadata.Duration.Ticks;
            }
        }
    }
}
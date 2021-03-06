﻿using System;
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
using api.Resources.Enum;
using api.Resources.Auth;
using api.Resources.Global;
using System.Net.Mail;

namespace api.Resources
{
    public class Database
    {

        public static Movie_Data movie;
        private static MovieDatabaseEntities db = new MovieDatabaseEntities();
        public static List<Movie_Data> MovieList { get; private set; }
        
        /// <summary>
        /// Movie Database  public / private items
        /// </summary>
        public static List<Movie_Data> AllMovies
        {
            get
            {
                if (MovieList != null) { return MovieList; }
                else
                {
                    try {  Movie.Refresh.RefreshAndOrganize(); }
                    catch (Exception ex)
                    {
                        
                        Debug.WriteLine("Exception caught on AllMovies() -> " + ex.Message);
                    }
                    finally
                    {
                        if (MovieList == null || MovieList.Count == 0) { MovieList = null; }
                    }
                    return MovieList;
                }
            }
            set { MovieList = value; }
        }


        public static class Movie
        {

            public static class Create
            {
                /// <summary>
                /// Create movie list every x minutes or hours, depends how it is set up
                /// </summary>
                /// <returns>null</returns>
                public static async Task<bool> List()
                {
                    try
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Creating new movie list",
                            api_type = "Task -> status",
                            api_datetime = DateTime.Now
                        });
                        AllMovies = await db.Movie_Data.Select(x => x).OrderByDescending(x => x.FileCreationDate).ToListAsync();
                        Organize.ByDate();
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Waiting for next movie list cycle ",
                            api_type = "Task -> waiting",
                            api_datetime = DateTime.Now
                        });
                        return true;
                    }
                    catch (Exception e)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Exception thrown at -> Movie.Create.List() | Error -> "+ e.Message,
                            api_type = "Task -> Error | Exception caught -> Movie.Create.List()",
                            api_datetime = DateTime.Now
                        });
                        return false;
                    }
                }

                /// <summary>
                /// GUID method that is called when a new movie is about to be added to local db. 
                /// This GUID is ussed for references when a movie is searched for
                /// </summary>
                /// <param name="movieName">string</param>
                /// <returns>Guid</returns>
                public static Guid Guid(string value)
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        return new Guid(md5.ComputeHash(Encoding.Default.GetBytes(value)));
                    }
                }
            }

            public static class Edit 
            {
                /// <summary>
                /// Edit movie from MVC view
                /// </summary>
                /// <param name="guid">string</param>
                /// <param name="collection">System.Web.Mvc.FormCollection</param>
                /// <returns>MovieData</returns>
                internal static async Task<Movie_Data> Form(string guid, System.Web.Mvc.FormCollection collection)
                {
                    try
                    {
                        var movie = await Database.Movie.Get.ByGuid(guid);
                        foreach (var item in collection.AllKeys)
                        {
                            var i = collection.GetValue(item);
                            switch (item)
                            {
                                case "movie_name": { movie.name = i.AttemptedValue; } break;
                                case "movie_ext": { movie.ext = i.AttemptedValue; } break;
                                case "movie_folder": { movie.folder = i.AttemptedValue; } break;
                                case "movie_guid": { movie.guid = i.AttemptedValue; } break;
                                case "movie_dir": { movie.dir = i.AttemptedValue; } break;
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
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exception Database.Edit --> " + e.Message);
                        return new Movie_Data();
                    }
                }

                public static async Task<Movie_Data> MovieEnable(string guid, MovieStatus status)
                {
                    var movie = await db.Movie_Data.Where(x => x.guid == guid).FirstAsync();
                    if (movie != null)
                    {
                        if (status == MovieStatus.Enable) movie.enabled = true;
                        else if (status == MovieStatus.Disable) movie.enabled = false;
                        await db.SaveChangesAsync();
                        return movie;
                    }
                    return null;
                }

                public static async Task<bool> Movie(Movie_Data movie)
                {
                    if (movie.guid.Length > 0)
                    {
                        var m = await Get.ByGuidAndChangeCounter(movie.guid, false);
                        if (m != null && m != movie)
                        {
                            db.Movie_Data.Remove(m);
                            if (!Functions.Functions.PropertyCheck.IsAnyNullOrEmpty(movie))
                            {
                                db.Movie_Data.Add(movie);
                                await db.SaveChangesAsync();
                                return true;
                            }
                            return false;
                        }
                    }
                    return false;
                }
            }

            public static class Get
            {

                /// <summary>
                /// Get movie data from local db
                /// </summary>
                /// <param name="guid">string</param>
                /// <returns>MovieData</returns>
                public static async Task<Movie_Data> ByGuid(string guid)
                {
                    try
                    {
                        var item = await db.Movie_Data.Where(x => x.guid == guid).FirstOrDefaultAsync();
                        if (item != null)
                            return new Movie_Data()
                            {
                                Movie_Info = item.Movie_Info,
                                views = item.views,
                                enabled = item.enabled,
                                guid = item.guid
                            };
                        return new Movie_Data();
                    }
                    catch (Exception e)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Exception was thrown with error " +e.Message,
                            api_type = "Exception caught on -> Movie.Get.ByGuid()",
                            api_datetime = DateTime.Now,
                        });
                        return new Movie_Data();
                    }
                }

                /// <summary>
                /// Retrieve movie data from db using a guid string as reference and increase the view counter if movie found
                /// </summary>
                /// <param name="guid">string</param>
                /// <returns name="MovieData">MovieData</returns>
                public static async Task<Movie_Data> ByGuidAndChangeCounter(string guid, bool isPlay = false)
                {
                    var movie = await db.Movie_Data.Where(x => x.guid == guid).FirstOrDefaultAsync();
                    if (movie != null)
                    {
                        if (!isPlay)
                        {
                            return movie;
                        }
                        else
                        {
                            if (movie.views == null) { movie.views = 1; }
                            else { movie.views += 1; }
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                await History.Create(History.Type.API, new History_API()
                                {
                                    api_action = "Exception was thrown with error " + e.Message,
                                    api_type = "Exception caught on -> Movie.Get.ByGuidAndChangeCounter()",
                                    api_datetime = DateTime.Now,
                                });
                            }
                            finally
                            {
                                movie = await db.Movie_Data.Where(x => x.guid == guid).FirstOrDefaultAsync();
                            }
                            if (movie == null)
                            {
                                return new Movie_Data();
                            }
                            return movie;
                        }

                    }
                    return new Movie_Data();

                }

                /// <summary>
                /// Retrieve movie from db and increase the view counter
                /// </summary>
                /// <param name="data">DatabaseUserModels</param>
                /// <returns>MovieData</returns>
                public static async Task<Movie_Data> ByModel(DatabaseUserModels data)
                {

                    var user = await db.User_Info.Where(x => x.unique_id == data.user_id).FirstOrDefaultAsync();
                    var movie = await db.Movie_Data.Where(x => x.guid == data.movie_id).FirstOrDefaultAsync();

                    if (user != null)
                    {

                        await History.Create(History.Type.User, new History_User()
                        {
                            user_action = "User | Requesting content-> " + data.movie_id + 
                            " | Username -> "+ user.username +", UserId-> " + user.unique_id,
                            user_datetime = DateTime.Now,
                            user_movie = movie.Movie_Info.title,
                            user_type = "User  Requesting content from Movie.Get.ByModel()"
                        });
                    }
                    return movie;
                }


                /// <summary>
                /// Retrieve movies from db where genre is the same as searched
                /// </summary>
                /// <param name="genre">string</param>
                /// <returns>MovieData</returns>
                public static List<Movie_Data> ByGenre(string genre)
                {
                    List<Movie_Data> searchedMovies = new List<Movie_Data>();
                    foreach (var item in AllMovies)
                    {
                        if (item.Movie_Info.genres != "")
                        {
                            var g = item.Movie_Info.genres;
                            if (g.Contains("|"))
                            {
                                var h = g.Split('|');
                                foreach (var gnr in h)
                                {
                                    var y = gnr.Split(':');
                                    if (y[1].ToLower() == genre.ToLower())
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
                                    if (h[1].ToLower() == genre.ToLower())
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
                public static async Task<List<Movie_Data>> Last10()
                {
                    return await db.Movie_Data.OrderByDescending(x => x.FileCreationDate).Take(10).ToListAsync();
                }

                /// <summary>
                /// Returns the top 10 most played movies 
                /// </summary>
                /// <returns>MovieData</returns>
                public static async Task<List<Movie_Data>> Top10()
                {
                    return await db.Movie_Data.Where(x => x.views > 0).Take(5).ToListAsync();
                }

                /// <summary>
                /// Returns an array of movies that contain the searched query
                /// </summary>
                /// <param name="name">string</param>
                /// <returns>List<Movie_Data></returns>
                public static async Task<List<Movie_Data>> ByName(string name)
                {
                    return await db.Movie_Data.Where(x => x.Movie_Info.title.Contains(name)).Take(10).ToListAsync();
                }
            }

            public static class Remove
            {
                /// <summary>
                /// Remove movie from db
                /// </summary>
                /// <param name="item">MovieData</param>
                /// <returns>int</returns>
                public static async Task<int> ByModel(Movie_Data item)
                {
                    try
                    {
                        db.Movie_Data.Remove(item);
                        return await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Exception caught",
                            api_type = "Exception -> Movie.Remove.ByModel() --> " + ex.Message,
                            api_datetime = DateTime.Now
                        });
                        return -1;
                    }

                }

            }

            public static class Organize
            {
                /// <summary>
                /// Organize list of movies by their release date
                /// </summary>
                public static void ByDate()
                {
                    if (AllMovies.Count > 0)
                    {
                        try
                        {
                            AllMovies.Sort((x, y) => y.Movie_Info.release_date.Value.CompareTo(x.Movie_Info.release_date.Value));
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine("Exception at Movie.Organize.ByDate() -> Message: " + ex.Message);
                        }
                        
                    }

                }
            }

            public static class Refresh
            {
                /// <summary>
                /// Force creating the movie list when an element is deleted from db or added
                /// </summary>
                public static void RefreshAndOrganize()
                {
                    AllMovies = db.Movie_Data.Select(x => x).ToList();
                    Organize.ByDate();
                }
            }
            

            public static class Update
            {
                public static async Task<bool> Directory(Movie_Data sqlData, Movie_Data readDataFromDisk)
                {
                    
                    if (sqlData != null && readDataFromDisk != null 
                        && sqlData != readDataFromDisk)
                    {
                        sqlData.dir     = readDataFromDisk.dir;
                        sqlData.folder  = readDataFromDisk.folder;
                        await db.SaveChangesAsync();
                        return true;
                    }
                    return false;
                    
                }
            }
        }

        public static class User
        {
            public enum Type
            {
                User,
                Administrator
            }

            public static class Get
            {
                /// <summary>
                /// Retrieve user profile image
                /// </summary>
                /// <param name="path"></param>
                /// <returns></returns>
                public static async Task<byte[]> ProfileImage(string path)
                {
                    var imgdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                                 Global.Global.GlobalServerSettings.Where(x => x.name == "ServerProfileImageDir").First().value.Replace('/', '\\'), 
                                 path.Replace('/', '\\'));
                    //var imgdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\users\", path);
                    if (File.Exists(imgdir))
                    {
                        try
                        {
                            ImageConverter converter = new ImageConverter();
                            var i = Image.FromFile(imgdir);
                            var b = (byte[])converter.ConvertTo(i, typeof(byte[]));
                            i.Dispose();
                            return b;
                        }
                        catch (Exception ex)
                        {
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_action = "Exception caught | Message " + ex.Message,
                                api_type = "Exception -> User.Get.ProfileImage()",
                                api_datetime = DateTime.Now
                            });
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
                public static async Task<User_Info> ByGuid(string guid)
                {
                    var u = await db.User_Info.Where(x => x.unique_id == guid).FirstOrDefaultAsync();
                    if (u == null) { return null; }
                    return u;
                }

                /// <summary>
                /// Retrieve user information from database by supplying the username
                /// </summary>
                /// <param name="username">string</param>
                /// <returns>User_Info</returns>
                public static async Task<User_Info>ByUsername(string username)
                {
                    return await db.User_Info.Where(x => x.username == username).FirstOrDefaultAsync();
                }

                /// <summary>
                /// Retrieve a session from database 
                /// </summary>
                /// <param name="data">DatabaseUserModels</param>
                /// <returns>SessionPlay</returns>
                public static async Task<Session_Play> Session(DatabaseUserModels data)
                {
                    return await db.Session_Play.Where(x => x.movie_id == data.movie_id && x.user_id == data.user_id).FirstOrDefaultAsync();
                }

                /// <summary>
                /// Retrieve data from db by providing session guid that was generated by requesting a movie
                /// </summary>
                /// <param name="session">string</param>
                /// <returns>SessionGuest</returns>
                public static async Task<Object> BySession(string session)
                {
                    Session_Guest guest;
                    guest = await db.Session_Guest.Where(x => x.session_id == session).FirstOrDefaultAsync();
                    if (guest == null)
                    {
                        return await db.Session_Play.Where(x => x.session_id == session).FirstOrDefaultAsync();
                    }
                    return guest;
                }

                /// <summary>
                /// Return all User data from database
                /// </summary>
                /// <returns>List</returns>
                public static async Task<CustomClasses.API.Users> AllUsersData()
                {
                    return new CustomClasses.API.Users()
                    {
                        groups = await db.User_Groups.Select(x => x).ToListAsync(),
                        users = await db.User_Info.Select(x => x).ToListAsync()
                    };
                }

                /// <summary>
                /// Return last 100 rows in user's history box
                /// </summary>
                /// <param name="value">string</param>
                /// <returns>List<History_User></returns>
                public static async Task<List<History_User>> UserHistory(string value)
                {
                    return await db.History_User
                        .OrderByDescending(x => x.user_datetime)
                        .Where(x => x.user_id == value)
                        .Take(20)
                        .ToListAsync();
                }
            }

            public static class Set
            {
                public static async Task LastLogon(User_Info user)
                {
                    try
                    {
                        user.last_logon = DateTime.Now;
                        await db.SaveChangesAsync();
                    }
                    catch(Exception ex)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Exception caught on User.Set.LastLogon with error -> "+ex.Message,
                            api_type = "Exception ->  User.Set.LastLogon"
                        });
                    }
                    
                }
            }

            public static class Create
            {
                /// <summary>
                /// Creating a session guid that allows users to play content
                /// </summary>
                /// <param name="data">DatabaseUserModels</param>
                /// <returns>string</returns>
                public static async Task<Object> Session<T>(DatabaseUserModels data)
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
                    List<CustomClasses.Random.values> v = new List<CustomClasses.Random.values>();
                    if (isGuest)
                    {
                        var s = new Session_Guest()
                        {
                            session_date = DateTime.Now,
                            session_id = Movie.Create.Guid(result.ToString()).ToString(),
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
                            session_id = Movie.Create.Guid(result.ToString()).ToString(),
                            movie_id = data.movie_id,
                            user_id = data.user_id
                        };
                        db.Session_Play.Add(s);
                        await db.SaveChangesAsync();
                        return s;
                    }


                }

                public static async Task<User_Info> New(User_Info data,User_Groups group = null)
                {
                    try
                    {
                        var birthdate = (data.birthday != null) ? Convert.ToDateTime(data.birthday) : DateTime.Now;

                        var user = new User_Info()
                        {
                            username = data.username,
                            password = Functions.Functions.Encode.StringToBase64(data.password),
                            email = data.email,
                            display_name = data.display_name ?? "User",
                            birthday = (data.birthday != null) ? Convert.ToDateTime(data.birthday) : DateTime.Now,
                            unique_id = Guid(data.username).ToString(),
                            profile_created = DateTime.Now
                        };
                        User_Groups groupDB;
                        if(group != null)
                            groupDB = await db.User_Groups.Where(x => x.type == group.type ).FirstAsync();
                        else
                            groupDB = await db.User_Groups.Where(x => x.type == "user").FirstAsync();

                        user.group = groupDB.Id;
                        user.User_Groups = groupDB;
                        db.User_Info.Add(user);
                        await db.SaveChangesAsync();
                        await History.Create(History.Type.User, new History_User()
                        {
                            user_action = "User create -> " + user.unique_id,
                            user_datetime = DateTime.Now,
                            user_id = user.unique_id,
                            user_movie = "",
                            user_type = "UserCreationSuccess"
                        });
                        return user;
                       
                    }
                    catch(Exception ex)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Exception caught on User.Create.New -> Message: "+ex.Message,
                            api_type = "Exception"
                        });
                    }
                    return null;
                    
                }

                public static Guid Guid(string data)
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        return new Guid(md5.ComputeHash(Encoding.Default.GetBytes(data)));
                    }
                }
            }

            

            public static class Edit
            {
                /// <summary>
                /// Changes user profile picture when user prompts to change it
                /// </summary>
                /// <param name="user"></param>
                /// <returns></returns>
                public static async Task<string> UserPicture(Auth.Auth.User user)
                {
                    var uData = await db.User_Info.Where(x => x.unique_id == user.unique_id).FirstOrDefaultAsync();
                    if (uData != null)
                    {
                        try
                        {
                            HttpClient client = new HttpClient();
                            var img = Image.FromStream(await client.GetStreamAsync(user.image_url));

                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\users\", uData.unique_id);

                            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                            var i = Path.Combine(path, "profile.jpg");
                            if (!File.Exists(i))
                            {
                                img.Save(i);
                            }
                            else
                            {

                                File.Delete(i);
                                img.Save(i);
                            }
                            img.Dispose();
                            uData.profile_image = uData.unique_id + "/profile.jpg";
                            db.Entry(uData).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            return "OK";
                        }
                        catch (HttpException ex)
                        {
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_action = "HttpException at ChangeUserPicture -->" + ex.Message + " | User -> name -" + user.display_name + ", guid - " + user.unique_id,
                                api_type = "Exception thrown -> ChangeUserPicture",
                                api_datetime = DateTime.Now
                            });
                            return "Exception";
                        }
                    }
                    return "NotAuthorized";
                }

                /// <summary>
                /// Edit user data in database and save it
                /// </summary>
                /// <param name="user">User_Info</param>
                /// <returns>bool</returns>
                public static async Task<bool> Data(User_Info user, User_Groups group)
                {
                    var u = await Get.ByGuid(user.unique_id);
                    if(u != null && u.username == user.username)
                    {
                        if(group != null && u.User_Groups != group)
                        {
                            try
                            {
                                User_Info tmp = new User_Info()
                                {
                                    birthday = u.birthday,
                                    display_name = u.display_name,
                                    email = u.email,
                                    last_logon = u.last_logon,
                                    password = u.password,
                                    profile_created = u.profile_created,
                                    profile_image = u.profile_image,
                                    unique_id = u.unique_id,
                                    username = u.username
                                };
                                db.User_Info.Remove(u);

                                //edit tmp user group
                                tmp.group = group.Id;

                                db.User_Info.Add(tmp);
                                await db.SaveChangesAsync();
                                return true;
                            }
                            catch (DbEntityValidationException ex)
                            {
                                await History.Create(History.Type.API, new History_API()
                                {
                                    api_action = "Exception caught on User.Edit.Data -> " + ex.Message,
                                    api_type = "Exception -> User.Edit.Data",
                                    api_datetime = DateTime.Now
                                });
                                return false;
                            }
                        }
                        
                    }
                    return false;
                }
            }

            public static class Check
            {
                public static async Task<bool> IsAdmin(Auth.Auth.User user)
                {
                    if(user.unique_id != null && user.username != null)
                    {
                        var admin = await Get.ByGuid(user.unique_id);
                        if(admin.username == user.username && admin.User_Groups.type == Type.Administrator.ToString())
                        {
                            return true;
                        }
                    }
                    return false;
                }

                public static async Task<bool> IsAdmin(Auth.Auth.Login user)
                {
                    if (user.password != null && user.username != null)
                    {
                        var admin = await Get.ByUsername(user.username);
                        if (admin.username == user.username 
                            && admin.User_Groups.type == Type.Administrator.ToString() 
                            && admin.password == user.password)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            public static class Remove
            {
                public static async Task<bool> User(User_Info user)
                {
                    db.User_Info.Remove(user);
                    await db.SaveChangesAsync();
                    if(await db.User_Info.Where(u => u.unique_id == user.unique_id).FirstOrDefaultAsync() == null)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

            

    }
}
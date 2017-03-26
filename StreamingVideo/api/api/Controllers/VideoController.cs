using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Resources;
using api.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace api.Controllers
{
    [EnableCors(origins: "http://31.15.224.24", headers: "*", methods: "GET, POST")]
    public class VideoController : ApiController
    {
        //public static string[] movieDir = { @"D:\Torrent2\Movies", @"K:\uTorrent\Movies" };
        public static HttpClient client = new HttpClient();
        
        //GET: api/videoplay/value
        [HttpGet, ActionName("Play")]
        public async Task<HttpResponseMessage> Play([FromUri]string value)
        {
            try
            {
                //get user id and movie id from session in database
                var s = await Database.User.GetBySession(value);

                Session_Play sp = new Session_Play();
                Session_Guest sg = new Session_Guest();
                string movieId = "";
                Movie_Data movie;
                if (s is Session_Play)
                {
                    sp = (Session_Play)s;
                    movieId = sp.movie_id;
                    movie = await Database.Movie.GetMovie(sp.movie_id, true);
                }
                else {
                    sg = (Session_Guest)s;
                    movieId = sg.movie_id;
                    movie = await Database.Movie.GetMovie(sg.movie_id, true);
                }
                if (sp != null || sg != null && movieId != null)
                {   
                     
                    await History.Set("user", new History_User()
                    {
                        user_action = "User requesting to watch movie: " + value,
                        user_datetime = DateTime.Now,
                        user_id = sp.user_id,
                        user_movie = movie.Movie_Info.title,
                        user_type = "Status -> User Request -> Movie"
                    });
                    //Getting movie from DB
                    if (movie != null && movie.enabled)
                    {
                        await History.Set("user", new History_User()
                        {
                            user_action = "Movie " + value + " is being served.",
                            user_datetime = DateTime.Now,
                            user_id = sp.user_id,
                            user_movie = movie.Movie_Info.title,
                            user_type = "Status -> Movie -> start send"
                        });

                        //streaming content to client
                        return Streaming.StreamingContent(movie, base.Request.Headers.Range);
                    }
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            catch(System.Web.HttpException ex)
            {
                Debug.WriteLine(ex);
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
            
        }

        //GET: api/video/allmovies
        [HttpGet, ActionName("AllMovies")]
        public IHttpActionResult AllMovies()
        {
            return Ok(Database.AllMovies);
        }

        //GET: api/video/getmovie/value
        [HttpGet,ActionName("GetMovie")]
        public async  Task<IHttpActionResult> GetMovie([FromUri]string value)
        {
            return Ok(await Database.Movie.GetMovie(value));
        }

        //GET: api/video/moviebyid
        [HttpGet, ActionName("GetMovieById")]
        public async Task<IHttpActionResult> GetMovieById([FromUri]string value)
        {
            return Ok(await Database.Movie.Get(value));
        }

        //POST: api/video/getmovie (object)
        [HttpPost,ActionName("GetMovie")]
        public async Task<IHttpActionResult> GetMovie([FromBody] DatabaseUserModels data)
        {
            var movie = await Database.Movie.GetMovie(data);
            if(movie == null)
            {
                Debug.WriteLine("Content '" + data.movie_id + "' does not exits");
                return NotFound();
            }
            CustomClasses.MovieSession a = new CustomClasses.MovieSession()
            {
                movieData = movie
            };
            if (data.user_id.Length < 20)
            {
                //user is a guest
                Debug.WriteLine("Guest is authorized to watch movie : " + movie.name);
                await History.Set("user", new History_User()
                {
                    user_action = "Guest | Auth to watch Movie -> " + movie.Movie_Info.title,
                    user_datetime = DateTime.Now,
                    user_id = data.user_id,
                    user_movie = movie.guid,
                    user_type = "Status -> AuthGuest -> View Content"
                });
                a.sessionGuest = (Session_Guest)await Database.User.CreateSession<Session_Guest>(data);
                
            }
            else
            {
                //user is registered
                var u = await Database.User.FindUser(data.user_id);
                if(u != null)
                {
                    await History.Set("user", new History_User()
                    {
                        user_action = "User -> " + u.display_name + " | Auth -> " + data.user_id + " | Movie -> " + movie.Movie_Info.title,
                        user_datetime = DateTime.Now,
                        user_id = data.user_id,
                        user_movie = movie.guid,
                        user_type = "Status -> AuthRegistered -> View Content"
                    });
                    a.sessionPlay = (Session_Play)await Database.User.CreateSession<Session_Play>(data);
                }
                
            }
            return Ok(a);

            
            
        }
        
        //GET: api/video/subs/value
        [HttpGet,ActionName("Subs")]
        public async Task<IHttpActionResult> Subs([FromUri]string value)
        {
            await Task.Delay(0);
            return Ok();
        }

        //GET: api/video/genre/value
        [HttpGet, ActionName("Genre")]
        public IHttpActionResult Genre([FromUri]string value)
        {
            if(value == "scifi") { value = "Science Fiction"; } //website has scifi short for science fiction
            var g = Database.Movie.GetByGenre(value);
            if(g.Count == 0)
            {
                return NotFound();
            }
            return Ok(g);
        }

        //GET: api/video/top10
        [HttpGet,ActionName("Top10")]
        public async Task<IHttpActionResult> Top10()
        {
            return Ok(await Database.Movie.GetTop10());
        }

        //GET: api/video/Last10
        [HttpGet, ActionName("Last10")]
        public async Task<IHttpActionResult> Last10()
        {
            return Ok(await Database.Movie.GetLast10());
        }
        //POST: api/video/session
        [HttpPost,ActionName("GetSession")]
        public async Task<IHttpActionResult> GetSession([FromBody] DatabaseUserModels data)
        {
            var s = await Database.User.GetSession(data);
            if(s.session_id == "" || s == null)
            {
                return NotFound();
            }
            return Ok(s.session_id);
        }
        
    }
}

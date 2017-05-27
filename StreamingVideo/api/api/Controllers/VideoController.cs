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
        
        //GET: api/video/play/value
        [HttpGet, ActionName("Play")]
        public async Task<HttpResponseMessage> Play([FromUri]string value)
        {
            try
            {
                //get user id and movie id from session in database
                var s = await Database.User.Get.BySession(value);

                Session_Play sp = new Session_Play();
                Session_Guest sg = new Session_Guest();
                string movieId = "";
                Movie_Data movie;
                if (s is Session_Play)
                {
                    sp = (Session_Play)s;
                    movieId = sp.movie_id;
                    movie = await Database.Movie.Get.ByGuidAndChangeCounter(sp.movie_id, true);
                }
                else {
                    sg = (Session_Guest)s;
                    movieId = sg.movie_id;
                    movie = await Database.Movie.Get.ByGuidAndChangeCounter(sg.movie_id, true);
                }
                if (sp != null || sg != null && movieId != null)
                {   
                     
                    await History.Create(History.Type.User, new History_User()
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
                        await History.Create(History.Type.User, new History_User()
                        {
                            user_action = "Movie " + value + " is being served.",
                            user_datetime = DateTime.Now,
                            user_id = sp.user_id,
                            user_movie = movie.Movie_Info.title,
                            user_type = "Status -> Movie -> start send"
                        });

                        //streaming content to client
                        return await Streaming.Content(movie, base.Request.Headers.Range);
                    }
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            catch(System.Web.HttpException ex)
            {
                await History.Create(History.Type.API, new History_API()
                {
                    api_action = "Exception caught on Play() -> " + ex.Message,
                    api_datetime = DateTime.Now,
                    api_type = "Exception thrown -> Play"
                });
                    
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
            
        }

        //GET: api/video/all
        [HttpGet, ActionName("All")]
        public IHttpActionResult All()
        {
            return Ok(Database.allMovies);
        }

        //GET: api/video/get/value
        [HttpGet,ActionName("Get")]
        public async  Task<IHttpActionResult> GetMovie([FromUri]string value)
        {
            return Ok(await Database.Movie.Get.ByGuid(value));
        }

        //POST: api/video/get
        [HttpPost,ActionName("Get")]
        public async Task<IHttpActionResult> GetMovie([FromBody] DatabaseUserModels data)
        {
            var movie = await Database.Movie.Get.ByModel(data);
            if(movie == null)
            {
                return NotFound();
            }
            CustomClasses.MovieSession a = new CustomClasses.MovieSession()
            {
                movieData = movie
            };
            if (data.user_id.Length < 20)
            {
                //user is a guest
                await History.Create(History.Type.User, new History_User()
                {
                    user_action = "Guest | Auth to watch Movie -> " + movie.Movie_Info.title,
                    user_datetime = DateTime.Now,
                    user_id = data.user_id,
                    user_movie = movie.guid,
                    user_type = "Status -> AuthGuest -> View Content"
                });
                a.sessionGuest = (Session_Guest)await Database.User.Create.Session<Session_Guest>(data);
                
            }
            else
            {
                //user is registered
                var u = await Database.User.Get.ByGuid(data.user_id);
                if(u != null)
                {
                    await History.Create(History.Type.User, new History_User()
                    {
                        user_action = "User -> " + u.display_name + " | Auth -> " + data.user_id + " | Movie -> " + movie.Movie_Info.title,
                        user_datetime = DateTime.Now,
                        user_id = data.user_id,
                        user_movie = movie.guid,
                        user_type = "Status -> AuthRegistered -> View Content"
                    });
                    a.sessionPlay = (Session_Play)await Database.User.Create.Session<Session_Play>(data);
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
        [HttpGet, ActionName("ByGenre")]
        public IHttpActionResult Genre([FromUri]string value)
        {
            if(value == "scifi") { value = "Science Fiction"; } //website has scifi short for science fiction
            var g = Database.Movie.Get.ByGenre(value);
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
            return Ok(await Database.Movie.Get.Top10());
        }

        //GET: api/video/Last10
        [HttpGet, ActionName("Last10")]
        public async Task<IHttpActionResult> Last10()
        {
            return Ok(await Database.Movie.Get.Last10());
        }

        //POST: api/video/session
        [HttpPost,ActionName("GetSession")]
        public async Task<IHttpActionResult> GetSession([FromBody] DatabaseUserModels data)
        {
            var s = await Database.User.Get.Session(data);
            if(s.session_id == "" || s == null)
            {
                return NotFound();
            }
            return Ok(s.session_id);
        }

        //GET: api/video/search
        [HttpGet, ActionName("Search")]
        public async Task<IHttpActionResult> Search([FromUri]string value)
        {
            return Ok(await Database.Movie.Get.ByName(value));
        }
        
    }
}

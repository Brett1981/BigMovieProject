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
using System.Net.Http.Formatting;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace api.Controllers
{
    [EnableCors(origins: "http://31.15.224.24", headers: "*", methods: "GET, POST")]
    public class VideoController : ApiController
    {
        public static string[] movieDir = { @"D:\Torrent2\Movies", @"K:\uTorrent\Movies" };
        public static HttpClient client = new HttpClient();
        //GET: api/videoplay/value
        [HttpGet, ActionName("Play")]
        public async Task<HttpResponseMessage> Play([FromUri]string value)
        {
            try
            {
                //get user id and movie id from session in database
                var s = await Database.GetBySession(value);
                if (s != null && s.movie_id != null)
                {
                    Debug.WriteLine("User requesting to watch movie: " + value);
                    //Getting movie from DB
                    var movie = await Database.Get(s.movie_id);
                    if (movie != null)
                    {
                        Debug.WriteLine("Movie " + value + " is being served.");
                        await History.Set("user", new History_User()
                        {
                            user_action = "User requesting to watch movie: " + value,
                            user_datetime = DateTime.Now,
                            user_id = "",
                            user_movie = movie.movie_guid,
                            user_type = "Request"
                        });
                        //streaming content to client
                        return Streaming.streamingContent(movie, base.Request.Headers.Range);
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
            return Ok(Database.allMovies);
        }

        //GET: api/video/getmovie/value
        [HttpGet,ActionName("GetMovie")]
        public async  Task<IHttpActionResult> GetMovie([FromUri]string value)
        {
            return Ok(await Database.GetMovie(value));
        }

        //POST: api/video/getmovie (object)
        [HttpPost,ActionName("GetMovie")]
        public async Task<IHttpActionResult> GetMovie([FromBody] DatabaseUserModels data)
        {
            var movie = await Database.GetMovie(data);
            if(movie == null)
            {
                Debug.WriteLine("Content '" + data.movie_id + "' does not exits");
                return NotFound();
            }
            Debug.WriteLine("User '"+data.user_id+"' is authorized to watch movie : " + movie.movie_name);
            await History.Set("user", new History_User()
            {
                user_action = "Authorization to watch movie: " + movie.movie_name,
                user_datetime = DateTime.Now,
                user_id = data.user_id,
                user_movie = movie.movie_guid,
                user_type = "Authorization"
            });
            await Database.CreateSession(data);
            return Ok(movie);
        }

        //GET: api/video/subs/value
        [HttpGet,ActionName("Subs")]
        public async Task<IHttpActionResult> Subs([FromUri]string value)
        {
            return Ok();
        }

        //GET: api/video/genre/value
        [HttpGet, ActionName("Genre")]
        public IHttpActionResult Genre([FromUri]string value)
        {
            if(value == "scifi") { value = "Science Fiction"; } //website has scifi short for science fiction
            var g = Database.GetByGenre(value);
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
            return Ok(await Database.GetTop10());
        }

        //GET: api/video/Last10
        [HttpGet, ActionName("Last10")]
        public async Task<IHttpActionResult> Last10()
        {
            return Ok(await Database.GetLast10());
        }
        //POST: api/video/session
        [HttpPost,ActionName("GetSession")]
        public async Task<IHttpActionResult> GetSession([FromBody] DatabaseUserModels data)
        {
            var s = await Database.GetSession(data);
            if(s.session_id == "" || s == null)
            {
                return NotFound();
            }
            return Ok(s.session_id);
        }
    }
}

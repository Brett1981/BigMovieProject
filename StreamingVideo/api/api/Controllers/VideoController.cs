﻿using System;
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
        //public const string movieDir = @"E:\Git\BigMovieProject\StreamingVideo\movies\";
        //public const string movieDir = @"E:\Torrent2\Movies";

        public static string[] movieDir = { @"E:\Torrent2\Movies", @"K:\uTorrent\Movies" };
        public static HttpClient client = new HttpClient();
        
        [HttpGet, ActionName("Play")]
        public async Task<HttpResponseMessage> Play([FromUri]string value)
        {
            Debug.WriteLine("User requesting to watch movie: " + value);
            //Getting movie from DB
            var movie = await Database.Get(value);
            if(movie != null)
            {
                Debug.WriteLine("Movie "+value+" is being served.");
                //streaming content to client
                return Streaming.streamingContent(movie, base.Request.Headers.Range);
            }
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
        [HttpGet, ActionName("AllMovies")]
        public IHttpActionResult AllMovies()
        {
            return Ok(Database.allMovies);
        }
        //GET: api/video/getmovie?id=
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
            return Ok(movie);
        }

        [HttpGet,ActionName("Subs")]
        public async Task<IHttpActionResult> Subs([FromUri]string value)
        {
            return Ok();
        }

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
    }
}

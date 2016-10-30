using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Resources;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;

namespace api.Controllers
{
    
    public class VideoController : ApiController
    {
        //public const string movieDir = @"E:\Git\BigMovieProject\StreamingVideo\movies\";
        //public const string streamDir = @"E:\Git\BigMovieProject\StreamingVideo\movies";
        public const string movieDir = @"E:\Torrent2\Movies";
        public const string streamDir = @"E:\Torrent2\Movies";
        [HttpGet, ActionName("Play")]
        public async Task<HttpResponseMessage> Play([FromUri]string id)
        {
            Debug.WriteLine("User connected from {0}", base.Request.RequestUri);
            //Getting movie from DB
            var movie = await Database.Get(id);
            if(movie != null)
            {
                //streaming content to client
                return Streaming.streamingContent(movie, base.Request.Headers.Range);
            }
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
        [HttpGet, ActionName("AllMovies")]
        public MovieData[] AllMovies()
        {
            return Database.allMovies;
        }
        [HttpGet,ActionName("GetMovie")]
        public async  Task<MovieData> GetMovie([FromUri]string id)
        {
            return await Database.Get(id);
        }
    }
}

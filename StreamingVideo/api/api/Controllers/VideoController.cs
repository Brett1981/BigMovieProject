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

namespace api.Controllers
{
    
    public class VideoController : ApiController
    {
        public const string movieDir = @"E:\Torrent2\Streaming\";
        [HttpGet, ActionName("Play")]
        public async Task<HttpResponseMessage> Play([FromUri]string id)
        {
            //Getting movie from DB
            var movie = await Database.Get(id);
            //streaming content to client
            return Streaming.streamingContent(movie, movieDir, base.Request.Headers.Range);
        }
        [HttpGet, ActionName("AllMovies")]
        public List<MovieData> AllMovies()
        {
            return Database.allMovies;
        }
    }
}

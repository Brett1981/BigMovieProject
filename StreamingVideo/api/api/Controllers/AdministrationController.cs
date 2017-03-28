using api.Resources;
using api.Resources.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data.Entity;
using api.Resources.Auth;
using api.Resources.Functions;
using api.Resources.Global;


namespace api.Controllers
{
    [EnableCors(origins: "http://31.15.224.24", headers: "*", methods: "GET, POST")]
    public class AdministrationController : ApiController
    {
        private MDBSQLEntities db = new MDBSQLEntities();

        //POST api/administration/refresh
        [HttpPost,ActionName("Refresh")]
        public async Task<IHttpActionResult> Refresh(Auth.User data)
        {
            if(data != null && data.unique_id != null )
            {
                var user = await Resources.Database.User.Get.ByGuid(data.unique_id);
                if(user != null && user.unique_id == data.unique_id)
                {
                    Resources.Database.Movie.Refresh.RefreshAndOrganize();
                    await History.Create("api", new History_API()
                    {
                        api_action = "Administration -> Refresh movie list requested from user " + user.username,
                        api_datetime = DateTime.Now,
                        api_type = "Refresh movie list"
                    });
                    return Ok(Resources.Database.AllMovies);
                }
            }
            return Unauthorized();

        }

        //POST api/administration/login
        [HttpPost,ActionName("Auth")]
        public async Task<IHttpActionResult> Auth([FromBody] Auth.Login data)
        {
            var user = await db.User_Info.Where(x => x.username == data.username).FirstOrDefaultAsync();
            if(user != null)
            {
                if (Functions.Decode.Base64toString(user.password) == Functions.Decode.Base64toString(data.password) 
                    && user.User_Groups.type == "administrator")
                {
                    user.last_logon = DateTime.Now;
                    await db.SaveChangesAsync();
                    await History.Create("api", new History_API()
                    {
                        api_action = "Administration -> User  '" + user.username + "' loged on",
                        api_datetime = DateTime.Now,
                        api_type = "Admin log on"
                    });
                    return Ok(new User_Info() {unique_id = user.unique_id, username = user.username });
                }
                return Unauthorized();
            }
            return NotFound();
        }

        //POST api/administration/ChangeMovieStatus
        [HttpPost, ActionName("ChangeMovieStatus")]
        public async Task<IHttpActionResult> ChangeMovieStatus([FromBody] Auth.AuthMovieEdit data)
        {
            if(data != null)
            {
                //x[2] is users guid 
                if (data.Movie != null && data.User != null && data.User.unique_id != null)
                {
                    var u = await Resources.Database.User.Get.ByGuid(data.User.unique_id);
                    if (u != null && u.User_Groups.access.Contains("w:true"))
                    {
                        Movie_Data movie;
                        if (data.Movie.enabled == true)
                        {
                            movie = await Resources.Database.Movie.Edit.MovieEnable(data.Movie.guid, MovieStatus.Enable);
                            if (movie != null && movie.enabled == true)
                            {
                                await History.Create("api", new History_API()
                                {
                                    api_action = "Administration -> Enable "+movie.Movie_Info.title + " movie",
                                    api_datetime = DateTime.Now,
                                    api_type = "Enable movie"
                                });
                                return Ok("Movie " + movie.Movie_Info.title + " is enabled!");
                            }
                        }
                        else if (data.Movie.enabled == false) 
                        {
                            movie = await Resources.Database.Movie.Edit.MovieEnable(data.Movie.guid, MovieStatus.Disable);
                            if (movie != null && movie.enabled == false)
                            {
                                await History.Create("api", new History_API()
                                {
                                    api_action = "Administration -> Disable " + movie.Movie_Info.title + " movie",
                                    api_datetime = DateTime.Now,
                                    api_type = "Disable movie"
                                });
                                return Ok("Movie " + movie.Movie_Info.title + " is disabled!");
                            }
                        }
                        return Conflict();
                    }
                    return Unauthorized();
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpPost,ActionName("Init")]
        public async Task<IHttpActionResult> Init(Auth.User data)
        {
            if(data != null && data.unique_id != null)
            {
                var u = await Resources.Database.User.Get.ByGuid(data.unique_id);
                if(u == null)
                {
                    return Unauthorized();
                }
                else
                {
                    if(u.User_Groups.type == "administrator")
                    {
                        return Ok(new CustomClasses.API.Data()
                        {
                            users = await Resources.Database.User.Get.AllUsersData(),
                            disks = MovieGlobal.GlobalMovieDisksList != null ? MovieGlobal.GlobalMovieDisksList : null,
                            movies = Resources.Database.AllMovies != null ? Resources.Database.AllMovies : new List<Movie_Data>()
                        });
                    }
                    return Unauthorized();
                }
                
                
            }
            return BadRequest();
        }

        [HttpPost, ActionName("EditMovie")]
        public async Task<IHttpActionResult> EditMovie(CustomClasses.API.Edit data)
        {
            if(data != null)
            {
                if(data.auth != null && data.auth.unique_id.Length > 0 && data.auth.username.Length > 0)
                {
                    var u = await Resources.Database.User.Get.ByGuid(data.auth.unique_id);
                    if( u != null 
                        && u.User_Groups.type == "administrator" 
                        && u.unique_id == data.auth.unique_id 
                        && data.auth.username == u.username
                        )
                    {
                        if(data.movie.guid.Length > 0)
                        {
                            var m = await Resources.Database.Movie.Get.ByGuidAndChangeCounter(data.movie.guid,false);
                            if(m != null)
                            {
                                var mdb = await db.Movie_Data.Where(x => x.guid == m.guid).FirstOrDefaultAsync();
                                if(mdb != null && mdb != data.movie)
                                {
                                    mdb.Movie_Info = data.movie.Movie_Info;
                                    await db.SaveChangesAsync();
                                    var mdbEdited = await db.Movie_Data.Where(x => x.guid == m.guid).FirstOrDefaultAsync();
                                    if(mdbEdited == data.movie)
                                    {
                                        return Ok("Movie Edited");
                                    }
                                    return Ok("Movie not edited");
                                }
                            }
                        }
                        return BadRequest("Movie id is not set");
                    }
                }
                return Unauthorized();
            }
            return BadRequest();
        }
    }
}

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
using api.Properties;

namespace api.Controllers
{
    [EnableCors(origins: "http://31.15.224.24", headers: "*", methods: "GET, POST")]
    public class AdministrationController : ApiController
    {

        //POST api/Administration/refresh
        [HttpPost,ActionName("Refresh")]
        public async Task<IHttpActionResult> Refresh(Auth.User data)
        {
            if(data != null && data.unique_id != null )
            {
                var user = await Resources.Database.User.Get.ByGuid(data.unique_id);
                if(user != null && user.unique_id == data.unique_id)
                {
                    Resources.Database.Movie.Refresh.RefreshAndOrganize();
                    await History.Create(History.Type.API, new History_API()
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

        //POST api/Administration/login
        [HttpPost,ActionName("Auth")]
        public async Task<IHttpActionResult> Auth([FromBody] Auth.Login data)
        {
            var user = await Resources.Database.User.Get.ByUsername(data.username);
            if(user != null)
            {
                if (Functions.Decode.Base64toString(user.password) == Functions.Decode.Base64toString(data.password) 
                    && await Resources.Database.User.Check.IsAdmin(data))
                {
                    await Resources.Database.User.Set.LastLogon(user);
                    await History.Create(History.Type.API, new History_API()
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

        //POST api/Administration/ChangeMovieStatus
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
                                await History.Create(History.Type.API, new History_API()
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
                                await History.Create(History.Type.API, new History_API()
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

        //POST: api/Administration/Init
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
                    if(await Resources.Database.User.Check.IsAdmin(data))
                    {
                        return Ok(new CustomClasses.API.Data()
                        {
                            users = await Resources.Database.User.Get.AllUsersData(),
                            disks = Global.GlobalMovieDisksList ?? null,
                            movies = Resources.Database.AllMovies ?? new List<Movie_Data>(),
                            settings = Global.GlobalServerSettings ?? null,
                            apiHistory = await History.Get.API()
                        });
                    }
                    return Unauthorized();
                }
                
                
            }
            return BadRequest();
        }

        //POST: api/Administration/Edit
        [HttpPost, ActionName("Edit")]
        public async Task<IHttpActionResult> Edit(CustomClasses.API.Edit data)
        {
            if (await Resources.Database.User.Check.IsAdmin(data.auth))
            {
                if (data != null)
                {
                    if (data.auth != null && data.auth.unique_id.Length > 0 && data.auth.username.Length > 0)
                    {
                        var u = await Resources.Database.User.Get.ByGuid(data.auth.unique_id);
                        if (u != null
                            && u.User_Groups.type == "administrator"
                            && u.unique_id == data.auth.unique_id
                            && data.auth.username == u.username
                            )
                        {
                            List<object> tasks = new List<object>();
                            if (data.api.disks != null || data.api.settings != null)
                            {
                                //Edit settings
                                if (await Resources.Settings.Edit.All(data.api))
                                {
                                    Global.GlobalMovieDisksList = await Resources.Settings.Get.ToObject(Resources.Settings.Type.Disks);
                                    Global.GlobalServerSettings = await Resources.Settings.Get.ToObject(Resources.Settings.Type.Settings);
                                    tasks.Add(data.api);
                                }
                            }
                            if (data.movie != null)
                            {
                                //edit movie
                                if (await Resources.Database.Movie.Edit.Movie(data.movie))
                                {
                                    tasks.Add(data.movie);
                                }
                            }
                            if (data.user != null)
                            {
                                //edit user data
                                if (await Resources.Database.User.Edit.Data(data.user))
                                {
                                    tasks.Add(data.user);
                                }
                            }


                            if (tasks.Count > 0)
                            {
                                return Ok(tasks);
                            }
                        }
                    }
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        //POST: api/Administration/GetAllUsers
        [HttpPost, ActionName("GetAllUsers")]
        public async Task<IHttpActionResult> GetAllUsers(Auth.User data)
        {
            if (await Resources.Database.User.Check.IsAdmin(data))
            {
                if (data != null && data.unique_id != null)
                {
                    var user = await Resources.Database.User.Get.ByGuid(data.unique_id);
                    if (user != null && user.username == data.username)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Administration -> Requesting user list from DB, user: " + user.username,
                            api_datetime = DateTime.Now,
                            api_type = "Request new user list"
                        });
                        return Ok(await Resources.Database.User.Get.AllUsersData());
                    }
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        //POST api/Administration/NewUser
        [HttpPost,ActionName("NewUser")]
        public async Task<IHttpActionResult> NewUser(CustomClasses.API.Edit data)
        {
            if(await Resources.Database.User.Check.IsAdmin(data.auth))
            {
                var user = await Resources.Database.User.Get.ByUsername(data.user.username);
                if (user == null)
                {
                    if (data.user.password != null && data.user.email != null && data.groups.Id > 0)
                    {
                        return Ok(await Resources.Database.User.Create.New(data.user, data.groups));
                    }
                    return BadRequest("not enough paramaters were set to create a user");
                }
                await History.Create(History.Type.API, new History_API()
                {
                    api_action = "User was not created",
                    api_type = "Error creating new user"
                });
                return BadRequest();
            }
            return Unauthorized();
        }

        //POST api/Administration/RemoveUser
        [HttpPost, ActionName("RemoveUser")]
        public async Task<IHttpActionResult> RemoveUser(CustomClasses.API.Edit data)
        {
            if(await Resources.Database.User.Check.IsAdmin(data.auth))
            {
                var user = await Resources.Database.User.Get.ByGuid(data.user.unique_id);
                if (user != null)
                {
                    if (data.user.username == user.username)
                    {
                        return Ok(await Resources.Database.User.Remove.User(user));
                    }
                    return BadRequest("User was not found or something else went wrong!");
                }
                await History.Create(History.Type.API, new History_API()
                {
                    api_action = "User was not removed",
                    api_type = "Error at User.Remove.User"
                });
                
            }
            return Unauthorized();
            
        }

    }
}

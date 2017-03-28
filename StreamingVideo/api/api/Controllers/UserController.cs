using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using api;
using api.Resources;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Web.Http.Cors;
using api.Models;
using api.Resources.Auth;
using api.Resources.Functions;

namespace api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "GET, POST")]
    public class UserController : ApiController
    {
        
        private MDBSQLEntities db = new MDBSQLEntities();
        
        // GET: api/Users
        [HttpGet,ActionName("GetUsers")]
        public IQueryable<User_Info> GetUsers()
        {
            return db.User_Info;
        }

        //POST: api/Users/Login
        [HttpPost,ActionName("Login")]
        public async Task<IHttpActionResult> Login([FromBody] Auth.Login data)
        {
            var user = await db.User_Info.Where(x => x.username == data.username).FirstOrDefaultAsync();
            if(user != null)
            {
                if(user.username == data.username && user.password == data.password) {
                    user.last_logon = DateTime.Now; db.SaveChanges();
                    await History.Create("user", new History_User()
                    {
                        user_action = "Login authorization -> " + user.unique_id,
                        user_datetime = DateTime.Now,
                        user_id = user.unique_id,
                        user_movie = "",
                        user_type = "Authorized"
                    });
                    return Ok(new DatabaseUserModels() { user_id = user.unique_id });
                }
                else {
                    await History.Create("user", new History_User()
                    {
                        user_action = "Login authorization -> " + user.unique_id,
                        user_datetime = DateTime.Now,
                        user_id = user.unique_id,
                        user_movie = "",
                        user_type = "NotAuthorized"
                    });
                    return Unauthorized();
                }
            }
            else { return NotFound(); }
        }

        //POST: api/Users/Create
        [HttpPost, ActionName("Create")]
        public async Task<IHttpActionResult> Create([FromBody] Auth.User data)
        {
            var user = await db.User_Info.Where(x => x.username == data.username).FirstOrDefaultAsync();
            if(user == null)
            {
                if (data.password != null && data.email != null)
                {
                    user = new User_Info() { username = data.username, password = data.password, email = data.email };
                    if (data.image_url != null)
                    {
                        HttpClient client = new HttpClient();
                        //user.profile_image = await client.GetByteArrayAsync(data.image_url);
                    }
                    if (data.birthday != null) { try { user.birthday = Convert.ToDateTime(data.birthday); } catch { user.birthday = DateTime.Now; } }
                    if (data.display_name != null) { user.display_name = data.display_name; }
                    user.unique_id = api.Resources.Database.Movie.Create.Guid(data.username).ToString();
                    user.profile_created = DateTime.Now;
                    user.groupId = db.User_Groups.Where(x => x.type == "user").Select(x => x.Id).First();
                    db.User_Info.Add(user);
                    await db.SaveChangesAsync();
                    await History.Create("user", new History_User()
                    {
                        user_action = "User create -> " + user.unique_id,
                        user_datetime = DateTime.Now,
                        user_id = user.unique_id,
                        user_movie = "",
                        user_type = "UserCreationSuccess"
                    });
                    return Ok(db.User_Info.Where(x => x.unique_id == user.unique_id).FirstOrDefaultAsync());
                }
            }
            await History.Create("user", new History_User()
            {
                user_action = "User create -> " + user.unique_id,
                user_datetime = DateTime.Now,
                user_id = user.unique_id,
                user_movie = "",
                user_type = "UserCreationError"
            });
            return Unauthorized();
        }

        // GET: api/Users/5
        [HttpGet,ActionName("GetUser"),ResponseType(typeof(User_Info))]
        public async Task<IHttpActionResult> GetUser(string value)
        {
            User_Info user = await db.User_Info.Where(x => x.unique_id == value).FirstOrDefaultAsync();
            if (user == null)
            {
                await History.Create("user", new History_User()
                {
                    user_action = "User search -> " + value,
                    user_datetime = DateTime.Now,
                    user_id = user.unique_id,
                    user_movie = "",
                    user_type = "UserNotFound"
                });
                return NotFound();
            }
            await History.Create("user", new History_User()
            {
                user_action = "User search -> " + value,
                user_datetime = DateTime.Now,
                user_id = user.unique_id,
                user_movie = "",
                user_type = "UserFound"
            });
            return Ok(user);
        }

        [HttpGet,ActionName("Check")]
        public async Task<IHttpActionResult> Check(string value)
        {
            var user = await db.User_Info.Where(x => x.username == value).FirstOrDefaultAsync();
            if(user == null)
            {
                return Ok(user);
            }
            return Ok(new User_Info());
        }
        [HttpPost,ActionName("ChangeProfilePicture")]
        public async Task<IHttpActionResult> ChangeProfilePicture([FromBody] Auth.User data)
        { 
            try
            {
                string status = "BadRequest";
                if (data != null && (data.image_url != null || data.unique_id != null)) {status = await Resources.Database.User.Edit.UserPicture(data); }
                switch (status){

                    case "OK": {
                            await History.Create("user", new History_User()
                            {
                                user_action = "User changed profile picture -> " + data.unique_id,
                                user_datetime = DateTime.Now,
                                user_id = data.unique_id,
                                user_movie = "",
                                user_type = "Ok-UserChangeProfilePicture"
                            });
                            return Ok();
                        } 
                    case "NotAuthorized": {
                            await History.Create("user", new History_User()
                            {
                                user_action = "User change profile picture -> " + data.unique_id,
                                user_datetime = DateTime.Now,
                                user_id = data.unique_id,
                                user_movie = "",
                                user_type = "NotAuth-UserChangeProfilePicture"
                            });
                            return Unauthorized();
                        } 
                    case "Exception": {
                            await History.Create("user", new History_User()
                            {
                                user_action = "User change profile picture -> " + data.unique_id,
                                user_datetime = DateTime.Now,
                                user_id = data.unique_id,
                                user_movie = "",
                                user_type = "Exception-UserChangeProfilePicture"
                            });
                            return BadRequest();
                        }
                    case "BadRequest": {
                            await History.Create("user", new History_User()
                            {
                                user_action = "User change profile picture -> " + data.unique_id,
                                user_datetime = DateTime.Now,
                                user_id = data.unique_id,
                                user_movie = "",
                                user_type = "BadReq-UserChangeProfilePicture"
                            });
                            return BadRequest();
                        }
                }
                await History.Create("user", new History_User()
                {
                    user_action = "User change profile picture : " + data.unique_id,
                    user_datetime = DateTime.Now,
                    user_id = data.unique_id,
                    user_movie = "",
                    user_type = "BadReq-UserChangeProfilePicture"
                });
                return BadRequest();
            }
            catch(Exception ex)
            {
                await History.Create("api", new History_API() { api_action = "UserController --> ChangeProfilePicture" + ex.Message, api_datetime = DateTime.Now, api_type = "Exception"});
                Debug.WriteLine("Exception in UserController --> ChangeProfilePicture : {0}", ex.Message);
                return BadRequest();
            }
            
        }

        [HttpGet,ActionName("GetProfilePicture")]
        public async Task<IHttpActionResult> GetProfilePicture([FromUri] string value)
        {
            var user = await db.User_Info.Where(x => x.unique_id == value).FirstOrDefaultAsync();
            if(user != null)
            {
                await History.Create("user", new History_User()
                {
                    user_action = "User get profile picture : " + value,
                    user_datetime = DateTime.Now,
                    user_id = user.unique_id,
                    user_movie = "",
                    user_type = "RetrieveUserProfilePicture"
                });
                return Ok(Resources.Database.User.Get.UserImage(user.profile_image));
            }
            return NotFound();
        }

        [HttpGet,ActionName("GetUserHistory")]
        public async Task<IHttpActionResult> GetUserHistory([FromUri] string value)
        {
            return Ok(await db.History_User.OrderByDescending(x => x.user_datetime).Where(x => x.user_id == value).Take(20).ToListAsync());
        }
        // DELETE: api/Users/5
        [ResponseType(typeof(User_Info))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            var user = await db.User_Info.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.User_Info.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.User_Info.Count(e => e.Id == id) > 0;
        }
    }
}
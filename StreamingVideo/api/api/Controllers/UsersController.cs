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

namespace api.Controllers
{
    public class UsersController : ApiController
    {
        private MDBSQLEntities db = new MDBSQLEntities();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        //POST: api/Users/Login
        [HttpPost,ActionName("Login")]
        public async Task<IHttpActionResult> Login([FromBody] UserLibrary data)
        {
            var user = await db.Users.Where(x => x.username == data.username).FirstOrDefaultAsync();
            if(user != null)
            {
                if(user.username == data.username && user.password == data.password) { user.last_logon = DateTime.Now; db.SaveChanges(); return Ok(user); }
                else { return Unauthorized(); }
            }
            else { return NotFound(); }
        }
        //POST: api/Users/Create
        [HttpPost, ActionName("Create")]
        public async Task<IHttpActionResult> Create([FromBody] UserLibrary data)
        {
            var user = await db.Users.Where(x => x.username == data.username).FirstOrDefaultAsync();
            if(user == null)
            {
                if (data.password != null && data.user_email != null)
                {
                    user = new api.User() { username = data.username, password = data.password, user_email = data.user_email };
                    if (data.image_url != null)
                    {
                        HttpClient client = new HttpClient();
                        user.profile_image = await client.GetByteArrayAsync(data.image_url);
                    }
                    if (data.user_birthday != null) { try { user.user_birthday = Convert.ToDateTime(data.user_birthday); } catch { user.user_birthday = DateTime.Now; } }
                    if (data.user_display_name != null) { user.user_display_name = data.user_display_name; }
                    user.unique_id = api.Resources.Database.CreateGuid(data.username).ToString();
                    user.profile_created = DateTime.Now;
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    return Ok(db.Users.Where(x => x.unique_id == user.unique_id).FirstOrDefaultAsync());
                }
            }
            return Unauthorized();
        }
        
        // GET: api/Users/5
        [HttpGet,ActionName("GetUser"),ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(string guid)
        {
            User user = await db.Users.Where(x => x.unique_id == guid).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost,ActionName("ChangeProfilePicture")]
        public async Task<IHttpActionResult> ChangeProfilePicture([FromBody]UserLibrary data)
        { 
            try
            {
                string status = "BadRequest";
                if (data != null && (data.image_url != null || data.unique_id != null)) {status = await Resources.Database.ChangeUserPicture(data); }
                switch (status){

                    case "OK": { return Ok(); } 
                    case "NotAuthorized": { return Unauthorized(); } 
                    case "Exception": { return BadRequest(); }
                    case "BadRequest": { return BadRequest(); }
                }
                return BadRequest();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception in UserController --> ChangeProfilePicture : {0} -- {1}",ex.Message, ex.InnerException.InnerException);
                return BadRequest();
            }
            
        }
        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
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
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}
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
        public async Task<IHttpActionResult> ChangeProfilePicture([FromBody]string data)
        { 
            try
            {
                string status = "BadRequest";
                var user = JsonConvert.DeserializeObject<UserLibrary>(data);
                if (data != null) {status = await Resources.Database.ChangeUserPicture(user); }
                switch (status){

                    case "OK": { return Ok(); } 
                    case "NotAuthorized": { return Unauthorized(); } 
                    case "Exception": { return BadRequest(); } 
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
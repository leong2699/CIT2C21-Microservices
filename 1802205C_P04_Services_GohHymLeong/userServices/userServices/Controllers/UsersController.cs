using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using userServices.Models;

namespace userServices.Controllers
{
    public class UsersController : ApiController
    {
        private userServicesContext db = new userServicesContext();


        // GET: api/Users
        public IHttpActionResult GetUsers()
        {
            var usersRecord = from e in db.Users
                                 select new
                                 {
                                     UserID = e.UserID,
                                     Username = e.Username,
                                     Email = e.Email
                                 };
            return Ok(usersRecord);
        }

        // GET: api/Users
        public IQueryable<User> GetUsersInfo(string token)
        {
            User member = db.Users.Where(u => u.Token == token).FirstOrDefault();
            if (member != null)
            {
                return db.Users;
            }
            else
            {
                return null;
            }
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id, string token)
        {
            User user = db.Users.Find(id);
            if (user != null)
            {
                if (user.Token == token)
                {
                    return Ok(user);
                }
                else
                {
                    return null;
                }
            }

            return NotFound();
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserID)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                if (user.Token == token)
                {
                    var hashpassword = EasyEncryption.MD5.ComputeMD5Hash(user.Password);
                    user.Password = hashpassword;
                    db.SaveChanges();
                }
                else
                {
                    return null;
                }
              
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
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hashpassword = EasyEncryption.MD5.ComputeMD5Hash(user.Password);
            user.Password = hashpassword;

            User users = db.Users.Where(u => u.Username == user.Username).FirstOrDefault();


            if (users != null)
            {
                return Json(new { result = "Username already exists!" });
            }
            else
            {
                db.Users.Add(user);
                db.SaveChanges();
            }

            return CreatedAtRoute("DefaultApi", new { id = user.UserID }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id, string token)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Token == token)
            {

                db.Users.Remove(user);
                db.SaveChanges();
            }

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
            return db.Users.Count(e => e.UserID == id) > 0;
        }
    }

}
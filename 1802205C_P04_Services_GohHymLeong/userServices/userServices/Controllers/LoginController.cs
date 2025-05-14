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
    public class LoginController : ApiController
    {
        private userServicesContext db = new userServicesContext();

        // POST: api/Login
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hashpassword = EasyEncryption.MD5.ComputeMD5Hash(user.Password);
            user.Password = hashpassword;

            User selectedUser = db.Users.Where(u => u.Username == user.Username && u.Password == user.Password).FirstOrDefault();

            if (selectedUser != null)
            {
                selectedUser.Token = Guid.NewGuid().ToString();
                db.SaveChanges();
                return Json(new { result = selectedUser.Token, username = user.Username, userID = selectedUser.UserID });
            }
            else
            {
                return Json(new { result = "" });
            }
        }
    }
}
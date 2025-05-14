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
    public class AdminController : ApiController
    {
        private userServicesContext db = new userServicesContext();
        // POST: api/Admin
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User selectedUser = db.Users.Where(u => u.Token == user.Token).FirstOrDefault();

            if (selectedUser != null)
            {
                if(selectedUser.Role == "Admin")
                {
                    return Json(new { result ="Admin" });
                } else
                {
                    return Json(new { result = "" });
                }
            }
            else
            {
                return Json(new { result = "" });
            }
        }
    }
}
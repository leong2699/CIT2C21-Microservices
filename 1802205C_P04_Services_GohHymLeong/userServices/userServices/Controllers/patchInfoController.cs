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
    public class patchInfoController : ApiController
    {
        private userServicesContext db = new userServicesContext();

        [HttpPatch]
        public IHttpActionResult PatchCustomer([FromBody] Customer user, string token)
        {
            var customer = db.Users.FirstOrDefault(c => c.UserID == user.UserID);
            if (customer == null)
                return NotFound();
            else
            {
                if (customer.Token == token)
                {
                    customer.Email = user.Email;
                    customer.Address = user.Address;
                    customer.UnitNumber = user.UnitNumber;
                    db.SaveChanges();
                    return Json(new { result = "success" });
                }
                else
                {
                    return Json(new { result = "failed" });
                }
            }
        }

        public class Customer
        {
            public int UserID { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string UnitNumber { get; set; }
        }
    }
}
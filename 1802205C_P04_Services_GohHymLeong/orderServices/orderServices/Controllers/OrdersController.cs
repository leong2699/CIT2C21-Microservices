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
using orderServices.Models;

namespace orderServices.Controllers
{
    public class OrdersController : ApiController
    {
        private orderServicesContext db = new orderServicesContext();

        // GET: api/Orders?status={status}
        public HttpResponseMessage GetOrders(string status="All", string token = "")
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44316/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Users?token=" + token);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<User[]>();
                    readTask.Wait();

                    var users = readTask.Result;

                    foreach (var user in users)
                    {
                        if (user.Token == token)
                        {
                            switch (status.ToLower())
                            {
                                case "all":
                                    return Request.CreateResponse(HttpStatusCode.OK, db.Orders.ToList());
                                case "processing":
                                    return Request.CreateResponse(HttpStatusCode.OK, db.Orders.Where(e => e.Status.ToLower() == "processing").ToList());
                                case "delivering":
                                    return Request.CreateResponse(HttpStatusCode.OK, db.Orders.Where(e => e.Status.ToLower() == "delivering").ToList());
                                case "completed":
                                    return Request.CreateResponse(HttpStatusCode.OK, db.Orders.Where(e => e.Status.ToLower() == "completed").ToList());
                                default:
                                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Value for status must be All, Processing, Delivering or Completed." + status + "is invalid || Token might be invalid as well");
                            }
                        }
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Value for status must be All, Processing, Delivering or Completed." + status + "is invalid || Token might be invalid as well");
            }
           
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id, string token)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44316/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Users?token=" + token);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<User[]>();
                    readTask.Wait();

                    var users = readTask.Result;

                    foreach (var user in users)
                    {
                        if (user.Token == token)
                        {
                            return Ok(order);
                        }
                    }
                }

            }
            return Ok(order);

        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.OrderID)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44316/api/");
                    //HTTP GET
                    var responseTask = client.GetAsync("Users?token=" + token);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsAsync<User[]>();
                        readTask.Wait();

                        var users = readTask.Result;

                        foreach (var user in users)
                        {
                            if (user.Token == token && user.Role == "Admin")
                            {
                                db.SaveChanges();
                                return Json(new { result = "Successfully updated!" });
                            }
                        }
                    }

                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.OrderID }, order);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id, string token)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44316/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Users?token=" + token);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<User[]>();
                    readTask.Wait();

                    var users = readTask.Result;

                    foreach (var user in users)
                    {
                        if (user.Token == token && user.Role == "Admin")
                        {
                            db.Orders.Remove(order);
                            db.SaveChanges();
                            return Json(new { result = "Successfully updated!" });
                        }
                    }
                }
            }

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderID == id) > 0;
        }
    }

    internal class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string Address { get; set; }
        public string UnitNumber { get; set; }
    }
}
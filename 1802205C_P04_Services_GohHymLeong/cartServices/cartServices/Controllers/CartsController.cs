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
using cartServices.Models;

namespace cartServices.Controllers
{
    public class CartsController : ApiController
    {
        private cartServicesContext db = new cartServicesContext();

        // GET: api/Carts
        public IHttpActionResult GetCarts()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Products");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Product[]>();
                    readTask.Wait();

                    var products = readTask.Result;

                    List<Cart> Carts = db.Carts.ToList();
                    List<Product> Products = products.ToList();

                    var cartRecord = from e in Carts
                                     join d in Products on e.ProductID equals d.ProductID into table1
                                     from d in table1.ToList()
                                     select new
                                     {
                                         CartID = e.CartID,
                                         UserID = e.UserID,
                                         ProductID = e.ProductID,
                                         Quantity = e.Quantity,
                                         Size = e.Size,
                                         Brand = d.Brand,
                                         ProductName = d.ProductName,
                                         Price = d.Price,
                                         ProductImage = d.ProductImage
                                    };
                    return Ok(cartRecord);
                }
            }
            return BadRequest();
        }

        // GET: api/Carts/5
        [ResponseType(typeof(Cart))]
        public IHttpActionResult GetCart(int id)
        {
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        // PUT: api/Carts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCart(int id, Cart cart, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cart.CartID)
            {
                return BadRequest();
            }

            db.Entry(cart).State = EntityState.Modified;

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
                            if (user.Token == token)
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
                if (!CartExists(id))
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

        // POST: api/Carts
        [ResponseType(typeof(Cart))]
        public IHttpActionResult PostCart(Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Carts.Add(cart);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = cart.CartID }, cart);
        }

        // DELETE: api/Carts/5
        [ResponseType(typeof(Cart))]
        public IHttpActionResult DeleteCart(int id, string token)
        {
            Cart cart = db.Carts.Find(id);
            if (cart == null)
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
                            db.Carts.Remove(cart);
                            db.SaveChanges();
                            return Json(new { result = "Successfully deleted!" });
                        }
                    }
                }
            }
            return Ok(cart);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CartExists(int id)
        {
            return db.Carts.Count(e => e.CartID == id) > 0;
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

    internal class Product
    {
        public string ProductID { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string Price { get; set; }
        public byte[] ProductImage { get; set; }
    }
}
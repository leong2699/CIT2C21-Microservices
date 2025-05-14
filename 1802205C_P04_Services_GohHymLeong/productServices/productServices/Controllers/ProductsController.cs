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
using productServices.Models;

namespace productServices.Controllers
{
    public class ProductsController : ApiController
    {
        private productServicesContext db = new productServicesContext();

        // GET: api/Products
        public HttpResponseMessage GetProducts(string sort = "All")
        {
            switch (sort.ToLower())
            {
                case "all":
                    return Request.CreateResponse(HttpStatusCode.OK, db.Products.ToList());
                case "ascendingprice":
                    return Request.CreateResponse(HttpStatusCode.OK, db.Products.OrderBy(e => e.Price).ToList());
                case "descendingprice":
                    return Request.CreateResponse(HttpStatusCode.OK, db.Products.OrderByDescending(e => e.Price).ToList());
                case "topseller":
                    return Request.CreateResponse(HttpStatusCode.OK, db.Products.OrderByDescending(e => e.Sold).ToList());
                default:
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "Value for sort must be Ascending, Descending, TopSeller or All. " + sort + " is invalid.");
            }

        }

        // GET: api/Products?ProductName={ProductName}
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetByProductName(string ProductName)
        {
            var product = db.Products.Where(e => e.ProductName.Contains(ProductName));
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductID)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

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
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

                            db.Products.Add(product);
                            db.SaveChanges();
                            return Json(new { result = "Successfully Added!" });
                        }
                    }
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id, string token)
        {
            Product product = db.Products.Find(id);
            if (product == null)
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

                            db.Products.Remove(product);
                            db.SaveChanges();
                            return Json(new { result = "Successfully deleted!" });
                        }
                    }
                }

            }

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductID == id) > 0;
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
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
using reviewServices.Models;

namespace reviewServices.Controllers
{
    public class ReviewsController : ApiController
    {
        private reviewServicesContext db = new reviewServicesContext();

        // GET: api/Reviews
        public IHttpActionResult GetReviews()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44316/api/");
                //HTTP GET
          
                var responseTask = client.GetAsync("Users");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<User[]>();
                    readTask.Wait();

                    var users = readTask.Result;

                    List<Review> Reviews = db.Reviews.ToList();
                    List<User> Users = users.ToList();

                    var joinedReviews = from e in Reviews
                                         join d in Users on e.UserID equals d.UserID into table1
                                         from d in table1.ToList()
                                         select new
                                         {
                                             ReviewID = e.ReviewID,
                                             ProductID = e.ProductID,
                                             UserID = d.UserID,
                                             Username = d.Username,
                                             Comment = e.Comment,
                                             CommentDate = e.CommentDate
                                         };
                    return Ok(joinedReviews);

                }

            }

            return BadRequest();
        }

        // GET: api/Reviews/5
        [ResponseType(typeof(Review))]
        public IHttpActionResult GetReview(int id)
        {
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        // PUT: api/Reviews/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutReview(int id, Review review, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != review.ReviewID)
            {
                return BadRequest();
            }

            db.Entry(review).State = EntityState.Modified;

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
                if (!ReviewExists(id))
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

        // POST: api/Reviews
        [ResponseType(typeof(Review))]
        public IHttpActionResult PostReview(Review review, string token)
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
                        if (user.Token == token)
                        {
                            db.Reviews.Add(review);
                            db.SaveChanges();
                            return Json(new { result = "Successfully updated!" });
                        }
                    }
                }
            }


            return CreatedAtRoute("DefaultApi", new { id = review.ReviewID }, review);
        }

        // DELETE: api/Reviews/5
        [ResponseType(typeof(Review))]
        public IHttpActionResult DeleteReview(int id, string token)
        {
            Review review = db.Reviews.Find(id);
            if (review == null)
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
                            db.Reviews.Remove(review);
                            db.SaveChanges();
                            return Json(new { result = "Successfully deleted!" });
                        }
                    }
                }
            }
            return Ok(review);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReviewExists(int id)
        {
            return db.Reviews.Count(e => e.ReviewID == id) > 0;
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
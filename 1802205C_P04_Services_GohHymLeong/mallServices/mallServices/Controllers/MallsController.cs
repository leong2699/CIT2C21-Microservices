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
using mallServices.Models;

namespace mallServices.Controllers
{
    public class MallsController : ApiController
    {
        private mallServicesContext db = new mallServicesContext();

        // GET: api/Malls
        public IQueryable<Mall> GetMalls()
        {
            return db.Malls;
        }

        // GET: api/Malls/5
        [ResponseType(typeof(Mall))]
        public IHttpActionResult GetMall(int id)
        {
            Mall mall = db.Malls.Find(id);
            if (mall == null)
            {
                return NotFound();
            }

            return Ok(mall);
        }

        // PUT: api/Malls/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMall(int id, Mall mall, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mall.MallID)
            {
                return BadRequest();
            }

            db.Entry(mall).State = EntityState.Modified;

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
                if (!MallExists(id))
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

        // POST: api/Malls
        [ResponseType(typeof(Mall))]
        public IHttpActionResult PostMall(Mall mall, string token)
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

                            db.Malls.Add(mall);
                            db.SaveChanges();
                            return Json(new { result = "Successfully Added!" });
                        }
                    }
                }
            }


            return CreatedAtRoute("DefaultApi", new { id = mall.MallID }, mall);
        }

        // DELETE: api/Malls/5
        [ResponseType(typeof(Mall))]
        public IHttpActionResult DeleteMall(int id, string token)
        {
            Mall mall = db.Malls.Find(id);
            if (mall == null)
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

                            db.Malls.Remove(mall);
                            db.SaveChanges();
                            return Json(new { result = "Successfully deleted!" });
                        }
                    }
                }

            }

            return Ok(mall);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MallExists(int id)
        {
            return db.Malls.Count(e => e.MallID == id) > 0;
        }

    }
}
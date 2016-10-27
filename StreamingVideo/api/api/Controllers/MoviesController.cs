using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace api.Controllers
{
    public class MoviesController : Controller
    {
        private static HttpClient client = new HttpClient();
        // GET: Movies
        public async Task<ActionResult> Index()
        {
            return View(JsonConvert.DeserializeObject<List<MovieData>>(await client.GetStringAsync("http://localhost:53851/api/video/allmovies")));
        }
        // GET: Movies/Edit/d4e06ba5-6a0a-98af-10cf-b597d49a7021
        public async Task<ActionResult> Edit(string guid)
        {
            return View(JsonConvert.DeserializeObject<MovieData>(await client.GetStringAsync("http://localhost:53851/api/video/getmovie?id=" + guid)));
        }
        // POST: Movies/Edit/d4e06ba5-6a0a-98af-10cf-b597d49a7021
        [HttpPost]
        public ActionResult Edit(string guid, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        // GET: Movies/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        
        

        // GET: Movies/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Movies/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using api.Resources;

namespace api.Controllers
{
    public class MoviesController : Controller
    {
        private static HttpClient client = new HttpClient();
        // GET: Movies
        public ActionResult Index()
        {
            return View(Database.AllMovies);
        }

        // GET: Movies/Edit/d4e06ba5-6a0a-98af-10cf-b597d49a7021
        public async Task<ActionResult> Edit(string guid)
        {
            return View(await Database.Movie.Get.ByGuid(guid));
        }
        // POST: Movies/Edit/d4e06ba5-6a0a-98af-10cf-b597d49a7021
        [HttpPost]
        public async Task<ActionResult> Edit(string guid, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return View(await Database.Movie.Edit.Form(guid, collection));
            }
            catch
            {
                return View(new Movie_Data());
            }
        }
        // GET: Movies/Details/5
        public async Task<ActionResult> Details(string guid)
        {
            return View(await Database.Movie.Get.ByGuid(guid));
        }


        // GET: Movies/Delete/5
        public async Task<ActionResult> Delete(string guid)
        {
            try
            {
                await Database.Movie.Remove.ByModel(await Database.Movie.Get.ByGuid(guid));
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
            
            
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

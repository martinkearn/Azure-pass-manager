using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APM.Web.Interfaces;

namespace APM.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly IApiRepository _apiRepository;

        public EventsController(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        // GET: Events
        public async Task<ActionResult> Index()
        {
            var owner = User.Identity.Name ?? "Anonymous";
            var eventsForOwner = await _apiRepository.GetEventsByOwner(owner);
            return View(eventsForOwner);
        }

        // GET: Events/Details/Hackference2017
        public async Task<ActionResult> Details(string eventName)
        {
            var evnt = await _apiRepository.GetEventByEventName(eventName);
            return View(evnt);
        }


        // GET: Events/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Events/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public FileContentResult Download(string contents, string fileName)
        {
            //string csv = "Charlie, Chaplin, Chuckles";
            contents = contents.TrimEnd(',');
            return File(new System.Text.UTF8Encoding().GetBytes(contents), "text/csv", $"{fileName}.csv");
        }
    }
}
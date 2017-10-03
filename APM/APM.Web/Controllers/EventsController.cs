using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APM.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace APM.Web.Controllers
{
#if (!DEBUG)
    [Authorize]
#endif

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
            var absoluteUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{evnt.Url}";
            ViewData["AbsoluteUrl"] = absoluteUrl;
            return View(evnt);
        }


        // GET: Events/Delete/Hackference2017
        public async Task<ActionResult> Delete(string eventName)
        {
            var evnt = await _apiRepository.GetEventByEventName(eventName);
            return View(evnt);
        }

        [HttpPost]
        // GET: Events/Delete/Hackference2017
        public async Task<ActionResult> Delete(string eventName, IFormCollection collection)
        {
            try
            {
                await _apiRepository.DeleteEventByEventName(eventName);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public FileContentResult Download(string contents, string fileName)
        {
            contents = contents.TrimEnd(',');
            return File(new System.Text.UTF8Encoding().GetBytes(contents), "text/csv", $"{fileName}.csv");
        }
    }
}
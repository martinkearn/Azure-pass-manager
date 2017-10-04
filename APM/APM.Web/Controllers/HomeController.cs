using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APM.Web.Models;
using APM.Web.Interfaces;
using Microsoft.AspNetCore.Http;

namespace APM.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiRepository _apiRepository;

        public HomeController(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Index(IFormCollection collection)
        {
            var eventName = collection["eventName"].ToString();

            var evnt = await _apiRepository.GetEventByEventName(eventName);

            if (evnt != null)
            {
                return RedirectToAction("Event", new { eventName = eventName });
            }
            else
            {
                ViewData["message"] = $"Could not find an event called {eventName}. Please check with your Microsoft representative that you got the name right with spacing included.";
                return View();
            }

        }

        public async Task<IActionResult> Event()
        {
            var eventName = RouteData.Values["eventname"];
            var eventNameValue = (eventName != null) ? RouteData.Values["eventname"].ToString() : string.Empty;

            if (eventNameValue != null)
            {
                var evnt = await _apiRepository.GetEventByEventName(eventNameValue);
                return View(evnt);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClaimCode(IFormCollection collection)
        {
            var eventName = collection["eventName"].ToString();

            if (eventName != null)
            {
                var code = await _apiRepository.ClaimCode(eventName);
                return View(code);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

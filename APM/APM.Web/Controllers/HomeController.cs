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
        private const string _existingCodeCookieKey = "apmexistingcode";

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

        public async Task<IActionResult> Event(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewData["Message"] = message;
                return View();
            }

            var eventName = RouteData.Values["eventname"];
            var eventNameValue = (eventName != null) ? RouteData.Values["eventname"].ToString() : string.Empty;

            if (eventNameValue != null)
            {
                var evnt = await _apiRepository.GetEventByEventName(eventNameValue);
                return View(evnt);
            }
            else
            {
                message = $"Could not find event {eventNameValue}. Please contact your Microsoft representative.";
                return RedirectToAction("Index", new { message = message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClaimCode(IFormCollection collection)
        {
            var eventName = collection["eventName"].ToString();

            //Check if the cookie is present (indicating this user has already had a code in the past 7 days for this event)
            var existingCode = Request.Cookies[_existingCodeCookieKey];
            if (existingCode != null) 
            {
                var message = $"Cheeky! ... You have already requested a code for the {eventName} on this machine and it was {existingCode}. Contact your Microsoft representative if you really need a second code.";
                return RedirectToAction("Event", new { message = message });
            }
            
            if (eventName != null)
            {
                var code = await _apiRepository.ClaimCode(eventName);

                if (code == null)
                {
                    var message = $"Could not get a code for {eventName}. We may have ranout of codes for this event. Contact your Microsoft representative.";
                    return RedirectToAction("Event", new { message = message });
                }
                else
                {
                    //Drop cookie to prevent accidental multiple codes requests
                    Response.Cookies.Append(
                        _existingCodeCookieKey,
                        code.PromoCode,
                        new CookieOptions()
                        {
                            Path = "/",
                            HttpOnly = false,
                            Secure = false,
                            Expires = DateTime.UtcNow.AddDays(7)
                        }
                    );

                    //return view
                    return View(code);
                }
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

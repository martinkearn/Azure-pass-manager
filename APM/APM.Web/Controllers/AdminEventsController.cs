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

    public class AdminEventsController : Controller
    {
        private readonly IApiRepository _apiRepository;

        public AdminEventsController(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        // GET: Events
        public async Task<ActionResult> Index(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewData["Message"] = message;
            }
            var eventsForOwner = await _apiRepository.GetEventsByOwner(CurrentUser());
            return View(eventsForOwner);
        }

        // GET: Events/Details/Hackference2017
        public async Task<ActionResult> Details(string eventName)
        {
            var evnt = await _apiRepository.GetEventByEventName(eventName);

            //check security
            if (evnt.Owner.ToLower() == CurrentUser().ToLower())
            {
                var absoluteUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{evnt.Url}";
                ViewData["AbsoluteUrl"] = absoluteUrl;
                return View(evnt);
            }
            else
            {
                var message = $"You cannot view events that you did not create. Please contact {evnt.Owner} who is the owner of this event.";
                return RedirectToAction("Index", new { message = message });
            }
        }


        // GET: Events/Delete/Hackference2017
        public async Task<ActionResult> Delete(string eventName)
        {
            var evnt = await _apiRepository.GetEventByEventName(eventName);

            //check security
            if (evnt.Owner.ToLower() == CurrentUser().ToLower())
            {
                return View(evnt);
            }
            else
            {
                var message = $"You cannot view events that you did not create. Please contact {evnt.Owner} who is the owner of this event.";
                return RedirectToAction("Index", new { message = message });
            }
        }

        [HttpPost]
        // GET: Events/Delete/Hackference2017
        public async Task<ActionResult> Delete(string eventName, IFormCollection collection)
        {
            var evnt = await _apiRepository.GetEventByEventName(eventName);

            //check security
            if (evnt.Owner.ToLower() == CurrentUser().ToLower())
            {
                try
                {
                    var isSucess = await _apiRepository.DeleteEventByEventName(eventName);

                    if (isSucess)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "There was a problem deleting the event.";
                        return View();
                    }
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                var message = $"You cannot view events that you did not create. Please contact {evnt.Owner} who is the owner of this event.";
                return RedirectToAction("Index", new { message = message });
            }
        }

        public FileContentResult Download(string contents, string fileName)
        {
            contents = contents.TrimEnd(',');
            return File(new System.Text.UTF8Encoding().GetBytes(contents), "text/csv", $"{fileName}.csv");
        }

        private string CurrentUser()
        {
            return User.Identity.Name ?? "Anonymous";
        }
    }
}
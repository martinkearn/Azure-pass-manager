using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APM.Web.Models;
using APM.Web.Interfaces;

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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APM.Web.Interfaces;

namespace APM.Web.Controllers
{
#if (!DEBUG)
    [Authorize]
#endif
    public class AdminCodeController : Controller
    {
        private readonly IApiRepository _apiRepository;

        public AdminCodeController(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        // GET: AdminCode
        public ActionResult Index()
        {
            //no reason to arrive here so redirect to events index action
            return RedirectToAction("Index", "AdminEvents");
        }

        // GET: AdminCode/Details/
        public async Task<ActionResult> Details(string eventName, string promoCode)
        {
            var code = await _apiRepository.GetCode(eventName, promoCode);

            if (code == null)
            {
                return RedirectToAction("Index", "AdminEvents");
            }
            else
            {
                return View(code);
            }
        }

        // GET: AdminCode/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminCode/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
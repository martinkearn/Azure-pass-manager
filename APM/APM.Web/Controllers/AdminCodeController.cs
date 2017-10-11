using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APM.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading;
using APM.Domain;

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

        // GET: AdminCode/Edit/
        public async Task<ActionResult> Edit(string eventName, string promoCode)
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

        // POST: AdminCode/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                //get code object
                var code = CastFormCollectionToCode(collection);
                if (code == null)
                {
                    ViewData["Message"] = "Please make sure all fields have valid values.";
                    return View();
                }

                //update
                var isSucess = await _apiRepository.UpdateCode(code);

                if (isSucess)
                {
                    //sleep the thread before redirecting because it takes a few seconds for the items to be added to storage.
                    Thread.Sleep(1000);

                    // redirect to details page
                    return RedirectToAction("Details", new { eventName = code.EventName, promoCode = code.PromoCode });
                }
                else
                {
                    ViewData["Message"] = "There was a problem storing the code.";
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        private Code CastFormCollectionToCode(IFormCollection collection)
        {
            var code = new Code()
            {
                PromoCode = collection["PromoCode"],
                Expiry = Convert.ToDateTime(collection["Expiry"]),
                Claimed = (collection["Claimed"] == bool.TrueString),
                EventName = collection["EventName"],
                Owner = collection["Owner"]
            };

            return code;
        }

    }
}
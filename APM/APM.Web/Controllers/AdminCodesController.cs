using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APM.Domain;
using System.IO;
using APM.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading;

namespace APM.Web.Controllers
{
#if (!DEBUG)
    [Authorize]
#endif
    public class AdminCodesController : Controller
    {
        private readonly IApiRepository _apiRepository;

        public AdminCodesController(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        // GET: Codes
        public ActionResult Create()
        {
            return View();
        }

        // POST: CodeBatch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                //get CodeBatch object
                var codeBatch = CastFormCollectionToCodeBatch(collection, Request.Form.Files.FirstOrDefault());
                if (codeBatch == null) {
                    ViewData["Message"] = "Please make sure all fields have valid values. This includes choosing a CSV file containing codes.";
                    return View();
                }

                //post
                var isSucess = await _apiRepository.StoreCodeBatch(codeBatch);

                if (isSucess)
                {
                    //sleep the thread before redirecting because it takes a few second for the items to be added to storage.
                    Thread.Sleep(2000);

                    // redirect to details page
                    return RedirectToAction("Details", "AdminEvents", new { eventName = codeBatch.EventName });
                }
                else
                {
                    ViewData["Message"] = "There was a problem storing the code batch. Check that the event name was unique and that the CSV is properly formatted";
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }

        private CodeBatch CastFormCollectionToCodeBatch(IFormCollection collection, IFormFile file)
        {
            //return null if there is no file attached
            if (file.Length == 0) return null;

            byte[] fileBytes = null;
            using (var fileStream = file.OpenReadStream())
            {
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
            }

            var codeBatch = new CodeBatch()
            {
                Expiry = Convert.ToDateTime(collection["Expiry"]),
                EventName = collection["EventName"],
                Owner = User.Identity.Name ?? "Anonymous",
                File = fileBytes
            };

            return codeBatch;
        }

    }
}
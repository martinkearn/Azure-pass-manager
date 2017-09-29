using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APM.Domain;
using System.IO;
using APM.Web.Interfaces;

namespace APM.Web.Controllers
{
    public class CodesController : Controller
    {
        private readonly IApiRepository _apiRepository;

        public CodesController(IApiRepository apiRepository)
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
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                //get CodeBatch object
                var codeBatch = CastFormCollectionToCodeBatch(collection, Request.Form.Files.FirstOrDefault());

                //post
                _apiRepository.StoreCodeBatch(codeBatch);

                return RedirectToAction("Index", "Events");
            }
            catch
            {
                return View();
            }
        }

        private CodeBatch CastFormCollectionToCodeBatch(IFormCollection collection, IFormFile file)
        {
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
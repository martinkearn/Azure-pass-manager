using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APM.Web.Controllers
{
    public class AdminCodeController : Controller
    {
        // GET: AdminCode
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdminCode/Details/
        public ActionResult Details(string id)
        {
            return View();
        }

        // GET: AdminCode/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminCode/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
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

        // GET: AdminCode/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminCode/Delete/5
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
    }
}
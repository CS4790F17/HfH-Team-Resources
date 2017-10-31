using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.Controllers
{
    public class WaiverController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

        // GET: Waiver
        public ActionResult Index()
        {
            return View(db.waivers.ToList());
        }

        // GET: Waiver/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Waiver waiver = db.waivers.Find(id);
            if (waiver == null)
            {
                return HttpNotFound();
            }
            return View(waiver);
        }

        // GET: Waiver/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Waiver/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,userId,signDate,emContFirstName,emContLastName,relation,emContHomePhone,emContWorkPhone")] Waiver waiver)
        {
            if (ModelState.IsValid)
            {
                db.waivers.Add(waiver);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(waiver);
        }

        // GET: Waiver/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Waiver waiver = db.waivers.Find(id);
            if (waiver == null)
            {
                return HttpNotFound();
            }
            return View(waiver);
        }

        // POST: Waiver/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,userId,signDate,emContFirstName,emContLastName,relation,emContHomePhone,emContWorkPhone")] Waiver waiver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(waiver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(waiver);
        }

        // GET: Waiver/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Waiver waiver = db.waivers.Find(id);
            if (waiver == null)
            {
                return HttpNotFound();
            }
            return View(waiver);
        }

        // POST: Waiver/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Waiver waiver = db.waivers.Find(id);
            db.waivers.Remove(waiver);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

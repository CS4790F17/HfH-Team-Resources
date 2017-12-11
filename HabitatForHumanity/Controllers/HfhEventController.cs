﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HabitatForHumanity.Models;
using HabitatForHumanity.ViewModels;

namespace HabitatForHumanity.Controllers
{
    public class HfhEventController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

        public ActionResult ManageEvent(int? id)
        {
            ManageEventVM vm = new ManageEventVM();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReturnStatus rs = Repository.GetManageEventVmById((int)id);
            if (rs.errorCode == ReturnStatus.ALL_CLEAR)
            {
                vm = (ManageEventVM)rs.data;
            }
            else
            {
                return RedirectToAction("Index");//TODO:
            }
       
            return View(vm);
            
        }
        // GET: HfhEvent
        public ActionResult Index()
        {
            return View(db.hfhEvents.ToList());
        }

        // GET: HfhEvent/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HfhEvent hfhEvent = db.hfhEvents.Find(id);
            if (hfhEvent == null)
            {
                return HttpNotFound();
            }
            return View(hfhEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProjectsToEvent(List<EventAddRemoveProjectVM> vmList)
        {
            ReturnStatus rs = Repository.AddProjectsToEvent(vmList);
            return RedirectToAction("ManageEvent", new { id = vmList.First().hfhEventId });
        }
        // GET: HfhEvent/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult RemoveEventProject(EventAddRemoveProjectVM vm)
        {
            ReturnStatus rs = Repository.RemoveEventProject(vm);
            if(rs.errorCode == ReturnStatus.ALL_CLEAR)
            {
                // notify project removed
            }
            else
            {
                // notify removal failed
            }
            return RedirectToAction("ManageEvent", new { id = vm.hfhEventId });
        }

        // POST: HfhEvent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name,description,eventPartner,startDate,endDate")] HfhEvent hfhEvent)
        {
            if (ModelState.IsValid)
            {
                db.hfhEvents.Add(hfhEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hfhEvent);
        }

        // GET: HfhEvent/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HfhEvent hfhEvent = db.hfhEvents.Find(id);
            if (hfhEvent == null)
            {
                return HttpNotFound();
            }
            return View(hfhEvent);
        }

        // POST: HfhEvent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,description,eventPartner,startDate,endDate")] HfhEvent hfhEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hfhEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hfhEvent);
        }

        // GET: HfhEvent/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HfhEvent hfhEvent = db.hfhEvents.Find(id);
            if (hfhEvent == null)
            {
                return HttpNotFound();
            }
            return View(hfhEvent);
        }

        // POST: HfhEvent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HfhEvent hfhEvent = db.hfhEvents.Find(id);
            db.hfhEvents.Remove(hfhEvent);
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
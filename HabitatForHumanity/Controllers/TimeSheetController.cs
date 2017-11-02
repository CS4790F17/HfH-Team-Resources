using System;
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
    public class TimeSheetController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

        // GET: TimeSheet
        public ActionResult Index()
        {
            return View(db.timeSheets.ToList());
        }

        // GET: TimeSheet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.timeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }

        // GET: TimeSheet/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: TimeSheet/Create

        public ActionResult PunchIn(int? userId)
        {
            int id = 2;
            if (userId != null)
            {
                id = (int)userId;
            }
            PunchInVM punchIn = new PunchInVM();
            punchIn = Repository.GetPunchInVM(id);
            return View("PunchIn", punchIn);

        }
        //public ActionResult PunchIn()
        //{
        //    PunchInVM punchIn = new PunchInVM();
        //    punchIn = Repository.GetPunchInVM(3);
        //    return View("PunchIn", punchIn);

        //}
        // GET: TimeSheet/Create
        public ActionResult PunchOut(int userId)
        {
            PunchOutVM punchOut = new PunchOutVM();
            punchOut = Repository.GetPunchClockVM(userId);
            if (punchOut.timeSheet == null)
            {
                PunchInVM punchIn = new PunchInVM();
                punchIn.userId = userId;
                punchIn.userName = punchOut.userName;
                punchIn.orgList = punchOut.orgList;
                punchIn.projectList = punchOut.projectList;
                return View("PunchIn");
            }
            return View("PunchOut", punchOut);
        }

        // POST: TimeSheet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PunchIn([Bind(Include = "userId,projectId")] PunchInVM punchInVM)
        {
            if (ModelState.IsValid)
            {
                Repository.PunchIn(punchInVM);
            
                return RedirectToAction("Index");
            }

            return View(punchInVM);
        }


        // POST: TimeSheet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,user_Id,project_Id,clockInTime,clockOutTime")] TimeSheet timeSheet)
        {
            if (ModelState.IsValid)
            {
                db.timeSheets.Add(timeSheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(timeSheet);
        }

        // GET: TimeSheet/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.timeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }

        // POST: TimeSheet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,user_Id,project_Id,clockInTime,clockOutTime")] TimeSheet timeSheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeSheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(timeSheet);
        }

        // GET: TimeSheet/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.timeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }

        // POST: TimeSheet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TimeSheet timeSheet = db.timeSheets.Find(id);
            db.timeSheets.Remove(timeSheet);
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

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
    public class ProjectController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

        // GET: Project
        public ActionResult Index()
        {
            return View(db.projects.ToList());
        }

        // GET: Project/Details/5
        public ActionResult Details(int? id)
        {
            int projectId = 0;
            projectId = (int)id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectVM projectVM = new ProjectVM();
            projectVM.project = Repository.GetProjectById(projectId);

            if (projectVM.project.status == 1)
            {
                projectVM._status = true;
            }
            else
            {
                projectVM._status = false;
            }

            if (projectVM == null)
            {
                return HttpNotFound();
            }
            return View(projectVM);
        }

        // GET: Project/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name,description,beginDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Project/Edit/5
        public ActionResult Edit(int? id)
        {
            int projectId = 0;
            projectId = (int)id;
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProjectVM projectVM = new ProjectVM();
            projectVM.project = Repository.GetProjectById(projectId);
            if (projectVM.project.status == 1)
            {
                projectVM._status = true;
            }
            else
            {
                projectVM._status = false;
            }

            if (projectVM == null)
            {
                return HttpNotFound();
            }
            return View(projectVM);
        }

        // POST: Project/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectVM projectVM)
        {
            if (ModelState.IsValid)
            {
                if (projectVM._status == true)
                {
                    projectVM.project.status = 1;
                }
                else
                {
                    projectVM.project.status = 0;
                }
                db.Entry(projectVM.project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projectVM.project);
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

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
using System.Text.RegularExpressions;

namespace HabitatForHumanity.Controllers
{
    [AdminFilter]
    [AuthorizationFilter]
    public class ProjectController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

       
        // GET: Project
        public ActionResult Index()
        {
            try
            {
                List<Project> temp = new List<Project>();
                temp = (List<Project>)Repository.GetAllProjects().data;
                List<User> userIds = new List<User>();
                List<ProjectIndexVM> index = new List<ProjectIndexVM>();

                foreach (Project project in temp)
                {
                    ProjectIndexVM projectIndexVM = new ProjectIndexVM();
                    projectIndexVM._name = project.name;
                    projectIndexVM._Id = project.Id;
                    projectIndexVM._beginDate = project.beginDate;
                    projectIndexVM._hoursLogged = (double)Repository.getTotalHoursLoggedIntoProject(project.Id).data;
                    userIds = (List<User>)TimeSheet.GetUsersbyTimeSheetFilters(project.Id, 0).data;
                    projectIndexVM._numVolunteers = userIds.Count();
                    index.Add(projectIndexVM);
                }

                return View(index);
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Project/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                int projectId = 0;
                projectId = (int)id;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Project project = new Project();
                ProjectVM projectVM = new ProjectVM();        
                project = (Project)Repository.GetProjectById(projectId).data;

                projectVM._hoursLogged = (double)Repository.getTotalHoursLoggedIntoProject(project.Id).data;
                projectVM._Id = project.Id;
                projectVM._name = project.name;
                projectVM._beginDate = project.beginDate;
                projectVM._description = project.description;
                if (project.status == 1)
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
            catch
            {
                return View("Error");
            }
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
            try
            {
                if (ModelState.IsValid)
                {
                    db.projects.Add(project);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(project);
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Project/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                int projectId = 0;
                projectId = (int)id;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Project project = new Project();
                ProjectVM projectVM = new ProjectVM();

                //TODO: add error checking
                project = (Project)Repository.GetProjectById(projectId).data;

                projectVM._Id = project.Id;
                projectVM._name = project.name;
                projectVM._beginDate = project.beginDate;
                projectVM._description = project.description;

                if (project.status == 1)
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
            catch
            {
                return View("Error");
            }
        }

        // POST: Project/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectVM projectVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Project project = new Project();

                    project.Id = projectVM._Id;
                    //Checking for name
                    if (projectVM._name != null)
                    {
                        project.name = projectVM._name;
                    }

                    //Checking for beginDate
                    if (projectVM._beginDate != null)
                    {
                        project.beginDate = projectVM._beginDate;
                    }

                    //Checking for description
                    if (projectVM._description != null)
                    {
                        project.description = projectVM._description;
                    }
                    else
                    {
                        project.description = "";
                    }

                    //Checking for status
                    if (projectVM._status == true)
                    {
                        project.status = 1;
                    }
                    else
                    {
                        project.status = 0;
                    }
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(projectVM);
            }
            catch
            {
                return View("Error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
            catch
            {
               //not sure what to put here
            }
        }
    }
}

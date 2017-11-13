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
    public class OrganizationController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

        // GET: Organization
        public ActionResult Index()
        {
            return View(db.organizations.ToList());
        }

        // GET: Organization/Details/5
        public ActionResult Details(int? id)
        {
            int orgId = 0;
            orgId = (int)id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            OrganizationVM organizationVM = new OrganizationVM();
            organizationVM.organization = Repository.GetOrganizationById(orgId);
            if (organizationVM.organization.status == 1)
            {
                organizationVM._status = true;
            }
            else
            {
                organizationVM._status = false;
            }

            if (organizationVM == null)
            {
                return HttpNotFound();
            }
            return View(organizationVM);
        }

        // GET: Organization/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Organization/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                db.organizations.Add(organization);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organization);
        }

        // GET: Organization/Edit/5
        public ActionResult Edit(int? id)
        {
            int orgId = 0;
            orgId = (int)id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            OrganizationVM organizationVM = new OrganizationVM();
            organizationVM.organization = Repository.GetOrganizationById(orgId);
            if (organizationVM.organization.status == 1)
            {
                organizationVM._status = true;
            }
            else
            {
                organizationVM._status = false;
            }

            if (organizationVM == null)
            {
                return HttpNotFound();
            }
            return View(organizationVM);
        }

        // POST: Organization/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,status")] OrganizationVM organizationVM)
        {
            if (ModelState.IsValid)
            {
                if (organizationVM._status == true)
                {
                    organizationVM.organization.status = 1;
                }
                else
                {
                    organizationVM.organization.status = 0;
                }
                db.Entry(organizationVM.organization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organizationVM.organization);
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

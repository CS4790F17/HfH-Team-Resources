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



        public ActionResult GetOrganizations()
        {
            return View();
        }




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

            Organization organization = new Organization();
            OrganizationVM organizationVM = new OrganizationVM();
            //TODO: add error checking
            organization = (Organization)Repository.GetOrganizationById(orgId).data;

            organizationVM._Id = organization.Id;
            organizationVM._name = organization.name;
            if (organization.status == 1)
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

            Organization organization = new Organization();
            OrganizationVM organizationVM = new OrganizationVM();

            ReturnStatus rs = Repository.GetOrganizationById(orgId);
            if(rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
                return View();
            }
            organization = (Organization)rs.data;

            organizationVM._Id = organization.Id;
            organizationVM._name = organization.name;
            if (organization.status == 1)
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
        public ActionResult Edit(OrganizationVM organizationVM)
        {
            if (ModelState.IsValid)
            {
                Organization organization = new Organization();

                organization.Id = organizationVM._Id;
                if (organizationVM._name != null)
                {
                    organization.name = organizationVM._name;
                }

                if (organizationVM._status == true)
                {
                    organization.status = 1;
                }
                else
                {
                    organization.status = 0;
                }
                db.Entry(organization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organizationVM);
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

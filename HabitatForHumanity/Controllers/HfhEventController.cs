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
    public class HfhEventController : Controller
    {
        public ActionResult ListHfhEvents()
        {     
            ReturnStatus rs = Repository.GetAllHfhEvents();
            List<HfhEvent> hfhEvents = (rs.errorCode==ReturnStatus.ALL_CLEAR) ? (List<HfhEvent>)rs.data : new List<HfhEvent>();
            return View(hfhEvents);
        }
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
                return RedirectToAction("ListHfhEvents");//TODO: manage errors
            }      
            return View(vm);       
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProjectsToEvent(List<EventAddRemoveProjectVM> vmList)
        {
            if(vmList != null && vmList.Count > 0)
            {
                ReturnStatus rs = Repository.AddProjectsToEvent(vmList);
                // TODO: manage bad results
                return RedirectToAction("ManageEvent", new { id = vmList.First().hfhEventId });
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

        }


        public ActionResult RemoveEventProject(EventAddRemoveProjectVM vm)
        {
            ReturnStatus rs = Repository.RemoveEventProject(vm);
            if(rs.errorCode == ReturnStatus.ALL_CLEAR)
            {
                // TODO: notify project removed
            }
            else
            {
                // TODO: notify removal failed
            }
            return RedirectToAction("ManageEvent", new { id = vm.hfhEventId });
        }

        // GET: HfhEvent/Create
        public ActionResult Create()
        {
            return View();
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
                ReturnStatus rs = Repository.CreateEvent(hfhEvent);
                if(rs.errorCode == ReturnStatus.ALL_CLEAR)
                {
                    return RedirectToAction("ListHfhEvents");
                }              
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
            ReturnStatus rs = Repository.GetHfhEventById((int)id);

            return (rs.errorCode == ReturnStatus.ALL_CLEAR) ? View((HfhEvent)rs.data) : View("_Error");
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
                ReturnStatus rs = Repository.EditHfhEvent(hfhEvent);
                if(rs.errorCode == ReturnStatus.ALL_CLEAR)
                {
                    return RedirectToAction("ListHfhEvents");
                }          
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
            ReturnStatus rs = Repository.GetHfhEventById((int)id);
            if(rs.errorCode == ReturnStatus.ALL_CLEAR)
            {
                return View((HfhEvent)rs.data);
            }
            else
            {
                return View("_Error");
            }
        }

        // POST: HfhEvent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReturnStatus rs = Repository.DeleteHfhEventById(id);
            if (rs.errorCode == ReturnStatus.ALL_CLEAR)
            {
                return RedirectToAction("ListHfhEvents");
            }
            return View("_Error");
        }
    }
}

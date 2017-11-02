using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HabitatForHumanity.ViewModels;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.Controllers
{
    public class HomeController : Controller
    {



        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Examples()
        {
            ToolKitExampleVM model = new ToolKitExampleVM();
            model.odd.createDropDownList(Repository.getAllOrganizations());
            model.pdd.createDropDownList(Repository.getAllProjects());

            model.projectTest = Project.getProjectByNameAndDate("test", "1/1/2081").name;
            model.projectTest2 = Project.getProjectByNameAndDate("test2", "1/1/2081").name;



            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
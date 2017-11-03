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
            model.odd.createDropDownList(Repository.GetAllOrganizations());
            model.pdd.createDropDownList(Repository.GetAllProjects());

            model.projectTest = Project.GetProjectByNameAndDate("test", "1/1/2081").name;
            model.projectTest2 = Project.GetProjectByNameAndDate("test2", "1/1/2081").name;

            //Repository.GetUserByName("test1", "LASTname");
            var temp = Repository.GetUsersByName("", "lastname");



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
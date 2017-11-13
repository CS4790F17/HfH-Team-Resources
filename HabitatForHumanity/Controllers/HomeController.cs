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

            var test = Repository.GetAllTimeSheetsByProjectId(1);
            Repository.GetAllTimeSheetsByVolunteer(1);
            var timetest = Repository.AddTimeSheetHours(test);
            var volunteerTest = Repository.GetAllTimeSheetsByVolunteer(6);
            var orgtest = Repository.GetAllTimeSheetsByOrganizationId(1);

            DateTime beginDate = DateTime.Parse("11/1/2017");
            DateTime endDate = DateTime.Parse("11/3/2017");

            var dateTest = Repository.GetAllTimeSheetsInDateRange(beginDate, endDate);

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
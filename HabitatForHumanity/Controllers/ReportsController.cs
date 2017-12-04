﻿using HabitatForHumanity.Models;
using HabitatForHumanity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabitatForHumanity.Controllers
{
    //[AdminFilter]
    //[AuthorizationFilter]
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectDemographics()//int period)
        {
            //List<ProjDemogReportVM> report = new List<ProjDemogReportVM>();
            List<List<ProjDemogReportVM>> monthsReports = new List<List<ProjDemogReportVM>>();
            ReturnStatus rs = Repository.GetProjectDemographicsReport(Project.YEARLY);
            if (rs.errorCode == ReturnStatus.ALL_CLEAR)
            {
                monthsReports = (List<List<ProjDemogReportVM>>)rs.data;
            }
            //List<List<ProjDemogReportVM>> monthsReports = new List<List<ProjDemogReportVM>>();
            //monthsReports.Add(report);
            //monthsReports.Add(report);
            return View(monthsReports);
            //            public static int MONTH = 1;
            //public static int QUARTER = 3;
            //public static int YEARLY = 12;
            //public static ReturnStatus GetProjectDemographicsReport(int period)
        }
    }
}
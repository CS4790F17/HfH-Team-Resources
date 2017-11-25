using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System.Drawing;
using HabitatForHumanity.ViewModels;
using HabitatForHumanity.Models;
using static HabitatForHumanity.Models.User;
using PagedList;

namespace HabitatForHumanity.Controllers
{
    public class AdminController : Controller
    {

        const int RecordsPerPage = 25;
        // GET: Admin dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Volunteers(VolunteerSearchModel vsm)
        {
            if (vsm.projects == null)
            {
                vsm = new VolunteerSearchModel();
                return View(vsm);
            }
            ReturnStatus rs = Repository.GetAllVolunteers(vsm.projectId, vsm.orgId);

            if (rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
                return View(vsm);
            }
            
            List<UsersVM> allVols = (List<UsersVM>)rs.data;
            List<UsersVM> filteredVols = new List<UsersVM>();

            if (!string.IsNullOrEmpty(vsm.queryString) || vsm.Page.HasValue)
            {
                foreach (UsersVM u in allVols)
                {
                    if (u != null && vsm.queryString != null)
                    {
                        if (u.email.ToLower().Contains(vsm.queryString.ToLower()) || u.volunteerName.ToLower().Contains(vsm.queryString.ToLower()))
                        {
                            filteredVols.Add(u);
                        }
                    }
                }
                var pageIndex = vsm.Page ?? 1;
                vsm.SearchResults = filteredVols.ToPagedList(pageIndex, RecordsPerPage);
            }
            else
            {                 
                var pageIndex = vsm.Page ?? 1;
                vsm.SearchResults = allVols.ToPagedList(pageIndex, RecordsPerPage);
            }           
            return View(vsm);
        }

        public ActionResult TimeCards(TimeCardSearchModel tsm)
        {
            //StaticPagedList<TimeCardVM> pagedCards = Repository.GetTimeCardPageWithFilter(
            //        tsm.Page, 0, tsm.orgId, tsm.projId, tsm.rangeStart, tsm.rangeEnd, tsm.queryString);
            ReturnStatus rs = Repository.GetTimeCardPageWithFilter(
                    tsm.Page, 0, tsm.orgId, tsm.projId, tsm.rangeStart, tsm.rangeEnd, tsm.queryString);
            var pageIndex = tsm.Page ?? 1;
            List<TimeCardVM> pagedCards = (List<TimeCardVM>)rs.data;
            tsm.SearchResults = pagedCards.ToPagedList(pageIndex, RecordsPerPage);
            tsm.Page = tsm.SearchResults.PageNumber;
            return View(tsm);

            //ReturnStatus rs = Repository.GetTimeCardsByFilters(tsm.orgId, tsm.projId, tsm.rangeStart, tsm.rangeEnd);
            //if(rs.errorCode != 0)
            //{
            //    ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
            //    return View(tsm);
            //}
            //else
            //{
            //    List<TimeCardVM> allVols = (List<TimeCardVM>)rs.data;
            //    List<TimeCardVM> filteredVols = new List<TimeCardVM>();

            //    if (!string.IsNullOrEmpty(tsm.queryString))// || tsm.Page.HasValue)
            //    {
                   
            //        foreach (TimeCardVM t in allVols)
            //        {
            //            if (t != null && !string.IsNullOrEmpty(tsm.queryString))
            //            {
            //                if (t.volName.ToLower().Contains(tsm.queryString.ToLower()))
            //                {
            //                    filteredVols.Add(t);
            //                }
            //            }
            //        }
            //        var pageIndex = tsm.Page ?? 1;
            //        tsm.SearchResults = filteredVols.ToPagedList(pageIndex, RecordsPerPage);
            //    }
            //    else
            //    {
            //        var pageIndex = tsm.Page ?? 1;
            //        tsm.SearchResults = allVols.ToPagedList(pageIndex, RecordsPerPage);
            //    }
            //}
            

            //return View(tsm);
        }

        public ActionResult EditTimeCard(int id)
        {
            TimeCardVM card = new TimeCardVM();
            ReturnStatus rs = Repository.GetTimeSheetById(id);
            if (rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
                return View();
            }
            TimeSheet t = (TimeSheet)rs.data;
            card.timeId = t.Id;
            card.userId = t.user_Id;
            card.projId = t.project_Id;
            card.orgId = t.org_Id;
            card.inTime = t.clockInTime;
            card.outTime = t.clockOutTime;
            card.orgName = "test";
            card.projName = "test";
            card.volName = "test";

            TimeSpan span = t.clockOutTime.Subtract(t.clockInTime);
            card.elapsedHrs = span.Minutes / 60.0;             
            
            return PartialView("_EditTimeCard", card);
        }

        [HttpPost]
        //public JsonResult EditTimeCard(TimeCardVM card)
        public ActionResult EditTimeCard(TimeCardVM card)
        {
            ReturnStatus rs = Repository.EditTimeCard(card);
            if (rs.errorCode != 0)
            {
                return RedirectToAction("Dashboard");
            }
            //return Json(new {
            //    timeId = card.timeId,
            //    userId = card.userId,
            //    projId = card.projId,
            //    inTime = card.inTime,
            //    outTime = card.outTime,
            //    orgName = card.orgName,
            //    projName = card.projName,
            //    volName = "holy canoli",//card.volName,
            //    elapsedHrs = card.elapsedHrs
            //    }, JsonRequestBehavior.AllowGet);

            return RedirectToAction("Timecards");
        }


        public ActionResult GetHoursChartBy(string period)
        {
            #region Build Month Chart
            ChartVM chartVM = new ChartVM();
            Highcharts columnChart = new Highcharts("columnchart");

            columnChart.InitChart(new Chart()
            {
                Type = DotNet.Highcharts.Enums.ChartTypes.Column,
                BackgroundColor = new BackColorOrGradient(System.Drawing.Color.AliceBlue),
                Style = "fontWeight: 'bold', fontSize: '17px'",
                BorderColor = System.Drawing.Color.LightBlue,
                BorderRadius = 0,
                BorderWidth = 2

            });

            columnChart.SetTitle(new Title()
            {
                //Text = "Volunteer Hours"
                Text = chartVM._title
            });

            columnChart.SetSubtitle(new Subtitle()
            {
                //Text = "Subtitle here"
                Text = chartVM._subtitle
            });

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "X axis stuff", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartVM._categories
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Hours",
                    Style = "fontWeight: 'bold', fontSize: '17px'"
                },
                ShowFirstLabel = true,
                ShowLastLabel = true,
                Min = 0
            });

            columnChart.SetLegend(new Legend
            {
                Enabled = true,
                BorderColor = System.Drawing.Color.CornflowerBlue,
                BorderRadius = 6,
                BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFADD8E6"))
            });


            columnChart.SetSeries(new Series[]
            //columnChart.SetSeries(new Series[] = chartVM._series;
            {

                new Series
                {

                    Name = chartVM._series[0]._name,
                    Data = new Data(chartVM._series[0]._data)
                },
                new Series
                {

                   Name = chartVM._series[1]._name,
                   Data = new Data(chartVM._series[1]._data)
                },
                new Series
                {
                   Name = chartVM._series[2]._name,
                   Data = new Data(chartVM._series[2]._data)
                }
            }
            );


            #endregion

            return PartialView("_HoursMonthChart", columnChart);

        }

        public ActionResult GetHoursDemogPieBy(string gender)
        {
            #region Build Demographics Pie
            /* gender options from javascript radios       
            All, M, F, O */
            ReturnStatus st = new ReturnStatus();
            st.data = new List<User.Demog>();
            try
            {
                st = Repository.GetDemographicsForPie(gender);

                Demog[] demogs = ((List<Demog>)st.data).ToArray();
                object[] outer = new object[demogs.Length];
                for (int i = 0; i < demogs.Length; i++)
                {
                    outer[i] = new object[] { demogs[i].ageBracket, demogs[i].numPeople };
                }


                Highcharts chart = new Highcharts("chart")
                    .InitChart(new Chart { PlotShadow = false })
                    .SetTitle(new Title { Text = "" })
                    .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage.toFixed(1) +' %'; }" })
                    .SetPlotOptions(new PlotOptions
                    {
                        Pie = new PlotOptionsPie
                        {
                            AllowPointSelect = true,
                            Cursor = Cursors.Pointer,
                            DataLabels = new PlotOptionsPieDataLabels
                            {
                                Color = ColorTranslator.FromHtml("#000000"),
                                ConnectorColor = ColorTranslator.FromHtml("#000000"),
                                Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage.toFixed(1) +' %'; }"
                            }
                        }
                    })
                    .SetSeries(
                         new Series
                         {
                             Type = ChartTypes.Pie,
                             Name = "Male",
                             Data = new Data(outer)
                         }
                   );
                return PartialView("_DemographicsPie", chart);
            }
            catch
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
                return null;
            }


            #endregion

        }

        public ActionResult GetBadPunches()
        {
            ReturnStatus badTimeSheets = Repository.GetBadTimeSheets();
            // List<TimeSheet> ts = Repository.GetBadTimeSheets();
            List<BadPunchVM> bp = new List<BadPunchVM>();

            if (badTimeSheets.errorCode != 0)
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
                return null;
            }
            ReturnStatus timesheetReturn = new ReturnStatus();
            timesheetReturn.data = new List<TimeSheet>();
          
            List<TimeSheet> ts = (List<TimeSheet>)timesheetReturn.data;
            foreach (TimeSheet t in ts)
            {
                try
                {
                    User user = (User)Repository.GetUser(t.user_Id).data;
                    string volName = "";

                    if (string.IsNullOrEmpty(user.firstName) && string.IsNullOrEmpty(user.lastName))
                    {
                        volName = user.emailAddress;
                    }
                    else if (string.IsNullOrEmpty(user.firstName))
                    {
                        volName += user.emailAddress + " ";
                    }
                    else if (string.IsNullOrEmpty(user.lastName))
                    {
                        volName += user.emailAddress;
                    }
                    else
                    {
                        volName += user.firstName + " " + user.lastName;
                    }

                    bp.Add(new BadPunchVM() { name = volName, strPunchDate = t.clockInTime.ToShortDateString() });
                }
                catch
                {
                    return null;
                }


            }


            return PartialView("_BadPunches", bp);
        }

        #region Manage Organization
        public ActionResult ViewOrganizations(OrganizationSearchModel model)
        {
            List<Organization> orgs = new List<Organization>();
            ReturnStatus st = new ReturnStatus();

            switch (model.statusChoice)
            {
                case 0:
                    st = Repository.GetOrganizationByNameSQL(model.queryString);
                    break;
                case 1: //active organizations
                    st = Repository.GetOrganizationSQL(model.queryString, 1);
                    break;
                case 2: //inactive organizations
                    st = Repository.GetOrganizationSQL(model.queryString, 0);
                    break;
            }
            var pageIndex = model.Page ?? 1;

            if (st.errorCode != 0)
            {
                //fill search results with empty list                              
                model.SearchResults = orgs.ToPagedList(pageIndex, RecordsPerPage);
            }

            orgs = (List<Organization>)st.data;               
            model.SearchResults = orgs.ToPagedList(pageIndex, RecordsPerPage);
            
            //tsm.SearchResults = filteredVols.ToPagedList(pageIndex, RecordsPerPage);
            return View(model);
        }

        [HttpGet]
        public ActionResult EditOrganization(int id)
        {
            ReturnStatus rs = Repository.GetOrganizationById(id);
            if(rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
                return View();
            }
            return PartialView("OrganizationPartialViews/_EditOrganization", (Organization)rs.data);
        }

        [HttpPost]
        public JsonResult EditOrganization(Organization org)
        {
            //save org
            Repository.EditOrganization(org);
            return Json(new { name = org.name, status = org.status, id = org.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ChangeOrganizationStatus(int id)
        {
            ReturnStatus st = Repository.GetOrganizationById(id);
            if (st.errorCode != 0)
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
            }
            else
            {
                ((Organization)st.data).status = 1 - ((Organization)st.data).status;
                Repository.EditOrganization((Organization)st.data);
            }
        }

        [HttpGet]
        public ActionResult AddOrganization()
        {
            return PartialView("OrganizationPartialViews/_AddOrganization");
        }

        [HttpPost]
        public ActionResult AddOrganization(String name)
        {
            Organization org = new Organization()
            {
                name = name,
                status = 1 //active by default
            };

            Repository.AddOrganization(org);
            return RedirectToAction("ViewOrganizations");
        }
        #endregion

        #region Manage Projects

        //Main view
        [HttpGet]
        public ActionResult ManageProjects(ProjectSearchModel model)
        {

            model.SearchResults = Repository.GetProjectPageWithFilter(model.Page, model.statusChoice, model.queryString);
            model.Page = model.SearchResults.PageNumber;

            return View(model);
        }

        [HttpGet]
        public ActionResult CreateProject()
        {
            return PartialView("ProjectPartialViews/_CreateProject");
        }

        [HttpPost]
        public ActionResult CreateProject([Bind(Include = "Id,name,description,beginDate")] Project proj)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProjectPartialViews/_CreateProject", proj);
            }
            proj.status = 0;
            Repository.AddProject(proj);
            return PartialView("ProjectPartialViews/_ProjectCreateSuccess");
        }


        public ActionResult ChangeProjectStatus(int id, int page, int statusChoice, string queryString)
        {
            ReturnStatus st = Repository.GetProjectById(id);
            if (st.errorCode == ReturnStatus.ALL_CLEAR)
            {
                ((Project)st.data).status = 1 - ((Project)st.data).status;
                Repository.EditProject((Project)st.data);
            }
            //once the status has been changed return a project list with filters
            //grabs the filters even though they hadn't been submitted
            StaticPagedList<Project> SearchResults = Repository.GetProjectPageWithFilter(page, statusChoice, queryString);
            //StaticPagedList<Project> SearchResults = GetProjectPage(page, "");
            return PartialView("ProjectPartialViews/_ProjectList", SearchResults);

        }

        [HttpGet]
        public ActionResult EditProject(int id)
        {
            ReturnStatus st = Repository.GetProjectById(id);
            if (st.errorCode == 0)
            {
                return PartialView("ProjectPartialViews/_EditProject", (Project)st.data);
            }
            //if no edit was found return create
            return CreateProject();
        }

        [HttpPost]
        public ActionResult EditProject([Bind(Include = "Id,name,description,beginDate")] Project proj)
        {
            //if model state isn't valid
            if (!ModelState.IsValid)
            {
                return PartialView("ProjectPartialViews/_EditProject", proj);
            }
            Repository.EditProject(proj);
            return PartialView("ProjectPartialViews/_ProjectCreateSuccess");
        }

        [HttpPost]
        public ActionResult ProjectSearch(int? Page, int statusChoice, string queryString)
        {
            ProjectSearchModel model = new ProjectSearchModel();
            model.Page = Page;
            model.statusChoice = statusChoice;
            model.queryString = queryString;
            return RedirectToAction("ManageProjects", model);
           // return PartialView("ProjectPartialViews/_ProjectList", Repository.GetProjectPageWithFilter(Page, statusChoice, queryString));
        }

 


        #endregion

    }
}
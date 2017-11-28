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
using System.Data.Entity;
using HabitatForHumanity.Controllers;

namespace HabitatForHumanity.Controllers
{
    public class AdminController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

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
            ReturnStatus rs = Repository.GetTimeCardPageWithFilter(tsm.Page, tsm.orgId, tsm.projId, tsm.rangeStart, tsm.rangeEnd, tsm.queryString);
            if(rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information.System is down.If problem persists, contact Support.";
                return View(tsm);
            }
            var pageIndex = tsm.Page ?? 1;
            List<TimeCardVM> pagedCards = (List<TimeCardVM>)rs.data;
            tsm.SearchResults = pagedCards.ToPagedList(pageIndex, RecordsPerPage);
            tsm.Page = tsm.SearchResults.PageNumber;
            return View(tsm);
        }

        public ActionResult EditTimeCard(int id)
        {
            ReturnStatus rs = Repository.GetTimeCardVM(id);
            if (rs.errorCode != ReturnStatus.ALL_CLEAR)
            {
                ViewBag.status = "Sorry, something went wrong while retrieving information. System is down. If problem persists, contact Support.";
                return View();
            }

            return PartialView("_EditTimeCard", (TimeCardVM)rs.data);
        }

        [HttpPost]
        public ActionResult EditTimeCard(TimeCardVM card)
        {
            TimeSpan span = card.outTime.Subtract(card.inTime);
            if (span.Hours > 24 || span.Minutes < 0)
            {
                // this doesn't work
                ViewBag.status = "Time can't be more than 24 hours or less than zero.";
                return PartialView("_EditTimeCard", card);
            }
            ReturnStatus rs = Repository.EditTimeCard(card);
            if (rs.errorCode != ReturnStatus.ALL_CLEAR)
            {
                ViewBag.status = "Failed to update time card, please try again later.";
                return PartialView("_EditTimeCard", card);
            }
            return RedirectToAction("Timecards");
        }


        public ActionResult GetHoursChartBy(string period)
        {
            if(period == null)
            {
                period = "Month";
            }
            #region Build Month Chart
            ReturnStatus chartRS = new ReturnStatus();
   
            if (period.Equals("Year"))
            {
                chartRS = Repository.GetHoursChartVMByYear();
            }
            else if (period.Equals("Week"))
            {
                chartRS = Repository.GetHoursChartVMByYear();
            }        
            else
            {
                // monthly
                chartRS = Repository.GetHoursChartVMByMonth();
            }
            
            ChartVM chartVM = (ChartVM)chartRS.data;
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

            //columnChart.SetSubtitle(new Subtitle()
            //{
            //    //Text = "Subtitle here"
            //    Text = chartVM._subtitle
            //});

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

        // GET: Admin/EditVolunteer
        public ActionResult EditVolunteer(int id)
        {
            ReturnStatus getUser = Repository.GetUser(id);
            ReturnStatus getTimeSheets = Repository.GetAllTimeSheetsByVolunteer(id);

            if (getUser.errorCode != 0)
            {
                ViewBag.status = "There was a problem getting that user. Please try again later";
                return RedirectToAction("Volunteers", "Admin");
            }
            else if (getTimeSheets.errorCode != 0)
            {
                ViewBag.status = "There was a problem getting that user's time sheets. Please try again later";
                return RedirectToAction("Volunteers", "Admin");
            }
            else
            {
                User user = new User();
                user = (User)getUser.data;

                UsersVM volunteer = new UsersVM();
                volunteer.userNumber = user.Id;
                // force all name to not be null for simple comparison in controller
                volunteer.volunteerName = user.firstName + " " + user.lastName;
                volunteer.email = user.emailAddress;
                volunteer.waiverExpiration = user.waiverSignDate.AddYears(1);
                if (volunteer.waiverExpiration > DateTime.Now)
                {
                    volunteer.waiverStatus = true;
                }
                else
                {
                    volunteer.waiverStatus = false;
                }
                volunteer.hoursToDate = (double)Repository.getTotalHoursWorkedByVolunteer(user.Id).data;

                List<TimeSheet> timeSheets = new List<TimeSheet>();
                timeSheets = (List<TimeSheet>)getTimeSheets.data;
                List<TimeCardVM> test = new List<TimeCardVM>();

                
                foreach (TimeSheet t in timeSheets)
                {
                    TimeCardVM temp = new TimeCardVM();
                    temp.timeId = t.Id;
                    temp.volName = volunteer.volunteerName;
                    ReturnStatus orgRS = Repository.GetOrganizationById(t.org_Id);
                    if (orgRS.errorCode == 0)
                    {
                        temp.orgName = ((Organization)orgRS.data).name;
                    }
                    else
                    {
                        ViewBag.status = "There was a problem getting the organization. Please try again later";
                        return RedirectToAction("Volunteers", "Admin");
                    }
                    ReturnStatus projRS = Repository.GetProjectById(t.project_Id);
                    if (projRS.errorCode == 0)
                    {
                        temp.projName = ((Project)projRS.data).name;
                    }
                    else
                    {
                        ViewBag.status = "There was a problem getting the project. Please try again later";
                        return RedirectToAction("Volunteers", "Admin");
                    }
                    temp.inTime = t.clockInTime;
                    temp.outTime = t.clockOutTime;

                    test.Add(temp);
                }
                volunteer.timeCardVM = test;
                return View(volunteer);
            }
        }

        // POST: Admin/EditVolunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVolunteer([Bind(Include = "userNumber, volunteerName, email")] UsersVM usersVM)
        {
            if (ModelState.IsValid)
            {
                ReturnStatus rs = Repository.GetUser(usersVM.userNumber);
                if (rs.errorCode != 0)
                {
                    ViewBag.status = "Sorry, the system is temporarily down. Please try again later.";
                    return View("EditVolunteer");
                }
                else
                {
                    User user = (User)rs.data;
                    user.Id = usersVM.userNumber;
                    String[] tempName = usersVM.volunteerName.Split(' ');
                    user.firstName = tempName[0];
                    if (tempName.Length > 1)
                    {
                        if (tempName[1] != null)
                        {
                            user.lastName = tempName[1];
                        }
                    }

                    user.emailAddress = usersVM.email;

                    ReturnStatus us = new ReturnStatus();
                    us = Repository.EditUser(user);
                    if (us.errorCode != 0)
                    {
                        ViewBag.status = "Sorry, the system is temporarily down. Please try again later.";
                        return View(usersVM);
                    }
                }
                return RedirectToAction("Volunteers");
            }
            return View(usersVM);
        }

        public ActionResult GetBadPunches()
        {   
            ReturnStatus rs = Repository.GetNumBadPunches();
            int numBadPunches = (rs.errorCode == ReturnStatus.ALL_CLEAR) ? (int)rs.data : 0;
            return PartialView("_BadPunches", numBadPunches);
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
            else
            {
                orgs = (List<Organization>)st.data;
                model.SearchResults = orgs.ToPagedList(pageIndex, RecordsPerPage);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult EditOrganization(int id)
        {
            ReturnStatus rs = Repository.GetOrganizationById(id);
            if (rs.errorCode != 0)
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
        public void ChangeOrganizationStatus(int id, int status)
        {
            ReturnStatus st = Repository.GetOrganizationById(id);

            if (st.errorCode == ReturnStatus.ALL_CLEAR)
            {
                ((Organization)st.data).status = status;
                Repository.EditOrganization((Organization)st.data);
            }
            else
            {
                ViewBag.status = "Error while attempting to change organization status.";
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
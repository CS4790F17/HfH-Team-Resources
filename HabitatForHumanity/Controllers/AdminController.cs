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

namespace HabitatForHumanity.Controllers
{
    public class AdminController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

        const int RecordsPerPage = 10;
        // GET: Admin dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Volunteers(VolunteerSearchModel vsm)
        {
            if(vsm.projects == null)
            {
                vsm = new VolunteerSearchModel();
                return View(vsm);
            }
            ReturnStatus rs = Repository.GetAllVolunteers(vsm.projectId,vsm.orgId);

            if (rs.errorCode == 0)
            {
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
            }
            else if(rs.errorCode == -1)
            {
                ViewBag.status = "Broke in repo";
            }
            else // bad returnStatus
            {
                ViewBag.status = "Broke in datalayer";
            }
            return View(vsm);
        }

        public ActionResult TimeCards(TimeCardSearchModel tsm)
        {
            int orgNum = 2;
            int projNum = 3;
            DateTime strt = Convert.ToDateTime("1/1/1950");
            DateTime end = Convert.ToDateTime("11/17/2017");
            ReturnStatus rs = Repository.GetTimeCardsByFilters(orgNum, projNum, strt, end);

            if (!string.IsNullOrEmpty(tsm.queryString) || tsm.Page.HasValue)
            {
                if (rs.errorCode == 0)
                {
                    List<TimeCardVM> allVols = (List<TimeCardVM>)rs.data;
                    List<TimeCardVM> filteredVols = new List<TimeCardVM>();
                    foreach (TimeCardVM t in allVols)
                    {
                        if (t != null && tsm.queryString != null)
                        {
                            if (t.volName.ToLower().Contains(tsm.queryString.ToLower()))
                            {
                                filteredVols.Add(t);
                            }
                        }
                    }

                    var pageIndex = tsm.Page ?? 1;
                    tsm.SearchResults = filteredVols.ToPagedList(pageIndex, RecordsPerPage);

                }
                else
                {
                    ViewBag.status = "We had trouble with that request, try again.";
                    return View(tsm);
                }

            }
            else
            {
                //if (rs.errorCode == 0)
                //{
                //    List<TimeCardVM> allVols = (List<TimeCardVM>)rs.data;
                //    List<TimeCardVM> filteredVols = new List<TimeCardVM>();
                //    foreach (TimeCardVM t in allVols)
                //    {
                //        if (t != null && tsm.queryString != null)
                //        {
                //            if (t.volName.ToLower().Contains(tsm.queryString.ToLower()))
                //            {
                //                filteredVols.Add(t);
                //            }
                //        }
                //    }

                //    var pageIndex = tsm.Page ?? 1;
                //    tsm.SearchResults = filteredVols.ToPagedList(pageIndex, RecordsPerPage);

                //}
                //else
                //{
                //    ViewBag.status = "We had trouble with that request, try again.";
                //    return View(tsm);
                //}
            }

            return View(tsm);
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

                double hours = 0.0;
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
                    TimeSpan elapsedHrs = t.clockOutTime.Subtract(t.clockInTime);
                    hours = Math.Round(elapsedHrs.TotalHours, 2, MidpointRounding.AwayFromZero);
                    temp.elapsedHrs = hours;

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
                        return View("EditVolunteer");
                    }
                }
                return RedirectToAction("Volunteers");
            }
            return View(usersVM);
        }

        public ActionResult GetBadPunches()
        {
            ReturnStatus badTimeSheets = Repository.GetBadTimeSheets();
            // List<TimeSheet> ts = Repository.GetBadTimeSheets();
            List<BadPunchVM> bp = new List<BadPunchVM>();

            if (badTimeSheets.errorCode != (int)ReturnStatus.ALL_CLEAR)
            {
                return null;
            }
            ReturnStatus timesheetReturn = new ReturnStatus();
            timesheetReturn.data = new List<TimeSheet>();
            if (timesheetReturn.errorCode != (int)ReturnStatus.ALL_CLEAR)
            {
                return null;
            }
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

    }
}
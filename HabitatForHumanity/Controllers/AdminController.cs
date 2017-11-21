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

        public ActionResult EditTimeCard(int id)
        {
            TimeCardVM card = new TimeCardVM();
            ReturnStatus rs = Repository.GetTimeSheetById(id);
            if(rs.errorCode == 0)
            {
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
            }

            return PartialView("_EditTimeCard", card);
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

        public ActionResult ViewOrganizations(OrganizationSearchModel model)
        {

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




            if (st.errorCode == ReturnStatus.ALL_CLEAR)
            {

                List<Organization> orgs = (List<Organization>)st.data;
                var pageIndex = model.Page ?? 1;
                model.SearchResults = orgs.ToPagedList(pageIndex, RecordsPerPage);
            }
            else
            {
                //fill search results with empty list
                List<Organization> orgs = new List<Organization>();
                var pageIndex = model.Page ?? 1;
                model.SearchResults = orgs.ToPagedList(pageIndex, RecordsPerPage);
            }

            //tsm.SearchResults = filteredVols.ToPagedList(pageIndex, RecordsPerPage);
            return View(model);
        }

        [HttpGet]
        public ActionResult EditOrganization(int id)
        {
            ReturnStatus st = Repository.GetOrganizationById(id);
            return PartialView("OrganizationPartialViews/_EditOrganization", (Organization)st.data);
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
            if (st.errorCode == ReturnStatus.ALL_CLEAR)
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


        public ActionResult Projects()
        {
            return RedirectToAction("Index", "Project");
        }

    }
}
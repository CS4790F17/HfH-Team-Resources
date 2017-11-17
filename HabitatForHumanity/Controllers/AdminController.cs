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
            if(!string.IsNullOrEmpty(vsm.SearchButton) || vsm.Page.HasValue)
            {
                List<UsersVM> vols = new List<UsersVM>();
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 4 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 5 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 24 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 43 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 44 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 42 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 41 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 45 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 64 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 244 });
                vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 34 });
                vols.Add(new UsersVM() { volunteerName = "last one", email = "testing", hoursToDate = 3.3, userNumber = 422 });

                var pageIndex = vsm.Page ?? 1;
                vsm.SearchResults = vols.ToPagedList(pageIndex, RecordsPerPage);
            }
            return View(vsm);
        }

       
        public ActionResult ViewVolunteers()
        {
            List<UsersVM> vols = new List<UsersVM>();
            vols.Add(new UsersVM() { volunteerName = "testing", email = "testing", hoursToDate = 3.3, userNumber = 4 });
            return View("_ViewVolunteers", vols);
        }
        public ActionResult GetUsersByQuery(string queryFilter = " AND 1=1 ")
        {
            ReturnStatus userResult = Repository.GetUsersByQuery(queryFilter);
            if(userResult.errorCode == 0)
            {
                return View("~/Views/User/Index.cshtml",(List<User>)userResult.data);
            }
            return View("~/Views/User/Index.cshtml",new List<User>());
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

            if(badTimeSheets.errorCode != (int)ReturnStatus.ALL_CLEAR)
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
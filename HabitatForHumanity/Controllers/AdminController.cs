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

namespace HabitatForHumanity.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        //GetHoursMonthChart
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
            // go get data and filter on gender
            #region Build Demographics Pie
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
                         Data = new Data(new object[]
                                               {
                                                   new object[] { "Firefox", 5 },
                                                   new object[] { "IE", 25 },
                                                   new DotNet.Highcharts.Options.Point
                                                   {
                                                       Name = "Chrome",
                                                       Y = 40,
                                                       Sliced = true,
                                                       Selected = true
                                                   },
                                                   new object[] { "Safari", 10},
                                                   new object[] { "Opera", 12 },
                                                   new object[] { "Others", 10 }
                                               })

                     }
               );

            #endregion
            return PartialView("_HoursByDemog", chart);
        }


        public ActionResult GetBadPunches()
        {
            return PartialView("_BadPunches", BadPunchVM.GetDummyBadPunches());
        }
    }
}
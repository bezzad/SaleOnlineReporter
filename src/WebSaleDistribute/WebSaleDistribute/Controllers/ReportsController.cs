using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using WebSaleDistribute.Core;
using System.Data;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.Threading.Tasks;
using WebSaleDistribute.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Globalization;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        #region Receipts

        // GET: Receipts        
        public ActionResult Receipts()
        {
            ViewBag.Title = "گزارش رسیدی";
            ViewData["dir"] = "ltr";


            return View();
        }

        // GET: ReceiptsChart      
        public ActionResult ReceiptsChart()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Fill Chart Data ------------------------------------------
            #region Chart Data

            var chartData = AdoManager.ConnectionManager.Find("SaleTabriz").SqlConn.Query("sp_GetInvoiceRemainChart", new { EmployeeID = currentUser.UserName, EmployeeTypeid = currentUser.EmployeeType, RunDate = "2" }, commandType: CommandType.StoredProcedure);

            var chartCategories = chartData.Select(x => (string)x.OfficerEmployeeName).ToArray();
            var chartValues = chartData.Select(x => x.InvoiceRemain).ToArray();

            var opt = new ChartOption()
            {
                Name = "receiptsChart",
                ChartType = ChartTypes.Column,
                XAxisData = chartCategories,
                YAxisData = chartValues,
                Tilte = "گزارش جمعی رسیدی ها به تفکیک متصدی ها",
                YAxisTitle = "جمع ریالی",
                SeriesName = "پرسنل",
                ShowLegend = true,
                ShowDataLabels = true,
                SubTitle = $"مبلغ کل رسیدی دفتر: {chartValues.Sum(x => (long)x).ToString("N0", CultureInfo.GetCultureInfo("fa-IR"))}"
            };

            #endregion
            //----------------------------------------------------------          

            return PartialView("ReceiptsChart", HtmlHelperExtensions.GetHighChart(opt));
        }

        // GET: ReceiptsTable
        public ActionResult ReceiptsTable()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Fill Table data ------------------------------------------
            #region Table Data
            var tableData = AdoManager.ConnectionManager.Find("SaleTabriz").SqlConn.ExecuteReader("sp_GetInvoiceRemain", new { EmployeeID = currentUser.UserName, EmployeeTypeid = currentUser.EmployeeType, RunDate = "2" }, commandType: CommandType.StoredProcedure);

            List<string> schema;
            var results = tableData.GetSchemaAndData(out schema);

            var model = Tuple.Create(schema, results);

            #endregion
            //-----------------------------------------------------------

            return PartialView("ReceiptsTable", model);
        }

        #endregion

        #region Customers Orders

        // GET: CustomersOrders
        public ActionResult CustomersOrders()
        {
            ViewBag.Title = "گزارشات درخواست مشتریان";
            ViewData["dir"] = "ltr";

            return View();
        }
        
        // GET: CustomersOrdersChart
        public ActionResult CustomersOrdersChart()
        {
            Highcharts chart = new Highcharts("chart")
                .SetTitle(new Title { Text = "Daily visits at www.highcharts.com" })
                .SetSubtitle(new Subtitle { Text = "Source: Google Analytics" })
                .SetXAxis(new XAxis
                {
                    Type = AxisTypes.Datetime,
                    TickInterval = 7 * 24 * 3600 * 1000, // one week
                    TickWidth = 0,
                    GridLineWidth = 1,
                    Labels = new XAxisLabels
                    {
                        Align = HorizontalAligns.Left,
                        X = 3,
                        Y = -3
                    }
                })
                .SetYAxis(new[]
                {
                    new YAxis
                    {
                        Title = new YAxisTitle { Text = "" },
                        Labels = new YAxisLabels
                        {
                            Align = HorizontalAligns.Left,
                            X = 3,
                            Y = 16,
                            Formatter = "function() { return Highcharts.numberFormat(this.value, 0); }",
                        },
                        ShowFirstLabel = false
                    },
                    new YAxis
                    {
                        LinkedTo = 0,
                        GridLineWidth = 0,
                        Opposite = true,
                        Title = new YAxisTitle { Text = "" },
                        Labels = new YAxisLabels
                        {
                            Align = HorizontalAligns.Right,
                            X = -3,
                            Y = 16,
                            Formatter = "function() { return Highcharts.numberFormat(this.value, 0); }"
                        },
                        ShowFirstLabel = false
                    }
                })
                .SetLegend(new Legend
                {
                    Align = HorizontalAligns.Left,
                    VerticalAlign = VerticalAligns.Top,
                    Y = 20,
                    Floating = true,
                    BorderWidth = 0
                })
                .SetTooltip(new Tooltip
                {
                    Shared = true,
                    Crosshairs = new Crosshairs(true)
                })
                .SetPlotOptions(new PlotOptions
                {
                    Series = new PlotOptionsSeries
                    {
                        Cursor = Cursors.Pointer,
                        Point = new PlotOptionsSeriesPoint
                        {
                            Events = new PlotOptionsSeriesPointEvents
                            {
                                Click = @"function() { alert(Highcharts.dateFormat('%A, %b %e, %Y', this.x) +': '+ this.y +' visits'); }"
                            }
                        },
                        Marker = new PlotOptionsSeriesMarker { LineWidth = 1 }
                    }
                })
                .SetSeries(new[]
                {
                    new Series { Name = "All visits" },
                    new Series { Name = "New visitors" }
                });

            return PartialView("CustomersOrdersChart", chart);
        }
        

        #endregion
    }
}
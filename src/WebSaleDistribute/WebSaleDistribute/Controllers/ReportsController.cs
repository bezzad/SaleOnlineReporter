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
            // Fill Chart Data ------------------------------------------
            #region Chart Data

            var api = new ReportsApiController();
            var data = api.GetInvoiceRemainChart();

            var opt = new ChartOption()
            {
                Name = "receiptsChart",
                ChartType = ChartTypes.Column,
                //XAxisData = chartCategories,
                YAxisData = new Data(data.Select(x => new Point() { Id = x.id.ToString(), Name = x.name, Y = x.y }).ToArray()),
                Tilte = "گزارش جمعی رسیدی ها به تفکیک متصدی ها",
                YAxisTitle = "جمع ریالی",
                SeriesName = "پرسنل",
                ShowLegend = false,
                ShowDataLabels = true,
                SubTitle = $"مبلغ کل رسیدی دفتر: {data.Sum(x => (long)x.y).ToString("N0", CultureInfo.GetCultureInfo("fa-IR"))}"
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
            try
            {
                var currentUser = UserManager.FindById(User.Identity.GetUserId());

                lock (currentUser)
                {
                    // Fill Chart Data ------------------------------------------
                    #region Chart Data

                    var opt = new ChartOption()
                    {
                        Name = "customersOrdersChart",
                        ChartType = ChartTypes.Column,
                        Tilte = "درخواست مشتریان هر متصدی",
                        SubTitle = "در خواست مشتریان ویزیتورها",
                        YAxisTitle = "جمع ریالی",
                        SeriesName = "پرسنل",
                        ShowLegend = false,
                        ShowDataLabels = true,
                        AjaxLoading = true,
                        LoadDataUrl = "GetOfficerOrderStatisticsChart"
                    };

                    #endregion
                    //----------------------------------------------------------          

                    return PartialView("CustomersOrdersChart", HtmlHelperExtensions.GetHighChart(opt));
                }
            }
            catch(Exception exp)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exp);
            }

            return PartialView("CustomersOrdersChart");
        }


        #endregion
    }
}
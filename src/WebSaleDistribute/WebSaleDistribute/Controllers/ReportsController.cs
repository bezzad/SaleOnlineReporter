using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using WebSaleDistribute.Core;
using System.Data;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.Globalization;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class ReportsController : BaseController
    {
        #region Receipts

        public ActionResult Receipts(ReceiptsModels model)
        {
            ViewBag.Title = "گزارش رسیدی";
            ViewData["dir"] = "ltr";

            if (model == null || (model.FromNo == 0 && model.ToNo == 0))
            {
                model = new ReceiptsModels()
                {
                    FromNo = 10000,
                    ToNo = 1000000000000,
                    DistanceAfterDistributeDate = 2
                };
            }

            return View("Receipt/Receipts", model);
        }

        // GET: ReceiptsChart      
        public ActionResult ReceiptsChart(ReceiptsModels model)
        {
            // Fill Chart Data ------------------------------------------
            #region Chart Data

            var api = new ReportsApiController();
            var data = api.GetInvoiceRemainChart(model).ToList();

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

            return PartialView("Receipt/ReceiptsChart", HtmlHelperExtensions.GetHighChart(opt));
        }

        // GET: ReceiptsTable
        public ActionResult ReceiptsTable(ReceiptsModels model)
        {
            ViewBag.Username = CurrentUser.UserName;

            // Fill Table data ------------------------------------------
            #region Table Data

            var tableData = Connections.SaleBranch.SqlConn.ExecuteReader(
                "sp_GetInvoiceRemain",
                new
                {
                    EmployeeID = CurrentUser.UserName,
                    EmployeeTypeid = CurrentUser.EmployeeType,
                    FromRemain = model.FromNo,
                    ToRemain = model.ToNo,
                    DistanceAfterDistributeDate = model.DistanceAfterDistributeDate,
                    RunDate = DateTime.Now.GetPersianDateNumber()
                },
                commandType: CommandType.StoredProcedure).ToDataTable();

            var tblModel = new TableOption()
            {
                Id = "receipts",
                Data = tableData,
                DisplayRowsLength = 10,
                Orders = new[] { Tuple.Create(0, OrderType.desc) },
                TotalFooterColumns = new[] { "InvoiceRemain", "Payable" },
                AverageFooterColumns = new[] { "ShamsiDateDiff" },
                CurrencyColumns = new[] { 8, 9 }
            };

            #endregion
            //-----------------------------------------------------------


            ToolsController.SetLastUserRunningAction(CurrentUser.UserName, "ReceiptsTable", tableData);

            return PartialView("Receipt/ReceiptsTable", tblModel);
        }

        #endregion

        #region Customers Orders

        // GET: CustomersOrders
        public ActionResult CustomersOrders(string fromDate, string toDate)
        {
            ViewBag.Title = "گزارشات درخواست مشتریان";
            ViewData["dir"] = "ltr";
            ViewBag.FromDate = fromDate ?? DateTime.Now.GetPersianDateByDashSpliter();
            ViewBag.ToDate = toDate ?? DateTime.Now.GetPersianDateByDashSpliter();

            return View("CustomersOrders/CustomersOrders");
        }

        // GET: CustomersOrdersChart/fromDate/toDate
        public ActionResult CustomersOrdersChart(string fromDate, string toDate)
        {
            ViewBag.FromDate = fromDate ?? DateTime.Now.GetPersianDateByDashSpliter();
            ViewBag.ToDate = toDate ?? DateTime.Now.GetPersianDateByDashSpliter();

            try
            {
                lock (CurrentUser)
                {
                    // Fill Chart Data ------------------------------------------
                    #region Chart Data

                    var opt = new ChartOption()
                    {
                        Name = "customersOrdersChart",
                        ChartType = ChartTypes.Column,
                        Tilte = "درخواست مشتریان هر متصدی",
                        SubTitle = "جمع درخواست ها: ",
                        YAxisTitle = "جمع ریالی",
                        SeriesName = "پرسنل",
                        ShowLegend = false,
                        ShowDataLabels = true,
                        AjaxLoading = true,
                        LoadDataUrl = $"GetOfficerOrderStatisticsChart/{fromDate}/{toDate}",
                        SubTitleFunc = "sum"
                    };

                    #endregion
                    //----------------------------------------------------------          


                    return PartialView("CustomersOrders/CustomersOrdersChart", HtmlHelperExtensions.GetHighChart(opt));
                }
            }
            catch (Exception exp)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exp);
            }

            return PartialView("CustomersOrders/CustomersOrdersChart");
        }


        #endregion
    }
}
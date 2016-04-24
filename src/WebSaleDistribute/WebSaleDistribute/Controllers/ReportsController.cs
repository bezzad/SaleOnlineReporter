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

namespace WebSaleDistribute.Controllers
{

    public class ReportsController : Controller
    {
        private AdoManager.ConnectionManager saleTabriz = AdoManager.ConnectionManager.Find("SaleTabriz");

        // GET: Receipts
        [Authorize]
        public ActionResult Receipts()
        {
            ViewBag.Title = "گزارش رسیدی";

            var data = saleTabriz.SqlConn.ExecuteReader("sp_GetInvoiceRemain", new { EmployeeID = 860003, EmployeeTypeid = 6, RunDate = "2" }, commandType: CommandType.StoredProcedure);

            List<string> schema;

            var results = data.GetSchemaAndData(out schema);

            ViewData["ModelSchema"] = schema;

            GetChart();

            return View(results);
        }

        // GET: Receipts
        public ActionResult Sales()
        {
            ViewBag.Title = "گزارشات فروش";



            return View();
        }


        private void GetChart()
        {
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
               .SetXAxis(new XAxis
               {
                   Categories = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }
               })
               .SetSeries(new Series
               {
                   Data = new Data(new object[] { 29.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4 })
               });
            ViewData["chart"] = chart.ToHtmlString();
        }
    }
}
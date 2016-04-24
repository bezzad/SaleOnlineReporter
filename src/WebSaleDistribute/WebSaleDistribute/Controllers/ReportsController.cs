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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: Receipts        
        public async Task<ActionResult> Receipts()
        {
            ViewBag.Title = "گزارش رسیدی";
            ViewData["dir"] = "ltr";

            // Fill Table data ------------------------------------------
            #region Table Data
            var tableData = await AdoManager.ConnectionManager.Find("SaleTabriz").SqlConn.ExecuteReaderAsync("sp_GetInvoiceRemain", new { EmployeeID = 860003, EmployeeTypeid = 6, RunDate = "2" }, commandType: CommandType.StoredProcedure);

            List<string> schema;
            var results = tableData.GetSchemaAndData(out schema);

            ViewData["ModelSchema"] = schema;
            #endregion
            //-----------------------------------------------------------

            // Fill Chart Data ------------------------------------------
            #region Chart Data

            var chartData = await AdoManager.ConnectionManager.Find("SaleTabriz").SqlConn.QueryAsync("sp_GetInvoiceRemainChart", new { EmployeeID = 860003, EmployeeTypeid = 6, RunDate = "2" }, commandType: CommandType.StoredProcedure);

            var chartCategories = chartData.Select(x => (string)x.OfficerEmployeeName).ToArray();
            var chartValues = chartData.Select(x => x.InvoiceRemain).ToArray();

            ViewData["ColumnChart"] = HtmlHelperExtensions.GetHighChart("testChart", ChartTypes.Column,
                chartCategories,
                chartValues,
                "گزارش جمعی رسیدی ها به تفکیک متصدی ها", "جمع ریالی", $"مبغ کل رسیدی دفتر: {chartValues.Sum(x => (long)x).ToString("N0")}").ToHtmlString();

            #endregion
            //----------------------------------------------------------          

            return View(results);
        }

        // GET: Receipts
        public ActionResult Sales()
        {
            ViewBag.Title = "گزارشات فروش";



            return View();
        }

    }
}
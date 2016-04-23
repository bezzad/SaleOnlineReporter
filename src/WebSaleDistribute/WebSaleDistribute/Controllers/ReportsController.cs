using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using WebSaleDistribute.Core;
using System.Dynamic;
using System.Threading.Tasks;
using System.Data;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private AdoManager.ConnectionManager saleTabriz = AdoManager.ConnectionManager.Find("SaleTabriz");

        // GET: Receipts
        public ActionResult Receipts()
        {
            ViewBag.Title = "گزارش رسیدی";

            var data = saleTabriz.SqlConn.ExecuteReader("sp_GetInvoiceRemain", new { EmployeeID = 860003, EmployeeTypeid = 6, RunDate = "2" }, commandType: CommandType.StoredProcedure);

            List<string> schema;

            var results = data.GetSchemaAndData(out schema);

            ViewData["ModelSchema"] = schema;
            
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
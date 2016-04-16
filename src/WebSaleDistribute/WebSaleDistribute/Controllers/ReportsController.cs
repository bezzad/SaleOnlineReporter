using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: Receipts
        public ActionResult Receipts()
        {
            ViewBag.Title = "گذارش رسیدی";
            return View();
        }

        // GET: Receipts
        public ActionResult Sales()
        {
            ViewBag.Title = "گزارشات فروش";
            return View();
        }
    }
}
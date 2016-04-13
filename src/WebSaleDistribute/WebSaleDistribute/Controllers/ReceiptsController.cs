using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSaleDistribute.Controllers
{
    public class ReceiptsController : Controller
    {
        // GET: Receipts
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Title = "گذارش رسیدی";
            return View();
        }

        
    }
}

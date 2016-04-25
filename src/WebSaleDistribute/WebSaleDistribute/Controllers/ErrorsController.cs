using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSaleDistribute.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Shared
        public ActionResult Error()
        {
            return View();
        }
    }
}
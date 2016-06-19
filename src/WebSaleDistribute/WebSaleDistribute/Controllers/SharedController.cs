using Elmah;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
    [AllowAnonymous]
    public class SharedController : Controller
    {
        [AllowAnonymous]
        public ActionResult Menu()
        {
            try
            {
                return PartialView("_MenuPartial", User?.GetMenus());
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
            }
            return null;
        }
    }
}
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
        public ApplicationUser CurrentUser
        {
            get
            {
                return UserManager.FindById(User.Identity.GetUserId());
            }
        }


        // GET: Home/SetEmployeeType Submit
        [HttpGet]
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
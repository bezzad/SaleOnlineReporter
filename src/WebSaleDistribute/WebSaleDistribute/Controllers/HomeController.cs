using System;
using Dapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class HomeController : Controller
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


        public ActionResult Index()
        {
            var menus = User?.GetMenus();

            return View(menus?.ToList());
        }

        // POST: Home/SetEmployeeType Submit
        [HttpPost]
        public ActionResult SetEmployeeType(int? employeeTypeId)
        {
            SetEmployeeTypesViewData(employeeTypeId);
            return PartialView("_EmployeeTypePartial");
        }

        // GET: Home/SetEmployeeType Submit
        [HttpGet]
        [ChildActionOnly]
        public ActionResult GetEmployeeType()
        {
            SetEmployeeTypesViewData(null);
            return PartialView("_EmployeeTypePartial");
        }

        private void SetEmployeeTypesViewData(int? employeeTypeId)
        {
            try
            {
                if (User.Identity.GetUserId() != null)
                {
                    var currentUser = UserManager.FindById(User.Identity.GetUserId());

                    if (currentUser == null) return;

                    int username;
                    if (int.TryParse(currentUser.UserName, out username))
                    {

                        var employeeTypes = Connections.SaleTabriz.SqlConn.Query<EmployeeTypeModels>("Select * From fn_GetEmployeeSaleTypes(@EmployeeID)", new { EmployeeID = currentUser.UserName });

                        ViewData["EmployeeTypes"] = employeeTypes.ToList();

                        if (employeeTypeId == null && employeeTypes.Any())
                        {
                            employeeTypeId = currentUser.EmployeeType ?? employeeTypes.Max(e => e.EmployeeTypeID);
                        }

                        currentUser.EmployeeType = employeeTypeId;

                        ViewData["SelectedEmployeTypeId"] = employeeTypeId;

                        UserManager.Update(currentUser);
                    }
                    else
                    {
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                        var empModel = new EmployeeTypeModels(1, currentUser.UserName);
                        ViewData["EmployeeTypes"] = new List<EmployeeTypeModels>() { empModel };
                        ViewData["SelectedEmployeTypeId"] = empModel.EmployeeTypeID;
                    }
                }
            }
            catch (Exception exp)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exp);
            }
        }

    }
}
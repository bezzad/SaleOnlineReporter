using Dapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Web.Mvc;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            #region Employee Type Partial Data

            if (User.Identity.GetUserId() != null)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                var employeeTypes = AdoManager.ConnectionManager.Find("SaleTabriz").SqlConn.Query("Select * From fn_GetEmployeeSaleTypes(@EmployeeID)", new { EmployeeID = currentUser.UserName });

                ViewData["EmployeeTypes"] = employeeTypes.ToList();
            }

            #endregion

            var menus = DynamicModels.GetMenus().ToList();

            return View(menus);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.Behzad = "930919";

            return View();
        }

    }
}
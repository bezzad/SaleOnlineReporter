using Dapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
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




        public async Task<ActionResult> Index()
        {
            var menus = DynamicModels.GetMenus().ToList();

            return View(menus);
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
            if (User.Identity.GetUserId() != null)
            {
                var currentUser = UserManager.FindById(User.Identity.GetUserId());

                var employeeTypes = AdoManager.ConnectionManager.Find("SaleTabriz").SqlConn.Query<EmployeeTypeModels>("Select * From fn_GetEmployeeSaleTypes(@EmployeeID)", new { EmployeeID = currentUser.UserName });

                ViewData["EmployeeTypes"] = employeeTypes.ToList();

                if (employeeTypeId == null && employeeTypes.Any())
                {
                    employeeTypeId = currentUser.EmployeeType ?? employeeTypes.Max(e => e.EmployeeTypeID);
                }

                currentUser.EmployeeType = employeeTypeId;

                ViewData["SelectedEmployeTypeId"] = employeeTypeId;

                UserManager.Update(currentUser);
            }
        }

    }
}
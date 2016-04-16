using System.Linq;
using System.Web.Mvc;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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
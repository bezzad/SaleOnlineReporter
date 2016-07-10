
using System.Web.Mvc;

namespace WebSaleDistribute.Controllers
{
    public class ErrorController : BaseController
    {
        public ViewResult Index()
        {
            return View("Index");
        }
    }
}
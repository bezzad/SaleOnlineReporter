using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
    public class BaseController : Controller
    {
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        public ApplicationUser CurrentUser => UserManager.FindById(User.Identity.GetUserId());


        //protected override void ExecuteCore()
        //{
        //    //
        //    // Set the thread's CurrentUICulture.
        //    //
        //    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa");
        //    //
        //    // Set the thread's CurrentCulture the same as CurrentUICulture.
        //    //
        //    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
        //    //
        //    // Invokes the action in the current controller context.
        //    //
        //    base.ExecuteCore();
        //}

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture =
                    new System.Globalization.CultureInfo("fa");

            return base.BeginExecuteCore(callback, state);
        }
    }
}
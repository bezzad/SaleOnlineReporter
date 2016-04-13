using System;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AdoManager;

namespace WebSaleDistribute
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SetConnection();

            Error += Application_Error;
        }

        private void SetConnection()
        {
            // Set Database Connection from [Web.config]
            var data = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Web.config");
            ConnectionManager.LoadFromXml(data);
#if DEBUG
            ConnectionManager.SetToDefaultConnection("UsersManagements"); // local
#else
            ConnectionManager.SetToDefaultConnection("UsersManagements"); // server
#endif
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();

            if (exc == null)
                return;

            // Handle HTTP errors
            if (exc.GetType() == typeof(HttpException))
            {
                // The Complete Error Handling Example generates
                // some errors using URLs with "NoCatch" in them;
                // ignore these here to simulate what would happen
                // if a global.asax handler were not implemented.
                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;

                //Redirect HTTP errors to HttpError page
                //Server.Transfer("~/Views/Errors");
                Response.Redirect("~/errors");
                return;
            }

            // Clear the error from the server
            Server.ClearError();

            // Redirect to a landing page
            Response.Redirect("~/home");
        }
    }
}

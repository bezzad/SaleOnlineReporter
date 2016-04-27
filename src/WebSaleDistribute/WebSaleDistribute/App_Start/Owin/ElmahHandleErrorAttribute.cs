using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Elmah;

namespace WebSaleDistribute.App_Start.Owin
{
    public class ElmahHandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            // Set TrySkipIisCustomErrors = true to tell IIS not to interfere
            context.HttpContext.Response.TrySkipIisCustomErrors = true;

            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.Result = new ContentResult
                {
                    Content = context.Exception.Message,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "text/plain"
                };

                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                base.OnException(context);
            }

            var e = context.Exception;
            if (!context.ExceptionHandled   // if unhandled, will be logged anyhow
                || RaiseErrorSignal(e)      // prefer signaling, if possible
                || IsFiltered(context))     // filtered?
                return;

            LogException(e);
        }

        // Check that elmah.mvc.disableHandleErrorFilter = "true" in web.config otherwise the exception may be logged twice
        private static bool RaiseErrorSignal(Exception e)
        {
            var context = HttpContext.Current;
            if (context == null)
                return false;
            var signal = ErrorSignal.FromContext(context);
            if (signal == null)
                return false;
            signal.Raise(e, context);
            return true;
        }

        private static bool IsFiltered(ExceptionContext context)
        {
            var config = context.HttpContext.GetSection("elmah/errorFilter")
                         as ErrorFilterConfiguration;

            if (config == null)
                return false;

            var testContext = new ErrorFilterModule.AssertionHelperContext(
                                      context.Exception, HttpContext.Current);

            return config.Assertion.Test(testContext);
        }

        private static void LogException(Exception e)
        {
            var context = HttpContext.Current;
            ErrorLog.GetDefault(context).Log(new Error(e, context));
        }
    }
}

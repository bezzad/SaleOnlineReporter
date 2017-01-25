using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using WebSaleDistribute.Core;
using System.Data;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.Globalization;
using System.Dynamic;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        #region CustomerPoint

        // GET: Index        
        public ActionResult Index()
        {
            ViewBag.Title = "موقعیت مشتریان";
            ViewData["dir"] = "ltr";

            IEnumerable < dynamic > result = Connections.SaleBranch.SqlConn.Query<dynamic>("SELECT * from dbo.fn_GetBranchLocation()",commandType: CommandType.Text).ToList();
            var pos = result.Select(x => new { Latitude = x.Latitude, Longitude = x.Longitude}).SingleOrDefault();
            ViewBag.Latitude = pos.Latitude;
            ViewBag.Longitude = pos.Longitude;
            ViewBag.UserName = 78273;// CurrentUser.UserName;
            var model = new List<Models.CustomerPointFilterModels>();
            return View("Point/Index", model);
        }




        public ActionResult GetCustomerPointStatus(Models.CustomerPointFilterViewModels filter)
        {
            var result = Connections.SaleBranch.SqlConn.Query<Models.CustomerPointFilterModels>("sp_GetCustomerPointStatus",
               new { PathCode = filter.PathCode, ClassNames = filter.ClassNames },
               commandType: System.Data.CommandType.StoredProcedure);
 
            return PartialView("Point/_CustomersPointStatus", result);
        }


        #endregion

    }
}
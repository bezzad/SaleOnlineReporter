using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WebSaleDistribute.Core;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class ReportsApiController : ApiController
    {
        public ApplicationUserManager UserManager => HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        public ApplicationUser CurrentUser => UserManager.FindById(User.Identity.GetUserId());



        [Route("Reports/GetInvoiceRemainChart")]
        public IEnumerable<dynamic> GetInvoiceRemainChart()
        {
            var result = Connections.SaleBranch.SqlConn.Query("sp_GetInvoiceRemainChart",
                new { EmployeeID = CurrentUser.UserName, EmployeeTypeid = CurrentUser.EmployeeType, RunDate = DateTime.Now.GetPersianDate() },
                commandType: System.Data.CommandType.StoredProcedure);

            return result;
        }


        [Route("Reports/GetOfficerOrderStatisticsChart")]
        public async Task<IHttpActionResult> GetOfficerOrderStatisticsChart()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (currentUser == null) throw new UnauthorizedAccessException("This user is not valid!");

            var routParams = Request.GetQueryStrings();
            var fromDate = routParams.ContainsKey("fromDate") ? routParams["fromDate"] : DateTime.Now.GetPersianDate();
            var toDate = routParams.ContainsKey("toDate") ? routParams["toDate"] : fromDate;

            var result = (CurrentUser.EmployeeType > 5) ?
                await Connections.SaleBranch.SqlConn.QueryAsync("sp_GetOfficerOrderStatisticsChart",
                new { FromDate = fromDate, ToDate = toDate },
                commandType: System.Data.CommandType.StoredProcedure)
            : await Connections.SaleBranch.SqlConn.QueryAsync("sp_GetOrderStatisticsChart",
                new { OfficerEmployeeID = CurrentUser.UserName, OfficerEmployeeTypeID = CurrentUser.EmployeeType, FromDate = fromDate, ToDate = toDate },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }


        [Route("Reports/GetOrderStatisticsChart/{officerEmployeeTypeId}/{officerEmployeeId}")]
        public async Task<IHttpActionResult> GetOrderStatisticsChart(int officerEmployeeTypeId, int officerEmployeeId)
        {
            var routParams = Request.GetQueryStrings();
            var fromDate = routParams.ContainsKey("fromDate") ? routParams["fromDate"] : DateTime.Now.GetPersianDate();
            var toDate = routParams.ContainsKey("toDate") ? routParams["toDate"] : fromDate;

            var result = await Connections.SaleBranch.SqlConn.QueryAsync("sp_GetOrderStatisticsChart",
                new { FromDate = fromDate, ToDate = toDate, OfficerEmployeeID = officerEmployeeId, OfficerEmployeeTypeID = officerEmployeeTypeId },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }
        

        [Route("Reports/GetVisitorCustomersOrderStatisticsChart/{visitorEmployeeId}")]
        public async Task<IHttpActionResult> GetVisitorCustomersOrderStatisticsChart(int visitorEmployeeId)
        {
            var routParams = Request.GetQueryStrings();
            var fromDate = routParams.ContainsKey("fromDate") ? routParams["fromDate"] : DateTime.Now.GetPersianDate();
            var toDate = routParams.ContainsKey("toDate") ? routParams["toDate"] : fromDate;

            var result = await Connections.SaleBranch.SqlConn.QueryAsync("sp_GetVisitorCustomersOrderStatisticsChart",
                new { FromDate = fromDate, ToDate = toDate, VisitorEmployeeID = visitorEmployeeId },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }
    }
}

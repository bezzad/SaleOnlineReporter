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



        // GET: api/Reports
        [Route("Reports/GetInvoiceRemainChart")]
        public IEnumerable<dynamic> GetInvoiceRemainChart()
        {
            var result = Connections.SaleTabriz.SqlConn.Query("sp_GetInvoiceRemainChart",
                new { EmployeeID = CurrentUser.UserName, EmployeeTypeid = CurrentUser.EmployeeType, RunDate = DateTime.Now.GetPersianDate() },
                commandType: System.Data.CommandType.StoredProcedure);

            return result;
        }


        // GET: api/Reports/GetOfficerOrderStatisticsChart
        [Route("Reports/GetOfficerOrderStatisticsChart")]
        public async Task<IHttpActionResult> GetOfficerOrderStatisticsChart()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (currentUser == null) throw new UnauthorizedAccessException("This user is not valid!");

            var routParams = Request.GetQueryStrings();
            var fromDate = routParams.ContainsKey("fromDate") ? routParams["fromDate"] : DateTime.Now.GetPersianDate();
            var toDate = routParams.ContainsKey("toDate") ? routParams["toDate"] : fromDate;

            var result = (CurrentUser.EmployeeType >= 5) ?
                await Connections.SaleTabriz.SqlConn.QueryAsync("sp_GetOfficerOrderStatisticsChart",
                new { FromDate = fromDate, ToDate = toDate },
                commandType: System.Data.CommandType.StoredProcedure)
            : await Connections.SaleTabriz.SqlConn.QueryAsync("sp_GetOrderStatisticsChart",
                new { OfficerEmployeeID = CurrentUser.UserName, OfficerEmployeeTypeID = CurrentUser.EmployeeType, FromDate = fromDate, ToDate = toDate },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }


        // GET: api/Reports/GetOrderStatisticsChart/{officerEmployeeTypeId}/{officerEmployeeId}
        [Route("Reports/GetOrderStatisticsChart/{officerEmployeeTypeId}/{officerEmployeeId}")]
        public async Task<IHttpActionResult> GetOrderStatisticsChart(int officerEmployeeTypeId, int officerEmployeeId)
        {
            var routParams = Request.GetQueryStrings();
            var fromDate = routParams.ContainsKey("fromDate") ? routParams["fromDate"] : DateTime.Now.GetPersianDate();
            var toDate = routParams.ContainsKey("toDate") ? routParams["toDate"] : fromDate;

            var result = await Connections.SaleTabriz.SqlConn.QueryAsync("sp_GetOrderStatisticsChart",
                new { FromDate = fromDate, ToDate = toDate, OfficerEmployeeID = officerEmployeeId, OfficerEmployeeTypeID = officerEmployeeTypeId },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }
    }
}

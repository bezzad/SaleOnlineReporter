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
        public IEnumerable<dynamic> GetInvoiceRemainChart(ReceiptsModels model)
        {
            var result = Connections.SaleBranch.SqlConn.Query("sp_GetInvoiceRemainChart",
                new
                {
                    EmployeeID = CurrentUser.UserName,
                    EmployeeTypeid = CurrentUser.EmployeeType,
                    FromRemain = model.FromNo,
                    ToRemain = model.ToNo,
                    DistanceAfterDistributeDate = model.DistanceAfterDistributeDate,
                    RunDate = DateTime.Now.GetPersianDateNumber()
                },
                commandType: System.Data.CommandType.StoredProcedure);

            return result;
        }


        [Route("Reports/GetOfficerOrderStatisticsChart/{fromDate}/{toDate}")]
        public async Task<IHttpActionResult> GetOfficerOrderStatisticsChart(string fromDate, string toDate)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (currentUser == null) throw new UnauthorizedAccessException("This user is not valid!");

            object result;

            if (CurrentUser.EmployeeType <= 5) // visitors order
            {
                result = await Connections.SaleBranch.SqlConn.QueryAsync("sp_GetOrderStatisticsChart",
                    new
                    {
                        FromDate = fromDate.Replace("-", "/"),
                        ToDate = toDate.Replace("-", "/"),
                        OfficerEmployeeID = int.Parse(CurrentUser.UserName),
                        OfficerEmployeeTypeID = CurrentUser.EmployeeType ?? 1 
                    },
                    commandType: System.Data.CommandType.StoredProcedure);
            }
            else
            {
                // officer orders:
                result = await Connections.SaleBranch.SqlConn.QueryAsync("sp_GetOfficerOrderStatisticsChart",
                    new {FromDate = fromDate.Replace("-", "/"), ToDate = toDate.Replace("-", "/")},
                    commandType: System.Data.CommandType.StoredProcedure);
            }

            return Ok(result);
        }


        [Route("Reports/GetOrderStatisticsChart/{fromDate}/{toDate}/{officerEmployeeTypeId}/{officerEmployeeId}")]
        public async Task<IHttpActionResult> GetOrderStatisticsChart(string fromDate, string toDate, int officerEmployeeTypeId, int officerEmployeeId)
        {
            var result = await Connections.SaleBranch.SqlConn.QueryAsync("sp_GetOrderStatisticsChart",
                new { FromDate = fromDate.Replace("-", "/"), ToDate = toDate.Replace("-", "/"), OfficerEmployeeID = officerEmployeeId, OfficerEmployeeTypeID = officerEmployeeTypeId },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }


        [Route("Reports/GetVisitorCustomersOrderStatisticsChart/{fromDate}/{toDate}/{visitorEmployeeId}")]
        public async Task<IHttpActionResult> GetVisitorCustomersOrderStatisticsChart(string fromDate, string toDate, int visitorEmployeeId)
        {
            var result = await Connections.SaleBranch.SqlConn.QueryAsync("sp_GetVisitorCustomersOrderStatisticsChart",
                new { FromDate = fromDate.Replace("-", "/"), ToDate = toDate.Replace("-", "/"), VisitorEmployeeID = visitorEmployeeId },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using System.Globalization;
using WebSaleDistribute.Core;

namespace WebSaleDistribute.Controllers
{
    public class ReportsApiController : ApiController
    {
        // GET: api/Reports
        [Route("Reports/GetOfficerOrderStatisticsChart")]
        public async Task<IHttpActionResult> GetOfficerOrderStatisticsChart()
        {
            var routParams = Request.GetQueryStrings();
            var fromDate = "1395/02/13"; // routParams.ContainsKey("fromDate") ? routParams["fromDate"] : DateTime.Now.GetPersianDate();
            var toDate = routParams.ContainsKey("toDate") ? routParams["toDate"] : fromDate;

            var sqlConn = AdoManager.ConnectionManager.Find(Properties.Settings.Default.SaleTabriz).SqlConn;
            var result = await sqlConn.QueryAsync("sp_GetOfficerOrderStatisticsChart",
                new { FromDate = fromDate, ToDate = toDate },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }


        // GET: api/Reports/GetOrderStatisticsChart/{officerEmployeeId}
        [Route("Reports/GetOrderStatisticsChart/{officerEmployeeId}")]
        public async Task<IHttpActionResult> GetOrderStatisticsChart(int officerEmployeeId)
        {
            var routParams = Request.GetQueryStrings();
            var fromDate = "1395/02/13"; //routParams.ContainsKey("fromDate") ? routParams["fromDate"] : DateTime.Now.GetPersianDate();
            var toDate = routParams.ContainsKey("toDate") ? routParams["toDate"] : fromDate;

            var sqlConn = AdoManager.ConnectionManager.Find(Properties.Settings.Default.SaleTabriz).SqlConn;
            var result = await sqlConn.QueryAsync("sp_GetOrderStatisticsChart",
                new { FromDate = fromDate, ToDate = toDate, OfficerEmployeeID = officerEmployeeId },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }
    }
}

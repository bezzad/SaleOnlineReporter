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
    public class WarehouseApiController : ApiController
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationUser CurrentUser
        {
            get
            {
                return UserManager.FindById(User.Identity.GetUserId());
            }
        }

        // GET: api/Warehouse/EntryInWayToWarehouse
        [Route("Warehouse/EntryInWayToWarehouse/{invoicId}")]
        public async Task<IHttpActionResult> EntryInWayToWarehouseAsync(int invoicId)
        {
            var sqlConn = AdoManager.ConnectionManager.Find(Properties.Settings.Default.SaleTabriz).SqlConn;
            var result = await sqlConn.ExecuteAsync("sp_EntryInWayToWareHouseByOldInvoiceId",
                new
                {
                    OldInvoiceId = invoicId,
                    UserId = CurrentUser.UserName                    
                }, 
                commandType: System.Data.CommandType.StoredProcedure);

            var msg = $"متاسفانه خطایی هنگام ورود به انبار {invoicId} رخ داده است!";

            if (result > 0)
                msg = $"فاکتور توراهی {invoicId} وارد انبار شد";
            
            return Ok(msg);
        }
    }
}

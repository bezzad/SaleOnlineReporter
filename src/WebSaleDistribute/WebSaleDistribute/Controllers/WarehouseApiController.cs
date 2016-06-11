using System;
using System.Web.Http;
using Dapper;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebSaleDistribute.Models;
using Elmah;

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
        [HttpGet]
        [Route("Warehouse/EntryInWayToWarehouse/{invoicId}")]
        public IHttpActionResult EntryInWayToWarehouse(int invoicId)
        {
            string msg = $"متاسفانه خطایی هنگام ورود به انبار {invoicId} رخ داده است!";
            try
            {
                var param = new
                {
                    OldInvoiceId = invoicId,
                    UserId = User.Identity.GetUserId()
                };

                var result = Connections.SaleTabriz.SqlConn.Execute("sp_EntryInWayToWareHouseByOldInvoiceId",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure);

                if (result > 0)
                    msg = $"فاکتور توراهی {invoicId} وارد انبار شد";
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                msg = exp.Message;
                return InternalServerError(exp);
            }

            return Ok(msg);
        }
    }
}

using System;
using System.Web.Http;
using Dapper;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebSaleDistribute.Models;
using Elmah;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using WebSaleDistribute.Core;

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
        public ApplicationUser CurrentUser => UserManager.FindById(User.Identity.GetUserId());


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

        // GET: api/Warehouse/GetInvoiceDetails
        [HttpGet]
        [Route("Warehouse/GetInvoiceDetails/{businessDocSerialNo}")]
        public IHttpActionResult GetInvoiceDetails(int businessDocSerialNo)
        {
            var result = Connections.SaleTabriz.SqlConn.Execute("sp_GetSaleReturnInvoiceDetailsTable",
                new { SerialNo = businessDocSerialNo },
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(result);
        }

        // POST: api/StoreReturnedInovicesInWarehouse
        [HttpPost]
        [Route("Warehouse/StoreReturnedInovicesInWarehouse")]
        public IHttpActionResult StoreReturnedInovicesInWarehouse(HttpRequestMessage request)
        {
            var content = request.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonContent);
            int invoiceSerialNo = data.invoiceSerialNo.ToObject(typeof(int));
            var saleableRows = (string[])data.saleableRows.ToObject(typeof(string[]));
            var unsaleableList = ((List<JArray>)data.unsaleableList.ToObject(typeof(List<JArray>))).Select(x => (System.Dynamic.ExpandoObject)x.ToObject(typeof(System.Dynamic.ExpandoObject)));
            
            
            return Ok("برگشتی با موفقیت ثبت شد. لطفا برای مشاهده نتیجه ثبت منتظر بمانید...");
        }

        // POST: api/CountingWarehouseFinalAccept
        [HttpPost]
        [Route("Warehouse/CountingWarehouseFinalAccept")]
        public IHttpActionResult CountingWarehouseFinalAccept(HttpRequestMessage request)
        {
            var content = request.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonContent);
            int countingSerialNo = data.countingSerialNo.ToObject(typeof(int));
            string msg = $"متاسفانه خطایی هنگام ثبت شمارش {countingSerialNo} رخ داده است!";

            try
            {
                var param = new
                {
                    CountingSerialNo = countingSerialNo,
                    UserId = User.Identity.GetUserId(),
                    RunDate = DateTime.Now.GetPersianDate()
                };

                var result = Connections.SaleTabriz.SqlConn.Execute("sp_FinalAcceptCountingWarehouseHistory",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure);

                if (result > 0)
                    msg = $"شمارش انبار {countingSerialNo} با موفقیت ثبت شد";
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                return InternalServerError(exp);
            }

            return Ok(msg);
        }
    }
}

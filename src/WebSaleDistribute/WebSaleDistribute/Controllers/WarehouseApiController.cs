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
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
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

                var result = Connections.SaleBranch.SqlConn.Execute("sp_EntryInWayToWareHouseByOldInvoiceId",
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


        // GET: api/Warehouse/EntryInWayToWarehouse
        [HttpPost]
        [Route("Warehouse/EntryProductionToWarehouse/")]
        public IHttpActionResult EntryProductionToWarehouse(HttpRequestMessage request)
        {
           
            var content = request.Content;
            var dict = HttpUtility.ParseQueryString(content.ReadAsStringAsync().Result);
            var javaScript = new JavaScriptSerializer().Serialize(
                    dict.AllKeys.ToDictionary(k => k, k => dict[k])
            );
            var json = JObject.Parse(javaScript);
            var businessDocNo = json["BusinessDocNo"].ToObject(typeof(long));  
            var priceId = json["PriceId"].ToObject(typeof(int)); 
            var productCode = json["ProductCode"].ToObject(typeof(int));
            var qty = json["Qty"].ToObject(typeof(int));
            var userId = User.Identity.GetUserId();
            string msg = $"متاسفانه خطایی هنگام ورود به انبار {businessDocNo} رخ داده است!";
            var errFlag = true;
            try
            {
                var param = new
                {
                    BusinessDocNo = businessDocNo,
                    PriceId = priceId,
                    ProductCode = productCode,
                    Qty = qty,
                    UserId = userId
                };

                var result = Connections.OldSale.SqlConn.Execute("sp_SignInProduction_Insert",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure);

                if (result > 0)
                {
                    msg = $"محصولات حواله  بشماره {businessDocNo} وارد انبار شد";
                    errFlag = false;
                }
                   
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                msg = exp.Message;
                 
            }
            var res = new
            {
                errFlag,
                msg
            };

            return Ok(res);
        }

        // GET: api/Warehouse/GetInvoiceDetails
        [HttpGet]
        [Route("Warehouse/GetInvoiceDetails/{businessDocSerialNo}")]
        public IHttpActionResult GetInvoiceDetails(int businessDocSerialNo)
        {
            var result = Connections.SaleBranch.SqlConn.Execute("sp_GetSaleReturnInvoiceDetailsTable",
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


   


        // POST: api/StoreReturnedInovicesInWarehouse
        [HttpPost]
        [Route("Warehouse/CountingWarehouseAutoSave")]
        public IHttpActionResult CountingWarehouseAutoSave(HttpRequestMessage request)
        {
            var content = request.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<dynamic>(jsonContent);
            var serialNo = data.serialNo.ToString();
            var countingDetails = data.countingRows.ToString();
            StoreWarehouseCounting(serialNo, countingDetails);
            return Ok("ذخیره سازی خودکار انجام شد.");
        }



        public DataTable StoreWarehouseCounting(string serial, string countingRows)
        {
            var serialNo = JsonConvert.DeserializeObject<int>(serial);

            var countingWarehouse = JsonConvert.DeserializeObject<List<JArray>>(countingRows);
            var countingDynamicTable = countingWarehouse.Select(x => x.ToObject<object[]>());

            var tableSchema = Connections.SaleBranch.SqlConn.ExecuteReader(
                sql: "sp_GetEmptyCountingWarehouseHistoryDetailsTable", param: new { CountingSerialNo = -1 },
                commandType: CommandType.StoredProcedure).ToDataTable().Clone();

            foreach (var row in countingDynamicTable)
            {
                tableSchema.Rows.Add(row);
            }

            var temp = tableSchema.AsEnumerable().Select(x => new
            {
                CountingNo = serial,
                WarehouseOrderNo = x["WarehouseOrderNo"],
                ProductCode = x["ProductCode"],
                ProductName = x["ProductName"],
                NetWeight = x["NetWeight"],
                Shortcut = x["Shortcut"],
                WarehouseCartonOnHand = x["WarehouseCartonOnHand"],
                WarehousePacketOnHand = x["WarehousePacketOnHand"],
                UserID = CurrentUser.UserName
            }).ToList();

            var connection = Connections.SaleCore.SqlConn;
            connection.Open();
            using (SqlTransaction trans = connection.BeginTransaction())
            {
                // First clear temp table data for this counting no.
                connection.Execute("sp_TempCountingWarehouseHistoryDetail_Delete", new { CountingNo = serialNo },
                    transaction: trans, commandType: CommandType.StoredProcedure);

                // Bulk copy all rows to temp table
                connection.Execute(@"
                 INSERT INTO TempCountingWarehouseHistoryDetail
                       (CountingNo
                       ,WarehouseOrderNo
                       ,ProductCode
                       ,ProductName
                       ,NetWeight
                       ,Shortcut
                       ,WarehouseCartonOnHand
                       ,WarehousePacketOnHand
                       ,UserID)
                 VALUES
                       (@CountingNo
                       ,@WarehouseOrderNo
                       ,@ProductCode
                       ,@ProductName
                       ,@NetWeight
                       ,@Shortcut
                       ,@WarehouseCartonOnHand
                       ,@WarehousePacketOnHand
                       ,@UserID)",
                       temp, transaction: trans);

                trans.Commit();
                connection.Close();
            }

            return tableSchema;
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

                var result = Connections.SaleBranch.SqlConn.Execute("sp_FinalAcceptCountingWarehouseHistory",
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
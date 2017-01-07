using System;
using System.Web.Http;
using System.Net.Http;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNet.Identity;
using Dapper;
using Elmah;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSaleDistribute.Core;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class WarehouseApiController : BaseApiController
    {
        #region Inway

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
                    param, commandTimeout: 99000,
                    commandType: CommandType.StoredProcedure);

                if (result > 0)
                    msg = $"فاکتور توراهی {invoicId} وارد انبار شد";
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                return InternalServerError(exp);
            }

            return Ok(msg);
        }

        #endregion

        #region Sale returned invoices to warehouse

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
            var data = JsonConvert.DeserializeObject<dynamic>(jsonContent);
            int invoiceSerialNo = data.invoiceSerialNo.ToObject(typeof(int));
            var saleableRows = (string[])data.saleableRows.ToObject(typeof(string[]));
            var unsaleableList = ((List<string[]>) data.unsaleableList.ToObject(typeof(List<string[]>))).ToDictionary(x => x[0], y => y[4]);
            var storeCode = data.warehouse.ToObject(typeof(int));

            var res = Connections.SaleBranch.SqlConn.Query("sp_TransferReturnSaleToWarehouse", new
                {
                    InvoiceSerialNo = invoiceSerialNo,
                    DestinationStoreCode = storeCode,
                    SaleableRows = saleableRows.ToDataTable<string>(),
                    UnSaleableRows = unsaleableList.ToDataTable(),
                    UserID = CurrentUser.Id,
                    RunDate = DateTime.Now.GetPersianDateNumber()
                }, commandType: CommandType.StoredProcedure);

            return Ok("برگشتی با موفقیت ثبت شد. لطفا برای مشاهده نتیجه ثبت منتظر بمانید...");
        }

        #endregion

        #region Counting Warehouse

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
                    RunDate = DateTime.Now.GetPersianDateNumber()
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

        #endregion

    }
}
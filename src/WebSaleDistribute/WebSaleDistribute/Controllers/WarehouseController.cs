using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSaleDistribute.Core;
using WebSaleDistribute.Models;
using AdoManager;
using Dapper;
using System.Data;
using Newtonsoft.Json;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
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


        private List<string> _steps = new List<string>()
                {
                    "انتخاب فاکتور برگشتی",
                    "تعیین اقلام قابل فروش",
                    "تایید نهایی اقلام قابل فروش و علت عدم قابل فروش",
                    "ورود برگشتی به انبار"
                };





        // GET: Warehouse
        public ActionResult Warehouse()
        {
            ViewBag.Title = "انبار";

            return View();
        }



        #region InWay

        // GET: Warehouse/InWay/?code={code}
        public ActionResult InWay(string code)
        {
            ViewBag.Title = "توراهی";

            var model = new TableOption() { Id = "entryInWay" };

            //string encryptedQrCode = Request.QueryString["code"];


            if (!string.IsNullOrEmpty(code))
            {
                if (code.Length < 8)
                {
                    ViewBag.QrCode = code;
                }
                else
                {
                    ViewBag.QrCode = code.RepairCipher()?.Decrypt();
                }

                #region Table Data

                var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                    "sp_GetInWayDetailsByOldInvoicId",
                    new { OldInvoicId = ViewBag.QrCode },
                    commandType: CommandType.StoredProcedure);

                List<string> schema;
                var results = tableData.GetSchemaAndData(out schema);

                model.Schema = schema;
                model.Rows = results;
                model.DisplayRowsLength = 50;
                model.TotalFooterColumns = new string[] { "9", "تعداد" }; // column by name "تعداد" and column by index 9
                model.CurrencyColumns = new[] { 7, 8, 9 };

                #endregion
            }

            return View(model);
        }

        #endregion


        #region Sale Return Invoice to Warehous


        // GET: Warehouse/SaleReturnInvoices
        public ActionResult SaleReturnInvoices()
        {
            ViewBag.Title = "انتخاب یک فاکتور برگشت از فروش";

            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _steps,
                CurrentStepIndex = 1
            };

            return View(multipleStepOpt);
        }

        // GET: Warehouse/SaleReturnInvoicesTable
        public ActionResult SaleReturnInvoicesTable()
        {
            #region Table Data

            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetSaleReturnInvoicesTable", commandType: CommandType.StoredProcedure);

            List<string> schema;
            var results = tableData.GetSchemaAndData(out schema);

            var model = new TableOption()
            {
                Id = "saleReturnInvoices",
                Schema = schema,
                Rows = results,
                DisplayRowsLength = 10,
                Orders = new[] { Tuple.Create(0, OrderType.desc) },
                TotalFooterColumns = new string[] { "مبلغ برگشتي" },
                CurrencyColumns = new int[] { 6 },
                Checkable = false
            };

            #endregion

            return PartialView("_SaleReturnInvoicesTablePartial", model);
        }
        
        // GET: Warehouse/ChooseReturnedInvoiceDetails/?invoiceSerial={invoiceSerial}
        public ActionResult ChooseReturnedInvoiceDetails(int invoiceSerial)
        {
            ViewBag.Title = $"انتخاب اقلام برگشتی قابل فروش";
            ViewBag.InvoiceSerial = invoiceSerial;

            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _steps,
                CurrentStepIndex = 2
            };

            return View(multipleStepOpt);
        }

        // GET: Warehouse/ChooseReturnedInvoiceDetailsTable/?invoiceSerial={invoiceSerial}
        public ActionResult ChooseReturnedInvoiceDetailsTable(int invoiceSerial)
        {
            ViewBag.InvoiceSerial = invoiceSerial;

            #region Table Data

            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetSaleReturnInvoiceDetailsTable", new { SerialNo = invoiceSerial },
                commandType: CommandType.StoredProcedure);

            List<string> schema;
            var results = tableData.GetSchemaAndData(out schema);

            var model = new TableOption()
            {
                Id = "saleReturnInvoices",
                Schema = schema,
                Rows = results,
                DisplayRowsLength = -1,
                Orders = new[] { Tuple.Create(0, OrderType.asc) },
                TotalFooterColumns = new string[] { "تعداد" },
                CurrencyColumns = new int[] { 8 },
                Checkable = true
            };

            #endregion

            return PartialView("_ChooseReturnedInvoiceDetailsTablePartial", model);
        }

        // GET: Warehouse/CertificationSelectedReturnedInvoiceDetails/?invoiceSerial={invoiceSerial}&rows={rows} 
        public ActionResult CertificationSelectedReturnedInvoiceDetails(int invoiceSerial, string rows)
        {
            ViewBag.Title = "تایید برگشتی قابل فروش و غیر قابل فروش";
            ViewBag.InvoiceSerial = invoiceSerial;
            string[] sRows = ViewBag.SaleableRows = rows.Split(',');

            #region Table Data

            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetSaleReturnInvoiceDetailsTable", new { SerialNo = invoiceSerial },
                commandType: CommandType.StoredProcedure);

            List<string> schema;
            var results = tableData.GetSchemaAndData(out schema);

            var lstSaleable = results.Where(x => sRows.Contains(((IDictionary<string, object>)x).First().Value.ToString())).ToList();
            var lstUnSaleable = results.Where(x => !sRows.Contains(((IDictionary<string, object>)x).First().Value.ToString())).ToList();

            var modelSaleable = new TableOption()
            {
                Id = "saleable",
                Schema = schema,
                Rows = lstSaleable,
                DisplayRowsLength = -1,
                Orders = new[] { Tuple.Create(0, OrderType.asc) },
                //TotalFooterColumns = new string[] { "تعداد" },
                //CurrencyColumns = new int[] { 7 },
                Checkable = false
            };

            var modelUnSaleable = new TableOption()
            {
                Id = "unsaleable",
                Schema = schema,
                Rows = lstUnSaleable,
                DisplayRowsLength = -1,
                Orders = new[] { Tuple.Create(0, OrderType.asc) }
            };

            var reasons = Connections.SaleCore.SqlConn.Query("SELECT ReasonID, ReasonName FROM Reason");
            var reasonComboOpt = new ComboBoxOption()
            {
                Placeholder = "انتخاب علت برگشتی",
                MenuHeaderText = "علت برگشتی را انتخاب کنید",
                ShowOptionSubText = false,
                DataStyle = Core.Enums.DataStyleType.warning,
                ShowTick = true,
                DataLiveSearch = true,
                DataSize = "8",
                Data = reasons.Select(x => new ComboBoxDataModel() { Value = ((object)x.ReasonID).ToString(), Text = x.ReasonName }).ToList()
            };
            modelUnSaleable.ComboBoxColumnsDataMember["4"] = reasonComboOpt;

            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _steps,
                CurrentStepIndex = 3
            };            

            var model = Tuple.Create(modelSaleable, modelUnSaleable, multipleStepOpt);

            #endregion

            return View("CertificationSelectedReturnedInvoiceDetails", model);
        }

        // POST: Warehouse/ShowEntryReturnedInvoiceDetails
        [HttpPost]
        public ActionResult ShowEntryReturnedInvoiceDetails(string invoiceSerialNo, string saleableRows, string unsaleableList)
        {
            ViewBag.Title = "ورود به انبار برگشتی";

            var serialNo = JsonConvert.DeserializeObject<int>(invoiceSerialNo);
            var saleable = JsonConvert.DeserializeObject(saleableRows);
            var unsaleable = JsonConvert.DeserializeObject(unsaleableList);


            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _steps,
                CurrentStepIndex = 4
            };

            return View("ShowEntryReturnedInvoiceDetails", multipleStepOpt);
        }

        #endregion
    }
}

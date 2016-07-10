using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebSaleDistribute.Core;
using WebSaleDistribute.Models;
using AdoManager;
using Dapper;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSaleDistribute.Core.Enums;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class WarehouseController : BaseController
    {
        #region Properties

        private readonly List<string> _saleReturnedSteps = new List<string>()
                {
                    "انتخاب فاکتور برگشتی",
                    "تعیین اقلام قابل فروش",
                    "تایید نهایی اقلام قابل فروش و علت عدم قابل فروش",
                    "ورود برگشتی به انبار"
                };

        private readonly List<string> _countingWarehouseSteps = new List<string>()
                {
                    "انتخاب شمارش انبار",
                    "شمارش موجودی انبار",
                    "تایید نهایی شمارش"
                };

        #endregion


        #region Mehtods

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
                    commandType: CommandType.StoredProcedure).ToDataTable();

                model.Data = tableData;
                model.DisplayRowsLength = 50;
                model.TotalFooterColumns = new [] { "TotalPrice", "Qty" }; // column by name "تعداد" and "قیمت کل"
                model.CurrencyColumns = new[] { 7, 8, 9 };

                #endregion
            }

            return View("InWay/InWay", model);
        }

        #endregion


        #region Sale Return Invoice to Warehous


        // GET: Warehouse/SaleReturnInvoices
        public ActionResult SaleReturnInvoices()
        {
            ViewBag.Title = "انتخاب یک فاکتور برگشت از فروش";

            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _saleReturnedSteps,
                CurrentStepIndex = 1
            };

            return View("SaleReturnedInvoice/SaleReturnInvoices", multipleStepOpt);
        }

        // GET: Warehouse/SaleReturnInvoicesTable
        public ActionResult SaleReturnInvoicesTable()
        {
            #region Table Data

            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetSaleReturnInvoicesTable", commandType: CommandType.StoredProcedure).ToDataTable();

            var model = new TableOption()
            {
                Id = "saleReturnInvoices",
                Data = tableData,
                DisplayRowsLength = 10,
                Orders = new[] { Tuple.Create(0, OrderType.desc) },
                TotalFooterColumns = new string[] { "مبلغ برگشتي" },
                CurrencyColumns = new int[] { 6 },
                Checkable = false
            };

            #endregion

            return PartialView("SaleReturnedInvoice/_SaleReturnInvoicesTablePartial", model);
        }

        // GET: Warehouse/ChooseReturnedInvoiceDetails/?invoiceSerial={invoiceSerial}
        public ActionResult ChooseReturnedInvoiceDetails(int invoiceSerial)
        {
            ViewBag.Title = $"انتخاب اقلام برگشتی قابل فروش";
            ViewBag.InvoiceSerial = invoiceSerial;

            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _saleReturnedSteps,
                CurrentStepIndex = 2
            };

            return View("SaleReturnedInvoice/ChooseReturnedInvoiceDetails", multipleStepOpt);
        }

        // GET: Warehouse/ChooseReturnedInvoiceDetailsTable/?invoiceSerial={invoiceSerial}
        public ActionResult ChooseReturnedInvoiceDetailsTable(int invoiceSerial)
        {
            ViewBag.InvoiceSerial = invoiceSerial;

            #region Table Data

            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetSaleReturnInvoiceDetailsTable", new { SerialNo = invoiceSerial },
                commandType: CommandType.StoredProcedure).ToDataTable();

            var model = new TableOption()
            {
                Id = "saleReturnInvoices",
                Data = tableData,
                DisplayRowsLength = -1,
                Orders = new[] { Tuple.Create(0, OrderType.asc) },
                TotalFooterColumns = new string[] { "تعداد" },
                CurrencyColumns = new int[] { 8 },
                Checkable = true
            };

            #endregion

            return PartialView("SaleReturnedInvoice/_ChooseReturnedInvoiceDetailsTablePartial", model);
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
                commandType: CommandType.StoredProcedure).ToDataTable();

            // creates a copy of the schema (columns)only.
            DataTable lstSaleable = tableData.Clone(), 
                lstUnSaleable = tableData.Clone();

            // check row id for saleable list
            foreach (DataRow row in tableData.Rows)
            {
                if (sRows.Contains(row[0].ToString())) // row[0] = row["Id"]
                {
                    lstSaleable.Rows.Add(row.ItemArray);
                }
                else
                {
                    lstUnSaleable.Rows.Add(row.ItemArray);
                }
            }

            var modelSaleable = new TableOption()
            {
                Id = "saleable",
                Data = lstSaleable,
                DisplayRowsLength = -1,
                Orders = new[] { Tuple.Create(0, OrderType.asc) },
                Checkable = false
            };

            var modelUnSaleable = new TableOption()
            {
                Id = "unsaleable",
                Data = lstUnSaleable,
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
            modelUnSaleable.InputColumnsDataMember["4"] = reasonComboOpt;

            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _saleReturnedSteps,
                CurrentStepIndex = 3
            };

            var model = Tuple.Create(modelSaleable, modelUnSaleable, multipleStepOpt);

            #endregion

            return View("SaleReturnedInvoice/CertificationSelectedReturnedInvoiceDetails", model);
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
                Steps = _saleReturnedSteps,
                CurrentStepIndex = 4
            };

            return View("SaleReturnedInvoice/ShowEntryReturnedInvoiceDetails", multipleStepOpt);
        }

        #endregion


        #region Counting Warehouse 

        // GET: Warehouse/CountingWarehouse
        public ActionResult CountingWarehouse()
        {
            ViewBag.Title = "انتخاب شمارش انبار";

            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _countingWarehouseSteps,
                CurrentStepIndex = 1
            };

            return View("CountingWarehouse/CountingWarehouse", multipleStepOpt);
        }

        // GET: Warehouse/CountingWarehouseHeaderTable
        public ActionResult CountingWarehouseHeaderTable()
        {
            #region Table Data

            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetCountingWarehouseHistoryTable", commandType: CommandType.StoredProcedure).ToDataTable();

            var model = new TableOption()
            {
                Id = "CountingWarehouseHeaderTable",
                Data = tableData,
                DisplayRowsLength = 10,
                Orders = new[] { Tuple.Create(0, OrderType.desc) },
                Checkable = false
            };

            #endregion

            return PartialView("CountingWarehouse/_CountingWarehouseHeaderTablePartial", model);
        }

        // GET: Warehouse/FillCountingWarehouseDetails/?serial={serial}
        public ActionResult FillCountingWarehouseDetails(int serial)
        {
            ViewBag.Title = $"شمارش موجودی انبار";
            ViewBag.Serial = serial;

            #region Table Data

            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetEmptyCountingWarehouseHistoryDetailsTable", new { CountingSerialNo = serial },
                commandType: CommandType.StoredProcedure).ToDataTable();

            var table = new TableOption()
            {
                Id = "EmptyCountingWarehouseHistoryDetails",
                Data = tableData,
                DisplayRowsLength = 10,
                Orders = new[] { Tuple.Create(0, OrderType.asc) },
                //TotalFooterColumns = new string[] { "وزن خالص" },
                CurrencyColumns = new int[] { 3 },
                Checkable = false
            };

            var txtOpt = new TextBoxOption()
            {
                Placeholder = "موجودی",
                DataStyle = DataStyleType.warning,
                Max = 999999,
                Min = 0,
                Step = 1,
                Type = InputTypes.Number
            };

            table.InputColumnsDataMember["5"] = txtOpt;
            table.InputColumnsDataMember["6"] = txtOpt;

            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _countingWarehouseSteps,
                CurrentStepIndex = 2
            };

            var model = Tuple.Create(table, multipleStepOpt);

            #endregion

            return View("CountingWarehouse/FillCountingWarehouseDetails", model);
        }

        // POST: Warehouse/CertificationCountingWarehouseDetails
        [HttpPost]
        public ActionResult CertificationCountingWarehouseDetails(string serial, string countingRows)
        {
            ViewBag.Title = "تایید نهایی شمارش انبار";

            var serialNo = JsonConvert.DeserializeObject<int>(serial);
            var countingWarehouse = JsonConvert.DeserializeObject<List<JArray>>(countingRows);
            var xx = countingWarehouse.Select(x => x.ToExpando());


            var multipleStepOpt = new MultipleStepProgressTabOption()
            {
                Steps = _countingWarehouseSteps,
                CurrentStepIndex = 3
            };

            return View("CountingWarehouse/CertificationCountingWarehouseDetails", multipleStepOpt);
        }

        #endregion


        #endregion
    }
}

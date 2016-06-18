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
using Newtonsoft.Json.Linq;

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


        // GET: Warehouse
        public ActionResult Warehouse()
        {
            ViewBag.Title = "انبار";

            return View();
        }

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


        // GET: Warehouse/SaleReturnInvoices
        public ActionResult SaleReturnInvoices()
        {
            ViewBag.Title = "برگشت از فروش";

            return View();
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

            return View();
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
            ViewBag.Title = "تایید نهایی برگشتی";
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
                Placeholder = " انتخاب علت برگشتی ",
                MenuHeaderText = " علت برگشتی را انتخاب کنید ",
                ShowOptionSubText = false,
                DataStyle = Core.Enums.DataStyleType.warning,
                ShowTick = true,
                DataSize = "5",
                Data = reasons.Select(x => new ComboBoxDataModel() { Value = ((object)x.ReasonID).ToString(), Text = x.ReasonName }).ToList()
            };
            modelUnSaleable.ComboBoxColumnsDataMember["4"] = reasonComboOpt;

            var model = Tuple.Create(modelSaleable, modelUnSaleable);

            #endregion

            return View("CertificationSelectedReturnedInvoiceDetails", model);
        }

    }
}

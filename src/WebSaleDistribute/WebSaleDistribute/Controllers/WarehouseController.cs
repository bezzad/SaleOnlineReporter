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

        // GET: Warehouse
        public ActionResult InWay()
        {
            ViewBag.Title = "توراهی";

            string encryptedQrCode = Request.QueryString["code"];

            if (!string.IsNullOrEmpty(encryptedQrCode))
            {
                if (encryptedQrCode.Length < 8)
                {
                    ViewBag.QrCode = encryptedQrCode;
                }
                else
                {
                    ViewBag.QrCode = encryptedQrCode.RepairCipher()?.Decrypt();
                }
            }

            return View();
        }

        // GET: EntryInWayTables
        public ActionResult EntryInWayTable(int? invoiceId)
        {
            ViewBag.QrCode = invoiceId;

            if (invoiceId == null) return null;
            
            #region Table Data

            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetInWayDetailsByOldInvoicId",
                new { OldInvoicId = invoiceId },
                commandType: CommandType.StoredProcedure);

            List<string> schema;
            var results = tableData.GetSchemaAndData(out schema);

            var model = new TableOption()
            {
                Id = "entryInWay",
                Schema = schema,
                Rows = results,
                DisplayRowsLength = 50,
                TotalFooterColumns = new string[] { "9", "تعداد" } // column by name "تعداد" and column by index 9
            };

            #endregion


            return PartialView("_EntryInWayTablePartial", model);
        }


        // GET: Warehouse
        public ActionResult SaleReturnInvoices()
        {
            ViewBag.Title = "برگشت از فروش";                       

            return View();
        }

        // GET: SaleReturnInvoicesTable
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
                TotalFooterColumns = new string[] { "مبلغ فاکتور" } // column by name "مبلغ فاکتور"
            };

            #endregion

            return PartialView("_SaleReturnInvoicesTablePartial", model);
        }
    }
}
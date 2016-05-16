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

            var encryptedQrCode = Request.QueryString["code"];
            //ViewBag.QrCode = encryptedQrCode?.Decrypt();
            ViewBag.QrCode = encryptedQrCode;



            return View();
        }

        

        // GET: EntryInWayTables
        public ActionResult EntryInWayTable(int? invoiceId)
        {
            ViewBag.QrCode = invoiceId;

            if (invoiceId == null) return null;

            // Fill Table data ------------------------------------------
            #region Table Data
            var tableData = AdoManager.ConnectionManager.Find(Properties.Settings.Default.SaleTabriz).SqlConn.ExecuteReader(
                "sp_GetInWayDetailsByOldInvoicId",
                new { OldInvoicId = invoiceId },
                commandType: CommandType.StoredProcedure);

            List<string> schema;
            var results = tableData.GetSchemaAndData(out schema);

            var model = Tuple.Create(schema, results);

            #endregion
            //-----------------------------------------------------------


            return PartialView("EntryInWayTable", model);
        }


    }
}
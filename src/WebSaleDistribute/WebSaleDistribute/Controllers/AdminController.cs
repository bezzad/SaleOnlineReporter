using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using Dapper;
using WebSaleDistribute.Core;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public AdminController()
        {
            Thread.CurrentThread.CurrentUICulture =
                Thread.CurrentThread.CurrentCulture =
                    CultureInfo.GetCultureInfo("fa-IR");
        }

        public ActionResult CheckAndRepairUser(string empId)
        {
            if (string.IsNullOrEmpty(empId)) return View();

            using (var multi = Connections.SaleDistributeIdentity.SqlConn.QueryMultiple("sp_CheckAndCreateEmployeesRoles", new { EmployeId = empId }, commandType: CommandType.StoredProcedure))
            {
                var customer = multi.Read(); // don't use .ToDataTable() here because it is read twice !!!
                var menus = multi.Read(); // don't use .ToDataTable() here because it is read twice !!!

                var optCustomer = new TableOption()
                {
                    Data = customer.ToDataTable()
                };

                var optMenus = new TableOption()
                {
                    Data = menus.ToDataTable()
                };

                ApplicationDbContext.Create().SaveChanges();
                
                return View(Tuple.Create(optCustomer, optMenus));
            }
        }

    }
}
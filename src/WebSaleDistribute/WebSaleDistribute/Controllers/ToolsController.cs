using Dapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebSaleDistribute.Core;

namespace WebSaleDistribute.Controllers
{    
    public class ToolsController : Controller
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

        private static Dictionary<string, IEnumerable<dynamic>> LastUserRunningAction =
            new Dictionary<string, IEnumerable<dynamic>>();

        internal static void SetLastUserRunningAction(string username, string actionName, IEnumerable<dynamic> data)
        {
            string key = $"{username}_{actionName}";

            LastUserRunningAction[key] = data;
        }

        internal static IEnumerable<dynamic> GetLastUserRunningAction(string username, string actionName)
        {
            string key = $"{username}_{actionName}";

            if (!LastUserRunningAction.ContainsKey(key)) return null;

            return LastUserRunningAction[key];
        }



        private string GetFileName(string actionName, string extension)
        {
            return $"{actionName}_{DateTime.Now.GetPersianDate().Replace("/", "")}.{extension}";
        }

        [HttpGet, FileDownload]
        public FileContentResult ExportToExcel(string actionName)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            var data = GetLastUserRunningAction(currentUser.UserName, actionName);

            if (data == null) return null;

            var grid = new GridView();
            grid.DataSource = data.ToDataTable();
            grid.DataBind();

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            var utf8S = Encoding.UTF8.GetBytes(sw.ToString());
            var result = Encoding.UTF8.GetPreamble().Concat(utf8S).ToArray();
            
            return File(result, "application/ms-excel", GetFileName(actionName, "xls"));
        }
    }
}
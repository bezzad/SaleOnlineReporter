using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebSaleDistribute.Core;

namespace WebSaleDistribute.Controllers
{    
    [AllowAnonymous]    
    public class ToolsController : BaseController
    {
        private static readonly Dictionary<string, DataTable> LastUserRunningAction = new Dictionary<string, DataTable>();
        
        internal static void SetLastUserRunningAction(string username, string actionName, DataTable data)
        {
            string key = $"{username}_{actionName}";

            LastUserRunningAction[key] = data;
        }

        internal static DataTable GetLastUserRunningAction(string username, string actionName)
        {
            string key = $"{username}_{actionName}";

            if (!LastUserRunningAction.ContainsKey(key)) return null;

            return LastUserRunningAction[key];
        }



        private string GetFileName(string actionName, string extension)
        {
            return $"{actionName}_{DateTime.Now.GetPersianDateNumber()}.{extension}";
        }

        [HttpGet, FileDownload]
        public FileContentResult ExportToExcel(string username, string actionName)
        {
            var data = GetLastUserRunningAction(username, actionName);

            if (data == null) return null;

            var grid = new GridView();
            grid.DataSource = data;
            grid.DataBind();

            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            var utf8S = Encoding.UTF8.GetBytes(sw.ToString());
            var result = Encoding.UTF8.GetPreamble().Concat(utf8S).ToArray();
            
            return File(result, "application/ms-excel", GetFileName(actionName, "xls"));
        }
    }
}
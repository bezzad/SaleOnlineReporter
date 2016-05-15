using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using WebSaleDistribute.Core;

namespace WebSaleDistribute.Models
{
    public class DynamicModels
    {
        public static IEnumerable<ExpandoObject> GetMenus()
        {
            var menuScript = HelperExtensions.ReadResourceFile(Properties.Settings.Default.QueryMenu);
            var result = AdoManager.DataAccessObject.GetFromQuery(menuScript, false); // fetch menu by not caching data
            var menus = result.Select(menu => (ExpandoObject)HelperExtensions.DapperRowToExpando(menu));

            return menus;
        }

    }
}
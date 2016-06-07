using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using WebSaleDistribute.Core;
using Dapper;

namespace WebSaleDistribute.Models
{
    public class DynamicModels
    {
        public static IEnumerable<ExpandoObject> GetMenus(string userId)
        {
            // fetch menu by caching data
            var result = AdoManager.DataAccessObject.GetFromQuery($"EXEC dbo.sp_CreateAndGetWebSaleDistributeMenus @UserId = {userId}", true); 
            var menus = result.Select(menu => (ExpandoObject)HelperExtensions.DapperRowToExpando(menu));

            return menus;
        }

    }
}
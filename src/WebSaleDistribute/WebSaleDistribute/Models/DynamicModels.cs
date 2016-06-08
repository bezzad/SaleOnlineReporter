using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Principal;
using WebSaleDistribute.Core;

namespace WebSaleDistribute.Models
{
    public static class DynamicModels
    {
        public static IEnumerable<ExpandoObject> GetMenus(this IPrincipal user)
        {
            var userId = user?.Identity?.GetUserId();
            if (userId != null)
            {
                if (user.IsInRole("Admin")) userId = "100";

                // fetch menu by caching data
                var result = AdoManager.DataAccessObject.GetFromQuery($"EXEC dbo.sp_CreateAndGetWebSaleDistributeMenus @UserId = {userId}", true);
                var menus = result.Select(menu => (ExpandoObject)HelperExtensions.DapperRowToExpando(menu));

                return menus;
            }

            return null;
        }

    }
}
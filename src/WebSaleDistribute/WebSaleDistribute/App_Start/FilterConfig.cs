using System.Web;
using System.Web.Mvc;
using WebSaleDistribute.App_Start.Owin;

namespace WebSaleDistribute
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ElmahHandleErrorAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}

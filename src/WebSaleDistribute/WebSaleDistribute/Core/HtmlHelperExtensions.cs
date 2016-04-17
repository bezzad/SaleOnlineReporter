using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebSaleDistribute.Core
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, string text, string action, string controller, string liCssClass = null)
        {
            var li = new TagBuilder("li");
            if (!String.IsNullOrEmpty(liCssClass))
            {
                li.AddCssClass(liCssClass);
            }
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase))
            {
                li.AddCssClass("active");
            }
            li.InnerHtml = String.Format("<a href=\"{0}\"><i class=\"glyphicon glyphicon-chevron-right\"></i>{1}</a>",
               new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(action, controller).ToString()
                , text);
            return MvcHtmlString.Create(li.ToString());
        }

        public static MvcHtmlString BoxItem(this HtmlHelper htmlHelper, string innerBody, string title, string divCssClass = null)
        {
            var div = new TagBuilder("div");
            if (!String.IsNullOrEmpty(divCssClass))
            {
                div.AddCssClass(divCssClass);
            }

            div.InnerHtml = $@"<div class='panel panel-default'>
                                    <div class='panel-heading panel-options'>
                                        <h3 class='panel-title'>
                                            {title}
                                            <a href='#' data-rel='fullscreen' style='float: right'><i class='glyphicon glyphicon-fullscreen'></i></a>
                                            <a href='#' data-rel='collapse' style='float: right'><i class='glyphicon glyphicon-refresh'>&nbsp;</i></a>
                                            <a href='#' data-rel='reload' style='float: right'><i class='glyphicon glyphicon-cog'>&nbsp;</i></a>
                                        </h3>
                                    </div>
                                    <div class='panel-body'>
                                        <!-- Body -->
                                        {innerBody}
                                        <div class='text-right'>
                                            <a href = '#'> نمایش جزئیات <span class='glyphicon glyphicon-circle-arrow-right'></span></a>
                                        </div>
                                    </div>
                               </div>";

            return MvcHtmlString.Create(div.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
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
               new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(action, controller).ToString(), text);
            return MvcHtmlString.Create(li.ToString());
        }

        /// <summary>
        /// Create a Panel
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="body"></param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="glyphicon">glyphicon name, for more detail see: <see cref="http://getbootstrap.com/components/"></see>/></param>
        /// <param name="divCssClass"></param>
        /// <param name="pType"></param>
        /// <param name="hasSetting"></param>
        /// <returns></returns>
        public static MvcHtmlString PanelItem(this HtmlHelper htmlHelper, PanelItemOption option)
        {
            var id = Guid.NewGuid().ToString();
            var div = new TagBuilder("div");
            div.Attributes.Add("id", "parent-" + id);
            div.Attributes.Add("data-sortable", "true");
            div.AddCssClass("panel panelitem");
            div.AddCssClass($"panel-{option.PanelType.ToString()}");

            if (option.CssClass != null && option.CssClass.Length > 0)
            {
                foreach (var divCssClass in option.CssClass)
                {
                    div.AddCssClass(divCssClass);
                }
            }

            var settingButton = option.HasSettingPanel ? $@"
                                        <ul class='dropdown-menu dropdown-menu-right'>
                                            <li>
                                                <a onclick='showSettingPanel(""{id}"");' data-tooltip='انتخاب پارامتر' >
                                                    <i class='panel-control-icon glyphicon glyphicon-wrench'></i>
                                                    <span class='control-title'>انتخاب پارامتر</span>
                                                </a>
                                            </li>
                                        </ul><div class='dropdown-toggle' data-toggle='dropdown'><span class='panel-control-icon glyphicon glyphicon-wrench'></span></div>
                                       " : "";

            var detailLink = $@"<hr/>
                               <div class='text-right' style='padding-right: 10px; padding-bottom: 10px;'>
                                   <a href='{option.Url}'> نمایش جزئیات <span class='glyphicon glyphicon-circle-arrow-right'></span></a>
                               </div>";

            div.InnerHtml = $@"<div class='panel-heading'>
                                    <div class='panel-title'>
                                        <i class='glyphicon glyphicon-{option.GlyphIcon}'>&nbsp;</i>
                                        { option.Title}
                                    </div>
                                    <div class='dropdown'>"
                                    + settingButton + $@"
                                   </div>
                               </div>
                               <div class='panel-body'>
                                   {option.Body}
                               </div>
                               ";



            var result = div.ToString();
            result += option.HasSettingPanel ? detailLink : "";
            result += Environment.NewLine;
            result += option.HasSettingPanel ? htmlHelper.SettingPanelItem(id, option.Title, option.PanelType).ToString() : "";

            return MvcHtmlString.Create(result);
        }

        public static MvcHtmlString SettingPanelItem(this HtmlHelper htmlHelper, string id, string title, PanelType pType)
        {
            string settingTitle = "تنظیمات";
            var div = new TagBuilder("div");
            div.Attributes.Add("id", id);
            div.Attributes.Add("style", "display: none;");
            div.AddCssClass("panel spanel");
            div.AddCssClass($"panel-{pType.ToString()}");

            div.InnerHtml = $@"<div class='panel-heading'>
                                    <div class='panel-title'>            
                                       {settingTitle + " " + title}
                                    </div>
                               </div>
                               <div class='panel-body'>
                                   Filters
                               </div>";

            return MvcHtmlString.Create(div.ToString());
        }

        public static MvcHtmlString TableItem(this HtmlHelper htmlHelper, string id, List<string> schema, List<ExpandoObject> rows)
        {
            var div = new TagBuilder("table");
            div.Attributes.Add("id", id);
            div.Attributes.Add("cellspacing", "0");
            div.Attributes.Add("width", "100%");
            div.AddCssClass("dataTables");
            div.AddCssClass("display");
            div.AddCssClass("hover");
            div.AddCssClass("order-column");
            div.AddCssClass("stripe");



            var thHeader = "";
            foreach (var colName in schema)
                thHeader += $"<th>{colName}</th>{Environment.NewLine}";

            var header = $@"
                            <thead>
                                <tr>
                                   {thHeader}
                                </tr>
                            </thead>
                            <tfoot>
                                <tr>
                                     {thHeader}
                                </tr>
                            </tfoot>{Environment.NewLine}";

            var tRows = "";
            foreach (IDictionary<string, object> row in rows)
            {
                var tds = "";
                foreach (var col in schema)
                {
                    tds += $"<td>{row[col]}</td>{Environment.NewLine}";
                }
                tRows += $"<tr>{Environment.NewLine}{tds}{Environment.NewLine}</tr>";
            }

            var body = $"{header}<tbody>{Environment.NewLine}{tRows}{Environment.NewLine}</tbody>";

            div.InnerHtml = body;

            return MvcHtmlString.Create(div.ToString());
        }
    }


}

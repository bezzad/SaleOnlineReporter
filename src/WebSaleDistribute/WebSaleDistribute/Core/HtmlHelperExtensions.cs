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
        public static MvcHtmlString PanelItem(this HtmlHelper htmlHelper, string body, string title, string url, string glyphicon = null, string divCssClass = null, PanelType pType = PanelType.@default, bool hasSetting = true)
        {
            var id = Guid.NewGuid().ToString();
            var div = new TagBuilder("div");
            div.Attributes.Add("id", "parent-" + id);
            div.Attributes.Add("data-sortable", "true");
            div.AddCssClass("panel panelitem");
            div.AddCssClass($"panel-{pType.ToString()}");

            if (!String.IsNullOrEmpty(divCssClass))
            {
                div.AddCssClass(divCssClass);
            }

            var settingButton = $@"
                                        <ul class='dropdown-menu dropdown-menu-right'>
                                            <li>
                                                <a onclick='showSettingPanel(""{id}"");' data-tooltip='انتخاب پارامتر' >
                                                    <i class='panel-control-icon glyphicon glyphicon-wrench'></i>
                                                    <span class='control-title'>انتخاب پارامتر</span>
                                                </a>
                                            </li>
                                        </ul><div class='dropdown-toggle' data-toggle='dropdown'><span class='panel-control-icon glyphicon glyphicon-wrench'></span></div>
                                       ";

            //TODO clear this code:
            settingButton = "";

            div.InnerHtml = $@"<div class='panel-heading'>
                                    <div class='panel-title'>
                                        <i class='glyphicon glyphicon-{glyphicon}'>&nbsp;</i>
                                        { title}
                                    </div>
                                    <div class='dropdown'>"
                                    + settingButton + $@"
                                   </div>
                               </div>
                               <div class='panel-body'>
                                   {body}
                               </div>
                               <hr/>
                               <div class='text-right' style='padding-right: 10px; padding-bottom: 10px;'>
                                   <a href='{url}'> نمایش جزئیات <span class='glyphicon glyphicon-circle-arrow-right'></span></a>
                               </div>";

            var result = div.ToString();
            result += Environment.NewLine;
            result += htmlHelper.SettingPanelItem(id, title, pType).ToString();

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
    }

    public enum PanelType
    {
        warning,
        danger,
        info,
        success,
        primary,
        @default
    }
}

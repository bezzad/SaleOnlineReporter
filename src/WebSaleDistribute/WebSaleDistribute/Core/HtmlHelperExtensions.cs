using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebSaleDistribute.Core.Enums;
using ZXing;
using ZXing.Common;

namespace WebSaleDistribute.Core
{
    public static class HtmlHelperExtensions
    {
        private static int _counter = 0;

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
            var div = new TagBuilder("div");
            div.Attributes.Add("id", "parent-" + option.Id);
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
                                                <a onclick='showSettingPanel(""{option.Id}"");' data-tooltip='انتخاب پارامتر' >
                                                    <i class='panel-control-icon glyphicon glyphicon-wrench'></i>
                                                    <span class='control-title'>انتخاب پارامتر</span>
                                                </a>
                                            </li>
                                        </ul><div class='dropdown-toggle' data-toggle='dropdown'><span class='panel-control-icon glyphicon glyphicon-wrench'></span></div>
                                       " : "";


            var refreshLink = option.IsRefreshable ?
                                     $@" <ul class='dropdown-menu dropdown-menu-right'>
                                            <li>
                                                <a onclick='$(""#{option.Id}"").submit();' >
                                                    <i id='btn_loading_{option.Id}' class='panel-control-icon glyphicon glyphicon-refresh'></i>
                                                    <i id='loading_{option.Id}' style='display: none;' class='panel-control-icon glyphicon glyphicon-refresh glyphicon-refresh-animate'></i>
                                                    <span class='control-title'>بروز رسانی</span>
                                                </a>
                                            </li>
                                        </ul><div class='dropdown-toggle' data-toggle='dropdown'><span class='panel-control-icon glyphicon glyphicon-refresh'></span></div>
                                        " : "";

            var printable = string.IsNullOrEmpty(option.ExportToExcelAction) ? "":
                                     $@"<ul class='dropdown-menu dropdown-menu-right'>
                                            <li>
                                                <a href='{option.ExportToExcelAction}' target='_parent' class='fileDownload' data-tooltip='خروج به اکسل' >
                                                    <i class='panel-control-icon glyphicon glyphicon-save-file'></i>
                                                    <span class='control-title'>خروج به اکسل</span>
                                                </a>
                                            </li>
                                        </ul><div class='dropdown-toggle' data-toggle='dropdown'><span class='panel-control-icon glyphicon glyphicon-save-file'></span></div>
                                        ";

            var detailLink = $@"<hr/>
                               <div class='text-right' style='padding-right: 10px; padding-bottom: 10px;'>
                                   <a href='{option.Url}'> نمایش جزئیات <span class='glyphicon glyphicon-circle-arrow-right'></span></a>
                               </div>";

            div.InnerHtml = $@"<div class='panel-heading'>
                                    <div class='panel-title'>
                                        <i class='glyphicon glyphicon-{option.GlyphIcon}'>&nbsp;</i>
                                        {option.Title}
                                    </div>
                                    <div class='dropdown'>"
                                    + printable + settingButton + refreshLink + $@"
                                   </div>
                               </div>
                               <div class='panel-body'>
                                   {option.Body}
                               </div>
                               ";



            var result = div.ToString();
            result += option.HasSettingPanel ? detailLink : "";
            result += Environment.NewLine;
            result += option.HasSettingPanel ? htmlHelper.SettingPanelItem(option.Id, option.Title, option.PanelType).ToString() : "";

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

        public static MvcHtmlString TableItem(this HtmlHelper htmlHelper, string id, List<string> schema, List<ExpandoObject> rows, params Tuple<int, OrderType>[] orders)
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

            if (orders != null && orders.Any())
            {
                var val = "[";
                foreach (Tuple<int, OrderType> order in orders)
                {
                    val += $@"[ {order.Item1}, ""{order.Item2.ToString()}"" ], ";
                }
                val = val.Substring(0, val.Length - 2) + "]";

                div.Attributes.Add("data-order", val);
            }



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

        public static DotNet.Highcharts.Highcharts GetHighChart(ChartOption option)
        {
            DotNet.Highcharts.Highcharts highChart = new DotNet.Highcharts.Highcharts(option.Name);

            var xa = new XAxis() { Type = AxisTypes.Category };
            if (option.XAxisData != null)
                xa.Categories = option.XAxisData;

            var chart = new Chart
            {
                Type = option.ChartType,
                DefaultSeriesType = option.ChartType
            };

            if (option.AjaxLoading)
            {                
                chart.Events = new ChartEvents()
                {
                    Load = "FetchDataFunc",
                    Drilldown = "function(e) { DrillDownFunc(e) }",
                    Drillup = "function(e) { FetchDataFunc() }"
                };
                chart.Style = "fontWeight: 'normal', fontFamily: 'IRANSans'";

                highChart.AddJavascripVariable("ChartParentUrl", '\"' + option.LoadDataUrl + '\"')
                .AddJavascripFunction("FetchDataFunc", $@"                            
                             $.get(ChartParentUrl, function (dataArr) {{ 
                            {option.Name}.series[0].setData(dataArr);
                         }});
                ")
                .AddJavascripFunction("DrillDownFunc",
                                $@"
                                if (!e.seriesOptions) {{                                                
                                                var tChart = {option.Name};
                                                tChart.showLoading('در حال بار گذاری داده ها');

                                                $.get(e.point.drilldown_url + ""/"" + e.point.id, function (dataArr) {{
                                                    drilldownData = {{
                                                        name: e.point.name,
                                                        id: e.point.id,
                                                        data: dataArr
                                                    }}
                                                    ChartParentUrl = dataArr[0].drillup_url;
                                                    tChart.hideLoading();
                                                    tChart.addSeriesAsDrilldown(e.point, drilldownData);
                                                }});

                                                tChart.setTitle({{
                                                    text: '{option.SubTitle}'
                                                }});
                                            }}
                ", "e");
            }

            highChart.InitChart(chart)
            .SetTitle(new Title
            {
                Text = option.Tilte
            })
            .SetSubtitle(new Subtitle
            {
                Text = option.SubTitle
            })
            .SetExporting(new Exporting { Enabled = true })
            .SetPlotOptions(new PlotOptions
            {
                Column = new PlotOptionsColumn()
                {
                    //Point = new PlotOptionsColumnPoint { Events = new PlotOptionsColumnPointEvents { Click = "ColumnPointClick" } },
                    Cursor = Cursors.Pointer,
                    DataLabels = new PlotOptionsColumnDataLabels
                    {
                        //Style = "fontWeight: 'bold', fontFamily: 'B Nazanin'",
                        Enabled = option.ShowDataLabels,
                        Color = Color.AliceBlue,
                        Formatter = "function() { return '<div style=\"color: black;\">' + this.y.toLocaleString('fa-IR'); + '</div>'; }"
                    }
                }
            })
            .SetXAxis(xa)
            .SetSeries(new Series
            {
                Type = option.ChartType,
                Name = option.SeriesName,
                Data = option.YAxisData ?? new Data(new object[0, 0]),
                PlotOptionsColumn = new PlotOptionsColumn() { ColorByPoint = option.ColorByPoint }                
            })
            .SetYAxis(new YAxis
            {
                Title = new YAxisTitle() { Text = option.YAxisTitle },
                Labels = new YAxisLabels
                {
                    Formatter = "function() { return '<div Locale=\"fa-IR\" style =\"color: black;\">'+ this.axis.defaultLabelFormatter.call(this) +'</div>'; }",
                    Style = "color: '#89A54E'",
                    Align = HorizontalAligns.Left,
                    X = 3,
                    Y = 16                    
                },
                ShowFirstLabel = false
               
            })
            .SetTooltip(new Tooltip
            {
                //UseHTML = true,
                //HeaderFormat = "<small dir=\"rtl\">{point.key}</small><table dir =\"rtl\">",
                //PointFormat = "<tr><td style=\"color= {series.color}\"></td>" +
                //                  "<td><b>{point.y} ریال</b></td></tr>",
                //FooterFormat = "</table>",
                //ValueDecimals = 0,
                //Style = "fontWeight: 'normal', fontFamily: 'B Nazanin'"
                Crosshairs = new Crosshairs(true),
                Formatter = "TooltipFormatter"
            })
            .AddJavascripFunction(
                    "TooltipFormatter",
                    @"var s = '<div dir=""rtl"">' + this.point.name +':<b>  '+ this.y.toLocaleString('fa-IR') +' ریال</b><br/>';
                      return s;"
                )
            .SetLegend(new Legend
            {
                Enabled = option.ShowLegend,
                Rtl = true,
                Align = HorizontalAligns.Left,
                VerticalAlign = VerticalAligns.Top,
                Y = 20,
                Floating = true,
                BorderWidth = 0
            })
            .SetOptions(new GlobalOptions()
            {
                Lang = new DotNet.Highcharts.Helpers.Lang()
                {
                    //DrillUpText = "",
                    Loading = "در حال بارگزاری",
                    PrintButtonTitle = "چاپ",
                    ThousandsSep = ",",
                    DecimalPoint = ".",
                    DownloadJPEG = "JPEG دانلود عکس",
                    DownloadPDF = "PDF دانلود در قالب",
                    DownloadPNG = "PNG دانلود عکس",
                    DownloadSVG = "SGV دانلود فایل",
                    ExportButtonTitle = "خروج"
                }
            });



            return highChart;
        }

        public static MvcHtmlString GetHighChart(this HtmlHelper htmlHelper, ChartOption option)
        {
            return new MvcHtmlString(GetHighChart(option).ToHtmlString());
        }

        public static MvcHtmlString GetNewNo(this HtmlHelper htmlHelper)
        {
            return new MvcHtmlString(_counter++.ToString());
        }

        public static MvcHtmlString GenerateRelayQrCode(this HtmlHelper html, string qrValue, int height = 250, int width = 250, int margin = 0)
        {
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };

            using (var bitmap = barcodeWriter.Write(qrValue))
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                var img = new TagBuilder("img");
                img.MergeAttribute("alt", "qr tag");
                img.Attributes.Add("src", String.Format("data:image/gif;base64,{0}",
                    Convert.ToBase64String(stream.ToArray())));

                return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
            }
        }
    }
}
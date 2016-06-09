using DotNet.Highcharts.Enums;
using System.Collections.Generic;

namespace WebSaleDistribute.Core
{
    public class ChartOption
    {
        public string Name { get; set; }
        public ChartTypes ChartType { get; set; }
        public string[] XAxisData { get; set; }
        public DotNet.Highcharts.Helpers.Data YAxisData { get; set; }
        public string Tilte { get; set; }
        public string SubTitle { get; set; }
        public string SubTitleFunc { get; set; } = null;
        public string YAxisTitle { get; set; }
        public string SeriesName { get; set; }
        public bool ShowLegend { get; set; } = true;
        public bool ShowDataLabels { get; set; } = false;
        public string DataLabelsFormat { get; set; } = "ریال {point.y:,.0f}";
        public string LoadDataUrl { get; set; }
        public bool ColorByPoint { get; set; } = true;
        public bool AjaxLoading { get; set; }
        public List<KeyValuePair<string, string>> AjaxRoutParams { get; set; } = new List<KeyValuePair<string, string>>();
    }
}
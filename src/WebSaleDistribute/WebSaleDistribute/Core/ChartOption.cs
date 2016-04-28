using DotNet.Highcharts.Enums;

namespace WebSaleDistribute.Core
{
    public class ChartOption
    {
        public string Name { get; set; }
        public ChartTypes ChartType { get; set; }
        public string[] XAxisData { get; set; }
        public object[] YAxisData { get; set; }
        public string Tilte { get; set; }
        public string SubTitle { get; set; }
        public string YAxisTitle { get; set; }
        public string SeriesName { get; set; }
        public bool ShowLegend { get; set; } = true;
        public bool ShowDataLabels { get; set; } = false;
        public string DataLabelsFormat { get; set; } = "ریال {point.y: .0f}";
    }
}
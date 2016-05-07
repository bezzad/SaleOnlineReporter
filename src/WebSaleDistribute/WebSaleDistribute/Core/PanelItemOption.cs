using WebSaleDistribute.Core.Enums;

namespace WebSaleDistribute.Core
{
    public class PanelItemOption
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string GlyphIcon { get; set; }
        public string[] CssClass { get; set; }
        public PanelType PanelType { get; set; }
        public bool HasSettingPanel { get; set; }
        public bool HasDetailLink { get; set; }
        public bool IsRefreshable { get; set; }
        public string ExportToExcelAction { get; set; }
    }
}
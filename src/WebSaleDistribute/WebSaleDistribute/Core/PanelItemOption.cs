using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSaleDistribute.Core
{
    public class PanelItemOption
    {
        public string Body { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string GlyphIcon { get; set; }
        public string[] CssClass { get; set; }
        public PanelType PanelType { get; set; }
        public bool HasSettingPanel { get; set; }
        public bool HasDetailLink { get; set; }

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

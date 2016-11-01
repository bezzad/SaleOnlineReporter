using System;
using System.Collections.Generic;
using System.Data;

namespace WebSaleDistribute.Core
{
    public class TableOption
    {
        public string Id { get; set; }
        public DataTable Data { get; set; }
        public Tuple<int, OrderType>[] Orders { get; set; }
        public string[] TotalFooterColumns { get; set; }
        public string[] AverageFooterColumns { get; set; }
        public int[] CurrencyColumns { get; set; } = new int[0];
        public int DisplayRowsLength { get; set; } = 10;
        public bool AutoWidth { get; set; } = true;
        public bool Checkable { get; set; } = false;
        public string AjaxUrl { get; set; } = null;
        public Dictionary<string, InputOption> InputColumnsDataMember { get; set; } = new Dictionary<string, InputOption>();
    }
}
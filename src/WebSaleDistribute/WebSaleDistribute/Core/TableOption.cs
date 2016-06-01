using System;
using System.Collections.Generic;
using System.Dynamic;

namespace WebSaleDistribute.Core
{
    public class TableOption
    {
        public string Id { get; set; }
        public List<string> Schema { get; set; }
        public List<ExpandoObject> Rows { get; set; }
        public Tuple<int, OrderType>[] Orders { get; set; }
        public string[] TotalFooterColumns { get; set; }
        public string[] AverageFooterColumns { get; set; }
        public int DisplayRowsLength { get; set; } = 10;
        public bool AutoWidth { get; set; } = true;

    }
}
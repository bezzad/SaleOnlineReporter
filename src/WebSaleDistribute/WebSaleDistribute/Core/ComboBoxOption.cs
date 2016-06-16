using System;
using System.Collections.Generic;
using WebSaleDistribute.Core.Enums;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Core
{
    /// <summary>
    /// https://silviomoreto.github.io/bootstrap-select/examples/
    /// </summary>
    public class ComboBoxOption
    {
        /// <summary>
        /// Combo element id attribute
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// options data
        /// </summary>
        public List<ComboBoxDataModel> Data { get; set; } = new List<ComboBoxDataModel>();

        /// <summary>
        /// You can set the button classes via the data-style attribute.
        /// </summary>
        public DataStyleType DataStyle { get; set; } = DataStyleType.none;
        
        /// <summary>
        /// You can add a search input by passing data-live-search="true" attribute
        /// </summary>
        public bool DataLiveSearch { get; set; } = false;
        
        /// <summary>
        /// Multiple select boxes
        /// </summary>
        public bool MultipleSelection { get; set; } = false;

        /// <summary>
        /// Limit the number of options that can be selected via the data-max-options attribute. 
        /// It also works for option groups.
        /// </summary>
        public int DataMaxOptions { get; set; } = 100;

        /// <summary>
        /// Using the title attribute will set the default placeholder text when nothing is selected. 
        /// This works for both multiple and standard select boxes.
        /// </summary>
        public string Placeholder { get; set; } = null;

        /// <summary>
        /// You can also show the checkmark icon on standard select boxes with the show-tick class.
        /// </summary>
        public bool ShowTick { get; set; } = false;

        /// <summary>
        /// The Bootstrap menu arrow can be added with the show-menu-arrow class.
        /// </summary>
        public bool ShowMenuArrow { get; set; } = false;

        /// <summary>
        /// The size option is set to 'auto' by default. 
        /// When size is set to 'auto', the menu always opens up to show as many items as the window will allow without being cut off. 
        /// Set size to false to always show all items. The size of the menu can also be specifed using the data-size attribute.
        /// </summary>
        public int DataSize { get; set; } = 10;

        /// <summary>
        /// Alternatively, use the data-width attribute to set the width of the select. 
        /// Set data-width to 'auto' to automatically adjust the width of the select to its widest option. 
        /// 'fit' automatically adjusts the width of the select to the width of its currently selected option. 
        /// An exact value can also be specified, e.g., 300px or 50%.
        /// </summary>
        public string DataWidth { get; set; } = null;

        /// <summary>
        /// Disabled or enable select box
        /// </summary>
        public bool Enable { get; set; } = true;
        
        /// <summary>
        /// Wrap selects in grid columns, or any custom parent element, to easily enforce desired widths.
        /// </summary>
        public bool EnforceDesiredWidths { get; set; } = true;
    }
}

using System;
using System.Collections.Generic;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Core
{
    /// <summary>
    /// https://silviomoreto.github.io/bootstrap-select/examples/
    /// </summary>
    public class ComboBoxOption: InputOption
    {
        #region Members


        /// <summary>
        /// options data
        /// </summary>
        public List<ComboBoxDataModel> Data { get; set; } = new List<ComboBoxDataModel>();
        
        /// <summary>
        /// You can add a search input by passing data-live-search="true" attribute
        /// </summary>
        public bool DataLiveSearch { get; set; } = false;

        /// <summary>
        /// Multiple select boxes
        /// </summary>
        public bool MultipleSelection { get; set; } = false;

        /// <summary> 
        /// Specify how the selection is displayed with the data-selected-text-format attribute on a multiple select.
        /// The supported values are:
        /// 
        ///    <list type="bullet">
        ///       <item>
        ///          <description>values: A comma delimited list of selected values (default)</description>
        ///       </item>
        ///       <item>
        ///          <description>
        ///          count: If one item is selected, then the option value is shown. 
        ///          If more than one is selected then the number of selected items is displayed, e.g. 2 of 6 selected
        ///          </description>
        ///       </item>
        ///       <item>
        ///          <description>
        ///          count > x: Where x is the number of items selected when the display format changes from values to count
        ///          </description>
        ///       </item>
        ///       <item>
        ///          <description>static: Always show the select title (placeholder), regardless of selection</description>
        ///       </item>
        ///    </list>
        /// 
        /// reference: https://silviomoreto.github.io/bootstrap-select/examples/#custom-button-text
        /// </summary>
        public string MultipleSelectedTextFormat { get; set; } = "count > 2";

        /// <summary>
        /// Limit the number of options that can be selected via the data-max-options attribute. 
        /// It also works for option groups.
        /// </summary>
        public int? DataMaxOptions { get; set; } = null;
        
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
        public string DataSize { get; set; } = "auto";

        /// <summary>
        /// Alternatively, use the data-width attribute to set the width of the select. 
        /// Set data-width to 'auto' to automatically adjust the width of the select to its widest option. 
        /// 'fit' automatically adjusts the width of the select to the width of its currently selected option. 
        /// An exact value can also be specified, e.g., 300px or 50%.
        /// </summary>
        public string DataWidth { get; set; } = null;

        /// <summary>
        /// Select/deselect all options
        /// 
        /// Adds two buttons to the top of the menu - Select All & Deselect All with data-actions-box="true".
        /// </summary>
        public bool ShowSelectDeselectAllOptionsBox { get; set; } = false;

        /// <summary>
        /// Add a header to the dropdown menu, e.g. header: 'Select a condiment' or data-header="Select a condiment"
        /// </summary>
        public string MenuHeaderText { get; set; } = null;

        
        /// <summary>
        /// Wrap selects in grid columns, or any custom parent element, to easily enforce desired widths.
        /// </summary>
        public bool EnforceDesiredWidths { get; set; } = true;

        /// <summary>
        /// Add subtext to an option or optgroup with the data-subtext attribute.
        /// </summary>
        public bool ShowOptionSubText { get; set; } = false;

        #endregion
    }
}

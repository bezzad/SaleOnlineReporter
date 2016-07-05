using System;
using WebSaleDistribute.Core.Enums;

namespace WebSaleDistribute.Core
{
    /// <summary>
    /// http://www.w3schools.com/tags/tag_input.asp
    /// </summary>
    public class InputOption
    {
        /// <summary>
        /// Combo element id attribute
        /// </summary>
        public string Id => Guid.NewGuid().ToString();

        /// <summary>
        /// Specifies whether an <code><input/></code> element should have autocomplete enabled
        /// </summary>
        public bool AutoComplete { get; set; } = false;

        /// <summary>
        /// Specifies that an <code><input/></code> element should automatically get focus when the page loads
        /// </summary>
        public bool AutoFocus { get; set; } = false;

        /// <summary>
        /// Specifies that an <code><input/></code> element should be pre-selected 
        /// when the page loads (for type="checkbox" or type="radio")
        /// </summary>
        public bool Checked { get; set; } = false;

        /// <summary>
        /// Specifies the maximum value for an <code><input/></code> element
        /// </summary>
        public double? Max { get; set; } = null;

        /// <summary>
        /// Specifies a minimum value for an <code><input/></code> element
        /// </summary>
        public double? Min { get; set; } = null;


        /// <summary>
        /// Specifies the legal number intervals for an input field
        /// </summary>
        public double? Step { get; set; } = null;

        /// <summary>
        /// You can set the button classes via the data-style attribute.
        /// </summary>
        public DataStyleType DataStyle { get; set; } = DataStyleType.none;

        /// <summary>
        /// Using the title attribute will set the default placeholder text when nothing is selected. 
        /// This works for both multiple and standard select boxes.
        /// </summary>
        public string Placeholder { get; set; } = null;

        /// <summary>
        /// Specifies that an <code><input/></code> element should be disabled
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// Specifies that an input field is read-only
        /// </summary>
        public bool ReadOnly { get; set; } = false;


        /// <summary>
        /// Specifies the type <code><input/></code> element to display
        /// </summary>
        public string Type { get; set; } = InputTypes.Text;

        /// <summary>
        /// Specifies the value of an <code><input/></code> element
        /// </summary>
        public string Value { get; set; } = null;

        /// <summary>
        /// Specifies the name of an <code><input/></code> element
        /// </summary>
        public string Name { get; set; } = null;
    }
}
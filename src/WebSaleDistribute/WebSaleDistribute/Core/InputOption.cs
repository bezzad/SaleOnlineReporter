using System;
using WebSaleDistribute.Core.Enums;

namespace WebSaleDistribute.Core
{
    public class InputOption
    {
        /// <summary>
        /// Combo element id attribute
        /// </summary>
        public string Id => Guid.NewGuid().ToString();

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
        /// Disabled or enable select box
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}
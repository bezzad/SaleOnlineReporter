using System;
using System.Collections.Generic;

namespace WebSaleDistribute.Core
{
    public class MultipleStepProgressTabOption
    {
        /// <summary>
        /// Step form's element id attribute
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Sorted steps text data
        /// </summary>
        public List<string> Steps { get; set; }

        /// <summary>
        /// Select any nodes from zero to this index
        /// </summary>
        public int CurrentStepIndex { get; set; }

        /// <summary>
        /// Background style (css) of selected nodes.
        /// </summary>
        public string SelectedNodeBackground { get; set; } = "#1fb559";

        /// <summary>
        /// Background style (css) of nodes.
        /// </summary>
        public string NodeBackground { get; set; } = "#fafafa";

        /// <summary>
        /// Fore color and border color style (css) of nodes.
        /// </summary>
        public string NodeBorderColor { get; set; } = "#bfc2cc";

        /// <summary>
        /// Text color style (css) of steps.
        /// </summary>
        public string StepTextColor { get; set; } = "#696c71";
    }
}

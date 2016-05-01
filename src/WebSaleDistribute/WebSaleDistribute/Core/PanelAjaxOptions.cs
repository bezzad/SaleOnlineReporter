using System;
using System.Web.Mvc.Ajax;

namespace WebSaleDistribute.Core
{
    public class PanelAjaxOptions : AjaxOptions
    {
        public PanelAjaxOptions(string id)
        {
            LoadingElementId = "loading_" + id;
            InsertionMode = InsertionMode.Replace;
            OnSuccess = "loadPanels();";
            OnBegin = $@"$(""#btn_loading_{id}"").css(""display"", ""none"");";
            OnComplete = $@"$(""#btn_loading_{id}"").removeAttr(""style"");";
        }
    }
}

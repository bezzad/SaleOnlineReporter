namespace WebSaleDistribute.Models
{
    public class ComboBoxDataModel
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string SubText { get; set; } = null;

        public bool IsDividerLine { get; set; } = false;
    }
}

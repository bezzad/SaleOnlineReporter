using System.ComponentModel.DataAnnotations;

namespace WebSaleDistribute.Models
{
    public class SginInWHCode
    {

        [Display(Name = "ردیف")]
        public string Row { get; set; }

        [Display(Name = "شماره")]
        public string WHCode { get; set; }

        [Display(Name = "تاریخ")]
        public string RegDate { get; set; }

        [Display(Name = "کد محصول")]
        public string ProductCode { get; set; }


        [Display(Name = "نام محصول ")]
        public string ProductName { get; set; }

        [Display(Name = "توضیحات")]
        public string ProductDescription { get; set; }

        [Display(Name = " تعداد")]
        public int Qty { get; set; }
        
    }
 
}

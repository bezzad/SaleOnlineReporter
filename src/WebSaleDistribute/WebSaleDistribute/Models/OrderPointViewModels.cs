using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebSaleDistribute.Models
{
    
    public class OrderPointViewModels
    {
        [Display(Name = "ردیف")]
        public string RowNumber { get; set; }

        [Display(Name = "نام مشتری")]
        public string PersonName { get; set; }

        [Display(Name = "کد مشتری")]
        public string CustomerID { get; set; }


        [Display(Name = "زمان ")]
        public string OrderDate { get; set; }

        [Display(Name = "وضعیت")]
        public string GPSStatus { get; set; }

        [Display(Name = " باطری")]
        public int BatteryStatus { get; set; }


        [Display(Name = "دقت")]
        public int Accuracy { get; set; }

        [Display(Name = "شماره ")]
        public int OrderNo { get; set; }


        [Display(Name = "عرض ")]
        public double Latitude { get; set; }

        [Display(Name = "طول ")]
        public double Longitude { get; set; }
    }
 
}

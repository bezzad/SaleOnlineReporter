using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebSaleDistribute.Models
{
    
    public class OrderPointViewModels
    {
        [Display(Name = "نام مشتری")]
        public string PersonName { get; set; }

        [Display(Name = "کد مشتری")]
        public string PersonID { get; set; }


        [Display(Name = "وضعیت")]
        public int GPSStatus { get; set; }

        [Display(Name = "درصد باطری")]
        public int BatteryStatus { get; set; }


        [Display(Name = "دقت")]
        public int Accuracy { get; set; }

        [Display(Name = "شماره درخواست")]
        public int OrderNo { get; set; }


        [Display(Name = "عرض جغرافیایی")]
        public double Latitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        public double Longitude { get; set; }
    }
 
}

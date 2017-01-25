using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebSaleDistribute.Models
{
    
    public class CustomerPointFilterModels
    {
        [Display(Name = "نام مشتری")]
        public string PersonName { get; set; }

        [Display(Name = "کد مشتری")]
        public string PersonID { get; set; }


        [Display(Name = "نام مسئول")]
        public string ContactLastName { get; set; }

        [Display(Name = "وضعیت")]
        public int Status { get; set; }

        [Display(Name = "عرض جغرافیایی")]
        public double Latitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        public double Longitude { get; set; }
    }
 
}

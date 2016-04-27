using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSaleDistribute.Models
{
    public class EmployeeTypeModels
    {
        public EmployeeTypeModels()
        { }


        public EmployeeTypeModels(int typeId, string typeName)
        {
            EmployeeTypeID = typeId;
            EmployeeTypeName = typeName;
        }


        public int EmployeeTypeID { get; set; }
        public string EmployeeTypeName { get; set; }
    }
}

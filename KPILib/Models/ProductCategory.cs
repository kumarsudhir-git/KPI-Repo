using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class ProductCategory
    {
        public int ProductCategoryID { get; set; }
        public string ProductCategoryName { get; set; }
        public bool IsDiscontinued { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    }
}

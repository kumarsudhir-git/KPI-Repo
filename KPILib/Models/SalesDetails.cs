using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class SalesDetails
    {
        [Key]
        public int SalesDetailsID { get; set; }
        public int SalesID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        [Display(Name = "Qty")]
        public int Qty { get; set; }
        public string Instructions { get; set; }
    }
}

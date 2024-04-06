using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class SalesDispatchItem
    {
        [Key]
        public int SalesDetailsID { get; set; }
        public int SalesID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        [Display(Name = "Qty Ordered")]
        public int QtyBooked { get; set; }
        public string Instructions { get; set; }

        [Display(Name = "Qty Blocked")]
        public int QtyBlocked { get; set; }

        [Display(Name = "Qty Balance")]
        public int QtyBal { get; set; }

        [Display(Name = "Qty To Dispatch")]
        public int QtyToDispatch { get; set; }

        [Display(Name = "Qty Dispatched")]
        public int QtyDispatched { get; set; }

        [Display(Name = "Qty In Stock")]
        public int QtyAvailable { get; set; }

        [Display(Name = "Qty To Produce")]
        public int QtyToProduce { get; set; }

        [Display(Name = "Qty In Production")]
        public int QtyInProduction { get; set; }
    }
}

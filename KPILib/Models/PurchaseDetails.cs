using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class PurchaseDetails
    {
        [Key]
        public int PurchaseDetailsID { get; set; }
        public int PurchaseID { get; set; }
        [Display(Name = "Item Type")]
        public int RawMatarialID { get; set; }
        public string RMGrade { get; set; }
        public string RawMatarialName { get; set; }

        [Display(Name = "Qty (in Kgs)")]
        public int Qty { get; set; }
        public string Instructions { get; set; }
    }
}

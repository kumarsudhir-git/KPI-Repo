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
        [Display(Name = "Spl Instruction")]
        public string Instructions { get; set; }
        [Display(Name = "Colour")]
        public string Color { get; set; }
        public string Gms { get; set; }
        public string GMSInfo { get; set; }
        public string Package { get; set; }
        public string Rate { get; set; }
        public bool SampleReq { get; set; }
        [Display(Name = "Status")]
        public int SalesStatusID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KPILib.Models
{
    public class ProductRawMaterialMapping
    {
        public int ProductRMMappingID { get; set; }
        public int ProductID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Raw Material")]
        public int RawMaterialID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "RM Reqd for Min Qty (Kgs)")]
        public int RMReqdForUOMQty { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Unit Type")]
        public byte UnitType { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}

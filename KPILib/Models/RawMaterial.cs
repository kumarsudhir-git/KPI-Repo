using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public enum enumPurchaseStatus
    {
        PO_Raised = 10,
        Part_Rcvd = 20,
        Full_Rcvd__Closed = 30,
        Cancelled = 999
    }
    public class RawMaterial
    {
        [Key]
        public int RawMaterialID { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Required(ErrorMessage = "Required")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "{0} should be minimum 1 characters and a maximum of 40 characters")]
        [DataType(DataType.Text)]
        [Display(Name = "Raw Material")]

        public string RawMaterialName { get; set; }
        [Display(Name = "RM Grade")]
        [Required(ErrorMessage = "Required")]
        public string RMGrade { get; set; }

        
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "{0} should be minimum 1 characters and a maximum of 1000 characters")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Unit of Measurement")]
        public int UOMID { get; set; }
        [Display(Name = "Unit of Measurement")]
        public string UOM { get; set; }
        public List<UOM> UOMs { get; set; }

        [Display(Name = "Discontinued?")]
        public bool IsDiscontinued { get; set; }
        public decimal InStock { get; set; }
        public decimal Reserved { get; set; }
        public decimal Short { get; set; }
        public decimal Ordered { get; set; }
        public decimal LoanedOut { get; set; }
        public decimal LoanedIn { get; set; }

        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "Required")]

        public int VendorId { get; set; }

        [Display(Name = "Supplier Details")]
        public string VendorName { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    }

    public class RawMaterialResponse
    {
        public RawMaterial data { get; set; }
        public ResponseObj Response { get; set; }

        public RawMaterialResponse()
        {
            this.data = new RawMaterial();
            this.Response = new ResponseObj();
        }
    }

    public class RawMaterialsResponse
    {
        public List<RawMaterial> data { get; set; }
        public ResponseObj Response { get; set; }

        public RawMaterialsResponse()
        {
            this.data = new List<RawMaterial>();
            this.Response = new ResponseObj();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class RackMaster
    {
        [Key]
        public int RackID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "{0} should be minimum 1 characters and a maximum of 15 characters")]
        [DataType(DataType.Text)]
        [Display(Name = "Rack No.")]
        public string RackNo { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }
        
        [Display(Name = "Discontinued?")] 
        public bool IsDiscontinued { get; set; }

        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public int PkgQty { get; set; }
        public int Pkts { get; set; }
        public int OpenPkts { get; set; }
        public string Location { get; set; }

        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    }

    public class RackMasterResponse
    {
        public RackMaster data { get; set; }
        public ResponseObj Response { get; set; }

        public RackMasterResponse()
        {
            this.data = new RackMaster();
            this.Response = new ResponseObj();
        }
    }

    public class RackMastersResponse
    {
        public List<RackMaster> data { get; set; }
        public ResponseObj Response { get; set; }

        public RackMastersResponse()
        {
            this.data = new List<RackMaster>();
            this.Response = new ResponseObj();
        }
    }
}

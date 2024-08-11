using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KPILib.Models
{
    public class VendorMasterModel
    {
        public int VendorId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string VendorName { get; set; }
        public string Notes { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ContactNumber { get; set; }
        public bool IsDiscontinued { get; set; }
        public int AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime AddedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class VendorMasterModelResponse
    {
        public VendorMasterModel vendorMaster { get; set; }
        public List<VendorMasterModel> data { get; set; }
        public ResponseObj Response { get; set; }

        public VendorMasterModelResponse()
        {
            this.vendorMaster = new VendorMasterModel();
            this.data = new List<VendorMasterModel>();
            this.Response = new ResponseObj();
        }
    }

}

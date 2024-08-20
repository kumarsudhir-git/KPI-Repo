using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class LocationMasterResponse
    {
        public LocationMasterModel LocationMasterModel { get; set; }
        public List<LocationMasterModel> data { get; set; }
        public ResponseObj Response { get; set; }

        public LocationMasterResponse()
        {
            LocationMasterModel = new LocationMasterModel();
            this.data = new List<LocationMasterModel>();
            this.Response = new ResponseObj();
        }
    }

    public class LocationMasterModel
    {
        public int LocationId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
        public int? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime AddedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}

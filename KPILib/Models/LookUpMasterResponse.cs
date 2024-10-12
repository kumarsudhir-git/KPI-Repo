using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public class LookUpMasterResponse
    {
        public LookUpMasterResponse()
        {
            lookupMasterData = new LookUpMasterModel();
            lookupMasterList = new List<LookUpMasterModel>();
            Response = new ResponseObj();
        }

        public LookUpMasterModel lookupMasterData { get; set; }
        public List<LookUpMasterModel> lookupMasterList { get; set; }
        public ResponseObj Response { get; set; }
    }

    public class LookUpMasterModel
    {
        public int LookUpID { get; set; }
        public string LookUpType { get; set; }
        public string LookUpName { get; set; }
        public string LookUpValue { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}

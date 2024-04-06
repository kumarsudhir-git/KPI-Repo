using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public class RoleMaster
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public DateTime AddedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int AddedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class RoleMasterResponse
    {
        public RoleMaster roleMasterObj { get; set; }
        public List<RoleMaster> data { get; set; }
        public ResponseObj Response { get; set; }

        public RoleMasterResponse()
        {
            this.roleMasterObj = new RoleMaster();
            this.data = new List<RoleMaster>();
            this.Response = new ResponseObj();
        }
    }
}

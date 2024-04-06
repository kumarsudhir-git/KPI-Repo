using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KPILib.Models
{
    public class RoleRights
    {
        public int RoleRightsID { get; set; }

        [Required(ErrorMessage = "Required")]
        public int ParentMenuID { get; set; }
        public int RoleID { get; set; }
        public string MenuName { get; set; }
        public int MenuID { get; set; }
        public bool Add { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool IsActive { get; set; }
    }

    public class RoleRightsResponse
    {
        public RoleRights RoleRightsObj { get; set; }
        public List<RoleRights> RoleRightsListObj { get; set; }
        public ResponseObj ResponseObj { get; set; }

        public RoleRightsResponse()
        {
            this.RoleRightsObj = new RoleRights();
            this.RoleRightsListObj = new List<RoleRights>();
            this.ResponseObj = new ResponseObj();
        }

    }

}

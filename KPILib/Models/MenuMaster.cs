using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public class MenuMaster
    {
        public int MenuID { get; set; }
        public int RoleID { get; set; }
        public string MenuCode { get; set; }
        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
        public int? MenuParentID { get; set; }
        public string Link { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsActive { get; set; }
    }

    public class MenuMasterResponse
    {
        public MenuMaster menuMasterObj { get; set; }
        public List<MenuMaster> menuMasterListObj { get; set; }
        public ResponseObj ResponseObj { get; set; }

        public MenuMasterResponse()
        {
            this.menuMasterObj = new MenuMaster();
            this.menuMasterListObj = new List<MenuMaster>();
            this.ResponseObj = new ResponseObj();
        }
    }

}

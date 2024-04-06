namespace KPILib.Models
{
    public class UserRoleRights
    {
        public int RoleRightsID { get; set; }
        public int RoleID { get; set; }
        public int MenuID { get; set; }
        public bool Add { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool IsActive { get; set; }
        public string MenuCode { get; set; }
        public string MenuName { get; set; }
        public int? MenuParentID { get; set; }
        public string MenuIcon { get; set; }
        public string Link { get; set; }

    }
}

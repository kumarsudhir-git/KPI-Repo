using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KPIWebAPI.Classes
{
    public static class CommonFunctions
    {
        public const string BASE_URL = "http://localhost:12345/External/ShowPO/";

        public static string DefaultDateFormat(this DateTime dt)
        {
            return dt.ToString("dd-MMM-yyyy");
        }

        public static void WriteToLog(string msg)
        {
            FileInfo fi = new FileInfo(HttpContext.Current.Server.MapPath("~/LOG_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt"));
            StreamWriter sw;
            string sLine = "";

            if (!fi.Exists)
                sw = fi.CreateText();
            else
                sw = fi.AppendText();

            sLine = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " :: " + msg;
            sw.WriteLine(sLine);

            sw.Close();
        }

        public static UserMaster GetUserMasterData(int UserId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                UserMaster userMasterObj = (from usrMstr in db.UserMasters
                                            where usrMstr.UserID == UserId
                                            && usrMstr.IsDiscontinued == false
                                            select usrMstr).FirstOrDefault();

                return userMasterObj;
            }
        }
        public static int ValidateUsername(string Username, int UserId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                int CountDuplicateUsername = (from usrMstr in db.UserMasters
                                              where usrMstr.UserID != UserId
                                              && usrMstr.Username != null
                                              && usrMstr.Username != ""
                                              && usrMstr.Username.Trim().ToLower() == Username.Trim().ToLower()
                                              && usrMstr.IsDiscontinued == false
                                              select usrMstr.UserID).Count();
                return CountDuplicateUsername;
            }
        }

        public static List<UserMaster> GetUserMasterListData(int UserId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<UserMaster> userMasters = (from usrMstr in db.UserMasters
                                                where usrMstr.IsDiscontinued == false
                                                orderby (usrMstr.LastModifiedOn != null ? usrMstr.LastModifiedOn : usrMstr.AddedOn)
                                                descending
                                                select usrMstr).ToList();
                if (UserId != 0)
                {
                    userMasters = (from usrLstData in userMasters
                                   where usrLstData.UserID == UserId
                                   select usrLstData).ToList();
                }
                return userMasters;
            }
        }

        public static RoleMaster GetRoleMasterData(int RoleId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                RoleMaster roleMasterObj = (from roleMstr in db.RoleMasters
                                            where roleMstr.RoleID == RoleId
                                            && roleMstr.IsDeleted == false
                                            select roleMstr).FirstOrDefault();

                return roleMasterObj;
            }
        }

        public static List<RoleMaster> GetRoleMasterListData(int RoleId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<RoleMaster> roleMasters = (from roleMstr in db.RoleMasters
                                                where roleMstr.IsDeleted == false
                                                orderby (roleMstr.ModifiedOn != null ? roleMstr.ModifiedOn : roleMstr.AddedOn)
                                                descending
                                                select roleMstr).ToList();
                if (RoleId != 0)
                {
                    roleMasters = (from roleLstData in roleMasters
                                   where roleLstData.RoleID == RoleId
                                   select roleLstData).ToList();
                }
                return roleMasters;
            }
        }

        public static RoleMaster ValidateRoleName(RoleMaster roleMaster)
        {
            using (KPIEntities db = new KPIEntities())
            {
                RoleMaster roleMasterObj = (from roleMstr in db.RoleMasters
                                            where roleMstr.RoleID != roleMaster.RoleID
                                            && roleMstr.RoleName.Trim().ToLower() == roleMaster.RoleName.Trim().ToLower()
                                            && roleMstr.IsDeleted == false
                                            select roleMstr).FirstOrDefault();

                return roleMasterObj;
            }
        }

        public static List<RoleRight> GetAssignedRoleRightsToMenu(int RoleID)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<RoleRight> roleRightsObj = (from rlRights in db.RoleRights
                                                 where rlRights.RoleID == RoleID
                                                 && rlRights.IsActive == true
                                                 select rlRights).ToList();

                return roleRightsObj;
            }
        }

        public static List<MenuMaster> GetParentMenuMasterData()
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MenuMaster> menuMasterObj = (from menuMstr in db.MenuMasters
                                                  where menuMstr.MenuParentID == null
                                                  && menuMstr.IsActive == true
                                                  select menuMstr).ToList();
                return menuMasterObj;
            }
        }

        public static List<MenuMaster> GetChildMenuMasterData(int menuParentId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MenuMaster> menuMasterObj = (from menuMstr in db.MenuMasters
                                                  where menuMstr.MenuParentID != null
                                                  && menuMstr.MenuParentID == menuParentId
                                                  && menuMstr.IsActive == true
                                                  select menuMstr).ToList();
                return menuMasterObj;
            }
        }

        public static int GetParentMenuIdFromMenuId(int MenuId)
        {
            using (KPIEntities db = new KPIEntities())
            {
                int? ParentMenuId = (from menuMstr in db.MenuMasters
                                     where menuMstr.MenuID == MenuId
                                     && menuMstr.MenuParentID != null
                                     select menuMstr.MenuParentID).FirstOrDefault();

                ParentMenuId = ParentMenuId == null ? 0 : ParentMenuId;

                return (int)ParentMenuId;
            }
        }

        public static List<MouldTypeMaster> GetMouldTypeMasterListData(int MouldTypeId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MouldTypeMaster> mouldTypeMasters = (from mouldTypeMstr in db.MouldTypeMasters
                                                          where mouldTypeMstr.IsDiscontinued == false
                                                          //orderby (mouldTypeMstr.LastModifiedOn != null ? mouldTypeMstr.LastModifiedOn : mouldTypeMstr.AddedOn)
                                                          //descending
                                                          orderby mouldTypeMstr.MouldType ascending
                                                          select mouldTypeMstr).ToList();
                if (MouldTypeId != 0)
                {
                    mouldTypeMasters = (from roleLstData in mouldTypeMasters
                                        where roleLstData.MouldTypeID == MouldTypeId
                                        select roleLstData).ToList();
                }
                return mouldTypeMasters;
            }
        }

        public static string getLocationNameFromId(int LocationId = 0)
        {
            string locationName = "";
            if (LocationId > 0)
            {
                using (KPIEntities db = new KPIEntities())
                {
                    locationName = (from LM in db.LocationMasters
                                    where LM.LocationId == LocationId
                                    select LM.LocationName).FirstOrDefault();
                }
            }
            return locationName;
        }

        public static VendorMaster GetVendorDetailsFromId(int VendorId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                VendorMaster vendorMaster = (from vm in db.VendorMasters
                                             where vm.VendorId == VendorId
                                             select vm).FirstOrDefault();
                return vendorMaster;
            }
        }

        public static LocationMaster GetLocationMaster(int LocationId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                LocationMaster locationMaster = (from lm in db.LocationMasters
                                                 where lm.LocationId == LocationId
                                                 select lm).FirstOrDefault();
                return locationMaster;
            }
        }

    }
}
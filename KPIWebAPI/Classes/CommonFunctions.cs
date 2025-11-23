using KPILib;
using KPILib.Models;
using Serilog;
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

        public static bool IsSalesRateAccess(int RoleId)
        {
            bool IsSalesRateAccessResult = false;

            using (KPIEntities db = new KPIEntities())
            {
                LookUpMaster lookUpMasterObj = (from LM in db.LookUpMasters
                                                where LM.LookUpType == ApplicationConstants.SMRateAccessLookUp
                                                && LM.IsActive
                                                select LM).FirstOrDefault();

                if (lookUpMasterObj != null && !string.IsNullOrEmpty(lookUpMasterObj.LookUpValue))
                {
                    List<int> accessRoleIds = lookUpMasterObj.LookUpValue.Split(',').Select(int.Parse).ToList();
                    if (accessRoleIds.Contains(RoleId))
                    {
                        IsSalesRateAccessResult = true;
                    }
                }
            }

            return IsSalesRateAccessResult;
        }

        public static List<LookUpMaster> GetLookUpMasterDataFromLookupType(string LookUpTypeName)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<LookUpMaster> lookUpMasterObj = (from LM in db.LookUpMasters
                                                      where LM.LookUpType == LookUpTypeName
                                                      && LM.IsActive
                                                      select LM).ToList();
                return lookUpMasterObj;
            }
        }
        public static LookUpMaster GetLookUpMasterDataFromLookupCode(string LookupCode)
        {
            using (KPIEntities db = new KPIEntities())
            {
                LookUpMaster lookUpMasterObj = (from LM in db.LookUpMasters
                                                where LM.LookUpValue == LookupCode
                                                && LM.IsActive
                                                select LM).FirstOrDefault();
                return lookUpMasterObj;
            }
        }
        public static List<int> GetRMIdsFromSalesId(int SalesId)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<int> RMIds = (from SRM in db.SalesRMMappings
                                   where SRM.SalesId == SalesId
                                   && SRM.RMId != null
                                   && SRM.IsActive
                                   select (int)SRM.RMId).ToList();
                return RMIds;
            }
        }

        public static List<RackMaster> GetRackMasterData()
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<RackMaster> rackMasters = (from RM in db.RackMasters
                                                where !RM.IsDiscontinued
                                                select RM).ToList();
                return rackMasters;
            }
        }
        public static MachineMaster GetMachineMasterData(int MachineId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                MachineMaster rackMasterData = (from MS in db.MachineMasters
                                                where MS.MachineID == MachineId
                                                && !MS.IsDiscontinued
                                                select MS).FirstOrDefault();
                return rackMasterData;
            }
        }
        public static List<MachineHistory> GetMachineHistoriesData(int MachineId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MachineHistory> machineHistories = (from MH in db.MachineHistories
                                                         where MH.MachineID == MachineId
                                                         select MH).ToList();
                return machineHistories;
            }
        }
        public static MachineTypeMaster GetMachineTypeMasterData(int MachineTypeId)
        {
            using (KPIEntities db = new KPIEntities())
            {
                MachineTypeMaster machineTypeMasterObj = (from MTM in db.MachineTypeMasters
                                                          where MTM.MachineTypeID == MachineTypeId
                                                          && !MTM.IsDiscontinued
                                                          select MTM).FirstOrDefault();
                return machineTypeMasterObj;
            }
        }
        public static List<MachineTypeMaster> GetMachineTypeMasterList()
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MachineTypeMaster> machineTypeMaster = (from MTM in db.MachineTypeMasters
                                                             where !MTM.IsDiscontinued
                                                             select MTM).ToList();
                return machineTypeMaster;
            }
        }
        public static MachineHistory GetMachineStatusData(int MachineId)
        {
            using (KPIEntities db = new KPIEntities())
            {
                MachineHistory machineHistoryMasterObj = (from MSM in db.MachineHistories
                                                          where MSM.MachineID == MachineId
                                                          && MSM.IsDeleted == false
                                                          select MSM).FirstOrDefault();
                return machineHistoryMasterObj;
            }
        }
        public static List<MachineStatusMaster> GetMachineStatusMasterList()
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MachineStatusMaster> machineStatusMaster = (from MSM in db.MachineStatusMasters
                                                                 select MSM).ToList();
                return machineStatusMaster;
            }
        }
        public static List<TagColourMaster> GetTagColourMasterList()
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<TagColourMaster> tagColorMaster = (from TCM in db.TagColourMasters
                                                        where !TCM.IsDiscontinued
                                                        select TCM).ToList();
                return tagColorMaster;
            }
        }

        public static List<SalesStatusMaster> GetSalesStatusMasterList()
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<SalesStatusMaster> salesStatusMaster = (from SSM in db.SalesStatusMasters
                                                             select SSM).ToList();
                return salesStatusMaster;
            }
        }
        public static List<CompanyMaster> GetCompanyMasterList()
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<CompanyMaster> companyMaster = (from CM in db.CompanyMasters
                                                     where CM.CompanyTypeID == 103 //Harcoded value for CompanyTypeId
                                                     && CM.IsDiscontinued == false
                                                     select CM).ToList();
                return companyMaster;
            }
        }
        public static CompanyMaster GetCompanyMasterById(int CompanyId)
        {
            using (KPIEntities db = new KPIEntities())
            {
                CompanyMaster companyMasterObj = (from CM in db.CompanyMasters
                                                  where CM.CompanyID == CompanyId
                                                  && CM.IsDiscontinued == false
                                                  select CM).FirstOrDefault();
                return companyMasterObj;
            }
        }
        public static Company GetCompanyLocationById(int CompanyId)
        {
            using (KPIEntities db = new KPIEntities())
            {
                Company companyLcsnMasterObj =
                    (from CM in db.CompanyMasters
                     join CLM in db.CompanyLocationMasters
                         on CM.CompanyID equals CLM.CompanyID into CLMGroup
                     from CLM in CLMGroup.DefaultIfEmpty()
                     where CM.CompanyID == CompanyId
                           && CM.IsDiscontinued == false
                     select new Company
                     {
                         CompanyID = CM.CompanyID,
                         CompanyName = CM.CompanyName,
                         LocationName = CLM != null ? CLM.LocationName : null,
                         Notes = CM.Notes,
                         AddedOn = CM.AddedOn,
                         LastModifiedOn = CM.LastModifiedOn
                     })
                    .FirstOrDefault();
                return companyLcsnMasterObj;
            }
        }

        public static List<usp_GetMachineMouldMapData_Result> GetMachineMouldMappingsData(string MapType = "Machine")
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<usp_GetMachineMouldMapData_Result> machineMouldMappings = db.usp_GetMachineMouldMapData(MapType).ToList();
                return machineMouldMappings;
            }
        }
        public static List<MachineMouldMapping> GetMachineMouldMappedData(int MachineId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MachineMouldMapping> machineMouldMappings = (from MM in db.MachineMouldMappings
                                                                  where MM.MachineID == MachineId
                                                                  && !MM.IsDiscontinued
                                                                  select MM).ToList();
                return machineMouldMappings;
            }
        }

        public static MachineMouldMapping GetMachineMouldMappingData(int MachineMouldMappingId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                MachineMouldMapping machineMouldMapping = (from MM in db.MachineMouldMappings
                                                           where MM.MachineMouldMappingID == MachineMouldMappingId
                                                           && !MM.IsDiscontinued
                                                           select MM).FirstOrDefault();
                return machineMouldMapping;
            }
        }

        public static List<MachineMaster> GetMachineMasters(int MachineId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MachineMaster> machineMasters = (from MM in db.MachineMasters
                                                      where !MM.IsDiscontinued
                                                      select MM).ToList();
                if (MachineId != 0)
                {
                    machineMasters = (from MM in machineMasters
                                      where MM.MachineID == MachineId
                                      select MM).ToList();
                }
                return machineMasters;
            }
        }
        public static List<MouldMaster> GetMouldMasters(int MouldId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MouldMaster> machineMasters = (from MM in db.MouldMasters
                                                    where !MM.IsDiscontinued
                                                    select MM).ToList();
                if (MouldId != 0)
                {
                    machineMasters = (from MM in machineMasters
                                      where MM.MouldID == MouldId
                                      select MM).ToList();
                }
                return machineMasters;
            }
        }

        public static List<MachineMouldMapping> GetMouldMachineMappedData(int MouldId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                List<MachineMouldMapping> machineMouldMappings = (from MM in db.MachineMouldMappings
                                                                  where MM.MouldID == MouldId
                                                                  && !MM.IsDiscontinued
                                                                  select MM).ToList();
                return machineMouldMappings;
            }
        }

        public static PalletMaster GetPalletMaster(int palletID = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                return (from PM in db.PalletMasters
                        where !PM.IsDiscontinued && (palletID == 0 || PM.PalletID == palletID)
                        select PM).FirstOrDefault();
            }
        }

        public static RMInventoryRejectionMaterial GetRejectionMaterialModel(int RejectionMaterialId = 0)
        {
            using (KPIEntities db = new KPIEntities())
            {
                return (from RM in db.RMInventoryRejectionMaterials
                        where RM.RejectionMaterialId == RejectionMaterialId
                        select RM).FirstOrDefault();
            }
        }

    }
}
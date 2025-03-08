using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using KPILib.Models;
using KPIWebAPI.Classes;
using KPIWebAPI.AuthFilters;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Users")]
    public class UserMasterAPIController : CCSPLBaseAPIController
    {
        #region User Master region CRUD

        static object myLock = new object();

        [HttpGet]
        public IHttpActionResult GetUsers(int UserId = 0)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();
            try
            {
                lock (myLock)
                {
                    List<UserMaster> userMasters = CommonFunctions.GetUserMasterListData(UserId);

                    List<KPILib.Models.UserMaster> usersData = mapper.Map<List<UserMaster>, List<KPILib.Models.UserMaster>>(userMasters);

                    userMasterResponse.data = usersData;
                    userMasterResponse.Response.IsSuccessful();
                }
            }
            catch (Exception ex)
            {
                userMasterResponse.Response.ResponseCode = 500;
                userMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(userMasterResponse);
        }

        [HttpGet]
        public IHttpActionResult GetUserMasterData(int UserId = 0)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();
            try
            {
                lock (myLock)
                {
                    UserMaster userMasterNewObj = CommonFunctions.GetUserMasterData(UserId);

                    KPILib.Models.UserMaster userMstrObj = mapper.Map<UserMaster, KPILib.Models.UserMaster>(userMasterNewObj);

                    userMasterResponse.userMasterObj = userMstrObj;
                    userMasterResponse.Response.IsSuccessful();
                }
            }
            catch (Exception ex)
            {
                userMasterResponse.Response.ResponseCode = 500;
                userMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(userMasterResponse);
        }

        [HttpGet]
        public IHttpActionResult ValidateUsername(string Username, int UserId = 0)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();
            try
            {
                lock (myLock)
                {
                    int CountDuplicateUsername = CommonFunctions.ValidateUsername(Username, UserId);

                    if (CountDuplicateUsername > 0)
                    {
                        userMasterResponse.Response.ResponseCode = 406;
                        userMasterResponse.Response.ResponseMsg = "Username already in use !!!";
                    }
                    else
                    {
                        userMasterResponse.Response.IsSuccessful();
                    }
                }
            }
            catch (Exception ex)
            {
                userMasterResponse.Response.ResponseCode = 500;
                userMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(userMasterResponse);
        }

        [HttpPost]
        public IHttpActionResult AddUser(UserMaster userMaster, int SessionUserID)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (userMaster != null)
                        {
                            if (userMaster.UserID == 0)
                            {
                                userMaster.AddedOn = DateTime.Now;
                                userMaster.AddedBy = SessionUserID;
                                userMaster.Email = userMaster.Email == null ? "" : userMaster.Email;
                                db.UserMasters.Add(userMaster);
                            }
                            else
                            {
                                UserMaster userMasterNewObj = CommonFunctions.GetUserMasterData(userMaster.UserID);

                                userMasterNewObj.Username = !string.IsNullOrEmpty(userMaster.Username) ? userMaster.Username : userMasterNewObj.Username;
                                userMasterNewObj.Password = !string.IsNullOrEmpty(userMaster.Password) ? userMaster.Password : userMasterNewObj.Password;
                                userMasterNewObj.RoleID = userMaster.RoleID;
                                userMasterNewObj.Email = userMaster.Email == null ? "" : userMaster.Email;
                                userMasterNewObj.Mobile = userMaster.Mobile;
                                userMasterNewObj.ModifiedBy = SessionUserID;
                                userMasterNewObj.LastModifiedOn = DateTime.Now;
                                db.Entry(userMasterNewObj).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                            transaction.Commit();
                        }
                        userMasterResponse.Response.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        userMasterResponse.Response.ResponseCode = 500;
                        userMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                        CommonLogger.Error(ex, ex.Message);
                    }
                }
            }
            return Json(userMasterResponse);
        }

        [HttpPost]
        public IHttpActionResult DeleteUser(int UserId, int SessionUserId)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        UserMaster userMaster = (from usrMstr in db.UserMasters
                                                 where usrMstr.UserID == UserId
                                                 && usrMstr.IsDiscontinued == false
                                                 select usrMstr).FirstOrDefault();
                        if (userMaster != null)
                        {
                            userMaster.IsDiscontinued = true;
                            userMaster.LastModifiedOn = DateTime.UtcNow;
                            userMaster.ModifiedBy = SessionUserId;
                            db.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            transaction.Commit();
                        }
                        userMasterResponse.Response.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        userMasterResponse.Response.ResponseCode = 500;
                        userMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                        CommonLogger.Error(ex, ex.Message);
                    }
                }
            }
            return Json(userMasterResponse);
        }

        [HttpPost]
        public IHttpActionResult ForgotPassword(UserMaster userMaster)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        UserMaster userMasterOldObj = (from usrMstr in db.UserMasters
                                                       where usrMstr.UserID == userMaster.UserID
                                                       && usrMstr.IsDiscontinued == false
                                                       select usrMstr).FirstOrDefault();
                        if (userMasterOldObj != null)
                        {
                            userMasterOldObj.Password = userMaster.Password;
                            userMasterOldObj.LastModifiedOn = DateTime.UtcNow;
                            userMasterOldObj.UserID = userMaster.UserID;
                            db.Entry(userMasterOldObj).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            transaction.Commit();
                        }
                        userMasterResponse.Response.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        userMasterResponse.Response.ResponseCode = 500;
                        userMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                        CommonLogger.Error(ex, ex.Message);
                    }
                }
            }
            return Json(userMasterResponse);
        }

        #endregion

        #region Role Master CRUD

        [HttpGet]
        public IHttpActionResult GetRoles(int RoleId = 0)
        {
            RoleMasterResponse roleMasterResponse = new RoleMasterResponse();

            try
            {
                List<RoleMaster> roleMasterData = CommonFunctions.GetRoleMasterListData(RoleId);

                List<KPILib.Models.RoleMaster> roleData = mapper.Map<List<RoleMaster>, List<KPILib.Models.RoleMaster>>(roleMasterData);

                roleMasterResponse.data = roleData;
                roleMasterResponse.Response.IsSuccessful();

                if (RoleId != 0 && roleData != null && roleData.Count() > 0)
                {
                    roleMasterResponse.roleMasterObj.RoleID = roleData[0].RoleID;
                    roleMasterResponse.roleMasterObj.RoleName = roleData[0].RoleName;
                }

            }
            catch (Exception ex)
            {
                roleMasterResponse.Response.ResponseCode = 500;
                roleMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(roleMasterResponse);
        }

        [HttpPost]
        public IHttpActionResult AddRoles(RoleMaster roleMaster)
        {
            RoleMasterResponse roleMasterResponse = new RoleMasterResponse();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (roleMaster != null)
                        {
                            RoleMaster role = CommonFunctions.ValidateRoleName(roleMaster);

                            if (role != null)
                            {
                                roleMasterResponse.Response.ResponseCode = 400;
                                roleMasterResponse.Response.ResponseMsg = "Rolename is already present !!!";
                                return Json(roleMasterResponse);
                            }

                            if (roleMaster.RoleID == 0)
                            {
                                roleMaster.IsDeleted = false;
                                roleMaster.AddedOn = DateTime.UtcNow;
                                db.RoleMasters.Add(roleMaster);
                            }
                            else
                            {
                                RoleMaster roleMasterNewObj = CommonFunctions.GetRoleMasterData(roleMaster.RoleID);

                                roleMasterNewObj.RoleName = roleMaster.RoleName;
                                roleMasterNewObj.ModifiedBy = roleMaster.ModifiedBy;
                                roleMasterNewObj.ModifiedOn = DateTime.UtcNow;
                                db.Entry(roleMasterNewObj).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                            transaction.Commit();
                        }
                        roleMasterResponse.Response.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        roleMasterResponse.Response.ResponseCode = 500;
                        roleMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                        CommonLogger.Error(ex, ex.Message);
                    }
                }
            }
            return Json(roleMasterResponse);
        }

        [HttpPost]
        public IHttpActionResult DeleteRole([FromUri] int RoleId = 0, int UserId = 0)
        {
            RoleMasterResponse roleMasterResponse = new RoleMasterResponse();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        RoleMaster roleMaster = CommonFunctions.GetRoleMasterData(RoleId);
                        if (roleMaster != null)
                        {
                            roleMaster.IsDeleted = true;
                            roleMaster.ModifiedBy = UserId;
                            roleMaster.ModifiedOn = DateTime.UtcNow;
                            db.Entry(roleMaster).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            transaction.Commit();
                        }
                        roleMasterResponse.Response.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        roleMasterResponse.Response.ResponseCode = 500;
                        roleMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                        CommonLogger.Error(ex, ex.Message);
                    }
                }
            }
            return Json(roleMasterResponse);
        }

        [HttpPost]
        public IHttpActionResult ValidateRoleName(RoleMaster roleMaster)
        {
            RoleMasterResponse roleMasterResponse = new RoleMasterResponse();

            RoleMaster role = CommonFunctions.ValidateRoleName(roleMaster);

            if (role != null)
            {
                roleMasterResponse.Response.ResponseCode = 400;
                roleMasterResponse.Response.ResponseMsg = "Rolename is already present !!!";
            }
            else
            {
                roleMasterResponse.Response.IsSuccessful();
            }

            return Json(roleMasterResponse);
        }

        [HttpGet]
        public IHttpActionResult GetAssignedRoleRightsToMenu(int RoleId)
        {
            RoleRightsResponse roleRightsResponse = new RoleRightsResponse();
            try
            {
                List<RoleRight> listOfAssignedRoleRight = CommonFunctions.GetAssignedRoleRightsToMenu(RoleId);

                if (listOfAssignedRoleRight != null && listOfAssignedRoleRight.Count() > 0)
                {
                    roleRightsResponse.RoleRightsListObj = mapper.Map<List<RoleRight>, List<RoleRights>>(listOfAssignedRoleRight);

                    foreach (var item in roleRightsResponse.RoleRightsListObj)
                    {
                        item.ParentMenuID = CommonFunctions.GetParentMenuIdFromMenuId(item.MenuID);
                    }
                }
                roleRightsResponse.ResponseObj.IsSuccessful();
            }
            catch (Exception ex)
            {
                roleRightsResponse.ResponseObj.ResponseCode = 500;
                roleRightsResponse.ResponseObj.ResponseMsg = $"Internal Server Error {ex.Message}";
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(roleRightsResponse);
        }

        [HttpGet]
        public IHttpActionResult GetParentMenuMasterData()
        {
            MenuMasterResponse roleRightsResponse = new MenuMasterResponse();
            try
            {
                List<MenuMaster> listOfMenuMaster = CommonFunctions.GetParentMenuMasterData();

                if (listOfMenuMaster != null && listOfMenuMaster.Count() > 0)
                {
                    roleRightsResponse.menuMasterListObj = mapper.Map<List<MenuMaster>, List<KPILib.Models.MenuMaster>>(listOfMenuMaster);
                }
                roleRightsResponse.ResponseObj.IsSuccessful();
            }
            catch (Exception ex)
            {
                roleRightsResponse.ResponseObj.ResponseCode = 500;
                roleRightsResponse.ResponseObj.ResponseMsg = $"Internal Server Error {ex.Message}";
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(roleRightsResponse);
        }

        [HttpGet]
        public IHttpActionResult GetChildMenuMasterData(int ParentMenuID)
        {
            MenuMasterResponse menuMasterResponse = new MenuMasterResponse();
            try
            {
                List<MenuMaster> listOfMenuMaster = CommonFunctions.GetChildMenuMasterData(ParentMenuID);

                if (listOfMenuMaster != null && listOfMenuMaster.Count() > 0)
                {
                    menuMasterResponse.menuMasterListObj = mapper.Map<List<MenuMaster>, List<KPILib.Models.MenuMaster>>(listOfMenuMaster);
                }
                menuMasterResponse.ResponseObj.IsSuccessful();
            }
            catch (Exception ex)
            {
                menuMasterResponse.ResponseObj.ResponseCode = 500;
                menuMasterResponse.ResponseObj.ResponseMsg = $"Internal Server Error {ex.Message}";
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(menuMasterResponse);
        }

        [HttpPost]
        public IHttpActionResult SaveRoleRightsForMenu(List<RoleRight> roleRights, int MenuParentID, int RoleID, int UserId)
        {
            RoleRightsResponse roleRightsResponse = new RoleRightsResponse();

            lock (myLock)
            {
                if (roleRights != null && roleRights.Count() > 0)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (roleRights != null && roleRights.Count() > 0)
                            {
                                List<RoleRight> oldRolesData = (from roleRghts in db.RoleRights
                                                                join mnuMstr in db.MenuMasters
                                                                on roleRghts.MenuID equals mnuMstr.MenuID
                                                                where mnuMstr.MenuParentID == MenuParentID
                                                                && roleRghts.RoleID == RoleID
                                                                && roleRghts.IsActive == true
                                                                && mnuMstr.IsActive == true
                                                                select roleRghts).ToList();

                                if (oldRolesData != null && oldRolesData.Count() > 0)
                                {
                                    oldRolesData.ForEach(z =>
                                    {
                                        z.IsActive = false;
                                        z.ModifiedOn = DateTime.Now;
                                        z.ModifiedBy = UserId;

                                        db.Entry(z).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    });
                                }

                                roleRights = roleRights.GroupBy(x => x.MenuID)
                                             //.Where(x => x.Count() == 1)
                                             .Select(x => x.First()).ToList();

                                roleRights.ForEach(z =>
                                {
                                    z.IsActive = true;
                                    z.AddedBy = UserId;
                                    z.AddedOn = DateTime.Now;
                                    z.RoleID = z.RoleID == 0 ? RoleID : z.RoleID;
                                });

                                db.RoleRights.AddRange(roleRights);
                                db.SaveChanges();
                                transaction.Commit();
                            }
                            roleRightsResponse.ResponseObj.IsSuccessful();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            roleRightsResponse.ResponseObj.ResponseCode = 500;
                            roleRightsResponse.ResponseObj.ResponseMsg = $"Internal server Error :- {ex.Message}";
                            CommonLogger.Error(ex, ex.Message);
                        }
                    }
                }
                else
                {
                    roleRightsResponse.ResponseObj.ResponseCode = 400;
                    roleRightsResponse.ResponseObj.ResponseMsg = "Bad Request !!!";
                }
            }
            return Json(roleRightsResponse);
        }

        #endregion

    }
}
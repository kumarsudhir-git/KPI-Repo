using KPILib;
using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Linq;
using System.Web.Caching;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    public class LoginAPIController : ApiController
    {
        // GET: Login
        public IHttpActionResult Index()
        {
            return Json("API Started");
        }

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult UserLogin(UserMaster userMaster)
        {

            KPIWebAPI.Classes.CommonFunctions.WriteToLog("success!");

            UserMasterResponse responseObj = new UserMasterResponse();
            try
            {
                using (KPIEntities db = new KPIEntities())
                {
                    UserMaster userMasterNewObj = (from usrMstr in db.UserMasters
                                                   where (usrMstr.Username == userMaster.Username
                                                   || usrMstr.Email == userMaster.Username)
                                                   && usrMstr.Password == userMaster.Password
                                                   && usrMstr.IsDiscontinued == false
                                                   select usrMstr).FirstOrDefault();

                    if (userMasterNewObj != null)
                    {
                        responseObj.Response.ResponseCode = 200;

                        responseObj.userMasterObj.UserID = userMasterNewObj.UserID;
                        responseObj.userMasterObj.RoleID = userMasterNewObj.RoleID;
                        responseObj.userMasterObj.Username = userMasterNewObj.Username;
                        responseObj.userMasterObj.Email = userMasterNewObj.Email;
                        responseObj.userMasterObj.Mobile = userMasterNewObj.Mobile;
                        var token = JwtManager.GenerateToken(userMasterNewObj);
                        responseObj.Response.ResponseMsg = token;

                        var userRights = (from rights in db.RoleRights
                                          join menu in db.MenuMasters
                                          on rights.MenuID equals menu.MenuID
                                          where rights.RoleID == userMasterNewObj.RoleID
                                          && rights.IsActive == true
                                          && menu.IsActive == true
                                          select new UserRoleRights
                                          {
                                              RoleID = userMasterNewObj.RoleID,
                                              MenuID = menu.MenuID,
                                              Add = rights.Add,
                                              Update = rights.Update,
                                              Delete = rights.Delete,
                                              View = rights.View,
                                              IsActive = rights.IsActive,
                                              Link = menu.Link,
                                              MenuCode = menu.MenuCode,
                                              MenuIcon = menu.MenuIcon,
                                              MenuName = menu.MenuName,
                                              MenuParentID = menu.MenuParentID,
                                              RoleRightsID = rights.RoleRightsID
                                          }).ToList();

                        responseObj.Response.ResponseData = userRights;
                        CacheManager.AddCache(token, userRights, 20);
                    }
                    else
                    {
                        responseObj.Response.ResponseCode = 401;
                        responseObj.Response.ResponseMsg = "Username or Password is wrong";
                    }
                }
            }
            catch (Exception ex)
            {
                responseObj.Response.ResponseCode = 999;
                responseObj.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(responseObj);
        }

        [HttpPost]
        public IHttpActionResult ForgotPassword(UserMaster userMaster)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();
            using (KPIEntities db = new KPIEntities())
            {
                try
                {
                    UserMaster userMasterObj = CommonFunctions.GetUserMasterData(userMaster.UserID);

                    if (userMasterObj != null)
                    {
                        userMasterObj.Password = userMaster.Password;
                        userMasterObj.LastModifiedOn = DateTime.UtcNow;
                        userMasterObj.ModifiedBy = userMaster.ModifiedBy;
                        db.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    userMasterResponse.Response.IsSuccessful();
                }
                catch (Exception ex)
                {
                    userMasterResponse.Response.ResponseCode = 500;
                    userMasterResponse.Response.ResponseMsg = $"Internal server Error :- {ex.Message}";
                    CommonLogger.Error(ex, ex.Message);
                }
            }
            return Json(userMasterResponse);
        }

        [HttpPost]
        public IHttpActionResult LogOutUser(string JWTTokenKey)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();
            CacheManager.Remove(JWTTokenKey);
            userMasterResponse.Response.IsSuccessful();
            return Json(userMasterResponse);
        }

    }
}
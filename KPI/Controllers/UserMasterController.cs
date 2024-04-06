using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Users")]
    public class UserMasterController : CCSPLBaseController
    {
        // GET: UserMaster
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotAllowed()
        {
            return View();
        }

        #region User Master region CRUD

        [HttpGet]
        public ActionResult GetUsers(int UserId = 0)
        {
            List<UserMaster> userMasters = new List<UserMaster>();
            UserMasterResponse roleMasterResponse = KPIAPIManager.GetUserMasterListResponse(UserId);
            if (roleMasterResponse != null && roleMasterResponse.Response != null)
            {
                if (roleMasterResponse.Response.ResponseCode == 200)
                {
                    userMasters = roleMasterResponse.data;
                }
            }
            return View(userMasters);
        }

        [HttpGet]
        public ActionResult AddUser(int UserId = 0)
        {
            ViewData["RoleID"] = new SelectList(new List<SelectListItem>(), "RoleID", "RoleName");

            UserMaster userMasters = new UserMaster();

            UserMasterResponse userMasterResponse = KPIAPIManager.GetUserMasterResponse(UserId);

            if (userMasterResponse != null && userMasterResponse.Response != null)
            {
                if (userMasterResponse.Response.ResponseCode == 200)
                {
                    if (userMasterResponse.userMasterObj != null)
                    {
                        userMasters = userMasterResponse.userMasterObj;
                    }
                }
            }

            RoleMasterResponse roleMasterResponse = KPIAPIManager.GetRoleResponse();

            if (roleMasterResponse != null && roleMasterResponse.Response != null)
            {
                if (roleMasterResponse.Response.ResponseCode == 200)
                {
                    if (roleMasterResponse.data != null && roleMasterResponse.data.Count() > 0)
                    {
                        ViewData["RoleID"] = new SelectList(roleMasterResponse.data, "RoleID", "RoleName");
                    }
                }
            }
            return View(userMasters);
        }

        [HttpPost]
        public ActionResult AddUser(UserMaster userMaster)
        {
            int SessionUserId = Convert.ToInt32(Session["UserID"]);
            UserMasterResponse userMasterResponse = KPIAPIManager.AddUpdateUserResponse(userMaster, SessionUserId);
            if (userMasterResponse != null && userMasterResponse.Response != null)
            {
                if (userMasterResponse.Response.ResponseCode == 200)
                {
                    return RedirectToAction("GetUsers");
                }
            }
            return View("Error");
        }

        [HttpGet]
        public ActionResult ValidateUserName(string Username, int UserId = 0)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();

            if (Request.IsAjaxRequest())
            {
                userMasterResponse.Response = CheckSessionForAjaxCall();

                if (userMasterResponse.Response.ResponseCode == 440)
                {
                    return Json(userMasterResponse);
                }
            }
            userMasterResponse = KPIAPIManager.ValidateUsername(Username, UserId);
            return Json(userMasterResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteUser(int UserId)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();

            if (Request.IsAjaxRequest())
            {
                ResponseObj responseObj = CheckSessionForAjaxCall();

                if (responseObj.ResponseCode == 440)
                {
                    userMasterResponse.Response = responseObj;

                    return Json(userMasterResponse);
                }
            }

            int SessionUserId = Convert.ToInt32(Session["UserID"]);
            userMasterResponse = new UserMasterResponse();
            if (SessionUserId == 0)
            {
                userMasterResponse.Response.ResponseCode = 401;
                userMasterResponse.Response.ResponseMsg = "Session Expired!!! Please relogin";
                return Json(userMasterResponse, JsonRequestBehavior.AllowGet);
            }

            userMasterResponse = KPIAPIManager.DeleteUserResponse(UserId, SessionUserId);
            return Json(userMasterResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ForgotPassword(int UserId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(UserMaster userMaster)
        {
            UserMasterResponse userMasterResponse = new UserMasterResponse();

            if (Request.IsAjaxRequest())
            {
                userMasterResponse.Response = CheckSessionForAjaxCall();

                if (userMasterResponse.Response.ResponseCode == 440)
                {
                    return Json(userMasterResponse);
                }
            }
            userMasterResponse = KPIAPIManager.ForgotPasswordResponse(userMaster);
            if (userMasterResponse != null && userMasterResponse.Response.ResponseCode == 200)
            {
                ViewBag.PasswordChangedMsg = "";
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
        #endregion

        #region Role Master CRUD

        [HttpGet]
        public ActionResult GetRoles(int RoleId = 0)
        {
            List<RoleMaster> roleMasters = new List<RoleMaster>();
            RoleMasterResponse roleMasterResponse = KPIAPIManager.GetRoleResponse(RoleId);
            if (roleMasterResponse != null && roleMasterResponse.Response != null)
            {
                if (roleMasterResponse.Response.ResponseCode == 200)
                {
                    roleMasters = roleMasterResponse.data;
                }
            }
            return View(roleMasters);
        }

        [HttpGet]
        public ActionResult AddRoles(int RoleId = 0)
        {
            RoleMaster roleMaster = new RoleMaster();

            RoleMasterResponse roleMasterResponse = KPIAPIManager.GetRoleResponse(RoleId);

            if (roleMasterResponse.Response.ResponseCode == 200)
            {
                roleMaster = roleMasterResponse.roleMasterObj;
            }

            return View(roleMaster);
        }

        [HttpPost]
        public ActionResult AddRoles(RoleMaster roleMaster)
        {
            RoleMasterResponse roleMasterResponse = new RoleMasterResponse();

            if (Request.IsAjaxRequest())
            {
                roleMasterResponse.Response = CheckSessionForAjaxCall();

                if (roleMasterResponse.Response.ResponseCode == 440)
                {
                    return Json(roleMasterResponse);
                }
            }

            if (roleMaster != null)
            {
                if (roleMaster.RoleID == 0)
                {
                    roleMaster.AddedBy = Convert.ToInt32(Session["UserID"]);
                }
                else
                {
                    roleMaster.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                }

                roleMasterResponse = KPIAPIManager.AddUpdateRoleResponse(roleMaster);

                if (roleMasterResponse.Response.ResponseCode == 200)
                {
                    return RedirectToAction("GetRoles");
                }
                else
                {
                    ViewBag.Error = roleMasterResponse.Response.ResponseMsg;
                    return RedirectToAction("GetRoles");
                }
            }
            return RedirectToAction("GetRoles");
        }

        //[HttpGet]
        //public ActionResult DeleteRole()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult DeleteRole(int RoleId = 0)
        {
            RoleMasterResponse roleMasterResponse = new RoleMasterResponse();

            if (Request.IsAjaxRequest())
            {
                roleMasterResponse.Response = CheckSessionForAjaxCall();

                if (roleMasterResponse.Response.ResponseCode == 440)
                {
                    return Json(roleMasterResponse);
                }
            }

            roleMasterResponse = new RoleMasterResponse();

            int SessionUserId = Convert.ToInt32(Session["UserID"]);

            if (SessionUserId == 0)
            {
                roleMasterResponse.Response.ResponseCode = 401;
                roleMasterResponse.Response.ResponseMsg = "Session Expired!!! Please relogin";
                return Json(roleMasterResponse, JsonRequestBehavior.AllowGet);
            }

            roleMasterResponse = KPIAPIManager.DeleteRoleResponse(RoleId, SessionUserId);
            return Json(roleMasterResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateRoleName(RoleMaster roleMaster)
        {
            RoleMasterResponse roleMasterResponse = new RoleMasterResponse();

            if (Request.IsAjaxRequest())
            {
                roleMasterResponse.Response = CheckSessionForAjaxCall();

                if (roleMasterResponse.Response.ResponseCode == 440)
                {
                    return Json(roleMasterResponse);
                }
            }
            roleMasterResponse = KPIAPIManager.ValidateRoleNameResponse(roleMaster);
            return Json(roleMasterResponse);
        }

        //TODO : Change the UI for assigning Role Rights

        [HttpGet]
        public ActionResult AssignRoleRightsToMenu(int RoleID)
        {
            MenuMasterResponse menuMasterResponse = KPIAPIManager.GetParentMenuMasterData();

            if (menuMasterResponse != null && menuMasterResponse.menuMasterObj != null)
            {
                menuMasterResponse.menuMasterObj.RoleID = RoleID;

                if (menuMasterResponse.menuMasterListObj != null && menuMasterResponse.menuMasterListObj.Count() > 0)
                {
                    menuMasterResponse.menuMasterObj.MenuParentID = menuMasterResponse.menuMasterListObj[0].MenuID;
                }
            }

            return View(menuMasterResponse);
        }

        [HttpGet]
        public ActionResult GetRoleRightsWithSubMenuData(RoleRights roleRights)
        {
            MenuMasterResponse childMenuMasterResponse = KPIAPIManager.GetChildMenuMasterData(roleRights.ParentMenuID);

            if (childMenuMasterResponse != null && childMenuMasterResponse.ResponseObj.ResponseCode == (int)HttpStatusCode.OK)
            {
                childMenuMasterResponse.menuMasterObj.RoleID = roleRights.RoleID;
                childMenuMasterResponse.menuMasterObj.MenuParentID = roleRights.ParentMenuID;

                RoleRightsResponse roleRightsResponseObj = KPIAPIManager.RoleRightsResponse(roleRights.RoleID);

                if (roleRightsResponseObj != null && roleRightsResponseObj.ResponseObj != null && roleRightsResponseObj.ResponseObj.ResponseCode == 200)
                {
                    if (roleRightsResponseObj.RoleRightsListObj.Count() > 0)
                    {
                        ViewBag.RoleRightsData = roleRightsResponseObj.RoleRightsListObj;
                    }
                }
            }
            return PartialView(childMenuMasterResponse);
        }

        [HttpGet]
        public ActionResult RoleRightsPartialView(RoleRights roleRights)
        {
            return PartialView("_RoleRightsPartialView", roleRights);
        }

        [HttpGet]
        public ActionResult GetChildMenuList(int ParentMenuId)
        {
            MenuMasterResponse childMenuMasterResponse = KPIAPIManager.GetChildMenuMasterData(ParentMenuId);
            return Json(childMenuMasterResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AssignRoleRightsToMenu(List<RoleRights> roleRights, int RoleID, int MenuParentID)
        {
            if (roleRights != null && roleRights.Count() > 0)
            {
                int UserID = Convert.ToInt32(Session["UserID"]);

                roleRights.ForEach(z =>
                {
                    //if (z.ParentMenuID != 0 && z.MenuID == 0)
                    //{
                    //    z.MenuID = z.ParentMenuID;
                    //}
                    if (!z.View && !z.Add && !z.Update && !z.Delete)
                    {
                        z.View = true;
                    }
                });

                roleRights = roleRights.GroupBy(x => x.MenuID)
                             //.Where(x => x.Count() == 1)
                             .Select(x => x.First()).ToList();

                RoleRightsResponse roleRightsResponse = KPIAPIManager.SaveRoleRightsForMenu(roleRights, MenuParentID, RoleID, UserID);

                if (roleRightsResponse != null && roleRightsResponse.ResponseObj != null && roleRightsResponse.ResponseObj.ResponseCode == (int)HttpStatusCode.OK)
                {
                    return RedirectToAction("GetRoles");
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("GetRoles");
            }
        }

        #endregion

    }
}
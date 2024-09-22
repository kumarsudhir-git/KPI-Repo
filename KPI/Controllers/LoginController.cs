using KPI.Classes;
using KPILib;
using KPILib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KPI.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["JWTTokenKey"] != null)
            {
                string JWTTokenKey = Session["JWTTokenKey"].ToString();
                CacheManager.Remove(JWTTokenKey);
                KPIAPIManager.LogOutUserResponse(JWTTokenKey);
            }
            Session.RemoveAll(); // removes all session value specific to this user
            Session.Clear();
            return View();
        }

        [HttpPost]
        public JsonResult UserLogin(UserMaster userMaster)
        {
            KPI.Classes.CommonFunctions.WriteToLog("success!");

            UserMasterResponse result = KPIAPIManager.LoginUserResponse(userMaster);
            if (result != null && result.Response.ResponseCode == (int)HttpStatusCode.OK)
            {
                KPI.Classes.CommonFunctions.WriteToLog("ok");

                Session["UserID"] = result.userMasterObj.UserID;
                Session["RoleID"] = result.userMasterObj.RoleID;
                Session["Username"] = result.userMasterObj.Username;
                Session["JWTToken"] = result.Response.ResponseMsg;

                if (result.Response.ResponseData != null)
                {
                    var userRights = JsonConvert.DeserializeObject<List<UserRoleRights>>(result.Response.ResponseData.ToString());
                    Session["UserRights"] = userRights;
                    Session["JWTTokenKey"] = result.Response.ResponseMsg;
                    CacheManager.AddCache(result.Response.ResponseMsg, userRights, 20);
                }
            }
            KPI.Classes.CommonFunctions.WriteToLog(":(");
            return Json(result);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(UserMaster userMaster)
        {
            return View();
        }
    }
}
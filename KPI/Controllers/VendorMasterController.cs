using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Vendors")]
    public class VendorMasterController : CCSPLBaseController
    {
        // GET: VendorMaster
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            VendorMasterModelResponse response = KPIAPIManager.GetAllVendorData();
            if (response.Response.ResponseCode == 200)
            {
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult AddNew(int id = 0)
        {
            VendorMasterModelResponse response = KPIAPIManager.GetVendorMasterData(id);
            if (response.Response.ResponseCode == 200)
            {
                return View(response.vendorMaster);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult AddNew(VendorMasterModel vendorMaster)
        {
            int SessionUserId = Convert.ToInt32(Session["UserID"]);
            if (vendorMaster.VendorId == 0)
            {
                vendorMaster.AddedBy = SessionUserId;
            }
            else
            {
                vendorMaster.LastModifiedBy = SessionUserId;
            }
            VendorMasterModelResponse vendorMasterModelObj = KPIAPIManager.AddUpdateVendorDataResponse(vendorMaster);
            if (vendorMasterModelObj != null && vendorMasterModelObj.Response != null)
            {
                if (vendorMasterModelObj.Response.ResponseCode == 200)
                {
                    return RedirectToAction("GetAll");
                }
            }
            return View("Error");
        }


        [HttpPost]
        public ActionResult DeleteVendor(int VendorId = 0)
        {
            VendorMasterModelResponse vendorMasterRspnsModelObj = new VendorMasterModelResponse();

            if (Request.IsAjaxRequest())
            {
                ResponseObj responseObj = CheckSessionForAjaxCall();

                if (responseObj.ResponseCode == 440)
                {
                    vendorMasterRspnsModelObj.Response = responseObj;

                    return Json(vendorMasterRspnsModelObj);
                }
            }

            int SessionUserId = Convert.ToInt32(Session["UserID"]);
            vendorMasterRspnsModelObj = new VendorMasterModelResponse();
            if (SessionUserId == 0)
            {
                vendorMasterRspnsModelObj.Response.ResponseCode = 401;
                vendorMasterRspnsModelObj.Response.ResponseMsg = "Session Expired!!! Please relogin";
                return Json(vendorMasterRspnsModelObj, JsonRequestBehavior.AllowGet);
            }
            VendorMasterModel vendorMaster = new VendorMasterModel()
            {
                VendorId = VendorId,
                LastModifiedBy = SessionUserId
            };
            vendorMasterRspnsModelObj = KPIAPIManager.DeleteVendorDataResponse(vendorMaster);
            return Json(vendorMasterRspnsModelObj, JsonRequestBehavior.AllowGet);
        }
    }
}
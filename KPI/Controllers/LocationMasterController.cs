using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Location")]
    public class LocationMasterController : CCSPLBaseController
    {
        #region Created new methods for LocationMaster CRUD

        // GET: Company
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllLocations();
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
        public ActionResult SaveLocation(int LocationId = 0)
        {
            LocationMasterResponse locationMasterResponse = new LocationMasterResponse();
            if (LocationId > 0)
            {
                locationMasterResponse.LocationMasterModel = KPIAPIManager.GetLocation(LocationId).LocationMasterModel;
            }
            else
            {
                locationMasterResponse.LocationMasterModel = new LocationMasterModel();
            }
            return View(locationMasterResponse.LocationMasterModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveLocation(KPILib.Models.LocationMasterModel locationMasterObj)
        {
            if (ModelState.IsValid)
            {
                if (locationMasterObj.LocationId == 0)
                {
                    locationMasterObj.AddedBy = GetUserSessionID();
                }
                else
                {
                    locationMasterObj.ModifiedBy = GetUserSessionID();
                }
                var response = KPIAPIManager.SaveLocation(locationMasterObj);
                if (response.Response.ResponseCode == 200)
                {
                    return RedirectToAction("GetAll");
                }
                else
                {
                    ViewBag.Error = response.Response.ResponseMsg;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Error = "Data provided is not valid";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult DeleteLocation(int LocationId)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.DeleteLocation(LocationId);
                if (response.Response.ResponseCode == 200)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("GetAll", "LocationMaster") });
                }
                else
                {
                    ViewBag.Error = response.Response.ResponseMsg;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Error = "Data provided is not valid";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ValidateLocationName(LocationMasterModel locationMasterObj)
        {
            var response = KPIAPIManager.ValidateLocationName(locationMasterObj);

            return Json(response);
        }

        #endregion
    }
}
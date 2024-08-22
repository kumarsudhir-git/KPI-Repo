using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_RMInventory")]
    public class RMInventoryController : Controller
    {
        // GET: RMInventory
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllRMInventory();
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

        public ActionResult GetAllByPallet(int id)
        {
            var response = KPIAPIManager.GetAllRMInventoryByRack(id);
            if (response.Response.ResponseCode == 200)
            {
                return View("GetAll", response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        public ActionResult GetAllByRM(int id)
        {
            var response = KPIAPIManager.GetAllRMInventoryByRM(id);
            if (response.Response.ResponseCode == 200)
            {
                return View("GetAll", response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        public ActionResult Get(int id)
        {
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");

            var response = KPIAPIManager.GetRMInventory(id);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.Pallets = new SelectList(response.data.Pallets, "PalletID", "PalletNo");
                ViewBag.RMs = new SelectList(response.data.RawMaterials, "RawMaterialID", "RawMaterialName");

                LocationMasterResponse locationMasterResponse = KPIAPIManager.GetListOfLocationMasterData();

                if (locationMasterResponse != null && locationMasterResponse.Response != null)
                {
                    if (locationMasterResponse.Response.ResponseCode == 200)
                    {
                        if (locationMasterResponse.data != null && locationMasterResponse.data.Count > 0)
                        {
                            ViewData["LocationId"] = new SelectList(locationMasterResponse.data, "LocationId", "LocationName");
                        }
                    }
                }
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }
        public ActionResult New()
        {
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");

            var response = KPIAPIManager.GetRMInventory(0);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.Pallets = new SelectList(response.data.Pallets, "PalletID", "PalletNo");
                ViewBag.RMs = new SelectList(response.data.RawMaterials, "RawMaterialID", "RawMaterialName");
                ViewBag.Tags = new SelectList(response.data.TagColours, "TagColourID", "TagColour");

                LocationMasterResponse locationMasterResponse = KPIAPIManager.GetListOfLocationMasterData();

                if (locationMasterResponse != null && locationMasterResponse.Response != null)
                {
                    if (locationMasterResponse.Response.ResponseCode == 200)
                    {
                        if (locationMasterResponse.data != null && locationMasterResponse.data.Count > 0)
                        {
                            ViewData["LocationId"] = new SelectList(locationMasterResponse.data, "LocationId", "LocationName");
                        }
                    }
                }
                //ViewBag.QtyKgs = 
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPILib.Models.RMInventory obj)
        {
            if (ModelState.IsValid)
            {
                obj.AddedBy = Convert.ToInt32(Session["UserID"]);
                var response = KPIAPIManager.AddRMInventory(obj);
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
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KPILib.Models.RMInventory obj)
        {
            if (ModelState.IsValid)
            {
                obj.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                var response = KPIAPIManager.EditRMInventory(obj);
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
    }
}
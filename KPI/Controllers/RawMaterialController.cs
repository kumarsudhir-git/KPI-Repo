using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_RM")]
    public class RawMaterialController : Controller
    {
        // GET: RawMaterials
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllRawMaterials();
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
        public ActionResult Get(int id)
        {
            ViewData["VendorId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");

            var response = KPIAPIManager.GetRawMaterial(id);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.UOMs = new SelectList(response.data.UOMs, "UOMID", "UnitsName");
                VendorMasterModelResponse vendorMasterModel = KPIAPIManager.GetAllVendorData();

                if (vendorMasterModel != null && vendorMasterModel.Response != null)
                {
                    if (vendorMasterModel.Response.ResponseCode == 200)
                    {
                        if (vendorMasterModel.data != null && vendorMasterModel.data.Count > 0)
                        {
                            ViewData["VendorId"] = new SelectList(vendorMasterModel.data, "VendorId", "VendorName");
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
            ViewData["VendorId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");
            var response = KPIAPIManager.GetRawMaterial(0);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.UOMs = new SelectList(response.data.UOMs, "UOMID", "UnitsName");
                VendorMasterModelResponse vendorMasterModel = KPIAPIManager.GetAllVendorData();

                if (vendorMasterModel != null && vendorMasterModel.Response != null)
                {
                    if (vendorMasterModel.Response.ResponseCode == 200)
                    {
                        if (vendorMasterModel.data != null && vendorMasterModel.data.Count > 0)
                        {
                            ViewData["VendorId"] = new SelectList(vendorMasterModel.data, "VendorId", "VendorName");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPILib.Models.RawMaterial obj)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.AddRawMaterial(obj);
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
        public ActionResult Edit(KPILib.Models.RawMaterial obj)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.EditRawMaterial(obj);
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
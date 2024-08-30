using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Moulds")]
    public class MouldController : CCSPLBaseController
    {
        // GET: Mould
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllMoulds();
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

        //[HttpGet]
        //public ActionResult Get(int id)
        //{
        //    var response = KPIAPIManager.GetMoulds(id);
        //    if (response.Response.ResponseCode == 200)
        //    {
        //        //ViewBag.UOMs = new SelectList(response.data.UOMs, "UOMID", "UnitsName");
        //        //ViewBag.ProductCategories = new SelectList(response.data.ProductCategories, "ProductCategoryID", "ProductCategoryName");
        //        //ViewBag.Moulds = new SelectList(response.data.Moulds, "MouldID", "MouldName");
        //        //ViewBag.RawMaterials = new SelectList(response.data.RawMaterials, "RawMaterialID", "RawMaterialName");
        //        return View(response.data);
        //    }
        //    else
        //    {
        //        ViewBag.Error = response.Response.ResponseMsg;
        //        return View("Error");
        //    }
        //}

        [HttpGet]
        public ActionResult New(int id = 0)
        {
            ViewData["MouldTypeID"] = new SelectList(new List<SelectListItem>(), "MouldTypeID", "RoleName");
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");
            MouldMasterResponse response = KPIAPIManager.GetMoulds(id);
            if (response.Response.ResponseCode == 200)
            {
                ViewData["MouldTypeID"] = new SelectList(response.MouldTypeMasters, "MouldTypeID", "MouldType");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPILib.Models.Mould obj)
        {
            if (ModelState.IsValid)
            {
                MouldMasterResponse response = KPIAPIManager.AddMoulds(obj);
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
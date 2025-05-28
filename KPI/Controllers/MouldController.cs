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

        [HttpGet]
        public ActionResult GetAllMouldMachineMappedData()
        {
            MachineMouldMappingResponse response = KPIAPIManager.GetAllMachineMouldMappedData("Mould");
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
        public ActionResult MapMouldMachine(int MouldId = 0)
        {
            ViewData["MachineID"] = new SelectList(new List<SelectListItem>(), "MachineID", "MachineName");
            ViewData["MouldID"] = new SelectList(new List<SelectListItem>(), "MouldID", "MouldName");
            MouldMastersResponse response = KPIAPIManager.GetMouldMachineMappedData(MouldId);
            if (response.Response.ResponseCode == 200)
            {
                MachineMastersResponse machineResponse = KPIAPIManager.GetMachineMasterData();
                if (machineResponse.Response.ResponseCode == 200)
                {
                    ViewData["MachineID"] = new SelectList(machineResponse.data, "MachineID", "MachineName");
                }
                MouldMastersResponse mouldResponse = KPIAPIManager.GetMouldMasterListData();
                if (mouldResponse.Response.ResponseCode == 200)
                {
                    ViewData["MouldID"] = new SelectList(mouldResponse.data, "MouldID", "MouldName");
                }
                return View(response.mouldMachineMapData);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult MapMouldMachine(List<MouldMachineMapping> mouldMachineMapping)
        {
            int UserID = GetUserSessionID();
            mouldMachineMapping.ForEach(z =>
            {
                z.UserID = UserID;
            });
            MouldMastersResponse response = KPIAPIManager.MapMouldMachine(mouldMachineMapping);
            if (response.Response.ResponseCode == 200)
            {
                return RedirectToAction("GetAllMouldMachineMappedData");
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

    }
}
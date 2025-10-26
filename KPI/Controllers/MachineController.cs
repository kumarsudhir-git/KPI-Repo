using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Machines")]
    public class MachineController : CCSPLBaseController
    {
        // GET: Machine
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllMachines();
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
        public ActionResult AddMachine(int MachineId = 0)
        {
            ViewData["MachineTypeID"] = new SelectList(new List<SelectListItem>(), "MachineTypeID", "MachineType");
            ViewData["MachineStatusID"] = new SelectList(new List<SelectListItem>(), "MachineStatusID", "MachineStatus");

            ViewData["VendorId"] = new SelectList(new List<SelectListItem>(), "VendorId", "VendorName");
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");

            MachineMasterResponse response = new MachineMasterResponse();

            response = KPIAPIManager.GetMachineData(MachineId);

            if (response.Response.ResponseCode == 200)
            {
                ViewData["MachineTypeID"] = new SelectList(response.machineTypeMasterData, "MachineTypeID", "MachineType");
                ViewData["MachineStatusID"] = new SelectList(response.machineStatusMasterData, "MachineStatusID", "MachineStatus");

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
        public ActionResult AddMachine(MachineMasterModel machine)
        {
            MachineMasterResponse response = KPIAPIManager.AddUpdateMachineData(machine);

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

        [HttpPost]
        public ActionResult DeleteMachine(int MachineId = 0)
        {
            MachineMasterResponse response = new MachineMasterResponse();

            //Implementation Pending

            if (response.Response.ResponseCode == 200)
            {
                return Json(response);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        //Need to create one method to validate Machine name

        //Machine - Mould Mapping

        [HttpGet]
        public ActionResult GetAllMachineMouldMappedData()
        {
            MachineMouldMappingResponse response = KPIAPIManager.GetAllMachineMouldMappedData();
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
        public ActionResult MapMachineMould(int MachineId = 0)
        {
            ViewData["MachineID"] = new SelectList(new List<SelectListItem>(), "MachineID", "MachineName");
            ViewData["MouldID"] = new SelectList(new List<SelectListItem>(), "MouldID", "MouldName");
            MachineMouldMappingResponse response = KPIAPIManager.GetMachineMouldMappedData(MachineId);
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
                return View(response.machineToMouldMap);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult MapMachineMould(List<MachineMouldMapping> machineMouldMapping)
        {
            int UserID = GetUserSessionID();
            machineMouldMapping.ForEach(z =>
            {
                z.UserID = UserID;
            });
            MachineMouldMappingResponse response = KPIAPIManager.MapMachineMould(machineMouldMapping);
            if (response.Response.ResponseCode == 200)
            {
                return RedirectToAction("GetAllMachineMouldMappedData");
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult DeleteMapMachineMould(int MachineMouldMappingId = 0)
        {
            MachineMouldMappingResponse mappingResponse = KPIAPIManager.DeleteMachineMouldMapping(MachineMouldMappingId);
            return Json(mappingResponse);
        }
    }
}
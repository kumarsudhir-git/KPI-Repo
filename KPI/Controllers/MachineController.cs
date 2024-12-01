using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
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

            MachineMasterResponse response = new MachineMasterResponse();

            response = KPIAPIManager.GetMachineData(MachineId);

            if (response.Response.ResponseCode == 200)
            {
                ViewData["MachineTypeID"] = new SelectList(response.machineTypeMasterData, "MachineTypeID", "MachineType");
                ViewData["MachineStatusID"] = new SelectList(response.machineStatusMasterData, "MachineStatusID", "MachineStatus");

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

    }
}
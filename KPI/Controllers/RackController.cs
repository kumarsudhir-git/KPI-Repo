using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace KPI.Controllers
{
    [CustomAuthFilter("M_Racks")]
    public class RackController : CCSPLBaseController
    {
        // GET: Racks
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllRacks();
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

        public ActionResult GetAllByProdID(int id)
        {
            var response = KPIAPIManager.GetAllRacks(id);
            if (response.Response.ResponseCode == 200)
            {
                if (response.data.Count > 0)
                {
                    ViewBag.ProdName = response.data.First().ProductName;
                    ViewBag.TotalQty = " (" + response.data.Sum(x => x.Qty) + ")";
                }
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

            var response = KPIAPIManager.GetRack(id);
            if (response.Response.ResponseCode == 200)
            {
                LocationMasterResponse locationMasterResponse = KPIAPIManager.GetListOfLocationMasterData();

                if (locationMasterResponse != null && locationMasterResponse.Response != null)
                {
                    if (locationMasterResponse.Response.ResponseCode == 200)
                    {
                        if (locationMasterResponse.data != null && locationMasterResponse.data.Count() > 0)
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

            LocationMasterResponse locationMasterResponse = KPIAPIManager.GetListOfLocationMasterData();

            if (locationMasterResponse != null && locationMasterResponse.Response != null)
            {
                if (locationMasterResponse.Response.ResponseCode == 200)
                {
                    if (locationMasterResponse.data != null && locationMasterResponse.data.Count() > 0)
                    {
                        ViewData["LocationId"] = new SelectList(locationMasterResponse.data, "LocationId", "LocationName");
                    }
                }
            }
            RackMaster rack = new RackMaster();
            return View(rack);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPILib.Models.RackMaster rack)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.AddRack(rack);
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
        public ActionResult Edit(KPILib.Models.RackMaster rack)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.EditRack(rack);
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
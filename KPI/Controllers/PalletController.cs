using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;


namespace KPI.Controllers
{
    [CustomAuthFilter("M_Pallets")]
    public class PalletController : CCSPLBaseController
    {
        // GET: Pallets
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllPallets();
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

        public ActionResult GetAllByRMID(int id)
        {
            var response = KPIAPIManager.GetAllPallets(id);
            if (response.Response.ResponseCode == 200)
            {
                if (response.data.Count > 0)
                {
                    ViewBag.RMName = response.data.First().RawMaterialName;
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
            var response = KPIAPIManager.GetPallet(id);
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

        public ActionResult New()
        {
            PalletMaster pallet = new PalletMaster();
            return View(pallet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPILib.Models.PalletMaster pallet)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.AddPallet(pallet);
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
        public ActionResult Edit(KPILib.Models.PalletMaster pallet)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.EditPallet(pallet);
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
using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_PR")]
    public class PurchaseRcvController : CCSPLBaseController
    {
        // GET: PurchaseRcv
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllPurchaseRcvMasts();
            if (response.Response.ResponseCode == 200)
            {
                return View(response.data.OrderByDescending(x => x.PurchaseRcvdID));
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        public ActionResult Rcv()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(FormCollection frm)
        {
            ViewData["CompanyLocationId"] = new SelectList(new List<SelectListItem>(), "CompanyLocationId", "CompanyLocationName");
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");

            string sPOID = frm["PONo"].ToString().Trim();
            var iPOID = Convert.ToInt32(sPOID);

            var response = KPIAPIManager.GetNewRcv(iPOID);
            if (response.Response.ResponseCode == 200)
            {
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
                //ViewBag.Locations = new SelectList(response.data.Locations, "Key", "Value");
                //ViewBag.Materials = new SelectList(response.data.Materials, "Key", "Value");

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
        //public ActionResult Add([Bind(Include = "PurchaseID,Notes,CompanyLocationId,LocationId,QCReceived,QCStatus,LineItems")] KPILib.Models.PurchaseRcvMast rcvd)
        public ActionResult Add(PurchaseRcvMast rcvd)
        {
            if (ModelState.IsValid)
            {
                rcvd.RcvdByUserID = GetUserSessionID();     //TODO: replace with UserID from session
                //purchase.LineItems.Add(new KPILib.Models.PurchaseDetails { RawMatarialID = 1002, Qty = 8 });
                //purchase.LineItems.Add(new KPILib.Models.PurchaseDetails { RawMatarialID = 1003, Qty = 2 });

                var response = KPIAPIManager.AddPurchaseRcv(rcvd);
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

        public ActionResult GetRMStackingPlan(int id)
        {
            var response = KPIAPIManager.GetRMStackingPlan(id);
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


    }
}
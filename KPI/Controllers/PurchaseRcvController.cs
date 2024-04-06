using KPI.Classes;
using KPI.Filters;
using System;
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
                return View(response.data.OrderByDescending(x=> x.PurchaseRcvdID));
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
            string sPOID = frm["PONo"].ToString().Trim();
            var iPOID = Convert.ToInt32(sPOID);

            var response = KPIAPIManager.GetNewRcv(iPOID);
            if (response.Response.ResponseCode == 200)
            {
                //ViewBag.Locations = new SelectList(response.data.Locations, "Key", "Value");
                //ViewBag.Materials = new SelectList(response.data.Materials, "Key", "Value");

                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "PurchaseID,Notes,LineItems")] KPILib.Models.PurchaseRcvMast rcvd)
        {
            if (ModelState.IsValid)
            {
                rcvd.RcvdByUserID = 1001;     //TODO: replace with UserID from session
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
using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_PO")]
    public class PurchaseController : CCSPLBaseController
    {
        // GET: Purchase
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllPurchases();
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
            var response = KPIAPIManager.GetAllByRMID(id);
            if (response.Response.ResponseCode == 200)
            {
                if (response.data.Count > 0)
                {
                    ViewBag.RMName = response.data.First().RawMaterialName;
                    ViewBag.TotalQty = " (" + response.data.Sum(x => x.Qty) + ")";
                }

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
            ViewData["CompanyLocationID"] = new SelectList(new List<SelectListItem>(), "CompanyLocationID", "CompanyLocationName");
            var response = KPIAPIManager.GetPurchase(id);
            if (response.Response.ResponseCode == 200)
            {
                VendorMasterModelResponse masterModelResponse = KPIAPIManager.GetAllVendorData();
                ViewData["CompanyLocationID"] = new SelectList(masterModelResponse.data, "VendorId", "VendorName");
                //ViewBag.Locations = new SelectList(response.data.Locations, "Key", "Value");
                ViewBag.Materials = new SelectList(response.data.Materials, "Key", "Value");

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
            ViewData["CompanyLocationID"] = new SelectList(new List<SelectListItem>(), "CompanyLocationID", "CompanyLocationName");
            var response = KPIAPIManager.GetPurchase(0);
            if (response.Response.ResponseCode == 200)
            {
                //ViewBag.Locations = new SelectList(response.data.Locations, "Key", "Value");
                ViewBag.Materials = new SelectList(response.data.Materials, "Key", "Value");
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult PurchaseLineItemsPartialAction(PurchaseDetails purchaseDetails)
        {
            ViewBag.Materials = new SelectList(new List<SelectList>(), "Key", "Value");
            var response = KPIAPIManager.GetPurchase(0);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.Materials = new SelectList(response.data.Materials, "Key", "Value");
            }
            return PartialView("_PurchaseLineItems", purchaseDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "PurchaseID,PurchaseDate,CompanyLocationID,Instructions,LineItems")] KPILib.Models.PurchaseMaster purchase)      //FormCollection frm)       //
        {
            //int i = 0;
            //foreach(var c in frm.AllKeys)
            //{
            //    Response.Write("Key " + c + " : " + c[i].ToString() + "<br/>");
            //    i++;
            //}

            //return View();

            if (ModelState.IsValid)
            {
                //purchase.UserID = 1001;     //TODO: replace with UserID from session
                purchase.UserID = GetUserSessionID();     //TODO: replace with UserID from session
                //purchase.LineItems.Add(new KPILib.Models.PurchaseDetails { RawMatarialID = 1002, Qty = 8 });
                //purchase.LineItems.Add(new KPILib.Models.PurchaseDetails { RawMatarialID = 1003, Qty = 2 });

                var response = KPIAPIManager.AddPurchase(purchase);
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
        public ActionResult Edit(KPILib.Models.PurchaseMaster purchase)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.EditPurchase(purchase);
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
        public ActionResult GetVendorWithLocationData(string vendorType)
        {
            VendorMasterModelResponse vendorMasterModel = KPIAPIManager.GetAllVendorData();

            if (vendorMasterModel != null && vendorMasterModel.Response.ResponseCode == 200)
            {
                vendorMasterModel.data = vendorMasterModel.data.Where(z => z.VendorType == vendorType).ToList();
                vendorMasterModel.data.ForEach(item =>
                {
                    item.VendorName += $" [{item.Address}]";
                });
            }
            return Json(vendorMasterModel.data);
        }

    }
}
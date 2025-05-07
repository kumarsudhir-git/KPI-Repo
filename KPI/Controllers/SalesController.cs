using KPI.Classes;
using KPI.Filters;
using KPILib;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Sales")]
    public class SalesController : CCSPLBaseController
    {
        // GET: Sales
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllSales();
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

        public ActionResult GetAllByProductID(int id)
        {
            var response = KPIAPIManager.GetAllByProductID(id);
            if (response.Response.ResponseCode == 200)
            {
                if (response.data.Count > 0)
                {
                    ViewBag.ProductName = response.data.First().ProductName;
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
            ViewData["Colour"] = new SelectList(new List<SelectListItem>(), "LookUpValue", "LookUpName");
            ViewData["GMS"] = new SelectList(new List<SelectListItem>(), "LookUpValue", "LookUpName");
            ViewData["RMIds"] = new SelectList(new List<SelectListItem>(), "RMId", "RMIdName");
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");
            ViewData["CompanyLocationID"] = new SelectList(new List<SelectListItem>(), "CompanyLocationID", "CompanyLocationName");

            var response = KPIAPIManager.GetSales(id, Convert.ToInt32(Session["RoleID"]));
            if (response.Response.ResponseCode == 200)
            {
                LookUpMasterResponse colorLookUpMaster = KPIAPIManager.GetLookUpData(ApplicationConstants.ColorLookUp);
                if (colorLookUpMaster != null && colorLookUpMaster.Response.ResponseCode == 200)
                {
                    ViewData["Colour"] = new SelectList(colorLookUpMaster.lookupMasterList, "LookUpValue", "LookUpName");
                }
                LookUpMasterResponse GSMLookUpMaster = KPIAPIManager.GetLookUpData(ApplicationConstants.GMSTypeLookUp);
                if (GSMLookUpMaster != null && GSMLookUpMaster.Response.ResponseCode == 200)
                {
                    ViewData["GMS"] = new SelectList(GSMLookUpMaster.lookupMasterList, "LookUpValue", "LookUpName");
                }
                RackMastersResponse rackMasterRspns = KPIAPIManager.GetRackMastersData();
                if (rackMasterRspns != null && rackMasterRspns.Response.ResponseCode == 200)
                {
                    List<SelectListItem> selectLists = new List<SelectListItem>();
                    // Add "Select All" option
                    selectLists.Insert(0, new SelectListItem { Value = "all", Text = "Select All" });

                    rackMasterRspns.data.ForEach(item =>
                    {
                        selectLists.Add(new SelectListItem { Value = item.RackID.ToString(), Text = item.RackNo });
                    });
                    ViewData["RMIds"] = selectLists;
                    //ViewData["RMIds"] = new SelectList(rackMasterRspns.data, "RackID", "RackNo");
                }
                VendorMasterModelResponse masterModelResponse = KPIAPIManager.GetAllVendorData();
                if (masterModelResponse != null && masterModelResponse.Response.ResponseCode == 200)
                {
                    ViewData["CompanyLocationID"] = new SelectList(masterModelResponse.data, "VendorId", "VendorName");
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
                //ViewBag.Locations = new SelectList(response.data.Locations, "Key", "Value");
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        public ActionResult New(int id = 0)
        {
            //ViewData["Colour"] = new SelectList(new List<SelectListItem>(), "LookUpValue", "LookUpName");
            //ViewData["RMIds"] = new SelectList(new List<SelectListItem>(), "RMId", "RMIdName");
            //ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");

            ViewData["CompanyLocationID"] = new SelectList(new List<SelectListItem>(), "CompanyID", "CompanyName");
            var response = KPIAPIManager.GetSales(id, Convert.ToInt32(Session["RoleID"]));
            if (response.Response.ResponseCode == 200)
            {
                //LookUpMasterResponse lookUpMaster = KPIAPIManager.GetLookUpData(ApplicationConstants.ColorLookUp);
                //if (lookUpMaster != null && lookUpMaster.Response.ResponseCode == 200)
                //{
                //    ViewData["Colour"] = new SelectList(lookUpMaster.lookupMasterList, "LookUpValue", "LookUpName");
                //}
                //RackMastersResponse rackMasterRspns = KPIAPIManager.GetRackMastersData();
                //if (rackMasterRspns != null && rackMasterRspns.Response.ResponseCode == 200)
                //{
                //    List<SelectListItem> selectLists = new List<SelectListItem>();
                //    // Add "Select All" option
                //    selectLists.Insert(0, new SelectListItem { Value = "0", Text = "Select All" });

                //    rackMasterRspns.data.ForEach(item =>
                //    {
                //        selectLists.Add(new SelectListItem
                //        {
                //            Value = item.RackID.ToString(),
                //            Text = item.RackNo,
                //            Selected = response.data.RMIds != null && response.data.RMIds.Contains(item.RackID)
                //        });
                //    });
                //    ViewData["RMIdSelectList"] = selectLists;//new SelectList(rackMasterRspns.data, "RackID", "RackNo");
                //}
                CompaniesResponse masterModelResponse = KPIAPIManager.GetAllCompanyList();
                if (masterModelResponse != null && masterModelResponse.Response.ResponseCode == 200)
                {
                    ViewData["CompanyLocationID"] = new SelectList(masterModelResponse.data, "CompanyID", "CompanyName");
                }
                //LocationMasterResponse locationMasterResponse = KPIAPIManager.GetListOfLocationMasterData();

                //if (locationMasterResponse != null && locationMasterResponse.Response != null)
                //{
                //    if (locationMasterResponse.Response.ResponseCode == 200)
                //    {
                //        if (locationMasterResponse.data != null && locationMasterResponse.data.Count > 0)
                //        {
                //            ViewData["LocationId"] = new SelectList(locationMasterResponse.data, "LocationId", "LocationName");
                //        }
                //    }
                //}
                //ViewBag.Locations = new SelectList(response.data.Locations, "Key", "Value");
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult GetSalesLineItemPartial(SalesDetails salesDetails)
        {
            ViewBag.Products = new SelectList(new List<SelectListItem>(), "ProductID", "ProductName");
            ViewData["GMS"] = new SelectList(new List<SelectListItem>(), "LookUpValue", "LookUpName");
            ViewData["SalesStatusID"] = new SelectList(new List<SelectListItem>(), "SalesStatusID", "SalesStatus");

            ProductMasterResponse productResponse = KPIAPIManager.GetProductsDetails();
            if (productResponse != null && productResponse.Response != null && productResponse.Response.ResponseCode == 200)
            {
                if (productResponse.productMastersListData != null)
                {
                    ViewBag.Products = new SelectList(productResponse.productMastersListData, "ProductID", "ProductName");
                }
            }
            LookUpMasterResponse GSMLookUpMaster = KPIAPIManager.GetLookUpData(ApplicationConstants.GMSTypeLookUp);
            if (GSMLookUpMaster != null && GSMLookUpMaster.Response.ResponseCode == 200)
            {
                ViewData["GMS"] = new SelectList(GSMLookUpMaster.lookupMasterList, "LookUpValue", "LookUpName");
            }
            //SalesMasterResponse companyResponse = KPIAPIManager.GetCompanyLocationDetails(103);
            //if (companyResponse != null && companyResponse.Response != null && companyResponse.Response.ResponseCode == 200)
            //{
            //    if (productResponse.Response.ResponseData != null)
            //    {
            //        ViewBag.Locations = companyResponse.Response.ResponseData;
            //    }
            //}
            SalesSatusMastersResponse salesSatusMasters = KPIAPIManager.GetSalesStatusMasterDetails();
            if (salesSatusMasters != null && salesSatusMasters.Response != null && salesSatusMasters.Response.ResponseCode == 200)
            {
                if (salesSatusMasters.data != null)
                {
                    ViewData["SalesStatusID"] = new SelectList(salesSatusMasters.data, "SalesStatusID", "SalesStatus");
                }
            }
            return PartialView("_SalesLineItems", salesDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Add([Bind(Include = "SalesID,SalesDate,CompanyLocationID,Instructions,LineItems")] KPILib.Models.SalesMaster sale)      //FormCollection frm)       //
        public ActionResult Add(KPILib.Models.SalesMaster sale)      //FormCollection frm)       //
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
                sale.UserID = Convert.ToInt32(Session["UserID"]);     //TODO: replace with UserID from session
                //sale.LineItems.Add(new KPILib.Models.SalesDetails { ProductID = 1002, Qty = 8 });
                //sale.LineItems.Add(new KPILib.Models.SalesDetails { ProductID = 1003, Qty = 2 });

                SalesMasterResponse response = new SalesMasterResponse();

                if (sale.SalesID == 0)
                {
                    response = KPIAPIManager.AddSales(sale);
                }
                else
                {
                    response = KPIAPIManager.EditSales(sale);
                }
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
        public ActionResult Edit(KPILib.Models.SalesMaster sale)
        {
            if (ModelState.IsValid)
            {
                sale.UserID = Convert.ToInt32(Session["UserID"]);

                var response = KPIAPIManager.EditSales(sale);
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
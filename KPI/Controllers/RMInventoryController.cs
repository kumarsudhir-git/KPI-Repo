using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_RMInventory")]
    public class RMInventoryController : CCSPLBaseController
    {
        // GET: RMInventory
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllRMInventory();
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

        public ActionResult GetAllByPallet(int id)
        {
            var response = KPIAPIManager.GetAllRMInventoryByRack(id);
            if (response.Response.ResponseCode == 200)
            {
                return View("GetAll", response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        public ActionResult GetAllByRM(int id)
        {
            var response = KPIAPIManager.GetAllRMInventoryByRM(id);
            if (response.Response.ResponseCode == 200)
            {
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

            var response = KPIAPIManager.GetRMInventory(id);
            if (response.Response.ResponseCode == 200)
            {
                //ViewBag.Pallets = new SelectList(response.data.Pallets, "PalletID", "PalletNo");
                ViewBag.RMs = new SelectList(response.data.RawMaterials, "RawMaterialID", "RawMaterialName");

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
        public ActionResult New(int id = 0)
        {
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");

            var response = KPIAPIManager.GetRMInventory(id);
            if (response.Response.ResponseCode == 200)
            {
                //ViewBag.Pallets = new SelectList(response.data.Pallets, "PalletID", "PalletNo");
                ViewBag.RMs = new SelectList(response.data.RawMaterials, "RawMaterialID", "RawMaterialName");
                ViewBag.Tags = new SelectList(response.data.TagColours, "TagColourID", "TagColour");

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
                //ViewBag.QtyKgs = 
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
        public ActionResult Add(KPILib.Models.RMInventory obj)
        {
            if (ModelState.IsValid)
            {
                obj.PalletID = 1002;// Hard coding this value as this field is not required as of now
                obj.AddedBy = Convert.ToInt32(Session["UserID"]);
                var response = KPIAPIManager.AddRMInventory(obj);
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
        public ActionResult Edit(KPILib.Models.RMInventory obj)
        {
            if (ModelState.IsValid)
            {
                obj.PalletID = 1002;// Hard coding this value as this field is not required as of now
                obj.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                var response = KPIAPIManager.EditRMInventory(obj);
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

        #region Master Batch 

        [HttpGet]
        public ActionResult GetAllMasterBatch()
        {
            RMInventoryMasterBatchResponse response = KPIAPIManager.GetAllMasterBatch();
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
        public ActionResult SaveMasterBatch(int MasterBatchId = 0)
        {
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");
            ViewData["VendorId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");

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
            RMInventoryMasterBatchResponse response = new RMInventoryMasterBatchResponse();

            if (MasterBatchId > 0)
            {
                response = KPIAPIManager.GetMasterBatchData(MasterBatchId);
            }
            return View(response.rMInventoryMasterBatchModel);
        }

        [HttpPost]
        public ActionResult SaveMasterBatch(RMInventoryMasterBatchModel rMInventoryMasterBatchModel)
        {
            if (rMInventoryMasterBatchModel.MasterBatchId == 0)
            {
                rMInventoryMasterBatchModel.AddedBy = Convert.ToInt32(Session["UserID"]);
            }
            else
            {
                rMInventoryMasterBatchModel.ModifiedBy = Convert.ToInt32(Session["UserID"]);
            }
            var response = KPIAPIManager.SaveMasterBatch(rMInventoryMasterBatchModel);
            if (response.Response.ResponseCode == 200)
            {
                return RedirectToAction("GetAllMasterBatch");
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult DeleteMasterBatch(RMInventoryMasterBatchModel rMInventoryMasterBatchModel)
        {
            RMInventoryMasterBatchResponse response = new RMInventoryMasterBatchResponse();

            if (Request.IsAjaxRequest())
            {
                ResponseObj responseObj = CheckSessionForAjaxCall();

                if (responseObj.ResponseCode == 440)
                {
                    response.Response = responseObj;

                    return Json(response);
                }
            }

            rMInventoryMasterBatchModel.ModifiedBy = Convert.ToInt32(Session["UserID"]);
            response = KPIAPIManager.DeleteMasterBatch(rMInventoryMasterBatchModel);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Packaging Bags

        [HttpGet]
        public ActionResult GetAllPackagBags()
        {
            var response = KPIAPIManager.GetAllPackagBags();
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
        public ActionResult SavePackageBags(int PackageBagId = 0)
        {
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");
            ViewData["VendorId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");
            //ViewData["ColorId"] = new SelectList(new List<SelectListItem>(), "TagColourID", "TagColour");

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
            
            //TagColorMasterModelResponse tagColourMasterModel = KPIAPIManager.GetTagColorMasterData();

            //if (tagColourMasterModel != null && tagColourMasterModel.Response != null)
            //{
            //    if (tagColourMasterModel.Response.ResponseCode == 200)
            //    {
            //        if (tagColourMasterModel.data != null && tagColourMasterModel.data.Count > 0)
            //        {
            //            ViewData["ColorId"] = new SelectList(tagColourMasterModel.data, "TagColourID", "TagColour");
            //        }
            //    }
            //}

            RMInventoryPackageBagsModelResponse response = new RMInventoryPackageBagsModelResponse();

            if (PackageBagId > 0)
            {
                response = KPIAPIManager.GetPackagBagData(PackageBagId);
            }
            return View(response.rMInventoryPackageBagsModel);
        }

        [HttpPost]
        public ActionResult SavePackageBags(RMInventoryPackageBagsModel rMInventoryPackageBags)
        {
            if (rMInventoryPackageBags.PackageBagId == 0)
            {
                rMInventoryPackageBags.AddedBy = Convert.ToInt32(Session["UserID"]);
            }
            else
            {
                rMInventoryPackageBags.ModifiedBy = Convert.ToInt32(Session["UserID"]);
            }
            var response = KPIAPIManager.SavePackageBags(rMInventoryPackageBags);
            if (response.Response.ResponseCode == 200)
            {
                return RedirectToAction("GetAllPackagBags");
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult DeletePackageBag(RMInventoryPackageBagsModel rMInventoryPackageBags)
        {
            RMInventoryPackageBagsModelResponse response = new RMInventoryPackageBagsModelResponse();

            if (Request.IsAjaxRequest())
            {
                ResponseObj responseObj = CheckSessionForAjaxCall();

                if (responseObj.ResponseCode == 440)
                {
                    response.Response = responseObj;

                    return Json(response);
                }
            }

            rMInventoryPackageBags.ModifiedBy = Convert.ToInt32(Session["UserID"]);
            response = KPIAPIManager.DeletePackageBags(rMInventoryPackageBags);
            return Json(response);
        }

        #endregion

        #region Finished Good

        [HttpGet]
        public ActionResult GetAllFinishedGood()
        {
            var response = KPIAPIManager.GetAllFinishedGood();
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
        public ActionResult SaveFinishedGood(int FinishedGoodId = 0)
        {
            ViewData["LocationId"] = new SelectList(new List<SelectListItem>(), "LocationId", "LocationName");
            ViewData["ProductId"] = new SelectList(new List<SelectListItem>(), "ProductID", "ProductName");
            ViewData["RackId"] = new SelectList(new List<SelectListItem>(), "RackID", "RackNo");

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

            ProductsResponse productsResponse = KPIAPIManager.GetAllProducts();

            if (productsResponse != null && productsResponse.Response != null)
            {
                if (productsResponse.Response.ResponseCode == 200)
                {
                    if (productsResponse.data != null && productsResponse.data.Count > 0)
                    {
                        ViewData["ProductId"] = new SelectList(productsResponse.data, "ProductID", "ProductName");
                    }
                }
            }

            RackMastersResponse rackMastersResponse = KPIAPIManager.GetAllRacks();

            if (rackMastersResponse != null && rackMastersResponse.Response != null)
            {
                if (rackMastersResponse.Response.ResponseCode == 200)
                {
                    if (rackMastersResponse.data != null && rackMastersResponse.data.Count > 0)
                    {
                        ViewData["RackId"] = new SelectList(rackMastersResponse.data, "RackID", "RackNo");
                    }
                }
            }

            RMInventoryFinishedGoodResponse response = new RMInventoryFinishedGoodResponse();

            if (FinishedGoodId > 0)
            {
                response = KPIAPIManager.GetFinishedGoodData(FinishedGoodId);
            }
            return View(response.rMInventoryFinishedGoodModel);
        }

        [HttpPost]
        public ActionResult SaveFinishedGood(RMInventoryFinishedGoodModel rMInventoryFinished)
        {
            if (rMInventoryFinished.FinishedGoodId == 0)
            {
                rMInventoryFinished.AddedBy = Convert.ToInt32(Session["UserID"]);
            }
            else
            {
                rMInventoryFinished.ModifiedBy = Convert.ToInt32(Session["UserID"]);
            }
            var response = KPIAPIManager.SaveFinishedGood(rMInventoryFinished);
            if (response.Response.ResponseCode == 200)
            {
                return RedirectToAction("GetAllFinishedGood");
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult DeleteFinishedGood(RMInventoryFinishedGoodModel rMInventoryFinished)
        {
            RMInventoryFinishedGoodResponse response = new RMInventoryFinishedGoodResponse();

            if (Request.IsAjaxRequest())
            {
                ResponseObj responseObj = CheckSessionForAjaxCall();

                if (responseObj.ResponseCode == 440)
                {
                    response.Response = responseObj;

                    return Json(response);
                }
            }

            rMInventoryFinished.ModifiedBy = Convert.ToInt32(Session["UserID"]);
            response = KPIAPIManager.DeleteFinishedGood(rMInventoryFinished);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
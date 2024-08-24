using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_RMInventory")]
    public class RMInventoryAPIController : CCSPLBaseAPIController
    {
        // GET api/values
        public IHttpActionResult GetAll()
        {
            var returnValue = new RMInventorysResponse();

            try
            {
                //var data = db.RawMaterialInventoryMasters.OrderByDescending(x => x.RMInventoryID).ToList();

                //var data = db.RawMaterialInventoryMasters.GroupBy(x => new { x.PalletID, x.RawMaterialID, x.TagColourID }).Select(grp => new RawMaterialInventoryMaster { PalletID = grp.Key.PalletID, RawMaterialID = grp.Key.RawMaterialID, TagColourID = grp.Key.TagColourID }).OrderByDescending(x => x.RMInventoryID).ToList();
                //var data = db.RawMaterialInventoryMasters
                //    .GroupBy(x => new { 
                //        x.PalletID, 
                //        x.RawMaterialID, 
                //        x.TagColourID
                //    }).Select(grp => new RawMaterialInventoryMaster { 
                //        PalletID = grp.Key.PalletID, 
                //        RawMaterialID = grp.Key.RawMaterialID, 
                //        TagColourID = grp.Key.TagColourID, 
                //    }).OrderByDescending(x => x.RMInventoryID).ToList();

                //var data = db.Database.SqlQuery(typeof(List<RMInventory>), "select rmim.PalletID,pm.PalletNo,rmim.TagColourID,tcm.TagColour,rmim.RawMaterialID,rmm.RawMaterialName,sum(rmim.Qty) Qty from RawMaterialInventoryMaster rmim, PalletMaster pm, RawMaterialMaster rmm,TagColourMaster tcm where pm.PalletID = rmim.PalletID and rmm.RawMaterialID = rmim.RawMaterialID and tcm.TagColourID = rmim.TagColourID group by rmim.PalletID, pm.PalletNo, rmim.TagColourID, tcm.TagColour, rmim.RawMaterialID, rmm.RawMaterialName", new object[] { });
                //var data = db.Database.ExecuteSqlCommand  Database.SqlQuery(typeof(List<RMInventory>), "select rmim.PalletID,pm.PalletNo,rmim.TagColourID,tcm.TagColour,rmim.RawMaterialID,rmm.RawMaterialName,sum(rmim.Qty) Qty from RawMaterialInventoryMaster rmim, PalletMaster pm, RawMaterialMaster rmm,TagColourMaster tcm where pm.PalletID = rmim.PalletID and rmm.RawMaterialID = rmim.RawMaterialID and tcm.TagColourID = rmim.TagColourID group by rmim.PalletID, pm.PalletNo, rmim.TagColourID, tcm.TagColour, rmim.RawMaterialID, rmm.RawMaterialName", new object[] { });


                var data = db.sp_GetRMInventory();
                foreach (var obj in data)
                {
                    var o = mapper.Map<sp_GetRMInventory_Result, RMInventory>(obj);

                    //o.Qty = o.Qty;
                    //o.QtyBags = o.Qty / RM_BAG_CAPACITY;
                    //o.QtyOpened
                    if (o.AddedBy != null && o.AddedBy > 0)
                    {
                        UserMaster userMaster = CommonFunctions.GetUserMasterData((int)o.AddedBy);
                        if (userMaster != null)
                        {
                            o.AddedByName = userMaster.Username;
                        }
                    }
                    if (o.ModifiedBy != null && o.ModifiedBy > 0)
                    {
                        UserMaster userMaster = CommonFunctions.GetUserMasterData((int)o.ModifiedBy);
                        if (userMaster != null)
                        {
                            o.ModifiedByName = userMaster.Username;
                        }
                    }
                    if (o.LocationId != null && o.LocationId > 0)
                    {
                        o.LocationName = CommonFunctions.getLocationNameFromId((int)o.LocationId);
                    }

                    returnValue.data.Add(o);
                }

                //foreach (var obj in data)
                //{
                //    var o = mapper.Map<RawMaterialInventoryMaster, RMInventory>(obj);
                //    o.PalletNo = obj.PalletMaster.PalletNo;
                //    o.RawMaterial = obj.RawMaterialMaster.RawMaterialName;
                //    o.QtyKgs = Convert.ToInt32(obj.Qty);
                //    o.Qty = Convert.ToInt32(obj.Qty) / RM_BAG_CAPACITY;
                //    o.TagColour = obj.TagColourMaster.TagColour;

                //    returnValue.data.Add(o);
                //}

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult GetAllByRack(int id)
        {
            var returnValue = new RMInventorysResponse();

            try
            {
                var data = db.RawMaterialInventoryMasters.Where(x => x.PalletID == id).OrderByDescending(x => x.RMInventoryID).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<RawMaterialInventoryMaster, RMInventory>(obj);
                    o.PalletNo = obj.PalletMaster.PalletNo;
                    o.RawMaterialName = obj.RawMaterialMaster.RawMaterialName;

                    returnValue.data.Add(o);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult GetAllByRM(int id)
        {
            var returnValue = new RMInventorysResponse();

            try
            {
                var data = db.RawMaterialInventoryMasters.Where(x => x.RawMaterialID == id).OrderByDescending(x => x.RMInventoryID).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<RawMaterialInventoryMaster, RMInventory>(obj);
                    o.PalletNo = obj.PalletMaster.PalletNo;
                    o.RawMaterialName = obj.RawMaterialMaster.RawMaterialName;

                    returnValue.data.Add(o);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            var returnValue = new RMInventoryResponse();

            try
            {
                #region get all Pallets for ddl
                var Pallets = db.PalletMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.PalletNo).ToList();
                List<KPILib.Models.PalletMaster> pallets = new List<KPILib.Models.PalletMaster>();
                foreach (var obj in Pallets)
                {
                    var o = mapper.Map<PalletMaster, KPILib.Models.PalletMaster>(obj);

                    pallets.Add(o);
                }
                #endregion

                #region get all RawMaterials for ddl
                var RawMaterials = db.RawMaterialMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.RawMaterialName).ToList();
                List<KPILib.Models.RawMaterial> rms = new List<KPILib.Models.RawMaterial>();
                foreach (var obj in RawMaterials)
                {
                    var o = mapper.Map<RawMaterialMaster, KPILib.Models.RawMaterial>(obj);

                    rms.Add(o);
                }
                #endregion

                #region get all TagColours for ddl
                var Tags = db.TagColourMasters.Where(x => !x.IsDiscontinued).ToList();
                List<KPILib.Models.TagColor> tags = new List<KPILib.Models.TagColor>();
                foreach (var obj in Tags)
                {
                    var o = mapper.Map<TagColourMaster, KPILib.Models.TagColor>(obj);

                    tags.Add(o);
                }
                #endregion

                var data = db.RawMaterialInventoryMasters.SingleOrDefault(x => x.RMInventoryID == id);
                if (data != null)
                {
                    var o = mapper.Map<RawMaterialInventoryMaster, RMInventory>(data);
                    o.PalletNo = data.PalletMaster.PalletNo;
                    o.Pallets = pallets;
                    o.RawMaterialName = data.RawMaterialMaster.RawMaterialName;
                    o.RawMaterials = rms;
                    o.TagColours = tags;

                    returnValue.data = o;
                }
                else
                {
                    var o = new RMInventory() { Pallets = pallets, RawMaterials = rms, TagColours = tags };
                    returnValue.data = o;
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Add(RMInventory data)
        {
            var returnValue = new RMInventoryResponse();

            try
            {
                var o = mapper.Map<RMInventory, RawMaterialInventoryMaster>(data);
                o.AddedOn = DateTime.Now;
                //o.LastModifiedOn = DateTime.Now;

                db.RawMaterialInventoryMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.RMInventoryID = o.RMInventoryID;

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Edit(RMInventory data)
        {
            var returnValue = new RMInventoryResponse();

            try
            {
                var o = db.RawMaterialInventoryMasters.SingleOrDefault(x => x.RMInventoryID == data.RMInventoryID);
                if (o != null)
                {
                    o.PalletID = data.PalletID;
                    o.RawMaterialID = data.RawMaterialID;
                    o.Qty = data.Qty;
                    o.MinOrderlevel = data.MinOrderlevel;
                    o.LastModifiedOn = DateTime.Now;
                    o.ModifiedBy = data.ModifiedBy;

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }

                returnValue.data = data;
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        #region Master Batch 

        public IHttpActionResult GetAllMasterBatch()
        {
            RMInventoryMasterBatchResponse rMInventoryMasterBatch = new RMInventoryMasterBatchResponse();

            try
            {
                List<usp_GetRMInventoryMasterBatch_Result> masterBatches = db.usp_GetRMInventoryMasterBatch().ToList();

                rMInventoryMasterBatch.data = mapper.Map<List<RMInventoryMasterBatchModel>>(masterBatches);

                rMInventoryMasterBatch.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                rMInventoryMasterBatch.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryMasterBatch.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryMasterBatch);
        }

        public IHttpActionResult GetMasterBatchData(int BatchId = 0)
        {
            RMInventoryMasterBatchResponse rMInventoryMasterBatch = new RMInventoryMasterBatchResponse();

            try
            {
                if (BatchId > 0)
                {
                    RMInventoryMasterBatch rMInventory = getMasterBatchModel(BatchId);
                    rMInventoryMasterBatch.rMInventoryMasterBatchModel = mapper.Map<RMInventoryMasterBatchModel>(rMInventory);
                }
                rMInventoryMasterBatch.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                rMInventoryMasterBatch.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryMasterBatch.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryMasterBatch);
        }

        [HttpPost]
        public IHttpActionResult SaveMasterBatch(RMInventoryMasterBatchModel rMInventoryMasterBatchModel)
        {
            RMInventoryMasterBatchResponse rMInventoryMasterBatch = new RMInventoryMasterBatchResponse();

            try
            {
                if (rMInventoryMasterBatchModel.MasterBatchId == 0)
                {
                    RMInventoryMasterBatch rMInventory = mapper.Map<RMInventoryMasterBatch>(rMInventoryMasterBatchModel);

                    rMInventory.IsActive = true;
                    rMInventory.AddedOn = DateTime.Now;
                    db.RMInventoryMasterBatches.Add(rMInventory);
                }
                else
                {
                    RMInventoryMasterBatch rMInventory = getMasterBatchModel(rMInventoryMasterBatchModel.MasterBatchId);

                    if (rMInventory != null)
                    {
                        rMInventory.CodeNo = rMInventoryMasterBatchModel.CodeNo;
                        rMInventory.Colour = rMInventoryMasterBatchModel.Colour;
                        rMInventory.VendorId = rMInventoryMasterBatchModel.VendorId;
                        rMInventory.LocationId = rMInventoryMasterBatchModel.LocationId;
                        rMInventory.MinOrderLevel = rMInventoryMasterBatchModel.MinOrderLevel;
                        rMInventory.QtyInStock = rMInventoryMasterBatchModel.QtyInStock;
                        rMInventory.ModifiedBy = rMInventoryMasterBatchModel.ModifiedBy;
                        rMInventory.ModifiedOn = DateTime.Now;

                        db.Entry(rMInventory).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                db.SaveChanges();
                rMInventoryMasterBatch.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                rMInventoryMasterBatch.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryMasterBatch.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryMasterBatch);
        }

        [HttpPost]
        public IHttpActionResult DeleteMasterBatch(RMInventoryMasterBatchModel rMInventoryMasterBatchModel)
        {
            RMInventoryMasterBatchResponse rMInventoryMasterBatch = new RMInventoryMasterBatchResponse();

            try
            {
                RMInventoryMasterBatch rMInventory = getMasterBatchModel(rMInventoryMasterBatchModel.MasterBatchId);

                if (rMInventory != null)
                {
                    rMInventory.IsActive = false;
                    rMInventory.ModifiedOn = DateTime.Now;
                    rMInventory.ModifiedBy = rMInventoryMasterBatchModel.ModifiedBy;

                    db.Entry(rMInventory).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                    rMInventoryMasterBatch.Response.IsSuccessful();
                }
                else
                {
                    rMInventoryMasterBatch.Response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    rMInventoryMasterBatch.Response.ResponseMsg = "Data Not Found";
                }
            }
            catch (Exception ex)
            {
                rMInventoryMasterBatch.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryMasterBatch.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryMasterBatch);
        }

        private RMInventoryMasterBatch getMasterBatchModel(int BatchId = 0)
        {
            RMInventoryMasterBatch masterBatchObj = (from MB in db.RMInventoryMasterBatches
                                                     where MB.MasterBatchId == BatchId
                                                     select MB).FirstOrDefault();

            return masterBatchObj;
        }

        #endregion

        #region Packaging Bags

        public IHttpActionResult GetAllPackagBags()
        {
            RMInventoryPackageBagsModelResponse rMInventoryPackageBags = new RMInventoryPackageBagsModelResponse();

            try
            {
                List<usp_GetRMInventoryPackageBags_Result> bagsData = db.usp_GetRMInventoryPackageBags().ToList();

                rMInventoryPackageBags.data = mapper.Map<List<RMInventoryPackageBagsModel>>(bagsData);

                rMInventoryPackageBags.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                rMInventoryPackageBags.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryPackageBags.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryPackageBags);
        }

        public IHttpActionResult GetPackagBagData(int PackagBagId = 0)
        {
            RMInventoryPackageBagsModelResponse rMInventoryPackageBags = new RMInventoryPackageBagsModelResponse();

            try
            {
                RMInventoryPackageBag rMInventoryPackage = getPackageBagModel(PackagBagId);
                if (rMInventoryPackage != null)
                {
                    rMInventoryPackageBags.rMInventoryPackageBagsModel = mapper.Map<RMInventoryPackageBagsModel>(rMInventoryPackage);

                    rMInventoryPackageBags.Response.IsSuccessful();
                }
                else
                {
                    rMInventoryPackageBags.Response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    rMInventoryPackageBags.Response.ResponseMsg = "No Data Found";
                }
            }
            catch (Exception ex)
            {
                rMInventoryPackageBags.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryPackageBags.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryPackageBags);
        }

        [HttpPost]
        public IHttpActionResult SavePackageBags(RMInventoryPackageBagsModel rMInventoryPackageBags)
        {
            RMInventoryPackageBagsModelResponse rMInventoryPackageBagsResponse = new RMInventoryPackageBagsModelResponse();

            try
            {
                if (rMInventoryPackageBags.PackageBagId == 0)
                {
                    RMInventoryPackageBag rMInventoryPackage = mapper.Map<RMInventoryPackageBag>(rMInventoryPackageBags);

                    rMInventoryPackage.IsActive = true;
                    rMInventoryPackage.AddedOn = DateTime.Now;

                    db.RMInventoryPackageBags.Add(rMInventoryPackage);
                }
                else
                {
                    RMInventoryPackageBag rMInventoryPackageObj = getPackageBagModel(rMInventoryPackageBags.PackageBagId);

                    if (rMInventoryPackageObj != null)
                    {
                        rMInventoryPackageObj.LocationId = rMInventoryPackageBags.LocationId;
                        rMInventoryPackageObj.MinOrderLevel = rMInventoryPackageBags.MinOrderLevel;
                        rMInventoryPackageObj.QtyInStock = rMInventoryPackageBags.QtyInStock;
                        rMInventoryPackageObj.Size = rMInventoryPackageBags.Size;
                        rMInventoryPackageObj.VendorId = rMInventoryPackageBags.VendorId;
                        rMInventoryPackageObj.ModifiedBy = rMInventoryPackageBags.ModifiedBy;
                        rMInventoryPackageObj.ModifiedOn = DateTime.Now;

                        db.Entry(rMInventoryPackageObj).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                db.SaveChanges();
                rMInventoryPackageBagsResponse.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                rMInventoryPackageBagsResponse.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryPackageBagsResponse.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryPackageBagsResponse);
        }

        [HttpPost]
        public IHttpActionResult DeletePackageBags(RMInventoryPackageBagsModel rMInventoryPackageBags)
        {
            RMInventoryPackageBagsModelResponse rMInventoryPackageBagsResponse = new RMInventoryPackageBagsModelResponse();

            try
            {
                RMInventoryPackageBag rMInventoryPackage = getPackageBagModel(rMInventoryPackageBags.PackageBagId);

                if (rMInventoryPackage != null)
                {
                    rMInventoryPackage.IsActive = false;
                    rMInventoryPackage.ModifiedOn = DateTime.Now;
                    rMInventoryPackage.ModifiedBy = rMInventoryPackageBags.ModifiedBy;

                    db.Entry(rMInventoryPackage).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                    rMInventoryPackageBagsResponse.Response.IsSuccessful();
                }
                else
                {
                    rMInventoryPackageBagsResponse.Response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    rMInventoryPackageBagsResponse.Response.ResponseMsg = "Data Not Found";
                }
            }
            catch (Exception ex)
            {
                rMInventoryPackageBagsResponse.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryPackageBagsResponse.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryPackageBagsResponse);
        }

        private RMInventoryPackageBag getPackageBagModel(int PackagBagId = 0)
        {
            RMInventoryPackageBag packageBagModel = (from PB in db.RMInventoryPackageBags
                                                     where PB.PackageBagId == PackagBagId
                                                     select PB).FirstOrDefault();

            return packageBagModel;
        }

        #endregion

        #region Finished Good

        public IHttpActionResult GetAllFinishedGood()
        {
            RMInventoryFinishedGoodResponse rMInventoryFinishedGood = new RMInventoryFinishedGoodResponse();

            try
            {
                List<usp_GetRMInventoryFinishedGood_Result> bagsData = db.usp_GetRMInventoryFinishedGood().ToList();

                rMInventoryFinishedGood.data = mapper.Map<List<RMInventoryFinishedGoodModel>>(bagsData);

                rMInventoryFinishedGood.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                rMInventoryFinishedGood.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryFinishedGood.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryFinishedGood);
        }

        public IHttpActionResult GetFinishedGoodData(int FinishedGoodId = 0)
        {
            RMInventoryFinishedGoodResponse rMInventoryFinishedGood = new RMInventoryFinishedGoodResponse();

            try
            {
                RMInventoryFinishedGood rMInventory = getFinishedGoodModel(FinishedGoodId);
                if (rMInventory != null)
                {
                    rMInventoryFinishedGood.rMInventoryFinishedGoodModel = mapper.Map<RMInventoryFinishedGoodModel>(rMInventory);

                    rMInventoryFinishedGood.Response.IsSuccessful();
                }
                else
                {
                    rMInventoryFinishedGood.Response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    rMInventoryFinishedGood.Response.ResponseMsg = "No Data Found";
                }
            }
            catch (Exception ex)
            {
                rMInventoryFinishedGood.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryFinishedGood.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryFinishedGood);
        }

        [HttpPost]
        public IHttpActionResult SaveFinishedGood(RMInventoryFinishedGoodModel rMInventoryFinishedGood)
        {
            RMInventoryFinishedGoodResponse rMInventoryFinishedGoodResponse = new RMInventoryFinishedGoodResponse();

            try
            {
                if (rMInventoryFinishedGood.FinishedGoodId == 0)
                {
                    RMInventoryFinishedGood rMInventoryFinished = mapper.Map<RMInventoryFinishedGood>(rMInventoryFinishedGood);

                    rMInventoryFinished.IsActive = true;
                    rMInventoryFinished.AddedOn = DateTime.Now;

                    db.RMInventoryFinishedGoods.Add(rMInventoryFinished);
                }
                else
                {
                    RMInventoryFinishedGood rMInventory = getFinishedGoodModel(rMInventoryFinishedGood.FinishedGoodId);
                    if (rMInventory != null)
                    {
                        rMInventory.LocationId = rMInventoryFinishedGood.LocationId;
                        rMInventory.MinOrderLevel = rMInventoryFinishedGood.MinOrderLevel;
                        rMInventory.Package = rMInventoryFinishedGood.Package;
                        rMInventory.ProductId = rMInventoryFinishedGood.ProductId;
                        rMInventory.Qty = rMInventoryFinishedGood.Qty;
                        rMInventory.RackId = rMInventoryFinishedGood.RackId;
                        rMInventory.ModifiedBy = rMInventoryFinishedGood.ModifiedBy;
                        rMInventory.ModifiedOn = DateTime.Now;

                        db.Entry(rMInventory).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                db.SaveChanges();
                rMInventoryFinishedGoodResponse.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                rMInventoryFinishedGoodResponse.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryFinishedGoodResponse.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryFinishedGoodResponse);
        }

        [HttpPost]
        public IHttpActionResult DeleteFinishedGood(RMInventoryFinishedGoodModel rMInventoryFinishedGood)
        {
            RMInventoryFinishedGoodResponse rMInventoryFinishedGoodResponse = new RMInventoryFinishedGoodResponse();

            try
            {
                RMInventoryFinishedGood rMInventoryFinished = getFinishedGoodModel(rMInventoryFinishedGood.FinishedGoodId);

                if (rMInventoryFinished != null)
                {
                    rMInventoryFinished.IsActive = false;
                    rMInventoryFinished.ModifiedOn = DateTime.Now;
                    rMInventoryFinished.ModifiedBy = rMInventoryFinishedGood.ModifiedBy;

                    db.Entry(rMInventoryFinished).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                    rMInventoryFinishedGoodResponse.Response.IsSuccessful();
                }
                else
                {
                    rMInventoryFinishedGoodResponse.Response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    rMInventoryFinishedGoodResponse.Response.ResponseMsg = "Data Not Found";
                }
            }
            catch (Exception ex)
            {
                rMInventoryFinishedGoodResponse.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                rMInventoryFinishedGoodResponse.Response.ResponseMsg = ex.Message;
            }
            return Json(rMInventoryFinishedGoodResponse);
        }

        private RMInventoryFinishedGood getFinishedGoodModel(int FinishedGoodId = 0)
        {
            RMInventoryFinishedGood FinishedGood = (from FG in db.RMInventoryFinishedGoods
                                                    where FG.FinishedGoodId == FinishedGoodId
                                                    select FG).FirstOrDefault();
            return FinishedGood;
        }

        #endregion

    }
}
using KPILib.Models;
using KPIWebAPI.AuthFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_PR")]
    public class PurchaseRcvAPIController : CCSPLBaseAPIController
    {
        static object myLock = new object();

        #region PurchaseRcv

        // GET api/values
        public IHttpActionResult GetAll()
        {
            var returnValue = new PurchaseRcvMastsResponse();

            try
            {
                var allUsers = db.UserMasters.ToList();

                var data = db.PurchaseRcvdMasters.OrderByDescending(x => x.PurchaseRcvdID).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<PurchaseRcvdMaster, KPILib.Models.PurchaseRcvMast>(obj);
                    o.PurchaseDate = obj.PurchaseMaster.PurchaseDate;
                    o.CompanyLocation = obj.PurchaseMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + obj.PurchaseMaster.CompanyLocationMaster.LocationName + "]";
                    o.User = obj.PurchaseMaster.UserMaster.Username;
                    o.Status = obj.PurchaseMaster.PurchaseStatusMaster.PurchaseStatus;
                    o.ReceivedByUser = allUsers.SingleOrDefault(x => x.UserID == obj.RcvdByUserID).Username;
                    o.LineItems = new List<PurchaseRcvDet>();

                    foreach (var item in obj.PurchaseRcvdDetails)
                        o.LineItems.Add(mapper.Map<PurchaseRcvdDetail, KPILib.Models.PurchaseRcvDet>(item));

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

        // GetNewRcv api/values/5
        public IHttpActionResult GetNewRcv(int id)
        {
            var returnValue = new PurchaseRcvMastResponse();

            try
            {
                var allUsers = db.UserMasters.ToList();

                var po = db.PurchaseMasters.SingleOrDefault(x => x.PurchaseID == id);
                if (po != null)
                {
                    returnValue.data = new PurchaseRcvMast
                    {
                        CompanyLocation = po.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + po.CompanyLocationMaster.LocationName + "]",
                        Instructions = po.Instructions,
                        PurchaseDate = po.PurchaseDate,
                        PurchaseID = po.PurchaseID,
                        Status = po.PurchaseStatusMaster.PurchaseStatus,
                        RcvdByUserID = 1001,
                        ReceivedByUser = allUsers.SingleOrDefault(x => x.UserID == 1001).Username,
                        User = po.UserMaster.Username,
                        LineItems = new List<PurchaseRcvDet>()
                    };

                    var allRcvd = db.PurchaseRcvdDetails.Where(x => x.PurchaseRcvdMaster.PurchaseID == po.PurchaseID).ToList();

                    foreach (var item in po.PurchaseDetails)
                    {
                        var itm = new KPILib.Models.PurchaseRcvDet();
                        itm.PurchaseDetailsID = item.PurchaseDetailsID;
                        itm.RawMatarialID = item.RawMatarialID;
                        itm.RawMatarialName = item.RawMaterialMaster.RawMaterialName;
                        itm.Qty = item.Qty;
                        itm.QtyRcvdSoFar = allRcvd.Where(x => x.PurchaseDetailsID == item.PurchaseDetailsID).Sum(x => x.Qty);
                        itm.QtyBalance = itm.Qty - itm.QtyRcvdSoFar;

                        returnValue.data.LineItems.Add(itm);
                    }

                    returnValue.Response.IsSuccessful();
                }
                else
                {
                    returnValue.Response.ResponseMsg = "PO No. " + id.ToString() + " does not exist.";
                }
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }


        //// GET api/values/5
        //public IHttpActionResult Get(int id)
        //{
        //    var returnValue = new RawMaterialResponse();

        //    try
        //    {
        //        #region get all UOM for ddl
        //        var UOMs = db.UOMMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.UnitsName).ToList();
        //        List<UOM> uoms = new List<UOM>();
        //        foreach (var obj in UOMs)
        //        {
        //            var o = mapper.Map<UOMMaster, KPILib.Models.UOM>(obj);

        //            uoms.Add(o);
        //        }
        //        #endregion

        //        var data = db.RawMaterialMasters.SingleOrDefault(x => x.RawMaterialID == id);
        //        if (data != null)
        //        {
        //            var o = mapper.Map<RawMaterialMaster, KPILib.Models.RawMaterial>(data);
        //            o.UOM = data.UOMMaster.UnitsName;
        //            o.UOMs = uoms;

        //            returnValue.data = o;
        //        }
        //        else
        //        {
        //            var o = new KPILib.Models.RawMaterial() { UOMs = uoms };
        //            returnValue.data = o;
        //        }

        //        returnValue.Response.IsSuccessful();
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO error handling
        //        returnValue.Response.ResponseMsg = ex.Message;
        //    }

        //    return Json(returnValue);
        //}

        public IHttpActionResult Add(KPILib.Models.PurchaseRcvMast data)
        {
            var returnValue = new PurchaseRcvMastResponse();

            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region ### old code...
                        ////############ BETTER APPROACH  ############
                        ////var racks = db.RackMasters
                        ////    .Where(x => !x.IsDiscontinued);
                        ////    .Select(x => new { x.RackID, RMQty = x.RawMaterialInventoryMasters.Sum(y => y.Qty), ProdQty = x.ProductInventoryMasters.Sum(z => z.Qty) });
                        ////    .Where(x => (x.RMQty == 0 || x.RMQty == null) && (x.ProdQty == 0 || x.ProdQty == null))
                        ////    .Select(x => new { x.RackID, RackQty = x.RMQty + x.ProdQty });

                        ////############ POOR APPROACH  ############
                        ////var racks = new Dictionary<int, int>();
                        ////foreach (var r in db.RackMasters.Where(x => !x.IsDiscontinued))
                        ////{ 
                        ////    var rmQty = db.RawMaterialInventoryMasters.Sum()
                        ////    racks.Add(r.RackID, 0);
                        ////}

                        //var racks = db.sp_GetEmptyRacks();
                        #endregion

                        //### Get all pallets that have empty space -AND- dont have all the tag colours in the pallet
                        //### -AND- have more of the raw material to be stacked
                        var allPallets = db.PalletMasters.Where(x => !x.IsDiscontinued);


                        //### new entry into PurchaseRcvdMaster
                        var o = new PurchaseRcvdMaster();
                        o.Notes = data.Notes + "";
                        o.PurchaseID = data.PurchaseID;
                        o.RcvdByUserID = data.RcvdByUserID;
                        o.RcvdDate = DateTime.Now;
                        o.PurchaseRcvdDetails = new List<PurchaseRcvdDetail>();

                        //### add entry for line items
                        foreach (var item in data.LineItems)
                        {
                            if (item.QtyRcvdNow > 0)
                            {
                                var itm = new PurchaseRcvdDetail { PurchaseDetailsID = item.PurchaseDetailsID, Qty = item.QtyRcvdNow };
                                itm.PurchaseRcvdStoreds = new List<PurchaseRcvdStored>();

                                //int iPalletCapacity = PALLET_CAPACITY;
                                int iQty = item.QtyRcvdNow;

                                while (iQty > 0)
                                {
                                    //### Get all the available tag colours in random order
                                    var tagColours = db.TagColourMasters.Where(x => !x.IsDiscontinued).OrderBy(x => Guid.NewGuid()).ToList();


                                    //### Create List of new class... PalletID, PalletNo, PalletCapacity, PalletOccupancy (Kgs), PalletAvailableWt (Kgs), TagColours (List<ID>), RawMaterials (List<ID, Qty>)
                                    var availablePallets = new List<dynamic>();

                                    foreach (var pallet in allPallets)
                                    {
                                        //### selecct pallets ONLY if not at full occupancy
                                        if (pallet.RawMaterialInventoryMasters.Sum(x => x.Qty) < PALLET_CAPACITY)
                                        {
                                            availablePallets.Add(new
                                            {
                                                PalletID = pallet.PalletID,
                                                PalletNo = pallet.PalletNo,
                                                PalletCapacity = PALLET_CAPACITY,
                                                PalletOccupency = pallet.RawMaterialInventoryMasters.Sum(x => x.Qty),
                                                PalletAvailableWt = (int)(PALLET_CAPACITY - pallet.RawMaterialInventoryMasters.Sum(x => x.Qty)),
                                                TagColours = pallet.RawMaterialInventoryMasters.Select(x => new { x.TagColourID }).Distinct(),
                                                TagColoursCount = pallet.RawMaterialInventoryMasters.Select(x => new { x.TagColourID }).Distinct().Count(),
                                                RawMaterials = pallet.RawMaterialInventoryMasters.Select(x => new { x.RawMaterialID, Qty = x.Qty })
                                            });
                                        }
                                    }

                                    //### ignore all pallets where there is no available space
                                    availablePallets = availablePallets.Where(x => x.PalletAvailableWt > 0).ToList();

                                    //### ignore all pallets where all tag colours are already taken
                                    availablePallets = availablePallets.Where(x => x.TagColoursCount < tagColours.Count).ToList();

                                    //### order by most available space and least tag colours
                                    availablePallets = availablePallets.OrderByDescending(x => x.PalletAvailableWt).ThenBy(x => x.TagColoursCount).ToList();


                                    var emptyPallet = availablePallets.FirstOrDefault();
                                    var availableTagColours = tagColours.ToList();

                                    //### from the available tag colours, remove the tag colours already in the pallet
                                    foreach (var tagcolor in emptyPallet.TagColours)
                                    {
                                        availableTagColours.Remove(availableTagColours.Find(x => x.TagColourID == tagcolor.TagColourID));
                                    }

                                    int availableTagCount = availableTagColours.Count;
                                    int randomIndex = RANDOM_NUMBER.Next(0, availableTagCount - 1);
                                    var newTagColour = availableTagColours.Skip(randomIndex).Take(1).Single();

                                    //### is new rcvd qty less than -OR- equal to available wt qty for the pallet
                                    if (iQty <= emptyPallet.PalletAvailableWt)
                                    {
                                        itm.PurchaseRcvdStoreds.Add(new PurchaseRcvdStored { Qty = iQty, PalletID = emptyPallet.PalletID, TagColourID = newTagColour.TagColourID, RawMaterialID = item.RawMatarialID, RcvdDate = DateTime.Now });

                                        //////add to inventory
                                        ////var rmInv = new RawMaterialInventoryMaster() { PalletID = emptyPallet.PalletID, RawMaterialID = item.RawMatarialID, Qty = iQty, TagColourID = newTagColour.TagColourID, AddedOn = DateTime.Now };
                                        ////db.RawMaterialInventoryMasters.Add(rmInv);
                                        ////db.SaveChanges();

                                        //add to inventory (as individual bags of RM_BAG_CAPACITY capacity)
                                        for (int i = 1; i <= (iQty / RM_BAG_CAPACITY); i++)
                                        {
                                            var rmInv = new RawMaterialInventoryMaster() { PalletID = emptyPallet.PalletID, RawMaterialID = item.RawMatarialID, Qty = RM_BAG_CAPACITY, TagColourID = newTagColour.TagColourID, AddedOn = DateTime.Now };
                                            db.RawMaterialInventoryMasters.Add(rmInv);
                                        }
                                        db.SaveChanges();

                                        iQty = 0;
                                    }
                                    else
                                    {
                                        itm.PurchaseRcvdStoreds.Add(new PurchaseRcvdStored { Qty = emptyPallet.PalletAvailableWt, PalletID = emptyPallet.PalletID, TagColourID = newTagColour.TagColourID, RawMaterialID = item.RawMatarialID, RcvdDate = DateTime.Now });

                                        ////add to inventory
                                        ////var rmInv = new RawMaterialInventoryMaster() { PalletID = emptyPallet.PalletID, RawMaterialID = item.RawMatarialID, Qty = emptyPallet.PalletAvailableWt, TagColourID = newTagColour.TagColourID, AddedOn = DateTime.Now };
                                        ////db.RawMaterialInventoryMasters.Add(rmInv);
                                        ////db.SaveChanges();

                                        //add to inventory (as individual bags of RM_BAG_CAPACITY capacity)
                                        for (int i = 1; i <= (emptyPallet.PalletAvailableWt / RM_BAG_CAPACITY); i++)
                                        {
                                            var rmInv = new RawMaterialInventoryMaster() { PalletID = emptyPallet.PalletID, RawMaterialID = item.RawMatarialID, Qty = RM_BAG_CAPACITY, TagColourID = newTagColour.TagColourID, AddedOn = DateTime.Now };
                                            db.RawMaterialInventoryMasters.Add(rmInv);
                                        }
                                        db.SaveChanges();

                                        iQty = iQty - emptyPallet.PalletAvailableWt;
                                    }
                                }

                                o.PurchaseRcvdDetails.Add(itm);

                                var purchaseDetails = db.PurchaseDetails.SingleOrDefault(x => x.PurchaseDetailsID == item.PurchaseDetailsID);
                                purchaseDetails.RcvdQty += item.QtyRcvdNow;
                                db.Entry(purchaseDetails).State = System.Data.Entity.EntityState.Modified;
                            }
                        }

                        db.PurchaseRcvdMasters.Add(o);
                        db.SaveChanges();

                        //### check if purchase closed or partially received
                        bool isPurchaseClosed = true;
                        var allRcvd = db.PurchaseRcvdDetails.Where(x => x.PurchaseRcvdMaster.PurchaseID == o.PurchaseID).ToList();
                        var po = db.PurchaseMasters.SingleOrDefault(x => x.PurchaseID == o.PurchaseID);

                        foreach (var item in po.PurchaseDetails)
                        {
                            if (item.Qty != allRcvd.Where(x => x.PurchaseDetailsID == item.PurchaseDetailsID).Sum(x => x.Qty))
                            {
                                isPurchaseClosed = false;
                                break;
                            }
                        }

                        if (isPurchaseClosed)
                            po.PurchaseStatusID = 30;   //Full Rcvd / Closed
                        else
                            po.PurchaseStatusID = 20;   //Part Rcvd

                        db.Entry(po).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        returnValue.data = data;
                        returnValue.data.PurchaseID = o.PurchaseID;

                        //var po = db.PurchaseMasters.Where(x => x.PurchaseID == o.PurchaseID).Select(x => new { x.PurchaseID, x.CompanyLocationMaster.ContactPerson, x.CompanyLocationMaster.Email, x.UserMaster.Username, UserEmail = x.UserMaster.Email }).FirstOrDefault();

                        //Dictionary<string, string> key_vals = new Dictionary<string, string>();
                        //key_vals.Add("%NAME%", po.ContactPerson);
                        //key_vals.Add("%PO_NO%", po.PurchaseID.ToString());
                        //key_vals.Add("%USER%", po.Username);
                        //key_vals.Add("%USER_EMAIL%", po.UserEmail);

                        //SendEmail(po.ContactPerson, po.Email, po.PurchaseID, key_vals);

                        transaction.Commit();

                        returnValue.Response.IsSuccessful();

                        #region ### old code for 1 bag per rack ##########
                        //var o = new PurchaseRcvdMaster();
                        //o.Notes = data.Notes + "";
                        //o.PurchaseID = data.PurchaseID;
                        //o.RcvdByUserID = data.RcvdByUserID;
                        //o.RcvdDate = DateTime.Today;
                        //o.PurchaseRcvdDetails = new List<PurchaseRcvdDetail>();

                        //foreach (var item in data.LineItems)
                        //{
                        //    if (item.QtyRcvdNow > 0)
                        //    {
                        //        var itm = new PurchaseRcvdDetail { PurchaseDetailsID = item.PurchaseDetailsID, Qty = item.QtyRcvdNow };
                        //        itm.PurchaseRcvdStoreds = new List<PurchaseRcvdStored>();

                        //        int iPalletCapacity = PALLET_CAPACITY;
                        //        int iQty = item.QtyRcvdNow;
                        //        while (iQty > 0)
                        //        {
                        //            if (iQty <= iPalletCapacity)
                        //            {
                        //                var emptyPallet = db.sp_GetEmptyPallets().ElementAtOrDefault(0).Value;

                        //                //itm.PurchaseRcvdStoreds.Add(new PurchaseRcvdStored { Qty = iQty, RackID = racks.FirstOrDefault(x => x.RMQty == 0 && x.ProdQty == 0).RackID });
                        //                itm.PurchaseRcvdStoreds.Add(new PurchaseRcvdStored { Qty = iQty, PalletID = emptyPallet });

                        //                //add to inventory
                        //                var rmInv = new RawMaterialInventoryMaster() { PalletID = emptyPallet, RawMaterialID = item.RawMatarialID, Qty = iQty, AddedOn = DateTime.Now };
                        //                db.RawMaterialInventoryMasters.Add(rmInv);
                        //                db.SaveChanges();

                        //                iQty = 0;

                        //            }
                        //            else
                        //            {
                        //                var emptyPallet = db.sp_GetEmptyPallets().ElementAtOrDefault(0).Value;

                        //                //itm.PurchaseRcvdStoreds.Add(new PurchaseRcvdStored { Qty = iRackQty, RackID = racks.FirstOrDefault(x => x.RMQty == 0 && x.ProdQty == 0).RackID });
                        //                itm.PurchaseRcvdStoreds.Add(new PurchaseRcvdStored { Qty = iPalletCapacity, PalletID = emptyPallet });

                        //                //add to inventory
                        //                var rmInv = new RawMaterialInventoryMaster() { PalletID = emptyPallet, RawMaterialID = item.RawMatarialID, Qty = iPalletCapacity, AddedOn = DateTime.Now };
                        //                db.RawMaterialInventoryMasters.Add(rmInv);
                        //                db.SaveChanges();

                        //                iQty = iQty - iPalletCapacity;
                        //            }
                        //        }

                        //        o.PurchaseRcvdDetails.Add(itm);

                        //        var purchaseDetails = db.PurchaseDetails.SingleOrDefault(x => x.PurchaseDetailsID == item.PurchaseDetailsID);
                        //        purchaseDetails.RcvdQty += item.QtyRcvdNow;
                        //        db.Entry(purchaseDetails).State = System.Data.Entity.EntityState.Modified;
                        //    }
                        //}

                        //db.PurchaseRcvdMasters.Add(o);
                        //db.SaveChanges();

                        ////check if purchse closed or partially received
                        //bool isPurchaseClosed = true;
                        //var allRcvd = db.PurchaseRcvdDetails.Where(x => x.PurchaseRcvdMaster.PurchaseID == o.PurchaseID).ToList();
                        //var po = db.PurchaseMasters.SingleOrDefault(x => x.PurchaseID == o.PurchaseID);

                        //foreach (var item in po.PurchaseDetails)
                        //{
                        //    if (item.Qty != allRcvd.Where(x => x.PurchaseDetailsID == item.PurchaseDetailsID).Sum(x => x.Qty))
                        //    {
                        //        isPurchaseClosed = false;
                        //        break;
                        //    }
                        //}

                        //if (isPurchaseClosed)
                        //    po.PurchaseStatusID = 30;   //Full Rcvd / Closed
                        //else
                        //    po.PurchaseStatusID = 20;   //Part Rcvd

                        //db.Entry(po).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();

                        //returnValue.data = data;
                        //returnValue.data.PurchaseID = o.PurchaseID;

                        ////var po = db.PurchaseMasters.Where(x => x.PurchaseID == o.PurchaseID).Select(x => new { x.PurchaseID, x.CompanyLocationMaster.ContactPerson, x.CompanyLocationMaster.Email, x.UserMaster.Username, UserEmail = x.UserMaster.Email }).FirstOrDefault();

                        ////Dictionary<string, string> key_vals = new Dictionary<string, string>();
                        ////key_vals.Add("%NAME%", po.ContactPerson);
                        ////key_vals.Add("%PO_NO%", po.PurchaseID.ToString());
                        ////key_vals.Add("%USER%", po.Username);
                        ////key_vals.Add("%USER_EMAIL%", po.UserEmail);

                        ////SendEmail(po.ContactPerson, po.Email, po.PurchaseID, key_vals);

                        //transaction.Commit();

                        //returnValue.Response.IsSuccessful();

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        //TODO error handling
                        returnValue.Response.ResponseMsg = ex.Message;
                    }
                }
            }

            return Json(returnValue);
        }

        public IHttpActionResult GetRMStackingPlan(int id)
        {
            var allUsers = db.UserMasters.ToList();

            var returnValue = new PurchaseRcvPrintResponse();

            try
            {
                var rcvMast = db.PurchaseRcvdMasters.SingleOrDefault(x => x.PurchaseRcvdID == id);
                if (rcvMast != null)
                {
                    returnValue.data = new PurchaseRcvPrint
                    {
                        CompanyLocation = rcvMast.PurchaseMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + rcvMast.PurchaseMaster.CompanyLocationMaster.LocationName + "]",
                        Instructions = rcvMast.PurchaseMaster.Instructions,
                        PurchaseDate = rcvMast.PurchaseMaster.PurchaseDate,
                        PurchaseID = rcvMast.PurchaseMaster.PurchaseID,
                        Status = rcvMast.PurchaseMaster.PurchaseStatusMaster.PurchaseStatus,
                        RcvdByUserID = 1001,
                        ReceivedByUser = allUsers.SingleOrDefault(x => x.UserID == 1001).Username,
                        PurchaseRcvdID = rcvMast.PurchaseRcvdID,
                        RcvdDate = rcvMast.RcvdDate,
                        User = rcvMast.UserMaster.Username,
                        LineItems = new List<PurchaseRcvDet>(),
                        StoredItems = new List<PurchaseRcvStor>()
                    };

                    var allRcvd = db.PurchaseRcvdDetails.Where(x => x.PurchaseRcvdMaster.PurchaseID == rcvMast.PurchaseID).ToList();
                    foreach (var item in rcvMast.PurchaseMaster.PurchaseDetails)
                    {
                        var itm = new KPILib.Models.PurchaseRcvDet();
                        itm.PurchaseDetailsID = item.PurchaseDetailsID;
                        itm.RawMatarialID = item.RawMatarialID;
                        itm.RawMatarialName = item.RawMaterialMaster.RawMaterialName;
                        itm.Qty = item.Qty;
                        itm.QtyRcvdSoFar = allRcvd.Where(x => x.PurchaseDetailsID == item.PurchaseDetailsID).Sum(x => x.Qty);
                        itm.QtyBalance = itm.Qty - itm.QtyRcvdSoFar;

                        returnValue.data.LineItems.Add(itm);
                    }

                    //var stackPlan = db.PurchaseRcvdStoreds.Where(x => rcvMast.PurchaseRcvdDetails.Select(y=> y.PurchaseRcvdDetailsID).Contains(x.PurchaseRcvdDetailsID)).ToList();

                    foreach (var rcvDetail in rcvMast.PurchaseRcvdDetails)
                    {
                        foreach (var item in rcvDetail.PurchaseRcvdStoreds)
                        {
                            var itm = new KPILib.Models.PurchaseRcvStor();
                            itm.PurchaseRcvdDetailsID = item.PurchaseRcvdDetailsID;
                            itm.PurRcvdStoredID = item.PurRcvdStoredID;
                            itm.PalletID = item.PalletID;
                            itm.PalletNo = item.PalletMaster.PalletNo;
                            itm.RMID = item.RawMaterialID.Value;
                            itm.RMName = item.RawMaterialMaster.RawMaterialName;
                            itm.TagColourID = item.TagColourID;
                            itm.TagColour = item.TagColourMaster.TagColour;
                            itm.Qty = item.Qty;
                            itm.QtyBags = item.Qty / RM_BAG_CAPACITY;

                            returnValue.data.StoredItems.Add(itm);
                        }
                    }

                    returnValue.Response.IsSuccessful();
                }
                else
                {
                    returnValue.Response.ResponseMsg = "Purchase Receive ID " + id.ToString() + " does not exist.";
                }
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }


        //public IHttpActionResult Edit(KPILib.Models.RawMaterial data)
        //{
        //    var returnValue = new RawMaterialResponse();

        //    try
        //    {
        //        var o = db.RawMaterialMasters.SingleOrDefault(x => x.RawMaterialID == data.RawMaterialID);
        //        if (o != null)
        //        {
        //            o.RawMaterialName = data.RawMaterialName;
        //            o.Description = data.Description;
        //            o.UOMID = data.UOMID;
        //            o.IsDiscontinued = data.IsDiscontinued;
        //            o.LastModifiedOn = DateTime.Now;

        //            db.Entry(o).State = System.Data.Entity.EntityState.Modified;

        //            db.SaveChanges();
        //        }

        //        returnValue.data = data;
        //        returnValue.Response.IsSuccessful();
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO error handling
        //        returnValue.Response.ResponseMsg = ex.Message;
        //    }

        //    return Json(returnValue);
        //}

        //public IHttpActionResult Delete(KPILib.Models.RawMaterial data)
        //{
        //    var returnValue = new RawMaterialResponse();

        //    try
        //    {
        //        var o = db.RawMaterialMasters.SingleOrDefault(x => x.RawMaterialID == data.RawMaterialID);
        //        if (o != null)
        //        {
        //            o.IsDiscontinued = true;

        //            db.Entry(o).State = System.Data.Entity.EntityState.Modified;

        //            db.SaveChanges();
        //        }

        //        returnValue.data = null;
        //        returnValue.Response.IsSuccessful();
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO error handling
        //        returnValue.Response.ResponseMsg = ex.Message;
        //    }

        //    return Json(returnValue);
        //}

        //public IHttpActionResult Delete(int id)
        //{
        //    var returnValue = new RawMaterialResponse();

        //    try
        //    {
        //        var o = db.RawMaterialMasters.SingleOrDefault(x => x.RawMaterialID == id);
        //        if (o != null)
        //        {
        //            o.IsDiscontinued = true;

        //            db.Entry(o).State = System.Data.Entity.EntityState.Modified;

        //            db.SaveChanges();
        //        }

        //        returnValue.data = null;
        //        returnValue.Response.IsSuccessful();
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO error handling
        //        returnValue.Response.ResponseMsg = ex.Message;
        //    }

        //    return Json(returnValue);
        //}

        #endregion
    }
}

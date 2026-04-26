using KPILib;
using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_OpenSales")]
    public class DispatchAPIController : CCSPLBaseAPIController
    {
        public IHttpActionResult GetAll(int id)
        {
            var returnValue = new SalesDispatchMasterResponse();

            try
            {
                var data = db.SalesDetails.Where(x => x.SalesStatusID < (int)enumSalesStatus.Completed_Closed && x.QtyBal > 0).OrderByDescending(x => x.SalesID);

                if (id == 1)
                {
                    data = db.SalesDetails.Where(x => x.SalesStatusID == (int)enumSalesStatus.Completed_Closed).OrderByDescending(x => x.SalesID);
                }

                foreach (var obj in data)
                {
                    SalesDispatchMaster sdm = new SalesDispatchMaster
                    {
                        SalesID = obj.SalesID,
                        SalesDate = obj.SalesMaster.SalesDate,
                        UserID = obj.SalesMaster.UserID,
                        User = obj.SalesMaster.UserMaster.Username,
                        CompanyLocationID = obj.SalesMaster.CompanyLocationID,
                        //CompanyLocation = obj.SalesMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + obj.SalesMaster.CompanyLocationMaster.LocationName + "]",
                        Instructions = obj.SalesMaster.Instructions,
                        SalesStatusID = obj.SalesStatusID,
                        Status = obj.SalesMaster.SalesStatusMaster.SalesStatus,
                        Item = new SalesDispatchItem
                        {
                            SalesDetailsID = obj.SalesDetailsID,
                            SalesID = obj.SalesID,
                            ProductID = obj.ProductID,
                            ProductName = obj.ProductMaster.ProductName,
                            Instructions = obj.Instructions,
                            QtyBooked = obj.Qty,
                            QtyBlocked = obj.QtyBlocked,
                            QtyToDispatch = obj.QtyToDispatch,
                            QtyDispatched = obj.SalesDispatchDetails.Sum(x => x.DispatchQty),      //replace with dispatched qty
                            QtyBal = obj.QtyBal,
                            //QtyAvailable = (int)obj.ProductMaster.ProductInventoryMasters.Sum(x => x.Qty) - db.SalesDetails.Where(x => x.ProductID == obj.ProductID).Sum(x => x.QtyBlocked + x.QtyToDispatch),
                            QtyAvailable = (int)obj.ProductMaster.ProdReadyStoreds.Sum(x => x.Qty) - db.SalesDetails.Where(x => x.ProductID == obj.ProductID).Sum(x => x.QtyBlocked + x.QtyToDispatch),
                            QtyToProduce = obj.ProductionPrograms.Where(x => x.ProductID == obj.ProductID && x.SalesDetailsID == obj.SalesDetailsID && x.ProductQtyCompleted == 0 && x.InProductionQty == 0).Sum(x => x.ProductQty),
                            QtyInProduction = obj.ProductionPrograms.Where(x => x.ProductID == obj.ProductID && x.SalesDetailsID == obj.SalesDetailsID).Sum(x => x.InProductionQty),
                        },
                        //LineItems = new List<SalesDispatchItem>()
                    };

                    //foreach (var itm in obj.SalesMaster.SalesDetails.Where(x => x.SalesStatusID != obj.SalesStatusID))
                    //{
                    //    sdm.LineItems.Add(new SalesDispatchItem { ProductID = itm.ProductID, ProductName = itm.ProductMaster.ProductName, Instructions = itm.Instructions, QtyBooked = itm.Qty, QtyBlocked = obj.QtyBlocked, QtyToDispatch = obj.QtyToDispatch, QtyDispatched = 0, QtyBal = itm.QtyBal, QtyAvailable = (int)itm.ProductMaster.ProductInventoryMasters.Sum(x => x.Qty), QtyInProduction = 0 });
                    //}

                    returnValue.data.Add(sdm);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }

        public IHttpActionResult GetOpenSalesOrderDispatchSummary()
        {
            var returnValue = new SalesOrderDispatchSummaryResponse();

            try
            {
                var readySalesDetails = db.SalesDetails
                    .Include(x => x.SalesMaster.UserMaster)
                    .Include(x => x.ProductMaster)
                    .Where(x => x.IsActive
                        && x.QtyBal > 0
                        && x.SalesMaster.SalesStatusID < (int)enumSalesStatus.Completed_Closed
                        && x.QtyProduced > x.QtyDispatched)
                    .OrderByDescending(x => x.SalesID)
                    .ThenBy(x => x.SalesDetailsID)
                    .ToList();

                var salesOrders = readySalesDetails
                    .GroupBy(x => x.SalesID)
                    .OrderByDescending(x => x.Key)
                    .ToList();

                var locationIds = readySalesDetails
                    .Select(x => x.SalesMaster.CompanyLocationID)
                    .Distinct()
                    .ToList();
                var companyLocations = db.CompanyLocationMasters
                    .Include(x => x.CompanyMaster)
                    .Where(x => locationIds.Contains(x.CompanyLocationID))
                    .ToList()
                    .ToDictionary(x => x.CompanyLocationID, x => x);
                var dispatchStatusMap = GetDispatchStatusDisplayMap(readySalesDetails.Select(x => x.SalesDetailsID).ToList());

                foreach (var salesOrderGroup in salesOrders)
                {
                    var salesOrder = salesOrderGroup.First().SalesMaster;
                    var openProducts = salesOrderGroup
                        .OrderBy(x => x.SalesDetailsID)
                        .ToList();
                    companyLocations.TryGetValue(salesOrder.CompanyLocationID, out var companyLocation);

                    var summary = new SalesOrderDispatchSummary
                    {
                        SalesDetailsID = openProducts.FirstOrDefault()?.SalesDetailsID ?? 0,
                        SalesID = salesOrder.SalesID,
                        SalesDate = salesOrder.SalesDate,
                        CustomerName = companyLocation != null && companyLocation.CompanyMaster != null ? companyLocation.CompanyMaster.CompanyName : string.Empty,
                        LocationName = companyLocation != null ? companyLocation.LocationName : string.Empty,
                        CommittedDate = salesOrder.CommittedDate,
                        SalesExecutive = salesOrder.UserMaster != null ? salesOrder.UserMaster.Username : string.Empty,
                        ProductCount = openProducts.Count
                    };

                    foreach (var product in openProducts)
                    {
                        var readyQty = GetReadyQty(product);
                        summary.Products.Add(new SalesOrderDispatchProduct
                        {
                            SalesDetailsID = product.SalesDetailsID,
                            ProductID = product.ProductID,
                            ProductName = product.ProductMaster != null ? product.ProductMaster.ProductName : string.Empty,
                            Packaging = product.Package,
                            Quantity = product.QtyBal,
                            ReadyQty = readyQty,
                            Color = product.Color,
                            Gms = product.Gms,
                            Instructions = product.Instructions,
                            DispatchStatus = dispatchStatusMap.ContainsKey(product.SalesDetailsID) ? dispatchStatusMap[product.SalesDetailsID] : string.Empty,
                            CanDispatch = readyQty > 0
                        });
                    }

                    if (summary.ProductCount == 1)
                    {
                        var product = summary.Products.First();
                        summary.ProductDisplay = product.ProductName;
                        summary.PackagingDisplay = product.Packaging;
                    }
                    else
                    {
                        summary.ProductDisplay = summary.ProductCount + " Products";
                        summary.PackagingDisplay = string.Join(", ", summary.Products
                            .Select(x => x.Packaging)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct());
                    }

                    returnValue.data.Add(summary);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                returnValue.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }

        [HttpGet]
        public IHttpActionResult GetDispatchedHistory(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var returnValue = new SalesDispatchHistoryResponse();

            try
            {
                var dispatchDetailsQuery = db.SalesDispatchDetails
                    .Include(x => x.SalesDetail.SalesMaster.UserMaster)
                    .Include(x => x.SalesDetail.ProductMaster)
                    .AsQueryable();

                if (fromDate.HasValue)
                {
                    var from = fromDate.Value.Date;
                    dispatchDetailsQuery = dispatchDetailsQuery.Where(x => DbFunctions.TruncateTime(x.DispatchDate) >= from);
                }

                if (toDate.HasValue)
                {
                    var to = toDate.Value.Date;
                    dispatchDetailsQuery = dispatchDetailsQuery.Where(x => DbFunctions.TruncateTime(x.DispatchDate) <= to);
                }

                var dispatchDetails = dispatchDetailsQuery
                    .OrderByDescending(x => x.DispatchDate)
                    .ToList();

                var locationIds = dispatchDetails.Select(x => x.SalesDetail.SalesMaster.CompanyLocationID).Distinct().ToList();
                var companyLocations = db.CompanyLocationMasters
                    .Include(x => x.CompanyMaster)
                    .Where(x => locationIds.Contains(x.CompanyLocationID))
                    .ToList()
                    .ToDictionary(x => x.CompanyLocationID, x => x);
                var salesDetailIds = dispatchDetails.Select(x => x.SalesDetailsID).Distinct().ToList();
                var transporterDetailsMap = GetLatestTransporterDetailMap(salesDetailIds);
                var dispatchStatusMap = GetDispatchStatusDisplayMap(salesDetailIds);

                foreach (var dispatch in dispatchDetails)
                {
                    transporterDetailsMap.TryGetValue(dispatch.SalesDetailsID, out var transporterDetail);
                    companyLocations.TryGetValue(dispatch.SalesDetail.SalesMaster.CompanyLocationID, out var companyLocation);

                    returnValue.data.Add(new SalesDispatchHistoryItem
                    {
                        SalesDispatchID = dispatch.SalesDispatchID,
                        SalesID = dispatch.SalesDetail.SalesID,
                        SalesDetailsID = dispatch.SalesDetailsID,
                        SalesDate = dispatch.SalesDetail.SalesMaster.SalesDate,
                        CustomerName = companyLocation != null && companyLocation.CompanyMaster != null ? companyLocation.CompanyMaster.CompanyName : string.Empty,
                        LocationName = companyLocation != null ? companyLocation.LocationName : string.Empty,
                        ProductName = dispatch.SalesDetail.ProductMaster != null ? dispatch.SalesDetail.ProductMaster.ProductName : string.Empty,
                        Packaging = dispatch.SalesDetail.Package,
                        DispatchQty = dispatch.DispatchQty,
                        DispatchDate = transporterDetail != null && transporterDetail.DispatchDate.HasValue ? transporterDetail.DispatchDate : dispatch.DispatchDate,
                        DispatchStatus = dispatchStatusMap.ContainsKey(dispatch.SalesDetailsID) ? dispatchStatusMap[dispatch.SalesDetailsID] : string.Empty,
                        Transporter = transporterDetail != null ? transporterDetail.Transporter : string.Empty,
                        DocketNo = transporterDetail != null ? transporterDetail.DocketNo : string.Empty,
                        BillNo = transporterDetail != null ? transporterDetail.BillNo : string.Empty,
                        PackedBy = transporterDetail != null ? transporterDetail.PackedBy : string.Empty
                    });
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                returnValue.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }

        public IHttpActionResult SetNewDispatchBlockValues(Dictionary<string, int> parameters) //(int iSalesDetailsID, int iBlockedQty, int iToDispatchQty)
        {
            int iSalesDetailsID, iBlockedQty, iToDispatchQty, iToProduceQty, userId;
            iSalesDetailsID = iBlockedQty = iToDispatchQty = iToProduceQty = userId = 0;

            Int32.TryParse(parameters["iSalesDetailsID"].ToString(), out iSalesDetailsID);
            Int32.TryParse(parameters["iBlockedQty"].ToString(), out iBlockedQty);
            Int32.TryParse(parameters["iToDispatchQty"].ToString(), out iToDispatchQty);
            Int32.TryParse(parameters["iToProduceQty"].ToString(), out iToProduceQty);
            Int32.TryParse(parameters["UserID"].ToString(), out userId);

            var returnValue = new ResponseObj();

            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var item = db.SalesDetails.SingleOrDefault(x => x.SalesDetailsID == iSalesDetailsID);
                    if (item == null)
                    {
                        returnValue.ResponseMsg = "Sales detail not found.";
                        return Json(returnValue);
                    }

                    item.QtyToDispatch = iToDispatchQty;
                    item.QtyBlocked = iBlockedQty;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;

                    SyncPendingProductionProgram(item, iToProduceQty, userId);

                    db.SaveChanges();
                    transaction.Commit();
                }

                returnValue.IsSuccessful();

                //return GetAll();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }

        public IHttpActionResult GetSalesDispatchDetailData(int salesId, int? salesDetailsId = null)
        {
            var returnValue = new SalesDispatchTransporterDetailsResponse();
            try
            {
                var detailId = salesDetailsId.GetValueOrDefault();
                var data = detailId > 0
                    ? db.SalesDispatchTransporterDetails.Where(x => x.SalesDetailsID == detailId).OrderByDescending(x => x.DispatchDate).FirstOrDefault()
                    : null;
                if (data != null)
                {
                    returnValue.transporterDetailsMasterObj = mapper.Map<SalesDispatchTransporterDetailsMaster>(data);
                }

                var salesMasterData = db.SalesMasters.SingleOrDefault(x => x.SalesID == salesId);
                if (salesMasterData != null)
                {
                    returnValue.transporterDetailsMasterObj.SalesMasterObj = mapper.Map<KPILib.Models.SalesMaster>(salesMasterData);

                    if (returnValue.transporterDetailsMasterObj.SalesMasterObj.CompanyLocationID != 0)
                    {
                        CompanyMaster companyMaster = CommonFunctions.GetCompanyMasterById(returnValue.transporterDetailsMasterObj.SalesMasterObj.CompanyLocationID);

                        if (companyMaster != null)
                        {
                            returnValue.transporterDetailsMasterObj.SalesMasterObj.CompanyLocation = companyMaster.CompanyName;
                        }
                    }

                    var salesDetailsData = db.SalesDetails.Where(x => x.SalesID == salesId);
                    if (detailId > 0)
                    {
                        salesDetailsData = salesDetailsData.Where(x => x.SalesDetailsID == detailId);
                    }

                    returnValue.transporterDetailsMasterObj.SalesDetailListObj = salesDetailsData
                        .ToList()
                        .Select(x =>
                        {
                            var salesDetail = mapper.Map<SalesDetails>(x);
                            salesDetail.ProductName = x.ProductMaster != null ? x.ProductMaster.ProductName : string.Empty;
                            salesDetail.ReadyQty = GetReadyQty(x);
                            salesDetail.SalesStatusName = x.SalesStatusID > 0 ? x.SalesMaster.SalesStatusMaster.SalesStatus : string.Empty;
                            return salesDetail;
                        })
                        .ToList();
                    if (returnValue.transporterDetailsMasterObj.SalesDetailsID == 0 && detailId > 0)
                    {
                        returnValue.transporterDetailsMasterObj.SalesDetailsID = detailId;
                    }
                    if (detailId > 0)
                    {
                        var selectedLine = returnValue.transporterDetailsMasterObj.SalesDetailListObj
                            .FirstOrDefault(x => x.SalesDetailsID == detailId);
                        if (selectedLine != null)
                        {
                            returnValue.transporterDetailsMasterObj.DispatchQty = selectedLine.QtyToDispatch > 0
                                ? selectedLine.QtyToDispatch
                                : selectedLine.ReadyQty;
                        }
                    }
                }

                returnValue.responseObj.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.responseObj.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(returnValue);
        }

        [HttpPost]
        public IHttpActionResult SaveSalesDispatchDetailData(SalesDispatchTransporterDetailsMaster salesDispatchDetailMaster)
        {
            var returnValue = new SalesDispatchTransporterDetailsResponse();

            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    if (salesDispatchDetailMaster.SalesDetailsID <= 0)
                    {
                        throw new InvalidOperationException("Please select a product line to dispatch.");
                    }

                    var selectedSalesDetail = db.SalesDetails
                        .Include(x => x.ProductMaster.ProdReadyStoreds)
                        .SingleOrDefault(x => x.SalesDetailsID == salesDispatchDetailMaster.SalesDetailsID);
                    if (selectedSalesDetail == null)
                    {
                        throw new InvalidOperationException("Sales detail not found.");
                    }

                    var requestedDispatchQty = Math.Max(0, salesDispatchDetailMaster.DispatchQty);
                    if (requestedDispatchQty > 0)
                    {
                        var readyQty = GetReadyQty(selectedSalesDetail);
                        if (requestedDispatchQty > readyQty)
                        {
                            throw new InvalidOperationException("Dispatch quantity cannot be greater than ready quantity.");
                        }

                        if (requestedDispatchQty > selectedSalesDetail.QtyBal)
                        {
                            throw new InvalidOperationException("Dispatch quantity cannot be greater than balance quantity.");
                        }

                        selectedSalesDetail.QtyToDispatch = requestedDispatchQty;
                        db.Entry(selectedSalesDetail).State = EntityState.Modified;
                    }

                    SalesDispatchTransporterDetail salesDispatchDetail =
                        mapper.Map<SalesDispatchTransporterDetailsMaster, SalesDispatchTransporterDetail>(salesDispatchDetailMaster);

                    SalesDispatchTransporterDetail dispatchDetailObj = null;

                    if (salesDispatchDetail.SDTRDId > 0)
                    {
                        dispatchDetailObj = db.SalesDispatchTransporterDetails.SingleOrDefault(x => x.SDTRDId == salesDispatchDetail.SDTRDId);
                    }
                    else if (salesDispatchDetail.SalesDetailsID > 0)
                    {
                        dispatchDetailObj = db.SalesDispatchTransporterDetails.FirstOrDefault(x => x.SalesDetailsID == salesDispatchDetail.SalesDetailsID);
                    }

                    if (dispatchDetailObj != null)
                    {
                        ApplyUpdates(dispatchDetailObj, salesDispatchDetail);
                        dispatchDetailObj.UpdatedOn = DateTime.Now;
                        db.Entry(dispatchDetailObj).State = EntityState.Modified;
                    }
                    else
                    {
                        salesDispatchDetail.CreatedOn = DateTime.Now;
                        dispatchDetailObj = salesDispatchDetail;
                        db.SalesDispatchTransporterDetails.Add(dispatchDetailObj);
                    }

                    db.SaveChanges();

                    if (IsDispatchCompleted(dispatchDetailObj.DispatchStatus))
                    {
                        CompleteDispatch(dispatchDetailObj);
                    }

                    transaction.Commit();
                    returnValue.responseObj.IsSuccessful();
                }
            }
            catch (Exception ex)
            {
                returnValue.responseObj.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }

        //private void SubmitSalesDispatchRacks(int salesDetailsId, List<int> rackIds, int userId)
        //{
        //    var existingRacks = db.SalesDispatchRacks.Where(x => x.SalesDetailsID == salesDetailsId).ToList();
        //    if (existingRacks.Count > 0)
        //    {
        //        db.SalesDispatchRacks.RemoveRange(existingRacks);
        //        db.SaveChanges();
        //    }
        //    if (rackIds != null && rackIds.Count > 0)
        //    {
        //        foreach (var rackId in rackIds)
        //        {
        //            db.SalesDispatchRacks.Add(new SalesDispatchRack
        //            {
        //                SalesDispatchID = salesDetailsId,
        //                RackID = rackId,
        //                UserID = userId,
        //                CreatedOn = DateTime.Now
        //            });
        //        }
        //        db.SaveChanges();
        //    }
        //}

        /// <summary>
        /// Assigns mapped fields without repeating code
        /// </summary>
        private void ApplyUpdates(SalesDispatchTransporterDetail target, SalesDispatchTransporterDetail source)
        {
            target.DispatchDate = source.DispatchDate;
            target.DispatchStatus = source.DispatchStatus;
            target.DocketNo = source.DocketNo;
            target.DocketPhotoPath = source.DocketPhotoPath;
            target.SmsSentFlag = source.SmsSentFlag;
            target.Transporter = source.Transporter;
            target.TransportationCharge = source.TransportationCharge;
            target.BillNo = source.BillNo;
            target.PackedBy = source.PackedBy;
            target.UpdatedBy = source.UpdatedBy;
            target.SalesDetailsID = source.SalesDetailsID;
        }

        private void CompleteDispatch(SalesDispatchTransporterDetail transporterDetail)
        {
            if (!transporterDetail.SalesDetailsID.HasValue || transporterDetail.SalesDetailsID.Value <= 0)
            {
                throw new InvalidOperationException("Sales detail is required to complete dispatch.");
            }

            var salesDetail = db.SalesDetails
                .Include(x => x.SalesMaster.SalesDetails)
                .Include(x => x.ProductMaster.ProdReadyStoreds.Select(y => y.RackMaster))
                .SingleOrDefault(x => x.SalesDetailsID == transporterDetail.SalesDetailsID.Value);

            if (salesDetail == null)
            {
                throw new InvalidOperationException("Sales detail not found.");
            }

            var dispatchQty = salesDetail.QtyToDispatch;
            if (dispatchQty <= 0)
            {
                throw new InvalidOperationException("No quantity is marked for dispatch for this sales order line.");
            }

            var availableStock = salesDetail.ProductMaster.ProdReadyStoreds.Where(x => x.Qty > 0).Sum(x => x.Qty);
            if (availableStock < dispatchQty)
            {
                throw new InvalidOperationException("Dispatch quantity cannot be completed because produced stock is not sufficient.");
            }

            var remainingQty = dispatchQty;
            var timestamp = transporterDetail.DispatchDate ?? DateTime.Now;
            var stockEntries = salesDetail.ProductMaster.ProdReadyStoreds
                .Where(x => x.Qty > 0)
                .OrderBy(x => x.RcvdDate)
                .ThenBy(x => x.ProdReadyStoredID)
                .ToList();

            foreach (var stockEntry in stockEntries)
            {
                if (remainingQty <= 0)
                {
                    break;
                }

                var qtyToConsume = Math.Min(stockEntry.Qty, remainingQty);
                stockEntry.Qty -= qtyToConsume;
                db.Entry(stockEntry).State = EntityState.Modified;
                remainingQty -= qtyToConsume;
            }

            if (remainingQty > 0)
            {
                throw new InvalidOperationException("Unable to allocate the full dispatch quantity from ready stock.");
            }

            db.SalesDispatchDetails.Add(new SalesDispatchDetail
            {
                SalesDetailsID = salesDetail.SalesDetailsID,
                ProductID = salesDetail.ProductID,
                DispatchQty = dispatchQty,
                UserID = transporterDetail.UpdatedBy ?? transporterDetail.CreatedBy ?? salesDetail.SalesMaster.UserID,
                DispatchDate = timestamp,
                DispatchNotes = GetDispatchStatusDisplay(salesDetail.SalesDetailsID)
            });

            salesDetail.QtyDispatched += dispatchQty;
            salesDetail.QtyBal = Math.Max(0, salesDetail.QtyBal - dispatchQty);
            salesDetail.QtyToDispatch = 0;
            db.Entry(salesDetail).State = EntityState.Modified;

            RecalculateSalesStatuses(salesDetail.SalesMaster);
            db.SaveChanges();
        }

        private void SyncPendingProductionProgram(SalesDetail item, int toProduceQty, int userId)
        {
            var pendingPrograms = db.ProductionPrograms
                .Where(x => x.SalesDetailsID == item.SalesDetailsID && x.ProductQtyCompleted == 0 && x.InProductionQty == 0)
                .OrderBy(x => x.ProductionProgramID)
                .ToList();

            if (toProduceQty <= 0)
            {
                RemovePendingPrograms(pendingPrograms);
                return;
            }

            var machineId = GetAvailableMachineId(item.ProductMaster.MouldID);
            if (!machineId.HasValue)
            {
                throw new InvalidOperationException("No machine is currently available for the selected mould.");
            }

            var timestamp = DateTime.Now;
            var targetProgram = pendingPrograms.FirstOrDefault();
            if (targetProgram == null)
            {
                targetProgram = new ProductionProgram
                {
                    SalesDetailsID = item.SalesDetailsID,
                    ProductID = item.ProductID,
                    MouldID = item.ProductMaster.MouldID,
                    AddedOn = timestamp
                };
                db.ProductionPrograms.Add(targetProgram);
            }

            targetProgram.ProductQty = toProduceQty;
            targetProgram.MachineID = machineId.Value;
            targetProgram.InProductionQty = 0;
            targetProgram.ProductQtyCompleted = 0;
            targetProgram.UserID = userId;
            targetProgram.LastModifiedOn = timestamp;

            db.SaveChanges();
            SyncProductionProgramMappings(targetProgram, item.ProductMaster, toProduceQty);

            if (pendingPrograms.Count > 1)
            {
                RemovePendingPrograms(pendingPrograms.Skip(1).ToList());
            }
        }

        private void SyncProductionProgramMappings(ProductionProgram program, ProductMaster product, int toProduceQty)
        {
            var existingMappings = db.ProductionProgramRMMappings
                .Where(x => x.ProductionProgramID == program.ProductionProgramID)
                .ToList();

            if (existingMappings.Count > 0)
            {
                db.ProductionProgramRMMappings.RemoveRange(existingMappings);
                db.SaveChanges();
            }

            var productRmMappings = db.ProductRawMaterialMappings
                .Where(x => x.ProductID == product.ProductID)
                .ToList();

            foreach (var productRm in productRmMappings)
            {
                db.ProductionProgramRMMappings.Add(new ProductionProgramRMMapping
                {
                    ProductID = productRm.ProductID,
                    RawMaterialID = productRm.RawMaterialID,
                    RMQty = CalculateRequiredRmQty(productRm.RMReqdForUOMQty, product.MinQtyUOM, toProduceQty),
                    ProductionProgramID = program.ProductionProgramID
                });
            }
        }

        private void RemovePendingPrograms(List<ProductionProgram> pendingPrograms)
        {
            if (pendingPrograms == null || pendingPrograms.Count == 0)
            {
                return;
            }

            var pendingProgramIds = pendingPrograms.Select(x => x.ProductionProgramID).ToList();
            var rmMappings = db.ProductionProgramRMMappings.Where(x => pendingProgramIds.Contains(x.ProductionProgramID)).ToList();
            if (rmMappings.Count > 0)
            {
                db.ProductionProgramRMMappings.RemoveRange(rmMappings);
            }

            db.ProductionPrograms.RemoveRange(pendingPrograms);
        }

        private int CalculateRequiredRmQty(decimal rmReqdForUomQty, decimal minQtyUom, int toProduceQty)
        {
            if (minQtyUom <= 0 || toProduceQty <= 0)
            {
                return 0;
            }

            return (int)Math.Ceiling(rmReqdForUomQty * (decimal)toProduceQty / minQtyUom);
        }

        private int? GetAvailableMachineId(int mouldId)
        {
            return db.MachineMouldMappings
                .Where(x => x.MouldID == mouldId && !x.IsDiscontinued && !x.MachineMaster.IsDiscontinued)
                .Select(x => new
                {
                    x.MachineID,
                    LatestHistoryId = x.MachineMaster.MachineHistories.Max(h => (int?)h.MachineHistoryID)
                })
                .Join(
                    db.MachineHistories,
                    machine => machine.LatestHistoryId,
                    history => (int?)history.MachineHistoryID,
                    (machine, history) => new { machine.MachineID, history.MachineStatusID })
                .Where(x => x.MachineStatusID == (int)enumMachineStatus.NotInUse)
                .Select(x => (int?)x.MachineID)
                .FirstOrDefault();
        }

        private bool IsReadyToDispatchLine(SalesDetail salesDetail)
        {
            if (salesDetail == null || !salesDetail.IsActive || salesDetail.QtyBal <= 0)
            {
                return false;
            }

            return GetReadyQty(salesDetail) > 0;
        }

        private int GetReadyQty(SalesDetail salesDetail)
        {
            if (salesDetail == null)
            {
                return 0;
            }

            return Math.Max(0, salesDetail.QtyProduced - salesDetail.QtyDispatched);
        }

        private Dictionary<int, SalesDispatchTransporterDetail> GetLatestTransporterDetailMap(List<int> salesDetailIds)
        {
            if (salesDetailIds == null || salesDetailIds.Count == 0)
            {
                return new Dictionary<int, SalesDispatchTransporterDetail>();
            }

            return db.SalesDispatchTransporterDetails
                .Where(x => x.SalesDetailsID.HasValue && salesDetailIds.Contains(x.SalesDetailsID.Value))
                .ToList()
                .GroupBy(x => x.SalesDetailsID.Value)
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderByDescending(y => y.UpdatedOn ?? y.CreatedOn).FirstOrDefault());
        }

        private Dictionary<int, string> GetDispatchStatusDisplayMap(List<int> salesDetailIds)
        {
            if (salesDetailIds == null || salesDetailIds.Count == 0)
            {
                return new Dictionary<int, string>();
            }

            var latestStatusCodes = db.SalesDispatchTransporterDetails
                .Where(x => x.SalesDetailsID.HasValue
                    && salesDetailIds.Contains(x.SalesDetailsID.Value)
                    && x.DispatchStatus != null)
                .ToList()
                .GroupBy(x => x.SalesDetailsID.Value)
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderByDescending(y => y.UpdatedOn ?? y.CreatedOn)
                        .Select(y => y.DispatchStatus)
                        .FirstOrDefault());

            var statusLookup = db.LookUpMasters
                .Where(x => x.LookUpType == ApplicationConstants.StatusType && x.IsActive)
                .ToList()
                .GroupBy(x => x.LookUpValue)
                .ToDictionary(x => x.Key, x => x.First().LookUpName);

            return latestStatusCodes.ToDictionary(
                x => x.Key,
                x => string.IsNullOrWhiteSpace(x.Value)
                    ? string.Empty
                    : (statusLookup.ContainsKey(x.Value) ? statusLookup[x.Value] : x.Value));
        }

        private string GetDispatchStatusDisplay(int salesDetailsId)
        {
            var dispatchStatusMap = GetDispatchStatusDisplayMap(new List<int> { salesDetailsId });
            return dispatchStatusMap.ContainsKey(salesDetailsId) ? dispatchStatusMap[salesDetailsId] : string.Empty;
        }

        private bool IsDispatchCompleted(string dispatchStatusCode)
        {
            if (string.IsNullOrWhiteSpace(dispatchStatusCode))
            {
                return false;
            }

            if (string.Equals(dispatchStatusCode, ApplicationStatus.Done, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var statusLookup = db.LookUpMasters
                .Where(x => x.LookUpType == ApplicationConstants.StatusType && x.LookUpValue == dispatchStatusCode && x.IsActive)
                .Select(x => x.LookUpName)
                .FirstOrDefault();

            return !string.IsNullOrWhiteSpace(statusLookup)
                && string.Equals(statusLookup.Trim(), ApplicationStatus.Done, StringComparison.OrdinalIgnoreCase);
        }

        private void RecalculateSalesStatuses(SalesMaster salesMaster)
        {
            if (salesMaster == null)
            {
                return;
            }

            foreach (var salesDetail in salesMaster.SalesDetails.Where(x => x.IsActive))
            {
                if (salesDetail.QtyBal <= 0)
                {
                    salesDetail.SalesStatusID = (int)enumSalesStatus.Completed_Closed;
                }
                else if (salesDetail.QtyInProduction > 0 || salesDetail.QtyProduced > 0 || salesDetail.QtyToDispatch > 0 || salesDetail.QtyDispatched > 0)
                {
                    salesDetail.SalesStatusID = (int)enumSalesStatus.In_Production;
                }
                else if (salesDetail.ProductionPrograms.Any(x => x.ProductQty > x.ProductQtyCompleted))
                {
                    salesDetail.SalesStatusID = (int)enumSalesStatus.Awaiting_Production;
                }
                else
                {
                    salesDetail.SalesStatusID = (int)enumSalesStatus.Procure_Material;
                }

                db.Entry(salesDetail).State = EntityState.Modified;
            }

            salesMaster.SalesStatusID = salesMaster.SalesDetails.Where(x => x.IsActive).All(x => x.QtyBal <= 0)
                ? (int)enumSalesStatus.Completed_Closed
                : salesMaster.SalesDetails.Where(x => x.IsActive).Max(x => x.SalesStatusID);

            db.Entry(salesMaster).State = EntityState.Modified;
        }
    }
}

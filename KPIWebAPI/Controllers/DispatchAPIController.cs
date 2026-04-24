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
                var salesOrders = db.SalesMasters
                    .Where(x => x.SalesStatusID < (int)enumSalesStatus.Completed_Closed)
                    .OrderByDescending(x => x.SalesID)
                    .ToList()
                    .Where(x => x.SalesDetails.Any(sd => IsDispatchEligibleLine(sd)))
                    .ToList();

                //var salesOrderData = (from SM in db.SalesMasters
                //                      join SD in db.SalesDetails on SM.SalesID equals SD.SalesID
                //                      join PP in db.ProductionPrograms on SD.SalesDetailsID equals PP.SalesDetailsID
                //                      where PP.ProductQtyCompleted > 0 && SD.SalesStatusID == (int)enumSalesStatus.Completed_Closed


                foreach (var salesOrder in salesOrders)
                {
                    var openProducts = salesOrder.SalesDetails
                        .Where(x => IsDispatchEligibleLine(x))
                        .OrderBy(x => x.SalesDetailsID)
                        .ToList();

                    var companyLocation = db.CompanyLocationMasters
                        .Include(x => x.CompanyMaster)
                        .SingleOrDefault(x => x.CompanyLocationID == salesOrder.CompanyLocationID);

                    var summary = new SalesOrderDispatchSummary
                    {
                        SalesDetailsID = salesOrder.SalesDetails.FirstOrDefault(x => IsDispatchEligibleLine(x))?.SalesDetailsID ?? 0,
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
                        summary.Products.Add(new SalesOrderDispatchProduct
                        {
                            SalesDetailsID = product.SalesDetailsID,
                            ProductID = product.ProductID,
                            ProductName = product.ProductMaster != null ? product.ProductMaster.ProductName : string.Empty,
                            Packaging = product.Package,
                            Quantity = product.QtyBal,
                            ReadyQty = GetReadyQty(product),
                            Color = product.Color,
                            Gms = product.Gms,
                            Instructions = product.Instructions,
                            DispatchStatus = GetDispatchStatusDisplay(product.SalesDetailsID),
                            CanDispatch = GetReadyQty(product) > 0 || product.QtyToDispatch > 0
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

                    var salesDetailsData = db.SalesDetails.Where(x => x.SalesID == salesId).ToList();
                    returnValue.transporterDetailsMasterObj.SalesDetailListObj = mapper.Map<List<SalesDetails>>(salesDetailsData);
                    if (returnValue.transporterDetailsMasterObj.SalesDetailsID == 0 && detailId > 0)
                    {
                        returnValue.transporterDetailsMasterObj.SalesDetailsID = detailId;
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

        private bool IsDispatchEligibleLine(SalesDetail salesDetail)
        {
            if (salesDetail == null || !salesDetail.IsActive || salesDetail.QtyBal <= 0)
            {
                return false;
            }

            return GetReadyQty(salesDetail) > 0
                || salesDetail.QtyToDispatch > 0
                || db.SalesDispatchTransporterDetails.Any(x => x.SalesDetailsID == salesDetail.SalesDetailsID);
        }

        private int GetReadyQty(SalesDetail salesDetail)
        {
            if (salesDetail == null)
            {
                return 0;
            }

            return Math.Max(0, salesDetail.QtyProduced - salesDetail.QtyDispatched);
        }

        private string GetDispatchStatusDisplay(int salesDetailsId)
        {
            var latestStatusCode = db.SalesDispatchTransporterDetails
                .Where(x => x.SalesDetailsID == salesDetailsId && x.DispatchStatus != null)
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => x.DispatchStatus)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(latestStatusCode))
            {
                return string.Empty;
            }

            var statusLookup = db.LookUpMasters
                .Where(x => x.LookUpType == ApplicationConstants.StatusType && x.LookUpValue == latestStatusCode && x.IsActive)
                .Select(x => x.LookUpName)
                .FirstOrDefault();

            return string.IsNullOrWhiteSpace(statusLookup) ? latestStatusCode : statusLookup;
        }

        private bool IsDispatchCompleted(string dispatchStatusCode)
        {
            if (string.IsNullOrWhiteSpace(dispatchStatusCode))
            {
                return false;
            }

            if (string.Equals(dispatchStatusCode, "ST002", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var statusLookup = db.LookUpMasters
                .Where(x => x.LookUpType == ApplicationConstants.StatusType && x.LookUpValue == dispatchStatusCode && x.IsActive)
                .Select(x => x.LookUpName)
                .FirstOrDefault();

            return !string.IsNullOrWhiteSpace(statusLookup)
                && string.Equals(statusLookup.Trim(), "DONE", StringComparison.OrdinalIgnoreCase);
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

using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Production")]
    public class ProductionAPIController : CCSPLBaseAPIController
    {
        static object myLock = new object();

        public IHttpActionResult GetAll(int id)
        {
            var returnValue = new ProductionProgramesResponse();

            try
            {
                var data = db.ProductionPrograms.Where(x => x.ProductQty > x.ProductQtyCompleted).OrderBy(x => x.ProductionProgramID);

                if (id == 1)
                {
                    data = db.ProductionPrograms.Where(x => x.ProductQty <= x.ProductQtyCompleted).OrderBy(x => x.ProductionProgramID);
                }

                foreach (var obj in data)
                {
                    var o = mapper.Map<ProductionProgram, KPILib.Models.ProductionPrograme>(obj);

                    o.SalesID = obj.SalesDetail.SalesID;
                    o.SalesDate = obj.SalesDetail.SalesMaster.SalesDate;
                    o.SalesUserID = obj.SalesDetail.SalesMaster.UserID;
                    o.SalesUser = obj.SalesDetail.SalesMaster.UserMaster.Username;
                    o.CompanyLocationID = obj.SalesDetail.SalesMaster.CompanyLocationID;
                    //o.CompanyLocation = obj.SalesDetail.SalesMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + obj.SalesDetail.SalesMaster.CompanyLocationMaster.LocationName + "]";

                    o.ProductName = obj.ProductMaster.ProductName;

                    //o.RawMaterialName = obj.RawMaterialMaster.RawMaterialName;

                    o.MouldName = obj.MouldMaster.MouldName;

                    o.MachineName = obj.MachineMaster != null ? obj.MachineMaster.MachineName : string.Empty;
                    o.BalanceQty = obj.ProductQty - obj.ProductQtyCompleted;

                    var machines = db.MachineMouldMappings.Where(x => x.MouldID == obj.MouldID).ToList();
                    foreach (var machine in machines)
                    {
                        if (!machine.IsDiscontinued && !machine.MachineMaster.IsDiscontinued)
                        {
                            var machineHistory = machine.MachineMaster.MachineHistories.LastOrDefault();
                            var bAvailability = (machineHistory != null ? machineHistory.MachineStatusID == (int)enumMachineStatus.NotInUse : false);
                            if (bAvailability)
                                o.Machines.Add(new MachineAvailability { MachineID = machine.MachineID, MachineName = machine.MachineMaster.MachineName, MachineAvailable = bAvailability });
                        }
                    }

                    o.ProductionUser = obj.UserMaster.Username;

                    o.MappedRawMaterials = obj.ProductionProgramRMMappings.Select(a =>
                     new MappedRawMaterial
                     {
                         RawMaterialID = a.RawMaterialID,
                         RawMaterialName = a.RawMaterialMaster.RawMaterialName,
                         RMQty = a.RMQty,
                         RMAvailableQty = a.RawMaterialMaster.RawMaterialInventoryMasters.Sum(s => s.Qty)
                     }).ToList();

                    var rmQrtLessThanReq = o.MappedRawMaterials.Where(a => a.IsRMQtyLessThanRequired);

                    if (rmQrtLessThanReq.Count() > 0)
                    {
                        var minRMQty = rmQrtLessThanReq.Min(a => a.RMAvailableQty);
                        var rMId = rmQrtLessThanReq.Where(a => a.RMAvailableQty == minRMQty).FirstOrDefault();
                        if (rMId != null)
                        {
                            var RMReqdForUOMQty = obj.ProductMaster.ProductRawMaterialMappings.FirstOrDefault(x => x.RawMaterialID == rMId.RawMaterialID);
                            if (RMReqdForUOMQty != null)
                            {
                                o.CanProduceQty = (int)Math.Round((minRMQty / RMReqdForUOMQty.RMReqdForUOMQty) * obj.ProductMaster.MinQtyUOM);
                            }
                        }

                    }
                    else
                    {
                        o.CanProduceQty = o.ProductQty;
                    }

                    o.RMAvailable = true;

                    obj.ProductionProgramRMMappings.ForEach(x =>
                    {
                        if (o.RMAvailable)
                        {
                            //if (x.RawMaterialMaster.RawMaterialInventoryMasters.Sum(a => a.Qty) >= x.RMQty)
                            if (x.RawMaterialMaster.RawMaterialInventoryMasters.Sum(a => a.Qty) >= 0)
                            {
                                o.RMAvailable = true;
                            }
                            else
                            {
                                o.RMAvailable = false;
                            }
                        }
                    });

                    // o.RMAvailable = (obj.RawMaterialMaster.RawMaterialInventoryMasters.Sum(x => x.Qty) >= obj.RMQty); -- Need to discuss

                    o.RMQty = obj.ProductionProgramRMMappings.Sum(a => a.RMQty);

                    o.MouldAvailable = (obj.MouldMaster.MouldHistories.LastOrDefault().MouldStatusID == (int)enumMouldStatus.NotInUse);           // ### NotInUse
                    o.MachineAvailable = obj.MachineMaster != null && obj.MachineMaster.MachineHistories.LastOrDefault() != null && obj.MachineMaster.MachineHistories.LastOrDefault().MachineStatusID == (int)enumMachineStatus.NotInUse;   // ### NotInUse

                    if (o.Machines.Count() > 0)
                    {
                        o.MachineAvailable = (o.Machines.Count() > 0);   // ### NotInUse
                        o.MachineNames = string.Join(" | ", o.Machines.Select(x => x.MachineName).ToArray());
                    }
                    if (obj.MachineID.HasValue)
                    {
                        o.MachineID = obj.MachineID.Value;
                        o.MachineName = obj.MachineMaster.MachineName;
                    }

                    o.ProgramInProgress = IsProgramInProgress(obj);

                    returnValue.data.Add(o);
                }

                //show all items ready for production first. where... RM is available, Mould is available. Machine is available. If available, boolean value will be TRUE (1). Hence sort order should be desc.
                returnValue.data = returnValue.data.OrderByDescending(x => x.MachineAvailable && x.MouldAvailable && x.RMAvailable && !x.ProgramInProgress).ToList();

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

        public IHttpActionResult GetProductionBatch(int ProductionProgramID)
        {
            ProductionProgramesResponse returnValue = new ProductionProgramesResponse();

            try
            {
                ProductionProgram productionObj = db.ProductionPrograms.Where(x => x.ProductionProgramID == ProductionProgramID).OrderBy(x => x.ProductionProgramID).FirstOrDefault();

                if (productionObj == null)
                {
                    returnValue.Response.ResponseMsg = "Production program not found.";
                    return Json(returnValue);
                }

                KPILib.Models.ProductionPrograme productionProgrameObj = mapper.Map<ProductionProgram, KPILib.Models.ProductionPrograme>(productionObj);

                productionProgrameObj.SalesID = productionObj.SalesDetail.SalesID;
                productionProgrameObj.SalesDate = productionObj.SalesDetail.SalesMaster.SalesDate;
                productionProgrameObj.SalesUserID = productionObj.SalesDetail.SalesMaster.UserID;
                productionProgrameObj.SalesUser = productionObj.SalesDetail.SalesMaster.UserMaster.Username;
                productionProgrameObj.CompanyLocationID = productionObj.SalesDetail.SalesMaster.CompanyLocationID;
                //productionProgrameObj.CompanyLocation = productionObj.SalesDetail.SalesMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + productionObj.SalesDetail.SalesMaster.CompanyLocationMaster.LocationName + "]";
                productionProgrameObj.ProductName = productionObj.ProductMaster.ProductName;
                productionProgrameObj.MouldName = productionObj.MouldMaster.MouldName;
                productionProgrameObj.InProductionQty = productionObj.InProductionQty;

                var machines = db.MachineMouldMappings.Where(x => x.MouldID == productionObj.MouldID).ToList();

                foreach (var machine in machines)
                {
                    if (!machine.IsDiscontinued && !machine.MachineMaster.IsDiscontinued)
                    {
                        var machineHistory = machine.MachineMaster.MachineHistories.LastOrDefault();
                        var bAvailability = (machineHistory != null ? machineHistory.MachineStatusID == (int)enumMachineStatus.NotInUse : false);
                        if (bAvailability)
                            productionProgrameObj.Machines.Add(new MachineAvailability { MachineID = machine.MachineID, MachineName = machine.MachineMaster.MachineName, MachineAvailable = bAvailability });
                    }
                }
                productionProgrameObj.ProductionUser = productionObj.UserMaster.Username;
                productionProgrameObj.MappedRawMaterials = productionObj.ProductionProgramRMMappings.Select(a =>
                 new MappedRawMaterial
                 {
                     RawMaterialID = a.RawMaterialID,
                     RawMaterialName = a.RawMaterialMaster.RawMaterialName,
                     RMQty = a.RMQty,
                     RMAvailableQty = a.RawMaterialMaster.RawMaterialInventoryMasters.Sum(s => s.Qty)
                 }).ToList();
                var rmQrtLessThanReq = productionProgrameObj.MappedRawMaterials.Where(a => a.IsRMQtyLessThanRequired);
                if (rmQrtLessThanReq.Count() > 0)
                {
                    var minRMQty = rmQrtLessThanReq.Min(a => a.RMAvailableQty);
                    var rMId = rmQrtLessThanReq.Where(a => a.RMAvailableQty == minRMQty).FirstOrDefault();
                    if (rMId != null)
                    {
                        var RMReqdForUOMQty = productionObj.ProductMaster.ProductRawMaterialMappings.FirstOrDefault(x => x.RawMaterialID == rMId.RawMaterialID);
                        if (RMReqdForUOMQty != null)
                        {
                            productionProgrameObj.CanProduceQty = (int)Math.Round((minRMQty / RMReqdForUOMQty.RMReqdForUOMQty) * productionObj.ProductMaster.MinQtyUOM);
                        }
                    }
                }
                else
                {
                    productionProgrameObj.CanProduceQty = productionProgrameObj.ProductQty;
                }
                productionProgrameObj.RMAvailable = true;
                productionObj.ProductionProgramRMMappings.ForEach(x =>
                {
                    if (productionProgrameObj.RMAvailable)
                    {
                        if (x.RawMaterialMaster.RawMaterialInventoryMasters.Sum(a => a.Qty) >= 0)
                        {
                            productionProgrameObj.RMAvailable = true;
                        }
                        else
                        {
                            productionProgrameObj.RMAvailable = false;
                        }
                    }
                });
                productionProgrameObj.RMQty = productionObj.ProductionProgramRMMappings.Sum(a => a.RMQty);
                productionProgrameObj.MouldAvailable = (productionObj.MouldMaster.MouldHistories.LastOrDefault().MouldStatusID == (int)enumMouldStatus.NotInUse);           // ### NotInUse
                if (productionProgrameObj.Machines.Count() > 0)
                {
                    productionProgrameObj.MachineAvailable = (productionProgrameObj.Machines.Count() > 0);   // ### NotInUse
                    productionProgrameObj.MachineNames = string.Join(" | ", productionProgrameObj.Machines.Select(x => x.MachineName).ToArray());
                }
                if (productionObj.MachineID.HasValue)
                {
                    productionProgrameObj.MachineID = productionObj.MachineID.Value;
                    productionProgrameObj.MachineName = productionObj.MachineMaster != null ? productionObj.MachineMaster.MachineName : string.Empty;
                }
                productionProgrameObj.ProgramInProgress = IsProgramInProgress(productionObj);

                List<ProductionProgramBatch> programBatches = productionObj.ProductionProgramBatches.Where(z => z.ProductionProgramID == ProductionProgramID).ToList();

                productionProgrameObj.ProgramBatches = mapper.Map<List<ProductionProgramBatch>, List<KPILib.Models.ProductionBatches>>(programBatches);

                if (productionProgrameObj.ProgramBatches.Count > 0)
                {
                    int balanceQty = productionProgrameObj.ProductQty;
                    //int balanceQtyToProduce = 0;
                    int producedQtyData = productionProgrameObj.ProgramBatches.Select(x => x.ProductQtyCompleted).ToArray().Sum();
                    productionProgrameObj.ProductQtyCompleted = producedQtyData;
                    int inProductionQtyData = productionProgrameObj.ProgramBatches.Select(x => x.InProductionQty).ToArray().Sum();
                    productionProgrameObj.BalanceQty = productionProgrameObj.ProductQty - producedQtyData;
                    //balanceQtyToProduce = productionProgrameObj.ProductQty - (productionProgrameObj.BalanceQty + inProductionQtyData);
                    foreach (var item in productionProgrameObj.ProgramBatches)
                    {
                        item.ProductionProgramID = ProductionProgramID;
                        item.ProductQty = balanceQty;
                        balanceQty = (item.ProductQty - (item.ProductQtyCompleted + item.InProductionQty));
                        item.BalanceQty = balanceQty;
                        item.MouldName = productionProgrameObj.MouldName;
                        item.RMQty = productionProgrameObj.RMQty;
                        item.CanProduceQty = productionProgrameObj.CanProduceQty != item.ProductQty ? item.ProductQty : productionProgrameObj.CanProduceQty;
                        item.MappedRawMaterials = productionProgrameObj.MappedRawMaterials;
                        item.ProgramInProgress = item.InProductionQty > 0 || productionProgrameObj.ProgramInProgress;
                        item.RMAvailable = productionProgrameObj.RMAvailable;
                        item.MouldAvailable = productionProgrameObj.MouldAvailable;
                        item.MachineAvailable = productionProgrameObj.MachineAvailable;
                        item.MachineNames = productionProgrameObj.MachineNames;
                        item.ProductionUser = productionProgrameObj.ProductionUser;
                        item.CompanyLocation = productionProgrameObj.CompanyLocation;
                        item.SalesID = productionProgrameObj.SalesID;
                        item.SalesDate = productionProgrameObj.SalesDate;
                        item.SalesUserID = productionProgrameObj.SalesUserID;
                        item.SalesUser = productionProgrameObj.SalesUser;
                        item.ProductName = productionProgrameObj.ProductName;
                    }

                    if (productionProgrameObj.BalanceQty > 0)
                    {
                        ProductionBatches productionBatchesObj = mapper.Map<ProductionBatches>(productionProgrameObj);

                        productionBatchesObj.ProgramInProgress = false;
                        productionBatchesObj.AddedOn = null;
                        productionBatchesObj.ProductQty = balanceQty;
                        //productionBatchesObj.CanProduceQty = balanceQty;
                        productionBatchesObj.CanProduceQty = productionProgrameObj.CanProduceQty > balanceQty ? balanceQty : productionProgrameObj.CanProduceQty;
                        productionBatchesObj.BalanceQty = productionBatchesObj.ProductQty;
                        productionBatchesObj.ProductQtyCompleted = 0;
                        productionBatchesObj.InProductionQty = 0;
                        productionProgrameObj.ProgramBatches.Add(productionBatchesObj);
                    }
                }
                else
                {
                    productionProgrameObj.BalanceQty = productionProgrameObj.ProductQty;
                    ProductionBatches productionBatchesObj = mapper.Map<ProductionBatches>(productionProgrameObj);

                    productionBatchesObj.ProgramInProgress = false;
                    productionBatchesObj.AddedOn = null;
                    productionProgrameObj.ProgramBatches.Add(productionBatchesObj);
                }

                returnValue.productionPrograme = productionProgrameObj;

                //Show all items ready for production first. where... RM is available, Mould is available. Machine is available.
                //If available, boolean value will be TRUE (1). Hence sort order should be desc.
                returnValue.data = returnValue.data.OrderByDescending(x => x.MachineAvailable && x.MouldAvailable && x.RMAvailable && !x.ProgramInProgress).ToList();
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

        public IHttpActionResult StartProduction(Dictionary<string, int> parameters) //(int iProductionProgramID)
        {
            int iProductionProgramID = 0;
            int iProduceQty = 0;

            Int32.TryParse(parameters["iProductionProgramID"].ToString(), out iProductionProgramID);
            Int32.TryParse(parameters["iToProduceQty"].ToString(), out iProduceQty);

            var returnValue = new ResponseObj();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var productionProgram = db.ProductionPrograms.SingleOrDefault(x => x.ProductionProgramID == iProductionProgramID);
                        //var productionProgramRMMapping = db.ProductionProgramRMMappings.Where(x => x.ProductionProgramID == iProductionProgramID);
                        //var bRMWasAvailable = true;

                        ////var machineIDs = new List<int>();

                        var timestamp = DateTime.Now;

                        //////get machine IDs of all machines compatible with the mould
                        ////var machineIDList = db.MachineMouldMappings.Where(x => x.MouldID == productionProgram.MouldID && !x.IsDiscontinued);
                        ////foreach (var mID in machineIDList)
                        ////    machineIDs.Add(mID.MachineID);

                        //////get all machine history of all compatible machines
                        ////var machines = db.MachineHistories.Where(x => machineIDs.Contains(x.MachineID)).GroupBy(x=> x.MachineID).Select(x=> new { x.Key, LastHistory = x.Max(y=> y.MachineHistoryID), x. ;

                        ////foreach (var machine in machines)
                        ////{
                        ////    if (!machine.IsDiscontinued && !machine.MachineMaster.IsDiscontinued)
                        ////    {
                        ////        var machineHistory = machine.MachineMaster.MachineHistories.LastOrDefault();
                        ////        var bAvailability = (machineHistory != null ? machineHistory.MachineStatusID == 101 : false);
                        ////        if (bAvailability)
                        ////        {
                        ////            machineID = machine.MachineID;
                        ////            break;
                        ////        }
                        ////    }
                        ////}

                        if (productionProgram == null)
                        {
                            returnValue.ResponseMsg = "Production program not found.";
                            return Json(returnValue);
                        }

                        var availableMachine = db.sp_GetAvailableMachineID(productionProgram.MouldID).FirstOrDefault();
                        if (!availableMachine.HasValue)
                        {
                            returnValue.ResponseMsg = "No machine is available to start this production program.";
                            return Json(returnValue);
                        }

                        var salesDetail = productionProgram.SalesDetail;
                        if (salesDetail == null)
                        {
                            returnValue.ResponseMsg = "Sales detail not found for this production program.";
                            return Json(returnValue);
                        }

                        productionProgram.MachineID = availableMachine.Value;
                        productionProgram.InProductionQty += iProduceQty;
                        db.Entry(productionProgram).State = System.Data.Entity.EntityState.Modified;

                        salesDetail.QtyInProduction += iProduceQty;
                        salesDetail.SalesStatusID = (int)enumSalesStatus.In_Production;
                        db.Entry(salesDetail).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        ProductionProgramBatch programBatch = new ProductionProgramBatch()
                        {
                            AddedOn = DateTime.Now,
                            InProductionQty = iProduceQty,
                            ProductionProgramID = productionProgram.ProductionProgramID,
                            UserID = productionProgram.UserID,
                            ProductQtyCompleted = 0
                        };
                        AddUpdateProductionBatch(programBatch);

                        var productRMMapping = productionProgram.ProductMaster.ProductRawMaterialMappings.ToList();

                        foreach (var pp in productRMMapping)
                        {
                            var iRMQty = (int)Math.Ceiling(pp.RMReqdForUOMQty * (decimal)iProduceQty / productionProgram.ProductMaster.MinQtyUOM);
                            var iRMQtyUsed = iRMQty;

                            while (iRMQtyUsed > 0)
                            {
                                var rmInventoryMaster = db.RawMaterialInventoryMasters.FirstOrDefault(x => x.RawMaterialID == pp.RawMaterialID && x.Qty > 0);
                                if (rmInventoryMaster != null)
                                {
                                    if (rmInventoryMaster.Qty > iRMQtyUsed)
                                    {
                                        //RM qty available (in bag) is more than required qty
                                        //add new entry to ProductionRawMaterial
                                        var productionRawMaterial = new ProductionRawMaterial()
                                        {
                                            ProductionProgramID = iProductionProgramID,
                                            RawMaterialID = pp.RawMaterialID,
                                            RMInventoryID = rmInventoryMaster.RMInventoryID,
                                            RMQty = iRMQtyUsed,
                                            AddedOn = timestamp
                                        };

                                        db.ProductionRawMaterials.Add(productionRawMaterial);
                                        //iRMQtyUsed = 0;

                                        //update record/s in RawMaterialInventoryMaster
                                        rmInventoryMaster.Qty -= iRMQtyUsed;
                                        rmInventoryMaster.LastModifiedOn = timestamp;
                                        db.Entry(rmInventoryMaster).State = System.Data.Entity.EntityState.Modified;
                                        //iRMQtyUsed -= (int)Math.Ceiling(rmInventoryMaster.Qty);
                                        db.SaveChanges();
                                        iRMQtyUsed = 0;
                                    }
                                    else
                                    {
                                        //RM qty available (in bag) is less than required qty
                                        //add new entry to ProductionRawMaterial
                                        var productionRawMaterial = new ProductionRawMaterial()
                                        {
                                            ProductionProgramID = iProductionProgramID,
                                            RawMaterialID = pp.RawMaterialID,
                                            RMInventoryID = rmInventoryMaster.RMInventoryID,
                                            RMQty = (int)Math.Ceiling(rmInventoryMaster.Qty),
                                            AddedOn = timestamp
                                        };
                                        db.ProductionRawMaterials.Add(productionRawMaterial);
                                        iRMQtyUsed -= (int)Math.Ceiling(rmInventoryMaster.Qty);

                                        //update record/s in RawMaterialInventoryMaster
                                        rmInventoryMaster.Qty = 0;
                                        rmInventoryMaster.LastModifiedOn = timestamp;
                                        db.Entry(rmInventoryMaster).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                //else
                                //{
                                //    bRMWasAvailable = false;
                                //}
                            }
                        }

                        /*
                        foreach (var pp in productionProgram.ProductionProgramRMMappings)
                        {
                            //get raw material required qty for production
                            var iRMQty = pp.RMQty;
                            var iRMQtyUsed = iRMQty;

                            while (iRMQtyUsed > 0)
                            {
                                var rmInventoryMaster = db.RawMaterialInventoryMasters.FirstOrDefault(x => x.RawMaterialID == pp.RawMaterialID && x.Qty > 0);
                                if (rmInventoryMaster != null)
                                {
                                    if (rmInventoryMaster.Qty > iRMQtyUsed)
                                    {
                                        //RM qty available (in bag) is more than required qty
                                        //add new entry to ProductionRawMaterial
                                        var productionRawMaterial = new ProductionRawMaterial()
                                        {
                                            ProductionProgramID = iProductionProgramID,
                                            RawMaterialID = pp.RawMaterialID,
                                            RMInventoryID = rmInventoryMaster.RMInventoryID,
                                            RMQty = iRMQtyUsed,
                                            AddedOn = timestamp
                                        };

                                        db.ProductionRawMaterials.Add(productionRawMaterial);
                                        //iRMQtyUsed = 0;

                                        //update record/s in RawMaterialInventoryMaster
                                        rmInventoryMaster.Qty -= iRMQtyUsed;
                                        rmInventoryMaster.LastModifiedOn = timestamp;
                                        db.Entry(rmInventoryMaster).State = System.Data.Entity.EntityState.Modified;
                                        //iRMQtyUsed -= (int)Math.Ceiling(rmInventoryMaster.Qty);
                                        db.SaveChanges();
                                        iRMQtyUsed = 0;
                                    }
                                    else
                                    {
                                        //RM qty available (in bag) is less than required qty
                                        //add new entry to ProductionRawMaterial
                                        var productionRawMaterial = new ProductionRawMaterial()
                                        {
                                            ProductionProgramID = iProductionProgramID,
                                            RawMaterialID = pp.RawMaterialID,
                                            RMInventoryID = rmInventoryMaster.RMInventoryID,
                                            RMQty = (int)Math.Ceiling(rmInventoryMaster.Qty),
                                            AddedOn = timestamp
                                        };
                                        db.ProductionRawMaterials.Add(productionRawMaterial);
                                        iRMQtyUsed -= (int)Math.Ceiling(rmInventoryMaster.Qty);

                                        //update record/s in RawMaterialInventoryMaster
                                        rmInventoryMaster.Qty = 0;
                                        rmInventoryMaster.LastModifiedOn = timestamp;
                                        db.Entry(rmInventoryMaster).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                //else
                                //{
                                //    bRMWasAvailable = false;
                                //}
                            }
                        }
                        */

                        //add to mobuld history
                        db.MouldHistories.Add(new MouldHistory { MouldID = productionProgram.MouldID, MouldStatusID = (int)enumMouldStatus.InUse, Description = "In production " + productionProgram.ProductionProgramID, AddedOn = timestamp }); ; ;

                        //add to machine history
                        //db.MachineHistories.Add(new MachineHistory { MachineID = productionProgram.MachineID, MachineStatusID = (int)enumMachineStatus.InUse, Description = "In production " + productionProgram.ProductionProgramID, AddedOn = DateTime.Today });

                        db.MachineHistories.Add(new MachineHistory { MachineID = productionProgram.MachineID.Value, MachineStatusID = (int)enumMachineStatus.InUse, Description = "In production " + productionProgram.ProductionProgramID, AddedOn = timestamp });

                        db.SaveChanges();

                        transaction.Commit();

                        returnValue.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        //TODO error handling
                        returnValue.ResponseMsg = ex.Message;
                        CommonLogger.Error(ex, ex.Message);
                    }
                }
            }

            return Json(returnValue);
        }

        public IHttpActionResult GetProductionPlan(int id)
        {
            var returnValue = new ProductionPlanResponse();

            try
            {
                var data = db.ProductionPrograms.SingleOrDefault(x => x.ProductionProgramID == id);

                var o = mapper.Map<ProductionProgram, KPILib.Models.ProductionPlan>(data);

                o.ProductionProgramDate = data.AddedOn;
                o.SalesID = data.SalesDetail.SalesID;
                o.SalesDate = data.SalesDetail.SalesMaster.SalesDate;
                o.SalesUserID = data.SalesDetail.SalesMaster.UserID;
                o.SalesUser = data.SalesDetail.SalesMaster.UserMaster.Username;
                o.CompanyLocationID = data.SalesDetail.SalesMaster.CompanyLocationID;
                //o.CompanyLocation = data.SalesDetail.SalesMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + data.SalesDetail.SalesMaster.CompanyLocationMaster.LocationName + "]";

                o.ProductName = data.ProductMaster.ProductName;

                //o.RawMaterialName = data.RawMaterialMaster.RawMaterialName;

                o.MouldName = data.MouldMaster.MouldName;

                o.MachineName = data.MachineMaster != null ? data.MachineMaster.MachineName : string.Empty;

                o.ProductionUser = data.UserMaster.Username;

                o.RawMaterialList = new List<RMInventory>();

                foreach (var itm in data.ProductionRawMaterials)
                {
                    o.RawMaterialList.Add(new RMInventory { PalletNo = itm.RawMaterialInventoryMaster.PalletMaster.PalletNo, RawMaterialName = itm.RawMaterialMaster.RawMaterialName, Qty = itm.RMQty, TagColour = itm.RawMaterialInventoryMaster.TagColourMaster.TagColour, AddedOn = itm.RawMaterialInventoryMaster.AddedOn });
                }
                returnValue.data = o;

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

        public IHttpActionResult UpdateProduction(Dictionary<string, int> parameters) //(int iProductionProgramID)
        {
            int iProductionProgramID, iProducedNow, iQty, iBatchId;
            iProductionProgramID = iProducedNow = 0;

            Int32.TryParse(parameters["iProductionProgramID"].ToString(), out iProductionProgramID);
            Int32.TryParse(parameters["iProducedNow"].ToString(), out iProducedNow);
            Int32.TryParse(parameters["iBatchId"].ToString(), out iBatchId);


            iQty = iProducedNow;

            var returnValue = new ResponseObj();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var productionProgram = db.ProductionPrograms
                            .Include(x => x.ProductMaster)
                            .Include(x => x.SalesDetail.SalesMaster.SalesDetails.Select(y => y.ProductionPrograms))
                            .SingleOrDefault(x => x.ProductionProgramID == iProductionProgramID);
                        if (productionProgram == null)
                        {
                            returnValue.ResponseMsg = "Production program not found.";
                            return Json(returnValue);
                        }

                        ProductionProgramBatch programBatch = db.ProductionProgramBatches.Where(z => z.ProgramBatchID == iBatchId).FirstOrDefault();
                        if (programBatch == null)
                        {
                            returnValue.ResponseMsg = "Production batch not found.";
                            return Json(returnValue);
                        }

                        var prod = productionProgram.ProductMaster;
                        var salesDetail = productionProgram.SalesDetail;
                        if (!prod.PkgQty.HasValue || prod.PkgQty.Value <= 0 || !prod.PkgsPerRack.HasValue || prod.PkgsPerRack.Value <= 0)
                        {
                            returnValue.ResponseMsg = "Product packaging configuration is invalid.";
                            return Json(returnValue);
                        }

                        if (iProducedNow <= 0)
                        {
                            returnValue.ResponseMsg = "Produced quantity must be greater than zero.";
                            return Json(returnValue);
                        }

                        var batchInProductionQty = programBatch.InProductionQty;
                        if (batchInProductionQty < iProducedNow)
                        {
                            returnValue.ResponseMsg = "Produced quantity cannot be greater than quantity currently in production.";
                            return Json(returnValue);
                        }

                        DateTime timestamp = DateTime.Now;
                        var stagedRackEntries = new List<ProdReadyStored>();
                        var pkgQty = prod.PkgQty.Value;
                        var pkgsPerRack = prod.PkgsPerRack.Value;
                        var rackCapacity = pkgQty * pkgsPerRack;

                        var racks = db.ProdReadyStoreds
                            .Where(x => x.ProductID == prod.ProductID)
                            .GroupBy(x => x.RackID)
                            .Select(y => new { y.Key, Qty = y.Sum(z => z.Qty) })
                            .Where(x => x.Qty < rackCapacity)
                            .OrderBy(r => r.Qty)
                            .ToList();

                        foreach (var r in racks)
                        {
                            var availableQty = rackCapacity - r.Qty;
                            while (availableQty > 0 && iQty > 0)
                            {
                                var qtyToStore = Math.Min(iQty >= pkgQty ? pkgQty : iQty, availableQty);
                                if (qtyToStore <= 0)
                                {
                                    break;
                                }

                                stagedRackEntries.Add(CreateProdReadyStored(productionProgram.ProductionProgramID, r.Key, prod.ProductID, qtyToStore, timestamp));
                                availableQty -= qtyToStore;
                                iQty -= qtyToStore;
                            }

                            if (iQty == 0)
                            {
                                break;
                            }
                        }

                        var emptyRackIds = iQty > 0
                            ? db.sp_GetEmptyRacks().Where(x => x.HasValue).Select(x => x.Value).ToList()
                            : new List<int>();
                        var emptyRackIndex = 0;

                        while (iQty > 0)
                        {
                            if (emptyRackIndex < emptyRackIds.Count)
                            {
                                var emptyRackID = emptyRackIds[emptyRackIndex++];
                                var availableQty = rackCapacity;

                                while (availableQty > 0 && iQty > 0)
                                {
                                    var qtyToStore = Math.Min(iQty >= pkgQty ? pkgQty : iQty, availableQty);
                                    if (qtyToStore <= 0)
                                    {
                                        break;
                                    }

                                    stagedRackEntries.Add(CreateProdReadyStored(productionProgram.ProductionProgramID, emptyRackID, prod.ProductID, qtyToStore, timestamp));
                                    availableQty -= qtyToStore;
                                    iQty -= qtyToStore;
                                }

                                if (iQty == 0)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                CommonFunctions.WriteToLog("### SYSTEM LIMITATION ###");
                                CommonFunctions.WriteToLog("No racks available to store product - " + prod.ProductID);
                                break;
                            }
                        }

                        if (stagedRackEntries.Count > 0)
                        {
                            db.ProdReadyStoreds.AddRange(stagedRackEntries);
                        }

                        productionProgram.InProductionQty -= iProducedNow;
                        if (productionProgram.InProductionQty < 0)
                        {
                            productionProgram.InProductionQty = 0;
                        }
                        productionProgram.ProductQtyCompleted += iProducedNow;
                        productionProgram.LastModifiedOn = timestamp;
                        db.Entry(productionProgram).State = System.Data.Entity.EntityState.Modified;

                        if (salesDetail != null)
                        {
                            salesDetail.QtyInProduction = Math.Max(0, salesDetail.QtyInProduction - iProducedNow);
                            salesDetail.QtyProduced += iProducedNow;
                            salesDetail.SalesStatusID = salesDetail.QtyBal <= 0
                                ? (int)enumSalesStatus.Completed_Closed
                                : (int)enumSalesStatus.In_Production;
                            db.Entry(salesDetail).State = System.Data.Entity.EntityState.Modified;
                        }

                        programBatch.ProductQtyCompleted += iProducedNow;
                        programBatch.InProductionQty -= iProducedNow;
                        db.Entry(programBatch).State = System.Data.Entity.EntityState.Modified;

                        //if (productionProgram.ProductQtyCompleted >= productionProgram.ProductQty)
                        if (programBatch.InProductionQty == 0)
                        {
                            var mouldHistory = new MouldHistory() { MouldID = productionProgram.MouldID, MouldStatusID = (int)enumMouldStatus.NotInUse, Description = "Completed production " + productionProgram.ProductionProgramID, AddedOn = timestamp };
                            db.MouldHistories.Add(mouldHistory);

                            var machineHistory = new MachineHistory() { MachineID = productionProgram.MachineID.Value, MachineStatusID = (int)enumMachineStatus.NotInUse, Description = "Completed production " + productionProgram.ProductionProgramID, AddedOn = timestamp };
                            db.MachineHistories.Add(machineHistory);
                        }

                        if (salesDetail != null && salesDetail.SalesMaster != null)
                        {
                            RecalculateSalesMasterStatus(salesDetail.SalesMaster);
                        }

                        db.SaveChanges();

                        transaction.Commit();

                        returnValue.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        //TODO error handling
                        returnValue.ResponseMsg = ex.Message;
                        CommonLogger.Error(ex, ex.Message);
                    }
                }
            }

            return Json(returnValue);
        }

        private ProdReadyStored CreateProdReadyStored(int productionProgramId, int rackId, int productId, int qty, DateTime timestamp)
        {
            return new ProdReadyStored
            {
                ProductionProgramID = productionProgramId,
                RackID = rackId,
                ProductID = productId,
                Qty = qty,
                RcvdDate = timestamp
            };
        }

        public IHttpActionResult GetRackingPlan(int id)
        {
            var returnValue = new ProductRackingPlanResponse();

            try
            {
                var data = db.ProductionPrograms.SingleOrDefault(x => x.ProductionProgramID == id);

                var o = mapper.Map<ProductionProgram, KPILib.Models.ProductRackingPlan>(data);

                o.ProductionProgramDate = data.AddedOn;
                o.SalesID = data.SalesDetail.SalesID;
                o.SalesDate = data.SalesDetail.SalesMaster.SalesDate;
                o.SalesUserID = data.SalesDetail.SalesMaster.UserID;
                o.SalesUser = data.SalesDetail.SalesMaster.UserMaster.Username;
                o.CompanyLocationID = data.SalesDetail.SalesMaster.CompanyLocationID;
                //o.CompanyLocation = data.SalesDetail.SalesMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + data.SalesDetail.SalesMaster.CompanyLocationMaster.LocationName + "]";

                o.ProductName = data.ProductMaster.ProductName;

                //o.RawMaterialName = data.RawMaterialMaster.RawMaterialName;

                o.MouldName = data.MouldMaster.MouldName;

                o.MachineName = data.MachineMaster != null ? data.MachineMaster.MachineName : string.Empty;

                o.ProductionUser = data.UserMaster.Username;

                o.ProductInRacks = new List<ProductInRack>();

                var products = db.ProductMasters.ToList();
                var racks = db.RackMasters.ToList();

                foreach (var itm in data.ProdReadyStoreds.GroupBy(x => new { x.RcvdDate, x.RackID, x.ProductID }).Select(x => new { x.Key.RcvdDate, x.Key.RackID, x.Key.ProductID, Qty = x.Sum(y => y.Qty) }))
                {
                    var prod = products.SingleOrDefault(x => x.ProductID == itm.ProductID);
                    var rack = racks.SingleOrDefault(x => x.RackID == itm.RackID);
                    var pkgs = (int)itm.Qty / prod.PkgQty;
                    var openPkgs = itm.Qty % prod.PkgQty;

                    o.ProductInRacks.Add(new ProductInRack { ProductID = itm.ProductID, ProductName = prod.ProductName, RackID = itm.RackID, RackNo = rack.RackNo, Qty = itm.Qty, RcvdDate = itm.RcvdDate, Description = "(" + prod.PkgQty + " x " + pkgs + " pks) + " + openPkgs + " nos." });
                }
                returnValue.data = o;

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

        public void AddUpdateProductionBatch(ProductionProgramBatch productionBatches)
        {
            if (productionBatches.ProgramBatchID == 0)
            {
                //Add
                db.ProductionProgramBatches.Add(productionBatches);
                db.SaveChanges();
            }
            else
            {
                //Update
                db.Entry(productionBatches).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        private bool IsProgramInProgress(ProductionProgram program)
        {
            if (program == null)
            {
                return false;
            }

            if (program.InProductionQty > 0)
            {
                return true;
            }

            if (program.ProductionProgramBatches != null && program.ProductionProgramBatches.Any(x => x.InProductionQty > 0))
            {
                return true;
            }

            return program.ProductionRawMaterials != null && program.ProductionRawMaterials.Count() > 0;
        }

        private void RecalculateSalesMasterStatus(SalesMaster salesMaster)
        {
            if (salesMaster == null)
            {
                return;
            }

            var activeSalesDetails = salesMaster.SalesDetails.Where(x => x.IsActive).ToList();
            if (activeSalesDetails.Count == 0)
            {
                return;
            }

            salesMaster.SalesStatusID = activeSalesDetails.All(x => x.QtyBal <= 0)
                ? (int)enumSalesStatus.Completed_Closed
                : activeSalesDetails.Max(x => x.SalesStatusID);

            db.Entry(salesMaster).State = System.Data.Entity.EntityState.Modified;
        }
    }
}

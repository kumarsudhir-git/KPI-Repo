using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
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

        public IHttpActionResult SetNewDispatchBlockValues(Dictionary<string, int> parameters) //(int iSalesDetailsID, int iBlockedQty, int iToDispatchQty)
        {
            int iSalesDetailsID, iBlockedQty, iToDispatchQty, iToProduceQty;
            iSalesDetailsID = iBlockedQty = iToDispatchQty = iToProduceQty = 0;

            Int32.TryParse(parameters["iSalesDetailsID"].ToString(), out iSalesDetailsID);
            Int32.TryParse(parameters["iBlockedQty"].ToString(), out iBlockedQty);
            Int32.TryParse(parameters["iToDispatchQty"].ToString(), out iToDispatchQty);
            Int32.TryParse(parameters["iToProduceQty"].ToString(), out iToProduceQty);

            var returnValue = new ResponseObj();

            try
            {
                var item = db.SalesDetails.SingleOrDefault(x => x.SalesDetailsID == iSalesDetailsID);
                if (item != null)
                {
                    item.QtyToDispatch = iToDispatchQty;
                    item.QtyBlocked = iBlockedQty;

                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                }

                if (iToProduceQty > 0)
                {
                    //var machines = new List<int>();
                    //foreach (var machine in item.ProductMaster.MouldMaster.MachineMouldMappings.Where(x => !x.IsDiscontinued))
                    //    machines.Add(machine.MachineID);

                    //var machineID = db.MachineHistories.Where(x => machines.Contains(x.MachineID) && x.MachineStatusID == 101).FirstOrDefault().MachineID;

                    //var rmInventory = db.RawMaterialInventoryMasters.Where(x => x.RawMaterialID == item.ProductMaster.RawMaterialID && x.Qty > 0);

                    //var toProduceQty = iToProduceQty;
                    //foreach (var rmInv in rmInventory)
                    //{
                    //    if (toProduceQty > 0)
                    //    {
                    //        var qtyCanBeProducedFromRMQty = (int)(rmInv.Qty / item.ProductMaster.RMReqdForUOMQty * item.ProductMaster.MinQtyUOM);

                    //        if (qtyCanBeProducedFromRMQty > toProduceQty)
                    //        {

                    var productRmMapping = db.ProductRawMaterialMappings.Where(x => x.ProductID == item.ProductID);

                    ProductionProgram program = new ProductionProgram()
                    {
                        SalesDetailsID = item.SalesDetailsID,
                        ProductID = item.ProductID,
                        ProductQty = iToProduceQty,
                        //RawMaterialID = item.ProductMaster.RawMaterialID,
                        //RMQty = (int)Math.Round(item.ProductMaster.RMReqdForUOMQty * (iToProduceQty / item.ProductMaster.MinQtyUOM), 0),
                        //RMInventoryID = rmInv.RMInventoryID,
                        MouldID = item.ProductMaster.MouldID,
                        //MachineID = machineID,
                        ProductQtyCompleted = 0,
                        UserID = 1001, //// ### admin
                        AddedOn = DateTime.Now,
                        LastModifiedOn = DateTime.Now
                    };

                    db.ProductionPrograms.Add(program);

                    foreach (var productRM in productRmMapping)
                    {
                        ProductionProgramRMMapping productionProgramRMMapping = new ProductionProgramRMMapping()
                        {
                            ProductID = productRM.ProductID,
                            RawMaterialID = productRM.RawMaterialID, 
                            RMQty = (int)Math.Round(productRM.RMReqdForUOMQty * (iToProduceQty / item.ProductMaster.MinQtyUOM), 0),
                            ProductionProgramID = program.ProductionProgramID
                        };

                        db.ProductionProgramRMMappings.Add(productionProgramRMMapping);
                    }

                    //        toProduceQty = 0;

                    //        break;
                    //    }
                    //    else
                    //    {
                    //        ProductionProgram program = new ProductionProgram()
                    //        {
                    //            SalesDetailsID = item.SalesDetailsID,
                    //            ProductID = item.ProductID,
                    //            ProductQty = qtyCanBeProducedFromRMQty,
                    //            RawMaterialID = item.ProductMaster.RawMaterialID,
                    //            RMQty = (int)Math.Round(item.ProductMaster.RMReqdForUOMQty * (qtyCanBeProducedFromRMQty / item.ProductMaster.MinQtyUOM), 0),
                    //            RMInventoryID = rmInv.RMInventoryID,
                    //            MouldID = item.ProductMaster.MouldID,
                    //            MachineID = machineID,
                    //            ProductQtyCompleted = 0,
                    //            UserID = 1001, //// ### admin
                    //            AddedOn = DateTime.Now,
                    //            LastModifiedOn = DateTime.Now
                    //        };

                    //        db.ProductionPrograms.Add(program);

                    //        toProduceQty = toProduceQty - qtyCanBeProducedFromRMQty;
                    //    }
                    //}
                }

                db.SaveChanges();
                //}

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
    }
}

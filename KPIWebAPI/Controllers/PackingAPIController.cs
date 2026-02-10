using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Dispatch")]
    public class PackingAPIController : CCSPLBaseAPIController
    {
        public IHttpActionResult GetAll(int id)
        {
            var returnValue = new SalesDispatchMasterResponse();

            try
            {
                var data = db.SalesDetails.Where(x => x.QtyToDispatch > 0).OrderByDescending(x => x.SalesID);

                if (id == 1)
                {
                    data = db.SalesDetails.Where(x => x.QtyDispatched > 0).OrderByDescending(x => x.SalesID);
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
                            QtyDispatched = obj.SalesDispatchDetails.Sum(x=> x.DispatchQty),      //replace with dispatched qty
                            QtyBal = obj.QtyBal,
                            //QtyAvailable = (int)obj.ProductMaster.ProductInventoryMasters.Sum(x => x.Qty) - db.SalesDetails.Where(x => x.ProductID == obj.ProductID).Sum(x => x.QtyBlocked + x.QtyToDispatch),
                            //QtyAvailable = (int)obj.ProductMaster.ProdReadyStoreds.Sum(x => x.Qty) - db.SalesDetails.Where(x => x.ProductID == obj.ProductID).Sum(x => x.QtyBlocked + x.QtyToDispatch),
                            //QtyToProduce = obj.ProductionPrograms.Where(x => x.ProductID == obj.ProductID && x.SalesDetailsID == obj.SalesDetailsID && x.ProductQtyCompleted == 0 && x.InProductionQty == 0).Sum(x => x.ProductQty),
                            //QtyInProduction = obj.ProductionPrograms.Where(x => x.ProductID == obj.ProductID && x.SalesDetailsID == obj.SalesDetailsID).Sum(x => x.InProductionQty),
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

        public IHttpActionResult GetPackingDetails(int id) //List<int> SalesIDProductID)
        {
            var returnValue = new PackingDispatchMasterResponse();

            try
            {
                var data = db.SalesDetails.SingleOrDefault(x => x.SalesDetailsID == id);      //(x => x.SalesID == salesID && x.ProductID == prodID);

                CompanyMaster companyMstr = CommonFunctions.GetCompanyMasterById(data.SalesMaster.CompanyLocationID);

                returnValue.data = new PackingDispatchMaster()
                {
                    SalesID = data.SalesID,
                    SalesDate = data.SalesMaster.SalesDate,                    

                    //CompanyID = data.SalesMaster.CompanyLocationMaster.CompanyID,
                    CompanyID = data.SalesMaster.CompanyLocationID,
                    //CompanyName = data.SalesMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + data.SalesMaster.CompanyLocationMaster.LocationName + "]",
                    CompanyName = companyMstr != null ? companyMstr.CompanyName : "",

                    CompanyLocationID = data.SalesMaster.CompanyLocationID,
                    CompanyLocation = companyMstr != null && companyMstr.CompanyLocationMasters != null ? string.Join(", ", companyMstr.CompanyLocationMasters.Select(z => z.LocationName)) : "",
                                      //vendorMstr.City + " " +
                                      //vendorMstr.State + " " +
                                      //vendorMstr.PostalCode + " " +
                                      //vendorMstr.Country,

                    //ContactPerson = data.SalesMaster.CompanyLocationMaster.ContactPerson,
                    //Phone = data.SalesMaster.CompanyLocationMaster.Phone,
                    //Phone = data.SalesMaster.CompanyLocationMaster.Phone,
                    //Mobile = companyMstr.ContactNumber,
                    //Email = data.SalesMaster.CompanyLocationMaster.Email,

                    Instructions = data.SalesMaster.Instructions,
                    SalesStatusID = data.SalesMaster.SalesStatusID,
                    Status = data.SalesMaster.SalesStatusMaster.SalesStatus,

                    UserID = data.SalesMaster.UserID,
                    User = data.SalesMaster.UserMaster.Username,

                    Item = new SalesDispatchItem
                    {
                        SalesDetailsID = data.SalesDetailsID,
                        SalesID = data.SalesID,
                        ProductID = data.ProductID,
                        ProductName = data.ProductMaster.ProductName,
                        Instructions = data.Instructions,
                        QtyBooked = data.Qty,
                        QtyBlocked = data.QtyBlocked,
                        QtyToDispatch = data.QtyToDispatch,
                        QtyDispatched = data.SalesDispatchDetails.Sum(x => x.DispatchQty),      //replace with dispatched qty
                        QtyBal = data.QtyBal,
                        ////QtyAvailable = (int)data.ProductMaster.ProductInventoryMasters.Sum(x => x.Qty) - db.SalesDetails.Where(x => x.ProductID == data.ProductID).Sum(x => x.QtyBlocked + x.QtyToDispatch),
                        QtyAvailable = (int)data.ProductMaster.ProdReadyStoreds.Sum(x => x.Qty) - db.SalesDetails.Where(x => x.ProductID == data.ProductID).Sum(x => x.QtyBlocked + x.QtyToDispatch),
                        QtyToProduce = data.ProductionPrograms.Where(x => x.ProductID == data.ProductID && x.SalesDetailsID == data.SalesDetailsID && x.ProductQtyCompleted == 0 && x.InProductionQty == 0).Sum(x => x.ProductQty),
                        QtyInProduction = data.ProductionPrograms.Where(x => x.ProductID == data.ProductID && x.SalesDetailsID == data.SalesDetailsID).Sum(x => x.InProductionQty),
                    }
                };

                int iDispatchQty = data.QtyToDispatch;
                var prodStored = db.ProdReadyStoreds.Where(x => x.ProductID == data.ProductID && x.Qty > 0).OrderBy(x => x.RcvdDate);

                var productsInRack = new List<ProductInRack>();     //TODO: For the print

                returnValue.data.RackDetails = new List<ProductInRack>();
                foreach (var obj in prodStored)
                {
                    if(obj.Qty <= iDispatchQty)
                    {
                        returnValue.data.RackDetails.Add(new ProductInRack
                        {
                            ProductID = obj.ProductID,
                            ProductName = obj.ProductMaster.ProductName,
                            RackID = obj.RackID,
                            RackNo = obj.RackMaster.RackNo,
                            RcvdDate = obj.RcvdDate,
                            ProductionProgramID = obj.ProductionProgramID,
                            ProdReadyStoredID = obj.ProdReadyStoredID,
                            Qty = obj.Qty
                        });

                        iDispatchQty = iDispatchQty - obj.Qty;
                        
                        //update qty in DB
                        obj.Qty = 0;
                        db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        returnValue.data.RackDetails.Add(new ProductInRack
                        {
                            ProductID = obj.ProductID,
                            ProductName = obj.ProductMaster.ProductName,
                            RackID = obj.RackID,
                            RackNo = obj.RackMaster.RackNo,
                            RcvdDate = obj.RcvdDate,
                            ProductionProgramID = obj.ProductionProgramID,
                            ProdReadyStoredID = obj.ProdReadyStoredID,
                            Qty = iDispatchQty,
                        });

                        iDispatchQty = 0;

                        //update qty in DB
                        obj.Qty = obj.Qty - iDispatchQty;
                        db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    }

                    if (iDispatchQty == 0)
                        break;
                }

                //Insert into SalesDispatchDetails
                SalesDispatchDetail dispatch = new SalesDispatchDetail
                {
                    SalesDetailsID = data.SalesDetailsID,
                    ProductID = data.ProductID,
                    DispatchQty = data.QtyToDispatch,
                    //UserID = 1001,                  //TODO: Current User
                    DispatchDate = DateTime.Now,    //TODO: UTC Tiime
                    DispatchNotes = ""
                };
                db.SalesDispatchDetails.Add(dispatch);

                //Update the SalesDetails entry
                data.QtyDispatched = data.QtyDispatched + data.QtyToDispatch;
                data.QtyBal = data.QtyBal - data.QtyToDispatch;
                data.QtyToDispatch = 0;
                db.Entry(data).State = System.Data.Entity.EntityState.Modified;

                //TODO: Email Client

                //TODO: Upload Invoice


                db.SaveChanges();

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
    }
}
﻿using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Sales")]
    public class SalesAPIController : CCSPLBaseAPIController
    {
        // GET: SalesAPI
        public IHttpActionResult GetAll()
        {
            var returnValue = new SalesMastersResponse();

            try
            {
                var data = db.SalesMasters.ToList();    //.OrderByDescending(x => x.SalesID).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<SalesMaster, KPILib.Models.SalesMaster>(obj);
                    //o.CompanyLocation = obj.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + obj.CompanyLocationMaster.LocationName + "]";
                    Company companyMaster = CommonFunctions.GetCompanyLocationById(obj.CompanyLocationID);
                    if (companyMaster != null)
                    {
                        o.CompanyLocation = companyMaster.CompanyName + " [" + companyMaster.LocationName + "]";
                    }
                    //if (!string.IsNullOrEmpty(obj.GMS))
                    //{
                    //    o.GMS = CommonFunctions.GetLookUpMasterDataFromLookupCode(obj.GMS)?.LookUpName;
                    //}
                    //o.Status = obj.SalesStatusMaster.SalesStatus;
                    o.User = obj.UserMaster.Username;

                    returnValue.data.Add(o);
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

        // GET: SalesAPI
        public IHttpActionResult GetAllByRMID(int id)
        {
            var returnValue = new SalesMastDetailsResponse();

            try
            {
                var data = from sd in db.SalesDetails
                           from sm in db.SalesMasters
                           from comp in db.CompanyMasters
                           from comploc in db.CompanyLocationMasters
                           from pm in db.ProductMasters
                           from usr in db.UserMasters
                           where
                                sd.SalesID == sm.SalesID &&
                                sm.CompanyLocationID == comploc.CompanyLocationID &&
                                comploc.CompanyID == comp.CompanyID &&
                                sd.ProductID == pm.ProductID &&
                                sm.UserID == usr.UserID &&
                                sm.SalesStatusID < ((int)enumSalesStatus.Completed_Closed) &&
                                sd.ProductID == id &&
                                sd.Qty > sd.Qty
                           select new
                           {
                               sd.SalesDetailsID,
                               sm.SalesDate,
                               sm.SalesID,
                               CompanyLocation = comp.CompanyName + " [" + comploc.LocationName + "]",
                               //SalesDetailsInstructions = sd.Instructions.Replace("\n", "<br/>"),
                               //SalesMasterInstructions = sm.Instructions.Replace("\n", "<br/>"),
                               SalesDetailsInstructions = sd.Instructions.Replace("\n", "..."),
                               SalesMasterInstructions = sm.Instructions.Replace("\n", "..."),
                               Qty = sd.Qty,
                               ProductID = sd.ProductID,
                               ProductName = pm.ProductName,
                               Status = sm.SalesStatusMaster.SalesStatus,
                               User = usr.Username
                           };

                //var data = db.SalesDetails.Where(x => x.SalesMaster.SalesStatusID < ((int)enumSalesStatus.Full_Rcvd__Closed) && x.RawMatarialID == id && x.Qty > x.RcvdQty).OrderByDescending(x => x.SalesID);
                foreach (var obj in data)
                {
                    var o = new SalesMastDetail()
                    {
                        SalesDetailsID = obj.SalesDetailsID,
                        CompanyLocation = obj.CompanyLocation,
                        SalesDate = obj.SalesDate,
                        SalesDetailsInstructions = obj.SalesDetailsInstructions,
                        SalesID = obj.SalesID,
                        SalesMasterInstructions = obj.SalesMasterInstructions,
                        Qty = obj.Qty,
                        ProductID = obj.ProductID,
                        ProductName = obj.ProductName,
                        Status = obj.Status,
                        User = obj.User

                        //SalesDetailsID = obj.SalesDetailsID,
                        //CompanyLocation = obj.SalesMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + obj.SalesMaster.CompanyLocationMaster.LocationName + "]",
                        //SalesDate = obj.SalesMaster.SalesDate,
                        //SalesDetailsInstructions = obj.Instructions.Replace("\n", "<br/>"),
                        //SalesID = obj.SalesID,
                        //SalesMasterInstructions = obj.SalesMaster.Instructions.Replace("\n", "<br/>"),
                        //Qty = obj.Qty,
                        //ProductID = obj.ProductID,
                        //ProductName = obj.ProductMaster.ProductName,
                        //Status = obj.SalesMaster.SalesStatusMaster.SalesStatus,
                        //User = obj.SalesMaster.UserMaster.Username
                    };

                    returnValue.data.Add(o);
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

        // GET api/values/5
        public IHttpActionResult Get(int id, int UserRoleId)
        {
            var returnValue = new SalesMasterResponse();

            try
            {
                #region get all Locations for ddl

                //TODO: 103 = Customers
                //var Locations = db.CompanyLocationMasters.Where(x => !x.CompanyMaster.IsDiscontinued && x.CompanyMaster.CompanyTypeID == 103).Where(x => !x.IsDiscontinued).OrderBy(x => x.CompanyMaster.CompanyName).OrderBy(x => x.LocationName).ToList();
                //List<KPILib.Models.KeyValuePair> compLocations = new List<KPILib.Models.KeyValuePair>();
                //foreach (var obj in Locations)
                //{
                //    var o = new KeyValuePair { Key = obj.CompanyLocationID, Value = obj.CompanyMaster.CompanyName + " [" + obj.LocationName + "]" };
                //    compLocations.Add(o);
                //}

                #endregion   

                #region get all Product for ddl
                var Products = db.ProductMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.ProductName).ToList();
                List<KPILib.Models.KeyValuePair> products = new List<KPILib.Models.KeyValuePair>();
                foreach (var obj in Products)
                {
                    var o = new KeyValuePair { Key = obj.ProductID, Value = obj.ProductName };
                    products.Add(o);
                }
                products.Insert(0, new KeyValuePair { Key = 0, Value = "--Select Product--" });

                #endregion

                var data = db.SalesMasters.SingleOrDefault(x => x.SalesID == id);
                if (data != null)
                {
                    var o = mapper.Map<SalesMaster, KPILib.Models.SalesMaster>(data);
                    //o.Locations = compLocations;
                    o.Products = products;
                    //o.Status = data.SalesStatusMaster.SalesStatus;
                    o.User = data.UserMaster.Username;
                    //o.CompanyLocation = data.CompanyLocationMaster.LocationName;

                    foreach (var item in data.SalesDetails.Where(x => x.IsActive))
                    {
                        var lineItem = mapper.Map<SalesDetail, KPILib.Models.SalesDetails>(item);
                        lineItem.ProductName = item.ProductMaster.ProductName;
                        o.LineItems.Add(lineItem);
                    }

                    //o.RMIds = CommonFunctions.GetRMIdsFromSalesId(o.SalesID);
                    //if (o.LineItems.Count < 5)
                    //{
                    //    for (int i = o.LineItems.Count + 1; i <= 5; i++)
                    //    {
                    //        o.LineItems.Add(new SalesDetails());
                    //    }
                    //}

                    returnValue.data = o;
                }
                else
                {
                    var o = new KPILib.Models.SalesMaster() { LineItems = new List<SalesDetails>(), Products = products };
                    o.LineItems.Add(new SalesDetails());

                    //if (o.LineItems.Count < 5)
                    //{
                    //    for (int i = o.LineItems.Count; i <= 5; i++)
                    //    {
                    //        o.LineItems.Add(new SalesDetails());
                    //    }
                    //}

                    returnValue.data = o;
                }
                returnValue.data.IsSalesRateAccess = CommonFunctions.IsSalesRateAccess(UserRoleId);
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

        public IHttpActionResult Add(KPILib.Models.SalesMaster data)
        {
            var returnValue = new SalesMasterResponse();

            try
            {
                var o = mapper.Map<KPILib.Models.SalesMaster, SalesMaster>(data);
                o.SalesStatusID = (int)enumSalesStatus.Procure_Material;
                foreach (var item in data.LineItems.Where(x => x.ProductID != 0))
                {
                    o.SalesDetails.Add(new SalesDetail
                    {
                        ProductID = item.ProductID,
                        Qty = item.Qty,
                        QtyBal = item.Qty,
                        Instructions = item.Instructions,
                        Color = item.Color,
                        Gms = item.Gms,
                        Package = item.Package,
                        Rate = item.Rate,
                        SampleReq = item.SampleReq,
                        SalesStatusID = item.SalesStatusID,
                        IsActive = true
                    });
                }
                o.Instructions += "";

                //o.AddedOn = DateTime.Now;
                //o.LastModifiedOn = DateTime.Now;

                db.SalesMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.SalesID = o.SalesID;

                if (data.RMIds != null && data.RMIds.Count > 0)
                {
                    data.RMIds.Where(z => z != 0).ToList().ForEach(item =>
                    {
                        SalesRMMapping salesRMMapping = new SalesRMMapping()
                        {
                            SalesId = o.SalesID,
                            RMId = item,
                            CreatedBy = data.UserID,
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        };
                        db.SalesRMMappings.Add(salesRMMapping);
                    });
                    db.SaveChanges();
                }
                //var po = db.SalesMasters.Where(x => x.SalesID == o.SalesID).Select(x => new { x.SalesID, x.CompanyLocationMaster.ContactPerson, x.CompanyLocationMaster.Email, x.UserMaster.Username, UserEmail = x.UserMaster.Email }).FirstOrDefault();

                //Dictionary<string, string> key_vals = new Dictionary<string, string>();
                //key_vals.Add("%NAME%", po.ContactPerson);
                //key_vals.Add("%PO_NO%", po.SalesID.ToString());
                //key_vals.Add("%USER%", po.Username);
                //key_vals.Add("%USER_EMAIL%", po.UserEmail);

                //SendEmail(po.ContactPerson, po.Email, po.SalesID, key_vals);

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

        public IHttpActionResult Edit(KPILib.Models.SalesMaster data)
        {
            var returnValue = new SalesMasterResponse();

            try
            {
                var o = db.SalesMasters.SingleOrDefault(x => x.SalesID == data.SalesID);
                if (o != null)
                {
                    //o.CompanyLocationID = data.CompanyLocationID;
                    //o.Instructions = data.Instructions;
                    o.SalesDate = data.SalesDate;
                    o.SalesStatusID = 10;    //Ack Pending
                    //o.Colour = data.Colour;
                    o.CommittedDate = data.CommittedDate;
                    o.CompanyLocationID = data.CompanyLocationID;
                    o.DeliveryAddress = data.DeliveryAddress;
                    //o.GMS = data.GMS;
                    //o.GMSInfo = data.GMSInfo;
                    o.Instructions = data.Instructions;
                    //o.Package = data.Package;
                    o.PaymentStatus = data.PaymentStatus;
                    //o.Quantity = data.Quantity;
                    o.Rate = data.Rate;
                    //o.SampleRequired = data.SampleRequired;
                    o.Transporter = data.Transporter;

                    //o.SalesDetails.Clear();

                    foreach (var item in o.SalesDetails.ToList())
                    {
                        //o.SalesDetails.Remove(item);
                        item.IsActive = false;
                        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                    foreach (var item in data.LineItems.Where(x => x.ProductID != 0))
                    {
                        o.SalesDetails.Add(new SalesDetail
                        {
                            ProductID = item.ProductID,
                            Color = item.Color,
                            Gms = item.Gms,
                            Qty = item.Qty,
                            QtyBal = item.Qty,
                            Package = item.Package,
                            Rate = item.Rate,
                            SampleReq = item.SampleReq,
                            Instructions = item.Instructions,
                            SalesStatusID = item.SalesStatusID,
                            IsActive = true
                        });
                    }

                    if (data.RMIds != null && data.RMIds.Count > 0)
                    {
                        List<SalesRMMapping> salesRMMappings = (from SRM in db.SalesRMMappings
                                                                where SRM.SalesId == data.SalesID && SRM.IsActive
                                                                select SRM
                                                                ).ToList();

                        if (salesRMMappings.Count > 0)
                        {
                            salesRMMappings.ForEach(item =>
                            {
                                item.IsActive = false;
                                item.ModifiedBy = data.UserID;
                                item.ModifiedOn = DateTime.Now;
                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            });
                            db.SaveChanges();
                        }

                        data.RMIds.ForEach(item =>
                        {
                            SalesRMMapping salesRMMapping = new SalesRMMapping()
                            {
                                SalesId = data.SalesID,
                                RMId = item,
                                CreatedBy = data.UserID,
                                CreatedDate = DateTime.Now,
                                IsActive = true
                            };
                            db.SalesRMMappings.Add(salesRMMapping);
                        });
                        db.SaveChanges();
                    }

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    returnValue.data = data;
                    returnValue.data.SalesID = o.SalesID;

                    //var po = db.SalesMasters.Where(x => x.SalesID == o.SalesID).Select(x => new { x.SalesID, x.CompanyLocationMaster.ContactPerson, x.CompanyLocationMaster.Email, x.UserMaster.Username, UserEmail = x.UserMaster.Email }).FirstOrDefault();

                    //Dictionary<string, string> key_vals = new Dictionary<string, string>();
                    //key_vals.Add("%NAME%", po.ContactPerson);
                    //key_vals.Add("%PO_NO%", po.SalesID.ToString());
                    //key_vals.Add("%USER%", po.Username);
                    //key_vals.Add("%USER_EMAIL%", po.UserEmail);

                    //SendEmail(po.ContactPerson, po.Email, po.SalesID, key_vals);

                    returnValue.Response.IsSuccessful();
                }

                returnValue.data = data;
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

        [HttpGet]
        public IHttpActionResult GetProductsDetails()
        {
            ProductMasterResponse returnValue = new ProductMasterResponse();

            try
            {
                #region get all Product for ddl

                using (KPIEntities dbEntities = new KPIEntities())
                {
                    dbEntities.Configuration.ProxyCreationEnabled = false;
                    dbEntities.Configuration.LazyLoadingEnabled = false;

                    List<ProductMaster> productMasters = (from prductMstr in dbEntities.ProductMasters
                                                          where !prductMstr.IsDiscontinued
                                                          orderby prductMstr.ProductName
                                                          select prductMstr).ToList();

                    if (productMasters != null && productMasters.Count > 0)
                    {
                        productMasters.ForEach(z =>
                        {
                            returnValue.productMastersListData.Add(new KPILib.Models.ProductMaster() { ProductID = z.ProductID, ProductName = z.ProductName });
                        });
                    }

                    #endregion

                    returnValue.Response.IsSuccessful();
                }
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }

        [HttpGet]
        public IHttpActionResult GetCompanyLocationDetails(int CompanyId = 103)
        {
            var returnValue = new SalesMasterResponse();

            try
            {
                #region get all Locations for ddl

                //TODO: 103 = Customers
                List<CompanyLocationMaster> companyLocationMasters = (from cmpMstr in db.CompanyLocationMasters
                                                                      where !cmpMstr.CompanyMaster.IsDiscontinued
                                                                      && cmpMstr.CompanyMaster.CompanyTypeID == CompanyId
                                                                      && cmpMstr.IsDiscontinued
                                                                      orderby cmpMstr.CompanyMaster.CompanyName ascending
                                                                      orderby cmpMstr.LocationName ascending
                                                                      select cmpMstr).ToList();
                #endregion

                returnValue.Response.ResponseData = companyLocationMasters;

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

        [HttpGet]
        public IHttpActionResult GetLookUpDataFromTypeName(string LookUpTypeName)
        {
            LookUpMasterResponse lookUpMasterResponse = new LookUpMasterResponse();

            try
            {
                List<LookUpMaster> lookUps = CommonFunctions.GetLookUpMasterDataFromLookupType(LookUpTypeName);

                lookUpMasterResponse.lookupMasterList = mapper.Map<List<LookUpMasterModel>>(lookUps);

                lookUpMasterResponse.Response.IsSuccessful();

            }
            catch (Exception ex)
            {
                lookUpMasterResponse.Response.ResponseCode = 500;
                lookUpMasterResponse.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(lookUpMasterResponse);
        }

        [HttpGet]
        public IHttpActionResult GetRackMasterData()
        {
            RackMastersResponse rackMasterResponse = new RackMastersResponse();

            try
            {
                List<RackMaster> lookUps = CommonFunctions.GetRackMasterData();

                rackMasterResponse.data = mapper.Map<List<KPILib.Models.RackMaster>>(lookUps);

                rackMasterResponse.Response.IsSuccessful();

            }
            catch (Exception ex)
            {
                rackMasterResponse.Response.ResponseCode = 500;
                rackMasterResponse.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(rackMasterResponse);
        }

        [HttpGet]
        public IHttpActionResult GetSalesStatusMasterData()
        {
            SalesSatusMastersResponse rackMasterResponse = new SalesSatusMastersResponse();

            try
            {
                List<SalesStatusMaster> salesStatusMasters = CommonFunctions.GetSalesStatusMasterList();

                rackMasterResponse.data = mapper.Map<List<KPILib.Models.SalesStatusMaster>>(salesStatusMasters);

                rackMasterResponse.Response.IsSuccessful();

            }
            catch (Exception ex)
            {
                rackMasterResponse.Response.ResponseCode = 500;
                rackMasterResponse.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(rackMasterResponse);
        }

    }
}

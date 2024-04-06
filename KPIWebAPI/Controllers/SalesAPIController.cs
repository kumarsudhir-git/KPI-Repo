using KPILib.Models;
using KPIWebAPI.AuthFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

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
                    o.CompanyLocation = obj.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + obj.CompanyLocationMaster.LocationName + "]";
                    o.Status = obj.SalesStatusMaster.SalesStatus;
                    o.User = obj.UserMaster.Username;

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
            }

            return Json(returnValue);
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            var returnValue = new SalesMasterResponse();

            try
            {
                #region get all Locations for ddl

                //TODO: 103 = Customers
                var Locations = db.CompanyLocationMasters.Where(x => !x.CompanyMaster.IsDiscontinued && x.CompanyMaster.CompanyTypeID == 103).Where(x => !x.IsDiscontinued).OrderBy(x => x.CompanyMaster.CompanyName).OrderBy(x => x.LocationName).ToList();
                List<KPILib.Models.KeyValuePair> compLocations = new List<KPILib.Models.KeyValuePair>();
                foreach (var obj in Locations)
                {
                    var o = new KeyValuePair { Key = obj.CompanyLocationID, Value = obj.CompanyMaster.CompanyName + " [" + obj.LocationName + "]" };
                    compLocations.Add(o);
                }

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
                    o.Locations = compLocations;
                    o.Products = products;
                    o.Status = data.SalesStatusMaster.SalesStatus;
                    o.User = data.UserMaster.Username;
                    o.CompanyLocation = data.CompanyLocationMaster.LocationName;

                    foreach (var item in data.SalesDetails)
                    {
                        var lineItem = mapper.Map<SalesDetail, KPILib.Models.SalesDetails>(item);
                        lineItem.ProductName = item.ProductMaster.ProductName;
                        o.LineItems.Add(lineItem);
                    }

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
                    var o = new KPILib.Models.SalesMaster() { LineItems = new List<SalesDetails>(), Locations = compLocations, Products = products };
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

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
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
                    o.SalesDetails.Add(new SalesDetail { ProductID = item.ProductID, Qty = item.Qty, QtyBal = item.Qty });
                }
                o.Instructions += "";

                //o.AddedOn = DateTime.Now;
                //o.LastModifiedOn = DateTime.Now;

                db.SalesMasters.Add(o);
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
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
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

                    //o.SalesDetails.Clear();

                    foreach (var item in o.SalesDetails.ToList())
                        //o.SalesDetails.Remove(item);
                        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;

                    foreach (var item in data.LineItems.Where(x => x.ProductID != 0))
                    {
                        o.SalesDetails.Add(new SalesDetail { ProductID = item.ProductID, Qty = item.Qty });
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
            }

            return Json(returnValue);
        }

    }
}

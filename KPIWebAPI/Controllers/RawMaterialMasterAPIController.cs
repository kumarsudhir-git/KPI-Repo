using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_RM")]
    public class RawMaterialMasterAPIController : CCSPLBaseAPIController
    {
        #region RawMaterial

        // GET api/values
        public IHttpActionResult GetAll()
        {
            var returnValue = new RawMaterialsResponse();

            try
            {
                var data = db.RawMaterialMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.RawMaterialName).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<RawMaterialMaster, KPILib.Models.RawMaterial>(obj);

                    if (obj.VendorId != null)
                    {
                        VendorMaster vendorMasterObj = CommonFunctions.GetVendorDetailsFromId((int)obj.VendorId);
                        if (vendorMasterObj != null)
                        {
                            o.VendorName = vendorMasterObj.VendorName;
                        }
                    }

                    o.UOM = obj.UOMMaster.UnitsName;
                    o.InStock = obj.RawMaterialInventoryMasters.Sum(x => x.Qty);
                    o.Ordered = obj.PurchaseDetails.Where(x => x.PurchaseMaster.PurchaseStatusID < ((int)enumPurchaseStatus.Full_Rcvd__Closed) && x.Qty - x.RcvdQty > 0).Sum(x => x.Qty - x.RcvdQty);

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
        public IHttpActionResult Get(int id)
        {
            var returnValue = new RawMaterialResponse();

            try
            {
                #region get all UOM for ddl
                var UOMs = db.UOMMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.UnitsName).ToList();
                List<UOM> uoms = new List<UOM>();
                foreach (var obj in UOMs)
                {
                    var o = mapper.Map<UOMMaster, KPILib.Models.UOM>(obj);

                    uoms.Add(o);
                }
                #endregion

                var data = db.RawMaterialMasters.SingleOrDefault(x => x.RawMaterialID == id);
                if (data != null)
                {
                    var o = mapper.Map<RawMaterialMaster, KPILib.Models.RawMaterial>(data);
                    o.UOM = data.UOMMaster.UnitsName;
                    o.UOMs = uoms;

                    returnValue.data = o;
                }
                else
                {
                    var o = new KPILib.Models.RawMaterial() { UOMs = uoms };
                    returnValue.data = o;
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

        public IHttpActionResult Add(KPILib.Models.RawMaterial data)
        {
            var returnValue = new RawMaterialResponse();

            try
            {
                var o = mapper.Map<KPILib.Models.RawMaterial, RawMaterialMaster>(data);
                o.AddedOn = DateTime.Now;
                o.LastModifiedOn = DateTime.Now;

                db.RawMaterialMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.RawMaterialID = o.RawMaterialID;

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

        public IHttpActionResult Edit(KPILib.Models.RawMaterial data)
        {
            var returnValue = new RawMaterialResponse();

            try
            {
                var o = db.RawMaterialMasters.SingleOrDefault(x => x.RawMaterialID == data.RawMaterialID);
                if (o != null)
                {
                    o.RawMaterialName = data.RawMaterialName;
                    o.Description = data.Description;
                    o.UOMID = data.UOMID;
                    o.IsDiscontinued = data.IsDiscontinued;
                    o.VendorId = data.VendorId;
                    o.LastModifiedOn = DateTime.Now;

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
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }


        public IHttpActionResult Delete(KPILib.Models.RawMaterial data)
        {
            var returnValue = new RawMaterialResponse();

            try
            {
                var o = db.RawMaterialMasters.SingleOrDefault(x => x.RawMaterialID == data.RawMaterialID);
                if (o != null)
                {
                    o.IsDiscontinued = true;

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }

                returnValue.data = null;
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


        public IHttpActionResult Delete(int id)
        {
            var returnValue = new RawMaterialResponse();

            try
            {
                var o = db.RawMaterialMasters.SingleOrDefault(x => x.RawMaterialID == id);
                if (o != null)
                {
                    o.IsDiscontinued = true;

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }

                returnValue.data = null;
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
        #endregion
    }
}

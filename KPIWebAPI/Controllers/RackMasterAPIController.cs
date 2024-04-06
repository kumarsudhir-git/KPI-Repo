using KPILib.Models;
using KPIWebAPI.AuthFilters;
using System;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Racks")]
    public class RackMasterAPIController : CCSPLBaseAPIController
    {
        #region Rack Module

        // GET api/values
        public IHttpActionResult GetAll()
        {
            var returnValue = new RackMastersResponse();

            try
            {
                var data = db.RackMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.RackNo).ToList();

                foreach (var obj in data)
                {
                    var o = mapper.Map<RackMaster, KPILib.Models.RackMaster>(obj);
                    //var rackData = obj.ProductInventoryMasters.SingleOrDefault();

                    var rackData = obj.ProdReadyStoreds.ToList();
                    if (rackData.Count() > 0)
                    {
                        //considering there is ONLY 1 product stocked on 1 rack
                        o.ProductID = rackData.FirstOrDefault().ProductID;
                        o.ProductName = rackData.FirstOrDefault().ProductMaster.ProductName;
                        o.PkgQty = (rackData.FirstOrDefault().ProductMaster.PkgQty.HasValue ? rackData.FirstOrDefault().ProductMaster.PkgQty.Value : 0);
                        o.Pkts = rackData.Count(x => x.Qty == o.PkgQty);
                        o.OpenPkts = rackData.Count(x => x.Qty < o.PkgQty && x.Qty > 0);
                        o.Qty = (int)rackData.Sum(x => x.Qty);
                    }

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
        public IHttpActionResult GetAllByProdID(int id)
        {
            var returnValue = new RackMastersResponse();

            try
            {
                //var data = db.ProductInventoryMasters.Where(x => x.ProductID == id).OrderBy(x => x.RackMaster.RackNo).ToList();
                var data = db.ProdReadyStoreds.Where(x => x.ProductID == id).OrderBy(x => x.RackMaster.RackNo).ToList();

                //### Considering ONLY 1 product is stacked in 1 rack

                //var data = db.RackMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.RackNo).ToList();
                foreach (var obj in data)
                {
                    if (returnValue.data.Count(z => z.RackID == obj.RackID) == 0)
                    {
                        var o = new KPILib.Models.RackMaster();
                        //var o = mapper.Map<PalletMaster, KPILib.Models.RackMaster>(obj);
                        //var rackData = obj.ProductInventoryMasters.SingleOrDefault();
                        if (obj != null)
                        {
                            o.RackID = obj.RackID;
                            o.RackNo = obj.RackMaster.RackNo;
                            o.Description = obj.RackMaster.Description;
                            o.IsDiscontinued = obj.RackMaster.IsDiscontinued;
                            o.ProductID = obj.ProductID;
                            o.ProductName = obj.ProductMaster.ProductName;

                            o.PkgQty = (obj.ProductMaster.PkgQty.HasValue ? obj.ProductMaster.PkgQty.Value : 0);
                            o.Pkts = data.Where(x => x.RackID == obj.RackID).Count(x => x.Qty == o.PkgQty);
                            o.OpenPkts = data.Where(x => x.RackID == obj.RackID).Count(x => x.Qty < o.PkgQty && x.Qty > 0);
                            o.Qty = (int)data.Where(x => x.RackID == obj.RackID).Sum(x => x.Qty);
                        }

                        returnValue.data.Add(o);
                    }
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
            var returnValue = new RackMasterResponse();

            try
            {
                var data = db.RackMasters.SingleOrDefault(x => x.RackID == id);
                if (data != null)
                {
                    var o = mapper.Map<RackMaster, KPILib.Models.RackMaster>(data);
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

        public IHttpActionResult Add(KPILib.Models.RackMaster data)
        {
            var returnValue = new RackMasterResponse();

            try
            {
                var o = mapper.Map<KPILib.Models.RackMaster, RackMaster>(data);
                o.AddedOn = DateTime.Now;
                o.LastModifiedOn = DateTime.Now;

                db.RackMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.RackID = o.RackID;

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Edit(KPILib.Models.RackMaster data)
        {
            var returnValue = new RackMasterResponse();

            try
            {
                var o = db.RackMasters.SingleOrDefault(x => x.RackID == data.RackID);
                if (o != null)
                {
                    o.RackNo = data.RackNo;
                    o.Description = data.Description;
                    o.IsDiscontinued = data.IsDiscontinued;
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
            }

            return Json(returnValue);
        }


        public IHttpActionResult Delete(KPILib.Models.RackMaster data)
        {
            var returnValue = new RackMasterResponse();

            try
            {
                var o = db.RackMasters.SingleOrDefault(x => x.RackID == data.RackID);
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
            }

            return Json(returnValue);
        }


        public IHttpActionResult Delete(int id)
        {
            var returnValue = new RackMasterResponse();

            try
            {
                var o = db.RackMasters.SingleOrDefault(x => x.RackID == id);
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
            }

            return Json(returnValue);
        }

        #endregion
    }
}

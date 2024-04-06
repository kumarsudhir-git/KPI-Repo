using KPILib.Models;
using KPIWebAPI.AuthFilters;
using System;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Pallets")]
    public class PalletMasterAPIController : CCSPLBaseAPIController
    {
        #region Pallet Module

        // GET api/values
        public IHttpActionResult GetAll()
        {
            var returnValue = new PalletMastersResponse();

            try
            {
                //var data = db.PalletMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.PalletNo).ToList();
                //foreach (var obj in data)
                //{
                //    //var o = mapper.Map<PalletMaster, KPILib.Models.PalletMaster>(obj);

                //    var o = new KPILib.Models.PalletMaster();
                //    var palletData = obj.RawMaterialInventoryMasters;

                //    if (palletData.Count() > 0)     //if (palletData != null)
                //    {
                //        foreach (var rm in palletData)
                //        {
                //            o = new KPILib.Models.PalletMaster();
                //            o = mapper.Map<PalletMaster, KPILib.Models.PalletMaster>(obj);
                //            o.RawMaterialID = rm.RawMaterialID;
                //            o.RawMaterialName = rm.RawMaterialMaster.RawMaterialName;
                //            o.Qty = (int)rm.Qty;
                //            o.QtyKgs = o.Qty * RM_BAG_CAPACITY;
                //            o.TagColour = rm.TagColourMaster.TagColour;

                //            returnValue.data.Add(o);
                //        }
                //    }
                //    else
                //    {
                //        o = mapper.Map<PalletMaster, KPILib.Models.PalletMaster>(obj);
                //        o.RawMaterialName = "";
                //        returnValue.data.Add(o);
                //    }

                var data = db.PalletMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.PalletNo).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<PalletMaster, KPILib.Models.PalletMaster>(obj);
                    var palletData = obj.RawMaterialInventoryMasters;

                    if (palletData != null)
                    {
                        o.Qty = palletData.Sum(x => x.Qty);
                        o.QtyBags = palletData.Count(x => x.Qty == RM_BAG_CAPACITY);
                        o.QtyOpened = palletData.Count(x => x.Qty < RM_BAG_CAPACITY);
                        o.AvailablityKgs = PALLET_CAPACITY - o.Qty;

                        //o.QtyKgs = palletData.Sum(x => x.Qty);
                        //palletData.Where(x => x.Qty == RM_BAG_CAPACITY).Sum(x => x.Qty / RM_BAG_CAPACITY);
                    }
                    //var colours = "";
                    foreach (var colour in palletData.Select(x => new { x.TagColourMaster.TagColour }).Distinct())
                    {
                        o.TagColours += "," + colour.TagColour;
                    }
                    if (o.TagColours != "" && o.TagColours != null)
                        o.TagColours = o.TagColours.Substring(1);
                    else
                        o.TagColours = "";

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
        public IHttpActionResult GetAllByRMID(int id)
        {
            var returnValue = new PalletMastersResponse();

            try
            {
                var data = db.RawMaterialInventoryMasters.Where(x=> x.RawMaterialID == id).OrderBy(x => x.PalletMaster.PalletNo).ToList();

                //var data = db.PalletMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.PalletNo).ToList();
                foreach (var obj in data)
                {
                    var o = new KPILib.Models.PalletMaster();
                    //var o = mapper.Map<PalletMaster, KPILib.Models.PalletMaster>(obj);
                    //var palletData = obj.RawMaterialInventoryMasters.SingleOrDefault();
                    if (obj != null && returnValue.data.Count(x => x.PalletID == obj.PalletID && x.RawMaterialID == obj.RawMaterialID && x.TagColourID == obj.TagColourID) == 0)
                    {
                        o.PalletID = obj.PalletID;
                        o.PalletNo = obj.PalletMaster.PalletNo;
                        o.Description = obj.PalletMaster.Description;
                        o.IsDiscontinued = obj.PalletMaster.IsDiscontinued;
                        o.RawMaterialID = obj.RawMaterialID;
                        o.RawMaterialName = obj.RawMaterialMaster.RawMaterialName;
                        //o.Qty = obj.Qty / RM_BAG_CAPACITY;
                        //o.QtyKgs = obj.Qty;

                        var dataForQty = data.Where(x => x.PalletID == obj.PalletID && x.RawMaterialID == obj.RawMaterialID && x.TagColourID == obj.TagColourID);

                        o.Qty = dataForQty.Sum(x => x.Qty);
                        o.QtyBags = dataForQty.Where(x=> x.Qty == RM_BAG_CAPACITY).Count();
                        o.QtyOpened = dataForQty.Where(x => x.Qty < RM_BAG_CAPACITY).Count();

                        o.TagColour = obj.TagColourMaster.TagColour;
                        o.TagColourID = obj.TagColourID;

                        //#################################
                        //#### TODO: TagColour has to be a comma separated array, just like it is in GetAll()
                        //#################################

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
            var returnValue = new PalletMasterResponse();

            try
            {
                var data = db.PalletMasters.SingleOrDefault(x => x.PalletID == id);
                if(data != null)
                {
                    var o = mapper.Map<PalletMaster, KPILib.Models.PalletMaster>(data);
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
        
        public IHttpActionResult Add(KPILib.Models.PalletMaster data)
        {
            var returnValue = new PalletMasterResponse();

            try
            {
                var o = mapper.Map<KPILib.Models.PalletMaster, PalletMaster>(data);
                o.AddedOn = DateTime.Now;
                o.LastModifiedOn = DateTime.Now;

                db.PalletMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.PalletID = o.PalletID;

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Edit(KPILib.Models.PalletMaster data)
        {
            var returnValue = new PalletMasterResponse();

            try
            {
                var o = db.PalletMasters.SingleOrDefault(x => x.PalletID == data.PalletID);
                if (o != null)
                {
                    o.PalletNo = data.PalletNo;
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


        public IHttpActionResult Delete(KPILib.Models.PalletMaster data)
        {
            var returnValue = new PalletMasterResponse();

            try
            {
                var o = db.PalletMasters.SingleOrDefault(x => x.PalletID == data.PalletID);
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
            var returnValue = new PalletMasterResponse();

            try
            {
                var o = db.PalletMasters.SingleOrDefault(x => x.PalletID == id);
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

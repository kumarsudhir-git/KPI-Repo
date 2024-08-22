using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_RMInventory")]
    public class RMInventoryAPIController : CCSPLBaseAPIController
    {
        // GET api/values
        public IHttpActionResult GetAll()
        {
            var returnValue = new RMInventorysResponse();

            try
            {
                //var data = db.RawMaterialInventoryMasters.OrderByDescending(x => x.RMInventoryID).ToList();

                //var data = db.RawMaterialInventoryMasters.GroupBy(x => new { x.PalletID, x.RawMaterialID, x.TagColourID }).Select(grp => new RawMaterialInventoryMaster { PalletID = grp.Key.PalletID, RawMaterialID = grp.Key.RawMaterialID, TagColourID = grp.Key.TagColourID }).OrderByDescending(x => x.RMInventoryID).ToList();
                //var data = db.RawMaterialInventoryMasters
                //    .GroupBy(x => new { 
                //        x.PalletID, 
                //        x.RawMaterialID, 
                //        x.TagColourID
                //    }).Select(grp => new RawMaterialInventoryMaster { 
                //        PalletID = grp.Key.PalletID, 
                //        RawMaterialID = grp.Key.RawMaterialID, 
                //        TagColourID = grp.Key.TagColourID, 
                //    }).OrderByDescending(x => x.RMInventoryID).ToList();

                //var data = db.Database.SqlQuery(typeof(List<RMInventory>), "select rmim.PalletID,pm.PalletNo,rmim.TagColourID,tcm.TagColour,rmim.RawMaterialID,rmm.RawMaterialName,sum(rmim.Qty) Qty from RawMaterialInventoryMaster rmim, PalletMaster pm, RawMaterialMaster rmm,TagColourMaster tcm where pm.PalletID = rmim.PalletID and rmm.RawMaterialID = rmim.RawMaterialID and tcm.TagColourID = rmim.TagColourID group by rmim.PalletID, pm.PalletNo, rmim.TagColourID, tcm.TagColour, rmim.RawMaterialID, rmm.RawMaterialName", new object[] { });
                //var data = db.Database.ExecuteSqlCommand  Database.SqlQuery(typeof(List<RMInventory>), "select rmim.PalletID,pm.PalletNo,rmim.TagColourID,tcm.TagColour,rmim.RawMaterialID,rmm.RawMaterialName,sum(rmim.Qty) Qty from RawMaterialInventoryMaster rmim, PalletMaster pm, RawMaterialMaster rmm,TagColourMaster tcm where pm.PalletID = rmim.PalletID and rmm.RawMaterialID = rmim.RawMaterialID and tcm.TagColourID = rmim.TagColourID group by rmim.PalletID, pm.PalletNo, rmim.TagColourID, tcm.TagColour, rmim.RawMaterialID, rmm.RawMaterialName", new object[] { });


                var data = db.sp_GetRMInventory();
                foreach (var obj in data)
                {
                    var o = mapper.Map<sp_GetRMInventory_Result, RMInventory>(obj);

                    //o.Qty = o.Qty;
                    //o.QtyBags = o.Qty / RM_BAG_CAPACITY;
                    //o.QtyOpened
                    if (o.AddedBy != null && o.AddedBy > 0)
                    {
                        UserMaster userMaster = CommonFunctions.GetUserMasterData((int)o.AddedBy);
                        if (userMaster != null)
                        {
                            o.AddedByName = userMaster.Username;
                        }
                    }
                    if (o.ModifiedBy != null && o.ModifiedBy > 0)
                    {
                        UserMaster userMaster = CommonFunctions.GetUserMasterData((int)o.ModifiedBy);
                        if (userMaster != null)
                        {
                            o.ModifiedByName = userMaster.Username;
                        }
                    }
                    if (o.LocationId != null && o.LocationId > 0)
                    {
                        o.LocationName = CommonFunctions.getLocationNameFromId((int)o.LocationId);
                    }

                    returnValue.data.Add(o);
                }

                //foreach (var obj in data)
                //{
                //    var o = mapper.Map<RawMaterialInventoryMaster, RMInventory>(obj);
                //    o.PalletNo = obj.PalletMaster.PalletNo;
                //    o.RawMaterial = obj.RawMaterialMaster.RawMaterialName;
                //    o.QtyKgs = Convert.ToInt32(obj.Qty);
                //    o.Qty = Convert.ToInt32(obj.Qty) / RM_BAG_CAPACITY;
                //    o.TagColour = obj.TagColourMaster.TagColour;

                //    returnValue.data.Add(o);
                //}

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult GetAllByRack(int id)
        {
            var returnValue = new RMInventorysResponse();

            try
            {
                var data = db.RawMaterialInventoryMasters.Where(x => x.PalletID == id).OrderByDescending(x => x.RMInventoryID).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<RawMaterialInventoryMaster, RMInventory>(obj);
                    o.PalletNo = obj.PalletMaster.PalletNo;
                    o.RawMaterialName = obj.RawMaterialMaster.RawMaterialName;

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

        public IHttpActionResult GetAllByRM(int id)
        {
            var returnValue = new RMInventorysResponse();

            try
            {
                var data = db.RawMaterialInventoryMasters.Where(x => x.RawMaterialID == id).OrderByDescending(x => x.RMInventoryID).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<RawMaterialInventoryMaster, RMInventory>(obj);
                    o.PalletNo = obj.PalletMaster.PalletNo;
                    o.RawMaterialName = obj.RawMaterialMaster.RawMaterialName;

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
            var returnValue = new RMInventoryResponse();

            try
            {
                #region get all Pallets for ddl
                var Pallets = db.PalletMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.PalletNo).ToList();
                List<KPILib.Models.PalletMaster> pallets = new List<KPILib.Models.PalletMaster>();
                foreach (var obj in Pallets)
                {
                    var o = mapper.Map<PalletMaster, KPILib.Models.PalletMaster>(obj);

                    pallets.Add(o);
                }
                #endregion

                #region get all RawMaterials for ddl
                var RawMaterials = db.RawMaterialMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.RawMaterialName).ToList();
                List<KPILib.Models.RawMaterial> rms = new List<KPILib.Models.RawMaterial>();
                foreach (var obj in RawMaterials)
                {
                    var o = mapper.Map<RawMaterialMaster, KPILib.Models.RawMaterial>(obj);

                    rms.Add(o);
                }
                #endregion

                #region get all TagColours for ddl
                var Tags = db.TagColourMasters.Where(x => !x.IsDiscontinued).ToList();
                List<KPILib.Models.TagColor> tags = new List<KPILib.Models.TagColor>();
                foreach (var obj in Tags)
                {
                    var o = mapper.Map<TagColourMaster, KPILib.Models.TagColor>(obj);

                    tags.Add(o);
                }
                #endregion

                var data = db.RawMaterialInventoryMasters.SingleOrDefault(x => x.RMInventoryID == id);
                if (data != null)
                {
                    var o = mapper.Map<RawMaterialInventoryMaster, RMInventory>(data);
                    o.PalletNo = data.PalletMaster.PalletNo;
                    o.Pallets = pallets;
                    o.RawMaterialName = data.RawMaterialMaster.RawMaterialName;
                    o.RawMaterials = rms;
                    o.TagColours = tags;

                    returnValue.data = o;
                }
                else
                {
                    var o = new RMInventory() { Pallets = pallets, RawMaterials = rms, TagColours = tags };
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

        public IHttpActionResult Add(RMInventory data)
        {
            var returnValue = new RMInventoryResponse();

            try
            {
                var o = mapper.Map<RMInventory, RawMaterialInventoryMaster>(data);
                o.AddedOn = DateTime.Now;
                //o.LastModifiedOn = DateTime.Now;

                db.RawMaterialInventoryMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.RMInventoryID = o.RMInventoryID;

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Edit(RMInventory data)
        {
            var returnValue = new RMInventoryResponse();

            try
            {
                var o = db.RawMaterialInventoryMasters.SingleOrDefault(x => x.RMInventoryID == data.RMInventoryID);
                if (o != null)
                {
                    o.PalletID = data.PalletID;
                    o.RawMaterialID = data.RawMaterialID;
                    o.Qty = data.Qty;
                    o.LastModifiedOn = DateTime.Now;
                    o.ModifiedBy = data.ModifiedBy;

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

    }
}
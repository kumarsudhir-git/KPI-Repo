using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Moulds")]
    public class MouldAPIController : CCSPLBaseAPIController
    {
        public IHttpActionResult GetAll()
        {
            var returnValue = new MouldMastersResponse();

            try
            {
                var data = db.MouldMasters.Where(x => !x.IsDiscontinued).OrderByDescending(x => x.LastModifiedOn != null ? x.LastModifiedOn : x.AddedOn).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<MouldMaster, KPILib.Models.Mould>(obj);

                    if (o.LocationId != null)
                    {
                        LocationMaster locationMaster = CommonFunctions.GetLocationMaster((int)o.LocationId);
                        if (locationMaster != null)
                        {
                            o.LocationName = locationMaster.LocationName;
                        }
                    }
                    foreach (var product in obj.ProductMasters.ToList())
                    {
                        o.AllProducts += "," + mapper.Map<ProductMaster, KPILib.Models.Product>(product).ProductName;
                        o.Products.Add(mapper.Map<ProductMaster, KPILib.Models.Product>(product));
                    }
                    if (o.AllProducts != null)
                        if (o.AllProducts.Length > 0)
                            o.AllProducts = o.AllProducts.Substring(1);

                    foreach (var machine in obj.MachineMouldMappings.ToList())
                    {
                        o.AllMachines += "," + mapper.Map<MachineMaster, KPILib.Models.Machine>(machine.MachineMaster).MachineName;
                        o.Machines.Add(mapper.Map<MachineMaster, KPILib.Models.Machine>(machine.MachineMaster));
                    }
                    if (o.AllMachines != null)
                        if (o.AllMachines.Length > 0)
                            o.AllMachines = o.AllMachines.Substring(1);

                    if (obj.MouldHistories.LastOrDefault() != null)
                    {
                        o.MouldStatusID = obj.MouldHistories.LastOrDefault().MouldStatusID;
                        o.MouldStatus = ((enumMouldStatus)obj.MouldHistories.LastOrDefault().MouldStatusID).ToString() + (((enumMouldStatus)obj.MouldHistories.LastOrDefault().MouldStatusID) == enumMouldStatus.InUse ? ": " + obj.MouldHistories.LastOrDefault().Description : "");
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
        public IHttpActionResult Get(int id)
        {
            MouldMasterResponse returnValue = new MouldMasterResponse();

            try
            {
                returnValue.MouldTypeMasters = mapper.Map<List<MouldTypeMaster>, List<KPILib.Models.MouldTypeMaster>>(CommonFunctions.GetMouldTypeMasterListData());

                MouldMaster data = GetMouldMasterData(id);
                if (data != null)
                {
                    var o = mapper.Map<MouldMaster, KPILib.Models.Mould>(data);

                    //o.RawMaterial = data.RawMaterialMaster.RawMaterialName;

                    returnValue.data = o;
                }
                else
                {
                    var o = new KPILib.Models.Mould();
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

        public IHttpActionResult Add(KPILib.Models.Mould data)
        {
            MouldMasterResponse returnValue = new MouldMasterResponse();

            try
            {
                if (data.MouldID == 0)
                {
                    MouldMaster o = mapper.Map<KPILib.Models.Mould, MouldMaster>(data);

                    o.AddedOn = DateTime.Now;
                    o.LastModifiedOn = DateTime.Now;

                    db.MouldMasters.Add(o);
                    db.SaveChanges();

                    returnValue.data = data;
                    returnValue.data.MouldID = o.MouldID;

                    returnValue.Response.IsSuccessful();
                }
                else
                {
                    MouldMaster o = GetMouldMasterData(data.MouldID);
                    if (o != null)
                    {
                        o.MouldName = data.MouldName;
                        o.Description = data.Description;
                        o.MouldTypeID = data.MouldTypeID; //TODO: Default UOM              //data.UOMID;
                        o.LocationId = data.LocationId;
                        o.TotalCavities = data.TotalCavities;
                        o.RunningCavities = data.RunningCavities;
                        o.CorePins = data.CorePins;
                        o.LastModifiedOn = DateTime.Now;

                        db.Entry(o).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    returnValue.data = data;
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
        public IHttpActionResult Edit(KPILib.Models.Mould data)
        {
            MouldMasterResponse returnValue = new MouldMasterResponse();

            try
            {
                MouldMaster o = GetMouldMasterData(data.MouldID);
                if (o != null)
                {
                    o.MouldName = data.MouldName;
                    o.Description = data.Description;
                    o.MouldTypeID = data.MouldTypeID; //TODO: Default UOM              //data.UOMID;
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

        public IHttpActionResult Delete(int id)
        {
            MouldMasterResponse returnValue = new MouldMasterResponse();

            try
            {
                MouldMaster o = GetMouldMasterData(id);
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

        private MouldMaster GetMouldMasterData(int MouldId)
        {
            MouldMaster mouldMaster = db.MouldMasters.SingleOrDefault(x => x.MouldID == MouldId);
            return mouldMaster;
        }

    }
}

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
                        o.AllMachines += "," + mapper.Map<MachineMaster, KPILib.Models.MachineMasterModel>(machine.MachineMaster).MachineName;
                        o.Machines.Add(mapper.Map<MachineMaster, KPILib.Models.MachineMasterModel>(machine.MachineMaster));
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
                CommonLogger.Error(ex, ex.Message);
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
                CommonLogger.Error(ex, ex.Message);
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
                CommonLogger.Error(ex, ex.Message);
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
                CommonLogger.Error(ex, ex.Message);
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
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }

        private MouldMaster GetMouldMasterData(int MouldId)
        {
            MouldMaster mouldMaster = db.MouldMasters.SingleOrDefault(x => x.MouldID == MouldId);
            return mouldMaster;
        }

        [HttpGet]
        public IHttpActionResult GetMouldMasterListData(int MouldId = 0)
        {
            MouldMastersResponse returnValue = new MouldMastersResponse();
            try
            {
                List<MouldMaster> machineMaster = CommonFunctions.GetMouldMasters(MouldId);
                if (machineMaster != null)
                {
                    returnValue.data = mapper.Map<List<Mould>>(machineMaster);
                    returnValue.Response.IsSuccessful();
                }
                else
                {
                    returnValue.Response.ResponseCode = 404;
                    returnValue.Response.ResponseMsg = "Machine not found.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Response.ResponseCode = 500;
                returnValue.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(returnValue);
        }

        [HttpGet]
        public IHttpActionResult GetMouldMachineMappedData(int MouldId = 0)
        {
            MouldMastersResponse returnValue = new MouldMastersResponse();
            try
            {
                List<MachineMouldMapping> machineMouldMappings = CommonFunctions.GetMouldMachineMappedData(MouldId);

                returnValue.mouldMachineMapData = ConvertToMouldMachineMapDto(machineMouldMappings);

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                returnValue.Response.ResponseCode = 500;
                returnValue.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(returnValue);
        }

        [HttpPost]
        public IHttpActionResult MapMouldMachine(List<KPILib.Models.MouldMachineMapping> mouldMachineMapping)
        {
            MouldMastersResponse returnValue = new MouldMastersResponse();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (mouldMachineMapping == null || mouldMachineMapping.Count == 0)
                    {
                        returnValue.Response.ResponseCode = 400;
                        returnValue.Response.ResponseMsg = "Invalid Machine or Mould ID.";
                        return Json(returnValue);
                    }

                    foreach (var item in mouldMachineMapping)
                    {
                        if (item.MouldID != 0)
                        {
                            List<MachineMouldMapping> machineMouldMappings = CommonFunctions.GetMouldMachineMappedData(item.MouldID);

                            if (machineMouldMappings != null && machineMouldMappings.Count > 0)
                            {
                                machineMouldMappings.ForEach(z =>
                                {
                                    z.IsDiscontinued = true;
                                    z.ModifiedBy = item.UserID;
                                    z.LastModifiedOn = DateTime.Now;
                                    db.Entry(z).State = System.Data.Entity.EntityState.Modified;
                                });
                            }

                            item.MachineID.ForEach(x =>
                            {
                                MachineMouldMapping machineMouldMap = new MachineMouldMapping
                                {
                                    MachineID = x,
                                    MouldID = item.MouldID,
                                    AddedOn = DateTime.Now,
                                    AddedBy = item.UserID
                                };
                                db.MachineMouldMappings.Add(machineMouldMap);
                            });
                        }
                    }
                    db.SaveChanges();
                    returnValue.Response.IsSuccessful();
                    scope.Commit();
                }
                catch (Exception ex)
                {
                    returnValue.Response.ResponseCode = 500;
                    returnValue.Response.ResponseMsg = ex.Message;
                    CommonLogger.Error(ex, ex.Message);
                    scope.Rollback();
                }
            }
            return Json(returnValue);
        }

        private List<MouldMachineMapping> ConvertToMouldMachineMapDto(List<MachineMouldMapping> flatList)
        {
            var result = flatList
                .Where(x => x.MouldID.HasValue) // Optional: filter out nulls
                .GroupBy(x => x.MouldID)
                .Select(group => new KPILib.Models.MouldMachineMapping
                {
                    MouldID = group.Key.Value,
                    MachineID = group.Select(x => x.MachineID).Distinct().ToList()
                })
                .ToList();

            return result;
        }

    }
}

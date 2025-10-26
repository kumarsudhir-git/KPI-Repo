using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Machines")]
    public class MachineAPIController : CCSPLBaseAPIController
    {
        public IHttpActionResult GetAll()
        {
            MachineMastersResponse returnValue = new MachineMastersResponse();

            try
            {
                var data = db.MachineMasters.Where(x => !x.IsDiscontinued).OrderByDescending(x => x.LastModifiedOn != null ? x.LastModifiedOn : x.AddedOn).ToList();
                foreach (var obj in data)
                {
                    MachineMasterModel machineMasterModel = mapper.Map<MachineMaster, KPILib.Models.MachineMasterModel>(obj);

                    if (obj.MachineHistories.LastOrDefault() != null)
                    {
                        machineMasterModel.MachineStatusID = obj.MachineHistories.LastOrDefault().MachineStatusID;
                        machineMasterModel.MachineStatus = ((enumMachineStatus)obj.MachineHistories.LastOrDefault().MachineStatusID).ToString() + (((enumMachineStatus)obj.MachineHistories.LastOrDefault().MachineStatusID) == enumMachineStatus.InUse ? ": " + obj.MachineHistories.LastOrDefault().Description : "");
                    }
                    machineMasterModel.MachineTypeName = CommonFunctions.GetMachineTypeMasterData(obj.MachineTypeID)?.MachineType;
                    if (obj.VendorId != null)
                    {
                        machineMasterModel.VendorName = CommonFunctions.GetVendorDetailsFromId((int)obj.VendorId)?.VendorName;
                    }
                    if (obj.LocationId != null)
                    {
                        machineMasterModel.LocationName = CommonFunctions.GetLocationMaster((int)obj.LocationId)?.LocationName;
                    }

                    returnValue.data.Add(machineMasterModel);
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

        [HttpGet]
        public IHttpActionResult AddMachine(int MachineId = 0)
        {
            MachineMasterResponse returnValue = new MachineMasterResponse();

            try
            {
                MachineMaster machineMaster = CommonFunctions.GetMachineMasterData(MachineId);

                returnValue.data = mapper.Map<MachineMasterModel>(machineMaster);

                if (MachineId != 0)
                {
                    int? MachineStatusId = CommonFunctions.GetMachineStatusData(MachineId)?.MachineStatusID;
                    if (MachineStatusId != null && MachineStatusId != 0)
                    {
                        returnValue.data.MachineStatusID = (int)MachineStatusId;
                        returnValue.data.MachineStatus = Enum.GetName(typeof(enumMachineStatus), returnValue.data.MachineStatusID);
                    }
                }

                List<MachineTypeMaster> machineTypeList = CommonFunctions.GetMachineTypeMasterList();

                returnValue.machineTypeMasterData = mapper.Map<List<KPILib.Models.MachineTypeMasterModel>>(machineTypeList);

                List<MachineStatusMaster> machineStatusList = CommonFunctions.GetMachineStatusMasterList();

                returnValue.machineStatusMasterData = mapper.Map<List<KPILib.Models.MachineStatusMasterModel>>(machineStatusList);

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
        public IHttpActionResult AddMachine(MachineMasterModel machineMasterModel)
        {
            MachineMasterResponse returnValue = new MachineMasterResponse();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (machineMasterModel.MachineID == 0)
                    {
                        MachineMaster machineMasterObj = mapper.Map<MachineMasterModel, MachineMaster>(machineMasterModel);

                        machineMasterObj.AddedOn = DateTime.Now;

                        db.MachineMasters.Add(machineMasterObj);
                        db.SaveChanges();
                        machineMasterModel.MachineID = machineMasterObj.MachineID;
                    }
                    else
                    {
                        MachineMaster machineMaster = CommonFunctions.GetMachineMasterData(machineMasterModel.MachineID);

                        if (machineMaster != null)
                        {
                            machineMaster.MachineName = machineMasterModel.MachineName;
                            machineMaster.Description = machineMasterModel.Description;
                            machineMaster.MachineTypeID = machineMasterModel.MachineTypeID;
                            machineMaster.VendorId = machineMasterModel.VendorId;
                            machineMaster.LocationId = machineMasterModel.LocationId;
                            machineMaster.IsDiscontinued = machineMasterModel.IsDiscontinued;
                            machineMaster.LastModifiedOn = DateTime.Now;

                            db.Entry(machineMaster).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //Update MachineHistory
                    AddUpdateMachineHistory(machineMasterModel);
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

        private void AddUpdateMachineHistory(MachineMasterModel machineMasterModel)
        {
            if (DeleteMachineHistory(machineMasterModel.MachineID))
            {
                MachineHistory machineHistory = new MachineHistory()
                {
                    MachineID = machineMasterModel.MachineID,
                    MachineStatusID = machineMasterModel.MachineStatusID,
                    Description = machineMasterModel.MachineStatus,
                    IsDeleted = false,
                    AddedOn = DateTime.Now
                };
                db.MachineHistories.Add(machineHistory);
                db.SaveChanges();
            }
        }
        private bool DeleteMachineHistory(int MachineId)
        {
            bool isSuccess = false;

            List<MachineHistory> machineHistories = CommonFunctions.GetMachineHistoriesData(MachineId);
            machineHistories.ForEach(item =>
            {
                item.IsDeleted = false;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            });
            db.SaveChanges();
            isSuccess = true;
            return isSuccess;
        }

        [HttpPost]
        public IHttpActionResult DeleteMachine(MachineMasterModel machineMasterModel)
        {
            MachineMasterResponse returnValue = new MachineMasterResponse();

            try
            {
                MachineMaster machineMaster = CommonFunctions.GetMachineMasterData(machineMasterModel.MachineID);

                if (machineMaster != null)
                {
                    machineMaster.IsDiscontinued = true;
                    machineMaster.LastModifiedOn = DateTime.Now;

                    db.Entry(machineMaster).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                DeleteMachineHistory(machineMasterModel.MachineID);
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

        [HttpGet]
        public IHttpActionResult GetMachineTypeMasterList()
        {
            MachineTypeMasterResponse machineTypeMasterResponse = new MachineTypeMasterResponse();

            try
            {
                List<MachineTypeMaster> lookUps = CommonFunctions.GetMachineTypeMasterList();

                machineTypeMasterResponse.machineTypeMasterData = mapper.Map<List<KPILib.Models.MachineTypeMasterModel>>(lookUps);

                machineTypeMasterResponse.Response.IsSuccessful();

            }
            catch (Exception ex)
            {
                machineTypeMasterResponse.Response.ResponseCode = 500;
                machineTypeMasterResponse.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(machineTypeMasterResponse);
        }

        [HttpGet]
        public IHttpActionResult GetMachineStatusMasterList()
        {
            MachineStatusMasterResponse machineTypeMasterResponse = new MachineStatusMasterResponse();

            try
            {
                List<MachineStatusMaster> lookUps = CommonFunctions.GetMachineStatusMasterList();

                machineTypeMasterResponse.machineStatusMasterData = mapper.Map<List<KPILib.Models.MachineStatusMasterModel>>(lookUps);

                machineTypeMasterResponse.Response.IsSuccessful();

            }
            catch (Exception ex)
            {
                machineTypeMasterResponse.Response.ResponseCode = 500;
                machineTypeMasterResponse.Response.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }
            return Json(machineTypeMasterResponse);
        }

        [HttpGet]
        public IHttpActionResult GetMachineMasterData(int MachineId = 0)
        {
            MachineMastersResponse returnValue = new MachineMastersResponse();
            try
            {
                List<MachineMaster> machineMaster = CommonFunctions.GetMachineMasters(MachineId);
                if (machineMaster != null)
                {
                    returnValue.data = mapper.Map<List<MachineMasterModel>>(machineMaster);
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
        public IHttpActionResult GetAllMachineMouldMappedData(string MapType = "Machine")
        {
            MachineMouldMappingResponse returnValue = new MachineMouldMappingResponse();
            try
            {
                List<usp_GetMachineMouldMapData_Result> machineMouldMappings = CommonFunctions.GetMachineMouldMappingsData(MapType);
                returnValue.data = mapper.Map<List<MachineMouldMappingModel>>(machineMouldMappings);
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

        [HttpGet]
        public IHttpActionResult GetMachineMouldMappedData(int MachineId = 0)
        {
            MachineMouldMappingResponse returnValue = new MachineMouldMappingResponse();
            try
            {
                List<MachineMouldMapping> machineMouldMappings = CommonFunctions.GetMachineMouldMappedData(MachineId);

                returnValue.machineToMouldMap = ConvertToMachineMouldMapDto(machineMouldMappings);

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
        public IHttpActionResult MapMachineMould(List<KPILib.Models.MachineMouldMapping> machineMouldMapping)
        {
            MachineMouldMappingResponse returnValue = new MachineMouldMappingResponse();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (machineMouldMapping == null || machineMouldMapping.Count == 0)
                    {
                        returnValue.Response.ResponseCode = 400;
                        returnValue.Response.ResponseMsg = "Invalid Machine or Mould ID.";
                        return Json(returnValue);
                    }

                    foreach (var item in machineMouldMapping)
                    {
                        if (item.MachineID != 0)
                        {
                            List<MachineMouldMapping> machineMouldMappings = CommonFunctions.GetMachineMouldMappedData(item.MachineID);

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

                            item.MouldID.ForEach(x =>
                            {
                                MachineMouldMapping machineMouldMap = new MachineMouldMapping
                                {
                                    MachineID = item.MachineID,
                                    MouldID = x,
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

        [HttpPost]
        public IHttpActionResult DeleteMachineMouldMapping(int MachineMouldMappingId, int UserId)
        {
            MachineMouldMappingResponse returnValue = new MachineMouldMappingResponse();
            try
            {
                MachineMouldMapping machineMouldMapping = CommonFunctions.GetMachineMouldMappingData(MachineMouldMappingId);
                if (machineMouldMapping != null)
                {
                    machineMouldMapping.IsDiscontinued = true;
                    machineMouldMapping.ModifiedBy = UserId;
                    machineMouldMapping.LastModifiedOn = DateTime.Now;
                    db.Entry(machineMouldMapping).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    returnValue.Response.IsSuccessful();
                }
                else
                {
                    returnValue.Response.ResponseCode = 404;
                    returnValue.Response.ResponseMsg = "Machine Mould Mapping not found.";
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

        private List<KPILib.Models.MachineMouldMapping> ConvertToMachineMouldMapDto(List<MachineMouldMapping> flatList)
        {
            List<KPILib.Models.MachineMouldMapping> result = flatList
                .Where(x => x.MouldID.HasValue) // Optional: filter out nulls
                .GroupBy(x => x.MachineID)
                .Select(group => new KPILib.Models.MachineMouldMapping
                {
                    MachineID = group.Key,
                    MouldID = group.Select(x => x.MouldID.Value).Distinct().ToList()
                })
                .ToList();

            return result;
        }

    }
}

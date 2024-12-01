using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
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
                    returnValue.data.Add(machineMasterModel);
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
            }
            return Json(machineTypeMasterResponse);
        }

    }
}

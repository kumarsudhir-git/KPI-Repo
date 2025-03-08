using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Location")]
    public class LocationMasterAPIController : CCSPLBaseAPIController
    {
        // GET: CompanyAPI

        #region
        public IHttpActionResult GetAllLocation()
        {
            LocationMasterResponse returnValue = new LocationMasterResponse();

            try
            {
                List<usp_GetLocationMasterAllData_Result> locationMasters = db.usp_GetLocationMasterAllData().ToList();

                returnValue.data = mapper.Map<List<usp_GetLocationMasterAllData_Result>, List<LocationMasterModel>>(locationMasters);

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
        public IHttpActionResult GetLocation(int LocationId)
        {
            LocationMasterResponse returnValue = new LocationMasterResponse();

            try
            {
                LocationMaster locationMasterObj = GetLocationMaster(LocationId);
                returnValue.LocationMasterModel = mapper.Map<LocationMaster, LocationMasterModel>(locationMasterObj);
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

        public IHttpActionResult SaveLocation(LocationMasterModel locationMasterModel)
        {
            LocationMasterResponse returnValue = new LocationMasterResponse();

            try
            {
                int locationNameCount = CountDuplicateLocationName(locationMasterModel);

                if (locationNameCount > 0)
                {
                    returnValue.Response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    returnValue.Response.ResponseMsg = "LocationName already exist";
                    return Json(returnValue);
                }

                LocationMaster locationMasterObj = mapper.Map<LocationMasterModel, LocationMaster>(locationMasterModel);

                if (locationMasterModel.LocationId == 0)
                {
                    locationMasterObj.AddedOn = DateTime.Now;
                    locationMasterObj.IsActive = true;

                    db.LocationMasters.Add(locationMasterObj);
                }
                else
                {
                    LocationMaster locationObj = GetLocationMaster(locationMasterModel.LocationId);
                    locationObj.LocationName = locationMasterModel.LocationName;
                    locationObj.ModifiedOn = DateTime.Now;
                    locationObj.ModifiedBy = locationMasterModel.ModifiedBy;

                    db.Entry(locationObj).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges();

                returnValue.Response.ResponseData = locationMasterModel;

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

        public IHttpActionResult DeleteLocation(int LocationId)
        {
            LocationMasterResponse returnValue = new LocationMasterResponse();

            try
            {
                var o = db.LocationMasters.SingleOrDefault(x => x.LocationId == LocationId);
                if (o != null)
                {
                    db.Entry(o).State = System.Data.Entity.EntityState.Deleted;

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

        public IHttpActionResult GetLocationMasterData()
        {
            LocationMasterResponse locationMasterResponse = new LocationMasterResponse();

            List<LocationMaster> locationMasters = (from LM in db.LocationMasters
                                                    where LM.IsActive == true
                                                    select LM).ToList();

            locationMasterResponse.data = mapper.Map<List<LocationMaster>, List<LocationMasterModel>>(locationMasters);
            locationMasterResponse.Response.IsSuccessful();
            return Json(locationMasterResponse);
        }

        public IHttpActionResult ValidateLocationName(LocationMasterModel locationMasterModel)
        {
            LocationMasterResponse locationMasterResponse = new LocationMasterResponse();

            int locationNameCount = CountDuplicateLocationName(locationMasterModel);
            if (locationNameCount > 0)
            {
                locationMasterResponse.Response.ResponseCode = (int)HttpStatusCode.BadRequest;
                locationMasterResponse.Response.ResponseMsg = "LocationName already exist";
            }
            else
            {
                locationMasterResponse.Response.IsSuccessful();
            }
            return Json(locationMasterResponse);
        }

        private LocationMaster GetLocationMaster(int LocationId)
        {
            return db.LocationMasters.Where(x => x.LocationId == LocationId).FirstOrDefault();
        }

        private int CountDuplicateLocationName(LocationMasterModel locationMasterModel)
        {
            int countDuplicate = (from LM in db.LocationMasters
                                  where LM.LocationName.Trim().ToLower() == locationMasterModel.LocationName.Trim().ToLower()
                                  && LM.LocationId != locationMasterModel.LocationId
                                  && LM.IsActive == true
                                  select LM.LocationId).Count();
            return countDuplicate;
        }

        #endregion
    }
}

using KPILib.Models;
using KPIWebAPI.AuthFilters;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System.Net;
using System.Data.Entity;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Vendors")]
    public class VendorMasterAPIController : CCSPLBaseAPIController
    {
        static object myLock = new object();

        // GET: VendorMasterAPI
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public IHttpActionResult GetAll()
        {
            VendorMasterModelResponse returnValue = new VendorMasterModelResponse();
            try
            {
                List<VendorMaster> vendorMasters = (from VndrMstr in db.VendorMasters
                                                    where VndrMstr.IsDiscontinued == false
                                                    select VndrMstr).ToList();

                returnValue.data = mapper.Map<List<VendorMaster>, List<KPILib.Models.VendorMasterModel>>(vendorMasters);
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                returnValue.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                returnValue.Response.ResponseMsg = ex.Message;
            }
            return Json(returnValue);
        }

        [HttpGet]
        public IHttpActionResult GetVendor(int id = 0)
        {
            VendorMasterModelResponse vendorMasterModelResponse = new VendorMasterModelResponse();
            try
            {
                if (id != 0)
                {
                    VendorMaster vendorMaster = GetVendorMaster(id);
                    if (vendorMaster != null)
                    {
                        vendorMasterModelResponse.vendorMaster = mapper.Map<VendorMaster, KPILib.Models.VendorMasterModel>(vendorMaster);
                    }
                }
                vendorMasterModelResponse.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                vendorMasterModelResponse.Response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                vendorMasterModelResponse.Response.ResponseMsg = ex.Message;
            }
            return Json(vendorMasterModelResponse);
        }

        [HttpPost]
        public IHttpActionResult AddNew(VendorMasterModel vendorMasterModel)
        {
            VendorMasterModelResponse vendorMasterModelResponse = new VendorMasterModelResponse();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (vendorMasterModel.VendorId == 0)
                        {
                            VendorMaster vendorMaster = mapper.Map<VendorMasterModel, VendorMaster>(vendorMasterModel);
                            vendorMaster.AddedOn = DateTime.Now;
                            db.VendorMasters.Add(vendorMaster);
                        }
                        else
                        {
                            VendorMaster vendorMaster = GetVendorMaster(vendorMasterModel.VendorId);
                            if (vendorMaster != null)
                            {
                                //vendorMaster = mapper.Map<VendorMasterModel, VendorMaster>(vendorMasterModel);
                                vendorMaster.VendorName = vendorMasterModel.VendorName;
                                vendorMaster.Notes = vendorMasterModel.Notes;
                                vendorMaster.ContactNumber = vendorMasterModel.ContactNumber;
                                vendorMaster.Address = vendorMasterModel.Address;
                                vendorMaster.LastModifiedBy = vendorMasterModel.LastModifiedBy;
                                vendorMaster.LastModifiedOn = DateTime.Now;
                                db.Entry(vendorMaster).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();

                        transaction.Commit();

                        vendorMasterModelResponse.Response.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        //TODO error handling
                        vendorMasterModelResponse.Response.ResponseMsg = ex.Message;
                    }
                }
            }
            return Json(vendorMasterModelResponse);
        }

        [HttpPost]
        public IHttpActionResult DeleteVendor(VendorMasterModel vendorMasterModel)
        {
            VendorMasterModelResponse vendorMasterModelResponse = new VendorMasterModelResponse();
            lock (myLock)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        VendorMaster vendorMasterObj = GetVendorMaster(vendorMasterModel.VendorId);
                        if (vendorMasterObj != null)
                        {
                            vendorMasterObj.IsDiscontinued = true;
                            vendorMasterObj.LastModifiedBy = vendorMasterModel.LastModifiedBy;
                            vendorMasterObj.LastModifiedOn = DateTime.Now;
                            db.Entry(vendorMasterObj).State = EntityState.Modified;

                            db.SaveChanges();
                        }
                        transaction.Commit();

                        vendorMasterModelResponse.Response.IsSuccessful();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        //TODO error handling
                        vendorMasterModelResponse.Response.ResponseMsg = ex.Message;
                    }
                }
            }
            return Json(vendorMasterModelResponse);
        }

        private VendorMaster GetVendorMaster(int VendorId)
        {
            VendorMaster vendorMaster = (from vm in db.VendorMasters
                                         where vm.VendorId == VendorId
                                         select vm).FirstOrDefault();
            return vendorMaster;
        }

    }
}
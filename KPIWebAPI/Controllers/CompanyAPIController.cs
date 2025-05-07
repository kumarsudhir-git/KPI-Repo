using KPILib.Models;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    public class CompanyAPIController : CCSPLBaseAPIController
    {
        // GET: CompanyAPI

        #region
        public IHttpActionResult GetAll()
        {
            var returnValue = new CompaniesResponse();

            try
            {
                //List<CompanyMaster> data = db.CompanyMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.CompanyName).ToList();
                List<CompanyMaster> data = (from CM in db.CompanyMasters
                                            where CM.CompanyTypeID == 103 //Harcoded value for CompanyTypeId
                                            && CM.IsDiscontinued == false
                                            orderby (CM.LastModifiedOn != null && CM.LastModifiedOn != DateTime.MinValue ? CM.LastModifiedOn : CM.AddedOn) descending
                                            select CM).ToList();
                foreach (var obj in data)
                {
                    Company companyObj = mapper.Map<CompanyMaster, Company>(obj);
                    companyObj.LocationCount = obj.CompanyLocationMasters.Where(x => !x.IsDiscontinued).Count();

                    returnValue.data.Add(companyObj);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                CommonLogger.Error(ex, ex.Message);
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            var returnValue = new CompanyResponse();

            try
            {
                var data = CommonFunctions.GetCompanyMasterById(id);//db.CompanyMasters.Where(x => x.CompanyID == id && x.IsDiscontinued == false).FirstOrDefault();
                if (data != null)
                {
                    var o = mapper.Map<CompanyMaster, KPILib.Models.Company>(data);
                    returnValue.data = o;
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                CommonLogger.Error(ex, ex.Message);
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Add(KPILib.Models.Company data)
        {
            var returnValue = new CompanyResponse();

            try
            {
                var o = mapper.Map<KPILib.Models.Company, CompanyMaster>(data);
                o.AddedOn = DateTime.Now;
                o.CompanyTypeID = 103; //Harcoded value for CompanyTypeId
                //o.LastModifiedOn = DateTime.Now;

                db.CompanyMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.CompanyID = o.CompanyID;

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                CommonLogger.Error(ex, ex.Message);
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Edit(KPILib.Models.Company data)
        {
            var returnValue = new CompanyResponse();

            try
            {
                var o = db.CompanyMasters.SingleOrDefault(x => x.CompanyID == data.CompanyID);
                if (o != null)
                {
                    o.CompanyName = data.CompanyName;
                    o.Notes = data.Notes;
                    o.IsDiscontinued = data.IsDiscontinued;
                    o.LastModifiedOn = DateTime.Now;
                    o.CompanyTypeID = 103;//Harcoded value for CompanyTypeId

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }

                returnValue.data = data;
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                CommonLogger.Error(ex, ex.Message);
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Delete(KPILib.Models.Company data)
        {
            var returnValue = new CompanyResponse();

            try
            {
                var o = db.CompanyMasters.SingleOrDefault(x => x.CompanyID == data.CompanyID);
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
                CommonLogger.Error(ex, ex.Message);
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Delete(int id)
        {
            var returnValue = new CompanyResponse();

            try
            {
                var o = db.CompanyMasters.SingleOrDefault(x => x.CompanyID == id);
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
                CommonLogger.Error(ex, ex.Message);
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult GetCompanyMasterList()
        {
            CompaniesResponse returnValue = new CompaniesResponse();
            try
            {
                List<CompanyMaster> companyMasterListData = CommonFunctions.GetCompanyMasterList();

                returnValue.data = mapper.Map<List<Company>>(companyMasterListData);

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

        #endregion
    }
}

using KPILib.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    public class CompanyAPIController : CCSPLBaseAPIController
    {
        // GET: CompanyAPI

        #region
        public IHttpActionResult GetAll(int id)
        {
            var returnValue = new CompaniesResponse();

            try
            {
                var data = db.CompanyMasters.Where(x => !x.IsDiscontinued && x.CompanyTypeID == id).OrderBy(x => x.CompanyName).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<CompanyMaster, KPILib.Models.Company>(obj);
                    o.LocationCount = obj.CompanyLocationMasters.Where(x => !x.IsDiscontinued).Count();

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
            var returnValue = new CompanyResponse();

            try
            {
                var data = db.CompanyMasters.SingleOrDefault(x => x.CompanyID == id);
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
                o.LastModifiedOn = DateTime.Now;

                db.CompanyMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.CompanyID = o.CompanyID;

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
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
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        #endregion
    }
}

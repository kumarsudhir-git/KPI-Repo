using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace KPIWebAPI.Controllers
{
    public class CompanyLocationAPIController : CCSPLBaseAPIController
    {
        // GET: CompanyLocationAPI
        #region
        public IHttpActionResult GetAll(int id)
        {
            var returnValue = new CompanyLocationsResponse();

            try
            {
                var comp = db.CompanyMasters.SingleOrDefault(x => x.CompanyID == id);   // && !x.IsDiscontinued
                if (comp.CompanyID != 0)
                {
                    returnValue.comp = mapper.Map<CompanyMaster, KPILib.Models.Company>(comp);
                    foreach (var obj in comp.CompanyLocationMasters.Where(x => !x.IsDiscontinued))
                    {
                        var o = mapper.Map<CompanyLocationMaster, KPILib.Models.CompanyLocation>(obj);

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
            var returnValue = new CompanyLocationResponse();

            try
            {
                var location = db.CompanyLocationMasters.SingleOrDefault(x => x.CompanyLocationID == id);   // && !x.IsDiscontinued
                returnValue.data = mapper.Map<CompanyLocationMaster, KPILib.Models.CompanyLocation>(location);
                if (location.CompanyLocationID != 0)
                {
                    var comp = db.CompanyMasters.SingleOrDefault(x => x.CompanyID == location.CompanyID);   // && !x.IsDiscontinued
                    if (comp.CompanyID != 0)
                    {
                        returnValue.comp = mapper.Map<CompanyMaster, KPILib.Models.Company>(comp);

                        //foreach (var obj in comp.CompanyLocationMasters.Where(x => !x.IsDiscontinued))
                        //{
                        //    var o = mapper.Map<CompanyLocationMaster, KPILib.Models.CompanyLocation>(obj);

                        //    returnValue.data.Add(o);
                        //}
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

        public IHttpActionResult Add(KPILib.Models.CompanyLocation data)
        {
            var returnValue = new CompanyLocationResponse();

            try
            {
                var o = mapper.Map<KPILib.Models.CompanyLocation, CompanyLocationMaster>(data);
                o.CompanyTypeIDs = "";
                o.AddedOn = DateTime.Now;
                o.LastModifiedOn = DateTime.Now;

                db.CompanyLocationMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.CompanyLocationID = o.CompanyLocationID;

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Edit(KPILib.Models.CompanyLocation data)
        {
            var returnValue = new CompanyLocationResponse();

            try
            {
                var o = db.CompanyLocationMasters.SingleOrDefault(x => x.CompanyLocationID == data.CompanyLocationID);
                if (o != null)
                {
                    o.Address = data.Address;
                    o.City = data.City;
                    o.CompanyTypeIDs = "-";  // data.CompanyTypeIDs;
                    o.ContactPerson = data.ContactPerson;
                    o.Country = data.Country;
                    o.Email = data.Email;
                    o.GSTN = data.GSTN;
                    o.IsDiscontinued = data.IsDiscontinued;
                    o.LocationName = data.LocationName;
                    o.Mobile = data.Mobile;
                    o.Notes = data.Notes;
                    o.PAN = data.PAN;
                    o.Phone = data.Phone;
                    o.PostalCode = data.PostalCode;
                    o.State = data.State;
                    o.Website = data.Website;
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

        //public IHttpActionResult Delete(KPILib.Models.Company data)
        //{
        //    var returnValue = new CompanyResponse();

        //    try
        //    {
        //        var o = db.CompanyMasters.SingleOrDefault(x => x.CompanyID == data.CompanyID);
        //        if (o != null)
        //        {
        //            o.IsDiscontinued = true;

        //            db.Entry(o).State = System.Data.Entity.EntityState.Modified;

        //            db.SaveChanges();
        //        }

        //        returnValue.data = null;
        //        returnValue.Response.IsSuccessful();
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO error handling
        //        returnValue.Response.ResponseMsg = ex.Message;
        //    }

        //    return Json(returnValue);
        //}


        //public IHttpActionResult Delete(int id)
        //{
        //    var returnValue = new CompanyResponse();

        //    try
        //    {
        //        var o = db.CompanyMasters.SingleOrDefault(x => x.CompanyID == id);
        //        if (o != null)
        //        {
        //            o.IsDiscontinued = true;

        //            db.Entry(o).State = System.Data.Entity.EntityState.Modified;

        //            db.SaveChanges();
        //        }

        //        returnValue.data = null;
        //        returnValue.Response.IsSuccessful();
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO error handling
        //        returnValue.Response.ResponseMsg = ex.Message;
        //    }

        //    return Json(returnValue);
        //}

        #endregion
    }
}
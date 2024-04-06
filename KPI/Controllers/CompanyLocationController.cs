using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [SessionCheckFilter]
    public class CompanyLocationController : CCSPLBaseController
    {
        // GET: CompanyLocation
        public ActionResult GetAll(int id)
        {
            var response = KPIAPIManager.GetAllCompanyLocations(id);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.Comp = response.comp;
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        public ActionResult Get(int id)
        {
            var response = KPIAPIManager.GetCompanyLocation(id);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.Comp = response.comp.CompanyName;
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        public ActionResult New(int id, string sCompanyName)
        {
            CompanyLocation location = new CompanyLocation() { CompanyID = id };
            ViewBag.Comp = sCompanyName;

            return View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPILib.Models.CompanyLocation location)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.AddCompanyLocation(location);
                if (response.Response.ResponseCode == 200)
                {
                    return RedirectToAction("GetAll", new { id = location.CompanyID });
                }
                else
                {
                    ViewBag.Error = response.Response.ResponseMsg;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Error = "Data provided is not valid";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KPILib.Models.CompanyLocation company)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.EditCompanyLocation(company);
                if (response.Response.ResponseCode == 200)
                {
                    return RedirectToAction("GetAll", new { id = company.CompanyID });
                }
                else
                {
                    ViewBag.Error = response.Response.ResponseMsg;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Error = "Data provided is not valid";
                return View("Error");
            }
        }
    }
}
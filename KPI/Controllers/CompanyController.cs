﻿using KPI.Classes;
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
    public class CompanyController : CCSPLBaseController
    {
        // GET: Company
        public ActionResult GetAll(int id)       //TODO: 101 = Raw Material Supplier, 103 = Customer
        {
            KPI.Classes.CommonFunctions.WriteToLog("success!");

            var response = KPIAPIManager.GetAllCompanies(id);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.TypeID = id;
                if (id == 101) ViewBag.Type = "Vendors";
                if (id == 103) ViewBag.Type = "Customers";

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
            var response = KPIAPIManager.GetCompany(id);
            if (response.Response.ResponseCode == 200)
            {
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        public ActionResult New(int id)     //TODO: 101 = Raw Material Supplier, 103 = Customer
        {
            Company company = new Company() { CompanyTypeID = id };   

            ViewBag.TypeID = id;
            if (id == 101) ViewBag.Type = "Vendor";
            if (id == 103) ViewBag.Type = "Customer";

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPILib.Models.Company company)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.AddCompany(company);
                if (response.Response.ResponseCode == 200)
                {
                    //return RedirectToAction("GetAll/" + company.CompanyTypeID.ToString());

                    return RedirectToAction("GetAll", "CompanyLocation", new { id = response.data.CompanyID });
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
        public ActionResult Edit(KPILib.Models.Company company)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.EditCompany(company);
                if (response.Response.ResponseCode == 200)
                {
                    return RedirectToAction("GetAll");
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
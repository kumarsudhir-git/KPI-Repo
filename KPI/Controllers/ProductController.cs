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
    [CustomAuthFilter("M_Products")]
    public class ProductController : CCSPLBaseController
    {
        // GET: RawMaterials
        [HttpGet]
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllProducts();
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

        [HttpGet]
        public ActionResult Get(int id)
        {
            var response = KPIAPIManager.GetProduct(id);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.UOMs = new SelectList(response.data.UOMs, "UOMID", "UnitsName");
                ViewBag.ProductCategories = new SelectList(response.data.ProductCategories, "ProductCategoryID", "ProductCategoryName");
                ViewBag.Moulds = new SelectList(response.data.Moulds, "MouldID", "MouldName");
                //ViewBag.RawMaterials = new SelectList(response.data.RawMaterials, "RawMaterialID", "RawMaterialName");
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }
        
        public ActionResult New()
        {
            var response = KPIAPIManager.GetProduct(0);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.UOMs = new SelectList(response.data.UOMs, "UOMID", "UnitsName");
                ViewBag.ProductCategories = new SelectList(response.data.ProductCategories, "ProductCategoryID", "ProductCategoryName");
                //ViewBag.RawMaterials = new SelectList(response.data.RawMaterials, "RawMaterialID", "RawMaterialName");
                ViewBag.Moulds = new SelectList(response.data.Moulds, "MouldID", "MouldName");
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult PartialProduct(ProductRawMaterialMapping productRawMaterialMapping)
        {
            ViewBag.RawMaterials = new SelectList(new List<SelectList>(), "RawMaterialID", "RawMaterialName");
            ViewBag.UnitMaster = new SelectList(new List<SelectList>(), "UnitID", "UnitName");

            var materialResponse = KPIAPIManager.GetRawMaterialData();
            if (materialResponse.Response.ResponseCode == 200)
            {
                ViewBag.RawMaterials = new SelectList(materialResponse.data, "RawMaterialID", "RawMaterialName");
            }

            var unitResponse = KPIAPIManager.GetUnitData();
            if (unitResponse.Response.ResponseCode == 200)
            {
                ViewBag.UnitMaster = new SelectList(unitResponse.data, "UnitID", "UnitName");
            }

            return PartialView("_PartialProduct", productRawMaterialMapping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(KPILib.Models.Product obj)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.AddProduct(obj);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KPILib.Models.Product obj)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.EditProduct(obj);
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
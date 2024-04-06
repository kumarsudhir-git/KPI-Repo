using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Sales")]
    public class SalesController : CCSPLBaseController
    {
        // GET: Sales
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllSales();
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

        public ActionResult GetAllByProductID(int id)
        {
            var response = KPIAPIManager.GetAllByProductID(id);
            if (response.Response.ResponseCode == 200)
            {
                if (response.data.Count > 0)
                {
                    ViewBag.ProductName = response.data.First().ProductName;
                    ViewBag.TotalQty = " (" + response.data.Sum(x => x.Qty) + ")";
                }

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
            var response = KPIAPIManager.GetSales(id);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.Locations = new SelectList(response.data.Locations, "Key", "Value");
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
            var response = KPIAPIManager.GetSales(0);
            if (response.Response.ResponseCode == 200)
            {
                ViewBag.Locations = new SelectList(response.data.Locations, "Key", "Value");
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult GetSalesLineItemPartial(SalesDetails salesDetails)
        {
            ViewBag.Products = new SelectList(new List<SelectListItem>(), "ProductID", "ProductName");

            ProductMasterResponse productResponse = KPIAPIManager.GetProductsDetails();
            if (productResponse != null && productResponse.Response != null && productResponse.Response.ResponseCode == 200)
            {
                if (productResponse.productMastersListData != null)
                {
                    ViewBag.Products = new SelectList(productResponse.productMastersListData, "ProductID", "ProductName");
                }
            }

            //SalesMasterResponse companyResponse = KPIAPIManager.GetCompanyLocationDetails(103);
            //if (companyResponse != null && companyResponse.Response != null && companyResponse.Response.ResponseCode == 200)
            //{
            //    if (productResponse.Response.ResponseData != null)
            //    {
            //        ViewBag.Locations = companyResponse.Response.ResponseData;
            //    }
            //}
            return PartialView("_SalesLineItems", salesDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "SalesID,SalesDate,CompanyLocationID,Instructions,LineItems")] KPILib.Models.SalesMaster sale)      //FormCollection frm)       //
        {
            //int i = 0;
            //foreach(var c in frm.AllKeys)
            //{
            //    Response.Write("Key " + c + " : " + c[i].ToString() + "<br/>");
            //    i++;
            //}

            //return View();

            if (ModelState.IsValid)
            {
                sale.UserID = 1001;     //TODO: replace with UserID from session
                //sale.LineItems.Add(new KPILib.Models.SalesDetails { ProductID = 1002, Qty = 8 });
                //sale.LineItems.Add(new KPILib.Models.SalesDetails { ProductID = 1003, Qty = 2 });

                var response = KPIAPIManager.AddSales(sale);
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
        public ActionResult Edit(KPILib.Models.SalesMaster sale)
        {
            if (ModelState.IsValid)
            {
                var response = KPIAPIManager.EditSales(sale);
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
using KPI.Classes;
using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Production")]
    public class ProductionController : Controller
    {
        // GET: Production
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllProduction(0);
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
        public ActionResult GetAllProduction()
        {
            var response = KPIAPIManager.GetAllProduction(0);
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
        public ActionResult GetProductionBatch(int ProductionProgramID = 0)
        {
            ProductionProgramesResponse productionProgrames = KPIAPIManager.GetProductionBatch(ProductionProgramID);
            return View(productionProgrames.productionPrograme);
        }

        public ActionResult GetBatchPartialView(ProductionBatches productionBatches)
        {
            return PartialView(productionBatches);
        }

        public ActionResult GetAllCompleted()
        {
            var response = KPIAPIManager.GetAllProduction(1);
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

        public ActionResult StartProduction(FormCollection frm)
        {
            int hidProductionProgramID = 0;
            int spnToProduce = 0;
            int hidSpnCanProduce = 0;


            Int32.TryParse(frm["hidProductionProgramID"].ToString(), out hidProductionProgramID);
            Int32.TryParse(frm["spnToProduce"].ToString(), out spnToProduce);
            Int32.TryParse(frm["hidSpnCanProduce"].ToString(), out hidSpnCanProduce);

            if (spnToProduce <= hidSpnCanProduce)
            {
                var parameters = new Dictionary<string, int>
                {
                    { "iProductionProgramID", hidProductionProgramID },
                    { "iToProduceQty", spnToProduce }
                };

                var response = KPIAPIManager.StartProduction(parameters);
                if (response.ResponseCode == 200)
                {
                    return RedirectToAction("GetProductionBatch", "Production",new { ProductionProgramID  = hidProductionProgramID });
                }
                else
                {
                    ViewBag.Error = response.ResponseMsg;
                    return View("Error");
                }
            }
            ViewBag.Error = "PRODUCED NOW can not be greater than CAN PRODUCE";
            return View("Error");
        }

        public ActionResult GetProductionPlan(int id)
        {
            var response = KPIAPIManager.GetProductionPlan(id);
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

        public ActionResult UpdateProduction(FormCollection frm)
        {
            int hidProductionProgramID, txtProducedNow02, hidSpnCanProduce02, iBatchId;
            hidProductionProgramID = txtProducedNow02 = hidSpnCanProduce02 = 0;

            Int32.TryParse(frm["hidProductionProgramID02"].ToString(), out hidProductionProgramID);
            Int32.TryParse(frm["txtProducedNow02"].ToString(), out txtProducedNow02);
            Int32.TryParse(frm["hidSpnCanProduce02"].ToString(), out hidSpnCanProduce02);
            Int32.TryParse(frm["iBatchId"].ToString(), out iBatchId);

            if (txtProducedNow02 <= hidSpnCanProduce02)
            {
                var response = KPIAPIManager.UpdateProduction(hidProductionProgramID, txtProducedNow02, iBatchId);
                if (response.ResponseCode == 200)
                {
                    return RedirectToAction("GetProductionBatch", "Production", new { ProductionProgramID = hidProductionProgramID });
                }
                else
                {
                    ViewBag.Error = response.ResponseMsg;
                    return View("Error");
                }
            }
            ViewBag.Error = "PRODUCED NOW can not be greater than CAN PRODUCE";
            return View("Error");
        }

        public ActionResult GetRackingPlan(int id)
        {
            var response = KPIAPIManager.GetRackingPlan(id);
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
    }
}
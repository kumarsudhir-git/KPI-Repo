using KPI.Classes;
using KPI.Filters;
using System;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_OpenSales")]
    public class DispatchController : CCSPLBaseController
    {
        // GET: Dispatch
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllDispatch01(0);
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

        public ActionResult GetAllClosed()
        {
            var response = KPIAPIManager.GetAllDispatch01(1);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetBookingDispatch(FormCollection frm)
        {
            int iSalesDetailsID, iBlockedQty, iToDispatchQty, iToProduceQty;
            iSalesDetailsID = iBlockedQty = iToDispatchQty = iToProduceQty = 0;

            Int32.TryParse(frm["hidSalesDetailsID"].ToString(), out iSalesDetailsID);
            Int32.TryParse(frm["txtBlocked"].ToString(), out iBlockedQty);
            Int32.TryParse(frm["txtToDispatch"].ToString(), out iToDispatchQty);
            Int32.TryParse(frm["txtToProduce"].ToString(), out iToProduceQty);

            int userId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;

            var response = KPIAPIManager.SetNewDispatchBlockValues(iSalesDetailsID, iBlockedQty, iToDispatchQty, iToProduceQty, userId);
            if (response.ResponseCode == 200)
            {
                return RedirectToAction("GetAll", "Dispatch");
            }
            else
            {
                ViewBag.Error = response.ResponseMsg;
                return View("Error");
            }
        }
    }
}
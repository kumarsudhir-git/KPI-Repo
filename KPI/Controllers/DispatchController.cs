using KPI.Classes;
using KPI.Filters;
using KPILib;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
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

        [HttpGet]
        public ActionResult GetSalesDispatchDetailData(int salesId)
        {
            ViewData["DispatchStatus"] = new SelectList(new List<SelectListItem>(), "LookUpValue", "LookUpName");
            var response = KPIAPIManager.GetSalesDispatchDetailData(salesId);
            if (response.Response.ResponseCode == 200)
            {
                LookUpMasterResponse GSMLookUpMaster = KPIAPIManager.GetLookUpData(ApplicationConstants.StatusType);
                if (GSMLookUpMaster != null && GSMLookUpMaster.Response.ResponseCode == 200)
                {
                    ViewData["DispatchStatus"] = new SelectList(GSMLookUpMaster.lookupMasterList, "LookUpValue", "LookUpName");
                }
                return PartialView(response.salesDispatchDetailObj);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }
        }
        
        [HttpPost]
        public ActionResult SaveSalesDispatchDetailData(SalesDispatchDetailMaster salesDispatchDetail, HttpPostedFileBase DocketImageFile)
        {
            if (DocketImageFile != null && DocketImageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(DocketImageFile.FileName);
                string path = Path.Combine(Server.MapPath("~/Uploads/Dockets/"), fileName);

                DocketImageFile.SaveAs(path);

                salesDispatchDetail.DocketPhotoPath = "~/Uploads/Dockets/" + fileName;
            }
            return View();
            //var response = KPIAPIManager.GetSalesDispatchDetailData(salesId);
            //if (response.Response.ResponseCode == 200)
            //{
            //    return PartialView(response.salesDispatchDetailObj);
            //}
            //else
            //{
            //    ViewBag.Error = response.Response.ResponseMsg;
            //    return View("Error");
            //}
        }

    }
}
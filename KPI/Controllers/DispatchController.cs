using KPI.Classes;
using KPI.Filters;
using KPILib;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return RedirectToAction("GetOpenSalesOrderDispatch");
        }

        public ActionResult GetAllClosed()
        {
            return RedirectToAction("GetDispatchedHistory");
        }

        [HttpGet]
        public ActionResult GetOpenSalesOrderDispatch()
        {
            var response = KPIAPIManager.GetOpenSalesOrderDispatchSummary();
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
        public ActionResult GetDispatchedHistory(DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 20)
        {
            var response = KPIAPIManager.GetDispatchedHistory(fromDate, toDate);
            if (response.Response.ResponseCode == 200)
            {
                var history = response.data.OrderByDescending(x => x.DispatchDate ?? DateTime.MinValue).ToList();
                var totalRecords = history.Count;
                var totalPages = totalRecords == 0 ? 1 : (int)Math.Ceiling((double)totalRecords / pageSize);
                page = Math.Max(1, Math.Min(page, totalPages));

                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalRecords = totalRecords;

                return View(history.Skip((page - 1) * pageSize).Take(pageSize).ToList());
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
        public ActionResult GetSalesDispatchDetailData(int salesId, int? salesDetailsId = null)
        {
            ViewData["DispatchStatus"] = new SelectList(new List<SelectListItem>(), "LookUpValue", "LookUpName");
            var response = KPIAPIManager.GetSalesDispatchDetailData(salesId, salesDetailsId);
            if (response.responseObj.ResponseCode == 200)
            {
                LookUpMasterResponse GSMLookUpMaster = KPIAPIManager.GetLookUpData(ApplicationConstants.StatusType);
                if (GSMLookUpMaster != null && GSMLookUpMaster.Response.ResponseCode == 200)
                {
                    ViewData["DispatchStatus"] = new SelectList(GSMLookUpMaster.lookupMasterList, "LookUpValue", "LookUpName");
                }
                return PartialView(response.transporterDetailsMasterObj);
            }
            else
            {
                ViewBag.Error = response.responseObj.ResponseMsg;
                return View("Error");
            }
        }
        
        [HttpPost]
        public ActionResult SaveSalesDispatchDetailData(SalesDispatchTransporterDetailsMaster salesDispatchDetailMaster, HttpPostedFileBase DocketImageFile)
        {
            if (DocketImageFile != null && DocketImageFile.ContentLength > 0)
            {
                string folderPath = Server.MapPath("~/Uploads/Dockets/");
                string fileName = Path.GetFileName(DocketImageFile.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                // Create folder if not exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Save file
                DocketImageFile.SaveAs(filePath);


                salesDispatchDetailMaster.DocketPhotoPath = "~/Uploads/Dockets/" + fileName;
            }
            int sessionUserId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;

            if (salesDispatchDetailMaster.SDTRDId > 0)
            {
                salesDispatchDetailMaster.UpdatedBy = sessionUserId;
            }
            else
            {
                salesDispatchDetailMaster.CreatedBy = sessionUserId;
            }
            var response = KPIAPIManager.SaveSalesDispatchDetailData(salesDispatchDetailMaster);
            if (response.responseObj.ResponseCode == 200)
            {
                TempData["SuccessMsg"] = "Data saved successfully !";
                return RedirectToAction("GetOpenSalesOrderDispatch");
            }
            else
            {
                ViewBag.Error = response.responseObj.ResponseMsg;
                return View("Error");
            }
        }
    }
}

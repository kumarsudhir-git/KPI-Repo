using KPI.Classes;
using KPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [SessionCheckFilter]
    public class ExternalController : CCSPLBaseController
    {
        // GET: External
        public ActionResult ShowPO(int id)
        {
            var response = KPIAPIManager.GetPurchase(id);
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

        //[HttpPost]
        public ActionResult POResponse(string id)
        {
            var response = KPIAPIManager.POResponse(id);
            if (response.ResponseCode == 200)
            {
                ViewBag.Msg = "Action Saved";
                return View("StdView");
            }
            else
            {
                ViewBag.Error = response.ResponseMsg;
                return View("Error");
            }
        }

        //[HttpPost]
        //public ActionResult POResponseAccept(KPILib.Models.PurchaseMaster purchase)
        //{


        //    return View("Error");
        //}
    }
}
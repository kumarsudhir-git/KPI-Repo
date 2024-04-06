using KPI.Filters;
using KPILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KPI.Controllers
{
    //[SessionCheckFilter]
    public class CCSPLBaseController : Controller
    {
        public ResponseObj CheckSessionForAjaxCall()
        {
            ResponseObj Response = new ResponseObj();

            int SessionUserId = Convert.ToInt32(Session["UserID"]);

            if (SessionUserId == 0)
            {
                Response.ResponseCode = 440;
                Response.ResponseMsg = "Session Expired!!! Please Login Again";
                Response.ResponseData = "/Login/Login";
            }
            else
            {
                Response.IsSuccessful();
            }
            return Response;
        }
    }
}
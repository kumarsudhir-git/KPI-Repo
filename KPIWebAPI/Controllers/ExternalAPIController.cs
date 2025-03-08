using KPILib.Models;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace KPIWebAPI.Controllers
{
    public class ExternalAPIController : CCSPLBaseAPIController
    {
        // GET: ExternalAPI
        [HttpPost]
        public IHttpActionResult POResponse(object id)
        {
            var returnValue = new ResponseObj();

            try
            {
                string param = (string) id;

                string action = "";

                if (param.IndexOf("Accept") > 0)
                    action = "Accept";
                if (param.IndexOf("Reject") > 0)
                    action = "Reject";

                int purchaseID = Convert.ToInt32(param.Replace(action, ""));

                var data = db.PurchaseMasters.SingleOrDefault(x => x.PurchaseID == purchaseID);
                if(data != null)
                {
                    data.PurchaseStatusID = (action == "Reject" ? 20 : 30);
                    
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }

                returnValue.IsSuccessful();
            }
            catch (Exception ex)
            {
                returnValue.ResponseMsg = ex.Message;
                CommonLogger.Error(ex, ex.Message);
            }

            return Json(returnValue);
        }
    }
}
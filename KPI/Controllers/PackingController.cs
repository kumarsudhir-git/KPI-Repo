using KPI.Classes;
using KPI.Filters;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Dispatch")]
    public class PackingController : CCSPLBaseController
    {
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllPacking(0);
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

        
        public ActionResult PrintPackingDetails(int id)     //SalesDetailsID   // List<int> SalesIDProductID)
        {
            var response = KPIAPIManager.GetPackingDetails(id);
            if (response.Response.ResponseCode == 200)
            {
                return View(response.data);
            }
            else
            {
                ViewBag.Error = response.Response.ResponseMsg;
                return View("Error");
            }

            //return View(itm);
        }
    }
}
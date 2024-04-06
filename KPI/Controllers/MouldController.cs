using KPI.Classes;
using KPI.Filters;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Moulds")]
    public class MouldController : CCSPLBaseController
    {
        // GET: Mould
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllMoulds();
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
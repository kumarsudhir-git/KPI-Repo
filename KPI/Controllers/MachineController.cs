using KPI.Classes;
using KPI.Filters;
using System.Web.Mvc;

namespace KPI.Controllers
{
    [CustomAuthFilter("M_Machines")]
    public class MachineController : CCSPLBaseController
    {
        // GET: Machine
        public ActionResult GetAll()
        {
            var response = KPIAPIManager.GetAllMachines();
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
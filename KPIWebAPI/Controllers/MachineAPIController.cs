using KPILib.Models;
using KPIWebAPI.AuthFilters;
using System;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Machines")]
    public class MachineAPIController : CCSPLBaseAPIController
    {
        public IHttpActionResult GetAll()
        {
            var returnValue = new MachineMastersResponse();

            try
            {
                var data = db.MachineMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.MachineName).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<MachineMaster, KPILib.Models.Machine>(obj);

                    if (obj.MachineHistories.LastOrDefault() != null)
                    {
                        o.MachineStatusID = obj.MachineHistories.LastOrDefault().MachineStatusID;
                        o.MachineStatus = ((enumMachineStatus)obj.MachineHistories.LastOrDefault().MachineStatusID).ToString() + (((enumMachineStatus)obj.MachineHistories.LastOrDefault().MachineStatusID) == enumMachineStatus.InUse ? ": " + obj.MachineHistories.LastOrDefault().Description : "");
                    }

                    returnValue.data.Add(o);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }
    }
}

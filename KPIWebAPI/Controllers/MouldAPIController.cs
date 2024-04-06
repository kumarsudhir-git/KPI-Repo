using KPILib.Models;
using KPIWebAPI.AuthFilters;
using System;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Moulds")]
    public class MouldAPIController : CCSPLBaseAPIController
    {
        public IHttpActionResult GetAll()
        {
            var returnValue = new MouldMastersResponse();

            try
            {
                var data = db.MouldMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.MouldName).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<MouldMaster, KPILib.Models.Mould>(obj);

                    foreach (var product in obj.ProductMasters.ToList())
                    {
                        o.AllProducts += "," + mapper.Map<ProductMaster, KPILib.Models.Product>(product).ProductName;
                        o.Products.Add(mapper.Map<ProductMaster, KPILib.Models.Product>(product));
                    }
                    if (o.AllProducts != null)
                        if (o.AllProducts.Length > 0)
                            o.AllProducts = o.AllProducts.Substring(1);

                    foreach (var machine in obj.MachineMouldMappings.ToList())
                    {
                        o.AllMachines += "," + mapper.Map<MachineMaster, KPILib.Models.Machine>(machine.MachineMaster).MachineName;
                        o.Machines.Add(mapper.Map<MachineMaster, KPILib.Models.Machine>(machine.MachineMaster));
                    }
                    if (o.AllMachines != null)
                        if (o.AllMachines.Length > 0)
                            o.AllMachines = o.AllMachines.Substring(1);

                    if (obj.MouldHistories.LastOrDefault() != null)
                    {
                        o.MouldStatusID = obj.MouldHistories.LastOrDefault().MouldStatusID;
                        o.MouldStatus = ((enumMouldStatus)obj.MouldHistories.LastOrDefault().MouldStatusID).ToString() + (((enumMouldStatus)obj.MouldHistories.LastOrDefault().MouldStatusID) == enumMouldStatus.InUse ? ": " + obj.MouldHistories.LastOrDefault().Description : "");
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

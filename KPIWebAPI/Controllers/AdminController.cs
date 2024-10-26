using System;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    public class AdminController : CCSPLBaseAPIController
    {
        [HttpGet]
        public IHttpActionResult Index()
        {
            return Json(new { message = "API is up and running" });
        }

        [HttpGet]
        public IHttpActionResult CheckConnection()
        {
            try
            {
                // Test the database connection by trying to open it
                var connection = db.Database.Connection;
                connection.Open();
                connection.Close();

                return Json(new { success = true, message = "Database connection successful" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
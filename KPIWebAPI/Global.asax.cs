using KPILib;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace KPIWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            CacheManager.CreatCacheInstance(Convert.ToInt32(ConfigurationManager.AppSettings["jwtTokenExpiryTime"]));
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;

            // Redirect to `api/Home/Index` if accessing the root URL
            if (context.Request.Url.AbsolutePath == "/")
            {
                context.Response.Redirect("~/api/Admin/Index");
            }
        }

    }
}

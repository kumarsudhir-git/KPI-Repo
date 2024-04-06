using System.Web.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using System;
using KPILib;
using System.Collections.Generic;
using KPILib.Models;
using System.Linq;
using Newtonsoft.Json;

namespace KPI.Filters
{
    public class CustomAuthFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        private readonly string menuCode;
        private readonly bool isMenuRoleCheck;

        public CustomAuthFilter(string menuCode, bool isMenuRoleCheck = true)
        {
            this.menuCode = menuCode;
            this.isMenuRoleCheck = isMenuRoleCheck;
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (
                string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["UserName"])) ||
                string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["JWTToken"]))
                )
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                                { "Controller", "Login" },
                                { "Action", "Login" }
                                });

                return;
            }

            var token = Convert.ToString(filterContext.HttpContext.Session["JWTToken"]);

            if (token != null && isMenuRoleCheck)
            {
                if (!AuthorizationProcess.IsUserAuthorize(token, filterContext.HttpContext.Request.HttpMethod, menuCode))
                {
                    //filterContext.Result = new HttpStatusCodeResult(statusCode: System.Net.HttpStatusCode.MethodNotAllowed);

                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                                { "Controller", "UserMaster" },
                                { "Action", "NotAllowed" }
                                });

                    return;
                }
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Login View of Account Controller  
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                                { "Controller", "Login" },
                                { "Action", "Login" }
                                });
            }
        }


    }
}

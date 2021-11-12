using Reimbursement_Web_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Reimbursement_Web_System
{
    public class SessionAuthorizeAttribute : AuthorizeAttribute
    {
        //authentication based on the role and controller
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["UserId"] != null)
            {
                var rd = httpContext.Request.RequestContext.RouteData;
                string currentController = rd.GetRequiredString("controller"); //get the controller name

                Role role = (Role)httpContext.Session["Role"];
                if (role.Equals(Role.Standard) && currentController.Contains("User")) //if the standard user visit from user controller return true
                {
                    return true;
                }
                //if the admin user visit from admin controller return true
                else if ((role.Equals(Role.Director) || role.Equals(Role.HSU) || role.Equals(Role.HR) || role.Equals(Role.SDAS) || role.Equals(Role.Finance)) && currentController.Contains("Admin"))
                {
                    return true;
                }
                //if the any role visit notification controller return true
                else if ((role.Equals(Role.Standard) || (role.Equals(Role.Director) || role.Equals(Role.HSU) || role.Equals(Role.HR) || role.Equals(Role.SDAS) || role.Equals(Role.Finance))) &&
                    currentController.Contains("Notification"))
                {
                    return true;
                }
            }

            //return false if not authenticated proceed to HandleUnauthorizedRequest
            return false;


        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var targetController = filterContext.RouteData.Values["controller"];
            if ((string)targetController == "Notification")
            {
                //if an unathenticated user visits calls Notification controller return HttpUnauthorizedResult
                filterContext.Result = new HttpUnauthorizedResult(); 
            }
            else
            {
                //if an unathenticated user visits anything redirect to home page
                filterContext.Result = new RedirectToRouteResult(
                                  new RouteValueDictionary
                                  {
                                   { "action", "Index" },
                                   { "controller", "Home" }
                                  });
            }
        }
    }
}
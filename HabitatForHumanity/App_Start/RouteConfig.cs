using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HabitatForHumanity
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "Examples",
             url: "examples",
             defaults: new { controller = "Home", action = "Examples" }
            );

            //routes.MapRoute(
            //    name: "ManageVolunteer",
            //    url: "Admin/ManageVolunteer/{id}",
            //    defaults: new { controller = "Admin", action = "manageVolunteer", id = UrlParameter.Optional, name = UrlParameter.Optional, standardId = UrlParameter.Optional },
            //    constraints: new { id = @"\d+" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}

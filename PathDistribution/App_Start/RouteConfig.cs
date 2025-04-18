using System.Web.Mvc;
using System.Web.Routing;

namespace PathDistribution
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("files/{*pathInfo}");
            routes.IgnoreRoute("css/{*pathInfo}");
            routes.IgnoreRoute("js/{*pathInfo}");

            routes.MapMvcAttributeRoutes();
            //TOdo add calendar route
            //routes.MapRoute(
            //    name: "Reports",
            //    url: "Admin/GetReport/{reportName}",
            //    defaults: new { controller = "Admin", action = "GetReport", reportName = "Test" }
            //);
            routes.MapRoute(
                name: "calendar",
                url: "{controller}/{action}/{_calendar}",
                defaults: new { controller = "Start", action = "Index", _calendar = true}
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Start", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

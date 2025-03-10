using System.IO;
using System.Web.Mvc;

namespace PathDistribution.Helpers
{
    public static class PartialHTML
    {
        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            context.Controller.ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
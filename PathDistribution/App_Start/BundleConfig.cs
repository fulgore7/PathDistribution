using System.Web.Optimization;

namespace PathDistribution
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Clear();
            bundles.ResetAll();

            BundleTable.EnableOptimizations = false;

            bundles.Add(new StyleBundle("~/bundles/1")
                            .Include("~/Content/css/screen.css")
                            .Include("~/Content/themes/base/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/bundles/2")
                            .Include("~/Content/css/jquery.multiselect.css")
                            .Include("~/Content/css/jquery.multiselect.filter.css"));

            bundles.Add(new StyleBundle("~/bundles/3")
                            .Include("~/Content/css/screenAdmin.css")
                            .Include("~/Content/themes/base/jquery-ui.css"));

            bundles.Add(new ScriptBundle("~/bundles/a")
                            .Include("~/Scripts/jquery-{version}.js")
                            .Include("~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/b")
                            .Include("~/Scripts/jquery.tmpl.js"));

            bundles.Add(new ScriptBundle("~/bundles/c")
                            .Include("~/Scripts/General.js"));

            bundles.Add(new StyleBundle("~/bundles/bs")
                .Include("~/Content/bootstrap.min.css")
                .Include("~/Content/themes/base/jquery-ui.css")
                .Include("~/Scripts/bootstrap.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/d").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/e")
                            .Include("~/Scripts/jquery.multiselect.js")
                            .Include("~/Scripts/jquery.multiselect.filter.js"));

            bundles.Add(new ScriptBundle("~/bundles/f")
                            .Include("~/Scripts/Holidays.js"));
            //BundleTable.EnableOptimizations = false;
        }
    }
}

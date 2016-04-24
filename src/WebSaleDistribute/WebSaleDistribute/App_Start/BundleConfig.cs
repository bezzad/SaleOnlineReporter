using System.Web.Optimization;

namespace WebSaleDistribute
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterScripts(bundles);
            RegisterContents(bundles);
        }

        public static void RegisterScripts(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_ui").Include(
                       "~/Scripts/jquery-ui.min.js",
                       "~/Scripts/jquery.ui.touch-punch.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/lobipanel").Include(
            "~/Scripts/lobipanel.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/panel").Include("~/Scripts/panel.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery.dataTables", "https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js").Include("~/Scripts/jquery.dataTables.min.js"));

        }

        public static void RegisterContents(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/Content/PersianDatePicker").Include(
            //          "~/Content/PersianDatePicker.min.css"));


            bundles.Add(new StyleBundle("~/Content/jquery_ui").Include(
                      "~/Content/jquery-ui.min.css"));

            // http://lobianijs.com/site/lobipanel#description
            bundles.Add(new StyleBundle("~/Content/lobipanel").Include(
                     "~/Content/lobipanel.min.css"));

            bundles.Add(new StyleBundle("~/Content/panel").Include("~/Content/panel.css"));

            bundles.Add(new StyleBundle("~/Content/jquery.dataTables", "https://cdn.datatables.net/1.10.11/css/jquery.dataTables.min.css").Include("~/Content/jquery.dataTables/jquery.dataTables.min.css"));
        }

    }
}

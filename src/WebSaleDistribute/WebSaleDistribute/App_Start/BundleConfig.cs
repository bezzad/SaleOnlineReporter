using System.Diagnostics;
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
            
            BundleTable.EnableOptimizations = !Debugger.IsAttached;
        }

        public static void RegisterScripts(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js",
                       "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_ui").Include(
                       "~/Scripts/jquery-ui.min.js",
                       "~/Scripts/jquery.ui.touch-punch.min.js",
                       "~/Scripts/jquery.fileDownload.js"));

            bundles.Add(new ScriptBundle("~/bundles/lobipanel").Include(
                        "~/Scripts/lobipanel.min.js",
                        "~/Scripts/panel.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-select.min.js",
                      "~/Scripts/respond.js"));


            //bundles.Add(new ScriptBundle("~/bundles/jquery.dataTables", "https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js").Include("~/Scripts/jquery.dataTables.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery_dataTables").Include(
                "~/Scripts/jquery.dataTables.min.js",
                "~/Scripts/DataTables-Plugins/type-detection/currency.js",
                "~/Scripts/DataTables-Plugins/type-detection/numeric-comma.js",
                "~/Scripts/DataTables-Plugins/sorting/currency.js",
                "~/Scripts/DataTables-Plugins/sorting/numeric-comma.js",
                "~/Scripts/DataTables-Plugins/sorting/persian.js"));

            bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
                    "~/Scripts/Highcharts-4.0.1/js/highcharts.js",
                    "~/Scripts/Highcharts-4.0.1/js/modules/exporting.js",
                    "~/Scripts/Highcharts-4.0.1/js/modules/drilldown.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include("~/Scripts/site.js"));

            bundles.Add(new ScriptBundle("~/bundles/clipboard").Include("~/Scripts/clipboard.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include("~/Scripts/toastr.min.js"));
        }

        public static void RegisterContents(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/bootstrap-theme.min.css",
                     "~/Content/bootstrap-select.min.css",
                     "~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/Content/PersianDatePicker").Include(
            //          "~/Content/PersianDatePicker.min.css"));


            bundles.Add(new StyleBundle("~/Content/jquery_ui").Include(
                      "~/Content/jquery-ui.min.css"));

            // http://lobianijs.com/site/lobipanel#description
            bundles.Add(new StyleBundle("~/Content/lobipanel").Include(
                     "~/Content/lobipanel.min.css",
                     "~/Content/panel.css"));

            //bundles.Add(new StyleBundle("~/Content/jquery.dataTables", "https://cdn.datatables.net/1.10.11/css/jquery.dataTables.min.css").Include("~/Content/jquery.dataTables.min.css"));
            bundles.Add(new StyleBundle("~/Content/jquery_dataTables").Include("~/Content/jquery.dataTables.min.css"));
            bundles.Add(new StyleBundle("~/Content/errors").Include("~/Content/errors.css"));
            bundles.Add(new StyleBundle("~/Content/toastr").Include("~/Content/toastr.min.css"));
        }

    }
}

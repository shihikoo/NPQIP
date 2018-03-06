using System.Web;
using System.Web.Optimization;

namespace NPQIP
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/npqip").Include(
            "~/Scripts/NPQIP*",
            "~/Scripts/googleanalytics*"));

            bundles.Add(new ScriptBundle("~/bundles/graph").Include("~/Scripts/drawreportgraph*"));

            bundles.Add(new ScriptBundle("~/bundles/contact").Include(
                "~/SCripts/contact*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapjquery").Include(
                        "~/Scripts/bootstrap*"));
                        ;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include( 
                 "~/Content/bootstrap.css",
                 "~/Content/bootstrap-theme.css",
                "~/Content/bootstrap-theme.min.css",
                "~/Content/bootstrap.min.css",
                "~/Content/bootstrap.css.map",
                "~/Content/bootstrap-theme.css.map",
                "~/Content/site.css",
                "~/Content/font-awesome.css"));
            bundles.Add(new StyleBundle("~/Content/Certificatecss").Include(
     "~/Content/certificate.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}
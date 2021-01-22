using System.Web;
using System.Web.Optimization;

namespace Cricket
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/bootstrap/bootstrap.min.js",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/toastr.js",
                        "~/Scripts/app.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap/bootstrap.min.css",
                      "~/Content/app.css",
                      "~/Content/font-awesome/font-awesome.min.css",
                      "~/Content/typeahead.css",
                      "~/Content/toastr.css",
                      "~/Content/flag-icon.min.css"
            ));
        }
    }
}

using System.Web;
using System.Web.Optimization;

namespace RefaccionariaLosMochis
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/complementos").Include(
                        "~/Scripts/fontawesome/all.min.js",
                        "~/Scripts/loadingoverlay/loadingoverlay.min.js",
                        "~/Scripts/DataTables/jquery.dataTables.js",
                        "~/Scripts/DataTables/dataTables.responsive.js",
                        "~/Scripts/sweetalert.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/Index/PermisosNav.js",
                        "~/Scripts/scripts.js"));



            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios.  De esta manera estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                       "~/Scripts/bootstrap.bundle.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css",
                "~/Content/sweetalert.css",
                "~/Content/DataTables/css/jquery.dataTables.css",
                "~/Content/jquery-ui.css",
                "~/Content/bootstrap.min.css",
                "~/Content/estilo.css",
                "~/Content/DataTables/css/responsive.dataTables.css"
                ));
        }
    }
}

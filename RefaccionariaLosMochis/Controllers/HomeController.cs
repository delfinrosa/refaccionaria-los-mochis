using CapaEntidad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Asegúrate de tener esto
using RefaccionariaLosMochis.Permisos;
using Newtonsoft.Json; // Importar el espacio de nombres de tus extensiones

namespace RefaccionariaLosMochis.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }


        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public async Task<IActionResult> CerrarSesion()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();

            // Redirigir al login
            return RedirectToAction("Index", "Acceso");
        }
    }
}

using CapaEntidad;
using CapaDatos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Newtonsoft.Json;
using RefaccionariaLosMochis.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using RefaccionariaLosMochis.Models;

namespace RefaccionariaLosMochis.Controllers
{
    public class AccesoController : Controller
    {

        private readonly IEmailSender _emailSender;
        private readonly IEmailService _emailService;

        public AccesoController(IEmailService emailService)
        {
            _emailService = emailService; 
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult OlvideMiContraseña()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string correo, string clave)
        {
            string mensajeError;
            try
            {
                clave = new Recursos().ConvertirSha256(clave);
                Usuario objeto = new CD_Usuario().Verificacion(correo, clave.ToUpper(), out mensajeError);

                if (objeto != null && objeto.Activo == "A")
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, objeto.Nombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    HttpContext.Session.SetString("Usuario", JsonConvert.SerializeObject(objeto));

                    Response.Cookies.Append("idUsuario", objeto.IdUsuario.ToString()); // Asegúrate de que objeto.IdUsuario tenga un valor
                    Response.Cookies.Append("tipo", objeto.Tipo.ToString());

                    if (objeto.Tipo =="V")
                    {
                        return RedirectToAction("Index", "Venta");

                    }                    
                    if (objeto.Tipo =="C")
                    {
                        return RedirectToAction("Caja", "Venta");

                    }                    
                    if (objeto.Tipo =="I")
                    {
                        return RedirectToAction("Producto", "Producto");

                    }
                    if (objeto.Tipo =="A")
                    {
                        return RedirectToAction("Index", "Home");

                    }
                    return RedirectToAction("Index", "Acceso");

                }
                else
                {
                    ViewBag.ErrorMessage = string.IsNullOrEmpty(mensajeError) ? "El usuario no está activo o no existe." : mensajeError;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ha ocurrido un error durante la verificación. Por favor, intente nuevamente. Error específico: {ex.Message}";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OlvideMiContraseña(string correo, string usuario)
        {
            try
            {
                string contra = "";
                Random random = new Random();
                for (int i = 0; i < 4; i++)
                {
                    contra += random.Next(0, 10).ToString();
                }

                string mensajeError = string.Empty;
                bool resultado = new CD_Usuario().CambiarContraseñaPorUsuario(usuario, correo, contra, out mensajeError);

                if (resultado)
                {
                    var request = new EmailTo()
                    {
                        Para = correo,
                        Asunto = "Cambio de contraseña",
                        Contenido = $"{usuario}, su nueva contraseña es: {contra}"
                    };

                    _emailService.SendEmail(request);

                    // Redirigir al Index de Acceso
                    TempData["SuccessMessage"] = "Se ha enviado la nueva contraseña a su correo.";
                    return RedirectToAction("Index", "Acceso");
                }
                else
                {
                    // Mostrar el error del proceso en ViewBag
                    ViewBag.ErrorMessage = mensajeError;
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Captura de cualquier excepción inesperada
                ViewBag.ErrorMessage = $"Ha ocurrido un error durante la verificación. Por favor, intente nuevamente. Error específico: {ex.Message}";
                return View();
            }
        }



    }
}

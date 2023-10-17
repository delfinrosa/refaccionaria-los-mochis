using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RefaccionariaLosMochis.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            clave = Recursos.ConvertirSha256(clave);
            Usuario objeto = new CN_Usuario().Verificacion(correo, clave.ToUpper());

            if (objeto.Activo == "A")
            {
                FormsAuthentication.SetAuthCookie(objeto.Nombre, false);

                Session["Usuario"] = objeto;

                return RedirectToAction("Index", "Home");
            }


            return View();
        }
    }
}
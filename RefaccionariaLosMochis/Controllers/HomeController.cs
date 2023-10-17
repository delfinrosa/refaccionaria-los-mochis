using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RefaccionariaLosMochis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            Usuario usuario = Session["Usuario"] as Usuario;
            ViewBag.tipo = usuario.Tipo;
            // Crear una nueva cookie y asignarle el valor de ViewBag.tipo
            HttpCookie tipoCookie = new HttpCookie("tipo", ViewBag.tipo);
            Response.Cookies.Add(tipoCookie);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CerrarSesion()
        {

            FormsAuthentication.SignOut();
            Session["Usuario"] = null;


            return RedirectToAction("Index", "Acceso");



        }
    }
}
using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using RefaccionariaLosMochis.Permisos;
using Microsoft.AspNetCore.Authorization;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]

    public class MetodoPagoController : Controller
    {
        // GET: MetodoPago
        [PermisosRol("I")]

        public ActionResult Index()
        {
            //Usuario usuario = HttpContext.Session.GetObject<Usuario>("Usuario");

            //if (usuario != null)
            //{
            //    // Agregar cookies primero
            //    Response.Cookies.Append("tipo", usuario.Tipo.ToString());
            //    Response.Cookies.Append("idUsuario", usuario.IdUsuario.ToString());

            //    // Establecer ViewBag.tipo
            //    ViewBag.tipo = usuario.Tipo;
            //}
            //else
            //{
            //    // Manejo de sesión expirada o sin usuario
            //    return RedirectToAction("Index", "Acceso");
            //}

            return View();
        }        
        public ActionResult MetodoPago()
        {
            Usuario usuario = HttpContext.Session.GetObject<Usuario>("Usuario");

            if (usuario != null)
            {
                // Agregar cookies primero
                Response.Cookies.Append("tipo", usuario.Tipo.ToString());
                Response.Cookies.Append("idUsuario", usuario.IdUsuario.ToString());

                // Establecer ViewBag.tipo
                ViewBag.tipo = usuario.Tipo;
            }
            else
            {
                // Manejo de sesión expirada o sin usuario
                return RedirectToAction("Index", "Acceso");
            }

            return View();
        }

        [HttpPost]
        public JsonResult GuardarMetodoPago(MetodoPago metodoPago, bool boolGuardar)
        {
            string mensaje = string.Empty;
            metodoPago.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());

            object resultado;
            if (boolGuardar)
            {
                resultado = new CD_MetodoPago().Registrar(metodoPago, out mensaje);
            }
            else
            {
                resultado = new CD_MetodoPago().Editar(metodoPago, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult EliminarMetodoPago(string id)
        {
            string mensaje = string.Empty;
            bool resultado = new CD_MetodoPago().Eliminar(id, out mensaje);

            return Json(new { resultado = resultado, mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult UltimoRegistro()
        {
            MetodoPago oMetodoPago = new MetodoPago();
            oMetodoPago = new CD_MetodoPago().UltimoRegistro();
            return Json(new { Lista = oMetodoPago });
        }
        //Resultado un solo objeto a partir de un nombre
        [HttpPost]
        public JsonResult BuscarMetodoPagoPorNombre(string nombre)
        {
            MetodoPago oMetodoPago = new MetodoPago();
            if (!string.IsNullOrEmpty(nombre))
            {
                oMetodoPago = new CD_MetodoPago().BuscarMetodoPagoPorNombre(nombre);
            }
            return Json(new { Estatus = oMetodoPago.Estatus, Descripcion = oMetodoPago.Descripcion, CFDIMetodoPagoId = oMetodoPago.CFDIMetodoPagoId });
        }

        //Resultado un solo objeto a partir de un id
        [HttpPost]
        public JsonResult ListarPorIdMetodoPago(string CFDIMetodoPagoId)
        {
            MetodoPago oMetodoPago = new MetodoPago();
            if (!string.IsNullOrEmpty(CFDIMetodoPagoId))
            {
                oMetodoPago = new CD_MetodoPago().BuscarPorId(CFDIMetodoPagoId);
            }
            return Json(new { Lista = oMetodoPago });
        }

        //Resultado un entero que es la cantidad de elementos que contienen el string que entro
        [HttpPost]
        public JsonResult countBuscadorMetodoPago(string nombre)
        {
            int registros = 0;
            registros = new CD_MetodoPago().countBuscadorMetodoPago(nombre);
            return Json(new { registros = registros });
        }

        //Resultado una lista de objetos que contienen el string que entro
        [HttpPost]
        public JsonResult elementosPaginacionBuscadorMetodoPago(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                lista = new CD_MetodoPago().elementosPaginacionBuscadorMetodoPago(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });
        }

        /************TABLA************/
        [HttpPost]
        public JsonResult CountTablaMetodoPago()
        {
            int registros = new CD_MetodoPago().CountTablaMetodoPago();
            return Json(new { registros = registros });
        }

        [HttpPost]
        public JsonResult ListarMetodoPagoTabla(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<MetodoPago> oLista = new CD_MetodoPago().ListarMetodoPagoTabla(pagina, tipoOrden, siguientes);
            return Json(new { data = oLista });
        }

        /************TABLA CON WHERE************/
        [HttpPost]
        public JsonResult ListarMetodoPagoTablaWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<MetodoPago> oLista = new CD_MetodoPago().ListarMetodoPagoTablaWhere(pagina, tipoOrden, siguientes, where);
            return Json(new { data = oLista });
        }

        [HttpPost]
        public JsonResult CountTablaWhereMetodoPago(string where)
        {
            int registros = new CD_MetodoPago().CountTablaWhereMetodoPago(where);
            return Json(new { registros = registros });
        }

        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tCFDIMetodoPago");
            return Json(new { campos = campos } );

        }

    }
}
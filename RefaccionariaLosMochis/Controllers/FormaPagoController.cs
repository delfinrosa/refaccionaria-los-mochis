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

    public class FormaPagoController : Controller
    {
        [PermisosRol("I")]

        public ActionResult FormaPago()
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
        // Método para guardar o editar la forma de pago
        [HttpPost]
        public JsonResult GuardarFormaPago(FormaPago formaPago, bool boolGuaradar)
        {
            string mensaje = string.Empty;
            bool resultado = false;

            formaPago.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());

            if (boolGuaradar)
            {
                resultado = new CD_FormaPago().Registrar(formaPago, out mensaje);
            }
            else
            {
                resultado = new CD_FormaPago().Editar(formaPago, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult EliminarFormaPago(string id)
        {
            string mensaje = string.Empty;
            bool resultado = new CD_FormaPago().Eliminar(id, out mensaje);

            return Json(new { resultado = resultado, mensaje = mensaje }    );
        }
        [HttpPost]
        public JsonResult UltimoRegistro()
        {
            FormaPago oFormaPago = new FormaPago();
            oFormaPago = new CD_FormaPago().UltimoRegistro();
            return Json(new { Lista = oFormaPago }      );
        }
        [HttpPost]
        public JsonResult ListarPorIdFormaPago(string id)
        {
            FormaPago oFormaPago = new FormaPago();
            if (!string.IsNullOrEmpty(id))
            {
                oFormaPago = new CD_FormaPago().BuscarFormaPagoPorId(id);
            }
            return Json(new { Lista = oFormaPago });
        }
        [HttpPost]
        public JsonResult BuscarFormaPagoPorDescripcion(string descripcion)
        {
            FormaPago oFormaPago = new FormaPago();
            if (!string.IsNullOrEmpty(descripcion))
            {
                oFormaPago = new CD_FormaPago().BuscarFormaPagoPorDescripcion(descripcion);
            }
            return Json(new { Activo = oFormaPago.Estatus, Descripcion = oFormaPago.Descripcion, CFDIFormaPagoId = oFormaPago.CFDIFormaPagoId });
        }

        // COUNT TABLA FORMA DE PAGO
        [HttpPost]
        public JsonResult CountTabla()
        {
            int registros = new CD_FormaPago().CountTabla();
            return Json(new { registros = registros });
        }

        // LISTAR FORMA DE PAGO
        [HttpPost]
        public JsonResult ListarTabla(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<FormaPago> oLista = new CD_FormaPago().ListarFormaPagoTabla(pagina, tipoOrden, siguientes);
            return Json(new { data = oLista });
        }

        // LISTAR FORMA DE PAGO CON WHERE
        [HttpPost]
        public JsonResult ListarTablaWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<FormaPago> oLista = new CD_FormaPago().ListarFormaPagoTablaWhere(pagina, tipoOrden, siguientes, where);
            return Json(new { data = oLista });
        }

        // COUNT TABLA FORMA DE PAGO CON WHERE
        [HttpPost]
        public JsonResult CountTablaWhere(string where)
        {
            int registros = new CD_FormaPago().CountTablaWhere(where);
            return Json(new { registros = registros });
        }

        /************BUSCADOR DE FORMA DE PAGO************/
        //Resultado un entero que es la cantidad de elementos que contienen el string que entro
        [HttpPost]
        public JsonResult CountBuscadorFormaPago(string descripcion)
        {
            int registros = 0;
            registros = new CD_FormaPago().CountBuscadorFormaPago(descripcion);
            return Json(new { registros = registros });
        }

        //Resultado una lista de descripciones de formas de pago que contienen el string que entro
        [HttpPost]
        public JsonResult ElementosPaginacionBuscadorFormaPago(string descripcion, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (!string.IsNullOrWhiteSpace(descripcion))
            {
                lista = new CD_FormaPago().ElementosPaginacionBuscadorFormaPago(descripcion, pagina, siguientes);
            }
            return Json(new { Lista = lista });
        }

        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tCFDIFormaPago");
            return Json(new { campos = campos }     );

        }
    }
}
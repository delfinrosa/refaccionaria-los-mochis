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
    public class RegimenFiscalController : Controller
    {
        // GET: RegimenFiscal
        [PermisosRol("I")]

        public ActionResult RegimenFiscal()
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
        [HttpPost]
        public JsonResult GuardarRegimenFiscal(RegimenFiscal regimenFiscal, bool boolGuardar)
        {
            string mensaje = string.Empty;
            regimenFiscal.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());

            object resultado;
            if (boolGuardar)
            {
                resultado = new CD_RegimenFiscal().Registrar(regimenFiscal, out mensaje);
            }
            else
            {
                resultado = new CD_RegimenFiscal().Editar(regimenFiscal, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult EliminarRegimenFiscal(string id)
        {
            string mensaje = string.Empty;
            bool resultado = new CD_RegimenFiscal().Eliminar(id, out mensaje);

            return Json(new { resultado = resultado, mensaje = mensaje });
        }
        //Resultado un solo objeto a partir de un nombre para RegimenFiscal
        [HttpPost]
        public JsonResult BuscarRegimenFiscalPorNombre(string nombre)
        {
            RegimenFiscal oRegimenFiscal = new RegimenFiscal();
            if (!string.IsNullOrEmpty(nombre))
            {
                // Asumiendo que existe una clase CD_RegimenFiscal con el método BuscarRegimenFiscalPorNombre
                oRegimenFiscal = new CD_RegimenFiscal().BuscarRegimenFiscalPorNombre(nombre);
            }
            return Json(new { Activo = oRegimenFiscal.Estatus, Descripcion = oRegimenFiscal.Descripcion, CFDIRegimenFiscalId = oRegimenFiscal.CFDIRegimenFiscalId });
        }

        //Resultado un solo objeto a partir de un ID para RegimenFiscal
        [HttpPost]
        public JsonResult ListarPorIdRegimenFiscal(string CFDIRegimenFiscalId)
        {
            RegimenFiscal oRegimenFiscal = new RegimenFiscal();
            if (!string.IsNullOrEmpty(CFDIRegimenFiscalId))
            {
                // Asumiendo que existe una clase CD_RegimenFiscal con el método BuscarPorId
                oRegimenFiscal = new CD_RegimenFiscal().BuscarRegimenFiscalPorId(CFDIRegimenFiscalId);
            }
            return Json(new { Lista = oRegimenFiscal });
        }

        [HttpPost]
        public JsonResult UltimoRegistroRegimenFiscal()
        {
            RegimenFiscal oRegimenFiscal = new RegimenFiscal();
            oRegimenFiscal = new CD_RegimenFiscal().UltimoRegistro();
            return Json(new { Lista = oRegimenFiscal });
        }
        [HttpPost]
        public JsonResult CountRegimenFiscal()
        {
            int registros = new CD_RegimenFiscal().CountTablaRegimenFiscal();
            return Json(new { registros = registros });
        }

        [HttpPost]
        public JsonResult ListarRegimenFiscalTabla(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<RegimenFiscal> oLista = new CD_RegimenFiscal().ListarRegimenFiscalTabla(pagina, tipoOrden, siguientes);
            return Json(new { data = oLista });
        }

        public JsonResult ListarRegimenFiscalTablaWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<RegimenFiscal> oLista = new CD_RegimenFiscal().ListarRegimenFiscalTablaWhere(pagina, tipoOrden, siguientes, where);
            return Json(new { data = oLista });
        }

        public JsonResult CountTablaRegimenFiscalWhere(string where)
        {
            int registros = new CD_RegimenFiscal().CountTablaRegimenFiscalWhere(where);
            return Json(new { registros = registros });
        }
        /************BUSCADOR************/
        //Resultado un entero que es la cantidad de elementos que contienen el string que entro
        [HttpPost]
        public JsonResult CountBuscadorRegimenFiscal(string nombre)
        {
            int registros = 0;
            registros = new CD_RegimenFiscal().CountBuscadorRegimenFiscal(nombre); // Asegúrate de tener este método en tu clase CD_RegimenFiscal
            return Json(new { registros = registros });
        }

        //Resultado una lista de objetos que contienen el string que entro
        [HttpPost]
        public JsonResult ElementosPaginacionBuscadorRegimenFiscal(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                lista = new CD_RegimenFiscal().ElementosPaginacionBuscadorRegimenFiscal(nombre, pagina, siguientes); // Asegúrate de tener este método en tu clase CD_RegimenFiscal
            }
            return Json(new { Lista = lista });
        }

    }
}
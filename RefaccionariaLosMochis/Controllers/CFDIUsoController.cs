using CapaDatos;
using CapaEntidad;
using Microsoft.AspNetCore.Mvc;
using RefaccionariaLosMochis.Permisos;
using Microsoft.AspNetCore.Authorization;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]

    public class CFDIUsoController : Controller
    {
        [PermisosRol("I")]

        // GET: CFDIUso
        public ActionResult CFDIUso()
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
        public JsonResult GuardarCFDIUso(CFDIUso cfdiUso, bool boolGuardar)
        {
            string mensaje = string.Empty;
            cfdiUso.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"] ?? "0");

            object resultado;
            if (boolGuardar)
            {
                resultado = new CD_CFDIUso().Registrar(cfdiUso, out mensaje);
            }
            else
            {
                resultado = new CD_CFDIUso().Editar(cfdiUso, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult EliminarCFDIUso(string id)
        {
            string mensaje = string.Empty;
            bool resultado = new CD_CFDIUso().Eliminar(id, out mensaje);

            return Json(new { resultado = resultado, mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult UltimoRegistro()
        {
            CFDIUso oCFDIUso = new CFDIUso();
            oCFDIUso = new CD_CFDIUso().UltimoRegistro();
            return Json(new { Lista = oCFDIUso });
        }


        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tCFDIUso");
            return Json(new { campos = campos });

        }
        // Buscar un CFDIUso por su descripción
        [HttpPost]
        public JsonResult BuscarCFDIUsoPorDescripcion(string descripcion)
        {
            CFDIUso oCFDIUso = new CFDIUso();
            if (!string.IsNullOrEmpty(descripcion))
            {
                oCFDIUso = new CD_CFDIUso().BuscarCFDIUsoPorDescripcion(descripcion);
            }
            return Json(new { Estatus = oCFDIUso.Estatus, Descripcion = oCFDIUso.Descripcion, CFDIUsoId = oCFDIUso.CFDIUsoId });
        }

        // Buscar un CFDIUso por su ID
        [HttpPost]
        public JsonResult ListarPorIdCFDIUso(string CFDIUsoId)
        {
            CFDIUso oCFDIUso = new CFDIUso();
            if (!string.IsNullOrEmpty(CFDIUsoId))
            {
                oCFDIUso = new CD_CFDIUso().BuscarPorId(CFDIUsoId);
            }
            return Json(new { Lista = oCFDIUso });
        }
        /************BUSCADOR************/
        // Cuenta la cantidad de CFDIUso que contienen el string ingresado
        [HttpPost]
        public JsonResult countBuscadorCFDIUso(string nombre)
        {
            int registros = 0;
            registros = new CD_CFDIUso().CountBuscadorCFDIUso(nombre);
            return Json(new { registros = registros });
        }

        // Devuelve una lista de descripciones de CFDIUso que contienen el string ingresado
        public JsonResult elementosPaginacionBuscadorCFDIUso(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (!string.IsNullOrEmpty(nombre))
            {
                lista = new CD_CFDIUso().ElementosPaginacionBuscadorCFDIUso(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });
        }

        /************TABLA************/
        // Cuenta todos los CFDIUso
        [HttpPost]
        public JsonResult CountTablaCFDIUso()
        {
            int registros = 0;
            registros = new CD_CFDIUso().CountTablaCFDIUso();
            return Json(new { registros = registros });
        }

        // Lista los CFDIUso paginados
        [HttpPost]
        public JsonResult ListarTablaCFDIUso(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<CFDIUso> oLista = new List<CFDIUso>();
            oLista = new CD_CFDIUso().ListarCFDIUsoTabla(pagina, tipoOrden, siguientes);
            return Json(new { data = oLista });
        }

        // Lista los CFDIUso paginados y filtrados por una condición
        public JsonResult ListarTablaCFDIUsoWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<CFDIUso> oLista = new List<CFDIUso>();
            oLista = new CD_CFDIUso().ListarCFDIUsoTablaWhere(pagina, tipoOrden, siguientes, where);
            return Json(new { data = oLista });
        }

        // Cuenta los CFDIUso filtrados por una condición
        public JsonResult countTablaCFDIUsoWhere(string where)
        {
            int registros = 0;
            registros = new CD_CFDIUso().CountTablaCFDIUsoWhere(where);
            return Json(new { registros = registros });
        }

    }
}
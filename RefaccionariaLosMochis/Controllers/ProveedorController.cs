using CapaEntidad;
using CapaDatos;
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

    public class ProveedorController : Controller
    {
        // GET: Proveedor
        [PermisosRol("I")]

        public ActionResult Proveedor()
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
        public JsonResult GuardarProveedor(Proveedor objeto, bool boolGuaradar)
        {
            object resultado;
            string mensaje = string.Empty;
            objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());

            if (boolGuaradar)
            {
                resultado = new CD_Proveedor().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CD_Proveedor().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }); ;

        }
        [HttpPost]
        public JsonResult EliminarProveedor(string rfc)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Proveedor().Eliminar(rfc, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }
        //Resultado un solo objeto apartir de un nombre 
        [HttpPost]
        public JsonResult buscarProveedorPorNombre(string nombre)
        {
            Proveedor oLista = new Proveedor();
            if (nombre != null || nombre != "")
            {
                oLista = new CD_Proveedor().BuscarProveedorPorNombre(nombre);
            }
            return Json(new { Lista = oLista });

        }
        //Resultado un solo objeto apartir de un id 
        [HttpPost]
        public JsonResult BusquedaID(string Id)
        {
            Proveedor oProveedor = new Proveedor();
            if (Id != null)
            {
                oProveedor = new CD_Proveedor().BusquedaID(Id);
            }
            return Json(new { Lista = oProveedor });
        }
        //Resultado un objeto que es el ultimo modificado 
        [HttpPost]
        public JsonResult UltimoRegistro()
        {
            Proveedor oProveedor = new Proveedor();
            oProveedor = new CD_Proveedor().UltimoRegistro();
            return Json(new { Lista = oProveedor });

        }
        /************BUSCADOR************/
        //Resultado un entero que es la cantidad de elementos que contienen el string que entro
        [HttpPost]
        public JsonResult CountBuscador(string nombre)
        {
            int registros = 0;
            registros = new CD_Proveedor().CountBuscador(nombre);
            return Json(new { registros = registros });

        }
        //Resultado una lista de objetos que contienen el string que entro
        [HttpPost]
        public JsonResult ElementosPaginacionBuscador(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (nombre != null || nombre != "")
            {
                lista = new CD_Proveedor().ElementosPaginacionBuscador(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });

        }
        /************TABLA************/
        [HttpPost]
        public JsonResult CountTabla()
        {
            int registros = 0;
            registros = new CD_Proveedor().CountTabla();
            return Json(new { registros = registros });

        }
        [HttpPost]

        public JsonResult ListarProveedor(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Proveedor> oLista = new List<Proveedor>();
            oLista = new CD_Proveedor().ListarProveedor(pagina, tipoOrden, siguientes);
            return Json(new { data = oLista });
        }
        /************TABLA CON WHERE************/
        public JsonResult ListarProveedorWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Proveedor> oLista = new List<Proveedor>();
            oLista = new CD_Proveedor().ListarProveedorWhere(pagina, tipoOrden, siguientes, where);
            return Json(new { data = oLista });
        }
        public JsonResult countTablaWhere(string where)
        {
            int registros = 0;
            registros = new CD_Proveedor().countWhere(where);
            return Json(new { registros = registros });

        }
        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tProveedores");
            return Json(new { campos = campos } );

        }
    }
}
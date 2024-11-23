using CapaEntidad;
using CapaDatos;
using RefaccionariaLosMochis.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]

    public class LineaController : Controller
    {
        [PermisosRol("I")]
        public ActionResult Linea()
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
        public JsonResult GuardarLinea(Linea objeto)
        {
            object resultado;
            string mensaje = string.Empty;
            objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());
            if (objeto.IdLinea == 0)
            {
                resultado = new CD_Linea().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CD_Linea().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }); ;

        }
        [HttpPost]
        public JsonResult EliminarLinea(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Linea().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }
        //Resultado un solo objeto apartir de un nombre 
        [HttpPost]
        public JsonResult bucarLineaPorNombre(string nombre)
        {
            Linea oLista = new Linea();
            if (nombre != null || nombre != "")
            {
                oLista = new CD_Linea().bucarLineaPorNombre(nombre);
            }
            return Json(new { Activo = oLista.Activo, Descripcion = oLista.Descripcion, IdLinea = oLista.IdLinea, Deslc = oLista.Deslc });

        }
        //Resultado un solo objeto apartir de un id 
        [HttpPost]
        public JsonResult ListarPorIdLineas(int Id)
        {
            Linea oLinea = new Linea();
            if (Id != null)
            {
                oLinea = new CD_Linea().BusquedaIDLinea(Id);
                }
                return Json(new { Lista = oLinea }  );
        }
        //Resultado un objeto que es el ultimo modificado 
        [HttpPost]
        public JsonResult UltimoRegistro()
        {
            Linea oLinea = new Linea();
            oLinea = new CD_Linea().UltimoRegistro();
            return Json(new { Lista = oLinea });

        }
        /************BUSCADOR************/
        //Resultado un entero que es la cantidad de elementos que contienen el string que entro
        [HttpPost]
        public JsonResult countBuscador(string nombre)
        {
            int registros = 0;
            registros = new CD_Linea().countBuscador(nombre);
            return Json(new { registros = registros });

        }
        //Resultado una lista de objetos que contienen el string que entro
        [HttpPost]
        public JsonResult elementosPaginacionBuscador(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (nombre != null || nombre != "")
            {
                lista = new CD_Linea().elementosPaginacionBuscador(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });

        }
        /************TABLA************/
        [HttpPost]
        public JsonResult countTabla()
        {
            int registros = 0;
            registros = new CD_Linea().countTabla();
            return Json(new { registros = registros });

        }
        [HttpPost]

        public JsonResult ListarLinea(string strpagina,string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Linea> oLista = new List<Linea>();
            oLista = new CD_Linea().ListarLinea(pagina, tipoOrden, siguientes);
            return Json(new { data = oLista });
        }
        /************TABLA CON WHERE************/
        public JsonResult ListarLineaWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Linea> oLista = new List<Linea>();
            oLista = new CD_Linea().ListarLineaWhere(pagina, tipoOrden, siguientes,  where);
            return Json(new { data = oLista });
        }
        public JsonResult countTablaWhere(string where)
        {
            int registros = 0;
            registros = new CD_Linea().countTablaWhere(where);
            return Json(new { registros = registros } );

        }
        public JsonResult ChecarConexion()
        {
            bool resultado= new CD_Linea().ChecarConexion();
            return Json(new { resultado = resultado });

        }
        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tLineas");
            return Json(new { campos = campos });

        }
        [HttpPost]
        public JsonResult tamañoCaracteristicas()
        {
            var campos = new Recursos().Tamaño("tLineasCaracteristicas");
            return Json(new { campos = campos } );

        }
    }
}
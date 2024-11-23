using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using CapaEntidad;
using CapaDatos;
using RefaccionariaLosMochis.Permisos;
using Microsoft.AspNetCore.Authorization;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]

    public class UsuarioController : Controller
    {
        // GET: Usuario
        [PermisosRol("A")]
        public ActionResult Usuario()
        {
            return View();
        }
        //Usuario
        [HttpPost]
        //Guardar

        public JsonResult GuardarUsuario(Usuario objeto)
        {
            object resultado;
            string mensaje = string.Empty;
            if (objeto.IdUsuario == 0)
            {
                objeto.Contraseña = new Recursos().ConvertirSha256( objeto.Contraseña);
                resultado = new CD_Usuario().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CD_Usuario().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }); ;

        }
        [HttpPost]
        //Eliminar
        public JsonResult EliminarUsuario(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Usuario().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }
        [HttpPost]
        public JsonResult bucarUsuarioPorNombre(string nombre)
        {
            Usuario oLista = new Usuario();
            if (nombre != null || nombre != "")
            {
                oLista = new CD_Usuario().bucarUsuarioPorNombre(nombre);
            }
            return Json(new { Lista = oLista});
        }
        //Resultado un solo objeto apartir de un id 
        [HttpPost]
        public JsonResult ListarPorIdUsuario(int Id)
        {
            Usuario oLinea = new Usuario();
            if (Id != null)
            {
                oLinea = new CD_Usuario().BusquedaIDUsuario(Id);
            }
            return Json(new { Lista = oLinea });
        }
        //Resultado un objeto que es el ultimo modificado 
        [HttpPost]
        public JsonResult UltimoRegistroUsuario()
        {
            Usuario oUsuario = new Usuario();
            oUsuario = new CD_Usuario().UltimoRegistro();
            return Json(new { Lista = oUsuario });
        }
        /************BUSCADOR************/
        //Resultado un entero que es la cantidad de elementos que contienen el string que entro
        [HttpPost]
        public JsonResult countBuscador(string nombre)
        {
            int registros = 0;
            registros = new CD_Usuario().countBuscador(nombre);
            return Json(new { registros = registros });
        }
        //Resultado una lista de objetos que contienen el string que entro
        public JsonResult elementosPaginacionBuscador(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (nombre != null || nombre != "")
            {
                lista = new CD_Usuario().elementosPaginacionBuscador(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });
        }
        /************TABLA************/
        [HttpPost]
        public JsonResult CountTabla()
        {
            int registros = 0;
            registros = new CD_Usuario().CountTabla();
            return Json(new { registros = registros });

        }
        [HttpPost]
        public JsonResult ListarUsuarioTabla(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Usuario> oLista = new List<Usuario>();
            oLista = new CD_Usuario().ListarUsuarioTabla(pagina, tipoOrden, siguientes);
            return Json(new { data = oLista });
        }
        /************TABLA CON WHERE************/
        public JsonResult ListarUsuarioTablaWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Usuario> oLista = new List<Usuario>();
            oLista = new CD_Usuario().ListarUsuarioTablaWhere(pagina, tipoOrden, siguientes, where);
            return Json(new { data = oLista });
        }
        public JsonResult countTablaWhere(string where)
        {
            int registros = 0;
            registros = new CD_Usuario().countTablaWhere(where);
            return Json(new { registros = registros });

        }

        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tUsuarios");
            return Json(new { campos = campos } );

        }
    }
}

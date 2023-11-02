using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;
using System.Data;
using RefaccionariaLosMochis.Permisos;
using System.Collections;
using System.Web.UI.WebControls;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]

    public class MantenedorController : Controller
    {
        #region Marca
        //Marca

        [PermisosRol("I")]


        public ActionResult Marca()
        {
            return View();
        }

        [HttpGet]
        //Listar
        public JsonResult ListarMarca()
        {
            List<Marca> oLista = new List<Marca>();
            oLista = new CN_Marca().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //Guardar

        public JsonResult GuardarMarca(Marca objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdMarca == 0)
            {
                resultado = new CN_Marca().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Marca().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpPost]
        //Eliminar
        public JsonResult EliminarMarca(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Marca().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

        #region Linea
        //Linea

        [PermisosRol("I")]
        public ActionResult Linea()
        {
            return View();
        }



        [HttpGet]
        //Listar General (TABLA)
        public JsonResult ListarLinea()
        {
            List<Linea> oLista = new List<Linea>();
            oLista = new CN_Linea().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        //Guardar
        public JsonResult GuardarLinea(Linea objeto)
        {
            object resultado;
            string mensaje = string.Empty;
            objeto.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.Value);
            Linea objDevolucion = null;

            if (objeto.IdLinea == 0)
            {
                resultado = new CN_Linea().Registrar(objeto, out mensaje, out objDevolucion);
            }
            else
            {
                resultado = new CN_Linea().Editar(objeto, out mensaje, out objDevolucion);
            }
            return Json(new { resultado = resultado, mensaje = mensaje, objDevolucion = objDevolucion }, JsonRequestBehavior.AllowGet); ;

        }


        [HttpPost]
        //Eliminar
        public JsonResult EliminarLinea(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Linea().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet); ;
        }

        //PRUEBA DE EDITAR NUEVO
        [HttpPost]
        //Busqueda Filtro Linea Por Nombre  
        //Resultado un solo objeto
        public JsonResult BusquedaFiltroLinea(string nombre)
        {

            Linea oLista = new Linea();
            if (nombre != null || nombre != "")
            {
                oLista = new CN_Linea().BusquedaFiltroLinea(nombre);
            }
            return Json(new { Activo = oLista.Activo, Descripcion = oLista.Descripcion, IdLinea = oLista.IdLinea, Deslc = oLista.Deslc }, JsonRequestBehavior.AllowGet);

        }

        //ListarNombreDeLineas
        [HttpPost]
        //Busqueda Filtro Linea Por Nombre  
        //Resultado un List<objeto>
        public JsonResult ListarNombreDeLineas(string nombre)
        {

            List<string> lista = new List<string>();
            if (nombre != null || nombre != "")
            {
                lista = new CN_Linea().ListarNombreDeLineas(nombre);
            }
            return Json(new { Lista = lista }, JsonRequestBehavior.AllowGet);

        }

        //ListarLineas
        [HttpPost]
        //Busqueda Filtro Linea Por ID  
        //Resultado un objeto
        public JsonResult ListarPorIdLineas(int Id)
        {

            Linea oLinea = new Linea();
            if (Id != null)
            {
                oLinea = new CN_Linea().ListarPorIdLineas(Id);
            }
            return Json(new { Lista = oLinea }, JsonRequestBehavior.AllowGet);

        }

        //UltimoRegistro
        [HttpPost]
        //Busqueda el ultimo modificado 
        //Resultado un objeto
        public JsonResult UltimoRegistro()
        {
            Linea oLinea = new Linea();
            oLinea = new CN_Linea().UltimoRegistro();
            return Json(new { Lista = oLinea }, JsonRequestBehavior.AllowGet);

        }

        //PruebaAutocompletado
        public JsonResult PruebaAutocompletado()
        {
            List<Linea> oLista = new List<Linea>();
            oLista = new CN_Linea().PruebasAutoCompletado();

            // Número de la página actual (puedes obtener esto de alguna manera en tu lógica)
            int intPaginaActual = 1;

            // Crear un objeto anidado con la lista de resultados y el número de página
            var respuesta = new
            {
                intPaginaActual = intPaginaActual,
                lstItems = oLista
            };

            return Json(new { intPaginaActual = intPaginaActual, lstItems = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //Busqueda el ultimo modificado 
        //Resultado un objeto
        public JsonResult COUNT_PruebasAutoCompletado(string nombre)
        {
            int registros = 0;
            registros = new CN_Linea().COUNT_PruebasAutoCompletado(nombre);
            return Json(new { registros = registros }, JsonRequestBehavior.AllowGet);

        }
        //Paginado
        public JsonResult PaginacionPRUEBA(string nombre,int pagina)
        {

            List<string> lista = new List<string>();
            if (nombre != null || nombre != "")
            {
                lista = new CN_Linea().PaginacionPRUEBA(nombre,pagina);
            }
            return Json(new { Lista = lista }, JsonRequestBehavior.AllowGet);

        }

        ////////////////
        /////PRUEBA PAGINADO TABLA
        /////////////////
        [HttpPost]

        public JsonResult COUNT_Tabla()
        {
            int registros = 0;
            registros = new CN_Linea().COUNT_Tabla();
            return Json(new { registros = registros }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]

        public JsonResult ListarPrueba(string strpagina)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Linea> oLista = new List<Linea>();
            oLista = new CN_Linea().ListarPrueba(pagina);
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        ////////////////
        /////PRUEBA PAGINADO TABLA FIN
        /////////////////


        #endregion


        #region Producto
        //Producto
        [PermisosRol("I")]

        public ActionResult Producto()
        {
            return View();
        }

        [HttpGet]
        //Listar
        public JsonResult ListarProducto()
        {
            List<Producto> oLista = new List<Producto>();
            oLista = new CN_Producto().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //Guardar

        public JsonResult GuardarProducto(string objeto, string archivoImagen)
        {




            object resultado;
            string mensaje = string.Empty;
            bool operacion_exitosa = true;
            bool guardar_imagen_exito = true;
            Producto oproducto = new Producto();
            oproducto = JsonConvert.DeserializeObject<Producto>(objeto);
            if (oproducto.IdProducto == 0)
            {
                int idProductoGenerado = new CN_Producto().Registrar(oproducto, out mensaje);

                if (idProductoGenerado != 0)
                {
                    oproducto.IdProducto = idProductoGenerado;
                    ImagenProducto oImagenProducto = new ImagenProducto();
                    //oImagenProducto.oProducto.IdProducto = idProductoGenerado;


                    //oImagenProducto.Imagen = File.ReadAllBytes(nombre_imagen);

                }
                else
                {
                    operacion_exitosa = false;
                }

            }
            else
            {
                resultado = new CN_Producto().Editar(oproducto, out mensaje);
            }
            return Json(new { operacionExitosa = operacion_exitosa, IdGenerado = oproducto.IdProducto, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //Eliminar
        public JsonResult EliminarProducto(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Producto().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

        #region Usuario
        //Usuario
        [PermisosRol("A")]

        public ActionResult Usuario()
        {
            return View();
        }

        [HttpGet]
        //Listar
        public JsonResult ListarUsuario()
        {
            List<Usuario> oLista = new List<Usuario>();
            oLista = new CN_Usuario().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //Guardar

        public JsonResult GuardarUsuario(Usuario objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdUsuario == 0)
            {
                resultado = new CN_Usuario().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Usuario().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpPost]
        //Eliminar
        public JsonResult EliminarUsuario(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Usuario().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

    }
}

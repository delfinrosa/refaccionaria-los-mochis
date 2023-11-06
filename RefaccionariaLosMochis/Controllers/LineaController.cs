using CapaEntidad;
using CapaNegocio;
using RefaccionariaLosMochis.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RefaccionariaLosMochis.Controllers
{
    public class LineaController : Controller
    {


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
        public JsonResult PaginacionPRUEBA(string nombre, int pagina)
        {

            List<string> lista = new List<string>();
            if (nombre != null || nombre != "")
            {
                lista = new CN_Linea().PaginacionPRUEBA(nombre, pagina);
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

        public JsonResult ListarPrueba(string strpagina,string tipoOrden)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Linea> oLista = new List<Linea>();
            oLista = new CN_Linea().ListarPrueba(pagina, tipoOrden);
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        ////////////////
        /////PRUEBA PAGINADO TABLA FIN
        /////////////////


        #endregion
    }
}
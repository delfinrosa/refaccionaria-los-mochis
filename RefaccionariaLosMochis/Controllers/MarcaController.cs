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
    public class MarcaController : Controller
    {
        #region Marca
        // Marca

        [PermisosRol("I")]
        public ActionResult Marca()
        {
            return View();
        }

        [HttpGet]
        // Listar General (TABLA)
        public JsonResult ListarMarcas()
        {
            List<Marca> oLista = new List<Marca>();
            oLista = new CN_Marca().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        // Guardar
        public JsonResult GuardarMarca(Marca objeto)
        {
            object resultado;
            string mensaje = string.Empty;
            objeto.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.Value);
            Marca objDevolucion = null;

            if (objeto.IdMarca == 0)
            {
                resultado = new CN_Marca().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Marca().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje, objDevolucion = objDevolucion }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        // Eliminar
        public JsonResult EliminarMarca(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Marca().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        // PRUEBA DE EDITAR NUEVO
        [HttpPost]
        // Busqueda Filtro Marca Por Nombre  
        // Resultado un solo objeto
        public JsonResult BusquedaFiltroMarca(string nombre)
        {
            Marca oLista = new Marca();
            if (nombre != null || nombre != "")
            {
                oLista = new CN_Marca().BusquedaFiltroMarca(nombre);
            }
            return Json(new { Activo = oLista.Activo, Descripcion = oLista.Descripcion, IdMarca = oLista.IdMarca }, JsonRequestBehavior.AllowGet);
        }

        // ListarMarcas
        [HttpPost]
        // Busqueda Filtro Marca Por ID  
        // Resultado un objeto
        public JsonResult BuscarPorId(int Id)
        {
            Marca oMarca = new Marca();
            if (Id != null)
            {
                oMarca = new CN_Marca().BuscarPorId(Id);
            }
            return Json(new { Lista = oMarca }, JsonRequestBehavior.AllowGet);
        }

        // UltimoRegistro
        [HttpPost]
        // Busqueda el ultimo modificado 
        // Resultado un objeto
        public JsonResult UltimoRegistroMarca()
        {
            Marca oMarca = new Marca();
            oMarca = new CN_Marca().UltimoRegistroMarca();
            return Json(new { Lista = oMarca }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        // Contar registros para auto completado
        public JsonResult COUNT_PruebasAutoCompletadoMarca(string nombre)
        {
            int registros = 0;
            registros = new CN_Marca().COUNT_PruebasAutoCompletadoMarca(nombre);
            return Json(new { registros = registros }, JsonRequestBehavior.AllowGet);
        }

        // Paginado
        public JsonResult PaginacionPRUEBA(string nombre, int pagina)
        {
            List<string> lista = new List<string>();
            if (nombre != null || nombre != "")
            {
                lista = new CN_Marca().PaginacionPRUEBAMarca(nombre, pagina);
            }
            return Json(new { Lista = lista }, JsonRequestBehavior.AllowGet);
        }

        // PRUEBA PAGINADO TABLA
        [HttpPost]
        public JsonResult COUNT_TablaMarca()
        {
            int registros = 0;
            registros = new CN_Marca().COUNT_TablaMarca();
            return Json(new { registros = registros }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarPruebaMarca(string strpagina)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Marca> oLista = new List<Marca>();
            oLista = new CN_Marca().ListarPruebaMarca(pagina);
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        // PRUEBA PAGINADO TABLA FIN
        #endregion

    }
}
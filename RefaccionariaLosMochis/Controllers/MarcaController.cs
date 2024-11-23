using CapaEntidad;
using CapaDatos;
using RefaccionariaLosMochis.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]

    public class MarcaController : Controller
    {
        [PermisosRol("I")]
        public ActionResult Marca()
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
        // Guardar
        public JsonResult GuardarMarca(Marca objeto)
        {
            object resultado;
            string mensaje = string.Empty;
            objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());
            Marca objDevolucion = null;
            if (objeto.IdMarca == 0)
            {
                resultado = new CD_Marca().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CD_Marca().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje, objDevolucion = objDevolucion });
            //Stimulsoft.Report.StiReport rpt = new  Stimulsoft.Report.StiReport();
            //rpt.Load("ruta|byte[]");
            //rpt.DataSources[0]. coneccion
            //rpt.DataSources[0].Parameters["strTicket"] = 
            //rpt.Render()
            //rpt.Print
            //    rpt.ExportDocument(Stimulsoft.Report.StiExportFormat.ExcelXml,)
        }
        [HttpPost]
        // Eliminar
        public JsonResult EliminarMarca(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Marca().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje });
        }
        //Resultado un solo objeto apartir de un nombre 
        [HttpPost]
        public JsonResult bucarMarcaPorNombre(string nombre)
        {
            Marca oLista = new Marca();
            if (nombre != null || nombre != "")
            {
                oLista = new CD_Marca().bucarMarcaPorNombre(nombre);
            }
            return Json(new { Activo = oLista.Activo, Descripcion = oLista.Descripcion, IdMarca = oLista.IdMarca });
        }
        //Resultado un solo objeto apartir de un id 
        [HttpPost]
        public JsonResult ListarPorIdMarca(int Id)
        {
            Marca oLinea = new Marca();
            if (Id != null)
            {
                oLinea = new CD_Marca().BuscarPorId(Id);
            }
            return Json(new { Lista = oLinea });
        }
        //Resultado un objeto que es el ultimo modificado 
        [HttpPost]
        public JsonResult UltimoRegistroMarca()
        {
            Marca oMarca = new Marca();
            oMarca = new CD_Marca().UltimoRegistro();
            return Json(new { Lista = oMarca });
        }
        /************BUSCADOR************/
        //Resultado un entero que es la cantidad de elementos que contienen el string que entro
        public JsonResult CountBuscador(string nombre)
        {
            int registros =  new CD_Marca().CountBuscador(nombre);
            return Json(new { registros = registros });
        }
        //Resultado una lista de objetos que contienen el string que entro
        public JsonResult elementosPaginacionBuscador(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (nombre != null || nombre != "")
            {
                lista = new CD_Marca().elementosPaginacionBuscador(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });
        }
        /************TABLA************/
        [HttpPost]
        public JsonResult CountTabla()
        {
            int registros = 0;
            registros = new CD_Marca().CountTabla();
            return Json(new { registros = registros });

        }
        [HttpPost]

        public JsonResult ListarMarcaTabla(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Marca> oLista = new List<Marca>();
            oLista = new CD_Marca().ListarMarcaTabla(pagina, tipoOrden, siguientes);
                return Json(new { data = oLista });
        }
        /************TABLA CON WHERE************/
        public JsonResult ListarMarcaTablaWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Marca> oLista = new List<Marca>();
            oLista = new CD_Marca().ListarMarcaTablaWhere(pagina, tipoOrden, siguientes, where);
            return Json(new { data = oLista });
        }
        public JsonResult countTablaWhere(string where)
        {
            int registros = 0;
            registros = new CD_Marca().countTablaWhere(where);
            return Json(new { registros = registros });

        }
        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tMarcas");
            return Json(new { campos = campos } );

        }
    }
}
using CapaEntidad;
using CapaDatos;
using RefaccionariaLosMochis.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CapaEntidad;
using CapaDatos;
using RefaccionariaLosMochis.Permisos;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RefaccionariaLosMochis.Controllers
{
    public class AlmacenController : Controller
    {
        // GET: Almacen
        [PermisosRol("I")]
        public ActionResult Almacenes()
        {


            return View();
        }

        [HttpPost]
        public JsonResult GuardarAlmacen(Almacen objeto)
        {
            object resultado;
            string mensaje = string.Empty;
            objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"] ?? "0");
            if (objeto.AlmacenId == 0)
            {
                resultado = new CD_Almacen().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CD_Almacen().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }); ;

        }
        [HttpPost]
        public JsonResult EliminarAlmacen(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Almacen().EliminarAlmacen(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }
        [HttpPost]
        public JsonResult EliminarRack(int almacenId, int rackId)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_AlmacenRack().EliminarRack(almacenId, rackId, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }
        //Resultado un objeto que es el ultimo modificado 
        [HttpPost]
        public JsonResult UltimoRegistro()
        {
            Almacen oAlmacen = new Almacen();
            oAlmacen = new CD_Almacen().UltimoRegistro();
            return Json(new { Lista = oAlmacen });

        }
        //RACK
        [HttpPost]
        public JsonResult GuardarAlmacenRack(AlmacenRack objeto, bool boolGuardar)
        {
            object resultado;
            string mensaje = string.Empty;

            objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"] ?? "0");
            if (boolGuardar)
            {
                resultado = new CD_AlmacenRack().RegistrarRack(objeto, out mensaje);
            }
            else
            {
                resultado = new CD_AlmacenRack().EditarRack(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }); ;

        }
        public JsonResult SeleccionarAlmacen()
        {
            string mensaje;
            List<Almacen> oLista = new List<Almacen>();
            oLista = new CD_Almacen().ObtenerAlmacenes(out mensaje);
            return Json(new { data = oLista, mensaje = mensaje });
        }
        public JsonResult SeleccionarAlmacenRack(int id)
        {
            string mensaje;
            List<AlmacenRack> oLista = new List<AlmacenRack>();
            oLista = new CD_AlmacenRack().ObtenerAlmacenRack(out mensaje, id);
            return Json(new { data = oLista, mensaje = mensaje });
        }
        //Seccion
        [HttpPost]
        public JsonResult GuardarAlmacenRackSeccion(AlmacenRackSeccion objeto, bool boolGuardar)
        {
            object resultado;
            string mensaje = string.Empty;
            objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"] ?? "0");
            if (true)
            {
                resultado = new CD_AlmacenRackSeccion().RegistrarSeccionRack(objeto, out mensaje);
            }
            else
            {
                //resultado = new CD_Almacen().(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }); ;

        }
        //Seccion
        [HttpPost]
        public JsonResult EliminarSeccion(AlmacenRackSeccion Objeto)
        {

            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_AlmacenRackSeccion().EliminarSeccion(Objeto, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }
        //Resultado un objeto que es el ultimo modificado 
        [HttpPost]
        public JsonResult UltimoRegistroRack(int id)
        {
            AlmacenRack oAlmacen = new AlmacenRack();
            oAlmacen = new CD_AlmacenRack().UltimoRegistroRack(id);
            return Json(new { Lista = oAlmacen });

        }
        [HttpPost]
        public JsonResult ObtenerSeccionesPorAlmacenYRack(int almacenId, int rackId)
        {
            List<AlmacenRackSeccion> seccionesList = new CD_AlmacenRackSeccion().ObtenerSeccionesPorAlmacenYRack(almacenId, rackId);
            return Json(new { data = seccionesList });
        }

        [HttpPost]
        public JsonResult SeleccionarAlmacenRackSeccion(int rackId)
        {
            string mensaje;

            List<AlmacenRackSeccion> seccionesList = new CD_AlmacenRackSeccion().ObtenerRackSeccion(out mensaje, rackId);
            return Json(new { data = seccionesList, mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tAlmacenes");
            return Json(new { campos = campos });

        }



        [HttpPost]
        public JsonResult CountTablaAlmacen()
        {
            int registros = new CD_Almacen().CountTablaAlmacen();
            return Json(new { registros = registros });
        }
        [HttpPost]
        public JsonResult ListarAlmacen(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Almacen> oLista = new CD_Almacen().ListarAlmacen(pagina, tipoOrden, siguientes);
            return Json(new { data = oLista });
        }




        [HttpPost]
        public JsonResult CountTablaRack()
        {
            int registros = new CD_AlmacenRack().CountTablaRack();
            return Json(new { registros = registros }   );
        }

        [HttpPost]
        public JsonResult ListarRacks(string strpagina, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<AlmacenRack> oLista = new CD_AlmacenRack().ListarRacks(pagina, siguientes);
            return Json(new { data = oLista }   );
        }

        [HttpPost]
        public JsonResult ObtenerRacksPorAlmacen(int almacenId)
        {
            List<AlmacenRack> oLista = new CD_AlmacenRack().ObtenerRacksPorAlmacen(almacenId);
            return Json(new { data = oLista });
        }

        /************BUSCADOR************/
        // Resultado un entero que es la cantidad de elementos que contienen el string que entró
        [HttpPost]
        public async Task<JsonResult> CountBuscadorAlmacenAsync(string nombre)
        {
            int registros = await new CD_Almacen().CountBuscadorAlmacen(nombre);
            return Json(new { registros = registros });
        }

        // Resultado una lista de objetos que contienen el string que entró
        [HttpPost]
        public JsonResult ElementosPaginacionBuscadorAlmacen(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (!string.IsNullOrEmpty(nombre))
            {
                lista = new CD_Almacen().ElementosPaginacionBuscadorAlmacen(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });
        }
        [HttpPost]
        public JsonResult bucarAlmacenPorNombre(string nombre)
        {
            Almacen almacen = new Almacen(); // Asumiendo que tienes una clase Almacen y una clase CD_Almacen con el método correspondiente
            if (!string.IsNullOrEmpty(nombre)) // Mejor uso de la verificación de strings
            {
                almacen = new CD_Almacen().BuscarAlmacenPorDescripcion(nombre);
            }
            // Devolver directamente las propiedades del almacén en la respuesta JSON
            return Json(new{ Lista = almacen});
        }
        [HttpPost]
        public JsonResult bucarAlmacenPorUbicacion(string Ubicacion)
        {
            Almacen almacen = new Almacen(); // Asumiendo que tienes una clase Almacen y una clase CD_Almacen con el método correspondiente
            if (!string.IsNullOrEmpty(Ubicacion)) // Mejor uso de la verificación de strings
            {
                almacen = new CD_Almacen().BuscarAlmacenPorUbicacion(Ubicacion);
            }
            // Devolver directamente las propiedades del almacén en la respuesta JSON
            return Json(new{ Lista = almacen});
        }
        [HttpPost]
        public JsonResult bucarRackPorUbicacion(string Ubicacion)
        {
            string mensaje = "";

            AlmacenRack almacen = new AlmacenRack(); // Asumiendo que tienes una clase Almacen y una clase CD_Almacen con el método correspondiente
            if (!string.IsNullOrEmpty(Ubicacion)) // Mejor uso de la verificación de strings
            {
                almacen = new CD_AlmacenRack().ObtenerRackUbicacion(out mensaje,Ubicacion);
            }
            // Devolver directamente las propiedades del almacén en la respuesta JSON
            return Json(new{ Lista = almacen, mensaje=mensaje});
        }
        [HttpPost]
        public JsonResult bucarSeccionPorUbicacion(string Ubicacion)
        {
            string mensaje = "";

            AlmacenRackSeccion almacen = new AlmacenRackSeccion(); // Asumiendo que tienes una clase Almacen y una clase CD_Almacen con el método correspondiente
            if (!string.IsNullOrEmpty(Ubicacion)) // Mejor uso de la verificación de strings
            {
                almacen = new CD_AlmacenRackSeccion().ObtenerSeccionUbicacion(out mensaje,Ubicacion);
            }
            // Devolver directamente las propiedades del almacén en la respuesta JSON
            return Json(new{ Lista = almacen, mensaje=mensaje});
        }

        [HttpPost]
        public JsonResult BuscarAlmacenPorId(int Id)
        {
            Almacen oAlmacen = new Almacen();
            if (Id != 0)  
            {
                oAlmacen = new CD_Almacen().BuscarAlmacenPorId(Id);
            }
            return Json(new { Lista = oAlmacen });
        }


    }
}
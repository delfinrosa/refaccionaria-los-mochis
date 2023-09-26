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


namespace RefaccionariaLosMochis.Controllers
{
    public class MantenedorController : Controller
    {
        #region Marca
        //Marca
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
        //Marca
        public ActionResult Linea()
        {
            return View();
        }

        [HttpGet]
        //Listar
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

            if (objeto.IdLinea == 0)
            {
                resultado = new CN_Linea().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Linea().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet); ;

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
        #endregion


        #region Producto
        //Marca
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

        public JsonResult GuardarProducto(string objeto ,string archivoImagen)
        {
    
            string directorio = Path.GetFullPath(""); // Obtiene el directorio donde se encuentra el archivo
            string nombreArchivo = Path.GetFileNameWithoutExtension(""); // Obtiene el nombre del archivo sin la extensión
            string extension = Path.GetExtension(""); // Obtiene la extensión del archivo

            // Juntar todas las partes en un solo string
            string rutaCompleta = Path.Combine(directorio, nombreArchivo + extension);

            new CN_Producto().imagen(rutaCompleta);



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

    }
}

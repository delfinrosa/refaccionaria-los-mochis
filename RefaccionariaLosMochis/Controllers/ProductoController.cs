using CapaEntidad;
using CapaDatos;
using RefaccionariaLosMochis.Permisos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]

    public class ProductoController : Controller
    {

        //Producto
        [PermisosRol("I")]

        public ActionResult Producto()
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
        public ActionResult FaltaDescripcion()
        {

            return View();
        }

        //[HttpPost]
        ////Listar
        //public JsonResult ListarProducto()
        //{
        //    List<Producto> oLista = new List<Producto>();
        //    oLista = new CD_Producto().Listar();
        //    return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public IActionResult ListarImagenes(int id)
        {
            // Obtener las imágenes del producto a través del método correspondiente
            List<(string, int)> imagenes = new CD_Producto().ListarImagenesProducto(id);

            // Crear y devolver un JsonResult con las imágenes y sus IDs
            return new JsonResult(new
            {
                imagenes = imagenes.Select(img => img.Item1).ToList(),
                ids = imagenes.Select(img => img.Item2).ToList()
            });
        }

        [HttpPost]
        public JsonResult UltimoRegistro()
        {
            Producto oProducto = new Producto();
            oProducto = new CD_Producto().UltimoRegistro();
            return Json(new { data = oProducto });
        }
        [HttpPost]
        public JsonResult BuscarProductoPorNombre(string nombre)
        {
            Producto oProducto = new Producto();
            oProducto = new CD_Producto().BuscarProductoPorNombre(nombre);
            return Json(new { data = oProducto });
        }
        [HttpPost]
        public JsonResult BuscarProductoPorNoParteYMarca(string nombre, string id)
        {
            Producto oProducto = new Producto();
            oProducto = new CD_Producto().BuscarProductoPorNoParteYMarca(nombre,id);
            return Json(new { data = oProducto });
        }
        [HttpPost]
        public JsonResult dsds(string nombre, string marca)
        {
            Producto oProducto = new Producto();
            oProducto = new CD_Producto().BuscarProductoPorNoParteYMarcaDescripcion(nombre, marca);
            return Json(new { data = oProducto });
        }

        //Guardar

        [HttpPost]
        public JsonResult GuardarProducto(Producto objeto)
        {
            var resultado=false;
            string mensaje = string.Empty;

            objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());

            if (objeto.IdProducto == 0)
            {
                objeto.IdProducto = new CD_Producto().Registrar(objeto, out mensaje);

            }
            else
            {
                resultado = new CD_Producto().Editar(objeto, out mensaje);
            }
            return Json(new { IdGenerado = objeto.IdProducto, mensaje = mensaje, resultado= resultado });
        }
        [HttpPost]
        public IActionResult GuardarProductoImagenes(int id, List<IFormFile> files)
        {
            List<string> base64Images = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0) // Verifica que el archivo tenga contenido
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream); // Copia el archivo a un MemoryStream
                        byte[] imageData = memoryStream.ToArray(); // Obtiene los bytes del archivo
                        string base64String = Convert.ToBase64String(imageData); // Convierte a Base64
                        base64Images.Add(base64String);
                    }
                }
            }

            new CD_Producto().GuardarDatosImagen(id, base64Images); // Guarda las imágenes en la base de datos
            return Json(new { success = true }); // Devuelve un JSON de éxito
        }



        //PRUEBA MOSTRAR IMAGEN
        public ActionResult ObtenerImagenesDesdeBaseDeDatos(int idProducto)
        {
            List<(string, int)> imagenes = new CD_Producto().ListarImagenesProducto(idProducto);
            return Json(new { Imagenes = imagenes });
        }

        //Resultado una lista de objetos que contienen el string que entro
        [HttpPost]
        public JsonResult LineaElementosPaginacionBuscadorDescripcionID(string nombre, int pagina, int siguientes)
        {
            List<Linea> lista = new List<Linea>();
            if (nombre != null || nombre != "")
            {
                lista = new CD_Linea().elementosPaginacionBuscadorDescripcionID(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });

        }
        //Resultado una lista de objetos que contienen el string que entro
        [HttpPost]
        public JsonResult MarcaElementosPaginacionBuscadorDescripcionID(string nombre, int pagina, int siguientes)
        {
            List<Marca> lista = new List<Marca>();
            if (nombre != null || nombre != "")
            {
                lista = new CD_Marca().elementosPaginacionBuscadorDescripcionID(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });

        }


        [HttpPost]
        //Eliminar
        public JsonResult EliminarProducto(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Producto().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }
        //Eliminar IMG
        public JsonResult EliminarIMGProducto(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Producto().EliminarIMG(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }
        /************BUSCADOR************/
        //Resultado un entero que es la cantidad de elementos que contienen el string que entro
        [HttpPost]
        public JsonResult countBuscadorProducto(string nombre)
        {
            int registros = 0;
            registros = new CD_Producto().countBuscador(nombre);
            return Json(new { registros = registros });

        }
        //Resultado una lista de objetos que contienen el string que entro
        [HttpPost]
        public JsonResult elementosPaginacionBuscadorProducto(string nombre, int pagina, int siguientes)
        {
            List<Producto> lista = new List<Producto>();
            if (nombre != null || nombre != "")
            {
                lista = new CD_Producto().elementosPaginacionBuscador(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });

        }
        //Resultado una lista de objetos que contienen el string que entro
        [HttpPost]
        public JsonResult elementosPaginacionTablaProducto(int pagina, int siguientes, string orden)
        {
            List<Producto> lista = new List<Producto>();
            lista = new CD_Producto().ListarProductos(pagina, siguientes, orden);
            return Json(new { Lista = lista });

        }        


        [HttpPost]
        public JsonResult countTabla()
        {
            int registros = 0;
            registros = new CD_Producto().countTabla();
            return Json(new { registros = registros });

        }
        //************************WHERE
        [HttpPost]
        public JsonResult elementosPaginacionTablaProductoWhere(int pagina, int siguientes, string orden, string where)
        {
            List<Producto> lista = new List<Producto>();
            lista = new CD_Producto().ListarProductosWhere(pagina, siguientes, orden, where);
            return Json(new { Lista = lista });

        }

        [HttpPost]
        public JsonResult countTablaWhere(string where)
        {
            int registros = 0;
            registros = new CD_Producto().countTablaWhere(where);
            return Json(new { registros = registros });

        }

        //Guardar

        [HttpPost]
        public JsonResult GuardarProductoProveedor(ProductoProveedor objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());
            
                objeto.IdProductoProveedor = new CD_Producto().RegistrarProductoProveedor(objeto, out mensaje);


            return Json(new { IdGenerado = objeto.IdProductoProveedor, mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult tamaño() { 
            var campos = new Recursos().Tamaño("tProductos");
            return Json(new { campos = campos });

        }          
        [HttpPost]
        public JsonResult tamañoValor() { 
            var campos = new Recursos().Tamaño("tProductosLineasCaracteristicas");
            return Json(new { campos = campos });

        }            
        [HttpPost]
        public JsonResult tamañoProvedor() { 
            var campos = new Recursos().Tamaño("tProductosProveedores");
            return Json(new { campos = campos });

        }      
        public JsonResult SeleccionarProductoProveedor(int id)
        {
            string mensaje = string.Empty;
            List<ProductoProveedor> data = new List<ProductoProveedor>();
            data = new CD_Producto().SeleccionarProductoProveedor(id, out mensaje);
            return Json(new { data = data, mensaje = mensaje });

        }

        [HttpPost]
        //Eliminar
        public JsonResult EliminarProductoProveedor(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Producto().EliminarProductoProveedor(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }); ;
        }

        [HttpPost]
        public JsonResult CountBuscadorAsync(string nombre)
        {
            int registros =  new CD_Marca().CountBuscador(nombre);
            return Json(new { registros = registros });
        }

        [HttpPost]
        public JsonResult ObtenerIdProductoConNoparteMarca(Producto producto)
        {
            try
            {
                int idProducto = new CD_Producto().ObtenerIdConNoparteMarca(producto);

                return Json(new { IdProducto = idProducto });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        //************************WHERE PENDIETE
        [HttpPost]
        public JsonResult BuscarPendientes()
        {
            List<Producto> lista = new List<Producto>();
            lista = new CD_Producto().BuscarPendientes();
            return Json(new { Lista = lista });

        }
        [HttpPost]
        public JsonResult GuardarPendientes(int id, string valor)
        {
            bool resultado = false;
            string mensaje = "";
            Producto oProducto = new Producto();
            oProducto.IdProducto = id;
            oProducto.Valor = valor;

            resultado = new CD_Producto().RegistrarPendiente(oProducto, out mensaje);

            return Json(new { resultado = resultado, mensaje = mensaje });
        }


        [HttpPost]
        public JsonResult BuscarProductosPorCodigoBarras(string codigoBarras)
        {
            List<Producto> oProducto = new List<Producto>();
            oProducto = new CD_Producto().BuscarProductosPorCodigoBarras(codigoBarras);
            return Json(new { data = oProducto });
        }

        [HttpPost]

        public ActionResult ExportarExcel(string query)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<Producto> productos = new CD_Producto().ExportarExcel(query);

            using (ExcelPackage excel = new ExcelPackage())
            {
                var hoja = excel.Workbook.Worksheets.Add("Productos");

                int columna = 1; // Controla en qué columna estamos añadiendo datos

                // Identificamos qué columnas están presentes según el query
                string queryLower = query.ToLower();
                bool hasIdProducto = queryLower.Contains("idproducto");
                bool hasDescripcion = queryLower.Contains("productodescripcion");
                bool hasPrecio = queryLower.Contains("precio");
                bool hasMinimo = queryLower.Contains("minimo");
                bool hasMaximo = queryLower.Contains("maximo");
                bool hasNoParte = queryLower.Contains("noparte");
                bool hasCodigoBarras = queryLower.Contains("codigobarras");
                bool hasLineaDescripcion = queryLower.Contains("lineadescripcion");
                bool hasMarcaDescripcion = queryLower.Contains("marcadescripcion");
                bool hasActivo = queryLower.Contains("activo");
                bool hasAlmacenDescripcion = queryLower.Contains("almacendescripcion");
                bool hasRackDescripcion = queryLower.Contains("rackdescripcion");
                bool hasSeccionDescripcion = queryLower.Contains("secciondescripcion");
                bool hasCantidadDisponible = queryLower.Contains("cantidaddisponible");

                // Agregamos los encabezados dinámicamente según las columnas presentes
                if (hasIdProducto)
                    hoja.Cells[1, columna++].Value = "IdProducto";
                if (hasNoParte)
                    hoja.Cells[1, columna++].Value = "NoParte";
                if (hasDescripcion)
                    hoja.Cells[1, columna++].Value = "Descripción";
                if (hasMarcaDescripcion)
                    hoja.Cells[1, columna++].Value = "Marca";
                if (hasPrecio)
                    hoja.Cells[1, columna++].Value = "Precio";                
                if (hasCantidadDisponible)
                    hoja.Cells[1, columna++].Value = "Cantidad Disponible";
                if (hasMinimo)
                    hoja.Cells[1, columna++].Value = "Mínimo";
                if (hasMaximo)
                    hoja.Cells[1, columna++].Value = "Máximo";
                if (hasCodigoBarras)
                    hoja.Cells[1, columna++].Value = "Código de Barras";
                if (hasLineaDescripcion)
                    hoja.Cells[1, columna++].Value = "Línea";
                if (hasActivo)
                    hoja.Cells[1, columna++].Value = "Activo";
                if (hasAlmacenDescripcion)
                    hoja.Cells[1, columna++].Value = "Almacén";
                if (hasRackDescripcion)
                    hoja.Cells[1, columna++].Value = "Rack";
                if (hasSeccionDescripcion)
                    hoja.Cells[1, columna++].Value = "Sección";   

                // Rellenamos las filas con los datos de los productos
                int fila = 2;
                foreach (var producto in productos)
                {
                    columna = 1; // Reiniciamos la columna para cada fila

                    if (hasIdProducto)
                        hoja.Cells[fila, columna++].Value = producto.IdProducto;
                    if (hasNoParte)
                        hoja.Cells[fila, columna++].Value = producto.NoParte;
                    if (hasDescripcion)
                        hoja.Cells[fila, columna++].Value = producto.Descripcion;
                    if (hasMarcaDescripcion)
                        hoja.Cells[fila, columna++].Value = producto.oMarca?.Descripcion;
                    if (hasPrecio)
                        hoja.Cells[fila, columna++].Value = producto.Precio;                    
                    if (hasCantidadDisponible)
                        hoja.Cells[fila, columna++].Value = producto.oUsuario.IdUsuario;
                    if (hasMinimo)
                        hoja.Cells[fila, columna++].Value = producto.Minimo;
                    if (hasMaximo)
                        hoja.Cells[fila, columna++].Value = producto.Maximo;
                    if (hasCodigoBarras)
                        hoja.Cells[fila, columna++].Value = producto.CodigoBarras;
                    if (hasLineaDescripcion)
                        hoja.Cells[fila, columna++].Value = producto.oLinea?.Descripcion;
                    if (hasActivo)
                        hoja.Cells[fila, columna++].Value = producto.Activo;
                    if (hasAlmacenDescripcion)
                        hoja.Cells[fila, columna++].Value = producto.oAlmacen?.Descripcion;
                    if (hasRackDescripcion)
                        hoja.Cells[fila, columna++].Value = producto.oRack?.Descripcion;
                    if (hasSeccionDescripcion)
                        hoja.Cells[fila, columna++].Value = producto.oSeccion?.Descripcion;

                    fila++;
                }

                // Exportamos el archivo Excel
                var archivoExcel = excel.GetAsByteArray();
                return File(archivoExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "productos.xlsx");
            }
        }
        [HttpPost]
        public JsonResult NombreColumnasExcel()
        {
            List<string> lista = new List<string>();
            lista = new CD_Producto().NombreColumnasExcel();
            
            return Json(new { Lista = lista }   );

        }

    }
}
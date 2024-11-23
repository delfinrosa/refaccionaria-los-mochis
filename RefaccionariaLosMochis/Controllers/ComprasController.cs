using CapaEntidad;
using CapaDatos;
using Microsoft.AspNetCore.Mvc;

using iTextSharp.text;
using iTextSharp.text.pdf;

using Rotativa;
using Rotativa.AspNetCore;
using Rotativa.Options;
using Org.BouncyCastle.Utilities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore; // Asegúrate de que esta referencia sea la correcta
using Rotativa.AspNetCore.Options;
using System.Drawing; // Asegúrate de tener este espacio de nombres
using System.IO;
using RefaccionariaLosMochis.Permisos;
using Microsoft.AspNetCore.Authorization;


namespace RefaccionariaLosMochis.Controllers
{
        [Authorize]
    public class ComprasController : Controller
    {
        [PermisosRol("I")]

        // GET: Compras
        public ActionResult Administracion()
        {

            return View();
        }
        public ActionResult Compras()
        {

            return View();
        }
        public ActionResult ProductosComprados()
        {

            return View();
        }



        public ActionResult RecibirMercanciaIncompleto(string idCompra, string productos)
        {
            // Deserializar el JSON recibido a una lista de objetos con la estructura adecuada
            var productosIncompletos = JsonConvert.DeserializeObject<List<CompraDtl>>(productos);

            // Pasar los datos a la vista
            ViewBag.ProductosIncompletos = productosIncompletos;
            ViewBag.IdCompra = idCompra;

            return View(productosIncompletos); // Pasar los datos como modelo a la vista
        }




        public ActionResult RecibirMercancia(string idCompra)
        {
            string mensaje;
            var detallesCompra = new CD_Compra().ObtenerDetallesCompra(idCompra, out mensaje);

            if (!string.IsNullOrEmpty(mensaje))
            {
                ViewBag.ErrorMessage = mensaje;
            }


            return View(detallesCompra);
        }


        public ActionResult CompraPDF(string idCompra)
        {
            try
            {
                // Generar el código QR
                Bitmap qrCodeImage = new Recursos().GenerarCodigoQR(idCompra);

                // Convertir el QR a Base64
                string codigoQRBase64 = new Recursos().BitmapToBase64(qrCodeImage);

                // Obtener los detalles de la compra
                var detallesCompra = new CD_CompraDTL().ObtenerCompraDtlPorCompraId(idCompra);

                if (detallesCompra == null || detallesCompra.Count == 0)
                {
                    return NotFound("No se encontraron detalles para esta compra.");
                }

                // Construir el modelo principal
                var modeloPDF = new OrdenCompraPDF
                {
                    IdCompra = idCompra,
                    Usuario = detallesCompra[0].UsuarioModificacion.Nombre,
                    Fecha = detallesCompra[0].Compra.Fecha,
                    Estatus = detallesCompra[0].Compra.Estatus switch
                    {
                        "AB" => "Abierto",
                        "CE" => "Cerrado",
                        "CA" => "Cancelado",
                        "AP" => "Aprobado",
                        _ => "Desconocido"
                    },
                    CodigoQRBase64 = codigoQRBase64, // Agregar el código QR
                    Detalles = detallesCompra.Select(detalle => new DetalleOrdenCompra
                    {
                        Proveedor = detalle.oProductoProveedor.oProveedor.RazonSocial,
                        DescripcionProducto = detalle.oProductoProveedor.oProducto.Descripcion,
                        NoParte = detalle.oProductoProveedor.oProducto.NoParte,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = Convert.ToDecimal(detalle.oProductoProveedor.Precio)
                    }).ToList()
                };

                // Generar el PDF
                var pdf = new Rotativa.AspNetCore.ViewAsPdf("CompraPDF", modeloPDF)
                {
                    FileName = $"Compra_{idCompra}.pdf"
                };

                return pdf;
            }
            catch (Exception ex)
            {
                // Manejo básico de errores
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Ocurrió un error al generar el PDF.");
            }
        }




        //BUSCADOR  RFC PROVEEDORES
        public JsonResult elementosPaginacionBuscadorRFC(string RFC, int pagina, int siguientes)
        {
            List<Proveedor> data = new List<Proveedor>();
            data = new CD_Proveedor().elementosPaginacionBuscadorRFC(RFC, pagina, siguientes);
            return Json(new { data = data });
        }
        //BUSCADOR  RazonSocial PROVEEDORES
        public JsonResult elementosPaginacionBuscadorRazonSocial(string RS, int pagina, int siguientes)
        {
            List<Proveedor> data = new List<Proveedor>();
            data = new CD_Proveedor().elementosPaginacionBuscadorRazonSocial(RS, pagina, siguientes);
            return Json(new { data = data });
        }
        //COUNT BUSCADOR  RazonSocial PROVEEDORES
        public JsonResult countBuscadorRazonSocial(string where)
        {
            int registros = 0;
            registros = new CD_Proveedor().countBuscadorRazonSocial(where);
            return Json(new { registros = registros });

        }
        //BUSCADOR  DESCRIPCION PRODUCTOS
        public JsonResult elementosBuscadorProductosDescripcion(string RS, int pagina, int siguientes)
        {
            List<Producto> data = new List<Producto>();
            data = new CD_Producto().elementosBuscadorProductosDescripcion(RS, pagina, siguientes);
            return Json(new { data = data }  );
        }
        //COUNT BUSCADOR  DESCRIPCION PRODUCTOS
        public JsonResult countProductosDescripcion(string where)
        {
            int registros = 0;
            registros = new CD_Producto().countProductosDescripcion(where);
            return Json(new { registros = registros });
        }
        //BUSCADOR  ID PRODUCTOS
        public JsonResult elementosBuscadorProductosID(string ID, int pagina, int siguientes)
        {
            List<Producto> data = new List<Producto>();
            data = new CD_Producto().elementosBuscadorProductosID(ID, pagina, siguientes);
            return Json(new { data = data });
        }
        //COUNT BUSCADOR  ID PRODUCTOS
        public JsonResult countProductosID(string where)
        {
            int registros = 0;
            registros = new CD_Producto().countProductosID(where);
            return Json(new { registros = registros });
        }



        [HttpPost]
        public JsonResult GuardarCompraProducto(CompraProducto objeto)
        {
            object resultado;
            string mensaje = string.Empty;
            // Asumiendo que el objeto CompraProducto también tiene un atributo para el Usuario que lo está guardando/editando.
            objeto.Usuario.IdUsuario = Convert.ToInt32(HttpContext.Request.Cookies["idUsuario"] ?? "0");

            if (objeto.IdCompraProducto == 0) // Si es una nueva compra de producto
            {
                resultado = new CD_CompraProducto().RegistrarCompraProducto(objeto, out mensaje);
            }
            else // Si se está editando una compra de producto existente
            {
                resultado = new CD_CompraProducto().EditarCompraProducto(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }        );
        }

        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tClientes");
            return Json(new { campos = campos });

        }






        [HttpPost]
        public JsonResult ObtenerCompraDtlPorNoParte(string noParte)
        {
            List<CompraDtl> data = new List<CompraDtl>();
            string mensaje = string.Empty;

            try
            {
                data = new CD_CompraDTL().ObtenerCompraDtlPorNoParte(noParte, out mensaje);
                if (data.Count == 0)
                {
                    mensaje = "No se encontraron registros para el número de parte proporcionado.";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener detalle de compra por número de parte: " + ex.Message;
            }
            return Json(new { data = data, Mensaje = mensaje });
        }

        [HttpPost]
        public JsonResult ListarCompra(int pagina, int siguientes, string estatus)
        {
            List<(string IdCompra, string Proveedor, int Cantidad, decimal Precio)> listaCompras = new List<(string, string, int, decimal)>();
            string mensaje = string.Empty;

            try
            {
                var compras = new CD_Compra().ListarCompra(pagina, siguientes, estatus);

                listaCompras = compras.Select(c => (c.Item1, c.Item2, c.Item3, c.Item4)).ToList();

                // Mapeo a un objeto anónimo para JSON
                var result = listaCompras.Select(c => new {
                    idCompra = c.Item1,
                    proveedor = c.Item2,
                    cantidad = c.Item3,
                    precio = c.Item4
                }).ToList();

                if (listaCompras.Count == 0)
                {
                    mensaje = "No se encontraron compras con el estatus proporcionado.";
                }

                return Json(new { data = result, Mensaje = mensaje });
            }
            catch (Exception ex)
            {
                mensaje = "Error al listar compras por estatus: " + ex.Message;
                return Json(new { data = new List<object>(), Mensaje = mensaje });
            }
        }




        [HttpPost]
        public JsonResult ContarComprasPorEstatus(string estatus)
        {
            int totalRegistros = 0;
            string mensaje = string.Empty;

            try
            {
                totalRegistros = new CD_Compra().CountCompras(estatus);

                if (totalRegistros == 0)
                {
                    mensaje = "No se encontraron compras con el estatus proporcionado.";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al contar compras por estatus: " + ex.Message;
            }

            return Json(new { TotalRegistros = totalRegistros, Mensaje = mensaje });
        }



        [HttpPost]
        public JsonResult GuardarCompra(List<CompraDtl> listaCompraDtl)
        {
            object resultado;
            string mensaje = string.Empty;
            bool bien = false;
            Guid idcompra = Guid.Empty; // Cambiando a Guid

            Compra objeto = new Compra
            {
                UsuarioModificacion = new Usuario { IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString()) }
            };

            if (true)
            {
                idcompra = new CD_Compra().RegistrarCompra(objeto, out mensaje);
                foreach (var CompraDtl in listaCompraDtl)
                {
                    bien = new CD_CompraDTL().RegistrarCompraDtl(CompraDtl, idcompra.ToString(), Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString()), out mensaje); // Convirtiendo Guid a string aquí
                }
                // AQUI QUIERO SE SE ENVIE idcompra Y SE HAGA UN PDF QUE DESPUES PUEDAN ABRIR 
                //GenerarPdfConQr(idcompra.ToString());
            }
            else
            {
                resultado = new CD_Compra().EditarCompra(objeto, out mensaje);
            }

            return Json(new { idcompra = idcompra.ToString(), mensaje = mensaje }); // Convirtiendo Guid a string aquí
        }
        [HttpPost]
        public JsonResult AgregarCompraDTL(List<CompraDtl> listaCompraDtl, string idCompra)
        {
            object resultado;
            string mensaje = string.Empty;
            bool bien = false;

            Compra objeto = new Compra
            {
                UsuarioModificacion = new Usuario { IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString()) }
            };

            if (true)
            {
                foreach (var CompraDtl in listaCompraDtl)
                {
                    bien = new CD_CompraDTL().RegistrarCompraDtl(CompraDtl, idCompra.ToString(), Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString()), out mensaje); // Convirtiendo Guid a string aquí
                }
                // AQUI QUIERO SE SE ENVIE idcompra Y SE HAGA UN PDF QUE DESPUES PUEDAN ABRIR 
                //GenerarPdfConQr(idcompra.ToString());
            }
            else
            {
                resultado = new CD_Compra().EditarCompra(objeto, out mensaje);
            }

            return Json(new { bien = bien, mensaje = mensaje }); // Convirtiendo Guid a string aquí
        }



        /////////////////////////////////
        ///PDF
        //////////////////////////////
        private string GuardarPdfEnServidor(string idCompra, byte[] pdfBytes)
        {
            try
            {
                string folderPath = @"C:\PDFs\Compras";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, $"Compra_{idCompra}.pdf");

                // Guardar el archivo en el servidor
                System.IO.File.WriteAllBytes(filePath, pdfBytes);

                return filePath; // Devolver la ruta completa donde se guardó el archivo
            }
            catch (Exception ex)
            {
                // Manejo de errores: registrar el error, mostrar un mensaje al usuario, etc.
                ViewBag.Error = $"Error al guardar el PDF en el servidor: {ex.Message}";
                return null;
            }
        }
        public FileResult DownloadPdf(string idCompra, byte[] pdfBytes)
        {
            try
            {

                string fileName = $"Compra_{idCompra}.pdf";

                // Devolver el archivo PDF como descarga al cliente sin especificar ruta
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al descargar el PDF: {ex.Message}";
                return File(pdfBytes, "application/pdf", "");

            }
        }



        public ActionResult GenerarPdfConQr(string idCompra)
        {
            try
            {
                // Generar el código QR
                Bitmap qrCodeImage = new Recursos().GenerarCodigoQR(idCompra);

                // Convertir el QR a Base64 (opcional)
                string codigoQRBase64 = new Recursos().BitmapToBase64(qrCodeImage);
                ViewBag.CodigoQRBase64 = codigoQRBase64;

                // Obtener datos para el PDF
                var model = new CD_CompraDTL().ObtenerCompraDtlPorCompraId(idCompra);

                ViewBag.ID = idCompra;
                ViewBag.USER = model[0].UsuarioModificacion.Nombre;
                ViewBag.FECHA = model[0].Compra.Fecha;

                switch (model[0].Compra.Estatus)
                {
                    case "AB":
                        ViewBag.ESTATUS = "Abierto";
                        break;
                    case "CE":
                        ViewBag.ESTATUS = "Cerrado";
                        break;
                    case "CA":
                        ViewBag.ESTATUS = "Cancelado";
                        break;
                    case "AP":
                        ViewBag.ESTATUS = "Aprobado";
                        break;
                    default:
                        ViewBag.ESTATUS = "Desconocido"; // Manejo por si hay un estatus inesperado
                        break;
                }

                // Configurar el PDF
                var pdf = new Rotativa.AspNetCore.ViewAsPdf("CompraPDF", model)
                {
                    FileName = $"Compra_{idCompra}.pdf", // Nombre del archivo PDF
                    PageSize = Rotativa.AspNetCore.Options.Size.Letter // Tamaño del papel
                };

                return pdf; // Retorna directamente el PDF
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.Error = $"Error al generar el PDF: {ex.Message}";
                return View("Error");
            }
        }


        public ActionResult GenerarPdfConQrProveedor(string idCompra)
        {
            try
            {
                // Obtener los detalles de la compra
                var detallesCompra = new CD_CompraDTL().ObtenerCompraDtlPorCompraId(idCompra);

                if (detallesCompra == null || detallesCompra.Count == 0)
                {
                    return NotFound("No se encontraron detalles para esta compra.");
                }

                // Construir el modelo principal
                var model = new OrdenCompraPDF
                {
                    IdCompra = idCompra,
                    Usuario = detallesCompra[0].UsuarioModificacion.Nombre,
                    Fecha = detallesCompra[0].Compra.Fecha,
                    Estatus = detallesCompra[0].Compra.Estatus switch
                    {
                        "AB" => "Abierto",
                        "CE" => "Cerrado",
                        "CA" => "Cancelado",
                        "AP" => "Aprobado",
                        _ => "Desconocido"
                    },
                    CodigoQRBase64 = null, // No se necesita el código QR para este caso
                    Detalles = detallesCompra.Select(detalle => new DetalleOrdenCompra
                    {
                        Proveedor = detalle.oProductoProveedor.oProveedor.RazonSocial,
                        DescripcionProducto = detalle.oProductoProveedor.oProducto.Descripcion,
                        NoParte = detalle.oProductoProveedor.oProducto.NoParte,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = 0 // No se necesita el precio unitario
                    }).ToList()
                };

                // Generar el PDF
                var pdf = new Rotativa.AspNetCore.ViewAsPdf("CompraPDFProveedor", model)
                {
                    FileName = $"CompraProveedor_{idCompra}.pdf"
                };

                return pdf;
            }
            catch (Exception ex)
            {
                // Manejo básico de errores
                Console.WriteLine($"Error al generar el PDF: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al generar el PDF.");
            }
        }



        //aciones tabla 
        public ActionResult OrdenCompra(string idCompra)
        {
            Bitmap qrCodeImage = new Recursos().GenerarCodigoQR(idCompra);

            string codigoQRBase64 = new Recursos().BitmapToBase64(qrCodeImage);
            ViewBag.CodigoQRBase64 = codigoQRBase64;

            var model = new CD_CompraDTL().ObtenerCompraDtlPorCompraId(idCompra);

            ViewBag.ID = idCompra;
            ViewBag.USER = model[0].UsuarioModificacion.Nombre;
            ViewBag.FECHA = model[0].Compra.Fecha; ;

            switch (model[0].Compra.Estatus)
            {
                case "AB":
                    ViewBag.ESTATUS = "Abierto";
                    break;
                case "CE":
                    ViewBag.ESTATUS = "Cerrado";
                    break;
                case "CA":
                    ViewBag.ESTATUS = "Cancelado";
                    break;
                case "AP":
                    ViewBag.ESTATUS = "Aprobado";
                    break;
            }


            return View(model);
        }

        [HttpPost]
        public JsonResult AprobarCompra(string compraId)
        {
            int aprovador = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());
            string mensaje = string.Empty;

            try
            {
                int totalRegistros = new CD_Compra().AprobarCompra(compraId, aprovador, out mensaje);

                if (totalRegistros == 0)
                {
                    mensaje = "No se aprobo compras con el estatus proporcionado. error:" + mensaje;
                }
                if (totalRegistros == 1)
                {
                    mensaje = "Se aprobo la compra.";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al contar compras por estatus: " + ex.Message;
            }

            return Json(new { Mensaje = mensaje });
        }
        [HttpPost]
        public JsonResult CancelarCompra(string compraId, string comentario)
        {
            int aprovador = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());
            string mensaje = string.Empty;

            try
            {
                int totalRegistros = new CD_Compra().CancelarCompra(compraId, aprovador, comentario, out mensaje);

                if (totalRegistros == 0)
                {
                    mensaje = "No se cancelar compras con el estatus proporcionado. error:" + mensaje;
                }
                if (totalRegistros == 1)
                {
                    mensaje = "Se cancelo la compra.";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al contar compras por estatus: " + ex.Message;
            }

            return Json(new { Mensaje = mensaje });
        }
        [HttpPost]
        public JsonResult EliminarCompra(string compraId )
        {
            string mensaje = string.Empty;

            try
            {
                int totalRegistros = new CD_Compra().EliminarCompra(compraId, out mensaje);

                if (totalRegistros == 0)
                {
                    mensaje = "No se pudo eliminar la compra. error:" + mensaje;
                }
                if (totalRegistros == 1)
                {
                    mensaje = "Se elimino la compra.";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al contar compras por estatus: " + ex.Message;
            }

            return Json(new { Mensaje = mensaje });
        }
        //eliminar CompraDTL

        [HttpPost]
        public JsonResult EliminarCompraDTL(string compraId)
        {
            string mensaje = string.Empty;

            try
            {
                int totalRegistros = new CD_CompraDTL().EliminarCompraDTL(compraId, out mensaje);

                if (totalRegistros == 0)
                {
                    mensaje = "No se pudo eliminar la compra. error:" + mensaje;
                }
                if (totalRegistros == 1)
                {
                    mensaje = "Se elimino la compra.";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al contar compras por estatus: " + ex.Message;
            }

            return Json(new { Mensaje = mensaje });
        }



        [HttpPost]
        public ActionResult ActualizarEstadoProducto(List<EstadoProducto> estadoProductos)
        {
            int idUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());

            string mensaje;
            var cdCompra = new CD_Compra();

            try
            {
                cdCompra.ActualizarCompraDtl(estadoProductos, idUsuario);
                return Json(new { exito = true, mensaje = "Datos actualizados correctamente." });
            }
            catch (Exception ex)
            {
                mensaje = $"Error al actualizar los datos: {ex.Message}";
                return Json(new { exito = false, mensaje });
            }
        }









        //Cambiar Catidad CompraDTL

        [HttpPost]
        public JsonResult ActualizarCompraDtl(List<EstadoProducto> estadoProductos, string idCompra)
        {
            try
            {
                int idUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());
                new CD_Compra().ActualizarCompraDtl(estadoProductos, idUsuario);

                // Retornar éxito
                return Json(new { Exito = true, Mensaje = "Actualización completada exitosamente." });
            }
            catch (Exception ex)
            {
                // Retornar error en caso de excepción
                return Json(new { Exito = false, Mensaje = "Error al actualizar detalles de la compra: " + ex.Message });
            }
        }


        public async Task<JsonResult> ActualizarCompraDtlIncompleto(List<CompraDtl> productosSinCantidad)
        {
            try
            {
                List<string> mensajes = new List<string>();
                int idUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());

                if (productosSinCantidad == null || !productosSinCantidad.Any())
                {
                    return Json(new { Exito = false, Mensaje = "No se recibieron datos para actualizar." });
                }

                var exito = await new CD_Compra().ActualizarCompraDtlIncompleto(productosSinCantidad, idUsuario);

                if (exito)
                {
                    return Json(new { Exito = true, Mensaje = "Datos actualizados correctamente.", Detalles = mensajes });
                }
                else
                {
                    return Json(new { Exito = false, Mensaje = "Ocurrió un problema al actualizar los datos.", Detalles = mensajes });
                }
            }
            catch (Exception ex)
            {
                // Capturar y devolver el mensaje de error
                return Json(new { Exito = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

        public JsonResult ObtenerComprasPorID(int id)
        {
            List<(string CompraId, string Proveedor, string FechaPedido, string FechaEntrega, string CantidadProductosPedidos, string TotalPedido)> compra = new List<(string, string, string, string, string, string)>();
            string error = "";

            try
            {
                compra = new CD_Compra().ObtenerComprasPorProductoID(id);

                // Mapeo de la lista de tuplas a un formato estructurado
                var listaMapeada = compra.Select(c => new
                {
                    CompraId = c.CompraId,
                    Proveedor = c.Proveedor,
                    FechaPedido = c.FechaPedido,
                    FechaEntrega = c.FechaEntrega,
                    CantidadProductosPedidos = c.CantidadProductosPedidos,
                    TotalPedido = c.TotalPedido
                }).ToList();

                return Json(new { compra = listaMapeada, error = error });
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return Json(new { compra = new List<object>(), error = error });
            }
        }



    }
}
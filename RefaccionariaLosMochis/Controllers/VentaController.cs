using CapaDatos;
using CapaEntidad;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Globalization;
using Font = System.Drawing.Font;
using System.Xml.Linq;
using Newtonsoft.Json;
using System;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Hosting.Server;
using Grpc.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RefaccionariaLosMochis.Permisos;
using Microsoft.AspNetCore.Components.Forms;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using Microsoft.AspNetCore.Authorization;

namespace RefaccionariaLosMochis.Controllers
{

    public class VentaController : Controller
    {
        public ActionResult CargarTicketParcial(string idVenta)
        {
            var data = new CD_Venta().informacionTicket(idVenta);
            Bitmap codigo = new Recursos().GenerarCodigoDeBarras(idVenta);
            string code = new Recursos().BitmapToBase64(codigo);

            // Pasar la información al ViewBag
            ViewBag.VentaData = data;
            ViewBag.CodigoBarras = code;

            // Retorna la vista parcial
            return PartialView("_TicketPartial");
        }



        // GET: Venta
        [PermisosRol("V")]

        public ActionResult Index()
        {
            return View();

        }

        [PermisosRol("C")]

        public ActionResult Caja()
        {
            return View();
        }
        [PermisosRol("A")]

        public ActionResult CorteCaja()
        {

            return View();
        }
        [PermisosRol("C")]

        public ActionResult ScanerDevolucion()
        {

            return View();
        }
        [PermisosRol("C")]

        public ActionResult Devolucion(string idVenta)
        {
            var carrito = new CD_Venta().ObtenerCarritoPorVenta(idVenta);
            ViewBag.Carrito = carrito;
            ViewBag.ID = idVenta;
            return View();
        }
        [PermisosRol("C")]

        public ActionResult ProductosVendidos()
        {
            return View();
        }
        public ActionResult ProductosDevueltos()
        {
            return View();
        }
        public ActionResult Ticket(string idVenta)
        {
            try
            {
                // Obtener datos del carrito
                var carrito = new CD_Venta().ObtenerCarritoPorVenta(idVenta);

                bool requiereFactura = carrito[0].RequiereFactura;

                // Configurar ViewBag con los datos necesarios para la vista
                ViewBag.Carrito = carrito;
                ViewBag.ID = idVenta;
                ViewBag.RequiereFactura = requiereFactura;

                return View();
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                ViewBag.Error = "Error al obtener el carrito.";
                return View();
            }
        }

        [HttpPost]
        public JsonResult TodosDescripcionProductos()
        {
            List<Producto> lista = new List<Producto>();
            lista = new CD_Venta().TodosDescripcionProductos();
            return Json(new { lista = lista }); ;

        }
        [HttpPost]
        public JsonResult BuscadorVenta(string descripcion, string anioCompleto, string anioAbreviado, string marcaCompleto, string marcaAbreviado, string modelo)
        {
            var resultado = new CD_Venta().BuscadorVentaInterno(descripcion, anioCompleto, anioAbreviado, marcaCompleto, marcaAbreviado, modelo);
            return Json(new
            {
                productosFiltrados = resultado.Item1,
                otrosProductos = resultado.Item2
            });
        }


        [HttpPost]
        public JsonResult BuscadorVenta2(string busqueda)
        {
            List<Producto> lista = new List<Producto>();
            lista = new CD_Venta().buscadorVenta(busqueda);
            return Json(new { lista = lista }); ;
        }

        [HttpPost]
        public JsonResult detalleProducto(int idproducto)
        {
            Producto lista = new Producto();
            lista = new CD_Venta().detalleProducto(idproducto);
            return Json(new { lista = lista }); ;
        }

        [HttpPost]
        public JsonResult BuscarProductos(string NoParte)
        {
            if (string.IsNullOrWhiteSpace(NoParte))
            {
                return Json(new { error = "El parámetro de búsqueda no puede estar vacío." });
            }

            try
            {
                // Realiza la búsqueda
                List<Producto> productos = new CD_Venta().busquedaVentaNoParte(NoParte);
                return Json(new { lista = productos });
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return Json(new { mensaje = "Error al realizar la búsqueda", detalle = ex.Message });
            }
        }





        [HttpPost]
        public JsonResult GuardarVenta(List<VentasProductos> listaVentasProductos, string nombre, int factura)
        {
            object resultado;
            string mensaje = string.Empty;
            bool bien = false;
            Guid idVenta = Guid.Empty; // Cambiando a Guid

            Venta objeto = new Venta
            {
                oVendedor = new Usuario { IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString()) },
            };

            if (true)
            {
                idVenta = new CD_Venta().RegistrarVenta(objeto, nombre, factura, out mensaje);
                foreach (var VentasProductos in listaVentasProductos)
                {
                    bien = new CD_Venta().RegistrarVentasProducto(VentasProductos, idVenta.ToString(), out mensaje); // Convirtiendo Guid a string aquí
                }
                // AQUI QUIERO SE SE ENVIE idcompra Y SE HAGA UN PDF QUE DESPUES PUEDAN ABRIR 
                //GenerarPdfConQr(idcompra.ToString());
            }

            return Json(new { idVenta = idVenta.ToString(), mensaje = mensaje, bien = bien }); // Convirtiendo Guid a string aquí
        }


        [HttpPost]
        public JsonResult CancelarVenta(string idVenta)
        {

            try
            {
                // Llama al método de la capa de datos para eliminar el producto
                new CD_Venta().CancelarVenta(idVenta);
            }
            catch (Exception ex)
            {
            }

            return Json(new { });
        }
        [HttpPost]
        public JsonResult ActualizarRequiereFactura(string idVenta, bool factura)
        {
            string mensaje = string.Empty;
            bool a = false;

            try
            {
                a = new CD_Venta().ActualizarRequiereFactura(idVenta, factura, out mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                a = false; // Asegura que a sea false en caso de excepción
            }

            return Json(new { mensaje = mensaje, a = a });
        }




        public JsonResult ObtenerVentasAgrupadas()
        {
            try
            {
                // Realiza la búsqueda
                List<(string NombreCliente, string NombreVendedor, int CantidadProductos, decimal TotalPagar, string idVenta)> ventasAgrupadas = new CD_Venta().ObtenerVentasAgrupadas();

                if (ventasAgrupadas == null || ventasAgrupadas.Count == 0)
                {
                    return Json(new { mensaje = "No se encontraron ventas." });
                }

                // Mapeo de las tuplas a objetos anónimos para la respuesta JSON
                var listaVentas = ventasAgrupadas.Select(v => new
                {
                    NombreCliente = v.NombreCliente,
                    NombreVendedor = v.NombreVendedor,
                    CantidadProductos = v.CantidadProductos,
                    TotalPagar = v.TotalPagar,
                    IdVenta = v.idVenta
                }).ToList();


                return Json(new { lista = listaVentas });
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return Json(new { mensaje = "Error al realizar la búsqueda", detalle = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult EliminarProducto(string idVentasProducto, string idVenta)
        {
            bool exito = false;
            bool eliminar = false;

            try
            {
                exito = new CD_Venta().EliminarProducto(idVentasProducto);

                if (exito)
                {
                    eliminar = new CD_Venta().EliminarVentaSiNoHayProductos(idVenta);
                }
            }
            catch (Exception ex)
            {
                exito = false;
            }

            return Json(new { exito, eliminar });
        }


        //public void ImprimirDocumento()
        //{
        //    PrintDocument pd = new PrintDocument();
        //    pd.PrintPage += (sender, e) =>
        //    {
        //        e.Graphics.DrawString("informacion", new Font("Arial", 12), Brushes.Black, 100, 100);
        //    };

        //    // Especificar la impresora (opcional)
        //    pd.PrinterSettings.PrinterName = @"\\192.168.0.250\EPSON TM-T20II Receipt5";

        //    // Imprimir
        //    pd.Print();
        //}
        //public ActionResult ImprimirTicket(string idVenta)
        //{
        //    var detalles = new CD_Venta().informacionTicket(idVenta);

        //    if (detalles.Count > 0)
        //    {
        //        var firstItem = detalles[0];
        //        ViewBag.ID = idVenta;
        //        ViewBag.USER = firstItem.Vendedor;
        //        ViewBag.FECHA = firstItem.Fecha.ToString("dd/MM/yyyy");
        //        ViewBag.NombreCliente = firstItem.NombreCliente;
        //    }

        //    return View("ImprimirTicket", detalles);
        //}


        //private void ImprimirPDF(string pdfPath)
        //{
        //    try
        //    {
        //        using (var pdfDoc = PdfiumViewer.PdfDocument.Load(pdfPath))
        //        {
        //            using (var printDocument = pdfDoc.CreatePrintDocument())
        //            {
        //                // Especificar la impresora
        //                printDocument.PrinterSettings.PrinterName = @"\\192.168.0.250\EPSON TM-T20II Receipt5";

        //                // Configurar opciones de impresión (opcional)
        //                printDocument.PrinterSettings.DefaultPageSettings.Landscape = true; // Si es necesario
        //                printDocument.DefaultPageSettings.PrinterSettings.Copies = 1; // Número de copias

        //                // Imprimir
        //                printDocument.Print();
        //            }
        //        }
        //    }
        //    catch (DllNotFoundException ex)
        //    {
        //        ViewBag.Error = $"No se puede cargar el archivo DLL 'pdfium.dll': {ex.Message}";
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        ViewBag.Error = $"Error al imprimir: {ex.Message}. Asegúrate de que la impresora esté disponible y configurada correctamente.";
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = $"Error inesperado al imprimir el PDF: {ex.Message}";
        //    }
        //}



        [HttpPost]
        //public ActionResult GenerarTicket(string idVenta)
        //{


        //    var detalles = new CD_Venta().informacionTicket(idVenta);

        //    if (detalles.Count > 0)
        //    {
        //        string directoryPath = Server.MapPath("~/Tickets/");
        //        if (!Directory.Exists(directoryPath))
        //        {
        //            Directory.CreateDirectory(directoryPath);
        //        }
        //        string filePath = Path.Combine(directoryPath, $"ticket_{idVenta}.pdf");
        //        GenerarPDF(idVenta, detalles, filePath);

        //        // Imprimir el PDF
        //       ImprimirPDF(filePath);

        //        // Enviar el archivo al navegador para que se descargue
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        //        string fileName = $"ticket_{idVenta}.pdf";
        //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
        //    }

        //    return View();
        //}

        public JsonResult InformacionTicket(string idVenta)
        {
            var data = new CD_Venta().informacionTicket(idVenta);
            Bitmap codigo = new Recursos().GenerarCodigoDeBarras(idVenta);
            string code = new Recursos().BitmapToBase64(codigo);
            return Json(new { data = data, codigo = code });
        }



        public string GenerarTicket(string idVenta)
        {
            var detalles = new CD_Venta().informacionTicket(idVenta);

            var resultado = new
            {
                Exito = false,
                Mensaje = "No hay detalles",
                Error = string.Empty
            };

            if (detalles.Count > 0)
            {
                try
                {
                    PrintDocument printDocument = new PrintDocument();
                    printDocument.PrinterSettings.PrinterName = @"\\REFACCIONARIALM\EPSON TM-T20II Receipt5";
                    printDocument.PrintPage += (sender, e) =>
                    {
                        float yPos = 0;
                        int leftMargin = e.MarginBounds.Left;
                        int topMargin = e.MarginBounds.Top;
                        Font printFont = new Font("Arial", 10);

                        // Encabezado del ticket
                        yPos = topMargin;
                        e.Graphics.DrawString("Refaccionaria Los Mochis", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString("CESAR MANUEL RAMIREZ PLACENCIA", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString("NATACION 106 NOGALES SONORA 84066", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString("RFC: RAPC890331J72", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString($"ID Venta: {idVenta}", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy}", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString($"Nombre del Cliente: {detalles[0].NombreCliente}", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString("--------------------------", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);

                        // Detalles del producto
                        e.Graphics.DrawString("Descripción        No Parte   Cantidad   Precio", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString("--------------------------", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);

                        foreach (var item in detalles)
                        {
                            string line = $"{item.Descripcion}  {item.NoParte}  {item.Cantidad}  {item.Precio:C}";
                            e.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos);
                            yPos += printFont.GetHeight(e.Graphics);
                        }

                        // Total y vendedor
                        e.Graphics.DrawString("--------------------------", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString($"Total: {detalles.Sum(x => x.Cantidad * x.Precio):C}", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString($"Vendedor: {detalles[0].Vendedor}", printFont, Brushes.Black, leftMargin, yPos);
                        yPos += printFont.GetHeight(e.Graphics);
                        e.Graphics.DrawString("--------------------------", printFont, Brushes.Black, leftMargin, yPos);
                    };

                    printDocument.Print();

                    resultado = new
                    {
                        Exito = true,
                        Mensaje = "Impresión realizada con éxito.",
                        Error = string.Empty
                    };
                }
                catch (Exception ex)
                {
                    resultado = new
                    {
                        Exito = false,
                        Mensaje = "Error al imprimir.",
                        Error = ex.Message
                    };
                }
            }

            return JsonConvert.SerializeObject(resultado);
        }





        public void GenerarPDF(string idVenta, List<Ticket> detalles, string savePath)
        {
            if (detalles.Count == 0)
                return;

            var firstItem = detalles[0];
            string vendedor = firstItem.Vendedor;
            string fecha = firstItem.Fecha.ToString("dd/MM/yyyy");
            string nombreCliente = firstItem.NombreCliente;

            float contenidoAltura = 0f;

            contenidoAltura += 120f; // Altura del logo
            contenidoAltura += 4 * 7 * 1.2f; // Información principal (4 líneas)
            contenidoAltura += detalles.Count * 10f; // Estimación de altura para cada fila de la tabla
            contenidoAltura += 50f; // Espacio para total y vendedor

            float pageWidth = 223f; // Ancho de la página en puntos
            float pageHeight = contenidoAltura;

            iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(pageWidth, pageHeight);
            Document document = new Document(pageSize, 10, 10, 0, 0);
            PdfWriter.GetInstance(document, new FileStream(savePath, FileMode.Create));

            document.Open();

            // Añadir información principal
            //var logoPath = Server.MapPath("~/Imagenes/LOGO-Musculoso-Circular-PNG.png");
            //var logo = iTextSharp.text.Image.GetInstance(logoPath);
            //logo.ScaleToFit(120f, 120f); // Ajustar el tamaño del logo
            //logo.Alignment = Element.ALIGN_CENTER;
            //document.Add(logo);

            document.Add(new Paragraph("Refaccionaria Los Mochis", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.BLACK)) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph("CESAR MANUEL RAMIREZ PLACENCIA", FontFactory.GetFont(FontFactory.HELVETICA, 7)) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph("NATACION 106 NOGALES SONORA 84066", FontFactory.GetFont(FontFactory.HELVETICA, 7)) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph("RFC: RAPC890331J72", FontFactory.GetFont(FontFactory.HELVETICA, 7)) { Alignment = Element.ALIGN_CENTER });

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph($"ID Venta: {idVenta}", FontFactory.GetFont(FontFactory.HELVETICA, 7)) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph($"Fecha: {fecha}", FontFactory.GetFont(FontFactory.HELVETICA, 7)) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph($"Nombre del Cliente: {nombreCliente}", FontFactory.GetFont(FontFactory.HELVETICA, 7)) { Alignment = Element.ALIGN_CENTER });

            document.Add(new Paragraph(" "));

            // Crear la tabla
            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 4, 2, 1, 1 });

            // Configurar la altura de las filas de la tabla
            table.DefaultCell.FixedHeight = 0.6f * 28.35f; // 0.6 cm en puntos

            table.AddCell(new PdfPCell(new Phrase("Descripción", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("No Parte", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Cantidad", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Precio", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7))) { HorizontalAlignment = Element.ALIGN_CENTER });

            decimal total = 0;
            foreach (var item in detalles)
            {
                decimal itemTotal = item.Cantidad * item.Precio;
                total += itemTotal;

                table.AddCell(new PdfPCell(new Phrase(item.Descripcion, FontFactory.GetFont(FontFactory.HELVETICA, 7))) { HorizontalAlignment = Element.ALIGN_LEFT });
                table.AddCell(new PdfPCell(new Phrase(item.NoParte, FontFactory.GetFont(FontFactory.HELVETICA, 7))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase(item.Cantidad.ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 7))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase(item.Precio.ToString("C", new CultureInfo("es-MX")), FontFactory.GetFont(FontFactory.HELVETICA, 7))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            }

            // Añadir fila de total
            table.AddCell(new PdfPCell(new Phrase("")) { Border = 0 });
            table.AddCell(new PdfPCell(new Phrase("")) { Border = 0 });
            table.AddCell(new PdfPCell(new Phrase("Total", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            table.AddCell(new PdfPCell(new Phrase(total.ToString("C", new CultureInfo("es-MX")), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7))) { HorizontalAlignment = Element.ALIGN_RIGHT });

            document.Add(table);

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph($"Vendedor: {vendedor}", FontFactory.GetFont(FontFactory.HELVETICA, 7)) { Alignment = Element.ALIGN_CENTER });

            document.Close();
        }





        [HttpPost]
        public JsonResult ActualizarVenta(Venta venta)
        {
            bool exito = false;
            venta.oCajero.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());
            try
            {
                if (venta != null && !string.IsNullOrEmpty(venta.IdVenta))
                {
                    exito = new CD_Venta().ActualizarVenta(venta);
                }
                bool factura = new CD_Venta().FacturaRequerida(venta);
                if (factura)
                {
                    XElement ventaXml = new XElement("Venta",
                                        new XElement("IdVenta", venta.IdVenta),
                                        new XElement("Fecha", "a"),
                                        new XElement("RequiereFactura", factura)
                                        );
                }
            }
            catch (Exception ex)
            {
                exito = false;
                string error = ex.ToString();
            }
            return Json(new { exito });
        }

        public JsonResult ListarComprasHoyEfectivo(string estatus)
        {
            try
            {
                List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos)> lista = new CD_Venta().ListarComprasHoyEfectivo(estatus);

                var listaMapeada = lista.Select(l => new
                {
                    IdVenta = l.IdVenta,
                    HoraMinuto = l.HoraMinuto,
                    TotalCompra = l.TotalCompra,
                    CantidadProductos = l.CantidadProductos
                }).ToList();

                return Json(new { lista = listaMapeada });
            }
            catch (Exception ex)
            {
                return Json(new {  mensaje = "Ocurrió un error al obtener las compras de hoy en efectivo.", error = ex.Message });
            }
        }
        public JsonResult ListarComprasDiaSeleccionado(string estatus,string dia)
        {
            try
            {
                List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos, Venta ObjVenta)> lista = new CD_Venta().ListarComprasDiaSeleccionado(estatus,dia);

                var listaMapeada = lista.Select(l => new
                {
                    IdVenta = l.IdVenta,
                    HoraMinuto = l.HoraMinuto,
                    TotalCompra = l.TotalCompra,
                    CantidadProductos = l.CantidadProductos,
                    ObjVenta = l.ObjVenta
                }).ToList();

                return Json(new { lista = listaMapeada });
            }
            catch (Exception ex)
            {
                return Json(new {  mensaje = "Ocurrió un error al obtener las compras de hoy en efectivo.", error = ex.Message });
            }
        }


        public JsonResult ListarTotalDineroHoy()
        {
            try
            {
                var capaDatos = new CD_Venta();

                List<(string tipoPago, decimal totalDinero)> lista = capaDatos.ListarTotalDineroHoy();

                var listaMapeada = lista.Select(l => new
                {
                    TipoPago = l.tipoPago,
                    TotalDinero = l.totalDinero
                }).ToList();

                return Json(new {  lista = listaMapeada });
            }
            catch (Exception ex)
            {
                return Json(new { mensaje = "Ocurrió un error al obtener el total de dinero de hoy.", error = ex.Message });
            }
        }
        public JsonResult ListarTotalDineroDiaSeleccionado(string dia)
        {
            try
            {
                var capaDatos = new CD_Venta();

                List<(string tipoPago, decimal totalDinero)> lista = capaDatos.ListarTotalDineroDiaSeleccionado(dia);

                var listaMapeada = lista.Select(l => new
                {
                    TipoPago = l.tipoPago,
                    TotalDinero = l.totalDinero
                }).ToList();

                return Json(new {  lista = listaMapeada });
            }
            catch (Exception ex)
            {
                return Json(new { mensaje = "Ocurrió un error al obtener el total de dinero de hoy.", error = ex.Message });
            }
        }







        public JsonResult ActualizarEstatus(string idVentasProducto)
        {
            bool exito = false;
            try
            {
                exito = new CD_Venta().ActualizarEstatus(idVentasProducto);
            }
            catch (Exception ex)
            {
                string a = ex.Message;
            }

            return Json(new { exito });
        }
        public JsonResult ExisteVenta(string idVentas)
        {
            string error="";
            bool existencia = false;
            try
            {
                existencia = new CD_Venta().ExisteVenta(idVentas, out error);
            }
            catch (Exception ex)
            {
                error += "Errror Interno"+ ex.Message;
            }

            return Json(new { existencia,error=error });
        }
        public JsonResult DevolucionProducto(string garantia, string idVentaProducto, string idVenta, int cantidad)
        {
            string error = "";
            try
            {
                // Obtener la cantidad vendida
                int cantidadVenta = new CD_Venta().ObtenerCantidadVendida(idVentaProducto, out error);
                if (!string.IsNullOrEmpty(error))
                {
                    return Json(new { exito = false, mensaje = error });
                }

                // Obtener la cantidad devuelta
                int cantidadDevuelta = new CD_Venta().ObtenerCantidadDevuelta(idVentaProducto, out error);
                if (!string.IsNullOrEmpty(error))
                {
                    return Json(new { exito = false, mensaje = error });
                }

                // Validar si ya se devolvió la cantidad total o se intenta devolver más de lo vendido
                if (cantidadDevuelta >= cantidadVenta)
                {
                    return Json(new { exito = false, mensaje = "No se puede devolver más de este producto, ya se devolvió la cantidad total." });
                }

                if ((cantidad + cantidadDevuelta) > cantidadVenta)
                {
                    return Json(new { exito = false, mensaje = "No se puede devolver más de lo que se vendió." });
                }

                // Si "garantia" está vacío, establecer a un valor por defecto
                if (string.IsNullOrEmpty(garantia))
                {
                    garantia = "garantia";
                }

                // Actualizar la devolución
                bool CompraDevolucion = new CD_Venta().ActualizarDevolucion(idVentaProducto, out error);
                if (!string.IsNullOrEmpty(error))
                {
                    return Json(new { exito = false, mensaje = error });
                }

                if (!CompraDevolucion)
                {
                    return Json(new { exito = false, mensaje = "No se pudo devolver el producto." });
                }

                // Obtener el ID del usuario desde las cookies
                int UsuarioDevolucion = Convert.ToInt32(Request.Cookies["idUsuario"]?.ToString());

                // Insertar la devolución
                string mensajeDevolucion = "";
                string registroDevolucion = new CD_Venta().InsertarDevolucion(idVenta, idVentaProducto, garantia, UsuarioDevolucion, cantidad, out mensajeDevolucion);

                if (registroDevolucion == null)
                {
                    return Json(new { exito = false, mensaje = mensajeDevolucion });
                }

                // Registrar la compra de devolución si aplica
                if (garantia == "pieza equivocada" || garantia == "no lo necesito")
                {
                    bool resultado = new CD_Venta().RegistrarCompraDevolucion(idVentaProducto, UsuarioDevolucion, cantidad, out mensajeDevolucion);
                    if (!resultado)
                    {
                        return Json(new { exito = false, mensaje = mensajeDevolucion });
                    }
                }

                // Devolución exitosa
                return Json(new { exito = true, mensaje = "Devolución realizada correctamente.", registro = registroDevolucion });
            }
            catch (Exception ex)
            {
                error = "Error interno: " + ex.Message;
                return Json(new { exito = false, mensaje = error });
            }
        }



        public JsonResult ObtenerVentasPorID(string id)
        {
            List<Venta> venta = new List<Venta>();
            string error = "";
            try
            {
                venta = new CD_Venta().ObtenerVentasPorID(id);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return Json(new {venta=venta , error=error});
        }



        /************TABLA************/

        public JsonResult ListarDevoluciones(string strpagina, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<(string NoParte, string Marca, string Motivo, string FechaRegistro, string Cantidad)> oLista = new CD_Venta().ListarDevoluciones(pagina, siguientes);

            // Mapeo de la lista de tuplas a un formato estructurado
            var listaMapeada = oLista.Select(l => new
            {
                NoParte = l.NoParte,
                Marca = l.Marca,
                Motivo = l.Motivo,
                FechaRegistro = l.FechaRegistro,
                Cantidad = l.Cantidad
            }).ToList();

            return Json(new { data = listaMapeada });
        }

        /************TABLA CON WHERE************/
        public JsonResult ListarDevolucionesWhere(string strpagina, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<(string NoParte, string Marca, string Motivo, string FechaRegistro, string Cantidad)> oLista = new CD_Venta().ListarDevolucionesWhere(pagina, siguientes, where);

            // Mapeo de la lista de tuplas a un formato estructurado
            var listaMapeada = oLista.Select(l => new
            {
                NoParte = l.NoParte,
                Marca = l.Marca,
                Motivo = l.Motivo,
                FechaRegistro = l.FechaRegistro,
                Cantidad = l.Cantidad
            }).ToList();

            return Json(new { data = listaMapeada });
        }

        public JsonResult countDevolucionesWhere(string where)
    {
        int registros = 0;
        registros = new CD_Venta().countDevolucionesWhere(where);
        return Json(new { registros = registros });

    }

    public JsonResult countDevoluciones()
    {
        int registros = 0;
        registros = new CD_Venta().countDevoluciones();
        return Json(new { registros = registros });

    }




}







}

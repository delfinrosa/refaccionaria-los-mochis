using CapaDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using CapaEntidad;
using RefaccionariaLosMochis.Permisos;
using Microsoft.AspNetCore.Authorization;
using Org.BouncyCastle.Utilities;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        [PermisosRol("A")]

        public ActionResult Graficos()
        {
            return View();
        }

        public JsonResult ObtenerVentasPorMes(int anio)
        {
            try
            {
                string mensaje = string.Empty;
                var (meses, totalProductosVendidos, totalVentas) = new CD_Reportes().ObtenerVentasPorMes(anio, out mensaje);

                if (meses == null || meses.Count == 0)
                {
                    return Json(new { mensaje = "No se encontraron registros de este año." });
                }

                var data = meses.Select((mes, index) => new
                {
                    Mes = mes,
                    TotalProductosVendidos = totalProductosVendidos[index],
                    TotalVentas = totalVentas[index]
                }).ToList();

                return Json(new { data = data, mensaje = mensaje });

            }
            catch (Exception ex)
            {
                return Json(new { mensaje = "Error al realizar la búsqueda", detalle = ex.Message });
            }
        }
        public JsonResult ObtenerVentasPorDias(int anio,int mes)
        {
            try
            {
                string mensaje = string.Empty;
                var (dias, totalProductosVendidos, totalVentas) = new CD_Reportes().ObtenerVentasPorDias(anio, mes, out mensaje);

                if (dias == null || dias.Count == 0)
                {
                    return Json(new { mensaje = "No se encontraron registros de este año." });
                }

                var data = dias.Select((dia, index) => new
                {
                    Dia = dias[index],
                    TotalProductosVendidos = totalProductosVendidos[index],
                    TotalVentas = totalVentas[index]
                }).ToList();

                return Json(new { data = data, mensaje = mensaje });

            }
            catch (Exception ex)
            {
                return Json(new { mensaje = "Error al realizar la búsqueda", detalle = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ObtenerVentasPorHora(string fecha)
        {
            try
            {
                // Llamada al método que obtiene las ventas por hora
                var (horas, totalProductosVendidos, totalVentas, mensaje) = new CD_Reportes().ObtenerVentasPorHora(fecha);

                if (horas == null || horas.Count == 0)
                {
                    return Json(new { mensaje = "No se encontraron registros para la fecha seleccionada." });
                }

                // Construcción de la respuesta con los datos obtenidos
                var data = horas.Select((hora, index) => new
                {
                    Hora = hora,
                    TotalProductosVendidos = totalProductosVendidos[index],
                    TotalVentas = totalVentas[index]
                }).ToList();

                return Json(new { data = data, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { mensaje = "Error al realizar la búsqueda", detalle = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ObtenerEvolucionPrecios(int idProducto)
        {
            try
            {
                // Llamada al método que obtiene la evolución de precios
                var (fechasCompra, preciosCompra) = new CD_Reportes().ObtenerEvolucionPreciosPorProducto(idProducto, out string mensaje);

                if (fechasCompra == null || fechasCompra.Count == 0)
                {
                    return Json(new { mensaje = "No se encontraron registros para el producto seleccionado." });
                }

                // Construcción de la respuesta con los datos obtenidos
                var data = fechasCompra.Select((fecha, index) => new
                {
                    FechaCompra = fecha.ToString("yyyy-MM-dd"),
                    PrecioCompra = preciosCompra[index]
                }).ToList();

                return Json(new { data = data, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { mensaje = "Error al realizar la búsqueda", detalle = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ObtenerTopProductos(int caso, string anio, string mes, string fecha)
        {
            try
            {
                List<string> noParte = new List<string>();
                List<int> totalProductosVendidos = new List<int>();
                string mensaje = string.Empty;

                switch (caso)
                {
                    case 1:
                        // Obtener datos históricos
                        (noParte, totalProductosVendidos) = new CD_Reportes().ObtenerTopProductosHistoricos(out mensaje);
                        break;

                    case 2:
                        // Obtener datos por día
                        (noParte, totalProductosVendidos) = new CD_Reportes().ObtenerTopProductosPorDia(fecha, out mensaje);
                        break;

                    case 3:
                        // Obtener datos por mes
                        (noParte, totalProductosVendidos) = new CD_Reportes().ObtenerTopProductosPorMes(anio, mes, out mensaje);
                        break;

                    case 4:
                        // Obtener datos por año
                        (noParte, totalProductosVendidos) = new CD_Reportes().ObtenerTopProductosPorAnio(anio, out mensaje);
                        break;

                    default:
                        return Json(new { mensaje = "Caso no reconocido. Verifica la información enviada." });
                }

                if (noParte == null || noParte.Count == 0)
                {
                    return Json(new { mensaje = "No se encontraron registros para los criterios seleccionados." });
                }

                // Construcción de la respuesta con los datos obtenidos
                var data = noParte.Select((parte, index) => new
                {
                    NoParte = parte,
                    TotalProductosVendidos = totalProductosVendidos[index],
                }).ToList();

                return Json(new { data = data, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { mensaje = "Error al realizar la búsqueda", detalle = ex.Message });
            }
        }


    }
}

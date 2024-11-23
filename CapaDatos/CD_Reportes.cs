using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Reportes
    {


        public (List<string> NoPartes, List<string> Descripciones, List<string> Marcas, List<decimal> TotalesVendidos, List<int> CantidadesDisponibles, string Mensaje) ObtenerProductosMasVendidos()
        {
            List<string> noPartes = new List<string>();
            List<string> descripciones = new List<string>();
            List<string> marcas = new List<string>();
            List<decimal> totalesVendidos = new List<decimal>();
            List<int> cantidadesDisponibles = new List<int>();
            string mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT TOP 10
                    p.NoParte,
                    p.Descripcion AS DescripcionProducto,
                    m.Descripcion AS Marca,
                    SUM(vp.Cantidad) AS TotalVendido,
                    sp.CantidadDisponible
                FROM
                    tVentasProductos vp
                JOIN
                    tProductos p ON vp.IdProducto = p.IdProducto
                JOIN
                    StockProductos sp ON sp.IdProducto = p.IdProducto
                JOIN
                    tMarcas m ON p.IdMarca = m.IdMarca
                GROUP BY
                    p.NoParte,
                    p.Descripcion,
                    m.Descripcion,
                    sp.CantidadDisponible
                ORDER BY
                    TotalVendido DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                noPartes.Add(reader["NoParte"].ToString());
                                descripciones.Add(reader["DescripcionProducto"].ToString());
                                marcas.Add(reader["Marca"].ToString());
                                totalesVendidos.Add(Convert.ToDecimal(reader["TotalVendido"]));
                                cantidadesDisponibles.Add(Convert.ToInt32(reader["CantidadDisponible"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (noPartes, descripciones, marcas, totalesVendidos, cantidadesDisponibles, mensaje);
        }

        public (List<int> Meses, List<int> TotalProductosVendidos, List<decimal> TotalVentas) ObtenerVentasPorMes(int anio, out string mensaje)
        {
            List<int> meses = new List<int>();
            List<int> totalProductosVendidos = new List<int>();
            List<decimal> totalVentas = new List<decimal>();
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT 
                    DATEPART(MONTH, v.Fecha) AS Mes,
                    SUM(vp.Cantidad) AS TotalProductosVendidos,
                    SUM(vp.Cantidad * vp.Precio) AS TotalVentas
                FROM 
                    tVentas v
                JOIN 
                    tVentasProductos vp ON v.IdVenta = vp.IdVenta
                WHERE 
                    DATEPART(YEAR, v.Fecha) = @Anio
                GROUP BY 
                    DATEPART(MONTH, v.Fecha)
                ORDER BY 
                    Mes;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Anio", anio);
                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                meses.Add(Convert.ToInt32(reader["Mes"]));
                                totalProductosVendidos.Add(Convert.ToInt32(reader["TotalProductosVendidos"]));
                                totalVentas.Add(Convert.ToDecimal(reader["TotalVentas"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (meses, totalProductosVendidos, totalVentas);
        }



        public (List<int> Dias, List<decimal> TotalProductosVendidos, List<decimal> TotalVentas) ObtenerVentasPorDias(int anio, int mes, out string mensaje)
        {
            List<int> dias = new List<int>();
            List<decimal> totalProductosVendidos = new List<decimal>();
            List<decimal> totalVentas = new List<decimal>();
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT 
                    DATEPART(DAY, v.Fecha) AS Dia,
                    SUM(vp.Cantidad) AS TotalProductosVendidos,
                    SUM(vp.Cantidad * vp.Precio) AS TotalVentas
                FROM 
                    tVentas v
                JOIN 
                    tVentasProductos vp ON v.IdVenta = vp.IdVenta
                WHERE 
                    DATEPART(YEAR, v.Fecha) = @Anio
                    AND DATEPART(MONTH, v.Fecha) = @Mes
                GROUP BY 
                    DATEPART(DAY, v.Fecha)
                ORDER BY 
                    Dia;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Anio", anio);
                        cmd.Parameters.AddWithValue("@Mes", mes);

                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dias.Add(Convert.ToInt32(reader["Dia"]));
                                totalProductosVendidos.Add(Convert.ToDecimal(reader["TotalProductosVendidos"]));
                                totalVentas.Add(Convert.ToDecimal(reader["TotalVentas"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (dias, totalProductosVendidos, totalVentas);
        }


        public (List<int> Horas, List<decimal> TotalProductosVendidos, List<decimal> TotalVentas, string Mensaje) ObtenerVentasPorHora(string fecha)
        {
            List<int> horas = new List<int>();
            List<decimal> totalProductosVendidos = new List<decimal>();
            List<decimal> totalVentas = new List<decimal>();
            string mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    // Consulta SQL
                    string query = @"
                SELECT 
                    DATEPART(HOUR, v.Fecha) AS Hora,
                    SUM(vp.Cantidad) AS TotalProductosVendidos,
                    SUM(vp.Cantidad * vp.Precio) AS TotalVentas
                FROM 
                    tVentas v
                JOIN 
                    tVentasProductos vp ON v.IdVenta = vp.IdVenta
                WHERE 
                    CAST(v.Fecha AS DATE) = @Fecha
                GROUP BY 
                    DATEPART(HOUR, v.Fecha)
                ORDER BY 
                    Hora;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Fecha", fecha);

                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                horas.Add(Convert.ToInt32(reader["Hora"]));
                                totalProductosVendidos.Add(Convert.ToDecimal(reader["TotalProductosVendidos"]));
                                totalVentas.Add(Convert.ToDecimal(reader["TotalVentas"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (horas, totalProductosVendidos, totalVentas, mensaje);
        }

        public (List<DateTime> FechasCompra, List<decimal> PreciosCompra) ObtenerEvolucionPreciosPorProducto(int idProducto, out string mensaje)
        {
            List<DateTime> fechasCompra = new List<DateTime>();
            List<decimal> preciosCompra = new List<decimal>();
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT 
                c.Fecha AS FechaCompra,
                cd.Precio AS PrecioCompra
            FROM 
                tCompras c
            JOIN 
                tComprasDtl cd ON c.CompraId = cd.CompraId
            JOIN 
                tProductosProveedores pp ON cd.ProductoProveedorId = pp.IdProductoProveedor
            JOIN 
                tProductos p ON pp.IdProducto = p.IdProducto
            WHERE 
                p.IdProducto = @IdProducto
            ORDER BY 
                c.Fecha ASC;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                fechasCompra.Add(Convert.ToDateTime(reader["FechaCompra"]));
                                preciosCompra.Add(Convert.ToDecimal(reader["PrecioCompra"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (fechasCompra, preciosCompra);
        }

        public (List<string> NoParte, List<int> TotalProductosVendidos) ObtenerTopProductosHistoricos(out string mensaje)
        {
            List<string> noParte = new List<string>();
            List<int> totalProductosVendidos = new List<int>();
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT TOP 10 
                p.NoParte,
                SUM(vp.Cantidad) AS TotalProductosVendidos
            FROM 
                tVentas v
            JOIN 
                tVentasProductos vp ON v.IdVenta = vp.IdVenta
            JOIN 
                tProductos p ON p.IdProducto = vp.IdProducto
            GROUP BY 
                p.NoParte
            ORDER BY 
                TotalProductosVendidos DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                noParte.Add(reader["NoParte"].ToString());
                                totalProductosVendidos.Add(Convert.ToInt32(reader["TotalProductosVendidos"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (noParte, totalProductosVendidos);
        }
        public (List<string> NoParte, List<int> TotalProductosVendidos) ObtenerTopProductosPorAnio(string anio, out string mensaje)
        {
            List<string> noParte = new List<string>();
            List<int> totalProductosVendidos = new List<int>();
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT TOP 10 
                p.NoParte,
                SUM(vp.Cantidad) AS TotalProductosVendidos
            FROM 
                tVentas v
            JOIN 
                tVentasProductos vp ON v.IdVenta = vp.IdVenta
            JOIN 
                tProductos p ON p.IdProducto = vp.IdProducto
            WHERE 
                DATEPART(YEAR, v.Fecha) = @Anio
            GROUP BY 
                p.NoParte
            ORDER BY 
                TotalProductosVendidos DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Anio", anio);

                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                noParte.Add(reader["NoParte"].ToString());
                                totalProductosVendidos.Add(Convert.ToInt32(reader["TotalProductosVendidos"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (noParte, totalProductosVendidos);
        }
        public (List<string> NoParte, List<int> TotalProductosVendidos) ObtenerTopProductosPorMes(string anio, string mes, out string mensaje)
        {
            List<string> noParte = new List<string>();
            List<int> totalProductosVendidos = new List<int>();
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT TOP 10 
                p.NoParte,
                SUM(vp.Cantidad) AS TotalProductosVendidos
            FROM 
                tVentas v
            JOIN 
                tVentasProductos vp ON v.IdVenta = vp.IdVenta
            JOIN 
                tProductos p ON p.IdProducto = vp.IdProducto
            WHERE 
                DATEPART(YEAR, v.Fecha) = @Anio
                AND DATEPART(MONTH, v.Fecha) = @Mes
            GROUP BY 
                p.NoParte
            ORDER BY 
                TotalProductosVendidos DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Anio", anio);
                        cmd.Parameters.AddWithValue("@Mes", mes);

                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                noParte.Add(reader["NoParte"].ToString());
                                totalProductosVendidos.Add(Convert.ToInt32(reader["TotalProductosVendidos"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (noParte, totalProductosVendidos);
        }
        public (List<string> NoParte, List<int> TotalProductosVendidos) ObtenerTopProductosPorDia(string fecha, out string mensaje)
        {
            List<string> noParte = new List<string>();
            List<int> totalProductosVendidos = new List<int>();
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT TOP 10 
                p.NoParte,
                SUM(vp.Cantidad) AS TotalProductosVendidos
            FROM 
                tVentas v
            JOIN 
                tVentasProductos vp ON v.IdVenta = vp.IdVenta
            JOIN 
                tProductos p ON p.IdProducto = vp.IdProducto
            WHERE 
                CONVERT(DATE, v.Fecha) = CONVERT(DATE, @Fecha)
            GROUP BY 
                p.NoParte
            ORDER BY 
                TotalProductosVendidos DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Fecha", fecha);

                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                noParte.Add(reader["NoParte"].ToString());
                                totalProductosVendidos.Add(Convert.ToInt32(reader["TotalProductosVendidos"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return (noParte, totalProductosVendidos);
        }


    }
}

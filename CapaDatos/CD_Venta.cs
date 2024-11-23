using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CapaDatos
{
    public class CD_Venta
    {


        public List<Producto> TodosDescripcionProductos()
        {
            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT p.Descripcion " +
                                      "FROM tProductos p " +
                                      "GROUP BY p.Descripcion";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                Descripcion = Convert.ToString(dr["Descripcion"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente (lanzar o registrar)
                throw;
            }
            return lista;
        }




        public (List<Producto> productosFiltrados, List<Producto> otrosProductos) BuscadorVentaInterno(string descripcion, string anioCompleto, string anioAbreviado, string marcaCompleto, string marcaAbreviado, string modelo)
        {
            List<Producto> lista = new List<Producto>();
            List<string> listaAños = new List<string>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT p.NoParte, plc.Valor
                FROM tProductos p
                JOIN tProductosLineasCaracteristicas plc ON p.IdProducto = plc.IdProducto
                JOIN tMarcas m ON p.IdMarca = m.IdMarca
                WHERE UPPER(p.Descripcion) = UPPER(@Descripcion)
                AND (
                    UPPER(plc.Valor) LIKE '%' + UPPER(@MarcaCompleto) + '%'
                    OR UPPER(plc.Valor) LIKE '%' + UPPER(@MarcaAbreviado) + '%'
                );";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@MarcaCompleto", marcaCompleto);
                    cmd.Parameters.AddWithValue("@MarcaAbreviado", marcaAbreviado);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                NoParte = Convert.ToString(dr["NoParte"]),
                                Valor = Convert.ToString(dr["Valor"])
                            });
                        }
                    }
                }

                // Filtrar resultados en C# por modelo
                List<Producto> productosFiltrados = lista.Where(p => p.Valor.Contains(modelo.ToUpper())).ToList();


                List<Producto> otrosProductos = lista.Except(productosFiltrados).ToList();

                // Extraer años de los valores filtrados y sustituir años incompletos
                foreach (var producto in productosFiltrados)
                {
                    producto.Valor = SustituirAnios(producto.Valor);
                }
                productosFiltrados = productosFiltrados.Where(p => p.Valor.Contains(anioCompleto)).ToList();

                return (productosFiltrados, otrosProductos);
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente (lanzar o registrar)
                throw;
            }
        }

        public string SustituirAnios(string valor)
        {
            List<string> partes = new List<string>(valor.Split(new char[] { ',', '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries));
            List<string> resultado = new List<string>();

            for (int i = 0; i < partes.Count; i++)
            {
                string parte = partes[i].Trim();

                if (parte.Length >= 2 && (parte.Contains("-") || (i + 1 < partes.Count && partes[i + 1] == "-")))
                {
                    string[] años;
                    if (parte.Contains("-"))
                    {
                        años = parte.Split('-');
                    }
                    else
                    {
                        años = new string[] { parte, partes[i + 2] };
                        i += 2; // Saltar el siguiente elemento
                    }

                    if (años.Length == 2 && años.All(a => a.Length == 2 && a.All(char.IsDigit)))
                    {
                        int añoInicio = int.Parse(años[0]);
                        int añoFin = int.Parse(años[1]);

                        if (añoInicio < 50) añoInicio += 2000;
                        else añoInicio += 1900;

                        if (añoFin < 50) añoFin += 2000;
                        else añoFin += 1900;

                        for (int año = añoInicio; año <= añoFin; año++)
                        {
                            if (año >= 1965 && año <= 2022)
                            {
                                resultado.Add(año.ToString());
                            }
                        }
                        resultado.Add("|");
                    }
                }
                else if (parte.Length == 2 && parte.All(char.IsDigit))
                {
                    int añoAbreviado = int.Parse(parte);
                    if (añoAbreviado < 50) añoAbreviado += 2000;
                    else añoAbreviado += 1900;

                    if (añoAbreviado >= 1965 && añoAbreviado <= 2022)
                    {
                        resultado.Add(añoAbreviado.ToString());
                    }
                }
                else
                {
                    resultado.Add(parte);
                }
            }

            return string.Join(" ", resultado);
        }



        public List<Producto> buscadorVenta(string busqueda)
        {
            List<Producto> lista = new List<Producto>();

            // Divide los términos de búsqueda por espacios
            var terminosBusqueda = busqueda.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    // Construye la consulta SQL
                    string query = @"
                                    SELECT 
                                        P.NoParte, 
                                        M.Descripcion AS DescripcionMarca, 
                                        P.Descripcion, 
                                        P.Precio, 
                                        P.IdProducto,
                                        SUM(SP.CantidadDisponible) AS CantidadDisponible
                                    FROM 
                                        tProductos P
                                    INNER JOIN 
                                        tMarcas M ON P.IdMarca = M.IdMarca
                                    INNER JOIN 
                                        StockProductos SP ON P.IdProducto = SP.IdProducto
                                    WHERE 
                                        (P.Descripcion LIKE '%" + string.Join("%' AND P.Descripcion LIKE '%", terminosBusqueda) + @"%')
                                        OR (P.NoParte LIKE '%" + string.Join("%' AND P.NoParte LIKE '%", terminosBusqueda) + @"%')
                                        OR (EXISTS (
                                            SELECT 1
                                            FROM tProductosLineasCaracteristicas PLC
                                            WHERE PLC.IdProducto = P.IdProducto
                                            AND PLC.Valor LIKE '%" + string.Join("%' AND PLC.Valor LIKE '%", terminosBusqueda) + @"%'
                                        ))
                                    GROUP BY 
                                        P.NoParte, M.Descripcion, P.Descripcion, P.Precio, P.IdProducto
                                    ORDER BY 
                                        P.Descripcion DESC";


                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                NoParte = Convert.ToString(dr["NoParte"]),
                                oMarca = new Marca
                                {
                                    Descripcion = Convert.ToString(dr["DescripcionMarca"])
                                },
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                Maximo = Convert.ToInt32(dr["CantidadDisponible"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                lista = new List<Producto>();
            }
            return lista;
        }





        public Producto detalleProducto(int idproducto)
        {
            Producto producto = null;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    // Construye la consulta SQL
                    string query = @"
                            SELECT Valor
                            FROM tProductosLineasCaracteristicas
                            WHERE IdProducto = @IdProducto;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdProducto", idproducto);
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            producto = new Producto
                            {
                                Valor = Convert.ToString(dr["Valor"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                producto = null;
            }

            return producto;
        }



        public List<Producto> busquedaVentaNoParte(string strNoParte)
        {
            List<Producto> lista = new List<Producto>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    // Construye la consulta SQL
                    string query = @"
                SELECT 
                    p.IdProducto,
                    p.IdMarca,
                    p.Precio,
                    m.Descripcion AS MarcaDescripcion,
                    p.noparte
                FROM 
                    tProductos p
                JOIN 
                    tMarcas m ON p.IdMarca = m.IdMarca
                WHERE 
                    p.NoParte LIKE @strNoParte";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@strNoParte", "%" + strNoParte + "%"); // Usa parámetro para evitar SQL injection
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                NoParte = Convert.ToString(dr["noparte"]),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                oMarca = new Marca
                                {
                                    IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                    Descripcion = Convert.ToString(dr["MarcaDescripcion"])
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                lista = new List<Producto>();
                // Puedes registrar o mostrar el mensaje de error según sea necesario
            }
            return lista;
        }

        /*NUEVO*/
        public Guid RegistrarVenta(Venta objVenta, string nombre, int factura, out string mensaje)
        {
            mensaje = string.Empty;
            Guid idVentaInsertada = Guid.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                INSERT INTO tVentas 
                (IdVenta, Fecha, IdVendedor, Estatus, RequiereFactura,NombreCliente)
                VALUES 
                (NEWID(), GETDATE(), @IdVendedor, 'A', @factura ,@nombre);";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdVendedor", objVenta.oVendedor?.IdUsuario ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@factura", factura);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            query = "SELECT TOP 1 IdVenta FROM tVentas ORDER BY Fecha DESC";
                            cmd.CommandText = query;
                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                idVentaInsertada = (Guid)result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return idVentaInsertada;
        }




        public bool RegistrarVentasProducto(VentasProductos objVentasProducto, string idVenta, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                INSERT INTO tVentasProductos 
                (IdVentasProducto, IdVenta, IdProducto, Cantidad, Precio,Estatus)
                VALUES 
                (NEWID(), @IdVenta, @IdProducto, @Cantidad, @Precio,'A' );";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                        cmd.Parameters.AddWithValue("@IdProducto", Convert.ToInt64(objVentasProducto.oProducto.IdProducto));
                        cmd.Parameters.AddWithValue("@Cantidad", objVentasProducto.Cantidad);
                        cmd.Parameters.AddWithValue("@Precio", objVentasProducto.Precio);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se pudo insertar el detalle de venta.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return resultado;
        }


        public List<(string NombreCliente, string NombreVendedor, int CantidadProductos, decimal TotalPagar, string idVenta)> ObtenerVentasAgrupadas()
        {
            List<(string, string, int, decimal, string)> lista = new List<(string, string, int, decimal, string)>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    // Construye la consulta SQL
                    string query = @"
                SELECT 
                    v.IdVenta,
                    v.NombreCliente,
                    uVendedor.Nombre AS NombreVendedor,
                    SUM(vp.Cantidad) AS CantidadProductos,
                    SUM(vp.Cantidad * vp.Precio) AS TotalPagar
                FROM 
                    tVentas v
                JOIN 
                    tUsuarios uVendedor ON v.IdVendedor = uVendedor.IdUsuario
                JOIN 
                    tVentasProductos vp ON v.IdVenta = vp.IdVenta
                WHERE 
                    v.Estatus = 'A'
                GROUP BY 
                    v.IdVenta, v.NombreCliente, uVendedor.Nombre";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add((
                                dr["NombreCliente"].ToString(),
                                dr["NombreVendedor"].ToString(),
                                Convert.ToInt32(dr["CantidadProductos"]),
                                Convert.ToDecimal(dr["TotalPagar"]),
                                dr["IdVenta"].ToString()
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<(string, string, int, decimal, string)>();
            }
            return lista;
        }





        public List<(int IdProducto, string CodigoBarras, string NoParte, string MarcaDescripcion, int Cantidad, decimal Precio, string IdVentasProducto, string Estatus, bool RequiereFactura)> ObtenerCarritoPorVenta(string idVenta)
        {
            List<(int, string, string, string, int, decimal, string, string, bool)> lista = new List<(int, string, string, string, int, decimal, string, string, bool)>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    // Construye la consulta SQL
                    string query = @"
                SELECT
                    vp.IdProducto,
                    p.CodigoBarras,
                    p.NoParte,
                    m.Descripcion AS MarcaDescripcion,
                    vp.Cantidad,
                    vp.Precio,
                    vp.IdVentasProducto,
                    vp.Estatus,
                    v.RequiereFactura
                FROM
                    tVentasProductos vp
                JOIN
                    tProductos p ON vp.IdProducto = p.IdProducto
                JOIN
                    tMarcaS m ON p.IdMarca = m.IdMarca
                JOIN
                    tVentas v ON vp.IdVenta = v.IdVenta
                WHERE
                    vp.IdVenta = @IdVenta";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add((
                                Convert.ToInt32(dr["IdProducto"]),
                                dr["CodigoBarras"].ToString(),
                                dr["NoParte"].ToString(),
                                dr["MarcaDescripcion"].ToString(),
                                Convert.ToInt32(dr["Cantidad"]),
                                Convert.ToDecimal(dr["Precio"]),
                                dr["IdVentasProducto"].ToString(),
                                dr["Estatus"].ToString(),
                                Convert.ToBoolean(dr["RequiereFactura"])
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                lista = new List<(int, string, string, string, int, decimal, string, string, bool)>();
                // Puedes registrar o mostrar el mensaje de error según sea necesario
            }

            return lista;
        }


        public bool EliminarProducto(string idVentasProducto)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                DELETE FROM tVentasProductos
                WHERE IdVentasProducto = @IdVentasProducto";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@IdVentasProducto", idVentasProducto);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    return filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                // Puedes registrar el error o lanzar una excepción personalizada
                return false;
            }
        }
        public bool EliminarVentaSiNoHayProductos(string idVenta)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    // Verificar si hay al menos un producto asociado a la venta
                    string verificarQuery = @"
                SELECT COUNT(*)
                FROM tVentasProductos
                WHERE IdVenta = @IdVenta";

                    SqlCommand verificarCmd = new SqlCommand(verificarQuery, conexion);
                    verificarCmd.Parameters.AddWithValue("@IdVenta", idVenta);
                    int productosAsociados = (int)verificarCmd.ExecuteScalar();

                    // Si no hay productos asociados, eliminar la venta
                    if (productosAsociados == 0)
                    {
                        string eliminarQuery = @"
                    DELETE FROM tVentas
                    WHERE IdVenta = @IdVenta";

                        SqlCommand eliminarCmd = new SqlCommand(eliminarQuery, conexion);
                        eliminarCmd.Parameters.AddWithValue("@IdVenta", idVenta);
                        int filasAfectadas = eliminarCmd.ExecuteNonQuery();

                        return filasAfectadas > 0; // Retorna true si la venta fue eliminada
                    }

                    return false; // No se eliminó la venta porque hay productos asociados
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return false;
            }
        }





        public List<Ticket> informacionTicket(string idVenta)
        {
            string connectionString = Conexion.cn; // Usa la cadena de conexión de la clase Conexion

            string query = @"
        SELECT
            p.Descripcion,
            p.NoParte,
            m.Descripcion AS MarcaDescripcion,
            vp.Cantidad,
            vp.Precio,
            u.Nombre AS Vendedor,
            v.NombreCliente,
            v.Fecha
        FROM
            tVentasProductos vp
        JOIN
            tProductos p ON vp.IdProducto = p.IdProducto
        JOIN
            tMarcaS m ON p.IdMarca = m.IdMarca
        JOIN
            tVentas v ON v.IdVenta = vp.IdVenta
        JOIN
            tUsuarios u ON u.IdUsuario = v.IdVendedor
        WHERE
            vp.IdVenta = @IdVenta";

            var result = new List<Ticket>();

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Ticket
                            {
                                Descripcion = dr["Descripcion"].ToString(),
                                NoParte = dr["NoParte"].ToString(),
                                MarcaDescripcion = dr["MarcaDescripcion"].ToString(),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                Vendedor = dr["Vendedor"].ToString(),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                Fecha = Convert.ToDateTime(dr["Fecha"])
                            });
                        }
                    }
                }
            }

            return result;
        }



        public bool ActualizarVenta(Venta venta)
        {
            bool respuesta = false;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                    UPDATE tVentas
                    SET 
                        IdCajero = CASE WHEN @IdCajero IS NOT NULL THEN @IdCajero ELSE IdCajero END,
                        TipoPago = CASE WHEN @TipoPago IS NOT NULL THEN @TipoPago ELSE TipoPago END,
                        Cambio = CASE WHEN @Cambio IS NOT NULL THEN @Cambio ELSE Cambio END,
                        Estatus = 'V'
                    WHERE IdVenta = @IdVenta;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdCajero", (object)venta.oCajero?.IdUsuario ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoPago", (object)venta.TipoPago ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cambio", (object)venta.Cambio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdVenta", venta.IdVenta);

                    oconexion.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    respuesta = filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
            }

            return respuesta;
        }

        //public List<(string TipoPago, decimal TotalDinero)> ListarTotalDineroHoy()
        //{
        //    List<(string TipoPago, decimal TotalDinero)> lista = new List<(string TipoPago, decimal TotalDinero)>();

        //    try
        //    {
        //        using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
        //        {
        //            string query = @"
        //        SELECT 
        //            TipoPago,
        //            SUM(Cantidad * Precio) AS TotalDinero
        //        FROM 
        //            tVentasProductos
        //        JOIN 
        //            tVentas ON tVentasProductos.IdVenta = tVentas.IdVenta
        //        WHERE 
        //            CONVERT(DATE, tVentas.Fecha) = CONVERT(DATE, GETDATE())
        //        GROUP BY 
        //            TipoPago;";

        //            SqlCommand cmd = new SqlCommand(query, oconexion);

        //            oconexion.Open();

        //            using (SqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    lista.Add((
        //                        dr["TipoPago"].ToString(),
        //                        dr.GetDecimal(dr.GetOrdinal("TotalDinero"))
        //                    ));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejo básico de excepciones
        //        Console.WriteLine($"Error al listar total de dinero: {ex.Message}");
        //        lista = new List<(string TipoPago, decimal TotalDinero)>(); // Devolver lista vacía en caso de error
        //    }

        //    return lista;
        //}


        public List<(string TipoPago, decimal TotalDinero)> ListarTotalDineroHoy()
        {
            List<(string TipoPago, decimal TotalDinero)> lista = new List<(string TipoPago, decimal TotalDinero)>
            {
                ("TARJETA", 0),
                ("EFECTIVO", 0),
                ("TRANSFERENCIA", 0)
            };

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT 
                    V.IdVenta,
                    V.TipoPago,
                    SUM(VP.Cantidad * VP.Precio) AS TotalDinero
                FROM 
                    tVentas V
                JOIN 
                    tVentasProductos VP ON VP.IdVenta = V.IdVenta
                WHERE 
                    CONVERT(DATE, V.Fecha) = CONVERT(DATE, GETDATE())
                GROUP BY 
                    V.IdVenta, V.TipoPago;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string tipoPago = dr["TipoPago"].ToString();
                            decimal totalDinero = dr.GetDecimal(dr.GetOrdinal("TotalDinero"));
                            Guid idVenta = (Guid)dr["IdVenta"]; // Obtiene el IdVenta para el caso "MIXTO"

                            switch (tipoPago)
                            {
                                case "TARJETA":
                                    lista[0] = (tipoPago, lista[0].TotalDinero + totalDinero);
                                    break;

                                case "EFECTIVO":
                                    lista[1] = (tipoPago, lista[1].TotalDinero + totalDinero);
                                    break;

                                case "TRANSFERENCIA":
                                    lista[2] = (tipoPago, lista[2].TotalDinero + totalDinero);
                                    break;

                                case "MIXTO":
                                    // Abre una segunda conexión para obtener el valor de Cambio
                                    using (SqlConnection oconexionMixto = new SqlConnection(Conexion.cn))
                                    {
                                        oconexionMixto.Open();
                                        string queryMixto = "SELECT Cambio FROM tVentas WHERE IdVenta = @IdVenta";
                                        using (SqlCommand cmdMixto = new SqlCommand(queryMixto, oconexionMixto))
                                        {
                                            cmdMixto.Parameters.AddWithValue("@IdVenta", idVenta);

                                            // Ejecuta la consulta y verifica si el resultado es null
                                            var result = cmdMixto.ExecuteScalar();
                                            decimal cambio = result != null ? (decimal)result : 0; // Si es null, asigna 0 a `cambio`

                                            // Calcula el monto en tarjeta
                                            decimal totalTarjeta = totalDinero - cambio;

                                            // Actualiza los totales en la lista
                                            lista[0] = ("TARJETA", lista[0].TotalDinero + totalTarjeta);
                                            lista[1] = ("EFECTIVO", lista[1].TotalDinero + cambio);
                                        }
                                    }
                                    break;


                                default:
                                    Console.WriteLine($"Tipo de pago desconocido: {tipoPago}");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo básico de excepciones
                Console.WriteLine($"Error al listar total de dinero: {ex.Message}");
                lista = new List<(string TipoPago, decimal TotalDinero)>(); // Devolver lista vacía en caso de error
            }

            return lista;
        }

        public List<(string TipoPago, decimal TotalDinero)> ListarTotalDineroDiaSeleccionado(string dia)
        {
            List<(string TipoPago, decimal TotalDinero)> lista = new List<(string TipoPago, decimal TotalDinero)>
            {
                ("TARJETA", 0),
                ("EFECTIVO", 0),
                ("TRANSFERENCIA", 0)
            };

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                                    SELECT 
                                        V.IdVenta,
                                        V.TipoPago,
                                        SUM(VP.Cantidad * VP.Precio) AS TotalDinero
                                    FROM 
                                        tVentas V
                                    JOIN 
                                        tVentasProductos VP ON VP.IdVenta = V.IdVenta
                                    WHERE 
                                        CONVERT(DATE, V.Fecha) = CONVERT(DATE, @dia)
                                    GROUP BY 
                                        V.IdVenta, V.TipoPago;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@dia", dia); // Añadido parámetro para evitar SQL Injection
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string tipoPago = dr["TipoPago"].ToString();
                            decimal totalDinero = dr.GetDecimal(dr.GetOrdinal("TotalDinero"));
                            Guid idVenta = (Guid)dr["IdVenta"];

                            switch (tipoPago)
                            {
                                case "TARJETA":
                                    lista[0] = (tipoPago, lista[0].TotalDinero + totalDinero);
                                    break;

                                case "EFECTIVO":
                                    lista[1] = (tipoPago, lista[1].TotalDinero + totalDinero);
                                    break;

                                case "TRANSFERENCIA":
                                    lista[2] = (tipoPago, lista[2].TotalDinero + totalDinero);
                                    break;

                                case "MIXTO":
                                    // Abre una segunda conexión para obtener el valor de Cambio
                                    using (SqlConnection oconexionMixto = new SqlConnection(Conexion.cn))
                                    {
                                        oconexionMixto.Open();
                                        string queryMixto = "SELECT Cambio FROM tVentas WHERE IdVenta = @IdVenta";
                                        using (SqlCommand cmdMixto = new SqlCommand(queryMixto, oconexionMixto))
                                        {
                                            cmdMixto.Parameters.AddWithValue("@IdVenta", idVenta);

                                            // Ejecuta la consulta y verifica si el resultado es null
                                            var result = cmdMixto.ExecuteScalar();
                                            decimal cambio = result != null ? (decimal)result : 0; // Si es null, asigna 0 a `cambio`

                                            // Calcula el monto en tarjeta
                                            decimal totalTarjeta = totalDinero - cambio;

                                            // Actualiza los totales en la lista
                                            lista[0] = ("TARJETA", lista[0].TotalDinero + totalTarjeta);
                                            lista[1] = ("EFECTIVO", lista[1].TotalDinero + cambio);
                                        }
                                    }
                                    break;


                                default:
                                    Console.WriteLine($"Tipo de pago desconocido: {tipoPago}");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo básico de excepciones
                Console.WriteLine($"Error al listar total de dinero: {ex.Message}");
                lista = new List<(string TipoPago, decimal TotalDinero)>(); // Devolver lista vacía en caso de error
            }

            return lista;
        }






        public List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos, Venta ObjVenta)> ListarComprasDiaSeleccionado(string estatus, string dia)
        {
            List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos,Venta ObjVenta)> lista = new List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos, Venta ObjVenta)>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
    SELECT 
        tVentas.IdVenta,
        CONVERT(VARCHAR(5), tVentas.Fecha, 108) AS HoraMinuto,
        SUM(tVentasProductos.Cantidad * tVentasProductos.Precio) AS TotalCompra,
        SUM(tVentasProductos.Cantidad) AS CantidadProductos,
        tVentas.TipoPago
    FROM 
        tVentasProductos
    JOIN 
        tVentas ON tVentasProductos.IdVenta = tVentas.IdVenta
    WHERE 
        CONVERT(DATE, tVentas.Fecha) = CONVERT(DATE, @dia) 
        AND (tVentas.TipoPago = @Estatus OR tVentas.TipoPago = 'MIXTO')
    GROUP BY 
            tVentas.IdVenta, tVentas.Fecha, tVentas.TipoPago;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@Estatus", estatus);
                    cmd.Parameters.AddWithValue("@dia", dia);

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (dr["TipoPago"].ToString() == "MIXTO")
                            {
                                Venta objVenta = new Venta();
                                using (SqlConnection oconexionMixto = new SqlConnection(Conexion.cn))
                                {
                                    oconexionMixto.Open();
                                    string queryMixto = "SELECT Cambio, TipoPago FROM tVentas WHERE IdVenta = @IdVenta";
                                    using (SqlCommand cmdMixto = new SqlCommand(queryMixto, oconexionMixto))
                                    {
                                        cmdMixto.Parameters.AddWithValue("@IdVenta", dr["IdVenta"].ToString());

                                        using (SqlDataReader drMixto = cmdMixto.ExecuteReader())
                                        {
                                            if (drMixto.Read())
                                            {
                                                objVenta.Cambio = drMixto["Cambio"] != DBNull.Value ? Convert.ToDecimal(drMixto["Cambio"]) : 0;
                                                objVenta.TipoPago = drMixto["TipoPago"] != DBNull.Value ? Convert.ToString(drMixto["TipoPago"]) : "";
                                            }
                                        }
                                    }
                                    lista.Add((
                                        dr["IdVenta"].ToString(),
                                        dr["HoraMinuto"].ToString(),
                                        dr.GetDecimal(dr.GetOrdinal("TotalCompra")),
                                        dr.GetInt32(dr.GetOrdinal("CantidadProductos")),
                                        objVenta
                                    ));
                                }
                            }
                            else
                            {
                                lista.Add((
                                    dr["IdVenta"].ToString(),
                                    dr["HoraMinuto"].ToString(),
                                    dr.GetDecimal(dr.GetOrdinal("TotalCompra")),
                                    dr.GetInt32(dr.GetOrdinal("CantidadProductos")),
                                    new Venta()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo básico de excepciones
                Console.WriteLine($"Error al listar compras: {ex.Message}");
                lista = new List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos, Venta ObjVenta)>(); // Devolver lista vacía en caso de error
            }

            return lista;
        }

        public List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos)> ListarComprasHoyEfectivo(string estatus)
        {
            List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos)> lista = new List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos)>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT 
                    tVentas.IdVenta,
                    CONVERT(VARCHAR(5), tVentas.Fecha, 108) AS HoraMinuto,
                    SUM(tVentasProductos.Cantidad * tVentasProductos.Precio) AS TotalCompra,
                    SUM(tVentasProductos.Cantidad) AS CantidadProductos
                FROM 
                    tVentasProductos
                JOIN 
                    tVentas ON tVentasProductos.IdVenta = tVentas.IdVenta
                WHERE 
                    CONVERT(DATE, tVentas.Fecha) = CONVERT(DATE, GETDATE())
                    AND tVentas.TipoPago = @Estatus
                GROUP BY 
                    tVentas.IdVenta, CONVERT(VARCHAR(5), tVentas.Fecha, 108);";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@Estatus", estatus);

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add((
                                dr["IdVenta"].ToString(),
                                dr["HoraMinuto"].ToString(),
                                dr.GetDecimal(dr.GetOrdinal("TotalCompra")),
                                dr.GetInt32(dr.GetOrdinal("CantidadProductos"))
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo básico de excepciones
                Console.WriteLine($"Error al listar compras: {ex.Message}");
                lista = new List<(string IdVenta, string HoraMinuto, decimal TotalCompra, int CantidadProductos)>(); // Devolver lista vacía en caso de error
            }

            return lista;
        }


        public bool CancelarVenta(string IdVenta)
        {
            bool respuesta = false;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                    UPDATE tVentas
                    SET 
                        Estatus = 'C'
                    WHERE IdVenta = @IdVenta;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdVenta", IdVenta);

                    oconexion.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    respuesta = filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
            }

            return respuesta;
        }


        public bool ActualizarEstatus(string idVentasProducto)
        {
            bool respuesta = false;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                UPDATE tVentasProductos
                SET 
                    Estatus = 'V'
                WHERE IdVentasProducto = @IdVentasProducto;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdVentasProducto", idVentasProducto);

                    oconexion.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    respuesta = filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
            }

            return respuesta;
        }





        public bool FacturaRequerida(Venta venta)
        {
            bool respuesta = false;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                    SELECT RequiereFactura
                    FROM tVentas    
                    WHERE IdVenta = @IdVenta;";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdVenta", venta.IdVenta);

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            respuesta = Convert.ToBoolean(dr["RequiereFactura"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
            }

            return respuesta;
        }


        public bool ActualizarRequiereFactura(string id, bool factura, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                UPDATE tVentas 
                SET RequiereFactura = @RequiereFactura
                WHERE IdVenta = @IdVenta;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdVenta", id);
                        cmd.Parameters.AddWithValue("@RequiereFactura", factura);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Retorna verdadero si se actualizó al menos una fila
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false; // Retorna falso en caso de error
            }
        }

        public bool ExisteVenta(string id, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                SELECT COUNT(1) 
                FROM tVentas 
                WHERE IdVenta = @IdVenta;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdVenta", id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "No se a encontrado la venta";
                return false;
            }
        }

        public bool ActualizarDevolucion(string id, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                                    UPDATE tVentasProductos 
                                    SET Estatus = 'D'
                                    WHERE IdVentasProducto = @IdVenta;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdVenta", id);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "No se a podido encontrar la venta de este producto";
                return false;
            }
        }

        public string InsertarDevolucion(string idVenta, string idVentasProducto, string motivo, int idUsuarioRegistro, int cantidad, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                bool garantia = false;
                if (motivo != "pieza equivocada" || motivo != "no lo necesito")
                {
                    garantia = true;
                }

                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    // Modificación en la consulta para obtener el ID generado
                    string query = @"
                DECLARE @IdVentaDevolucion UNIQUEIDENTIFIER;
                SET @IdVentaDevolucion = NEWID();
                
                INSERT INTO tVentaDevolucion 
                (IdVentaDevolucion, IdVenta, IdVentasProducto, Garantia, Motivo, FechaRegistro, IdUsuarioRegistro, Cantidad)
                VALUES 
                (@IdVentaDevolucion, @IdVenta, @IdVentasProducto, @Garantia, @Motivo, GETDATE(), @IdUsuarioRegistro, @Cantidad);
                
                SELECT @IdVentaDevolucion;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        // Agregar los parámetros al comando
                        cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                        cmd.Parameters.AddWithValue("@IdVentasProducto", idVentasProducto);
                        cmd.Parameters.AddWithValue("@Garantia", garantia);
                        cmd.Parameters.AddWithValue("@Motivo", motivo.ToUpper());
                        cmd.Parameters.AddWithValue("@IdUsuarioRegistro", idUsuarioRegistro);
                        cmd.Parameters.AddWithValue("@Cantidad", cantidad);

                        // Ejecutar el comando y obtener el ID generado
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return result.ToString(); // Devolver el ID generado
                        }
                        else
                        {
                            mensaje = "No se pudo obtener el ID de la devolución.";
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al insertar la devolución: " + ex.Message;
                return null; // En caso de error, devuelve null
            }
        }



        public bool RegistrarCompraDevolucion(string idVentasProducto, int usuarioDevolucion, int cantidad, out string mensaje)
        {
            mensaje = string.Empty;
            Guid idCompraInsertada = Guid.Empty;
            int idProductoProveedor = 0;
            decimal precio = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                            INSERT INTO tCompras 
                            (CompraId, Estatus, Fecha, UsuarioModificacion, FechaModificacion)
                            VALUES 
                            (NEWID(), 'DE', GETDATE(), @UsuarioModificacion, GETDATE());";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioModificacion", usuarioDevolucion);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            query = "SELECT TOP 1 CompraId FROM tCompras ORDER BY FechaModificacion DESC";
                            cmd.CommandText = query;
                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                idCompraInsertada = (Guid)result;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    // Consulta para obtener el IdProducto basado en el IdVentasProducto
                    string queryProducto = @"
                SELECT vp.IdProducto
                FROM tVentasProductos vp
                WHERE vp.IdVentasProducto = @IdVentasProducto;";

                    int idProducto = 0;

                    using (SqlCommand cmdProducto = new SqlCommand(queryProducto, conexion))
                    {
                        cmdProducto.Parameters.AddWithValue("@IdVentasProducto", idVentasProducto);
                        object result = cmdProducto.ExecuteScalar();
                        if (result != null)
                        {
                            idProducto = Convert.ToInt32(result);
                        }
                        else
                        {
                            mensaje = "No se encontró el producto asociado al IdVentasProducto proporcionado.";
                            return false;
                        }
                    }

                    // Consulta para obtener el IdProductoProveedor y el precio del producto
                    string queryProveedorPrecio = @"
                SELECT pp.IdProductoProveedor, p.Precio
                FROM tProductosProveedores pp
                INNER JOIN tProductos p ON p.IdProducto = pp.IdProducto
                WHERE pp.IdProducto = @IdProducto AND pp.RFCProveedor = 'REFACCIONARIA';";

                    using (SqlCommand cmdProveedorPrecio = new SqlCommand(queryProveedorPrecio, conexion))
                    {
                        cmdProveedorPrecio.Parameters.AddWithValue("@IdProducto", idProducto);

                        using (SqlDataReader reader = cmdProveedorPrecio.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idProductoProveedor = reader.GetInt32(0);
                                precio = reader.GetDecimal(1);

                            }
                            else
                            {
                                mensaje = "No se encontró el proveedor 'REFACCIONARIA' asociado al producto.";
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener el producto y precio: " + ex.Message;
                return false;
            }
            mensaje = string.Empty;
            bool resultado = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                                        INSERT INTO tComprasDtl 
                                        (CompraId , ProductoProveedorId , Cantidad,CantidadEntrada,Estatus ,Precio , FechaEstimadaEntrega, UsuarioModificacion, FechaModificacion)
                                        VALUES 
                                        (@IdCompra, @IdProductoProveedor, @Cantidad, @Cantidad,'DE',@Precio , GETDATE(), @UsuarioModificacion, GETDATE());";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdCompra", idCompraInsertada);
                        cmd.Parameters.AddWithValue("@IdProductoProveedor", idProductoProveedor);
                        cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmd.Parameters.AddWithValue("@UsuarioModificacion", usuarioDevolucion);
                        cmd.Parameters.AddWithValue("@Precio ", precio);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            resultado = false;
                            mensaje = "No se pudo insertar el detalle de compra.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            return resultado;

        }

        public int ObtenerCantidadDevuelta(string idVentasProducto, out string mensaje)
        {
            mensaje = string.Empty;
            int cantidadDevuelta = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                            SELECT ISNULL(SUM(Cantidad), 0) AS CantidadDevuelta
                            FROM tVentaDevolucion
                            WHERE IdVentasProducto = @IdVentasProducto;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdVentasProducto", idVentasProducto);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            cantidadDevuelta = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener la cantidad de devoluciones: " + ex.Message;
            }

            return cantidadDevuelta;
        }
        public int ObtenerCantidadVendida(string idVentasProducto, out string mensaje)
        {
            mensaje = string.Empty;
            int cantidadVendida = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                            SELECT ISNULL(Cantidad, 0) AS CantidadVendida
                            FROM tVentasProductos
                            WHERE IdVentasProducto = @IdVentasProducto;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdVentasProducto", idVentasProducto);

                        // Ejecutar el query y obtener la cantidad vendida
                        object result = cmd.ExecuteScalar();

                        // Convertir el resultado a entero
                        if (result != null)
                        {
                            cantidadVendida = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener la cantidad vendida: " + ex.Message;
            }

            return cantidadVendida;
        }



        public List<Venta> ObtenerVentasPorID(string id)
        {
            List<Venta> ventas = new List<Venta>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cn))
            {
                conexion.Open();

                string query = @"
            SELECT 
                v.IdVenta, 
                v.IdCliente, 
                v.Estatus, 
                v.TipoPago, 
                v.NombreCliente, 
                v.Cambio, 
                v.RequiereFactura,
                v.Fecha,
                cajero.Nombre AS NombreCajero,
                vendedor.Nombre AS NombreVendedor
            FROM tVentas v
            INNER JOIN tVentasProductos vp ON v.IdVenta = vp.IdVenta
            INNER JOIN tProductos p ON p.IdProducto = vp.IdProducto
            LEFT JOIN tUsuarios cajero ON v.IdCajero = cajero.IdUsuario
            INNER JOIN tUsuarios vendedor ON v.IdVendedor = vendedor.IdUsuario
            WHERE p.IdProducto = @id";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Venta venta = new Venta
                            {
                                IdVenta = reader["IdVenta"].ToString(),
                                RFCCliente = reader["IdCliente"] == DBNull.Value ? "PUBLICO" : reader["IdCliente"].ToString(),
                                Estatus = reader["Estatus"].ToString(),
                                TipoPago = reader["TipoPago"].ToString(),
                                NombreCliente = reader["NombreCliente"].ToString(),
                                Cambio = reader["Cambio"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Cambio"]),
                                RequiereFactura = Convert.ToBoolean(reader["RequiereFactura"]),
                                oCajero = new Usuario { Nombre = reader["NombreCajero"] == DBNull.Value ? "N/A" : reader["NombreCajero"].ToString() },
                                oVendedor = new Usuario { Nombre = reader["NombreVendedor"].ToString() },
                                Fecha = Convert.ToString(reader["Fecha"])
                            };

                            ventas.Add(venta);
                        }
                    }
                }
            }

            return ventas;
        }


        public decimal ObtenerTotalPorCompra(string idCompra)
        {
            decimal total = 0;

            using (SqlConnection conexion = new SqlConnection(Conexion.cn))
            {
                conexion.Open();

                string query = @"
            SELECT SUM(CONVERT(decimal, vp.Cantidad) * vp.Precio) AS TotalPagado
            FROM tVentasProductos vp
            INNER JOIN tVentas v ON vp.IdVenta = v.IdVenta
            WHERE v.IdVenta = @IdCompra";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdCompra", idCompra);

                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        total = Convert.ToDecimal(result);
                    }
                }
            }

            return total;
        }


        public List<(string, string, string, string, string)> ListarDevoluciones(int pagina, int siguientes)
        {
            List<(string, string, string, string, string)> lista = new List<(string, string, string, string, string)>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT 
                    p.NoParte,
                    m.Descripcion AS Marca,
                    vd.Motivo,
                    vd.FechaRegistro,
                    vd.Cantidad
                FROM 
                    tVentaDevolucion vd
                INNER JOIN 
                    tVentasProductos vp ON vd.IdVentasProducto = vp.IdVentasProducto
                INNER JOIN 
                    tProductos p ON vp.IdProducto = p.IdProducto
                INNER JOIN 
                    tMarcas m ON m.IdMarca = p.IdMarca 
                    ORDER BY vd.FechaRegistro OFFSET " + pagina * siguientes + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string noParte = dr["NoParte"].ToString();
                            string marca = dr["Marca"].ToString();
                            string motivo = dr["Motivo"].ToString();
                            string fechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]).ToString("dd/MM/yyyy");
                            string cantidad = dr["Cantidad"].ToString();

                            lista.Add((noParte, marca, motivo, fechaRegistro, cantidad));
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<(string, string, string, string, string)>();
            }
            return lista;
        }

        //      COUNT Devoluciones
        public int countDevoluciones()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT COUNT(*) AS TotalRegistros 
                FROM 
                    tVentaDevolucion vd
                INNER JOIN 
                    tVentasProductos vp ON vd.IdVentasProducto = vp.IdVentasProducto
                INNER JOIN 
                    tProductos p ON vp.IdProducto = p.IdProducto
                INNER JOIN 
                    tMarcas m ON m.IdMarca = p.IdMarca;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    resultado = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }

        public List<(string, string, string, string, string)> ListarDevolucionesWhere(int pagina, int siguientes, string where)
        {
            List<(string, string, string, string, string)> lista = new List<(string, string, string, string, string)>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT 
                    p.NoParte,
                    m.Descripcion AS Marca,
                    vd.Motivo,
                    vd.FechaRegistro,
                    vd.Cantidad
                FROM 
                    tVentaDevolucion vd
                INNER JOIN 
                    tVentasProductos vp ON vd.IdVentasProducto = vp.IdVentasProducto
                INNER JOIN 
                    tProductos p ON vp.IdProducto = p.IdProducto
                INNER JOIN 
                    tMarcas m ON m.IdMarca = p.IdMarca
                WHERE " + where + " ORDER BY vd.FechaRegistro OFFSET " + pagina * siguientes + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string noParte = dr["NoParte"].ToString();
                            string marca = dr["Marca"].ToString();
                            string motivo = dr["Motivo"].ToString();
                            string fechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]).ToString("dd/MM/yyyy");
                            string cantidad = dr["Cantidad"].ToString();

                            lista.Add((noParte, marca, motivo, fechaRegistro, cantidad));
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<(string, string, string, string, string)>();
            }
            return lista;
        }

        //      COUNT Devoluciones WHERE
        public int countDevolucionesWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT COUNT(*) AS TotalRegistros 
                FROM 
                    tVentaDevolucion vd
                INNER JOIN 
                    tVentasProductos vp ON vd.IdVentasProducto = vp.IdVentasProducto
                INNER JOIN 
                    tProductos p ON vp.IdProducto = p.IdProducto
                INNER JOIN 
                    tMarcas m ON m.IdMarca = p.IdMarca
                WHERE " + where + ";";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    resultado = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }



    }
}

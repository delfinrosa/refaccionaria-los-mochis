using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Compra
    {
        public Guid RegistrarCompra(Compra objCompra, out string mensaje)
        {
            mensaje = string.Empty;
            Guid idCompraInsertada = Guid.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                            INSERT INTO tCompras 
                            (CompraId, Estatus, Fecha, UsuarioModificacion, FechaModificacion)
                            VALUES 
                            (NEWID(), 'AB', GETDATE(), @UsuarioModificacion, GETDATE());";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioModificacion", objCompra.UsuarioModificacion.IdUsuario);

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
            }

            return idCompraInsertada;
        }







        public int EditarCompra(Compra objCompra, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    string queryCompra = @"
                                        UPDATE tCompras 
                                        SET Estatus = @Estatus,
                                            Fecha = @Fecha,
                                            UsuarioAutorizo = @UsuarioAutorizo,
                                            FechaAutorizacion = @FechaAutorizacion,
                                            UsuarioModificacion = @UsuarioModificacion,
                                            FechaModificacion = GETDATE()
                                        WHERE CompraId = @CompraId";

                    using (SqlCommand cmdCompra = new SqlCommand(queryCompra, oconexion))
                    {
                        cmdCompra.Parameters.AddWithValue("@CompraId", objCompra.CompraId);
                        cmdCompra.Parameters.AddWithValue("@Estatus", objCompra.Estatus);
                        cmdCompra.Parameters.AddWithValue("@Fecha", objCompra.Fecha);
                        cmdCompra.Parameters.AddWithValue("@UsuarioAutorizo", objCompra.UsuarioAutorizo.IdUsuario);
                        cmdCompra.Parameters.AddWithValue("@FechaAutorizacion", objCompra.FechaAutorizacion);
                        cmdCompra.Parameters.AddWithValue("@UsuarioModificacion", objCompra.UsuarioModificacion.IdUsuario);
                        cmdCompra.ExecuteNonQuery();
                    }

                    return 1;
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return 0;
            }
        }



        /************TABLA************/
        public List<(string, string, int, decimal)> ListarCompra(int pagina, int siguientes, string Estatus)
        {
            List<(string, string, int, decimal)> lista = new List<(string, string, int, decimal)>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT 
                    c.CompraId,
                    c.Fecha,
                    COUNT(d.CompraDtlId) AS CantidadProductos,
                    SUM(d.Cantidad * d.Precio) AS TotalPrecio
                FROM 
                    tCompras c
                LEFT JOIN 
                    tComprasDtl d ON c.CompraId = d.CompraId
                WHERE 
                    c.Estatus = @Estatus
                GROUP BY 
                    c.CompraId, c.Fecha
                ORDER BY 
                    c.CompraId
                OFFSET @offset ROWS
                FETCH NEXT @siguientes ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@offset", pagina * siguientes);
                    cmd.Parameters.AddWithValue("@siguientes", siguientes);
                    cmd.Parameters.AddWithValue("@Estatus", Estatus);

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add((
                                dr["CompraId"].ToString(),
                                dr["Fecha"].ToString(),
                                dr.GetInt32(dr.GetOrdinal("CantidadProductos")),
                                dr.GetDecimal(dr.GetOrdinal("TotalPrecio"))
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo básico de excepciones, puedes personalizar según tus necesidades
                Console.WriteLine($"Error al listar compras: {ex.Message}");
                lista = new List<(string, string, int, decimal)>(); // Devolver lista vacía en caso de error
            }

            return lista;
        }

        //      COUNT Tabla
        public int CountCompras(string Estatus)
        {
            int totalRegistros = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tCompras WHERE Estatus = @Estatus;";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Estatus", Estatus);
                    oconexion.Open();
                    totalRegistros = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                totalRegistros = 0;
            }
            return totalRegistros;
        }
        //ACCIONES TABLA
        public int AprobarCompra(string compraId, int UsuarioAutorizo, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    string queryCompra = @"
                                    UPDATE tCompras 
                                    SET Estatus = 'AP',
                                        UsuarioAutorizo  = @UsuarioAutorizo ,
                                        FechaModificacion = GETDATE()
                                    WHERE CompraId = @CompraId";

                    using (SqlCommand cmdCompra = new SqlCommand(queryCompra, oconexion))
                    {
                        cmdCompra.Parameters.AddWithValue("@CompraId", compraId);
                        cmdCompra.Parameters.AddWithValue("@UsuarioAutorizo ", UsuarioAutorizo); // Asumiendo que tienes el ID del usuario

                        cmdCompra.ExecuteNonQuery();
                    }

                    return 1;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return 0;
            }
        }

        public int CancelarCompra(string compraId, int UsuarioCancelo, string MotivoCancelacion, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    string queryCompra = @"
                                    UPDATE tCompras 
                                    SET Estatus = 'CA',
                                        UsuarioCancelo   = @UsuarioCancelo ,
                                        FechaCancelacion  = GETDATE(),
                                        MotivoCancelacion   = @MotivoCancelacion
                                    WHERE CompraId = @CompraId";

                    using (SqlCommand cmdCompra = new SqlCommand(queryCompra, oconexion))
                    {
                        cmdCompra.Parameters.AddWithValue("@CompraId", compraId);
                        cmdCompra.Parameters.AddWithValue("@UsuarioCancelo", UsuarioCancelo); // Asumiendo que tienes el ID del usuario
                        cmdCompra.Parameters.AddWithValue("@MotivoCancelacion", MotivoCancelacion); // Asumiendo que tienes el ID del usuario

                        cmdCompra.ExecuteNonQuery();
                    }

                    return 1;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return 0;
            }
        }

        public int EliminarCompra(string compraId, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Primero eliminar los detalles de la compra
                    string queryDetallesCompra = @"
                                DELETE FROM tComprasDtl 
                                WHERE CompraId = @CompraId";

                    using (SqlCommand cmdDetallesCompra = new SqlCommand(queryDetallesCompra, oconexion))
                    {
                        cmdDetallesCompra.Parameters.AddWithValue("@CompraId", compraId);
                        cmdDetallesCompra.ExecuteNonQuery();
                    }

                    // Luego eliminar la compra
                    string queryCompra = @"
                                DELETE FROM tCompras 
                                WHERE CompraId = @CompraId";

                    using (SqlCommand cmdCompra = new SqlCommand(queryCompra, oconexion))
                    {
                        cmdCompra.Parameters.AddWithValue("@CompraId", compraId);
                        cmdCompra.ExecuteNonQuery();
                    }

                    return 1;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return 0;
            }
        }



        public List<CompraDtl> ObtenerDetallesCompra(string idCompra, out string mensaje)
        {
            mensaje = "";
            var detallesCompra = new List<CompraDtl>();

            try
            {
                using (var conexion = new SqlConnection(Conexion.cn))
                {
                    var query = @"
                SELECT 
                    cdt.Cantidad, 
                    cdt.CantidadEntrada,
                    cdt.CompraDtlId,
                    p.NoParte,
                    cdt.Precio
                FROM 
                    tComprasDtl cdt
                INNER JOIN 
                    tProductosProveedores pp ON cdt.ProductoProveedorId = pp.IdProductoProveedor
                INNER JOIN 
                    tProductos p ON pp.IdProducto = p.IdProducto
                WHERE 
                    cdt.CompraId = @CompraId
                    AND Estatus = 'FA'";

                    using (var cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@CompraId", idCompra);
                        conexion.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cantidad = Convert.ToInt32(reader["Cantidad"]);
                                var cantidadEntrada = reader["CantidadEntrada"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(reader["CantidadEntrada"]);

                                var detalle = new CompraDtl
                                {
                                    Cantidad = cantidad - cantidadEntrada,
                                    Precio = Convert.ToDouble(reader["Precio"]),
                                    CompraDtlId = reader["CompraDtlId"].ToString(),
                                    oProductoProveedor = new ProductoProveedor
                                    {
                                        oProducto = new Producto
                                        {
                                            NoParte = reader["NoParte"].ToString()
                                        }
                                    }
                                };
                                detallesCompra.Add(detalle);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener los detalles de la compra: " + ex.Message;
            }
            return detallesCompra;
        }







        public void ActualizarCompraDtl(List<CompraDtl> detallesCompra, int idUsuario)
        {
            using (var conexion = new SqlConnection(Conexion.cn))
            {
                conexion.Open();

                foreach (var detalle in detallesCompra)
                {
                    // Convertir el string CompraDtlId a GUID
                    Guid compraDtlId;
                    if (!Guid.TryParse(detalle.CompraDtlId, out compraDtlId))
                    {
                        throw new ArgumentException("El identificador de detalle de compra no es un GUID válido.");
                    }

                    // Actualizar el precio si está marcado
                    if (detalle.Precio > 0)  // Asumimos que si Precio > 0, se debe actualizar
                    {
                        var queryPrecio = @"
                        SELECT p.Precio
                        FROM tProductos p
                        INNER JOIN tProductosProveedores pp ON p.IdProducto = pp.IdProducto
                        INNER JOIN tComprasDtl cdt ON pp.IdProductoProveedor = cdt.ProductoProveedorId
                        WHERE cdt.CompraDtlId = @CompraDtlId";

                        decimal PrecioProducto;
                        using (var cmdPrecio = new SqlCommand(queryPrecio, conexion))
                        {
                            cmdPrecio.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                            PrecioProducto = (decimal)cmdPrecio.ExecuteScalar();
                        }

                        // Calcula el nuevo precio
                        double precioCompra = detalle.Precio * 1.16;  // Suponemos 1.16 como el factor de ajuste

                        // Comparar y actualizar si el precio del producto es menor
                        if (PrecioProducto < (decimal)precioCompra)
                        {
                            var queryUpdate = @"
                            UPDATE p
                            SET p.Precio = @NuevoPrecio
                            FROM tProductos p
                            INNER JOIN tProductosProveedores pp ON p.IdProducto = pp.IdProducto
                            INNER JOIN tComprasDtl cdt ON pp.IdProductoProveedor = cdt.ProductoProveedorId
                            WHERE cdt.CompraDtlId = @CompraDtlId"
                            ;

                            using (var cmdUpdate = new SqlCommand(queryUpdate, conexion))
                            {
                                cmdUpdate.Parameters.AddWithValue("@NuevoPrecio", (decimal)precioCompra);
                                cmdUpdate.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                    }

                    // Actualizar la cantidad si está marcada
                    if (detalle.Cantidad > 0)  // Asumimos que si Cantidad > 0, se debe actualizar
                    {
                        var queryUpdateCantidad = @"
                        UPDATE tComprasDtl
                        SET 
                            CantidadEntrada = @CantidadEntrada,
                            FechaEntrega = CASE WHEN FechaEntrega IS NULL THEN GETDATE() ELSE FechaEntrega END,
                            FechaModificacion = GETDATE(),
                            UsuarioModificacion = @IdUsuario,
                            Estatus = 'CO'
                        WHERE 
                            CompraDtlId = @CompraDtlId"
                        ;

                        using (var cmdUpdateCantidad = new SqlCommand(queryUpdateCantidad, conexion))
                        {
                            cmdUpdateCantidad.Parameters.AddWithValue("@CantidadEntrada", detalle.Cantidad);
                            cmdUpdateCantidad.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                            cmdUpdateCantidad.Parameters.AddWithValue("@IdUsuario", idUsuario);
                            cmdUpdateCantidad.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        var queryCantidadEntrada = @"
                        SELECT ISNULL(CantidadEntrada, 0) AS CantidadEntrada
                        FROM tComprasDtl
                        WHERE CompraDtlId = @CompraDtlId"
                        ;

                        int cantidadEntrada;
                        using (var cmdCantidadEntrada = new SqlCommand(queryCantidadEntrada, conexion))
                        {
                            cmdCantidadEntrada.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                            cantidadEntrada = (int)cmdCantidadEntrada.ExecuteScalar();
                        }

                        if (cantidadEntrada == 0)
                        {
                            var queryUpdateCantidad = @"
                            UPDATE tComprasDtl
                            SET 
                                CantidadEntrada = 0
                            WHERE 
                                CompraDtlId = @CompraDtlId"
                            ;

                            using (var cmdUpdateCantidad = new SqlCommand(queryUpdateCantidad, conexion))
                            {
                                cmdUpdateCantidad.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                                cmdUpdateCantidad.ExecuteNonQuery();
                            }
                        }

                    }

                }
            }
        }










        public void ActualizarCompraDtl(List<EstadoProducto> estadoProductos, int idUsuario)
        {
            using (var conexion = new SqlConnection(Conexion.cn))
            {
                conexion.Open();

                foreach (var item in estadoProductos)
                {
                    Guid compraDtlId;
                    if (!Guid.TryParse(item.CompraDtlId, out compraDtlId))
                    {
                        // Manejar el caso donde CompraDtlId no es un GUID válido
                        throw new ArgumentException("El identificador de detalle de compra no es un GUID válido.");
                    }

                    if (item.IsPrecioChecked)
                    {
                        var queryPrecio = @"
                SELECT p.Precio
                FROM tProductos p
                INNER JOIN tProductosProveedores pp ON p.IdProducto = pp.IdProducto
                INNER JOIN tComprasDtl cdt ON pp.IdProductoProveedor = cdt.ProductoProveedorId
                WHERE cdt.CompraDtlId = @CompraDtlId";

                        decimal PrecioProducto;
                        using (var cmdPrecio = new SqlCommand(queryPrecio, conexion))
                        {
                            cmdPrecio.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                            PrecioProducto = (decimal)cmdPrecio.ExecuteScalar();
                        }

                        var queryReferencia = @"
                SELECT 
                    pp.Referencia,
                    cdt.Precio AS PrecioCompra
                FROM 
                    tProductosProveedores pp
                INNER JOIN 
                    tComprasDtl cdt ON pp.IdProductoProveedor = cdt.ProductoProveedorId
                WHERE 
                    cdt.CompraDtlId = @CompraDtlId";

                        int referenciaProducto = 0;
                        double precioCompra = 0;

                        using (var cmdReferencia = new SqlCommand(queryReferencia, conexion))
                        {
                            cmdReferencia.Parameters.AddWithValue("@CompraDtlId", compraDtlId);

                            using (var reader = cmdReferencia.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    referenciaProducto = Convert.ToInt32(reader["Referencia"]);
                                    precioCompra = Convert.ToDouble(reader["PrecioCompra"]);
                                }
                            }
                        }

                        var queryUpdateProductoProveedor = @"
                                                            UPDATE pp
                                                            SET pp.Precio = @NuevoPrecio
                                                            FROM tProductosProveedores pp
                                                            INNER JOIN tComprasDtl cdt ON pp.IdProductoProveedor  = cdt.ProductoProveedorId
                                                            WHERE cdt.CompraDtlId = @CompraDtlId";

                        using (var cmdUpdateProductoProveedor = new SqlCommand(queryUpdateProductoProveedor, conexion))
                        {
                            cmdUpdateProductoProveedor.Parameters.AddWithValue("@NuevoPrecio", precioCompra);
                            cmdUpdateProductoProveedor.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                            cmdUpdateProductoProveedor.ExecuteNonQuery();
                        }

                        // Calcular el nuevo precio
                        precioCompra = precioCompra * 1.16;
                        precioCompra = precioCompra * (1 + (referenciaProducto / 100.0));
                        precioCompra = Convert.ToDouble(Redondeo5o0(Convert.ToInt32(precioCompra)));

                        // Comparar y actualizar si es necesario
                        if (PrecioProducto < (int)precioCompra)
                        {
                            var queryUpdate = @"
                                                UPDATE p
                                                SET p.Precio = @NuevoPrecio
                                                FROM tProductos p
                                                INNER JOIN tProductosProveedores pp ON p.IdProducto = pp.IdProducto
                                                INNER JOIN tComprasDtl cdt ON pp.IdProductoProveedor = cdt.ProductoProveedorId
                                                WHERE cdt.CompraDtlId = @CompraDtlId";

                            using (var cmdUpdate = new SqlCommand(queryUpdate, conexion))
                            {
                                cmdUpdate.Parameters.AddWithValue("@NuevoPrecio", (int)precioCompra);
                                cmdUpdate.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                    }

                    // Manejo de cantidad
                    if (item.IsCantidadChecked)
                    {
                        var queryCantidad = @"
                SELECT ISNULL(Cantidad, 0) AS Cantidad
                FROM tComprasDtl
                WHERE CompraDtlId = @CompraDtlId";

                        int cantidad;
                        using (var cmdCantidad = new SqlCommand(queryCantidad, conexion))
                        {
                            cmdCantidad.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                            cantidad = (int)cmdCantidad.ExecuteScalar();
                        }

                        var queryUpdateCantidad = @"
                                                    UPDATE tComprasDtl
                                                    SET 
                                                        CantidadEntrada = @Cantidad,
                                                        FechaEntrega = CASE WHEN FechaEntrega IS NULL THEN GETDATE() ELSE FechaEntrega END,
                                                        FechaModificacion = GETDATE(),
                                                        UsuarioModificacion = @IdUsuario,
                                                        Estatus = 'CO'
                                                    WHERE 
                                                        CompraDtlId = @CompraDtlId";


                        using (var cmdUpdateCantidad = new SqlCommand(queryUpdateCantidad, conexion))
                        {
                            cmdUpdateCantidad.Parameters.AddWithValue("@Cantidad", cantidad);
                            cmdUpdateCantidad.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                            cmdUpdateCantidad.Parameters.AddWithValue("@IdUsuario", idUsuario);  // Asegúrate de definir esta variable
                            cmdUpdateCantidad.ExecuteNonQuery();
                        }

                    }
                    else
                    {
                        var queryCantidadEntrada = @"
                SELECT ISNULL(CantidadEntrada, 0) AS CantidadEntrada
                FROM tComprasDtl
                WHERE CompraDtlId = @CompraDtlId";

                        int cantidadEntrada;
                        using (var cmdCantidadEntrada = new SqlCommand(queryCantidadEntrada, conexion))
                        {
                            cmdCantidadEntrada.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                            cantidadEntrada = (int)cmdCantidadEntrada.ExecuteScalar();
                        }

                        if (cantidadEntrada == 0)
                        {
                            var queryUpdateCantidad = @"
                    UPDATE tComprasDtl
                    SET 
                        CantidadEntrada = 0
                    WHERE 
                        CompraDtlId = @CompraDtlId";

                            using (var cmdUpdateCantidad = new SqlCommand(queryUpdateCantidad, conexion))
                            {
                                cmdUpdateCantidad.Parameters.AddWithValue("@CompraDtlId", compraDtlId);
                                cmdUpdateCantidad.ExecuteNonQuery();
                            }
                        }
                    }
                    bool aaa = VerificarYActualizarEstatusCompra(compraDtlId, idUsuario);
                }

            }
        }







        public async Task<bool> ActualizarCompraDtlIncompleto(List<CompraDtl> productosSinCantidad, int idUsuario)
        {
            try
            {
                using (var conexion = new SqlConnection(Conexion.cn))
                {
                    await conexion.OpenAsync();

                    foreach (var item in productosSinCantidad)
                    {
                        // Extraer CantidadEntrada actual
                        var queryGetCantidadEntrada = @"
                SELECT CantidadEntrada 
                FROM tComprasDtl 
                WHERE CompraDtlId = @CompraDtlId";

                        double cantidadEntradaActual = 0;

                        using (var cmdGetCantidadEntrada = new SqlCommand(queryGetCantidadEntrada, conexion))
                        {
                            cmdGetCantidadEntrada.Parameters.AddWithValue("@CompraDtlId", item.CompraDtlId);

                            using (var reader = await cmdGetCantidadEntrada.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    cantidadEntradaActual = Convert.ToDouble(reader["CantidadEntrada"]);
                                }
                                else
                                {
                                    continue; // No se encontró el registro, pasar al siguiente
                                }
                            }
                        }

                        // Sumar la cantidad nueva
                        double nuevaCantidadEntrada = cantidadEntradaActual + item.CantidadEntrada;

                        // Actualizar la cantidad de entrada
                        var queryUpdateCantidad = @"
                UPDATE tComprasDtl
                SET 
                    CantidadEntrada = @NuevaCantidadEntrada,
                    FechaModificacion = GETDATE(),
                    UsuarioModificacion = @IdUsuario,
                    Estatus = 'FA'
                WHERE 
                    CompraDtlId = @CompraDtlId";

                        using (var cmdUpdateCantidad = new SqlCommand(queryUpdateCantidad, conexion))
                        {
                            cmdUpdateCantidad.Parameters.AddWithValue("@NuevaCantidadEntrada", nuevaCantidadEntrada);
                            cmdUpdateCantidad.Parameters.AddWithValue("@CompraDtlId", item.CompraDtlId);
                            cmdUpdateCantidad.Parameters.AddWithValue("@IdUsuario", idUsuario);

                            await cmdUpdateCantidad.ExecuteNonQueryAsync();
                        }

                        // Verificar si CantidadEntrada es igual o mayor que Cantidad
                        var queryCheckCantidad = @"
                SELECT Cantidad, CantidadEntrada 
                FROM tComprasDtl 
                WHERE CompraDtlId = @CompraDtlId";

                        int cantidad = 0;
                        int cantidadEntrada = 0;

                        using (var cmdCheckCantidad = new SqlCommand(queryCheckCantidad, conexion))
                        {
                            cmdCheckCantidad.Parameters.AddWithValue("@CompraDtlId", item.CompraDtlId);

                            using (var reader = await cmdCheckCantidad.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    cantidad = Convert.ToInt32(reader["Cantidad"]);
                                    cantidadEntrada = Convert.ToInt32(reader["CantidadEntrada"]);
                                }
                            }
                        }

                        if (cantidadEntrada >= cantidad)
                        {
                            // Actualizar el estatus a 'CO'
                            var queryUpdateEstatus = @"
                    UPDATE tComprasDtl
                    SET 
                        Estatus = 'CO',
                        FechaModificacion = GETDATE(),
                        FechaEntrega = GETDATE(),
                        UsuarioModificacion = @IdUsuario
                    WHERE 
                        CompraDtlId = @CompraDtlId";

                            using (var cmdUpdateEstatus = new SqlCommand(queryUpdateEstatus, conexion))
                            {
                                cmdUpdateEstatus.Parameters.AddWithValue("@CompraDtlId", item.CompraDtlId);
                                cmdUpdateEstatus.Parameters.AddWithValue("@IdUsuario", idUsuario);

                                await cmdUpdateEstatus.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                // Podrías agregar un log de excepción aquí para más detalles si es necesario.
                return false;
            }
        }























        public int Redondeo5o0(int numero)
        {
            string numeroletra = numero.ToString();
            int ultimonumero = int.Parse(numeroletra[numeroletra.Length - 1].ToString());

            if (ultimonumero == 0 || ultimonumero == 5)
            {
                return numero; // No hay cambios si el último dígito es 0 o 5.
            }
            else if (ultimonumero >= 1 && ultimonumero <= 4)
            {
                // Reemplazar el último dígito con 5
                numeroletra = numeroletra.Substring(0, numeroletra.Length - 1) + "5";
                return int.Parse(numeroletra);
            }
            else if (ultimonumero >= 6 && ultimonumero <= 9)
            {
                // Reemplazar el último dígito con 0 y sumar 10
                numeroletra = numeroletra.Substring(0, numeroletra.Length - 1) + "0";
                return int.Parse(numeroletra) + 10;
            }

            return numero;
        }


        public bool VerificarYActualizarEstatusCompra(Guid compraId, int usuarioModificacion)
        {
                Guid ID;
            try
            {

                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                SELECT CompraId 
                FROM tComprasDtl 
                WHERE CompraDtlId = @CompraDtlId;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        // Parámetro para filtrar por CompraDtlId
                        cmd.Parameters.AddWithValue("@CompraDtlId", compraId);

                        // Ejecutar la consulta
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && resultado != DBNull.Value)
                        {
                            // Convertir el resultado a Guid
                            ID = (Guid)resultado;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();
                    string queryVerificarDetalles = @"
                SELECT COUNT(*) 
                FROM tComprasDtl 
                WHERE CompraId = @CompraId AND Estatus != 'CO';";

                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificarDetalles, conexion))
                    {
                        cmdVerificar.Parameters.AddWithValue("@CompraId", ID);

                        int detallesPendientes = (int)cmdVerificar.ExecuteScalar();

                        if (detallesPendientes == 0)
                        {
                            string queryActualizarCompra = @"
                        UPDATE tCompras 
                        SET Estatus = 'CE', 
                            FechaModificacion = GETDATE(), 
                            UsuarioModificacion = @UsuarioModificacion
                        WHERE CompraId = @CompraId;";

                            using (SqlCommand cmdActualizar = new SqlCommand(queryActualizarCompra, conexion))
                            {
                                cmdActualizar.Parameters.AddWithValue("@CompraId", ID);
                                cmdActualizar.Parameters.AddWithValue("@UsuarioModificacion", usuarioModificacion);

                                int rowsAffected = cmdActualizar.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    return true; 
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<(string CompraId, string Proveedor, string FechaPedido, string FechaEntrega, string CantidadProductosPedidos, string TotalPedido)> ObtenerComprasPorProductoID(int productoId)
        {
            var compras = new List<(string, string, string, string, string, string)>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cn))
            {
                conexion.Open();

                string query = @"
                                SELECT 
                                    c.CompraId,
                                    pproveedor.RFCProveedor AS Proveedor,
                                    c.Fecha AS FechaPedido,
                                    cd.FechaEntrega,
                                    cd.Cantidad AS CantidadProductosPedidos,
                                    (cd.Cantidad * pproveedor.Precio) AS TotalPedido
                                FROM tCompras c
                                INNER JOIN tComprasDtl cd ON c.CompraId = cd.CompraId
                                INNER JOIN tProductosProveedores pproveedor ON cd.ProductoProveedorId = pproveedor.IdProductoProveedor
                                INNER JOIN tProductos p ON pproveedor.IdProducto = p.IdProducto
                                WHERE p.IdProducto = @ProductoId";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@ProductoId", productoId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var compra = (
                                CompraId: reader["CompraId"].ToString(),
                                Proveedor: reader["Proveedor"].ToString(),
                                FechaPedido: Convert.ToDateTime(reader["FechaPedido"]).ToShortDateString(),
                                FechaEntrega: reader["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEntrega"]).ToShortDateString() : "N/A",
                                CantidadProductosPedidos: reader["CantidadProductosPedidos"].ToString(),
                                TotalPedido: Convert.ToDecimal(reader["TotalPedido"]).ToString("C")
                            );

                            compras.Add(compra);
                        }
                    }
                }
            }

            return compras;
        }



    }
}

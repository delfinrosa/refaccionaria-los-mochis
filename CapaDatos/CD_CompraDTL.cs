using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_CompraDTL
    {

        public List<CompraDtl> ObtenerCompraDtlPorNoParte(string noParte, out string mensaje)
        {
            mensaje = string.Empty;
            List<CompraDtl> comprasDtlEncontrados = new List<CompraDtl>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT pp.IdProductoProveedor, pr.RazonSocial, p.Descripcion , pp.precio
            FROM tProductosProveedores pp
            JOIN tProductos p ON pp.IdProducto = p.IdProducto
            JOIN tProveedores pr ON pp.RFCProveedor = pr.RFC
            WHERE p.NoParte = @NoParte";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@NoParte", noParte);

                    conexion.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        CompraDtl compraDtl = new CompraDtl
                        {
                            oProductoProveedor = new ProductoProveedor
                            {
                                IdProductoProveedor = Convert.ToInt32(dr["IdProductoProveedor"]),
                                oProveedor = new Proveedor
                                {
                                    RazonSocial = Convert.ToString(dr["RazonSocial"])
                                },
                                oProducto = new Producto
                                {
                                    Descripcion = Convert.ToString(dr["Descripcion"])
                                },
                                Precio = Convert.ToInt32(dr["precio"])
                            }
                        };

                        comprasDtlEncontrados.Add(compraDtl);
                    }

                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener CompraDtl por número de parte: " + ex.Message;
            }

            return comprasDtlEncontrados;
        }



        public bool RegistrarCompraDtl(CompraDtl objCompraDtl, string idcompra, int  usuario, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                INSERT INTO tComprasDtl 
                (CompraId , ProductoProveedorId , Cantidad,Estatus ,Precio , FechaEstimadaEntrega, UsuarioModificacion, FechaModificacion)
                VALUES 
                (@IdCompra, @IdProductoProveedor, @Cantidad,'FA',@Precio , @FechaEstimadaEntrega, @UsuarioModificacion, GETDATE());";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdCompra", idcompra); 
                        cmd.Parameters.AddWithValue("@IdProductoProveedor", objCompraDtl.oProductoProveedor.IdProductoProveedor);
                        cmd.Parameters.AddWithValue("@Cantidad", objCompraDtl.Cantidad);
                        cmd.Parameters.AddWithValue("@FechaEstimadaEntrega", objCompraDtl.FechaEstimadaEntrega.Date);
                        cmd.Parameters.AddWithValue("@UsuarioModificacion", usuario);
                        cmd.Parameters.AddWithValue("@Precio ", objCompraDtl.Precio);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se pudo insertar el detalle de compra.";
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

        public List<CompraDtl> ObtenerCompraDtlPorCompraId(string compraId)
        {
            List<CompraDtl> comprasDtlEncontrados = new List<CompraDtl>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                                    SELECT cdtl.CompraDtlId, cdtl.ProductoProveedorId, cdtl.Cantidad, cdtl.Precio,
                                           pp.IdProductoProveedor, pr.RazonSocial, prod.NoParte, prod.Descripcion,
                                           usr.Nombre, c.Estatus, c.Fecha
                                    FROM tComprasDtl cdtl
                                    INNER JOIN tProductosProveedores pp ON cdtl.ProductoProveedorId = pp.IdProductoProveedor
                                    INNER JOIN tProveedores pr ON pp.RFCProveedor = pr.RFC
                                    INNER JOIN tProductos prod ON pp.IdProducto = prod.IdProducto
                                    INNER JOIN tCompras c ON cdtl.CompraId = c.CompraId
                                    INNER JOIN tUsuarios usr ON c.UsuarioModificacion = usr.IdUsuario
                                    WHERE cdtl.CompraId = @CompraId";


                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@CompraId", compraId);

                    conexion.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        CompraDtl compraDtl = new CompraDtl
                        {
                            CompraDtlId = Convert.ToString(dr["CompraDtlId"]),
                            oProductoProveedor = new ProductoProveedor
                            {
                                IdProductoProveedor = Convert.ToInt32(dr["IdProductoProveedor"]),
                                oProveedor = new Proveedor
                                {
                                    RazonSocial = Convert.ToString(dr["RazonSocial"])
                                },
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                oProducto = new Producto
                                {
                                    NoParte = Convert.ToString(dr["NoParte"]),
                                    Descripcion = Convert.ToString(dr["Descripcion"])
                                }
                            },
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Precio = Convert.ToDouble(dr["Precio"]),
                            UsuarioModificacion = new Usuario
                            {
                                Nombre = Convert.ToString(dr["Nombre"])
                            },
                            Compra = new Compra {
                                Estatus = Convert.ToString(dr["Estatus"]),
                                Fecha = Convert.ToString(dr["Fecha"])
                            }
                        };

                        comprasDtlEncontrados.Add(compraDtl);
                    }

                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejo básico de excepciones, puedes personalizar según tus necesidades
                Console.WriteLine($"Error al obtener detalle de compra por ID de compra: {ex.Message}");
            }

            return comprasDtlEncontrados;
        }

        public bool ActualizarCantidadCompraDtl(string idCompraDtl, int nuevaCantidad, int usuarioModificacion, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"
                UPDATE tComprasDtl 
                SET Cantidad = @NuevaCantidad,
                    UsuarioModificacion = @UsuarioModificacion,
                    FechaModificacion = GETDATE()
                WHERE CompraDtlId = @IdCompraDtl;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@NuevaCantidad", nuevaCantidad);
                        cmd.Parameters.AddWithValue("@UsuarioModificacion", usuarioModificacion);
                        cmd.Parameters.AddWithValue("@IdCompraDtl", idCompraDtl);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se pudo actualizar la cantidad del detalle de compra.";
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


        public int EliminarCompraDTL(string compraId, out string mensaje)
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
                                WHERE CompraDtlId  = @CompraId";

                    using (SqlCommand cmdDetallesCompra = new SqlCommand(queryDetallesCompra, oconexion))
                    {
                        cmdDetallesCompra.Parameters.AddWithValue("@CompraId", compraId);
                        cmdDetallesCompra.ExecuteNonQuery();
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




















    }


}

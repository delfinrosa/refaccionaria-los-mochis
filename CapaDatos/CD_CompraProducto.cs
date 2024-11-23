using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_CompraProducto
    {

        public int RegistrarCompraProducto(CompraProducto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    string queryCompraProducto = @"
INSERT INTO tComprasProductos 
(IdCompra, ProductoId, Cantidad, CostoUnitario, UsuarioModificacion, FechaModificacion)
VALUES 
(@IdCompra, @ProductoId, @Cantidad, @CostoUnitario, @UsuarioModificacion, GETDATE());
SELECT SCOPE_IDENTITY();"; // Obtener el último ID insertado.

                    using (SqlCommand cmd = new SqlCommand(queryCompraProducto, oconexion))
                    {
                        cmd.Parameters.AddWithValue("@IdCompra", obj.Compra.CompraId);
                        cmd.Parameters.AddWithValue("@ProductoId", obj.Producto.IdProducto);
                        cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
                        cmd.Parameters.AddWithValue("@CostoUnitario", obj.CostoUnitario);
                        cmd.Parameters.AddWithValue("@UsuarioModificacion", obj.Usuario.IdUsuario); // Asegúrate que el objeto Usuario dentro de CompraProducto está correctamente instanciado y contiene IdUsuario

                        // Ejecutar la consulta y obtener el último ID insertado.
                        int idCompraProductoInsertada = Convert.ToInt32(cmd.ExecuteScalar());
                        return idCompraProductoInsertada; // Devuelve el ID de la compra de producto insertada.
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return 0; // Devuelve 0 en caso de error.
            }
        }








        // GUARDAR COMPRA DE PRODUCTO Y DEVOLVER ID
        public int EditarCompraProducto(CompraProducto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    string queryCompraProducto = @"
INSERT INTO tComprasProductos 
(IdCompra, ProductoId, Cantidad, CostoUnitario, UsuarioModificacion, FechaModificacion)
VALUES 
(@IdCompra, @ProductoId, @Cantidad, @CostoUnitario, @UsuarioModificacion, GETDATE());
SELECT SCOPE_IDENTITY();"; // Obtener el último ID insertado.

                    using (SqlCommand cmd = new SqlCommand(queryCompraProducto, oconexion))
                    {
                        cmd.Parameters.AddWithValue("@IdCompra", obj.Compra.CompraId);
                        cmd.Parameters.AddWithValue("@ProductoId", obj.Producto.IdProducto);
                        cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
                        cmd.Parameters.AddWithValue("@CostoUnitario", obj.CostoUnitario);
                        cmd.Parameters.AddWithValue("@UsuarioModificacion", obj.Usuario.IdUsuario); // Asegúrate que el objeto Usuario dentro de CompraProducto está correctamente instanciado y contiene IdUsuario

                        // Ejecutar la consulta y obtener el último ID insertado.
                        int idCompraProductoInsertada = Convert.ToInt32(cmd.ExecuteScalar());
                        return idCompraProductoInsertada; // Devuelve el ID de la compra de producto insertada.
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return 0; // Devuelve 0 en caso de error.
            }
        }

    }
}

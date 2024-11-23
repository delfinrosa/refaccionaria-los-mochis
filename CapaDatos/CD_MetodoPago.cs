using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_MetodoPago
    {
        public bool Registrar(MetodoPago metodoPago, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"INSERT INTO tCFDIMetodoPago 
                             (CFDIMetodoPagoId, Estatus, Descripcion, PersonaUltimoCambio, PersonaRegistro, FechaRegistro, FechaUltimoCambio) 
                             VALUES 
                             (@CFDIMetodoPagoId, @Estatus, @Descripcion, @PersonaUltimoCambio, @PersonaRegistro, GETDATE(), GETDATE());";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@CFDIMetodoPagoId", metodoPago.CFDIMetodoPagoId);
                    cmd.Parameters.AddWithValue("@Estatus", metodoPago.Estatus);
                    cmd.Parameters.AddWithValue("@Descripcion", metodoPago.Descripcion);
                    cmd.Parameters.AddWithValue("@PersonaUltimoCambio", metodoPago.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@PersonaRegistro", metodoPago.oUsuario.IdUsuario);

                    conexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        resultado = true;
                    }
                    else
                    {
                        mensaje = "No se insertó ningún registro.";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al intentar registrar: " + ex.Message;
            }

            return resultado;
        }

        public bool Editar(MetodoPago obj, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"UPDATE tCFDIMetodoPago SET 
                            Estatus = @Estatus, 
                            Descripcion = @Descripcion, 
                            FechaUltimoCambio = GETDATE(), 
                            PersonaUltimoCambio = @PersonaUltimoCambio
                            WHERE CFDIMetodoPagoId = @CFDIMetodoPagoId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@CFDIMetodoPagoId", obj.CFDIMetodoPagoId);
                        cmd.Parameters.AddWithValue("@Estatus", obj.Estatus);
                        cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);

                        conexion.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró el registro a actualizar o no se realizó ninguna modificación.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al editar el método de pago: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        public bool Eliminar(string CFDIMetodoPagoId, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "DELETE FROM tCFDIMetodoPago WHERE CFDIMetodoPagoId = @CFDIMetodoPagoId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@CFDIMetodoPagoId", CFDIMetodoPagoId);

                        conexion.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró el método de pago a eliminar.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar el método de pago: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }

        public MetodoPago UltimoRegistro()
        {
            MetodoPago metodoPago = null;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT TOP 1 CFDIMetodoPagoId, Estatus, Descripcion
                FROM tCFDIMetodoPago
                ORDER BY FechaRegistro DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        conexion.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                metodoPago = new MetodoPago
                                {
                                    CFDIMetodoPagoId = dr["CFDIMetodoPagoId"].ToString(),
                                    Estatus = dr["Estatus"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString(),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return metodoPago;
        }






        // Buscar Método de Pago por Descripción
        public List<MetodoPago> BuscarMetodoPagoPorDescripcion(string descripcion, int pagina, int cantidadPorPagina)
        {
            List<MetodoPago> lista = new List<MetodoPago>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT CFDIMetodoPagoId, Descripcion FROM tCFDIMetodoPago
                             WHERE Descripcion LIKE @Descripcion
                             ORDER BY Descripcion
                             OFFSET @Pagina ROWS FETCH NEXT @CantidadPorPagina ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Descripcion", $"%{descripcion}%");
                    cmd.Parameters.AddWithValue("@Pagina", pagina * cantidadPorPagina);
                    cmd.Parameters.AddWithValue("@CantidadPorPagina", cantidadPorPagina);

                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MetodoPago
                            {
                                CFDIMetodoPagoId = dr["CFDIMetodoPagoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones o log de errores según sea necesario
            }

            return lista;
        }

        // Buscar Método de Pago por ID
        public List<MetodoPago> BuscarMetodoPagoPorID(string id, int pagina, int cantidadPorPagina)
        {
            List<MetodoPago> lista = new List<MetodoPago>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT CFDIMetodoPagoId, Descripcion FROM tCFDIMetodoPago
                             WHERE CFDIMetodoPagoId LIKE @Id
                             ORDER BY CFDIMetodoPagoId
                             OFFSET @Pagina ROWS FETCH NEXT @CantidadPorPagina ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Id", $"%{id}%");
                    cmd.Parameters.AddWithValue("@Pagina", pagina * cantidadPorPagina);
                    cmd.Parameters.AddWithValue("@CantidadPorPagina", cantidadPorPagina);

                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MetodoPago
                            {
                                CFDIMetodoPagoId = dr["CFDIMetodoPagoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones o log de errores según sea necesario
            }

            return lista;
        }
        public MetodoPago BuscarMetodoPagoPorNombre(string nombre)
        {
            MetodoPago metodoPago = new MetodoPago();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIMetodoPagoId, Descripcion, Estatus FROM tCFDIMetodoPago WHERE Descripcion = @Nombre";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            metodoPago = new MetodoPago
                            {
                                CFDIMetodoPagoId = Convert.ToString(dr["CFDIMetodoPagoId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Estatus = Convert.ToString(dr["Estatus"]),
                                // Asumiendo que existe una propiedad para Usuario en el objeto MetodoPago
                                // oUsuario podría necesitar ser llenado aquí si tu estructura lo requiere
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                metodoPago = new MetodoPago();
            }
            return metodoPago;
        }

        public MetodoPago BuscarPorId(string CFDIMetodoPagoId)
        {
            MetodoPago metodoPago = new MetodoPago();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIMetodoPagoId, Descripcion, Estatus FROM tCFDIMetodoPago WHERE CFDIMetodoPagoId = @CFDIMetodoPagoId";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CFDIMetodoPagoId", CFDIMetodoPagoId);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            metodoPago = new MetodoPago
                            {
                                CFDIMetodoPagoId = Convert.ToString(dr["CFDIMetodoPagoId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Estatus = Convert.ToString(dr["Estatus"]),
                                // Aquí también, asumiendo la existencia de una propiedad para Usuario
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                metodoPago = new MetodoPago();
            }
            return metodoPago;
        }


        // Contar entradas de Método de Pago según una condición
        public int ContarMetodosPago(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(CFDIMetodoPagoId) FROM tCFDIMetodoPago WHERE {where};";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    resultado = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones o log de errores según sea necesario
            }

            return resultado;
        }
        public List<MetodoPago> ListarMetodoPagoTabla(int pagina, string tipoOrden, int siguientes)
        {
            List<MetodoPago> lista = new List<MetodoPago>();
            try
            {
                string orden = Orden(tipoOrden); // Asegúrate de tener un método Orden que maneje los casos para MetodoPago.
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT CFDIMetodoPagoId, Descripcion, Estatus FROM tCFDIMetodoPago ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MetodoPago
                            {
                                CFDIMetodoPagoId = dr["CFDIMetodoPagoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                                // Asegúrate de manejar la propiedad oUsuario si es necesario aquí.
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<MetodoPago>();
            }
            return lista;
        }
        public int CountTablaMetodoPago()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tCFDIMetodoPago;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    resultado = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }
        /************TABLA CON WHERE************/
        public List<MetodoPago> ListarMetodoPagoTablaWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<MetodoPago> lista = new List<MetodoPago>();
            try
            {
                string orden = Orden(tipoOrden); // Asegúrate de utilizar el método adaptado para FormaPago
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT CFDIMetodoPagoId, Descripcion, Estatus FROM tCFDIMetodoPago  WHERE {where} ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new MetodoPago
                            {
                                CFDIMetodoPagoId = dr["CFDIMetodoPagoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<MetodoPago>();
            }
            return lista;
        }

        // COUNT
        public int CountTablaWhereMetodoPago(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(*) AS TotalRegistros FROM tFormaPago m WHERE {where};";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            resultado = Convert.ToInt32(dr["TotalRegistros"]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }
        public List<string> elementosPaginacionBuscadorMetodoPago(string nombre, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT Descripcion 
                             FROM tCFDIMetodoPago 
                             WHERE Descripcion LIKE @Nombre + '%' 
                             ORDER BY Descripcion 
                             OFFSET @Pagina ROWS FETCH NEXT @Siguientes ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Pagina", pagina);
                    cmd.Parameters.AddWithValue("@Siguientes", siguientes);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(Convert.ToString(dr["Descripcion"]));
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<string>();
            }
            return lista;
        }
        public int countBuscadorMetodoPago(string nombre)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT COUNT(*) AS TotalRegistros 
                             FROM tCFDIMetodoPago 
                             WHERE Descripcion LIKE '%' + @Nombre + '%';";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    conexion.Open();
                    resultado = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }

        public string Orden(string tipoOrden)
        {
            switch (tipoOrden)
            {
                case "I_A":
                    return "CFDIMetodoPagoId";
                case "I_D":
                    return "CFDIMetodoPagoId DESC";
                case "D_A":
                    return "Descripcion";
                case "D_D":
                    return "Descripcion DESC";
                default:
                    return "CFDIMetodoPagoId";
            }
        }



    }
}

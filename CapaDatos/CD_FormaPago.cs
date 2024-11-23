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
    public class CD_FormaPago
    {
        public bool Registrar(FormaPago formaPago, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"INSERT INTO tCFDIFormaPago (CFDIFormaPagoId, Estatus, Descripcion, PersonaUltimoCambio, PersonaRegistro, FechaRegistro, FechaUltimoCambio) 
                             VALUES (@CFDIFormaPagoId, @Estatus, @Descripcion, @PersonaUltimoCambio, @PersonaRegistro, GETDATE(), GETDATE());";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@CFDIFormaPagoId", formaPago.CFDIFormaPagoId);
                    cmd.Parameters.AddWithValue("@Estatus", formaPago.Estatus);
                    cmd.Parameters.AddWithValue("@Descripcion", formaPago.Descripcion);
                    cmd.Parameters.AddWithValue("@PersonaUltimoCambio", formaPago.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@PersonaRegistro", formaPago.oUsuario.IdUsuario);

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
                mensaje = "Error al insertar la forma de pago: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        public bool Editar(FormaPago obj, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();
                    string query = @"
                UPDATE tCFDIFormaPago SET 
                Descripcion = @Descripcion, 
                Estatus = @Estatus,
                FechaUltimoCambio = GETDATE(),
                PersonaUltimoCambio = @PersonaUltimoCambio
                WHERE CFDIFormaPagoId = @CFDIFormaPagoId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@CFDIFormaPagoId", obj.CFDIFormaPagoId);
                        cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                        cmd.Parameters.AddWithValue("@Estatus", obj.Estatus);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró el registro para actualizar.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al editar la forma de pago: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        public bool Eliminar(string cfdiFormaPagoId, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();
                    string query = @"
                DELETE FROM tCFDIFormaPago 
                WHERE CFDIFormaPagoId = @CFDIFormaPagoId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@CFDIFormaPagoId", cfdiFormaPagoId);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró el registro para eliminar.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar la forma de pago: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }

        public FormaPago UltimoRegistro()
        {
            FormaPago formaPago = new FormaPago();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 CFDIFormaPagoId, Descripcion, Estatus FROM tCFDIFormaPago ORDER BY FechaUltimoCambio DESC";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            formaPago = new FormaPago
                            {
                                CFDIFormaPagoId = Convert.ToString(dr["CFDIFormaPagoId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Estatus = Convert.ToString(dr["Estatus"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                formaPago = new FormaPago(); 
            }
            return formaPago;
        }
        public FormaPago BuscarFormaPagoPorDescripcion(string descripcion)
        {
            FormaPago formaPago = new FormaPago();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIFormaPagoId, Descripcion, Estatus FROM tCFDIFormaPago WHERE Descripcion = @Descripcion";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            formaPago = new FormaPago
                            {
                                CFDIFormaPagoId = dr["CFDIFormaPagoId"].ToString(),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Estatus = Convert.ToString(dr["Estatus"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                formaPago = new FormaPago();
            }
            return formaPago;
        }

        // Buscar una Forma de Pago por el ID
        public FormaPago BuscarFormaPagoPorId(string id)
        {
            FormaPago formaPago = new FormaPago();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIFormaPagoId, Descripcion, Estatus FROM tCFDIFormaPago WHERE CFDIFormaPagoId = @Id";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            formaPago = new FormaPago
                            {
                                CFDIFormaPagoId = dr["CFDIFormaPagoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                formaPago = new FormaPago(); // Si ocurre un error, retorna un objeto FormaPago vacío.
            }
            return formaPago;
        }



        /************BUSCADOR************/
        public List<string> ElementosPaginacionBuscadorFormaPago(string descripcion, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT Descripcion FROM tCFDIFormaPago 
                             WHERE Descripcion LIKE @Descripcion 
                             ORDER BY Descripcion DESC 
                             OFFSET @Pagina ROWS FETCH NEXT @Siguientes ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Descripcion", $"%{descripcion}%");
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


        //      COUNT
        public int CountBuscadorFormaPago(string descripcion)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tCFDIFormaPago WHERE Descripcion LIKE @Descripcion;";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Descripcion", $"%{descripcion}%");
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




        public List<FormaPago> BuscarFormaPagoPorDescripcion(string descripcion, int pagina, int cantidadPorPagina)
        {
            List<FormaPago> lista = new List<FormaPago>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT CFDIFormaPagoId, Descripcion FROM tCFDIFormaPago
                                 WHERE Descripcion LIKE @Descripcion
                                 ORDER BY Descripcion
                                 OFFSET @Pagina ROWS FETCH NEXT @CantidadPorPagina ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Descripcion", $"%{descripcion}%");
                    cmd.Parameters.AddWithValue("@Pagina", pagina  * cantidadPorPagina);
                    cmd.Parameters.AddWithValue("@CantidadPorPagina", cantidadPorPagina);

                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new FormaPago
                            {
                                CFDIFormaPagoId = dr["CFDIFormaPagoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return lista;
        }

        public List<FormaPago> BuscarFormaPagoPorId(string id, int pagina, int cantidadPorPagina)
        {
            List<FormaPago> lista = new List<FormaPago>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT CFDIFormaPagoId, Descripcion FROM tCFDIFormaPago
                                 WHERE CFDIFormaPagoId LIKE @Id
                                 ORDER BY CFDIFormaPagoId
                                 OFFSET @Pagina ROWS FETCH NEXT @CantidadPorPagina ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Id", $"%{id}%");
                    cmd.Parameters.AddWithValue("@Pagina", pagina  * cantidadPorPagina);
                    cmd.Parameters.AddWithValue("@CantidadPorPagina", cantidadPorPagina);

                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new FormaPago
                            {
                                CFDIFormaPagoId = dr["CFDIFormaPagoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return lista;
        }

        public int ContarFormasPago(string where)
        {
            int resultado = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(CFDIFormaPagoId) AS TotalRegistros FROM tCFDIFormaPago WHERE {where};";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    resultado = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
            }

            return resultado;
        }

        //TABLA
        public List<FormaPago> ListarFormaPagoTabla(int pagina, string tipoOrden, int siguientes)
        {
            List<FormaPago> lista = new List<FormaPago>();
            try
            {
                string orden = Orden(tipoOrden); // Asumiendo que tienes un método Orden que determina cómo se ordenan los resultados
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT f.CFDIFormaPagoId, f.Estatus, f.Descripcion FROM tCFDIFormaPago f ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new FormaPago
                            {
                                CFDIFormaPagoId = dr["CFDIFormaPagoId"].ToString(),
                                Estatus = dr["Estatus"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                // En un caso real, aquí deberías manejar la excepción adecuadamente.
                lista = new List<FormaPago>();
            }
            return lista;
        }
        //      COUNT para Forma de Pago
        public int CountTabla()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tCFDIFormaPago ;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    resultado = Convert.ToInt32(cmd.ExecuteScalar()); 
                }
            }
            catch (Exception)
            {
                resultado = 0; // En caso de error, se retorna 0
            }
            return resultado;
        }


        /************TABLA CON WHERE PARA FORMA DE PAGO************/
        public List<FormaPago> ListarFormaPagoTablaWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<FormaPago> lista = new List<FormaPago>();
            try
            {
                string orden = Orden(tipoOrden); // Asegúrate de ajustar este método para que funcione con FormaPago
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT f.CFDIFormaPagoId, f.Descripcion, f.Estatus FROM tCFDIFormaPago f WHERE {where} ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new FormaPago
                            {
                                CFDIFormaPagoId = dr["CFDIFormaPagoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<FormaPago>();
            }
            return lista;
        }
        // COUNT CON WHERE PARA FORMA DE PAGO
        public int CountTablaWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(*) AS TotalRegistros FROM tCFDIFormaPago WHERE {where};";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    resultado = Convert.ToInt32(cmd.ExecuteScalar());
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
                    return "f.CFDIFormaPagoId";
                case "I_D":
                    return "f.CFDIFormaPagoId DESC";
                case "D_A":
                    return "f.Descripcion";
                case "D_D":
                    return "f.Descripcion DESC";
                default:
                    return "f.CFDIFormaPagoId"; 
            }
        }


    }

}

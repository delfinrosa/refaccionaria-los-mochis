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
    public class CD_RegimenFiscal
    {
        public bool Registrar(RegimenFiscal regimenFiscal, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"INSERT INTO tCFDIRegimenFiscal (CFDIRegimenFiscalId, Estatus, Descripcion, PersonaUltimoCambio, PersonaRegistro, FechaRegistro, FechaUltimoCambio) 
                            VALUES (@CFDIRegimenFiscalId, @Estatus, @Descripcion, @PersonaUltimoCambio, @PersonaRegistro, GETDATE(), GETDATE());";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@CFDIRegimenFiscalId", regimenFiscal.CFDIRegimenFiscalId);
                    cmd.Parameters.AddWithValue("@Estatus", regimenFiscal.Estatus);
                    cmd.Parameters.AddWithValue("@Descripcion", regimenFiscal.Descripcion);
                    cmd.Parameters.AddWithValue("@PersonaUltimoCambio", regimenFiscal.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@PersonaRegistro", regimenFiscal.oUsuario.IdUsuario);

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
                mensaje = "Error al insertar el régimen fiscal: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        public bool Editar(RegimenFiscal regimenFiscal, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"UPDATE tCFDIRegimenFiscal 
                            SET Estatus = @Estatus, Descripcion = @Descripcion, 
                                PersonaUltimoCambio = @PersonaUltimoCambio, FechaUltimoCambio = GETDATE() 
                            WHERE CFDIRegimenFiscalId = @CFDIRegimenFiscalId;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@CFDIRegimenFiscalId", regimenFiscal.CFDIRegimenFiscalId);
                    cmd.Parameters.AddWithValue("@Estatus", regimenFiscal.Estatus);
                    cmd.Parameters.AddWithValue("@Descripcion", regimenFiscal.Descripcion);
                    cmd.Parameters.AddWithValue("@PersonaUltimoCambio", regimenFiscal.oUsuario.IdUsuario);

                    conexion.Open();
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
            catch (Exception ex)
            {
                mensaje = "Error al editar el régimen fiscal: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        public bool Eliminar(string CFDIRegimenFiscalId, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"DELETE FROM tCFDIRegimenFiscal WHERE CFDIRegimenFiscalId = @CFDIRegimenFiscalId";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@CFDIRegimenFiscalId", CFDIRegimenFiscalId);

                    conexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        resultado = true;
                    }
                    else
                    {
                        mensaje = "No se encontró el régimen fiscal a eliminar.";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar el régimen fiscal: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        // Buscar Un RegimenFiscal por el nombre 
        public RegimenFiscal BuscarRegimenFiscalPorNombre(string nombre)
        {
            RegimenFiscal regimenFiscal = new RegimenFiscal();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIRegimenFiscalId, Descripcion, Estatus FROM tCFDIRegimenFiscal WHERE Descripcion = @Nombre";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            regimenFiscal = new RegimenFiscal
                            {
                                CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                regimenFiscal = new RegimenFiscal();
            }
            return regimenFiscal;
        }

        // Buscar Un RegimenFiscal por el ID 
        public RegimenFiscal BuscarRegimenFiscalPorId(string CFDIRegimenFiscalId)
        {
            RegimenFiscal regimenFiscal = new RegimenFiscal();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIRegimenFiscalId, Descripcion, Estatus FROM tCFDIRegimenFiscal WHERE CFDIRegimenFiscalId = @CFDIRegimenFiscalId";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CFDIRegimenFiscalId", CFDIRegimenFiscalId);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            regimenFiscal = new RegimenFiscal
                            {
                                CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                regimenFiscal = new RegimenFiscal();
            }
            return regimenFiscal;
        }

        public RegimenFiscal UltimoRegistro()
        {
            RegimenFiscal regimenFiscal = new RegimenFiscal();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT TOP 1 CFDIRegimenFiscalId, Estatus, Descripcion, PersonaUltimoCambio 
                             FROM tCFDIRegimenFiscal 
                             ORDER BY FechaUltimoCambio DESC";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            regimenFiscal = new RegimenFiscal
                            {
                                CFDIRegimenFiscalId = Convert.ToString(dr["CFDIRegimenFiscalId"]),
                                Estatus = Convert.ToString(dr["Estatus"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                regimenFiscal = null; // o manejar de otra manera
            }

            return regimenFiscal;
        }


        // Buscar Regimen Fiscal Por Descripción
        public List<RegimenFiscal> BuscarRegimenFiscalPorDescripcion(string descripcion, int pagina, int cantidadPorPagina)
        {
            List<RegimenFiscal> lista = new List<RegimenFiscal>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                   string query = @"SELECT CFDIRegimenFiscalId, Descripcion FROM tCFDIRegimenFiscal
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
                            lista.Add(new RegimenFiscal
                            {
                                CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"].ToString(),
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

        // Buscar Regimen Fiscal Por ID
        public List<RegimenFiscal> BuscarRegimenFiscalPorID(string id, int pagina, int cantidadPorPagina)
        {
            List<RegimenFiscal> lista = new List<RegimenFiscal>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT CFDIRegimenFiscalId, Descripcion FROM tCFDIRegimenFiscal
                                 WHERE CFDIRegimenFiscalId LIKE @Id
                                 ORDER BY CFDIRegimenFiscalId
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
                            lista.Add(new RegimenFiscal
                            {
                                CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"].ToString(),
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

        // Contar Regímenes Fiscales
        public int ContarRegimenesFiscales(string where)
        {
            int resultado = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(CFDIRegimenFiscalId) AS TotalRegistros FROM tCFDIRegimenFiscal WHERE {where};";

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
        public List<RegimenFiscal> ListarRegimenFiscalTabla(int pagina, string tipoOrden, int siguientes)
        {
            List<RegimenFiscal> lista = new List<RegimenFiscal>();
            try
            {
                string orden = OrdenRegimenFiscal(tipoOrden);
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT CFDIRegimenFiscalId, Descripcion, Estatus FROM tCFDIRegimenFiscal ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new RegimenFiscal
                            {
                                CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString(),
                                // Aquí necesitarías llenar oUsuario si es requerido en tu implementación
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
            }
            return lista;
        }
        public int CountTablaRegimenFiscal()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tCFDIRegimenFiscal;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    resultado = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
            }
            return resultado;
        }
        public List<RegimenFiscal> ListarRegimenFiscalTablaWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<RegimenFiscal> lista = new List<RegimenFiscal>();
            try
            {
                string orden = OrdenRegimenFiscal(tipoOrden);
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT CFDIRegimenFiscalId, Descripcion, Estatus FROM tCFDIRegimenFiscal WHERE {where} ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new RegimenFiscal
                            {
                                CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString(),
                                // Asumiendo que existe una propiedad para Usuario en el objeto RegimenFiscal
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                lista = new List<RegimenFiscal>();
            }
            return lista;
        }
        public int CountTablaRegimenFiscalWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(*) AS TotalRegistros FROM tCFDIRegimenFiscal WHERE {where};";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();
                    resultado = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                resultado = 0;
            }
            return resultado;
        }
        /************BUSCADOR************/
        public List<string> ElementosPaginacionBuscadorRegimenFiscal(string nombre, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tCFDIRegimenFiscal WHERE Descripcion LIKE '%' + @Nombre + '%' ORDER BY Descripcion DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
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

        // COUNT
        public int CountBuscadorRegimenFiscal(string nombre)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tCFDIRegimenFiscal WHERE Descripcion LIKE '%' + @Nombre + '%';";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    conexion.Open();
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

        public string OrdenRegimenFiscal(string tipoOrden)
        {
            switch (tipoOrden)
            {
                case "I_A":
                    return "CFDIRegimenFiscalId ASC";
                case "I_D":
                    return "CFDIRegimenFiscalId DESC";
                case "D_A":
                    return "Descripcion ASC";
                case "D_D":
                    return "Descripcion DESC";
                default:
                    return "CFDIRegimenFiscalId ASC";
            }
        }

    }
}

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
    public class CD_CFDIUso
    {
        public bool Registrar(CFDIUso cfdiUso, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"INSERT INTO tCFDIUso (CFDIUsoId, Estatus, Descripcion, PersonaUltimoCambio, PersonaRegistro, FechaRegistro, FechaUltimoCambio) 
                             VALUES (@CFDIUsoId, @Estatus, @Descripcion, @PersonaUltimoCambio, @PersonaRegistro, GETDATE(), GETDATE());";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@CFDIUsoId", cfdiUso.CFDIUsoId);
                        cmd.Parameters.AddWithValue("@Estatus", cfdiUso.Estatus);
                        cmd.Parameters.AddWithValue("@Descripcion", cfdiUso.Descripcion);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", cfdiUso.oUsuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@PersonaRegistro", cfdiUso.oUsuario.IdUsuario);

                        conexion.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se pudo insertar el registro de uso de CFDI.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al insertar el registro de uso de CFDI: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        public bool Editar(CFDIUso cfdiUso, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"UPDATE tCFDIUso 
                             SET Estatus = @Estatus, 
                                 Descripcion = @Descripcion, 
                                 PersonaUltimoCambio = @PersonaUltimoCambio, 
                                 FechaUltimoCambio = GETDATE() 
                             WHERE CFDIUsoId = @CFDIUsoId;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@CFDIUsoId", cfdiUso.CFDIUsoId);
                        cmd.Parameters.AddWithValue("@Estatus", cfdiUso.Estatus);
                        cmd.Parameters.AddWithValue("@Descripcion", cfdiUso.Descripcion);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", cfdiUso.oUsuario.IdUsuario);

                        conexion.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró el registro de uso de CFDI para actualizar o no se requirieron cambios.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al actualizar el registro de uso de CFDI: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        public bool Eliminar(string cfdiUsoId, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "DELETE FROM tCFDIUso WHERE CFDIUsoId = @CFDIUsoId;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@CFDIUsoId", cfdiUsoId);

                        conexion.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró el registro de uso de CFDI para eliminar.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar el registro de uso de CFDI: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }
        public CFDIUso UltimoRegistro()
        {
            CFDIUso cfdiUso = new CFDIUso();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 CFDIUsoId, Descripcion, Estatus FROM tCFDIUso ORDER BY FechaUltimoCambio DESC";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            cfdiUso = new CFDIUso
                            {
                                CFDIUsoId = Convert.ToString(dr["CFDIUsoId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Estatus = Convert.ToString(dr["Estatus"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                cfdiUso = new CFDIUso();
            }
            return cfdiUso;
        }

        // Buscar un CFDIUso por la descripción
        public CFDIUso BuscarCFDIUsoPorDescripcion(string descripcion)
        {
            CFDIUso cfdiUso = new CFDIUso();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIUsoId, Descripcion, Estatus FROM tCFDIUso WHERE Descripcion = @Descripcion";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cfdiUso = new CFDIUso
                            {
                                CFDIUsoId = dr["CFDIUsoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                cfdiUso = new CFDIUso(); // Asegurar que cfdiUso siempre es un objeto válido.
            }
            return cfdiUso;
        }

        // Buscar un CFDIUso por el ID
        public CFDIUso BuscarPorId(string cfdiUsoId)
        {
            CFDIUso cfdiUso = new CFDIUso();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIUsoId, Descripcion, Estatus FROM tCFDIUso WHERE CFDIUsoId = @CFDIUsoId";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CFDIUsoId", cfdiUsoId);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cfdiUso = new CFDIUso
                            {
                                CFDIUsoId = dr["CFDIUsoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                cfdiUso = new CFDIUso(); // Asegurar que cfdiUso siempre es un objeto válido.
            }
            return cfdiUso;
        }





        //Buscador CFDI Uso Existente para Productos
        public List<CFDIUso> elementosPaginacionBuscadorDescripcionID(string uso, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<CFDIUso> lista = new List<CFDIUso>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIUsoId, Descripcion FROM tCFDIUso WHERE Descripcion LIKE '%" + uso + "%' ORDER BY FechaUltimoCambio DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CFDIUso
                            {
                                CFDIUsoId = Convert.ToString(dr["CFDIUsoId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<CFDIUso>();
            }
            return lista;
        }
        //Buscador CFDI Uso Existente para Productos
        public List<CFDIUso> elementosPaginacionBuscadorIDDescripcion(string uso, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<CFDIUso> lista = new List<CFDIUso>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT CFDIUsoId, Descripcion FROM tCFDIUso WHERE CFDIUsoId LIKE '%" + uso + "%' ORDER BY FechaUltimoCambio DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CFDIUso
                            {
                                CFDIUsoId = Convert.ToString(dr["CFDIUsoId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<CFDIUso>();
            }
            return lista;
        }

        // COUNT en la tabla tCFDIUso con una condición WHERE específica
        public int countCFDIUsoWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(Estatus) AS TotalRegistros FROM tCFDIUso WHERE {where};";

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

        /************BUSCADOR************/
        // Obtener elementos de paginación para el buscador de CFDIUso
        public List<string> ElementosPaginacionBuscadorCFDIUso(string nombre, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tCFDIUso WHERE Descripcion LIKE @Nombre ORDER BY Descripcion OFFSET @Pagina ROWS FETCH NEXT @Siguientes ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", "%" + nombre + "%");
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

        // Contar el número de registros de CFDIUso que coinciden con el término de búsqueda
        public int CountBuscadorCFDIUso(string nombre)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tCFDIUso WHERE Descripcion LIKE '%' + @Nombre + '%';";
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
        /************TABLA************/
        // Listar registros de CFDIUso con paginación y orden
        public List<CFDIUso> ListarCFDIUsoTabla(int pagina, string tipoOrden, int siguientes)
        {
            List<CFDIUso> lista = new List<CFDIUso>();
            try
            {
                string orden = OrdenCFDIUso(tipoOrden);
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT u.CFDIUsoId, u.Descripcion, u.Estatus FROM tCFDIUso u ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CFDIUso
                            {
                                CFDIUsoId = dr["CFDIUsoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<CFDIUso>();
            }
            return lista;
        }

        // Contar la cantidad de registros en la tabla CFDIUso
        public int CountTablaCFDIUso()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tCFDIUso;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
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
        /************TABLA CON WHERE************/
        // Listar registros de CFDIUso con condiciones, paginación y orden
        public List<CFDIUso> ListarCFDIUsoTablaWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<CFDIUso> lista = new List<CFDIUso>();
            try
            {
                string orden = OrdenCFDIUso(tipoOrden);
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT u.CFDIUsoId, u.Descripcion, u.Estatus FROM tCFDIUso u WHERE {where} ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new CFDIUso
                            {
                                CFDIUsoId = dr["CFDIUsoId"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Estatus = dr["Estatus"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<CFDIUso>();
            }
            return lista;
        }

        // Contar la cantidad de registros en la tabla CFDIUso que cumplen con la condición
        public int CountTablaCFDIUsoWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(*) AS TotalRegistros FROM tCFDIUso u WHERE {where};";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
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
        public string OrdenCFDIUso(string tipoOrden)
        {
            switch (tipoOrden)
            {
                case "I_A":
                    return "u.CFDIUsoId";
                case "I_D":
                    return "u.CFDIUsoId DESC";
                case "D_A":
                    return "u.Descripcion";
                case "D_D":
                    return "u.Descripcion DESC";
                default:
                    return "u.CFDIUsoId";
            }
        }


    }
}

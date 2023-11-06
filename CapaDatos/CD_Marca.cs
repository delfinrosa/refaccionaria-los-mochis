using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
namespace CapaDatos
{
    public class CD_Marca
    {
        public List<Marca> Listar()
        {

            List<Marca> lista = new List<Marca>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdMarca,Descripcion,Activo FROM tMarcas";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Marca
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Marca>();
            }
            return lista;
        }
        public int Registrar(Marca obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarMarca", oconexion);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    idautogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;
        }

        public bool Editar(Marca obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_EditarMarca", oconexion);
                    cmd.Parameters.AddWithValue("IdMarca", obj.IdMarca);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }
        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarMarca", oconexion);
                    cmd.Parameters.AddWithValue("IdMarca", id);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

        ///////////
        //Pruebas
        ///////////
        public Marca BuscarPorNombre(string nombre)
        {
            Marca marca = new Marca();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdMarca, Descripcion, Activo FROM tMarcas WHERE Descripcion = @Nombre";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            marca = new Marca
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                marca = new Marca();
            }
            return marca;
        }

        public List<string> ListarNombresDeMarcas(string nombre)
        {
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tMarcas WHERE Descripcion LIKE '%' + @Nombre + '%';";

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

        public Marca BuscarPorId(int id)
        {
            Marca marca = new Marca();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdMarca, Descripcion, Activo FROM tMarcas WHERE IdMarca = @Id";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            marca = new Marca
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                marca = new Marca();
            }
            return marca;
        }

        public Marca UltimoRegistro()
        {
            Marca marca = new Marca();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 IdMarca, Descripcion, Activo FROM tMarcas ORDER BY FechaUltimoCambio DESC";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            marca = new Marca
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                marca = new Marca();
            }
            return marca;
        }

        public List<Marca> PruebasAutoCompletado()
        {
            List<Marca> lista = new List<Marca>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdMarca, Descripcion, Activo FROM tMarcas ORDER BY FechaUltimoCambio DESC";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Marca
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Marca>();
            }
            return lista;
        }

        public int ContarPruebasAutoCompletado(string nombre)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tMarcas WHERE Descripcion LIKE '%' + @Nombre + '%';";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
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

        public List<string> PaginacionPruebasAutoCompletado(string nombre, int pagina)
        {
            int offset = pagina * 5;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tMarcas WHERE Descripcion LIKE '%' + @Nombre + '%' ORDER BY FechaUltimoCambio DESC OFFSET @Offset ROWS FETCH NEXT 5 ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Offset", offset);
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

        ///////////
        //Pruebas FIn
        ///////////



        ///////////////
        ///tabla paginado 
        ///////////////

        public List<Marca> ListarMarcas(int pagina)
        {
            List<Marca> lista = new List<Marca>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT m.IdMarca, m.Descripcion, m.Activo, m.FechaCreacion, m.PersonaUltimoCambio, m.FechaUltimoCambio FROM tMarcas m ORDER BY m.IdMarca OFFSET " + pagina * 10 + " ROWS FETCH NEXT 10 ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Marca
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                fechaCreaccion = Convert.ToString(dr["FechaCreacion"]),
                                IdUsuario = Convert.ToInt32(dr["PersonaUltimoCambio"]),
                                fechaActualizacion = Convert.ToString(dr["FechaUltimoCambio"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Marca>();
            }
            return lista;
        }

        public int ContarMarcas()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tMarcas;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
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




    }
}

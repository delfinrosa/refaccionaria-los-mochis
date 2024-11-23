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
    public class CD_Almacen
    {
        public int Registrar(Almacen almacen, out string mensaje)
        {
            mensaje = string.Empty;
            int idAutogenerado = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "INSERT INTO tAlmacenes (Descripcion, Ubicacion, PersonaRegistro, PersonaUltimoCambio) VALUES (@Descripcion, @Ubicacion, @PersonaRegistro, @PersonaUltimoCambio); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Descripcion", almacen.Descripcion);
                        cmd.Parameters.AddWithValue("@Ubicacion", almacen.Ubicacion);
                        cmd.Parameters.AddWithValue("@PersonaRegistro", almacen.oUsuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", almacen.oUsuario.IdUsuario);

                        conexion.Open();

                        idAutogenerado = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al registrar el almacén: " + ex.Message;
            }

            return idAutogenerado;
        }
        public bool Editar(Almacen almacen, out string mensaje)
        {
            mensaje = string.Empty;
            bool actualizacionExitosa = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "UPDATE tAlmacenes SET Descripcion = @Descripcion, Ubicacion = @Ubicacion, PersonaUltimoCambio = @PersonaUltimoCambio, FechaUltimoCambio = GETDATE() WHERE AlmacenId = @AlmacenId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Descripcion", almacen.Descripcion);
                        cmd.Parameters.AddWithValue("@Ubicacion", almacen.Ubicacion);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", almacen.oUsuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@AlmacenId", almacen.AlmacenId);

                        conexion.Open();

                        int filasActualizadas = cmd.ExecuteNonQuery();
                        if (filasActualizadas > 0)
                        {
                            actualizacionExitosa = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al editar el almacén: " + ex.Message;
            }

            return actualizacionExitosa;
        }


        public bool EliminarAlmacen(int almacenId, out string mensaje)
        {
            mensaje = string.Empty;
            bool eliminado = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "DELETE FROM tAlmacenes WHERE AlmacenId = @AlmacenId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@AlmacenId", almacenId);

                        conexion.Open();

                        int filasEliminadas = cmd.ExecuteNonQuery();

                        if (filasEliminadas > 0)
                        {
                            eliminado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró ningún almacén con el ID proporcionado.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar el almacén: " + ex.Message;
            }

            return eliminado;
        }

        //Ultimo Registro 
        public Almacen UltimoRegistro()
        {
            Almacen ultimoAlmacen = new Almacen();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 AlmacenId, Descripcion, Ubicacion FROM tAlmacenes ORDER BY FechaUltimoCambio DESC;";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            ultimoAlmacen = new Almacen
                            {
                                AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return ultimoAlmacen;
        }


        public List<Almacen> ObtenerAlmacenes(out string mensaje)
        {
            mensaje = string.Empty;
            List<Almacen> almacenes = new List<Almacen>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM tAlmacenes";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        conexion.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Almacen almacen = new Almacen();
                                almacen.AlmacenId = Convert.ToInt32(reader["AlmacenId"]);
                                almacen.Ubicacion = reader["Ubicacion"].ToString();
                                almacenes.Add(almacen);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener los almacenes: " + ex.Message;
            }

            return almacenes;
        }


        public List<Almacen> ListarAlmacen(int pagina, string tipoOrden, int siguientes)
        {
            List<Almacen> lista = new List<Almacen>();
            try
            {
                string orden = Orden(tipoOrden); // Asegúrate de implementar la función Orden() adecuadamente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT AlmacenId, Descripcion, Ubicacion FROM tAlmacenes ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Almacen
                            {
                                AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Almacen>();
            }
            return lista;
        }

        public int CountTablaAlmacen()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tAlmacenes;";
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


        public string Orden(string tipoOrden)
        {
            switch (tipoOrden)
            {
                case "I_A":
                    return "AlmacenId ";
                case "I_D":
                    return "AlmacenId DESC";
                case "D_A":
                    return "Descripcion ";
                case "D_D":
                    return "Descripcion DESC";
                default:
                    return "AlmacenId ";
            }
        }

        public List<string> ElementosPaginacionBuscadorAlmacen(string nombre, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tAlmacenes WHERE Descripcion LIKE '%' + @Nombre + '%' ORDER BY Descripcion DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
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

        public async Task<int> CountBuscadorAlmacen(string nombre)
        {
            try
            {
                using (var conexion = new SqlConnection(Conexion.cn))
                {
                    await conexion.OpenAsync();
                    var query = "SELECT COUNT(AlmacenId) FROM tAlmacenes WHERE Descripcion LIKE '%' + @Nombre + '%'";
                    using (var cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        var result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public Almacen BuscarAlmacenPorDescripcion(string descripcion)
        {
            Almacen almacen = new Almacen();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT AlmacenId, Descripcion, Ubicacion FROM tAlmacenes WHERE Descripcion = @Descripcion";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) // Usamos if en lugar de while si solo esperamos un resultado
                        {
                            almacen = new Almacen
                            {
                                AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                almacen = new Almacen(); // Retorna un almacen vacío si hay un error
            }
            return almacen;
        }
        public Almacen BuscarAlmacenPorUbicacion(string Ubicacion)
        {
            Almacen almacen = new Almacen();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT AlmacenId, Descripcion, Ubicacion FROM tAlmacenes WHERE Ubicacion = @Ubicacion";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Ubicacion", Ubicacion);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) // Usamos if en lugar de while si solo esperamos un resultado
                        {
                            almacen = new Almacen
                            {
                                AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                almacen = new Almacen(); // Retorna un almacen vacío si hay un error
            }
            return almacen;
        }


        public Almacen BuscarAlmacenPorId(int id)
        {
            Almacen almacen = new Almacen();  
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT AlmacenId, Descripcion, Ubicacion FROM tAlmacenes WHERE AlmacenId = @Id";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            almacen = new Almacen
                            {
                                AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                almacen = new Almacen();  
            }
            return almacen;
        }


    }
}

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
    public class CD_AlmacenRack
    {
        public bool RegistrarRack(AlmacenRack rack, out string mensaje)
        {
            mensaje = string.Empty;
            bool exito = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "INSERT INTO tAlmacenRacks (AlmacenId, RackId, Descripcion, Ubicacion, PersonaRegistro, PersonaUltimoCambio) " +
                                   "VALUES (@AlmacenId, @RackId, @Descripcion, @Ubicacion, @PersonaRegistro, @PersonaUltimoCambio); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@AlmacenId", rack.oAlmacen.AlmacenId);
                        cmd.Parameters.AddWithValue("@RackId", rack.RackId);
                        cmd.Parameters.AddWithValue("@Descripcion", rack.Descripcion);
                        cmd.Parameters.AddWithValue("@Ubicacion", rack.Ubicacion);
                        cmd.Parameters.AddWithValue("@PersonaRegistro", rack.oUsuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", rack.oUsuario.IdUsuario);

                        conexion.Open();

                        var resultado = cmd.ExecuteScalar();
                        if (resultado != null)
                        {
                            exito = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al registrar el rack en el almacén: " + ex.Message;
                exito = false;
            }

            return exito;
        }


        public int EditarRack(AlmacenRack rack, out string mensaje)
        {
            mensaje = string.Empty;
            int filasActualizadas = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "UPDATE tAlmacenRacks SET Descripcion = @Descripcion, Ubicacion = @Ubicacion " +
                                   "WHERE AlmacenId = @AlmacenId AND RackId = @RackId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@AlmacenId", rack.oAlmacen.AlmacenId);
                        cmd.Parameters.AddWithValue("@RackId", rack.RackId);
                        cmd.Parameters.AddWithValue("@Descripcion", rack.Descripcion);
                        cmd.Parameters.AddWithValue("@Ubicacion", rack.Ubicacion);

                        conexion.Open();

                        filasActualizadas = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al editar el rack en el almacén: " + ex.Message;
            }

            return filasActualizadas;
        }

        public bool EliminarRack(int almacenId, int rackId, out string mensaje)
        {
            mensaje = string.Empty;
            bool eliminado = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "DELETE FROM tAlmacenRacks WHERE AlmacenId = @AlmacenId AND RackId = @RackId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@AlmacenId", almacenId);
                        cmd.Parameters.AddWithValue("@RackId", rackId);

                        conexion.Open();

                        int filasEliminadas = cmd.ExecuteNonQuery();

                        if (filasEliminadas > 0)
                        {
                            eliminado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró ningún rack con el ID proporcionado en el almacén.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar el rack en el almacén: " + ex.Message;
            }

            return eliminado;
        }

        public List<AlmacenRack> ObtenerAlmacenRack(out string mensaje, int almacenId)
        {
            mensaje = "";
            var racks = new List<AlmacenRack>();

            try
            {
                using (var conexion = new SqlConnection(Conexion.cn))
                {
                    var query = "SELECT RackId, Ubicacion FROM tAlmacenRacks WHERE AlmacenId = @AlmacenId";

                    using (var cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@AlmacenId", almacenId);
                        conexion.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                racks.Add(new AlmacenRack
                                {
                                    RackId = Convert.ToInt32(reader["RackId"]),
                                    Ubicacion = reader["Ubicacion"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener los racks: " + ex.Message;
            }

            return racks;
        }
        public AlmacenRack ObtenerRackUbicacion(out string mensaje, string Ubicacion)
        {
            mensaje = "";
            AlmacenRack racks= new AlmacenRack();

            try
            {
                using (var conexion = new SqlConnection(Conexion.cn))
                {
                    var query = "SELECT RackId,Descripcion, Ubicacion FROM tAlmacenRacks WHERE Ubicacion = @Ubicacion";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Ubicacion", Ubicacion);
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) // Usamos if en lugar de while si solo esperamos un resultado
                        {
                            racks = new AlmacenRack
                            {
                                RackId = Convert.ToInt32(dr["RackId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener los racks: " + ex.Message;
            }

            return racks;
        }

        public AlmacenRack UltimoRegistroRack(int almacenId)
        {
            AlmacenRack ultimoRack = new AlmacenRack();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 RackId, Descripcion, Ubicacion FROM tAlmacenRacks WHERE AlmacenId = @AlmacenId ORDER BY FechaUltimoCambio DESC;";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@AlmacenId", almacenId);
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            ultimoRack = new AlmacenRack
                            {
                                RackId = Convert.ToInt32(dr["RackId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"]),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
            }

            return ultimoRack;
        }




        public List<AlmacenRack> ListarRacks(int pagina, int siguientes)
        {
            List<AlmacenRack> lista = new List<AlmacenRack>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT RackId, Descripcion, Ubicacion FROM tAlmacenRacks OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new AlmacenRack
                            {
                                RackId = Convert.ToInt32(dr["RackId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<AlmacenRack>();
            }
            return lista;
        }

        public int CountTablaRack()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(RackId) AS TotalRegistros FROM tAlmacenRacks;";
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



        public List<AlmacenRack> ObtenerRacksPorAlmacen(int almacenId)
        {
            List<AlmacenRack> lista = new List<AlmacenRack>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
        SELECT RackId, Descripcion, Ubicacion
        FROM tAlmacenRacks
        WHERE AlmacenId = @AlmacenId;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@AlmacenId", almacenId);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new AlmacenRack
                            {
                                RackId = Convert.ToInt32(dr["RackId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
            }
            return lista;
        }




    }

}

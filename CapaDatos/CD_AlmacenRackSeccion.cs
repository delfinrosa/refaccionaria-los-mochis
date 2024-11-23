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
    public class CD_AlmacenRackSeccion
    {
        public int RegistrarSeccionRack(AlmacenRackSeccion seccion, out string mensaje)
        {
            mensaje = string.Empty;
            int idAutogenerado = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"INSERT INTO tAlmacenesRacksSecciones (AlmacenId, RackId, SeccionId, Descripcion, Ubicacion, PersonaRegistro, PersonaUltimoCambio) 
                             VALUES (@AlmacenId, @RackId, @SeccionId, @Descripcion, @Ubicacion, @UsuarioId, @UsuarioId); "; 

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@AlmacenId", seccion.oAlmacen.AlmacenId);
                        cmd.Parameters.AddWithValue("@RackId", seccion.oRack.RackId);
                        cmd.Parameters.AddWithValue("@SeccionId", seccion.SeccionId);
                        cmd.Parameters.AddWithValue("@Descripcion", seccion.Descripcion);
                        cmd.Parameters.AddWithValue("@Ubicacion", seccion.Ubicacion);
                        cmd.Parameters.AddWithValue("@UsuarioId", seccion.oUsuario.IdUsuario);

                        conexion.Open();

                        idAutogenerado = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al registrar la sección en el rack del almacén: " + ex.Message;
            }

            return idAutogenerado;
        }

        public int EditarSeccionEnRack(AlmacenRackSeccion seccion, out string mensaje)
        {
            mensaje = string.Empty;
            int filasActualizadas = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "UPDATE tAlmacenesRacksSecciones SET Descripcion = @Descripcion, Ubicacion = @Ubicacion " +
                                   "WHERE AlmacenId = @AlmacenId AND RackId = @RackId AND SeccionId = @SeccionId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@AlmacenId", seccion.oAlmacen.AlmacenId);
                        cmd.Parameters.AddWithValue("@RackId", seccion.oRack.RackId);
                        cmd.Parameters.AddWithValue("@SeccionId", seccion.SeccionId);
                        cmd.Parameters.AddWithValue("@Descripcion", seccion.Descripcion);
                        cmd.Parameters.AddWithValue("@Ubicacion", seccion.Ubicacion);

                        conexion.Open();

                        filasActualizadas = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al editar la sección en el rack del almacén: " + ex.Message;
            }

            return filasActualizadas;
        }

        public bool EliminarSeccion(AlmacenRackSeccion seccion, out string mensaje)
        {
            mensaje = string.Empty;
            bool eliminado = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "DELETE FROM tAlmacenesRacksSecciones WHERE AlmacenId = @AlmacenId AND RackId = @RackId AND SeccionId = @SeccionId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@AlmacenId", seccion.oAlmacen.AlmacenId);
                        cmd.Parameters.AddWithValue("@RackId", seccion.oRack.RackId);
                        cmd.Parameters.AddWithValue("@SeccionId", seccion.SeccionId);

                        conexion.Open();

                        int filasEliminadas = cmd.ExecuteNonQuery();

                        if (filasEliminadas > 0)
                        {
                            eliminado = true;
                        }
                        else
                        {
                            mensaje = "No se encontró ninguna sección con el ID proporcionado en el rack del almacén.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar la sección en el rack del almacén: " + ex.Message;
            }

            return eliminado;
        }

        public List<AlmacenRackSeccion> ObtenerSeccionesPorAlmacenYRack(int almacenId, int rackId)
        {
            List<AlmacenRackSeccion> secciones = new List<AlmacenRackSeccion>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT ARS.SeccionId, ARS.Descripcion, ARS.Ubicacion, 
                   A.Ubicacion AS UbicacionAlmacen, A.AlmacenId, 
                   AR.Ubicacion AS UbicacionRack, AR.RackId
            FROM tAlmacenesRacksSecciones ARS
            JOIN tAlmacenRacks AR ON ARS.RackId = AR.RackId
            JOIN tAlmacenes A ON AR.AlmacenId = A.AlmacenId
            WHERE A.AlmacenId = @AlmacenId AND AR.RackId = @RackId
            ORDER BY A.AlmacenId, AR.RackId, ARS.SeccionId;
            ";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@AlmacenId", almacenId);
                    cmd.Parameters.AddWithValue("@RackId", rackId);
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            secciones.Add(new AlmacenRackSeccion
                            {
                                oAlmacen = new Almacen { Ubicacion = Convert.ToString(dr["UbicacionAlmacen"]), AlmacenId = Convert.ToInt32(dr["AlmacenId"]) },
                                oRack = new AlmacenRack { Ubicacion = Convert.ToString(dr["UbicacionRack"]), RackId = Convert.ToInt32(dr["RackId"]) },
                                SeccionId = Convert.ToInt32(dr["SeccionId"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Ubicacion = Convert.ToString(dr["Ubicacion"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return secciones;
        }
        public AlmacenRackSeccion ObtenerSeccionUbicacion(out string mensaje, string Ubicacion)
        {
            mensaje = "";
            AlmacenRackSeccion racks = new AlmacenRackSeccion();

            try
            {
                using (var conexion = new SqlConnection(Conexion.cn))
                {
                    var query = "SELECT SeccionId,Descripcion, Ubicacion FROM tAlmacenesRacksSecciones WHERE Ubicacion = @Ubicacion";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Ubicacion", Ubicacion);
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) // Usamos if en lugar de while si solo esperamos un resultado
                        {
                            racks = new AlmacenRackSeccion
                            {
                                SeccionId = Convert.ToInt32(dr["SeccionId"]),
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

        public List<AlmacenRackSeccion> ObtenerRackSeccion(out string mensaje, int rackId)
        {
            mensaje = "";
            List<AlmacenRackSeccion> secciones = new List<AlmacenRackSeccion>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT SeccionId, Ubicacion FROM tAlmacenesRacksSecciones WHERE RackId = @RackId";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@RackId", rackId);
                        conexion.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                secciones.Add(new AlmacenRackSeccion
                                {
                                    SeccionId = Convert.ToInt32(reader["SeccionId"]),
                                    Ubicacion = reader["Ubicacion"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener las secciones: " + ex.Message;
            }

            return secciones;
        }


    }

}

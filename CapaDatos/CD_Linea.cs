using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Linea
    {
        //GUARDAR
        public int Registrar(Linea obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Verificar si la línea ya existe
                    string queryVerificar = @"
            SELECT COUNT(*) FROM tLineas WHERE Descripcion = @Descripcion";

                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, oconexion))
                    {
                        cmdVerificar.Parameters.AddWithValue("@Descripcion", obj.Descripcion);

                        int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                        if (existe > 0)
                        {
                            Mensaje = "La línea con esa descripción ya existe.";
                            return 0; // Retorna 0 para indicar que no se creó una nueva línea
                        }
                    }

                    // Inserción de la línea si no existe
                    string queryLinea = @"
            INSERT INTO tLineas (Descripcion, Activo, PersonaUltimoCambio, FechaCreacion, FechaUltimoCambio)
            VALUES (@Descripcion, @Activo, @PersonaUltimoCambio, GETDATE(), GETDATE());
            SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmdLinea = new SqlCommand(queryLinea, oconexion))
                    {
                        cmdLinea.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                        cmdLinea.Parameters.AddWithValue("@Activo", obj.Activo);
                        cmdLinea.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);

                        idautogenerado = Convert.ToInt32(cmdLinea.ExecuteScalar()); // ID de la nueva línea insertada
                    }

                    // Inserción de la característica de la línea
                    if (!string.IsNullOrEmpty(obj.Deslc))
                    {
                        string queryCaracteristica = @"
                INSERT INTO tLineasCaracteristicas (Descripcion, IdLinea)
                VALUES (@Descripcion, @IdLinea);";

                        using (SqlCommand cmdCaracteristica = new SqlCommand(queryCaracteristica, oconexion))
                        {
                            cmdCaracteristica.Parameters.AddWithValue("@Descripcion", obj.Deslc);
                            cmdCaracteristica.Parameters.AddWithValue("@IdLinea", idautogenerado);

                            cmdCaracteristica.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }

            return idautogenerado;
        }




   

              //EDITAR
        public bool Editar(Linea obj, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    using (SqlTransaction transaccion = conexion.BeginTransaction())
                    {
                        string queryVerificacion = "SELECT COUNT(IdLinea) FROM tLineas " +
                                                   "WHERE Descripcion = @Descripcion ";
                        using (SqlCommand cmdVerificacion = new SqlCommand(queryVerificacion, conexion, transaccion))
                        {
                            cmdVerificacion.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                            cmdVerificacion.Parameters.AddWithValue("@IdLinea", obj.IdLinea);

                            int existe = Convert.ToInt32(cmdVerificacion.ExecuteScalar());
                            if (existe > 1)
                            {
                                mensaje = "Ya existe otra línea con el mismo nombre.";
                                transaccion.Rollback();
                                return false; // Sale del método sin realizar la actualización
                            }
                        }

                        string queryLineas = "UPDATE tLineas " +
                                             "SET Descripcion = @Descripcion, " +
                                             "Activo = @Activo, " +
                                             "PersonaUltimoCambio = @PersonaUltimoCambio, " +
                                             "FechaUltimoCambio = GETDATE() " +
                                             "WHERE IdLinea = @IdLinea";

                        using (SqlCommand cmdLineas = new SqlCommand(queryLineas, conexion, transaccion))
                        {
                            cmdLineas.Parameters.AddWithValue("@IdLinea", obj.IdLinea);
                            cmdLineas.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                            cmdLineas.Parameters.AddWithValue("@Activo", obj.Activo);
                            cmdLineas.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);

                            int filasActualizadasLineas = cmdLineas.ExecuteNonQuery();
                            if (filasActualizadasLineas == 0)
                            {
                                mensaje = "No se encontró la línea especificada o no se requirieron cambios.";
                                transaccion.Rollback();
                                return false;
                            }
                        }

                        // Actualizar la característica asociada
                        string queryCaracteristicas = @"
                    UPDATE tLineasCaracteristicas
                    SET Descripcion = @DescripcionCaracteristica
                    WHERE IdLinea = @IdLinea";

                        using (SqlCommand cmdCaracteristicas = new SqlCommand(queryCaracteristicas, conexion, transaccion))
                        {
                            cmdCaracteristicas.Parameters.AddWithValue("@IdLinea", obj.IdLinea);
                            cmdCaracteristicas.Parameters.AddWithValue("@DescripcionCaracteristica", obj.Deslc);

                            cmdCaracteristicas.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        resultado = true;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al editar la línea y su característica: " + ex.Message;
                resultado = false;
            }

            return resultado;
        }




        //ELIMINAR
        public bool Eliminar(int IdLinea, out string Mensaje)
        {
            bool Resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Verificar si la línea está relacionada con algún producto
                    string queryVerificacion = @"
            SELECT COUNT(*)
            FROM tProductos p
            INNER JOIN tLineas l ON l.IdLinea = p.IdLinea
            WHERE l.IdLinea = @IdLinea";

                    using (SqlCommand cmdVerificacion = new SqlCommand(queryVerificacion, oconexion))
                    {
                        cmdVerificacion.Parameters.AddWithValue("@IdLinea", IdLinea);
                        int relacionado = (int)cmdVerificacion.ExecuteScalar();

                        if (relacionado == 0)
                        {
                            // Si no está relacionada, eliminar características y la línea
                            string queryEliminarCaracteristicas = "DELETE TOP (1) FROM tLineasCaracteristicas WHERE IdLinea = @IdLinea";
                            SqlCommand cmdEliminarCaracteristicas = new SqlCommand(queryEliminarCaracteristicas, oconexion);
                            cmdEliminarCaracteristicas.Parameters.AddWithValue("@IdLinea", IdLinea);
                            cmdEliminarCaracteristicas.ExecuteNonQuery();

                            string queryEliminarLinea = "DELETE TOP (1) FROM tLineas WHERE IdLinea = @IdLinea";
                            SqlCommand cmdEliminarLinea = new SqlCommand(queryEliminarLinea, oconexion);
                            cmdEliminarLinea.Parameters.AddWithValue("@IdLinea", IdLinea);
                            cmdEliminarLinea.ExecuteNonQuery();

                            Resultado = true;
                        }
                        else
                        {
                            // Si está relacionada, establecer mensaje de error
                            Mensaje = "La linea se encuentra relacionada a un producto";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }

            return Resultado;
        }

        // Buscar Una Linea por el nombre 
        public Linea bucarLineaPorNombre(string nombre)
        {
            Linea lista = new Linea();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "select l.IdLinea,l.Descripcion , l.Activo, lc.Descripcion as Deslc  from tLineas l inner join tLineasCaracteristicas lc on lc.IdLinea = l.IdLinea where l.Descripcion ='" + nombre + "'";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista = new Linea
                            {
                                IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Deslc = Convert.ToString(dr["Deslc"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new Linea();
            }
            return lista;
        }
        // Buscar Una Linea por el ID 
        public Linea BusquedaIDLinea(int Id)
        {
            Linea lista = new Linea();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "select l.IdLinea,l.Descripcion , l.Activo, lc.Descripcion as Deslc  from tLineas l inner join tLineasCaracteristicas lc on lc.IdLinea = l.IdLinea where l.IdLinea ='" + Id + "'";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista = new Linea
                            {
                                IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Deslc = Convert.ToString(dr["Deslc"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new Linea();
            }
            return lista;
        }
        //Ultimo Registro
        public Linea UltimoRegistro()
        {
            Linea lista = new Linea();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 l.IdLinea, l.Descripcion, l.Activo, lc.Descripcion AS Deslc FROM tLineas l INNER JOIN tLineasCaracteristicas lc ON lc.IdLinea = l.IdLinea ORDER BY l.FechaUltimoCambio DESC;";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista = new Linea
                            {
                                IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Deslc = Convert.ToString(dr["Deslc"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new Linea();
            }
            return lista;
        }
        /************BUSCADOR************/
        public List<string> elementosPaginacionBuscador(string linea, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tLineas WHERE Descripcion LIKE '%" + linea + "%' ORDER BY Descripcion DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
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
        public int countBuscador(string linea)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tLineas WHERE Descripcion LIKE '%" + linea + "%' ;";

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
        /************TABLA************/
        public List<Linea> ListarLinea(int pagina, string tipoOrden, int siguientes)
        {
            List<Linea> lista = new List<Linea>();
            try
            {
                string orden = Orden(tipoOrden);
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT l.IdLinea, l.Descripcion, l.Activo, lc.Descripcion AS Deslc, l.FechaCreacion, l.PersonaUltimoCambio, l.FechaUltimoCambio FROM tLineas l INNER JOIN tLineasCaracteristicas lc ON lc.IdLinea = l.IdLinea ORDER BY " + orden + " OFFSET " + pagina * siguientes + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY;";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Linea
                            {
                                IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Deslc = Convert.ToString(dr["Deslc"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Linea>();
            }
            return lista;
        }
        //      COUNT Tabla
        public int countTabla()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tLineas l INNER JOIN tLineasCaracteristicas lc ON lc.IdLinea = l.IdLinea;";
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
        /************TABLA CON WHERE************/
        public List<Linea> ListarLineaWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<Linea> lista = new List<Linea>();
            try
            {
                string orden = Orden(tipoOrden);
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT l.IdLinea, l.Descripcion, l.Activo, lc.Descripcion AS Deslc, l.FechaCreacion, l.PersonaUltimoCambio, l.FechaUltimoCambio FROM tLineas l INNER JOIN tLineasCaracteristicas lc ON lc.IdLinea = l.IdLinea WHERE " + where + " ORDER BY " + orden + " OFFSET " + pagina * siguientes + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Linea
                            {
                                IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Deslc = Convert.ToString(dr["Deslc"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Linea>();
            }
            return lista;
        }
        //      COUNT Tabla WHERE

        public int countTablaWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tLineas l INNER JOIN tLineasCaracteristicas lc ON lc.IdLinea = l.IdLinea WHERE " + where + " ;";

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
        public string Orden(string tipoOrden)
        {
            switch (tipoOrden)
            {
                case "I_A":
                    return "l.IdLinea ";
                case "I_D":
                    return "l.IdLinea DESC";
                case "D_A":
                    return "l.Descripcion ";
                case "D_D":
                    return "l.Descripcion DESC";
                default:
                    return "l.IdLinea ";
            }
        }
        //Buscador Linea Existente para Productos

        public List<Linea> elementosPaginacionBuscadorDescripcionID(string linea, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<Linea> lista = new List<Linea>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdLinea , Descripcion FROM tLineas WHERE Descripcion LIKE '%" + linea + "%' ORDER BY FechaUltimoCambio DESC OFFSET " + pagina*siguientes + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Linea
                            {
                                IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                Descripcion = Convert.ToString(dr["Descripcion"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Linea>();
            }
            return lista;
        }

        //Quizas lo mueva a recursos 
        public bool ChecarConexion()
        {
            bool resultado = false;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("ChecarConexion", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            resultado = Convert.ToBoolean(dr["respuesta"]);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
            }
            return resultado;
        }

    }
}

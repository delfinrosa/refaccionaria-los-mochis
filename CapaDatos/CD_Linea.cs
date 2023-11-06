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
        public List<Linea> Listar()
        {

            List<Linea> lista = new List<Linea>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "select l.IdLinea,l.Descripcion , l.Activo, lc.Descripcion as Deslc  ,l.FechaCreacion ,l.PersonaUltimoCambio ,l.FechaUltimoCambio from tLineas l inner join tLineasCaracteristicas lc on lc.IdLinea = l.IdLinea";

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
                                Deslc = Convert.ToString(dr["Deslc"]),
                                fechaCreaccion = Convert.ToString(dr["FechaCreacion"]),
                                fechaActualizacion = Convert.ToString(dr["FechaUltimoCambio"]),
                                IdUsuario = Convert.ToInt32(dr["PersonaUltimoCambio"])
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
        ////////////////
        /////PRUEBA PAGINADO TABLA
        /////////////////
        public List<Linea> ListarPrueba(int pagina,string tipoOrden)
        {
            string orden="";
            switch (tipoOrden)
            {
                case "I_A":
                    orden = "l.IdLinea ";
                    break;
                case "I_D":
                    orden = "l.IdLinea DESC";
                    break;
                case "D_A":
                    orden = "l.Descripcion ";
                    break;
                case "D_D":
                    orden = "l.Descripcion DESC";
                    break;
                default:
                    orden = "l.IdLinea ";
                    break;
            }
            List<Linea> lista = new List<Linea>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT l.IdLinea, l.Descripcion, l.Activo, lc.Descripcion AS Deslc, l.FechaCreacion, l.PersonaUltimoCambio, l.FechaUltimoCambio FROM tLineas l INNER JOIN tLineasCaracteristicas lc ON lc.IdLinea = l.IdLinea ORDER BY "+orden+" OFFSET "+pagina*10+" ROWS FETCH NEXT 10 ROWS ONLY;";

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
                                Deslc = Convert.ToString(dr["Deslc"]),
                                fechaCreaccion = Convert.ToString(dr["FechaCreacion"]),
                                fechaActualizacion = Convert.ToString(dr["FechaUltimoCambio"]),
                                IdUsuario = Convert.ToInt32(dr["PersonaUltimoCambio"])
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

        //COUNT
        public int COUNT_Tabla()
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


        ////////////////
        /////PRUEBA PAGINADO TABLA     FIN
        /////////////////


        public int Registrar(Linea obj, out string Mensaje,out Linea objDevolucion)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;
            objDevolucion = obj;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarLinea", oconexion);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Deslc", obj.Deslc);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ResFechaCreacion", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ResFechaUltimoCambio", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    idautogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                    objDevolucion.fechaCreaccion = cmd.Parameters["ResFechaCreacion"].Value.ToString();
                    objDevolucion.fechaActualizacion = cmd.Parameters["ResFechaUltimoCambio"].Value.ToString();

                }
                objDevolucion.IdLinea = idautogenerado;
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;
        }

        public bool Editar(Linea obj, out string Mensaje, out Linea objDevolucion)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            objDevolucion = obj;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_EditarLinea", oconexion);
                    cmd.Parameters.AddWithValue("IdLinea", obj.IdLinea);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Deslc", obj.Deslc);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ResFechaCreacion", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ResFechaUltimoCambio", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                    objDevolucion.fechaCreaccion = cmd.Parameters["ResFechaCreacion"].Value.ToString();
                    objDevolucion.fechaActualizacion = cmd.Parameters["ResFechaUltimoCambio"].Value.ToString();
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
                    SqlCommand cmd = new SqlCommand("sp_EliminarLinea", oconexion);
                    cmd.Parameters.AddWithValue("IdLinea", id);
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

        
        public Linea BusquedaFiltroLinea(string nombre)
        {

            Linea lista = new Linea();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "select l.IdLinea,l.Descripcion , l.Activo, lc.Descripcion as Deslc  from tLineas l inner join tLineasCaracteristicas lc on lc.IdLinea = l.IdLinea where l.Descripcion ='"+nombre+"'";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista= new Linea
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

        //NOMBRE DE LAS LINEAS PARA AUTOCOMPLETAR EN BUSCADOR
        public List<string> ListarNombreDeLineas(string linea)
        {

            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tLineas WHERE Descripcion LIKE '%"+linea+"%';";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(
                                Convert.ToString(dr["Descripcion"])
                            );
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
        public Linea UltimoRegistro()
        {

            Linea lista = new Linea();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 l.IdLinea, l.Descripcion, l.Activo, lc.Descripcion AS Deslc FROM tLineas l INNER JOIN tLineasCaracteristicas lc ON lc.IdLinea = l.IdLinea ORDER BY l.FechaUltimoCambio DESC;\r\n";

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

        //PRUEBA AUTOCOMPLETADO
        public List<Linea> PruebasAutoCompletado()
        {
            List<Linea> lista = new List<Linea>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdLinea, Descripcion, Activo FROM tLineas ORDER BY FechaUltimoCambio DESC";

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
                                Activo = Convert.ToString(dr["Activo"])
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
        //////////////////
        ///Prueba AutoCompletado PAGINADO
        //////////////////
        public int COUNT_PruebasAutoCompletado(string linea)
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


        //NOMBRE DE LAS LINEAS PARA AUTOCOMPLETAR EN BUSCADOR
        public List<string> PaginacionPRUEBA(string linea,int pagina )
        {
            pagina = pagina * 5;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tLineas WHERE Descripcion LIKE '%" + linea + "%' ORDER BY FechaUltimoCambio DESC OFFSET "+pagina+" ROWS FETCH NEXT 5 ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(
                                Convert.ToString(dr["Descripcion"])
                            );
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



    }
}

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
        //GUARDAR
        public int Registrar(Marca obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Verificar si la marca ya existe
                    string queryVerificacion = "SELECT COUNT(*) FROM tMarcas WHERE Descripcion = @Descripcion";
                    using (SqlCommand cmdVerificacion = new SqlCommand(queryVerificacion, oconexion))
                    {
                        cmdVerificacion.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                        int existe = Convert.ToInt32(cmdVerificacion.ExecuteScalar());

                        if (existe == 0)
                        {
                            string queryInsertar = @"
                    INSERT INTO tMarcas 
                    (Descripcion, Activo, FechaRegistro, PersonaUltimoCambio, PersonaRegistro, FechaUltimoCambio) 
                    VALUES 
                    (@Descripcion, @Activo, GETDATE(), @Persona, @Persona, GETDATE()); 
                    SELECT SCOPE_IDENTITY();";
                            using (SqlCommand cmdInsertar = new SqlCommand(queryInsertar, oconexion))
                            {
                                cmdInsertar.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                                cmdInsertar.Parameters.AddWithValue("@Activo", obj.Activo);
                                cmdInsertar.Parameters.AddWithValue("@Persona", obj.oUsuario.IdUsuario);

                                idautogenerado = Convert.ToInt32(cmdInsertar.ExecuteScalar());
                            }
                        }
                        else
                        {
                            Mensaje = "La marca ya existe";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }

            return idautogenerado;
        }

        //EDITAR
        public bool Editar(Marca obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Verificar si existe otra marca con la misma descripción pero diferente ID
                    string queryVerificacion = "SELECT COUNT(*) FROM tMarcas WHERE Descripcion = @Descripcion AND IdMarca != @IdMarca";
                    using (SqlCommand cmdVerificacion = new SqlCommand(queryVerificacion, oconexion))
                    {
                        cmdVerificacion.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                        cmdVerificacion.Parameters.AddWithValue("@IdMarca", obj.IdMarca);
                        int existe = Convert.ToInt32(cmdVerificacion.ExecuteScalar());

                        if (existe == 0)
                        {
                            string queryActualizar = @"
                    UPDATE tMarcas SET 
                    Descripcion = @Descripcion, 
                    Activo = @Activo, 
                    FechaUltimoCambio = GETDATE(), 
                    PersonaUltimoCambio = @PersonaUltimoCambio 
                    WHERE IdMarca = @IdMarca";

                            using (SqlCommand cmdActualizar = new SqlCommand(queryActualizar, oconexion))
                            {
                                cmdActualizar.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                                cmdActualizar.Parameters.AddWithValue("@Activo", obj.Activo);
                                cmdActualizar.Parameters.AddWithValue("@IdMarca", obj.IdMarca);
                                cmdActualizar.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario






                                    );

                                int filasAfectadas = cmdActualizar.ExecuteNonQuery();
                                resultado = filasAfectadas > 0;
                                if (!resultado) Mensaje = "No se encontró la marca para actualizar.";
                            }
                        }
                        else
                        {
                            Mensaje = "La marca ya existe con ese nombre";
                            resultado = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        //ELIMINAR
        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Verificar si la marca está relacionada con algún producto
                    string queryVerificacion = @"
            SELECT COUNT(*)
            FROM tProductos
            WHERE IdMarca = @IdMarca";

                    using (SqlCommand cmdVerificacion = new SqlCommand(queryVerificacion, oconexion))
                    {
                        cmdVerificacion.Parameters.AddWithValue("@IdMarca", id);
                        int relacionado = Convert.ToInt32(cmdVerificacion.ExecuteScalar());

                        if (relacionado == 0)
                        {
                            // Si no está relacionada, eliminar la marca
                            string queryEliminar = "DELETE FROM tMarcas WHERE IdMarca = @IdMarca";
                            using (SqlCommand cmdEliminar = new SqlCommand(queryEliminar, oconexion))
                            {
                                cmdEliminar.Parameters.AddWithValue("@IdMarca", id);
                                int filasAfectadas = cmdEliminar.ExecuteNonQuery();
                                resultado = filasAfectadas > 0; // Verdadero si se afectaron filas, falso si no
                                if (!resultado) Mensaje = "No se encontró la marca para eliminar.";
                            }

                        }
                        else
                        {
                            Mensaje = "La marca se encuentra relacionada a un producto";
                            resultado = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        // Buscar Una Marca por el nombre 
        public Marca bucarMarcaPorNombre(string nombre)
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
        // Buscar Una Linea por el ID 
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
        //Ultimo Registro
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
        /************BUSCADOR************/
        public List<string> elementosPaginacionBuscador(string nombre, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Descripcion FROM tMarcas WHERE Descripcion LIKE '%" + nombre + "%' ORDER BY Descripcion DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
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

        //      COUNT
public int CountBuscador(string descripcion)
        {
            try
            {
                using (var conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();
                    var query = "SELECT COUNT(IdMarca) FROM tMarcas WHERE Descripcion LIKE '%" + descripcion + "%' ";
                    using (var cmd = new SqlCommand(query, conexion))
                    {

                        var result = cmd.ExecuteScalar();  
                        return Convert.ToInt32(result);   
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;  
            }
        }



        /************TABLA************/
        public List<Marca> ListarMarcaTabla(int pagina, string tipoOrden, int siguientes)
        {
            List<Marca> lista = new List<Marca>();
            try
            {
                string orden = Orden(tipoOrden);
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT m.IdMarca, m.Descripcion, m.Activo FROM tMarcas m ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

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
        //      COUNT
        public int CountTabla()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tMarcas ;";

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
        public List<Marca> ListarMarcaTablaWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<Marca> lista = new List<Marca>();
            try
            {
                string orden = Orden(tipoOrden);
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT m.IdMarca, m.Descripcion, m.Activo FROM tMarcas m WHERE {where} ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

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
        //      COUNT
        public int countTablaWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    // La consulta directa con 'where' es vulnerable a SQL Injection
                    string query = $"SELECT COUNT(m.IdMarca) AS TotalRegistros FROM tMarcas m WHERE {where};";
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

        //Buscador Marcas Existente para Productos
        public List<Marca> elementosPaginacionBuscadorDescripcionID(string marca, int pagina, int siguientes)
        {

            List<Marca> lista = new List<Marca>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT m.IdMarca, m.Descripcion FROM tMarcas m WHERE m.Descripcion LIKE @Marca   ORDER BY m.IdMarca  OFFSET @Pagina ROWS FETCH NEXT @Siguientes ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    // Agrega los parámetros necesarios para la consulta
                    cmd.Parameters.AddWithValue("@Marca", $"%{marca}%");
                    cmd.Parameters.AddWithValue("@Pagina", pagina * siguientes);
                    cmd.Parameters.AddWithValue("@Siguientes", siguientes);

                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Marca
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = Convert.ToString(dr["Descripcion"])
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

        public string Orden(string tipoOrden)
        {
            switch (tipoOrden)
            {
                case "I_A":
                    return "m.IdMarca";
                case "I_D":
                    return "m.IdMarca DESC";
                case "D_A":
                    return "m.Descripcion ";
                case "D_D":
                    return "m.Descripcion DESC";
                default:
                    return "m.IdMarca";
            }
        }
    }
}

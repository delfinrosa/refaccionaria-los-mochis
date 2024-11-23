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
    public class CD_Usuario
    {
        public int Registrar(Usuario obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Verificar si el usuario ya existe
                    string queryVerificacion = "SELECT COUNT(*) FROM tUsuarios WHERE Nombre = @Nombres";
                    using (SqlCommand cmdVerificacion = new SqlCommand(queryVerificacion, oconexion))
                    {
                        cmdVerificacion.Parameters.AddWithValue("@Correo", obj.Correo);
                        cmdVerificacion.Parameters.AddWithValue("@Nombres", obj.Nombre);
                        int existe = Convert.ToInt32(cmdVerificacion.ExecuteScalar());

                        if (existe == 0)
                        {
                            // Insertar nuevo usuario si no existe y obtener el ID autogenerado
                            string queryInsertar = @"
INSERT INTO tUsuarios (Nombre, Contraseña, Correo, Tipo, Activo) 
VALUES (@Nombres, @Contraseña, @Correo, @Tipo, @Activo);
SELECT SCOPE_IDENTITY();"; // Esta línea obtiene el último ID generado

                            using (SqlCommand cmdInsertar = new SqlCommand(queryInsertar, oconexion))
                            {
                                cmdInsertar.Parameters.AddWithValue("@Nombres", obj.Nombre);
                                cmdInsertar.Parameters.AddWithValue("@Contraseña", obj.Contraseña);
                                cmdInsertar.Parameters.AddWithValue("@Correo", obj.Correo);
                                cmdInsertar.Parameters.AddWithValue("@Tipo", obj.Tipo);
                                cmdInsertar.Parameters.AddWithValue("@Activo", obj.Activo);

                                // ExecuteScalar devuelve el valor de la primera columna de la primera fila
                                idautogenerado = Convert.ToInt32(cmdInsertar.ExecuteScalar());
                            }

                        }
                        else
                        {
                            Mensaje = "El usuario ya existe";
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
        public bool Editar(Usuario obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Obtener la contraseña almacenada en la base de datos
                    string queryObtenerContraseña = "SELECT Contraseña FROM tUsuarios WHERE IdUsuario = @IdUsuario";
                    string contraseñaActualBD = string.Empty;

                    using (SqlCommand cmdObtenerContraseña = new SqlCommand(queryObtenerContraseña, oconexion))
                    {
                        cmdObtenerContraseña.Parameters.AddWithValue("@IdUsuario", obj.IdUsuario);
                        var resultadoConsulta = cmdObtenerContraseña.ExecuteScalar();
                        contraseñaActualBD = resultadoConsulta != null ? resultadoConsulta.ToString() : string.Empty;
                    }

                    // Verificar si la contraseña no ha cambiado
                    bool contraseñaModificada = obj.Contraseña != contraseñaActualBD;

                    // Verificar si existe otro usuario con el mismo correo pero diferente ID
                    string queryVerificacion = "SELECT COUNT(*) FROM tUsuarios WHERE Correo = @Correo AND IdUsuario != @IdUsuario";
                    using (SqlCommand cmdVerificacion = new SqlCommand(queryVerificacion, oconexion))
                    {
                        cmdVerificacion.Parameters.AddWithValue("@Correo", obj.Correo);
                        cmdVerificacion.Parameters.AddWithValue("@IdUsuario", obj.IdUsuario);
                        int existe = Convert.ToInt32(cmdVerificacion.ExecuteScalar());

                        if (existe == 0)
                        {
                            // Construir la consulta de actualización
                            string queryActualizar = @"
                        UPDATE tUsuarios SET
                        Nombre = @Nombres,
                        Correo = @Correo,
                        Tipo = @Tipo,
                        Activo = @Activo,
                        FechaUltimoCambio=GETDATE()";

                            // Solo incluir la contraseña si fue modificada
                            if (contraseñaModificada)
                            {
                                queryActualizar += ", Contraseña = @Contraseña";
                                obj.Contraseña = new Recursos().ConvertirSha256(obj.Contraseña);

                            }

                            queryActualizar += " WHERE IdUsuario = @IdUsuario";

                            using (SqlCommand cmdActualizar = new SqlCommand(queryActualizar, oconexion))
                            {
                                cmdActualizar.Parameters.AddWithValue("@IdUsuario", obj.IdUsuario);
                                cmdActualizar.Parameters.AddWithValue("@Nombres", obj.Nombre);
                                cmdActualizar.Parameters.AddWithValue("@Correo", obj.Correo);
                                cmdActualizar.Parameters.AddWithValue("@Tipo", obj.Tipo);
                                cmdActualizar.Parameters.AddWithValue("@Activo", obj.Activo);

                                if (contraseñaModificada)
                                {
                                    cmdActualizar.Parameters.AddWithValue("@Contraseña", obj.Contraseña);
                                }

                                int filasAfectadas = cmdActualizar.ExecuteNonQuery();
                                resultado = filasAfectadas > 0;

                                if (!resultado)
                                {
                                    Mensaje = "No se encontró el usuario para actualizar.";
                                }
                            }
                        }
                        else
                        {
                            Mensaje = "El usuario ya existe con otro correo.";
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



        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Eliminar el usuario por ID
                    string queryEliminar = "DELETE FROM tUsuarios WHERE IdUsuario = @IdUsuario";
                    using (SqlCommand cmdEliminar = new SqlCommand(queryEliminar, oconexion))
                    {
                        cmdEliminar.Parameters.AddWithValue("@IdUsuario", id);
                        int filasAfectadas = cmdEliminar.ExecuteNonQuery();
                        resultado = filasAfectadas > 0; // Verdadero si se eliminó el usuario
                        if (!resultado) Mensaje = "No se encontró el usuario para eliminar.";
                        else Mensaje = "Usuario eliminado correctamente.";
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

        //LOGGIN
        public Usuario Verificacion(string correo, string clave, out string Mensaje)
        {
            Usuario ObjUsuario = null; 
            Mensaje = string.Empty; 
            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdUsuario, Nombre, Tipo, Activo FROM tUsuarios WHERE Nombre = @Correo AND Contraseña = @Clave";
                    SqlCommand cmd = new SqlCommand(query, oconexion);

                    cmd.Parameters.AddWithValue("@Correo", correo);
                    cmd.Parameters.AddWithValue("@Clave", clave);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) 
                        {
                            ObjUsuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Nombre = Convert.ToString(dr["Nombre"]),
                                Tipo = Convert.ToString(dr["Tipo"]),
                                Activo = Convert.ToString(dr["Activo"])
                            };
                        }
                        else
                        {
                            Mensaje = "No se encontró el usuario o la contraseña es incorrecta.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ObjUsuario = null;
                Mensaje = "Error: " + ex.Message;
            }

            return ObjUsuario;
        }

        // Buscar Una Usuario por el nombre 
        public Usuario bucarUsuarioPorNombre(string nombre)
        {
            Usuario lista = new Usuario();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "Select u.IdUsuario, u.Nombre, u.Activo , u.Tipo, u.Correo, u.Contraseña FROM tUsuarios as u where nombre ='" + nombre + "'";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Nombre = Convert.ToString(dr["Nombre"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Tipo = Convert.ToString(dr["Tipo"]),
                                Correo = Convert.ToString(dr["Correo"]),
                                Contraseña = Convert.ToString(dr["Contraseña"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new Usuario();
            }
            return lista;
        }
        // Buscar Una Usuario por el ID 
        public Usuario BusquedaIDUsuario(int Id)
        {
            Usuario lista = new Usuario();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "Select u.IdUsuario, u.Nombre, u.Activo , u.Tipo, u.Correo, u.Contraseña FROM tUsuarios as u where u.IdUsuario ='" + Id + "'";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Nombre = Convert.ToString(dr["Nombre"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Tipo = Convert.ToString(dr["Tipo"]),
                                Correo = Convert.ToString(dr["Correo"]),
                                Contraseña = Convert.ToString(dr["Contraseña"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new Usuario();
            }
            return lista;
        }
        //Ultimo Registro
        public Usuario UltimoRegistro()
        {
            Usuario lista = new Usuario();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 u.IdUsuario, u.Nombre, u.Activo , u.Tipo, u.Correo, u.Contraseña FROM tUsuarios as u ORDER BY u.FechaUltimoCambio DESC;";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Nombre = Convert.ToString(dr["Nombre"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Tipo = Convert.ToString(dr["Tipo"]),
                                Correo= Convert.ToString(dr["Correo"]),
                                Contraseña = Convert.ToString(dr["Contraseña"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new Usuario();
            }
            return lista;
        }
        /************BUSCADOR************/
        public List<string> elementosPaginacionBuscador(string Usuario, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Nombre FROM tUsuarios WHERE Nombre LIKE '%" + Usuario + "%' ORDER BY nombre DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(Convert.ToString(dr["Nombre"]));
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
        public int countBuscador(string Usuario)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(IdUsuario) AS TotalRegistros FROM tUsuarios WHERE Nombre LIKE '%" + Usuario + "%' ;";

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
        public List<Usuario> ListarUsuarioTabla(int pagina, string tipoOrden, int siguientes)
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                string orden = Orden(tipoOrden);
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT u.IdUsuario, u.Nombre, u.Activo , u.Tipo, u.Correo, u.Contraseña FROM tUsuarios as u ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Nombre = Convert.ToString(dr["Nombre"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Tipo = Convert.ToString(dr["Tipo"]),
                                Correo = Convert.ToString(dr["Correo"]),
                                Contraseña = Convert.ToString(dr["Contraseña"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Usuario>();
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
                    string query = "SELECT COUNT(IdUsuario) AS TotalRegistros FROM tUsuarios ;";

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
        public List<Usuario> ListarUsuarioTablaWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                string orden = Orden(tipoOrden);
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT u.IdUsuario, u.Nombre, u.Activo , u.Tipo, u.Correo, u.Contraseña FROM tUsuarios as u WHERE {where} ORDER BY {orden} OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Nombre = Convert.ToString(dr["Nombre"]),
                                Activo = Convert.ToString(dr["Activo"]),
                                Tipo = Convert.ToString(dr["Tipo"]),
                                Correo = Convert.ToString(dr["Correo"]),
                                Contraseña = Convert.ToString(dr["Contraseña"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Usuario>();
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
                    string query = "SELECT COUNT(u.IdUsuario) AS TotalRegistros FROM tUsuarios u WHERE " + where + ";";

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
        public bool CambiarContraseñaPorUsuario(string nombre, string correo, string nuevaContraseña, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Verificar si el usuario existe con el nombre y correo proporcionados
                    string queryVerificacion = "SELECT IdUsuario FROM tUsuarios WHERE Nombre = @Nombre AND Correo = @Correo";
                    int idUsuario = 0;

                    using (SqlCommand cmdVerificacion = new SqlCommand(queryVerificacion, oconexion))
                    {
                        cmdVerificacion.Parameters.AddWithValue("@Nombre", nombre);
                        cmdVerificacion.Parameters.AddWithValue("@Correo", correo);

                        var resultadoConsulta = cmdVerificacion.ExecuteScalar();
                        if (resultadoConsulta != null)
                        {
                            idUsuario = Convert.ToInt32(resultadoConsulta);
                        }
                    }

                    if (idUsuario > 0)
                    {
                        // Si el usuario existe, actualizar la contraseña
                        string nuevaContraseñaCifrada = new Recursos().ConvertirSha256(nuevaContraseña);
                        string queryActualizar = @"
                    UPDATE tUsuarios 
                    SET Contraseña = @Contraseña, 
                        FechaUltimoCambio = GETDATE() 
                    WHERE IdUsuario = @IdUsuario";

                        using (SqlCommand cmdActualizar = new SqlCommand(queryActualizar, oconexion))
                        {
                            cmdActualizar.Parameters.AddWithValue("@Contraseña", nuevaContraseñaCifrada);
                            cmdActualizar.Parameters.AddWithValue("@IdUsuario", idUsuario);

                            int filasAfectadas = cmdActualizar.ExecuteNonQuery();
                            resultado = filasAfectadas > 0;

                            if (resultado)
                            {
                                mensaje = "Contraseña actualizada correctamente.";
                            }
                            else
                            {
                                mensaje = "No se pudo actualizar la contraseña.";
                            }
                        }
                    }
                    else
                    {
                        // Si no se encuentra el usuario, devolver mensaje
                        mensaje = "El usuario con el nombre y correo proporcionados no existe.";
                        resultado = false;
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = ex.Message;
            }

            return resultado;
        }

        public string Orden(string tipoOrden)
        {
            switch (tipoOrden)
            {
                case "I_A":
                    return "u.IdUsuario";
                case "I_D":
                    return "u.IdUsuario DESC";
                case "N_A":
                    return "u.Nombre";
                case "N_D":
                    return "u.Nombre DESC";
                default:
                    return "u.IdUsuario";
            }
        }







    }
}

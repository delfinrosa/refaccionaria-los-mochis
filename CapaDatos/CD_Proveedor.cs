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
    public class CD_Proveedor
    {
        //GUARDAR
        public int Registrar(Proveedor obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    string queryProveedor = @"
            INSERT INTO tProveedores 
            (RFC, Estatus, RazonSocial, Calle, Colonia, NumeroInterior, NumeroExterior, Pais, Ciudad, Estado, CodigoPostal, Telefono, Correo, Comentarios, FechaRegistro, PersonaUltimoCambio, PersonaRegistro, FechaUltimoCambio)
            VALUES 
            (@RFC, @Estatus, @RazonSocial, @Calle, @Colonia, @NumeroInterior, @NumeroExterior, @Pais, @Ciudad, @Estado, @CodigoPostal, @Telefono, @Correo, @Comentarios, GETDATE(), @PersonaUltimoCambio, @PersonaRegistro, GETDATE());";

                    using (SqlCommand cmdProveedor = new SqlCommand(queryProveedor, oconexion))
                    {
                        cmdProveedor.Parameters.AddWithValue("@RFC", obj.RFC);
                        cmdProveedor.Parameters.AddWithValue("@Estatus", obj.Estatus);
                        cmdProveedor.Parameters.AddWithValue("@RazonSocial", obj.RazonSocial);
                        cmdProveedor.Parameters.AddWithValue("@Calle", obj.Calle);
                        cmdProveedor.Parameters.AddWithValue("@Colonia", obj.Colonia);
                        cmdProveedor.Parameters.AddWithValue("@NumeroInterior", obj.NumeroInt);
                        cmdProveedor.Parameters.AddWithValue("@NumeroExterior", obj.NumeroExt);
                        cmdProveedor.Parameters.AddWithValue("@Pais", obj.Pais);
                        cmdProveedor.Parameters.AddWithValue("@Ciudad", obj.Ciudad);
                        cmdProveedor.Parameters.AddWithValue("@Estado", obj.Estado);
                        cmdProveedor.Parameters.AddWithValue("@CodigoPostal", obj.CP);
                        cmdProveedor.Parameters.AddWithValue("@Telefono", obj.Telefono);
                        cmdProveedor.Parameters.AddWithValue("@Correo", obj.Correo);
                        cmdProveedor.Parameters.AddWithValue("@Comentarios", obj.Comentario);
                        cmdProveedor.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);
                        cmdProveedor.Parameters.AddWithValue("@PersonaRegistro", obj.oUsuario.IdUsuario);

                        cmdProveedor.ExecuteNonQuery();
                    }

                    oconexion.Close();
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return 0; // Devuelve 0 en caso de error
            }

            return 1; // Devuelve 1 si la inserción fue exitosa
        }


        // EDITAR
        public bool Editar(Proveedor obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            bool resultado = false;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    string queryProveedor = @"
            UPDATE tProveedores SET 
            Estatus = @Estatus, 
            RazonSocial = @RazonSocial, 
            Calle = @Calle, 
            Colonia = @Colonia, 
            NumeroInterior = @NumeroInterior, 
            NumeroExterior = @NumeroExterior, 
            Pais = @Pais, 
            Ciudad = @Ciudad, 
            Estado = @Estado, 
            CodigoPostal = @CodigoPostal, 
            Telefono = @Telefono, 
            Correo = @Correo, 
            Comentarios = @Comentarios, 
            PersonaUltimoCambio = @PersonaUltimoCambio, 
            FechaUltimoCambio = GETDATE()
            WHERE RFC = @RFC;";

                    using (SqlCommand cmdProveedor = new SqlCommand(queryProveedor, oconexion))
                    {
                        // Añadir los parámetros necesarios para la consulta
                        cmdProveedor.Parameters.AddWithValue("@RFC", obj.RFC);
                        cmdProveedor.Parameters.AddWithValue("@Estatus", obj.Estatus);
                        cmdProveedor.Parameters.AddWithValue("@RazonSocial", obj.RazonSocial);
                        cmdProveedor.Parameters.AddWithValue("@Calle", obj.Calle);
                        cmdProveedor.Parameters.AddWithValue("@Colonia", obj.Colonia);
                        cmdProveedor.Parameters.AddWithValue("@NumeroInterior", obj.NumeroInt);
                        cmdProveedor.Parameters.AddWithValue("@NumeroExterior", obj.NumeroExt);
                        cmdProveedor.Parameters.AddWithValue("@Pais", obj.Pais);
                        cmdProveedor.Parameters.AddWithValue("@Ciudad", obj.Ciudad);
                        cmdProveedor.Parameters.AddWithValue("@Estado", obj.Estado);
                        cmdProveedor.Parameters.AddWithValue("@CodigoPostal", obj.CP);
                        cmdProveedor.Parameters.AddWithValue("@Telefono", obj.Telefono);
                        cmdProveedor.Parameters.AddWithValue("@Correo", obj.Correo);
                        cmdProveedor.Parameters.AddWithValue("@Comentarios", obj.Comentario);
                        cmdProveedor.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);

                        int filasAfectadas = cmdProveedor.ExecuteNonQuery();
                        resultado = filasAfectadas > 0; // Si se afectó al menos una fila, la operación fue exitosa
                    }

                    oconexion.Close();
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                resultado = false;
            }

            return resultado;
        }

        // ELIMINAR
        public bool Eliminar(string RFC, out string Mensaje)
        {
            Mensaje = string.Empty;
            bool resultado = false;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    string queryProveedor = "DELETE FROM tProveedores WHERE RFC = @RFC;";

                    using (SqlCommand cmdProveedor = new SqlCommand(queryProveedor, oconexion))
                    {
                        cmdProveedor.Parameters.AddWithValue("@RFC", RFC);

                        int filasAfectadas = cmdProveedor.ExecuteNonQuery();
                        resultado = filasAfectadas > 0; // Si se afectó al menos una fila, la operación fue exitosa
                    }

                    oconexion.Close();
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                resultado = false; // En caso de error
            }

            return resultado; // Devuelve true si la eliminación fue exitosa, false en caso contrario
        }



        // Buscar Una Provedor por el nombre 
        public Proveedor BuscarProveedorPorNombre(string nombre)
        {
            Proveedor proveedorEncontrado = null;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 RFC, Estatus, RazonSocial, Calle, Colonia, NumeroInterior, NumeroExterior, " +
                                   "Pais, Ciudad, Estado, CodigoPostal, Telefono, Correo, Comentarios" +
                                   " FROM tProveedores " +
                                   "WHERE RazonSocial = @Nombre "; // Ordena por FechaUltimoCambio para obtener el último registro modificado
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Nombre", nombre); // Parámetro para filtrar por RazonSocial
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            proveedorEncontrado = new Proveedor
                            {
                                RFC = Convert.ToString(dr["RFC"]),
                                Estatus = Convert.ToString(dr["Estatus"]),
                                RazonSocial = Convert.ToString(dr["RazonSocial"]),
                                Calle = Convert.ToString(dr["Calle"]),
                                Colonia = Convert.ToString(dr["Colonia"]),
                                NumeroInt = Convert.ToString(dr["NumeroInterior"]),
                                NumeroExt = Convert.ToString(dr["NumeroExterior"]),
                                Pais = Convert.ToString(dr["Pais"]),
                                Ciudad = Convert.ToString(dr["Ciudad"]),
                                Estado = Convert.ToString(dr["Estado"]),
                                CP = Convert.ToString(dr["CodigoPostal"]),
                                Telefono = Convert.ToString(dr["Telefono"]),
                                Correo = Convert.ToString(dr["Correo"]),
                                Comentario = Convert.ToString(dr["Comentarios"]),
                            };
                        }
                    }
                }


            }
            catch (Exception)
            {
                proveedorEncontrado = null;
            }
            return proveedorEncontrado;
        }
        // Buscar Un Proveedor por el ID 
        public Proveedor BusquedaID(string RFC)
        {
            Proveedor proveedorEncontrado = null;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM tProveedores WHERE RFC = @RFC";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@RFC", RFC);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            proveedorEncontrado = new Proveedor
                            {
                                RFC = Convert.ToString(dr["RFC"]),
                                Estatus = Convert.ToString(dr["Estatus"]),
                                RazonSocial = Convert.ToString(dr["RazonSocial"]),
                                Calle = Convert.ToString(dr["Calle"]),
                                Colonia = Convert.ToString(dr["Colonia"]),
                                NumeroInt = Convert.ToString(dr["NumeroInterior"]),
                                NumeroExt = Convert.ToString(dr["NumeroExterior"]),
                                Pais = Convert.ToString(dr["Pais"]),
                                Ciudad = Convert.ToString(dr["Ciudad"]),
                                Estado = Convert.ToString(dr["Estado"]),
                                CP = Convert.ToString(dr["CodigoPostal"]),
                                Telefono = Convert.ToString(dr["Telefono"]),
                                Correo = Convert.ToString(dr["Correo"]),
                                Comentario = Convert.ToString(dr["Comentarios"]),
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                proveedorEncontrado = null;
            }
            return proveedorEncontrado;
        }
        //Ultimo Registro
        public Proveedor UltimoRegistro()
        {
            Proveedor ultimoRegistro = null;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT TOP 1 RFC, Estatus, RazonSocial, Calle, Colonia, NumeroInterior, NumeroExterior, Pais, Ciudad, Estado, CodigoPostal, Telefono, Correo, Comentarios FROM tProveedores ORDER BY FechaUltimoCambio DESC;";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            ultimoRegistro = new Proveedor
                            {
                                RFC = Convert.ToString(dr["RFC"]),
                                Estatus = Convert.ToString(dr["Estatus"]),
                                RazonSocial = Convert.ToString(dr["RazonSocial"]),
                                Calle = Convert.ToString(dr["Calle"]),
                                Colonia = Convert.ToString(dr["Colonia"]),
                                NumeroInt = Convert.ToString(dr["NumeroInterior"]),
                                NumeroExt= Convert.ToString(dr["NumeroExterior"]),
                                Pais = Convert.ToString(dr["Pais"]),
                                Ciudad = Convert.ToString(dr["Ciudad"]),
                                Estado = Convert.ToString(dr["Estado"]),
                                CP = Convert.ToString(dr["CodigoPostal"]),
                                Telefono = Convert.ToString(dr["Telefono"]),
                                Correo = Convert.ToString(dr["Correo"]),
                                Comentario = Convert.ToString(dr["Comentarios"])
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                ultimoRegistro = null;
            }
            return ultimoRegistro;
        }
        /************BUSCADOR************/
        public List<string> ElementosPaginacionBuscador(string nombreProveedor, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT RazonSocial FROM tProveedores WHERE RazonSocial LIKE '%" + nombreProveedor + "%' ORDER BY RazonSocial DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(Convert.ToString(dr["RazonSocial"]));
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
        //COUNT
        public int CountBuscador(string nombreProveedor)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(RazonSocial) AS TotalRegistros FROM tProveedores WHERE RazonSocial LIKE '%" + nombreProveedor + "%' ;";

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
        public List<Proveedor> ListarProveedor(int pagina, string tipoOrden, int siguientes)
        {
            List<Proveedor> lista = new List<Proveedor>();
            try
            {
                string orden = Orden(tipoOrden);
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT  P.RFC, P.RazonSocial, P.Telefono, P.Correo FROM tProveedores AS P ORDER BY " + orden + " OFFSET " + pagina * siguientes + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY;";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Proveedor
                            {
                                //IdProveedor = Convert.ToInt32(dr["IdProvedor"]),
                                RFC = Convert.ToString(dr["RFC"]),
                                RazonSocial = Convert.ToString(dr["RazonSocial"]),
                                Telefono = Convert.ToString(dr["Telefono"]),
                                Correo = Convert.ToString(dr["Correo"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Proveedor>();
            }
            return lista;
        }
        //      COUNT Tabla
        public int CountTabla()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(RFC) AS TotalRegistros FROM tProveedores ;";
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
        public List<Proveedor> ListarProveedorWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<Proveedor> lista = new List<Proveedor>();
            try
            {
                string orden = Orden(tipoOrden);
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT P.RFC, P.RazonSocial, P.Telefono, P.Correo " +
                                   "FROM tProveedores AS P " +
                                   "WHERE " + where +
                                   "ORDER BY " + orden + " " +
                                   "OFFSET " + (pagina * siguientes) + " ROWS " +
                                   "FETCH NEXT " + siguientes + " ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Proveedor
                            {
                                //IdProveedor = Convert.ToInt32(dr["IdProvedor"]),
                                RFC = Convert.ToString(dr["RFC"]),
                                RazonSocial = Convert.ToString(dr["RazonSocial"]),
                                Telefono = Convert.ToString(dr["Telefono"]),
                                Correo = Convert.ToString(dr["Correo"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Proveedor>();
            }
            return lista;
        }
        public int countWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(RFC) AS TotalRegistros FROM tProveedores WHERE " + where + ";";

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
                    return "P.RFC ";
                case "I_D":
                    return "P.RFC DESC";
                case "R_A":
                    return "P.RazonSocial ";
                case "R_D":
                    return "P.RazonSocial DESC";
                default:
                    return "P.RFC ";
            }
        }
        /*
         ESTO ES PARA CUANDO SE OCUPE
         */
        public List<Proveedor> elementosPaginacionBuscadorRFC(string criterio, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<Proveedor> lista = new List<Proveedor>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT RFC, RazonSocial FROM tProveedores WHERE RFC LIKE '%" + criterio  + "%' ORDER BY FechaUltimoCambio DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Proveedor
                            {
                                RFC = Convert.ToString(dr["RFC"]),
                                RazonSocial = Convert.ToString(dr["RazonSocial"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Proveedor>();
            }
            return lista;
        }
        //BUSCADOR CON RAZON SOCIAL 
        public List<Proveedor> elementosPaginacionBuscadorRazonSocial(string criterio, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<Proveedor> lista = new List<Proveedor>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT RFC, RazonSocial FROM tProveedores WHERE RazonSocial LIKE '%" + criterio  + "%' ORDER BY FechaUltimoCambio DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Proveedor
                            {
                                RFC = Convert.ToString(dr["RFC"]),
                                RazonSocial = Convert.ToString(dr["RazonSocial"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Proveedor>();
            }
            return lista;
        }
        //COUNT RAZON SOCIAL 
        public int countBuscadorRazonSocial(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(RFC) AS TotalRegistros FROM tProveedores WHERE RazonSocial LIKE '%" + where + "%' ;";

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

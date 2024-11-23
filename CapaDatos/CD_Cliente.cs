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
    public class CD_Cliente
    {
        public int Registrar(Cliente obj, out string mensaje)
        {
            int idAutogenerado = 0;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();
                    string query = @"
                INSERT INTO tClientes 
                (RFC, Estatus, RazonSocial, NombreCorto, Calle, NumeroInterior, NumeroExterior, Colonia, Ciudad, Estado, Pais, CodigoPostal, Telefono, Credito, LimiteCredito, DiasCredito, CuentaPago, CuentaOrdenante, RFCBanco, NombreBanco, CFDIUsoId, CFDIMetodoPagoId, CFDIFormaPagoId, CFDIRegimenFiscalId, Correo, Comentarios, PersonaUltimoCambio, PersonaRegistro, FechaRegistro, FechaUltimoCambio) 
                VALUES 
                (@RFC, @Estatus, @RazonSocial, @NombreCorto, @Calle, @NumeroInterior, @NumeroExterior, @Colonia, @Ciudad, @Estado, @Pais, @CodigoPostal, @Telefono, @Credito, @LimiteCredito, @DiasCredito, @CuentaPago, @CuentaOrdenante, @RFCBanco, @NombreBanco, @CFDIUsoId, @CFDIMetodoPagoId, @CFDIFormaPagoId, @CFDIRegimenFiscalId, @Correo, @Comentarios, @PersonaUltimoCambio, @PersonaRegistro, GETDATE(), GETDATE());
                SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@RFC", obj.RFC);
                        cmd.Parameters.AddWithValue("@Estatus", obj.Estatus);
                        cmd.Parameters.AddWithValue("@RazonSocial", obj.RazonSocial);
                        cmd.Parameters.AddWithValue("@NombreCorto", obj.NombreCorto);
                        cmd.Parameters.AddWithValue("@Calle", obj.Calle);
                        cmd.Parameters.AddWithValue("@NumeroInterior", obj.NumeroInterior);
                        cmd.Parameters.AddWithValue("@NumeroExterior", obj.NumeroExterior);
                        cmd.Parameters.AddWithValue("@Colonia", obj.Colonia);
                        cmd.Parameters.AddWithValue("@Ciudad", obj.Ciudad);
                        cmd.Parameters.AddWithValue("@Estado", obj.Estado);
                        cmd.Parameters.AddWithValue("@Pais", obj.Pais);
                        cmd.Parameters.AddWithValue("@CodigoPostal", obj.CodigoPostal);
                        cmd.Parameters.AddWithValue("@Telefono", obj.Telefono);
                        cmd.Parameters.AddWithValue("@Credito", obj.Credito);
                        cmd.Parameters.AddWithValue("@LimiteCredito", obj.LimiteCredito);
                        cmd.Parameters.AddWithValue("@DiasCredito", obj.DiasCredito);
                        cmd.Parameters.AddWithValue("@CuentaPago", obj.CuentaPago);
                        cmd.Parameters.AddWithValue("@CuentaOrdenante", obj.CuentaOrdenante);
                        cmd.Parameters.AddWithValue("@RFCBanco", obj.RFCBanco);
                        cmd.Parameters.AddWithValue("@NombreBanco", obj.NombreBanco);
                        cmd.Parameters.AddWithValue("@CFDIUsoId", obj.oCFDIUso.CFDIUsoId);
                        cmd.Parameters.AddWithValue("@CFDIMetodoPagoId", obj.oCFDIMetodoPago.CFDIMetodoPagoId);
                        cmd.Parameters.AddWithValue("@CFDIFormaPagoId", obj.oCFDIFormaPago.CFDIFormaPagoId);
                        cmd.Parameters.AddWithValue("@CFDIRegimenFiscalId", obj.oCFDIRegimenFiscal.CFDIRegimenFiscalId);
                        cmd.Parameters.AddWithValue("@Correo", obj.Correo);
                        cmd.Parameters.AddWithValue("@Comentarios", obj.Comentarios);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@PersonaRegistro", obj.oUsuario.IdUsuario);

                        idAutogenerado = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al registrar el cliente: " + ex.Message;
            }

            return idAutogenerado;
        }
        public void Editar(Cliente clienteModificado, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();
                    string query = @"
                UPDATE tClientes SET
                Estatus = @Estatus,
                RazonSocial = @RazonSocial,
                NombreCorto = @NombreCorto,
                Calle = @Calle,
                NumeroInterior = @NumeroInterior,
                NumeroExterior = @NumeroExterior,
                Colonia = @Colonia,
                Ciudad = @Ciudad,
                Estado = @Estado,
                Pais = @Pais,
                CodigoPostal = @CodigoPostal,
                Telefono = @Telefono,
                Credito = @Credito,
                LimiteCredito = @LimiteCredito,
                DiasCredito = @DiasCredito,
                CuentaPago = @CuentaPago,
                CuentaOrdenante = @CuentaOrdenante,
                RFCBanco = @RFCBanco,
                NombreBanco = @NombreBanco,
                CFDIUsoId = @CFDIUsoId,
                CFDIMetodoPagoId = @CFDIMetodoPagoId,
                CFDIFormaPagoId = @CFDIFormaPagoId,
                CFDIRegimenFiscalId = @CFDIRegimenFiscalId,
                Correo = @Correo,
                Comentarios = @Comentarios,
                PersonaUltimoCambio = @PersonaUltimoCambio,
                FechaUltimoCambio = GETDATE()
                WHERE RFC = @RFC";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Estatus", clienteModificado.Estatus);
                        cmd.Parameters.AddWithValue("@RazonSocial", clienteModificado.RazonSocial);
                        cmd.Parameters.AddWithValue("@NombreCorto", clienteModificado.NombreCorto);
                        cmd.Parameters.AddWithValue("@Calle", clienteModificado.Calle);
                        cmd.Parameters.AddWithValue("@NumeroInterior", clienteModificado.NumeroInterior);
                        cmd.Parameters.AddWithValue("@NumeroExterior", clienteModificado.NumeroExterior);
                        cmd.Parameters.AddWithValue("@Colonia", clienteModificado.Colonia);
                        cmd.Parameters.AddWithValue("@Ciudad", clienteModificado.Ciudad);
                        cmd.Parameters.AddWithValue("@Estado", clienteModificado.Estado);
                        cmd.Parameters.AddWithValue("@Pais", clienteModificado.Pais);
                        cmd.Parameters.AddWithValue("@CodigoPostal", clienteModificado.CodigoPostal);
                        cmd.Parameters.AddWithValue("@Telefono", clienteModificado.Telefono);
                        cmd.Parameters.AddWithValue("@Credito", clienteModificado.Credito);
                        cmd.Parameters.AddWithValue("@LimiteCredito", clienteModificado.LimiteCredito);
                        cmd.Parameters.AddWithValue("@DiasCredito", clienteModificado.DiasCredito);
                        cmd.Parameters.AddWithValue("@CuentaPago", clienteModificado.CuentaPago);
                        cmd.Parameters.AddWithValue("@CuentaOrdenante", clienteModificado.CuentaOrdenante);
                        cmd.Parameters.AddWithValue("@RFCBanco", clienteModificado.RFCBanco);
                        cmd.Parameters.AddWithValue("@NombreBanco", clienteModificado.NombreBanco);
                        cmd.Parameters.AddWithValue("@CFDIUsoId", clienteModificado.oCFDIUso.CFDIUsoId);
                        cmd.Parameters.AddWithValue("@CFDIMetodoPagoId", clienteModificado.oCFDIMetodoPago.CFDIMetodoPagoId);
                        cmd.Parameters.AddWithValue("@CFDIFormaPagoId", clienteModificado.oCFDIFormaPago.CFDIFormaPagoId);
                        cmd.Parameters.AddWithValue("@CFDIRegimenFiscalId", clienteModificado.oCFDIRegimenFiscal.CFDIRegimenFiscalId);
                        cmd.Parameters.AddWithValue("@Correo", clienteModificado.Correo);
                        cmd.Parameters.AddWithValue("@Comentarios", clienteModificado.Comentarios);
                        cmd.Parameters.AddWithValue("@PersonaUltimoCambio", clienteModificado.oUsuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@RFC", clienteModificado.RFC);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al editar el cliente: " + ex.Message;
            }
        }


        public bool Eliminar(string RFC, out string mensaje)
        {
            bool exito = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "DELETE FROM tClientes WHERE RFC = '" + RFC + "'";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        conexion.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            exito = true;
                        }
                        else
                        {
                            mensaje = "No se encontró el cliente con RFC: " + RFC;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar el cliente: " + ex.Message;
            }

            return exito;
        }

        public Cliente BuscarClientePorNombreCorto(string nombreCorto)
        {
            Cliente cliente = null;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT c.*, 
                u.Descripcion AS UsoDescripcion, 
                mp.Descripcion AS MetodoPagoDescripcion, 
                fp.Descripcion AS FormaPagoDescripcion, 
                rf.Descripcion AS RegimenFiscalDescripcion 
                FROM tClientes c
                LEFT JOIN tCFDIUso u ON c.CFDIUsoId = u.CFDIUsoId
                LEFT JOIN tCFDIMetodoPago mp ON c.CFDIMetodoPagoId = mp.CFDIMetodoPagoId
                LEFT JOIN tCFDIFormaPago fp ON c.CFDIFormaPagoId = fp.CFDIFormaPagoId
                LEFT JOIN tCFDIRegimenFiscal rf ON c.CFDIRegimenFiscalId = rf.CFDIRegimenFiscalId
                WHERE c.NombreCorto = @NombreCorto";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@NombreCorto", nombreCorto);
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cliente = new Cliente
                            {
                                RFC = dr["RFC"] != DBNull.Value ? dr["RFC"].ToString() : string.Empty,
                                Estatus = dr["Estatus"] != DBNull.Value ? dr["Estatus"].ToString() : string.Empty,
                                RazonSocial = dr["RazonSocial"] != DBNull.Value ? dr["RazonSocial"].ToString() : string.Empty,
                                NombreCorto = dr["NombreCorto"] != DBNull.Value ? dr["NombreCorto"].ToString() : string.Empty,
                                Calle = dr["Calle"] != DBNull.Value ? dr["Calle"].ToString() : string.Empty,
                                NumeroInterior = dr["NumeroInterior"] != DBNull.Value ? dr["NumeroInterior"].ToString() : string.Empty,
                                NumeroExterior = dr["NumeroExterior"] != DBNull.Value ? dr["NumeroExterior"].ToString() : string.Empty,
                                Colonia = dr["Colonia"] != DBNull.Value ? dr["Colonia"].ToString() : string.Empty,
                                Ciudad = dr["Ciudad"] != DBNull.Value ? dr["Ciudad"].ToString() : string.Empty,
                                Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : string.Empty,
                                Pais = dr["Pais"] != DBNull.Value ? dr["Pais"].ToString() : string.Empty,
                                CodigoPostal = dr["CodigoPostal"] != DBNull.Value ? dr["CodigoPostal"].ToString() : string.Empty,
                                Telefono = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : string.Empty,
                                Correo = dr["Correo"] != DBNull.Value ? dr["Correo"].ToString() : string.Empty,
                                Comentarios = dr["Comentarios"] != DBNull.Value ? dr["Comentarios"].ToString() : string.Empty,
                                Credito = dr["Credito"] != DBNull.Value && Convert.ToBoolean(dr["Credito"]),
                                LimiteCredito = dr["LimiteCredito"] != DBNull.Value ? Convert.ToDecimal(dr["LimiteCredito"]) : 0,
                                DiasCredito = dr["DiasCredito"] != DBNull.Value ? Convert.ToInt32(dr["DiasCredito"]) : 0,
                                CuentaPago = dr["CuentaPago"] != DBNull.Value ? dr["CuentaPago"].ToString() : string.Empty,
                                CuentaOrdenante = dr["CuentaOrdenante"] != DBNull.Value ? dr["CuentaOrdenante"].ToString() : string.Empty,
                                RFCBanco = dr["RFCBanco"] != DBNull.Value ? dr["RFCBanco"].ToString() : string.Empty,
                                NombreBanco = dr["NombreBanco"] != DBNull.Value ? dr["NombreBanco"].ToString() : string.Empty,
                                oCFDIUso = new CFDIUso
                                {
                                    CFDIUsoId = dr["CFDIUsoId"] != DBNull.Value ? dr["CFDIUsoId"].ToString() : string.Empty,
                                    Descripcion = dr["UsoDescripcion"] != DBNull.Value ? dr["UsoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIMetodoPago = new MetodoPago
                                {
                                    CFDIMetodoPagoId = dr["CFDIMetodoPagoId"] != DBNull.Value ? dr["CFDIMetodoPagoId"].ToString() : string.Empty,
                                    Descripcion = dr["MetodoPagoDescripcion"] != DBNull.Value ? dr["MetodoPagoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIFormaPago = new FormaPago
                                {
                                    CFDIFormaPagoId = dr["CFDIFormaPagoId"] != DBNull.Value ? dr["CFDIFormaPagoId"].ToString() : string.Empty,
                                    Descripcion = dr["FormaPagoDescripcion"] != DBNull.Value ? dr["FormaPagoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIRegimenFiscal = new RegimenFiscal
                                {
                                    CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"] != DBNull.Value ? dr["CFDIRegimenFiscalId"].ToString() : string.Empty,
                                    Descripcion = dr["RegimenFiscalDescripcion"] != DBNull.Value ? dr["RegimenFiscalDescripcion"].ToString() : string.Empty
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo adecuado de la excepción
                cliente = null;
            }
            return cliente;
        }


        public Cliente BuscarClientePorRFC(string rfc)
        {
            Cliente cliente = null;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT c.*, 
                u.Descripcion AS UsoDescripcion, 
                mp.Descripcion AS MetodoPagoDescripcion, 
                fp.Descripcion AS FormaPagoDescripcion, 
                rf.Descripcion AS RegimenFiscalDescripcion 
                FROM tClientes c
                LEFT JOIN tCFDIUso u ON c.CFDIUsoId = u.CFDIUsoId
                LEFT JOIN tCFDIMetodoPago mp ON c.CFDIMetodoPagoId = mp.CFDIMetodoPagoId
                LEFT JOIN tCFDIFormaPago fp ON c.CFDIFormaPagoId = fp.CFDIFormaPagoId
                LEFT JOIN tCFDIRegimenFiscal rf ON c.CFDIRegimenFiscalId = rf.CFDIRegimenFiscalId
                WHERE c.RFC = @RFC";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@RFC", rfc);
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cliente = new Cliente
                            {
                                RFC = dr["RFC"] != DBNull.Value ? dr["RFC"].ToString() : string.Empty,
                                Estatus = dr["Estatus"] != DBNull.Value ? dr["Estatus"].ToString() : string.Empty,
                                RazonSocial = dr["RazonSocial"] != DBNull.Value ? dr["RazonSocial"].ToString() : string.Empty,
                                NombreCorto = dr["NombreCorto"] != DBNull.Value ? dr["NombreCorto"].ToString() : string.Empty,
                                Calle = dr["Calle"] != DBNull.Value ? dr["Calle"].ToString() : string.Empty,
                                NumeroInterior = dr["NumeroInterior"] != DBNull.Value ? dr["NumeroInterior"].ToString() : string.Empty,
                                NumeroExterior = dr["NumeroExterior"] != DBNull.Value ? dr["NumeroExterior"].ToString() : string.Empty,
                                Colonia = dr["Colonia"] != DBNull.Value ? dr["Colonia"].ToString() : string.Empty,
                                Ciudad = dr["Ciudad"] != DBNull.Value ? dr["Ciudad"].ToString() : string.Empty,
                                Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : string.Empty,
                                Pais = dr["Pais"] != DBNull.Value ? dr["Pais"].ToString() : string.Empty,
                                CodigoPostal = dr["CodigoPostal"] != DBNull.Value ? dr["CodigoPostal"].ToString() : string.Empty,
                                Telefono = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : string.Empty,
                                Correo = dr["Correo"] != DBNull.Value ? dr["Correo"].ToString() : string.Empty,
                                Comentarios = dr["Comentarios"] != DBNull.Value ? dr["Comentarios"].ToString() : string.Empty,
                                Credito = dr["Credito"] != DBNull.Value && (bool)dr["Credito"],
                                LimiteCredito = dr["LimiteCredito"] != DBNull.Value ? (decimal)dr["LimiteCredito"] : 0,
                                DiasCredito = dr["DiasCredito"] != DBNull.Value ? (int)dr["DiasCredito"] : 0,
                                CuentaPago = dr["CuentaPago"] != DBNull.Value ? dr["CuentaPago"].ToString() : string.Empty,
                                CuentaOrdenante = dr["CuentaOrdenante"] != DBNull.Value ? dr["CuentaOrdenante"].ToString() : string.Empty,
                                RFCBanco = dr["RFCBanco"] != DBNull.Value ? dr["RFCBanco"].ToString() : string.Empty,
                                NombreBanco = dr["NombreBanco"] != DBNull.Value ? dr["NombreBanco"].ToString() : string.Empty,
                                oCFDIUso = new CFDIUso
                                {
                                    CFDIUsoId = dr["CFDIUsoId"] != DBNull.Value ? dr["CFDIUsoId"].ToString() : string.Empty,
                                    Descripcion = dr["UsoDescripcion"] != DBNull.Value ? dr["UsoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIMetodoPago = new MetodoPago
                                {
                                    CFDIMetodoPagoId = dr["CFDIMetodoPagoId"] != DBNull.Value ? dr["CFDIMetodoPagoId"].ToString() : string.Empty,
                                    Descripcion = dr["MetodoPagoDescripcion"] != DBNull.Value ? dr["MetodoPagoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIFormaPago = new FormaPago
                                {
                                    CFDIFormaPagoId = dr["CFDIFormaPagoId"] != DBNull.Value ? dr["CFDIFormaPagoId"].ToString() : string.Empty,
                                    Descripcion = dr["FormaPagoDescripcion"] != DBNull.Value ? dr["FormaPagoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIRegimenFiscal = new RegimenFiscal
                                {
                                    CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"] != DBNull.Value ? dr["CFDIRegimenFiscalId"].ToString() : string.Empty,
                                    Descripcion = dr["RegimenFiscalDescripcion"] != DBNull.Value ? dr["RegimenFiscalDescripcion"].ToString() : string.Empty
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
            }
            return cliente;
        }

        public Cliente UltimoRegistro()
        {
            Cliente cliente = new Cliente();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    // La consulta se ha actualizado para incluir los JOIN necesarios para obtener las descripciones de los campos relacionados.
                    string query = @"
                SELECT TOP 1 c.*, 
                u.Descripcion AS UsoDescripcion, 
                mp.Descripcion AS MetodoPagoDescripcion, 
                fp.Descripcion AS FormaPagoDescripcion, 
                rf.Descripcion AS RegimenFiscalDescripcion 
                FROM tClientes c
                LEFT JOIN tCFDIUso u ON c.CFDIUsoId = u.CFDIUsoId
                LEFT JOIN tCFDIMetodoPago mp ON c.CFDIMetodoPagoId = mp.CFDIMetodoPagoId
                LEFT JOIN tCFDIFormaPago fp ON c.CFDIFormaPagoId = fp.CFDIFormaPagoId
                LEFT JOIN tCFDIRegimenFiscal rf ON c.CFDIRegimenFiscalId = rf.CFDIRegimenFiscalId
                ORDER BY c.FechaUltimoCambio DESC";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cliente = new Cliente
                            {
                                RFC = dr["RFC"]?.ToString() ?? string.Empty,
                                Estatus = dr["Estatus"]?.ToString() ?? string.Empty,
                                RazonSocial = dr["RazonSocial"]?.ToString() ?? string.Empty,
                                NombreCorto = dr["NombreCorto"]?.ToString() ?? string.Empty,
                                Calle = dr["Calle"]?.ToString() ?? string.Empty,
                                NumeroInterior = dr["NumeroInterior"] != DBNull.Value ? dr["NumeroInterior"].ToString() : string.Empty,
                                NumeroExterior = dr["NumeroExterior"] != DBNull.Value ? dr["NumeroExterior"].ToString() : string.Empty,
                                Colonia = dr["Colonia"]?.ToString() ?? string.Empty,
                                Ciudad = dr["Ciudad"]?.ToString() ?? string.Empty,
                                Estado = dr["Estado"]?.ToString() ?? string.Empty,
                                Pais = dr["Pais"]?.ToString() ?? string.Empty,
                                CodigoPostal = dr["CodigoPostal"]?.ToString() ?? string.Empty,
                                Telefono = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : string.Empty,
                                Correo = dr["Correo"] != DBNull.Value ? dr["Correo"].ToString() : string.Empty,
                                Comentarios = dr["Comentarios"] != DBNull.Value ? dr["Comentarios"].ToString() : string.Empty,
                                Credito = dr["Credito"] != DBNull.Value ? Convert.ToBoolean(dr["Credito"]) : false,
                                LimiteCredito = dr["LimiteCredito"] != DBNull.Value ? Convert.ToDecimal(dr["LimiteCredito"]) : 0m,
                                DiasCredito = dr["DiasCredito"] != DBNull.Value ? Convert.ToInt32(dr["DiasCredito"]) : 0,
                                CuentaPago = dr["CuentaPago"] != DBNull.Value ? dr["CuentaPago"].ToString() : string.Empty,
                                CuentaOrdenante = dr["CuentaOrdenante"] != DBNull.Value ? dr["CuentaOrdenante"].ToString() : string.Empty,
                                RFCBanco = dr["RFCBanco"] != DBNull.Value ? dr["RFCBanco"].ToString() : string.Empty,
                                NombreBanco = dr["NombreBanco"] != DBNull.Value ? dr["NombreBanco"].ToString() : string.Empty,
                                oCFDIUso = new CFDIUso
                                {
                                    CFDIUsoId = dr["CFDIUsoId"] != DBNull.Value ? dr["CFDIUsoId"].ToString() : string.Empty,
                                    Descripcion = dr["UsoDescripcion"] != DBNull.Value ? dr["UsoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIMetodoPago = new MetodoPago
                                {
                                    CFDIMetodoPagoId = dr["CFDIMetodoPagoId"] != DBNull.Value ? dr["CFDIMetodoPagoId"].ToString() : string.Empty,
                                    Descripcion = dr["MetodoPagoDescripcion"] != DBNull.Value ? dr["MetodoPagoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIFormaPago = new FormaPago
                                {
                                    CFDIFormaPagoId = dr["CFDIFormaPagoId"] != DBNull.Value ? dr["CFDIFormaPagoId"].ToString() : string.Empty,
                                    Descripcion = dr["FormaPagoDescripcion"] != DBNull.Value ? dr["FormaPagoDescripcion"].ToString() : string.Empty
                                },
                                oCFDIRegimenFiscal = new RegimenFiscal
                                {
                                    CFDIRegimenFiscalId = dr["CFDIRegimenFiscalId"] != DBNull.Value ? dr["CFDIRegimenFiscalId"].ToString() : string.Empty,
                                    Descripcion = dr["RegimenFiscalDescripcion"] != DBNull.Value ? dr["RegimenFiscalDescripcion"].ToString() : string.Empty
                                },
                            };



                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar el error adecuadamente
                cliente = null;
            }
            return cliente;
        }



        public List<string> ElementosPaginacionBuscadorCliente(string nombre, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    // Puedes ajustar esta consulta para buscar en otras columnas relevantes
                    string query = @"SELECT NombreCorto FROM tClientes 
                             WHERE NombreCorto LIKE @Nombre OR RazonSocial LIKE @Nombre
                             ORDER BY NombreCorto DESC 
                             OFFSET @Pagina ROWS FETCH NEXT @Siguientes ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
                    cmd.Parameters.AddWithValue("@Pagina", pagina);
                    cmd.Parameters.AddWithValue("@Siguientes", siguientes);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(Convert.ToString(dr["NombreCorto"]));
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

        public int CountBuscadorCliente(string nombre)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT COUNT(*) AS TotalRegistros 
                             FROM tClientes 
                             WHERE NombreCorto LIKE @Nombre OR RazonSocial LIKE @Nombre";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
                    conexion.Open();
                    resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }
        public List<Cliente> ListarClienteTabla(int pagina, string tipoOrden, int siguientes)
        {
            List<Cliente> lista = new List<Cliente>();
            try
            {
                string orden = OrdenCliente(tipoOrden);
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $@"SELECT RFC, RazonSocial, Telefono, Correo
                              FROM tClientes
                              ORDER BY {orden} 
                              OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Cliente
                            {
                                RFC = dr["RFC"].ToString(),
                                RazonSocial = dr["RazonSocial"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Cliente>();
            }
            return lista;
        }

        public int CountTablaCliente()
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tClientes;";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }

        public List<Cliente> ListarClienteTablaWhere(int pagina, string tipoOrden, int siguientes, string where)
        {
            List<Cliente> lista = new List<Cliente>();
            try
            {
                string orden = OrdenCliente(tipoOrden);
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $@"SELECT c.RFC, c.RazonSocial,  c.Telefono, c.Correo
                              FROM tClientes c
                              WHERE {where} 
                              ORDER BY {orden} 
                              OFFSET {pagina * siguientes} ROWS FETCH NEXT {siguientes} ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Cliente
                            {
                                RFC = dr["RFC"].ToString(),
                                RazonSocial = dr["RazonSocial"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Cliente>();
            }
            return lista;
        }

        public int CountTablaClienteWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = $"SELECT COUNT(*) AS TotalRegistros FROM tClientes WHERE {where};";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }


        public List<Cliente> BuscarCliente(string nombre, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<Cliente> lista = new List<Cliente>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    // Puedes ajustar esta consulta para buscar en otras columnas relevantes
                    string query = @"SELECT RFC, RazonSocial FROM tClientes 
                             WHERE NombreCorto LIKE @Nombre OR RazonSocial LIKE @Nombre
                             ORDER BY NombreCorto DESC 
                             OFFSET @Pagina ROWS FETCH NEXT @Siguientes ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
                    cmd.Parameters.AddWithValue("@Pagina", pagina);
                    cmd.Parameters.AddWithValue("@Siguientes", siguientes);
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Cliente
                            {
                                RFC = dr["RFC"].ToString(),
                                RazonSocial = dr["RazonSocial"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Cliente>();
            }
            return lista;
        }


        public string OrdenCliente(string tipoOrden)
        {
            switch (tipoOrden)
            {
                case "R_A":
                    return "RFC ASC";
                case "R_D":
                    return "RFC DESC";
                case "N_A":
                    return "NombreCorto ASC";
                case "N_D":
                    return "NombreCorto DESC";
                default:
                    return "RFC ASC";
            }
        }



    }
}

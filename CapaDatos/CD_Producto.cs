using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace CapaDatos
{
    public class CD_Producto
    {

        public List<Producto> Listar()
        {

            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "Select p.IdProducto , p.Descripcion, p.Precio, p.Minimo,p.Maximo, m.IdMarca ,m.descripcion desm,l.IdLinea,l.descripcion desl,p.activo ,plc.valor from tproductos p inner join tmarcas m on m.IdMarca=p.IdMarca inner join tLineas l on l.IdLinea = p.IdLinea inner join tProductosLineasCaracteristicas plc on plc.Idproducto=p.IdProducto";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                Minimo = Convert.ToInt32(dr["Minimo"]),
                                Maximo = Convert.ToInt32(dr["Maximo"]),
                                oLinea  = new Linea {IdLinea= Convert.ToInt32(dr["IdLinea"]), Descripcion = Convert.ToString(dr["Desl"]) },
                                oMarca= new Marca {IdMarca= Convert.ToInt32(dr["IdMarca"]), Descripcion = Convert.ToString(dr["Desm"]) },
                                Activo = Convert.ToString(dr["Activo"]),
                                Valor = Convert.ToString(dr["Valor"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Producto>();
            }
            return lista;
        }
        public int Registrar(Producto obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarProducto", oconexion);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Precio", obj.Precio);
                    cmd.Parameters.AddWithValue("Minimo", obj.Minimo);
                    cmd.Parameters.AddWithValue("Maximo", obj.Maximo);
                    cmd.Parameters.AddWithValue("IdMarca", obj.oMarca.IdMarca);
                    cmd.Parameters.AddWithValue("IdLinea", obj.oLinea.IdLinea);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("Valor", obj.Valor);

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

        public bool Editar(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_EditarProducto", oconexion);
                    cmd.Parameters.AddWithValue("IdProducto", obj.IdProducto);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Precio", obj.Precio);
                    cmd.Parameters.AddWithValue("Minimo", obj.Minimo);
                    cmd.Parameters.AddWithValue("Maximo", obj.Maximo);
                    cmd.Parameters.AddWithValue("IdMarca", obj.oMarca.IdMarca);
                    cmd.Parameters.AddWithValue("IdLinea", obj.oLinea.IdLinea);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("Valor", obj.Valor);
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
                    SqlCommand cmd = new SqlCommand("sp_EliminarProducto", oconexion);
                    cmd.Parameters.AddWithValue("IdProducto", id);
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

        public bool GuardarDatosImagen(Producto oProducto, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "update producto set RutaImagen=@rutaimagen,NombreImagen=@nombreimagen where IdProducto =@idproducto";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("rutaimagen", oProducto);
                    cmd.Parameters.AddWithValue("nombreimagen", oProducto);
                    cmd.Parameters.AddWithValue("idproducto", oProducto);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        resultado = true;
                    }
                    else
                    {
                        Mensaje = "No se pudo actualizar la imagen";
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

    }
}

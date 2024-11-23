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
using static System.Net.Mime.MediaTypeNames;
using static System.Collections.Specialized.BitVector32;

namespace CapaDatos
{
    public class CD_Producto
    {

        public List<Producto> ListarProductos(int pagina, int siguientes, string orden)
        {
            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT p.NoParte, p.Descripcion, p.Precio, m.Descripcion AS Desm, l.Descripcion AS Desl " +
                                   "FROM tProductos p " +
                                   "INNER JOIN tMarcas m ON m.IdMarca = p.IdMarca " +
                                   "INNER JOIN tLineas l ON l.IdLinea = p.IdLinea " +
                                   "INNER JOIN tProductosLineasCaracteristicas plc ON plc.IdProducto = p.IdProducto " +
                                   "ORDER BY " + orden +
                                   " OFFSET " + pagina * siguientes + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                NoParte = Convert.ToString(dr["NoParte"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                oLinea = new Linea { Descripcion = Convert.ToString(dr["Desl"]) },
                                oMarca = new Marca { Descripcion = Convert.ToString(dr["Desm"]) }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente (lanzar o registrar)
                throw;
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
                    string query = "SELECT COUNT(NoParte) AS TotalRegistros FROM tProductos ";
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

        public List<Producto> ListarProductosWhere(int pagina, int siguientes, string orden, string where)
        {
            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT p.NoParte, p.Descripcion, p.Precio, m.Descripcion AS Desm, l.Descripcion AS Desl " +
                                   "FROM tProductos p " +
                                   "INNER JOIN tMarcas m ON m.IdMarca = p.IdMarca " +
                                   "INNER JOIN tLineas l ON l.IdLinea = p.IdLinea " +
                                   "INNER JOIN tProductosLineasCaracteristicas plc ON plc.IdProducto = p.IdProducto " +
                                   "WHERE " + where +
                                   " ORDER BY " + orden +
                                   " OFFSET " + pagina * siguientes + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                NoParte = Convert.ToString(dr["NoParte"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                oLinea = new Linea { Descripcion = Convert.ToString(dr["Desl"]) },
                                oMarca = new Marca { Descripcion = Convert.ToString(dr["Desm"]) }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente (lanzar o registrar)
                throw;
            }
            return lista;
        }
        //      COUNT Tabla
        public int countTablaWhere(string where)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros " +
                                   "FROM tProductos p " +
                                   "INNER JOIN tMarcas m ON m.IdMarca = p.IdMarca " +
                                   "INNER JOIN tLineas l ON l.IdLinea = p.IdLinea " +
                                   "INNER JOIN tProductosLineasCaracteristicas plc ON plc.IdProducto = p.IdProducto " +
                                   "WHERE " + where + ";";

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

        public List<string> ListarImagenes(int id)
        {

            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Imagen FROM tProductosImagenes where IdProducto =" + id;

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(Convert.ToBase64String((byte[])dr["Imagen"]));
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

        public int Registrar(Producto obj, out string Mensaje)
        {
            int idAutogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    // Verificar si el producto ya existe
                    string queryVerificar = "SELECT COUNT(IdProducto) FROM tProductos WHERE NoParte = @NoParte AND IdMarca =@IdMarca";
                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, conexion))
                    {
                        cmdVerificar.Parameters.AddWithValue("@NoParte", obj.NoParte);
                        cmdVerificar.Parameters.AddWithValue("@IdMarca", obj.oMarca.IdMarca);
                        int existe = (int)cmdVerificar.ExecuteScalar();
                        if (existe > 0)
                        {
                            Mensaje = "El producto ya existe con ese NoParte y esta Marca verfique la Marca o el NoParte.";
                            return 0;
                        }
                    }

                    // Insertar en tProductos incluyendo AlmacenId, RackId, y SeccionId
                    string queryProducto = @"
INSERT INTO tProductos 
(Descripcion, Precio, Minimo, Maximo, NoParte, CodigoBarras, IdMarca, IdLinea, Activo, FechaRegistro, PersonaUltimoCambio, PersonaRegistro, FechaUltimoCambio, AlmacenId, RackId, SeccionId) 
VALUES 
(@Descripcion, @Precio, @Minimo, @Maximo, @NoParte, @CodigoBarras, @IdMarca, @IdLinea, @Activo, GETDATE(), @PersonaUltimoCambio, @PersonaRegistro, GETDATE(), @AlmacenId, @RackId, @SeccionId);
SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmdProducto = new SqlCommand(queryProducto, conexion))
                    {
                        cmdProducto.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                        cmdProducto.Parameters.AddWithValue("@Precio", obj.Precio);
                        cmdProducto.Parameters.AddWithValue("@Minimo", obj.Minimo);
                        cmdProducto.Parameters.AddWithValue("@Maximo", obj.Maximo);
                        cmdProducto.Parameters.AddWithValue("@NoParte", obj.NoParte);
                        cmdProducto.Parameters.AddWithValue("@CodigoBarras", obj.CodigoBarras);
                        cmdProducto.Parameters.AddWithValue("@IdMarca", obj.oMarca.IdMarca);
                        cmdProducto.Parameters.AddWithValue("@IdLinea", obj.oLinea.IdLinea);
                        cmdProducto.Parameters.AddWithValue("@Activo", obj.Activo);
                        cmdProducto.Parameters.AddWithValue("@AlmacenId", obj.oAlmacen.AlmacenId);
                        cmdProducto.Parameters.AddWithValue("@RackId", obj.oRack.RackId);
                        cmdProducto.Parameters.AddWithValue("@SeccionId", obj.oSeccion.SeccionId);
                        cmdProducto.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);
                        cmdProducto.Parameters.AddWithValue("@PersonaRegistro", obj.oUsuario.IdUsuario);

                        // Ejecutar la inserción y obtener el ID autogenerado
                        idAutogenerado = Convert.ToInt32(cmdProducto.ExecuteScalar());
                    }


                    if (idAutogenerado > 0)
                    {
                        string queryCaracteristica = @"
INSERT INTO tProductosLineasCaracteristicas (IdProducto, IdLineaCaracteristica, Valor)
VALUES (@IdProducto, @IdLineaCaracteristica, @Valor);";

                        using (SqlCommand cmdCaracteristica = new SqlCommand(queryCaracteristica, conexion))
                        {
                            // Suponiendo que ya tienes definido el IdLineaCaracteristica y Valor para este producto
                            cmdCaracteristica.Parameters.AddWithValue("@IdProducto", idAutogenerado);
                            cmdCaracteristica.Parameters.AddWithValue("@Valor", obj.Valor); // Valor de la característica para este producto
                            cmdCaracteristica.Parameters.AddWithValue("@IdLineaCaracteristica", obj.oLinea.IdLinea); // Asegúrate de tener este valor

                            cmdCaracteristica.ExecuteNonQuery();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                idAutogenerado = 0; // En caso de error, asegurarse de que el ID autogenerado se resetee
            }

            return idAutogenerado; // Devolver el ID autogenerado o 0 en caso de error
        }


        public bool Editar(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string queryVerificar = "SELECT COUNT(*) FROM tProductos WHERE NoParte = @NoParte AND IdMarca =@IdMarca";
                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, conexion))
                    {
                        cmdVerificar.Parameters.AddWithValue("@NoParte", obj.NoParte);
                        cmdVerificar.Parameters.AddWithValue("@IdMarca", obj.oMarca.IdMarca);
                        int existe = (int)cmdVerificar.ExecuteScalar();
                        if (existe > 1)
                        {
                            Mensaje = "El producto ya existe con ese NoParte y esta Marca verfique la Marca o el NoParte.";
                            return false;
                        }
                    }

                    // Si no existe otro producto con la misma descripción, proceder con la actualización
                    string queryProducto = @"
            UPDATE tProductos SET
            Descripcion = @Descripcion,
            Precio = @Precio,
            Minimo = @Minimo,
            Maximo = @Maximo,
            NoParte = @NoParte,
            CodigoBarras = @CodigoBarras,
            IdMarca = @IdMarca,
            IdLinea = @IdLinea,
            Activo = @Activo,
            AlmacenId = @AlmacenId,
            RackId = @RackId,
            SeccionId = @SeccionId,
            FechaUltimoCambio = GETDATE(),
            PersonaUltimoCambio = @PersonaUltimoCambio
            WHERE IdProducto = @IdProducto";

                    using (SqlCommand cmdProducto = new SqlCommand(queryProducto, conexion))
                    {
                        cmdProducto.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                        cmdProducto.Parameters.AddWithValue("@Precio", obj.Precio);
                        cmdProducto.Parameters.AddWithValue("@Minimo", obj.Minimo);
                        cmdProducto.Parameters.AddWithValue("@Maximo", obj.Maximo);
                        cmdProducto.Parameters.AddWithValue("@NoParte", obj.NoParte);
                        cmdProducto.Parameters.AddWithValue("@CodigoBarras", obj.CodigoBarras);
                        cmdProducto.Parameters.AddWithValue("@IdMarca", obj.oMarca.IdMarca);
                        cmdProducto.Parameters.AddWithValue("@IdLinea", obj.oLinea.IdLinea);
                        cmdProducto.Parameters.AddWithValue("@Activo", obj.Activo);
                        cmdProducto.Parameters.AddWithValue("@AlmacenId", obj.oAlmacen.AlmacenId);
                        cmdProducto.Parameters.AddWithValue("@RackId", obj.oRack.RackId);
                        cmdProducto.Parameters.AddWithValue("@SeccionId", obj.oSeccion.SeccionId);
                        cmdProducto.Parameters.AddWithValue("@PersonaUltimoCambio", obj.oUsuario.IdUsuario);
                        cmdProducto.Parameters.AddWithValue("@IdProducto", obj.IdProducto);

                        cmdProducto.ExecuteNonQuery(); // Ejecutar la actualización
                        resultado = true; // La actualización fue exitosa
                    }

                    if (resultado) // Solo proceder si la actualización anterior fue exitosa
                    {
                        string queryActualizarCaracteristica = @"
        UPDATE tProductosLineasCaracteristicas
        SET Valor = @Valor
        WHERE IdProducto = @IdProducto ";

                        using (SqlCommand cmdActualizarCaracteristica = new SqlCommand(queryActualizarCaracteristica, conexion))
                        {
                            // Añadir los parámetros necesarios para la consulta
                            cmdActualizarCaracteristica.Parameters.AddWithValue("@IdProducto", obj.IdProducto);
                            cmdActualizarCaracteristica.Parameters.AddWithValue("@Valor", obj.Valor); // El nuevo valor para la característica

                            int filasAfectadas = cmdActualizarCaracteristica.ExecuteNonQuery();
                            resultado = true; // Puedes decidir si esto afecta el resultado de la operación de edición

                            if (filasAfectadas == 0)
                            {
                                // Si no se afectaron filas, podría significar que no se encontró la combinación de IdProducto y IdLineaCaracteristica
                                Mensaje = "No se pudo actualizar la característica del producto.";
                                resultado = false; // Puedes decidir si esto afecta el resultado de la operación de edición
                            }
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




        public bool Eliminar(int idProducto, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    // Iniciar una transacción para asegurar que todas las eliminaciones se realicen como una operación atómica
                    SqlTransaction transaccion = conexion.BeginTransaction();
                    try
                    {
                        // Eliminar todas las imágenes asociadas al producto
                        string queryEliminarImagenes = "DELETE FROM tProductosImagenes WHERE IdProducto = @IdProducto";
                        using (SqlCommand cmdEliminarImagenes = new SqlCommand(queryEliminarImagenes, conexion, transaccion))
                        {
                            cmdEliminarImagenes.Parameters.AddWithValue("@IdProducto", idProducto);
                            cmdEliminarImagenes.ExecuteNonQuery();
                        }

                        // Eliminar todas las características asociadas al producto
                        string queryEliminarCaracteristicas = "DELETE FROM tProductosLineasCaracteristicas WHERE IdProducto = @IdProducto";
                        using (SqlCommand cmdEliminarCaracteristicas = new SqlCommand(queryEliminarCaracteristicas, conexion, transaccion))
                        {
                            cmdEliminarCaracteristicas.Parameters.AddWithValue("@IdProducto", idProducto);
                            cmdEliminarCaracteristicas.ExecuteNonQuery();
                        }

                        // Eliminar el producto
                        string queryEliminarProducto = "DELETE FROM tProductos WHERE IdProducto = @IdProducto";
                        using (SqlCommand cmdEliminarProducto = new SqlCommand(queryEliminarProducto, conexion, transaccion))
                        {
                            cmdEliminarProducto.Parameters.AddWithValue("@IdProducto", idProducto);
                            int filasAfectadas = cmdEliminarProducto.ExecuteNonQuery();
                            if (filasAfectadas == 0)
                            {
                                throw new Exception("El producto no existe.");
                            }
                        }

                        // Si todo fue exitoso, comprometer la transacción
                        transaccion.Commit();
                        resultado = true;
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre un error, revertir la transacción
                        transaccion.Rollback();
                        throw ex; // Relanzar la excepción para manejarla en el bloque catch externo
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

        public void GuardarDatosImagen(int idProducto, List<string> listaImagenesBase64)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    foreach (var imagenBase64 in listaImagenesBase64)
                    {
                        // Convertir la cadena Base64 a un arreglo de bytes
                        byte[] imagenBinaria = Convert.FromBase64String(imagenBase64);

                        string queryInsertarImagen = @"
                    INSERT INTO tProductosImagenes (IdProducto, Imagen) 
                    VALUES (@IdProducto, @Imagen);";

                        using (SqlCommand cmd = new SqlCommand(queryInsertarImagen, conexion))
                        {
                            cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                            cmd.Parameters.AddWithValue("@Imagen", imagenBinaria);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("Error al guardar imágenes: " + ex.Message);
            }
        }

        public List<(string, int)> ListarImagenesProducto(int idProducto)
        {
            List<(string, int)> lista = new List<(string, int)>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT Imagen, IdProductosImagen FROM tProductosImagenes WHERE IdProducto = @IdProducto";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string imagenBase64 = Convert.ToBase64String((byte[])dr["Imagen"]);
                            int id = Convert.ToInt32(dr["IdProductosImagen"]);
                            lista.Add((imagenBase64, id));
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<(string, int)>();
            }
            return lista;
        }


        // Último Registro de Producto 
        public Producto UltimoRegistro()
        {
            Producto producto = new Producto();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    // Se ajusta la consulta para incluir IDs de Almacen, Rack, y Seccion, y el valor del producto
                    string query = @"
SELECT TOP 1 p.IdProducto,
              p.Descripcion AS ProductoDescripcion,
              p.Precio,
              p.Minimo,
              p.Maximo,
              p.CodigoBarras,
              p.NoParte,
              m.IdMarca,
              m.Descripcion AS MarcaDescripcion,
              l.IdLinea,
              l.Descripcion AS LineaDescripcion,
              a.AlmacenId,    -- Incluyendo ID de Almacen
              a.Ubicacion AS AlmacenDescripcion, 
              r.RackId,       -- Incluyendo ID de Rack
              r.Ubicacion AS RackDescripcion,    
              s.SeccionId,    -- Incluyendo ID de Seccion
              s.Ubicacion AS SeccionDescripcion,
              plc.Valor AS ProductoValor, -- Asumiendo que este es el valor relevante del producto
              p.Activo,
              p.FechaUltimoCambio  -- Incluyendo la fecha de último cambio
FROM tProductos p
INNER JOIN tMarcas m ON p.IdMarca = m.IdMarca
INNER JOIN tLineas l ON p.IdLinea = l.IdLinea
LEFT JOIN tProductosLineasCaracteristicas plc ON p.IdProducto = plc.IdProducto
LEFT JOIN tAlmacenes a ON p.AlmacenId = a.AlmacenId
LEFT JOIN tAlmacenRacks r ON p.RackId = r.RackId
LEFT JOIN tAlmacenesRacksSecciones s ON p.SeccionId = s.SeccionId
ORDER BY p.FechaUltimoCambio DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.CommandType = CommandType.Text;
                        oconexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                producto = new Producto
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                    Descripcion = Convert.ToString(dr["ProductoDescripcion"]),
                                    Precio = Convert.ToDecimal(dr["Precio"]),
                                    Minimo = Convert.ToInt32(dr["Minimo"]),
                                    Maximo = Convert.ToInt32(dr["Maximo"]),
                                    CodigoBarras = Convert.ToString(dr["CodigoBarras"]),
                                    NoParte = Convert.ToString(dr["NoParte"]),
                                    oMarca = new Marca
                                    {
                                        IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                        Descripcion = Convert.ToString(dr["MarcaDescripcion"])
                                    },
                                    oLinea = new Linea
                                    {
                                        IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                        Descripcion = Convert.ToString(dr["LineaDescripcion"])
                                    },
                                    oAlmacen = new Almacen
                                    {
                                        AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                        Ubicacion = Convert.ToString(dr["AlmacenDescripcion"])
                                    },
                                    oRack = new AlmacenRack
                                    {
                                        RackId = Convert.ToInt32(dr["RackId"]),
                                        Ubicacion = Convert.ToString(dr["RackDescripcion"])
                                    },
                                    oSeccion = new AlmacenRackSeccion
                                    {
                                        SeccionId = Convert.ToInt32(dr["SeccionId"]),
                                        Ubicacion = Convert.ToString(dr["SeccionDescripcion"])
                                    },
                                    Valor = Convert.ToString(dr["ProductoValor"]),
                                    Activo = Convert.ToString(dr["Activo"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente
                producto = null;
            }

            return producto;
        }




        /************BUSCADOR************/
        public List<Producto> elementosPaginacionBuscador(string linea, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT P.NoParte, M.Descripcion AS DescripcionMarca, M.IdMarca as id
                FROM tProductos P
                INNER JOIN tMarcas M ON P.IdMarca = M.IdMarca
                WHERE P.NoParte LIKE '%" + linea + @"%' 
                ORDER BY P.Descripcion DESC 
                OFFSET " + pagina + @" ROWS FETCH NEXT " + siguientes + @" ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                NoParte = Convert.ToString(dr["NoParte"]),
                                oMarca = new Marca { 
                                    Descripcion = Convert.ToString(dr["DescripcionMarca"]),
                                    IdMarca = Convert.ToInt32(dr["id"])
                                }
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

        //      COUNT
        public int countBuscador(string linea)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tProductos WHERE NoParte LIKE '%" + linea + "%' ;";

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
        public Producto BuscarProductoPorNombre(string nombre)
        {
            Producto producto = new Producto();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT p.IdProducto,
                   p.Descripcion AS ProductoDescripcion,
                   p.Precio,
                   p.Minimo,
                   p.Maximo,
                   p.CodigoBarras,
                   p.NoParte,
                   m.IdMarca,
                   m.Descripcion AS MarcaDescripcion,
                   l.IdLinea,
                   l.Descripcion AS LineaDescripcion,
                   a.AlmacenId,
                   a.Ubicacion AS AlmacenUbicacion,
                   r.RackId,
                   r.Ubicacion AS RackUbicacion,
                   s.SeccionId,
                   s.Ubicacion AS SeccionUbicacion,
                   plc.Valor AS ProductoValor,
                   p.Activo
            FROM tProductos p
            INNER JOIN tMarcas m ON p.IdMarca = m.IdMarca
            INNER JOIN tLineas l ON p.IdLinea = l.IdLinea
            LEFT JOIN tAlmacenes a ON p.AlmacenId = a.AlmacenId
            LEFT JOIN tAlmacenRacks r ON p.RackId = r.RackId
            LEFT JOIN tAlmacenesRacksSecciones s ON p.SeccionId = s.SeccionId
            LEFT JOIN tProductosLineasCaracteristicas plc ON p.IdProducto = plc.IdProducto
            WHERE p.NoParte LIKE = Nombre";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.Parameters.AddWithValue("@Nombre",  nombre );
                        cmd.CommandType = CommandType.Text;
                        oconexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                producto = new Producto
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                    Descripcion = Convert.ToString(dr["ProductoDescripcion"]),
                                    Precio = Convert.ToDecimal(dr["Precio"]),
                                    Minimo = Convert.ToInt32(dr["Minimo"]),
                                    Maximo = Convert.ToInt32(dr["Maximo"]),
                                    CodigoBarras = Convert.ToString(dr["CodigoBarras"]),
                                    NoParte = Convert.ToString(dr["NoParte"]),
                                    oMarca = new Marca
                                    {
                                        IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                        Descripcion = Convert.ToString(dr["MarcaDescripcion"])
                                    },
                                    oLinea = new Linea
                                    {
                                        IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                        Descripcion = Convert.ToString(dr["LineaDescripcion"])
                                    },
                                    oAlmacen = new Almacen
                                    {
                                        AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                        Ubicacion = Convert.ToString(dr["AlmacenUbicacion"])
                                    },
                                    oRack = new AlmacenRack
                                    {
                                        RackId = Convert.ToInt32(dr["RackId"]),
                                        Ubicacion = Convert.ToString(dr["RackUbicacion"])
                                    },
                                    oSeccion = new AlmacenRackSeccion
                                    {
                                        SeccionId = Convert.ToInt32(dr["SeccionId"]),
                                        Ubicacion = Convert.ToString(dr["SeccionUbicacion"])
                                    },
                                    Valor = Convert.ToString(dr["ProductoValor"]),
                                    Activo = Convert.ToString(dr["Activo"]),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                producto = new Producto();
            }

            return producto;
        }
        
        public Producto BuscarProductoPorNoParteYMarca(string nombre, string id)
        {
            Producto producto = new Producto();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT p.IdProducto,
                   p.Descripcion AS ProductoDescripcion,
                   p.Precio,
                   p.Minimo,
                   p.Maximo,
                   p.CodigoBarras,
                   p.NoParte,
                   m.IdMarca,
                   m.Descripcion AS MarcaDescripcion,
                   l.IdLinea,
                   l.Descripcion AS LineaDescripcion,
                   a.AlmacenId,
                   a.Ubicacion AS AlmacenUbicacion,
                   r.RackId,
                   r.Ubicacion AS RackUbicacion,
                   s.SeccionId,
                   s.Ubicacion AS SeccionUbicacion,
                   plc.Valor AS ProductoValor,
                   p.Activo
            FROM tProductos p
            INNER JOIN tMarcas m ON p.IdMarca = m.IdMarca
            INNER JOIN tLineas l ON p.IdLinea = l.IdLinea
            LEFT JOIN tAlmacenes a ON p.AlmacenId = a.AlmacenId
            LEFT JOIN tAlmacenRacks r ON p.RackId = r.RackId
            LEFT JOIN tAlmacenesRacksSecciones s ON p.SeccionId = s.SeccionId
            LEFT JOIN tProductosLineasCaracteristicas plc ON p.IdProducto = plc.IdProducto
            WHERE p.NoParte = @Nombre AND p.IdMarca =@id";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.Parameters.AddWithValue("@Nombre",  nombre );
                        cmd.Parameters.AddWithValue("@id",id );
                        cmd.CommandType = CommandType.Text;
                        oconexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                producto = new Producto
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                    Descripcion = Convert.ToString(dr["ProductoDescripcion"]),
                                    Precio = Convert.ToDecimal(dr["Precio"]),
                                    Minimo = Convert.ToInt32(dr["Minimo"]),
                                    Maximo = Convert.ToInt32(dr["Maximo"]),
                                    CodigoBarras = Convert.ToString(dr["CodigoBarras"]),
                                    NoParte = Convert.ToString(dr["NoParte"]),
                                    oMarca = new Marca
                                    {
                                        IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                        Descripcion = Convert.ToString(dr["MarcaDescripcion"])
                                    },
                                    oLinea = new Linea
                                    {
                                        IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                        Descripcion = Convert.ToString(dr["LineaDescripcion"])
                                    },
                                    oAlmacen = new Almacen
                                    {
                                        AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                        Ubicacion = Convert.ToString(dr["AlmacenUbicacion"])
                                    },
                                    oRack = new AlmacenRack
                                    {
                                        RackId = Convert.ToInt32(dr["RackId"]),
                                        Ubicacion = Convert.ToString(dr["RackUbicacion"])
                                    },
                                    oSeccion = new AlmacenRackSeccion
                                    {
                                        SeccionId = Convert.ToInt32(dr["SeccionId"]),
                                        Ubicacion = Convert.ToString(dr["SeccionUbicacion"])
                                    },
                                    Valor = Convert.ToString(dr["ProductoValor"]),
                                    Activo = Convert.ToString(dr["Activo"]),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                producto = new Producto();
            }

            return producto;
        }
        public Producto BuscarProductoPorNoParteYMarcaDescripcion(string nombre, string id)
        {
            Producto producto = new Producto();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT p.IdProducto,
                   p.Descripcion AS ProductoDescripcion,
                   p.Precio,
                   p.Minimo,
                   p.Maximo,
                   p.CodigoBarras,
                   p.NoParte,
                   m.IdMarca,
                   m.Descripcion AS MarcaDescripcion,
                   l.IdLinea,
                   l.Descripcion AS LineaDescripcion,
                   a.AlmacenId,
                   a.Ubicacion AS AlmacenUbicacion,
                   r.RackId,
                   r.Ubicacion AS RackUbicacion,
                   s.SeccionId,
                   s.Ubicacion AS SeccionUbicacion,
                   plc.Valor AS ProductoValor,
                   p.Activo
            FROM tProductos p
            INNER JOIN tMarcas m ON p.IdMarca = m.IdMarca
            INNER JOIN tLineas l ON p.IdLinea = l.IdLinea
            LEFT JOIN tAlmacenes a ON p.AlmacenId = a.AlmacenId
            LEFT JOIN tAlmacenRacks r ON p.RackId = r.RackId
            LEFT JOIN tAlmacenesRacksSecciones s ON p.SeccionId = s.SeccionId
            LEFT JOIN tProductosLineasCaracteristicas plc ON p.IdProducto = plc.IdProducto
            WHERE p.NoParte = @Nombre AND m.Descripcion =@id";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.Parameters.AddWithValue("@Nombre",  nombre );
                        cmd.Parameters.AddWithValue("@id",id );
                        cmd.CommandType = CommandType.Text;
                        oconexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                producto = new Producto
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                    Descripcion = Convert.ToString(dr["ProductoDescripcion"]),
                                    Precio = Convert.ToDecimal(dr["Precio"]),
                                    Minimo = Convert.ToInt32(dr["Minimo"]),
                                    Maximo = Convert.ToInt32(dr["Maximo"]),
                                    CodigoBarras = Convert.ToString(dr["CodigoBarras"]),
                                    NoParte = Convert.ToString(dr["NoParte"]),
                                    oMarca = new Marca
                                    {
                                        IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                        Descripcion = Convert.ToString(dr["MarcaDescripcion"])
                                    },
                                    oLinea = new Linea
                                    {
                                        IdLinea = Convert.ToInt32(dr["IdLinea"]),
                                        Descripcion = Convert.ToString(dr["LineaDescripcion"])
                                    },
                                    oAlmacen = new Almacen
                                    {
                                        AlmacenId = Convert.ToInt32(dr["AlmacenId"]),
                                        Ubicacion = Convert.ToString(dr["AlmacenUbicacion"])
                                    },
                                    oRack = new AlmacenRack
                                    {
                                        RackId = Convert.ToInt32(dr["RackId"]),
                                        Ubicacion = Convert.ToString(dr["RackUbicacion"])
                                    },
                                    oSeccion = new AlmacenRackSeccion
                                    {
                                        SeccionId = Convert.ToInt32(dr["SeccionId"]),
                                        Ubicacion = Convert.ToString(dr["SeccionUbicacion"])
                                    },
                                    Valor = Convert.ToString(dr["ProductoValor"]),
                                    Activo = Convert.ToString(dr["Activo"]),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                producto = new Producto();
            }

            return producto;
        }



        public bool EliminarIMG(int idImagen, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string queryEliminarImagen = "DELETE FROM tProductosImagenes WHERE IdProductosImagen = @IdImagen";

                    using (SqlCommand cmd = new SqlCommand(queryEliminarImagen, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdImagen", idImagen);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        resultado = filasAfectadas > 0; // Si se eliminó al menos una fila, la operación fue exitosa
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
        /************BUSCADOR  PRODUCTOS Descripcion************/
        public List<Producto> elementosBuscadorProductosDescripcion(string linea, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdProducto, Descripcion FROM tProductos WHERE Descripcion LIKE '%" + linea + "%' ORDER BY Descripcion DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
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
                                Descripcion = Convert.ToString(dr["Descripcion"])
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
        //      COUNT PRODUCTOS Descripcion
        public int countProductosDescripcion(string linea)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tProductos WHERE Descripcion LIKE '%" + linea + "%' ;";

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
        /************BUSCADOR  PRODUCTOS ID************/
        public List<Producto> elementosBuscadorProductosID(string linea, int pagina, int siguientes)
        {
            pagina = pagina * siguientes;
            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdProducto, Descripcion FROM tProductos WHERE IdProducto LIKE '%" + linea + "%' ORDER BY Descripcion DESC OFFSET " + pagina + " ROWS FETCH NEXT " + siguientes + " ROWS ONLY";
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
                                Descripcion = Convert.ToString(dr["Descripcion"])
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
        //      COUNT PRODUCTOS IdProducto
        public int countProductosID(string linea)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COUNT(*) AS TotalRegistros FROM tProductos WHERE IdProducto LIKE '%" + linea + "%' ;";

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


        public int RegistrarProductoProveedor(ProductoProveedor obj, out string Mensaje)
        {
            int idAutogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    // Verificar si el producto proveedor ya existe
                    string queryVerificar = "SELECT COUNT(*) FROM tProductosProveedores WHERE IdProducto = @IdProducto AND RFCProveedor = @RFCProveedor";
                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, conexion))
                    {
                        cmdVerificar.Parameters.AddWithValue("@IdProducto", obj.oProducto.IdProducto);
                        cmdVerificar.Parameters.AddWithValue("@RFCProveedor", obj.oProveedor.RFC);
                        int existe = (int)cmdVerificar.ExecuteScalar();
                        if (existe > 0)
                        {
                            Mensaje = "El producto proveedor ya existe";
                            return 0; // Salir del método ya que el producto proveedor existe
                        }
                    }

                    // Insertar en tProductosProveedores
                    string queryProductoProveedor = @"
                INSERT INTO tProductosProveedores 
                (IdProducto, RFCProveedor, Precio, Referencia, FechaRegistro, PersonaRegistro) 
                VALUES 
                (@IdProducto, @RFCProveedor, @Precio, @Referencia, GETDATE(), @PersonaRegistro);
                SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmdProductoProveedor = new SqlCommand(queryProductoProveedor, conexion))
                    {
                        cmdProductoProveedor.Parameters.AddWithValue("@IdProducto", obj.oProducto.IdProducto);
                        cmdProductoProveedor.Parameters.AddWithValue("@RFCProveedor", obj.oProveedor.RFC);
                        cmdProductoProveedor.Parameters.AddWithValue("@Precio", obj.Precio);
                        cmdProductoProveedor.Parameters.AddWithValue("@Referencia", obj.Referencia);
                        cmdProductoProveedor.Parameters.AddWithValue("@PersonaRegistro", obj.oUsuario.IdUsuario);

                        // Ejecutar la inserción y obtener el ID autogenerado
                        idAutogenerado = Convert.ToInt32(cmdProductoProveedor.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                idAutogenerado = 0; // En caso de error, asegurarse de que el ID autogenerado se resetee
            }

            return idAutogenerado; // Devolver el ID autogenerado o 0 en caso de error
        }

        public List<ProductoProveedor> SeleccionarProductoProveedor(int idProducto, out string Mensaje)
        {
            Mensaje = string.Empty;

            List<ProductoProveedor> listaProductosProveedores = new List<ProductoProveedor>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string query = @"SELECT pp.IdProductoProveedor, p.Descripcion AS DescripcionProducto, pp.RFCProveedor, pp.Precio, pp.Referencia
                             FROM tProductosProveedores pp
                             INNER JOIN tProductos p ON pp.IdProducto = p.IdProducto
                             WHERE pp.IdProducto = @IdProducto";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaProductosProveedores.Add(new ProductoProveedor
                                {
                                    IdProductoProveedor = Convert.ToInt32(reader["IdProductoProveedor"]),
                                    oProducto = new Producto { Descripcion = reader["DescripcionProducto"].ToString() },
                                    oProveedor = new Proveedor { RFC = reader["RFCProveedor"].ToString() },
                                    Precio = Convert.ToDecimal(reader["Precio"]),
                                    Referencia = reader["Referencia"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;

            }

            return listaProductosProveedores;
        }

        public bool EliminarProductoProveedor(int idProductoProveedor, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    // Iniciar una transacción para asegurar que todas las eliminaciones se realicen como una operación atómica
                    SqlTransaction transaccion = conexion.BeginTransaction();
                    try
                    {
                        // Eliminar el producto proveedor
                        string queryEliminarProductoProveedor = "DELETE FROM tProductosProveedores WHERE IdProductoProveedor = @IdProductoProveedor";
                        using (SqlCommand cmdEliminarProductoProveedor = new SqlCommand(queryEliminarProductoProveedor, conexion, transaccion))
                        {
                            cmdEliminarProductoProveedor.Parameters.AddWithValue("@IdProductoProveedor", idProductoProveedor);
                            int filasAfectadas = cmdEliminarProductoProveedor.ExecuteNonQuery();
                            if (filasAfectadas == 0)
                            {
                                throw new Exception("El producto proveedor no existe.");
                            }
                        }

                        // Si todo fue exitoso, comprometer la transacción
                        transaccion.Commit();
                        resultado = true;
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre un error, revertir la transacción
                        transaccion.Rollback();
                        throw ex; // Relanzar la excepción para manejarla en el bloque catch externo
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


        public int ObtenerIdConNoparteMarca(Producto producto)
        {
            int IdProducto = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
                SELECT P.IdProducto 
                FROM tProductos P
                INNER JOIN tMarcas M ON P.IdMarca = M.IdMarca
                WHERE P.NoParte = @NoParte AND M.Descripcion = @DescripcionMarca";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@NoParte", producto.NoParte);
                        cmd.Parameters.AddWithValue("@DescripcionMarca", producto.oMarca.Descripcion);

                        oconexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                IdProducto = Convert.ToInt32(dr["IdProducto"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el ID del producto.", ex);
            }

            return IdProducto;
        }

        /************BUSCADOR PENDIENTE************/
        public List<Producto> BuscarPendientes()
        {
            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"
            SELECT P.IdProducto, P.NoParte, P.Descripcion, M.Descripcion as DescripcionMarca
            FROM tProductos P
            INNER JOIN tMarcas M ON P.IdMarca = M.IdMarca
            INNER JOIN tProductosLineasCaracteristicas PLC ON P.IdProducto = PLC.IdProducto
            WHERE PLC.Valor = 'PENDIENTE'
            ORDER BY P.FechaRegistro ";

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
                                NoParte = Convert.ToString(dr["NoParte"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                oMarca = new Marca
                                {
                                    Descripcion = Convert.ToString(dr["DescripcionMarca"]),
                                }
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


        public bool RegistrarPendiente(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open(); // Abre la conexión antes de ejecutar la consulta

                    string queryActualizarCaracteristica = @"
                UPDATE tProductosLineasCaracteristicas
                SET Valor = @Valor
                WHERE IdProducto = @IdProducto ";

                    using (SqlCommand cmdActualizarCaracteristica = new SqlCommand(queryActualizarCaracteristica, conexion))
                    {
                        cmdActualizarCaracteristica.Parameters.AddWithValue("@IdProducto", obj.IdProducto);
                        cmdActualizarCaracteristica.Parameters.AddWithValue("@Valor", obj.Valor);

                        int filasAfectadas = cmdActualizarCaracteristica.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            Mensaje = "No se pudo actualizar la característica del producto.";
                            resultado = false;
                        }
                    }

                    // Cierra la conexión después de ejecutar la consulta
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public List<Producto> BuscarProductosPorCodigoBarras(string codigoBarras)
        {
            List<Producto> productos = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT p.IdProducto, p.NoParte, p.Descripcion, p.Precio, " +
                                   "m.Descripcion AS Desm " +
                                   "FROM tProductos p " +
                                   "INNER JOIN tMarcas m ON m.IdMarca = p.IdMarca " +
                                   "WHERE p.CodigoBarras = @CodigoBarras";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CodigoBarras", codigoBarras);
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            productos.Add(new Producto
                            {
                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                NoParte = Convert.ToString(dr["NoParte"]),
                                Descripcion = Convert.ToString(dr["Descripcion"]),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                oMarca = new Marca { Descripcion = Convert.ToString(dr["Desm"]) }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return productos;
        }



        public List<Producto> ExportarExcel(string query)
        {
            List<Producto> lista = new List<Producto>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        string queryLower = query.ToLower();
                        bool hasIdProducto = queryLower.Contains("idproducto");
                        bool hasDescripcion = queryLower.Contains("productodescripcion");
                        bool hasPrecio = queryLower.Contains("precio");
                        bool hasMinimo = queryLower.Contains("minimo");
                        bool hasMaximo = queryLower.Contains("maximo");
                        bool hasNoParte = queryLower.Contains("noparte");
                        bool hasCodigoBarras = queryLower.Contains("codigobarras");
                        bool hasLineaDescripcion = queryLower.Contains("lineadescripcion");
                        bool hasMarcaDescripcion = queryLower.Contains("marcadescripcion");
                        bool hasActivo = queryLower.Contains("activo");
                        bool hasAlmacenDescripcion = queryLower.Contains("almacendescripcion");
                        bool hasRackDescripcion = queryLower.Contains("rackdescripcion");
                        bool hasSeccionDescripcion = queryLower.Contains("secciondescripcion");
                        bool hasCantidadDisponible = queryLower.Contains("cantidaddisponible");

                        while (dr.Read())
                        {
                            var producto = new Producto();

                            if (hasIdProducto)
                                producto.IdProducto = Convert.ToInt32(dr["IdProducto"]);

                            if (hasDescripcion)
                                producto.Descripcion = Convert.ToString(dr["ProductoDescripcion"]);

                            if (hasPrecio)
                                producto.Precio = Convert.ToDecimal(dr["Precio"]);

                            if (hasMinimo)
                                producto.Minimo = Convert.ToInt32(dr["Minimo"]);

                            if (hasMaximo)
                                producto.Maximo = Convert.ToInt32(dr["Maximo"]);

                            if (hasNoParte)
                                producto.NoParte = Convert.ToString(dr["NoParte"]);

                            if (hasCodigoBarras)
                                producto.CodigoBarras = Convert.ToString(dr["CodigoBarras"]);

                            if (hasLineaDescripcion)
                                producto.oLinea = new Linea { Descripcion = Convert.ToString(dr["LineaDescripcion"]) };

                            if (hasMarcaDescripcion)
                                producto.oMarca = new Marca { Descripcion = Convert.ToString(dr["MarcaDescripcion"]) };

                            if (hasActivo)
                                producto.Activo = Convert.ToString(dr["Activo"]);

                            if (hasAlmacenDescripcion)
                                producto.oAlmacen = new Almacen { Descripcion = Convert.ToString(dr["AlmacenDescripcion"]) };

                            if (hasRackDescripcion)
                                producto.oRack = new AlmacenRack { Descripcion = Convert.ToString(dr["RackDescripcion"]) };

                            if (hasSeccionDescripcion)
                                producto.oSeccion = new AlmacenRackSeccion { Descripcion = Convert.ToString(dr["SeccionDescripcion"]) };

                            if (hasCantidadDisponible) 
                                producto.oUsuario = new Usuario { IdUsuario = Convert.ToInt16(dr["CantidadDisponible"]) };


                            lista.Add(producto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw;
            }
            return lista;
        }


        public List<string>NombreColumnasExcel ()
        {

            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tProductos' AND COLUMN_NAME <> 'IdMarca' AND COLUMN_NAME <> 'IdLinea' AND COLUMN_NAME <> 'FechaRegistro' AND COLUMN_NAME <> 'PersonaUltimoCambio' AND COLUMN_NAME <> 'PersonaRegistro' AND COLUMN_NAME <> 'FechaUltimoCambio' AND COLUMN_NAME <> 'AlmacenId' AND COLUMN_NAME <> 'RackId' AND COLUMN_NAME <> 'SeccionId' \r\n";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(Convert.ToString(dr["COLUMN_NAME"]));
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

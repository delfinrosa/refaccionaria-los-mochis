using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Producto
    {
        private CD_Producto ObjCapaDatos = new CD_Producto();
        public List<Producto > Listar()
        {
            return ObjCapaDatos.Listar();
        }

        public int Registrar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de el Producto no puede ser vacio";
            }
            else if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de el Producto no puede ser vacio";
            }
            else if (obj.oMarca.IdMarca == 0)
            {
                Mensaje = "La debe de seleccionar una marca para el Producto";
            }
            else if (obj.oLinea.IdLinea == 0)
            {
                Mensaje = "La debe de seleccionar una categoria para el Producto";
            }
            else if (obj.Precio == 0)
            {
                Mensaje = "Ingesar el Precio de el Producto";
            }
            else if (obj.Minimo == 0)
            {
                Mensaje = "Ingesar el Minimo de el Producto";
            }            
            else if (obj.Maximo== 0)
            {
                Mensaje = "Ingesar el Maximo de el Producto";
            }            
            else if (obj.Valor== "")
            {
                Mensaje = "Ingesar el Valor de el Producto";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                return ObjCapaDatos.Registrar(obj, out Mensaje);
            }
            else
            {
                Mensaje = "No se puede editar la Producto";
                return 0;
            }
        }

        public bool Editar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            
            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de el Producto no puede ser vacio";
            }
            else if (obj.oMarca.IdMarca == 0)
            {
                Mensaje = "La debe de seleccionar una marca para el Producto";
            }
            else if (obj.oLinea.IdLinea== 0)
            {
                Mensaje = "La debe de seleccionar una categoria para el Producto";
            }
            else if (obj.Precio == 0)
            {
                Mensaje = "Ingesar el Precio de el Producto";
            }
            else if (obj.Minimo == 0)
            {
                Mensaje = "Ingesar el Minimo de el Producto";
            }            
            else if (obj.Maximo == 0)
            {
                Mensaje = "Ingesar el Maximo de el Producto";
            }            
            else if (obj.Valor == "")
            {
                Mensaje = "Ingesar el Valor de el Producto";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                return ObjCapaDatos.Editar(obj, out Mensaje);
            }
            else
            {
                return false;
            }
        }
        public bool Eliminar(int id, out string Mensaje)
        {
            return ObjCapaDatos.Eliminar(id, out Mensaje);
        }
        public void imagen(string direccion)
        {
            byte[] imageBytes = File.ReadAllBytes(direccion);
        }


    }
}

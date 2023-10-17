using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Usuario
    {
        private CD_Usuario ObjCapaDatos = new CD_Usuario();
        public List<Usuario> Listar()
        {
            return ObjCapaDatos.Listar();
        }        
        public Usuario Verificacion(string correo,string clave)
        {
            return ObjCapaDatos.Verificacion(correo,clave);
        }
        public int Registrar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "La Nombre de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(obj.Contraseña) || string.IsNullOrWhiteSpace(obj.Contraseña))
            {
                Mensaje = "La Contraseña de caracteristicas de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "La Correo de caracteristicas de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(obj.Tipo) || string.IsNullOrWhiteSpace(obj.Tipo))
            {
                Mensaje = "La Tipo de caracteristicas de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                obj.Contraseña = Recursos.ConvertirSha256(obj.Contraseña);
                obj.Contraseña= obj.Contraseña.ToUpper();
                return ObjCapaDatos.Registrar(obj, out Mensaje);
            }
            else
            {
                Mensaje = "No se puede editar la linea";
                return 0;
            }
        }
        public bool Editar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "La Nombre de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(obj.Contraseña) || string.IsNullOrWhiteSpace(obj.Contraseña))
            {
                Mensaje = "La Contraseña de caracteristicas de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "La Correo de caracteristicas de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(obj.Tipo) || string.IsNullOrWhiteSpace(obj.Tipo))
            {
                Mensaje = "La Tipo de caracteristicas de la linea no puede ser vacio";
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
    }
}

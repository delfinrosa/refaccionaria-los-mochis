using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Linea
    {

        private CD_Linea ObjCapaDatos = new CD_Linea();
        public List<Linea> Listar()
        {
            return ObjCapaDatos.Listar();
        }
        public int Registrar(Linea obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de la linea no puede ser vacio";
            }          
            if (string.IsNullOrEmpty(obj.Deslc) || string.IsNullOrWhiteSpace(obj.Deslc))
            {
                Mensaje = "La descripcion de caracteristicas de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                return ObjCapaDatos.Registrar(obj, out Mensaje);
            }
            else
            {
                Mensaje = "No se puede editar la linea";
                return 0;
            }
        }
        public bool Editar(Linea obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de la linea no puede ser vacio";
            }
            if (string.IsNullOrEmpty(obj.Deslc) || string.IsNullOrWhiteSpace(obj.Deslc))
            {
                Mensaje = "La descripcion de caracteristicas de la linea no puede ser vacio";
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


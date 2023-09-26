using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;
namespace CapaNegocio
{
    public class CN_Marca
    {

        private CD_Marca ObjCapaDatos = new CD_Marca();
        public List<Marca> Listar()
        {
            return ObjCapaDatos.Listar();
        }
        public int Registrar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de la marca no puede ser vacio";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                return ObjCapaDatos.Registrar(obj, out Mensaje);
            }
            else
            {
                Mensaje = "No se puede editar la marca";
                return 0;
            }
        }
        public bool Editar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de la marca no puede ser vacio";
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

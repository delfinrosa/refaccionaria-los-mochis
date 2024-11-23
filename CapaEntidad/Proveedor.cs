using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    //public class Proveedor
    //{
    //    public int IdProveedor { get; set; }
    //    public string RFC { get; set; }
    //    public string RazonSocial { get; set; }
    //    public string Calle { get; set; }
    //    public string Estado { get; set; }
    //    public string Pais { get; set; }
    //    public string CP { get; set; }
    //    public string NumeroInt { get; set; }
    //    public string NumeroExt { get; set; }
    //    public string Telefono { get; set; }
    //    public string Correo { get; set; }
    //}
    public class Proveedor
    {
        public string RFC { get; set; }
        public string Estatus { get; set; }
        public string RazonSocial { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string NumeroInt { get; set; }
        public string NumeroExt { get; set; }
        public string Pais { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string CP { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Comentario { get; set; }
        public Usuario oUsuario { get; set; }

    }

}

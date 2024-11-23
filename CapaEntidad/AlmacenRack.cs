using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class AlmacenRack
    {
        public int RackId { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public Almacen oAlmacen { get;set; }
        public Usuario oUsuario { get; set; }

    }

}

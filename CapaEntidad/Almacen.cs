using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Almacen
    {
        public int AlmacenId { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public Usuario oUsuario{ get; set; }

    }

}

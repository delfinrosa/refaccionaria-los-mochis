using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace CapaEntidad
{
    public class AlmacenRackSeccion
    {
        public Almacen oAlmacen { get; set; } 
        public AlmacenRack oRack { get; set; } 
        public Usuario oUsuario { get; set; } 

        public int SeccionId { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }


    }

}

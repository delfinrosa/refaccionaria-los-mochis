using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Linea
    {
        public int IdLinea { get; set; }
        public string Descripcion { get; set; }
        public string Activo { get; set; }
        public string Deslc { get; set; }
        public Usuario oUsuario { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class MantenimientoEquipo
    {
        public int IdMantenimiento { get; set; } // Llave primaria
        public bool Limpieza { get; set; }
        public bool Fallo { get; set; }
        public string Observacion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public Usuario Usuario { get; set; } // Relación con tUsuarios
        public Equipo Equipo { get; set; } // Relación con tEquipo
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Compra
    {
        public string CompraId { get; set; }
        public string Estatus { get; set; }
        public string Fecha { get; set; }
        public Usuario UsuarioAutorizo { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public Usuario UsuarioCancelo { get; set; }
        public DateTime FechaCancelacion { get; set; }
        public string MotivoCancelacion { get; set; }
        public Usuario UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }

}

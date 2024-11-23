using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Ticket
    {
        public string Descripcion { get; set; }
        public string NoParte { get; set; }
        public string MarcaDescripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Vendedor { get; set; }
        public string NombreCliente { get; set; }
        public DateTime Fecha { get; set; }
    }

}

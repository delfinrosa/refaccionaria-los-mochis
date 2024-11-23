using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Venta
    {
        public string IdVenta { get; set; }
        public string RFCCliente { get; set; }
        public Usuario oVendedor { get; set; }
        public Usuario oCajero { get; set; } 
        public string Estatus { get; set; }
        public string TipoPago { get; set; }
        public string NombreCliente { get; set; }
        public decimal Cambio { get; set; } 
        public bool RequiereFactura { get; set; }

        public string Fecha { get; set; }
    }

}

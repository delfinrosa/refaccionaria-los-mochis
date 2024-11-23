using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class DetalleOrdenCompra
    {
        public string Proveedor { get; set; } 
        public string DescripcionProducto { get; set; } 
        public string NoParte { get; set; }
        public int Cantidad { get; set; } 
        public decimal PrecioUnitario { get; set; } 
        public decimal Subtotal => Cantidad * PrecioUnitario; 
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class VentasProductos
    {
        public string IdVentasProducto { get; set; }
        public Venta oVenta { get; set; }
        public Producto oProducto { get; set; }
        public string Cantidad { get; set; }
        public decimal Precio { get; set; }
    }

}

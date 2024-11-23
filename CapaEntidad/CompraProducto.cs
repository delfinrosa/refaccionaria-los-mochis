using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class CompraProducto
    {
        public int IdCompraProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public Compra Compra { get; set; }
        public Producto Producto { get; set; }
        public Usuario Usuario { get; set; }
    }

}

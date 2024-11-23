using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ProductoProveedor
    {
        public int IdProductoProveedor { get; set; }
        public Producto oProducto { get; set; }
        public Proveedor oProveedor{ get; set; }
        public decimal Precio { get; set; }
        public string Referencia { get; set; }

        public Usuario oUsuario { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Minimo{ get; set; }
        public int Maximo { get; set; }
        public Marca oMarca { get; set; }
        public Linea oLinea { get; set; }     
        public string Valor { get; set; }
        public string Activo { get; set; }

        public string img { get; set;}
    }
}

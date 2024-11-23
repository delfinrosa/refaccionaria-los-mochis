using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
        public Almacen oAlmacen { get; set; }
        public AlmacenRack oRack { get; set; }
        public AlmacenRackSeccion oSeccion { get; set; }
        public Usuario oUsuario { get; set; }
        public string NoParte { get; set; } 
        public string CodigoBarras { get; set; } 
    }
}

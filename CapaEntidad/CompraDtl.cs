using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class CompraDtl
    {
        public string CompraDtlId { get; set; } 
        public Compra Compra { get; set; }
        public ProductoProveedor oProductoProveedor { get; set; }
        public double Precio { get; set; }
        public int Cantidad { get; set; }
        public int CantidadEntrada { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaEstimadaEntrega { get; set; } 
        public DateTime FechaEntrega { get; set; } 
        public Usuario UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; } 
    }
}

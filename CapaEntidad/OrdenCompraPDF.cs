using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class OrdenCompraPDF
    {
        public string IdCompra { get; set; } 
        public string Usuario { get; set; } 
        public string Fecha { get; set; } 
        public string Estatus { get; set; } 
        public List<DetalleOrdenCompra> Detalles { get; set; }
        public string CodigoQRBase64 { get; set; } 

    }

}

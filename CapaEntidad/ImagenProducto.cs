using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ImagenProducto
    {
        public int IdImagenProducto { get; set; }
        public string Imagen { get; set; }
        public Producto oProducto { get; set; }
        public Usuario oUsuario { get; set; }


    }
}

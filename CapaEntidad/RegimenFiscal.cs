using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class RegimenFiscal
    {
        public string CFDIRegimenFiscalId { get; set; }
        public string Estatus { get; set; }
        public string Descripcion { get; set; }
        public Usuario oUsuario { get; set; }

    }
}

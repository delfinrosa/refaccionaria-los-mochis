﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class FormaPago
    {
        public string CFDIFormaPagoId { get; set; }
        public string Descripcion { get; set; }
        public string Estatus { get; set; }
        public Usuario oUsuario { get; set; }

    }
}

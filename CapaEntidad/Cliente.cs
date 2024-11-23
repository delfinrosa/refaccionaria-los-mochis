using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Cliente
    {
        public string RFC { get; set; }
        public string Estatus { get; set; }
        public string RazonSocial { get; set; }
        public string NombreCorto { get; set; }
        public string Calle { get; set; }
        public string NumeroInterior { get; set; }
        public string NumeroExterior { get; set; }
        public string Colonia { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public bool Credito { get; set; }
        public decimal LimiteCredito { get; set; }
        public int DiasCredito { get; set; }
        public string CuentaPago { get; set; }
        public string CuentaOrdenante { get; set; }
        public string RFCBanco { get; set; }
        public string NombreBanco { get; set; }
        public CFDIUso oCFDIUso { get; set; }
        public MetodoPago oCFDIMetodoPago { get; set; }
        public FormaPago oCFDIFormaPago { get; set; }
        public RegimenFiscal oCFDIRegimenFiscal { get; set; }
        public string Correo { get; set; }
        public string Comentarios { get; set; }
public Usuario oUsuario { get; set; }

    }



}

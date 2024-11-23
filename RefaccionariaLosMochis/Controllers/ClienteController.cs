using CapaDatos;
using CapaEntidad;
using Microsoft.AspNetCore.Mvc;
using RefaccionariaLosMochis.Permisos;
using Microsoft.AspNetCore.Authorization;

namespace RefaccionariaLosMochis.Controllers
{
    [Authorize]

    public class ClienteController : Controller
    {
        // GET: Cliente
        [PermisosRol("I")]

        public ActionResult Cliente()
        {
            //Usuario usuario = HttpContext.Session.GetObject<Usuario>("Usuario");

            //if (usuario != null)
            //{
            //    // Agregar cookies primero
            //    Response.Cookies.Append("tipo", usuario.Tipo.ToString());
            //    Response.Cookies.Append("idUsuario", usuario.IdUsuario.ToString());

            //    // Establecer ViewBag.tipo
            //    ViewBag.tipo = usuario.Tipo;
            //}
            //else
            //{
            //    // Manejo de sesión expirada o sin usuario
            //    return RedirectToAction("Index", "Acceso");
            //}

            return View();
        }
        [HttpPost]
        // Guardar
        public JsonResult GuardarCliente(Cliente objeto, bool boolGuaradar)
        {
            object resultado;
            string mensaje = string.Empty;
                objeto.oUsuario.IdUsuario = Convert.ToInt32(Request.Cookies["idUsuario"] ?? "0");

            if (boolGuaradar)
            {
                int a = new CD_Cliente().Registrar(objeto, out mensaje);
            }
            else
            {
                new CD_Cliente().Editar(objeto, out mensaje);
            }
            return Json(new { mensaje = mensaje });
        }

        // BUSCADOR CFDIUso DESCRIPCION
        public JsonResult BuscadorCFDIUsoDescripcionID(string descripcion, int pagina, int siguientes)
        {
            List<CFDIUso> data = new List<CFDIUso>();
            data = new CD_CFDIUso().elementosPaginacionBuscadorDescripcionID(descripcion, pagina, siguientes);
            return Json(new { data = data });
        }        
        // BUSCADOR CFDIUso DESCRIPCION
        public JsonResult BuscadorCFDIUsoIDDescripcion(string descripcion, int pagina, int siguientes)
        {
            List<CFDIUso> data = new List<CFDIUso>();
            data = new CD_CFDIUso().elementosPaginacionBuscadorIDDescripcion(descripcion, pagina, siguientes);
            return Json(new { data = data });
        }

        // COUNT BUSCADOR CFDIUso DESCRIPCION
        public JsonResult countCFDIUsoWhere(string where)
        {
            int registros = new CD_CFDIUso().countCFDIUsoWhere(where);
            return Json(new { registros = registros });
        }

        // BUSCADOR Método Pago por Descripción
        [HttpPost]
        public JsonResult BuscadorMetodoPagoDescripcion(string descripcion, int pagina, int siguientes)
        {
            var data =new CD_MetodoPago().BuscarMetodoPagoPorDescripcion(descripcion, pagina, siguientes);
            return Json(new { data = data });
        }

        // BUSCADOR Método Pago por ID
        [HttpPost]
        public JsonResult BuscadorMetodoPagoID(string id, int pagina, int siguientes)
        {
            var data = new CD_MetodoPago().BuscarMetodoPagoPorID(id, pagina, siguientes);
            return Json(new { data = data });
        }

        // COUNT para el buscador de Método Pago
        [HttpPost]
        public JsonResult ContarMetodosPago(string condicion)
        {
            var registros =new CD_MetodoPago().ContarMetodosPago(condicion);
            return Json(new { registros = registros });
        }



        // Acción para registrar o editar una forma de pago
        [HttpPost]
        public JsonResult GuardarFormaPago(FormaPago formaPago, bool esNuevo)
        {
            var resultado = false;
            string mensaje = "";

            try
            {
                if (esNuevo)
                {
                    // Llamada a la capa de acceso a datos para registrar
                    resultado = new CD_FormaPago().Registrar(formaPago, out mensaje);
                }
                else
                {
                    // Llamada a la capa de acceso a datos para editar
                    resultado = new CD_FormaPago().Editar(formaPago, out mensaje);
                }
            }
            catch (Exception ex)
            {
                mensaje = "Ocurrió un error: " + ex.Message;
            }

            return Json(new { resultado = resultado, mensaje = mensaje });
        }

        // Acción para buscar formas de pago por descripción
        [HttpPost]
        // GET: FormaPago/BuscarPorDescripcion
        public ActionResult BuscarFormasPagoDescripcion(string descripcion, int pagina, int cantidadPorPagina)
        {
            var cdFormaPago = new CD_FormaPago();
            var lista = cdFormaPago.BuscarFormaPagoPorDescripcion(descripcion, pagina, cantidadPorPagina);
            return Json(new { data = lista });
        }

        // GET: FormaPago/BuscarPorId
        public ActionResult BuscarFormasPagoId(string id, int pagina, int cantidadPorPagina)
        {
            var cdFormaPago = new CD_FormaPago();
            var lista = cdFormaPago.BuscarFormaPagoPorId(id, pagina, cantidadPorPagina);
            return Json(new { data = lista });
        }

        // GET: FormaPago/ContarFormasPago
        public ActionResult ContarFormasPago(string where)
        {
            var cdFormaPago = new CD_FormaPago();
            var total = cdFormaPago.ContarFormasPago(where);
            return Json(new { registros = total });
        }

        [HttpPost]
        public JsonResult BuscadorRegimenFiscalDescripcion(string descripcion, int pagina, int siguientes)
        {
            var data = new CD_RegimenFiscal().BuscarRegimenFiscalPorDescripcion(descripcion, pagina, siguientes);
            return Json(new { data = data });
        }

        [HttpPost]
        public JsonResult BuscadorRegimenFiscalID(string id, int pagina, int siguientes)
        {
            var data = new CD_RegimenFiscal().BuscarRegimenFiscalPorID(id, pagina, siguientes);
            return Json(new { data = data });
        }

        [HttpPost]
        public JsonResult ContarRegimenesFiscales(string condicion)
        {
            var registros = new CD_RegimenFiscal().ContarRegimenesFiscales(condicion);
            return Json(new { registros = registros });
        }
        [HttpPost]
        public JsonResult tamaño()
        {
            var campos = new Recursos().Tamaño("tClientes");
            return Json(new { campos = campos });

        }
        [HttpPost]
        public JsonResult tamañoCFDIUso()
        {
            var campos = new Recursos().Tamaño("tCFDIUso");
            return Json(new { campos = campos });

        }
        [HttpPost]
        public JsonResult tamañoCFDIFormaPago()
        {
            var campos = new Recursos().Tamaño("tCFDIFormaPago");
            return Json(new { campos = campos });

        }
        [HttpPost]
        public JsonResult tamañoCFDIMetodoPago()
        {
            var campos = new Recursos().Tamaño("tCFDIMetodoPago");
            return Json(new { campos = campos });

        }
        [HttpPost]
        public JsonResult tamañoCFDIRegimenFiscal()
        {
            var campos = new Recursos().Tamaño("tCFDIRegimenFiscal");
            return Json(new { campos = campos });

        }
        //Resultado un objeto que es el último modificado o agregado
        [HttpPost]
        public JsonResult UltimoRegistroCliente()
        {
            Cliente oCliente = new Cliente();
            oCliente = new CD_Cliente().UltimoRegistro();
            return Json(new { Lista = oCliente });
        }
        [HttpPost]
        public JsonResult CountTablaCliente()
        {
            int registros = 0;
            registros = new CD_Cliente().CountTablaCliente(); // Asegúrate de tener este método en tu capa de acceso a datos
            return Json(new { registros = registros });
        }

        [HttpPost]
        public JsonResult ListarClienteTabla(string strpagina, string tipoOrden, int siguientes)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Cliente> oLista = new List<Cliente>();
            oLista = new CD_Cliente().ListarClienteTabla(pagina, tipoOrden, siguientes); // Asegúrate de tener este método en tu capa de acceso a datos
            return Json(new { data = oLista });
        }

        [HttpPost]
        public JsonResult ListarClientesTablaWhere(string strpagina, string tipoOrden, int siguientes, string where)
        {
            int pagina = Convert.ToInt32(strpagina);
            List<Cliente> oLista = new List<Cliente>();
            oLista = new CD_Cliente().ListarClienteTablaWhere(pagina, tipoOrden, siguientes, where); // Asegúrate de tener este método en tu capa de acceso a datos
            return Json(new { data = oLista });
        }

        [HttpPost]
        public JsonResult CountTablaClienteWhere(string where)
        {
            int registros = 0;
            registros = new CD_Cliente().CountTablaClienteWhere(where); // Asegúrate de tener este método en tu capa de acceso a datos
            return Json(new { registros = registros });
        }

        [HttpPost]
        public JsonResult CountBuscadorCliente(string nombre)
        {
            int registros = 0;
            // Asumiendo que tienes una clase similar para acceder a los datos de Cliente
            registros = new CD_Cliente().CountBuscadorCliente(nombre);
            return Json(new { registros = registros });
        }

        [HttpPost]
        public JsonResult ElementosPaginacionBuscadorCliente(string nombre, int pagina, int siguientes)
        {
            List<string> lista = new List<string>();
            if (!string.IsNullOrEmpty(nombre))
            {
                // Asumiendo que tienes una clase similar para acceder a los datos de Cliente
                lista = new CD_Cliente().ElementosPaginacionBuscadorCliente(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });
        }
        [HttpPost]
        public JsonResult BuscarCliente(string nombre, int pagina, int siguientes)
        {
            List<Cliente> lista = new List<Cliente>();
            if (!string.IsNullOrEmpty(nombre))
            {
                lista = new CD_Cliente().BuscarCliente(nombre, pagina, siguientes);
            }
            return Json(new { Lista = lista });
        }

        [HttpPost]
        public JsonResult BuscarClientePorNombre(string nombre)
        {
            Cliente cliente = new Cliente();
            if (!string.IsNullOrEmpty(nombre))
            {
                cliente = new CD_Cliente().BuscarClientePorNombreCorto(nombre);
            }
            return Json(new { Lista =cliente });
        }
        [HttpPost]
        public JsonResult BuscarClientePorRFC(string rfc)
        {
            Cliente cliente = new Cliente();
            if (!string.IsNullOrEmpty(rfc))
            {
                cliente = new CD_Cliente().BuscarClientePorNombreCorto(rfc);
            }
            return Json(new { Lista =cliente });
        }

        [HttpPost]
        public JsonResult ListarPorIdCliente(string RFC)
        {
            Cliente cliente = new Cliente();
            if (!string.IsNullOrEmpty(RFC))
            {
                cliente = new CD_Cliente().BuscarClientePorRFC(RFC);
            }
            return Json(new { Lista = cliente });
        }

        // Eliminar Cliente
        [HttpPost]
        public JsonResult EliminarCliente(string rfc)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CD_Cliente().Eliminar(rfc, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje });
        }


    }
}
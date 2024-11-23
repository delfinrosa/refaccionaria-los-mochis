/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />
/// <reference path="../general/jqueryajax.js" />
var rol = getCookie("tipo");
var dataTabla;
var filaSeleccionada;
var tablaInicializada = false;
var pagina = 0;
var paginaTabla = 0;
var activarWhere = false;
var query = 'R_A';
var opcioneslista = $('#opcionesLista');
var input = $('#txtBusqueda');
var boolGuaradar = 0;


//MOSTRAR DATOS EN LA TABLA
function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/Cliente/ListarClientesTablaWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Cliente/ListarClienteTabla', { strpagina: paginaTabla, tipoOrden: query, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }
    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.rfc ;
        row.insertCell(1).textContent = item.razonSocial;
        row.insertCell(2).textContent = item.correo;
        row.insertCell(3).textContent = item.telefono;

        var accionesCell = row.insertCell(4);
        accionesCell.innerHTML = '<span class="icono-texto-container">' +
            '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
            '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button></span>';
    });
    if ($("#checkFiltros").is(':checked')) {
        if (dataTabla.length != 0) {
            ClientedorTabla()
        }
        else {
            swal("Filtro NO valido", "No se a encontrado ningun Registro con el Filtro puesto")

        }
    }
}
/////////
//METODOS
/////////
//BOTON GUARDAR 
function Guardar() {
    var Estatus;
    if ($('#cboEstatus').val() == "A") {
        Estatus = "A";
    } else {
        Estatus = "D";
    }
    var Cliente = {
        RFC: $('#txtRegistroRFC').val(),
        Estatus: $('#cboEstatus').val(),
        Comentario: $('#txtRegistroComentario').val(),
        RazonSocial: $('#txtRegistroRazonSocial').val(),
        Correo: $('#txtRegistroCorreo').val(),
        NombreCorto: $('#txtRegistroNombreCorto').val(),
        Calle: $('#txtRegistroCalle').val(),
        Colonia: $('#txtRegistroColonia').val(),
        NumeroInterior: $('#txtRegistroNumeroInterior').val(),
        NumeroExterior: $('#txtRegistroNumeroExterior').val(),
        Ciudad: $('#txtRegistroCiudad').val(),
        Estado: $('#txtRegistroEstado').val(),
        Pais: $('#txtRegistroPais').val(),
        CodigoPostal: $('#txtRegistroCodigoPostal').val(),
        Telefono: $('#txtRegistroTelefono').val(),
        Credito: $('#cboCredito').val() === "S",
        LimiteCredito: $('#txtRegistroLimiteCredito').val(),
        DiasCredito: $('#txtRegistroDiasCredito').val(),
        CuentaPago: $('#txtRegistroCuentaPago').val(),
        CuentaOrdenante: $('#txtRegistroCuentaOrdenante').val(),
        RFCBanco: $('#txtRegistroRFCBanco').val(),
        NombreBanco: $('#txtRegistroNombreBanco').val(),
        Comentarios: $('#txtRegistroComentario').val(),
        oCFDIUso: {
            CFDIUsoId: $('#txtRegistroCFDIUsoDescripcion').data('id'),
        },
        oCFDIMetodoPago: {
            CFDIMetodoPagoId: $('#txtRegistroCFDIMetodoPagoDescripcion').data('id'),
        },
        oCFDIFormaPago: {
            CFDIFormaPagoId: $('#txtRegistroCFDIFormaPagoDescripcion').data('id'),
        },
        oCFDIRegimenFiscal: {
            CFDIRegimenFiscalId: $('#txtRegistroCFDIRegimenFiscalDescripcion').data('id'),
        },
        oUsuario: {
            UsuarioId: 0 
        }
    };
    JQueryAjax_Normal('/Cliente/GuardarCliente', { objeto: Cliente, boolGuaradar: boolGuaradar }, false, function (data) {
        //Cliente guardar
        if (boolGuaradar == 1) {
            if (data.resultado != 0) {
                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombreCorto').val(), "success")
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
            boolGuaradar = 0;

        }
        //Cliente editar
        else {
            if (data.resultado) {
                $('#mensajeError').hide();
                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }

        }
    }, function () { })
}
//Nuevo Editar
function DesplegarInformacionCliente(json) {
    if (json != null && json.lista != null) {
        $('#txtRegistroRFC').val(json.lista.rfc);
        $('#cboEstatus').val(json.lista.estatus); // Asumiendo que tienes un campo de selección para Estatus
        $('#txtRegistroComentario').val(json.lista.comentarios);
        $('#txtRegistroRazonSocial').val(json.lista.razonSocial);
        $('#txtRegistroCorreo').val(json.lista.correo);
        $('#txtRegistroNombreCorto').val(json.lista.nombreCorto);
        $('#txtRegistroCalle').val(json.lista.calle);
        $('#txtRegistroColonia').val(json.lista.colonia);
        $('#txtRegistroNumeroInterior').val(json.lista.numeroInterior);
        $('#txtRegistroNumeroExterior').val(json.lista.numeroExterior);
        $('#txtRegistroCiudad').val(json.lista.ciudad);
        $('#txtRegistroEstado').val(json.lista.estado);
        $('#txtRegistroPais').val(json.lista.pais);
        $('#txtRegistroCodigoPostal').val(json.lista.codigoPostal);
        $('#txtRegistroTelefono').val(json.lista.telefono);
        $('#cboCredito').val(json.lista.credito ? "S" : "N"); // Asumiendo que tienes un campo de selección para Crédito
        $('#txtRegistroLimiteCredito').val(json.lista.limiteCredito);
        $('#txtRegistroDiasCredito').val(json.lista.diasCredito);
        $('#txtRegistroCuentaPago').val(json.lista.cuentaPago);
        $('#txtRegistroCuentaOrdenante').val(json.lista.cuentaOrdenante);
        $('#txtRegistroRFCBanco').val(json.lista.rfcBanco);
        $('#txtRegistroNombreBanco').val(json.lista.nombreBanco);

        if (json.lista.oCFDIUso) {
            $('#txtRegistroCFDIUsoDescripcion').attr('data-id', json.lista.oCFDIUso.cfdiUsoId);
            $('#txtRegistroCFDIUsoDescripcion').val(json.lista.oCFDIUso.descripcion);
        }

        if (json.lista.oCFDIMetodoPago) {
            $('#txtRegistroCFDIMetodoPagoDescripcion').attr('data-id', json.lista.oCFDIMetodoPago.cfdiMetodoPagoId);
            $('#txtRegistroCFDIMetodoPagoDescripcion').val(json.lista.oCFDIMetodoPago.descripcion);
        }

        if (json.lista.oCFDIFormaPago) {
            $('#txtRegistroCFDIFormaPagoDescripcion').attr('data-id', json.lista.oCFDIFormaPago.cfdiFormaPagoId);
            $('#txtRegistroCFDIFormaPagoDescripcion').val(json.lista.oCFDIFormaPago.descripcion);
        }

        if (json.lista.oCFDIRegimenFiscal) {
            $('#txtRegistroCFDIRegimenFiscalDescripcion').attr('data-id', json.lista.oCFDIRegimenFiscal.cfdiRegimenFiscalId);
            $('#txtRegistroCFDIRegimenFiscalDescripcion').val(json.lista.oCFDIRegimenFiscal.descripcion);
        }
    }
}


function desplegarClientePorNombre(texto) {
    $('#cboRegistroActivo').focus();
    JQueryAjax_Normal('/Cliente/buscarClientePorNombre', { nombre: texto }, false, function (data) {
        if (data.descripcion != "") {
            DesplegarInformacionCliente(data)
            console.log(data)
            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();

        } else {
            limpiar();
        }
    }, function () { }
    );
}
function limpiar() {
    $('#txtRegistroid').val("");
    $('#txtRegistroNombre').val("");
    $('#cboRegistroActivo').val("A");
    $('#cboRegistroActivo').focus();
}
///////////////
///Carga el ultimo
///////////////
function CargarUltimo() {
    JQueryAjax_Normal('/Cliente/UltimoRegistroCliente', {}, false, function (data) {
        DesplegarInformacionCliente(data);
    }, function () { });
}
function ActualizarTabla() {
    datosTablaPaginado(0, $("#cboDivicion-tabla").val());
    var division = Math.ceil(CountPaginadoTabla() / $("#cboDivicion-tabla").val());
    configurarPaginacion(0, division, datosTablaPaginado, "T", $("#cboDivicion-tabla").val())
}
//////////
//BOTONES
//////////

//Barra de heramientas 
$("#btn-Nuevo").click(function () {
    limpiar();
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', false);
    modoGuardando()
    document.getElementById('SpantabDatosGenerales').textContent = '4';
    document.getElementById('SpantabDatosDireccion').textContent = '8';
    document.getElementById('SpantabDatosPago').textContent = '8';
    document.getElementById('SpantabDatosContacto').textContent = '3';
    boolGuaradar = 1;

});
$("#btn-Cancelar").click(function () {
    swal({
        title: "¿Esta Seguro?",
        text: "Si cancela no se realizara ningun cambio en el registro",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            CargarUltimo();
            $('#btn-Eliminar').prop('disabled', false);
            $('#btn-Cancelar').prop('disabled', true);
            $('#btn-Guardar').prop('disabled', true);
            $('#btn-Nuevo').prop('disabled', false);
            $('#modo').hide();

            $(".bg-edicion").removeClass("bg-edicion");
            $(".bg-guardar").removeClass("bg-guardar");
            Navegar()
            document.getElementById('SpantabDatosGenerales').textContent = '';
            boolGuaradar = 0;

        });



});
$("#btn-Guardar").click(function () {
    if (ValidarCampoVacio($('[data-requerido]'))) {
        $('#btn-Eliminar').prop('disabled', false);
        $('#btn-Guardar').prop('disabled', true);
        $('#btn-Cancelar').prop('disabled', true);
        $('#btn-Nuevo').prop('disabled', false);
        Guardar();
        ActualizarTabla();
        CargarUltimo();

        $(".bg-edicion").removeClass("bg-edicion");
        $(".bg-guardar").removeClass("bg-guardar");
        Navegar()
        $('#modo').hide();

    }
});
$("#btn-Eliminar").click(function () {
    $('#btn-Guardar').prop('disabled', false);
    var idBorrar = $("#txtRegistroid").val();
    if (idBorrar != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar la línea?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/Cliente/EliminarCliente', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimo();
                } else {

                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });
        });
    }
});

$('[data-requerido]').on('change input', function () {
    $('#btn-Cancelar').prop('disabled', false);
    $('#btn-Guardar').prop('disabled', false);
    if (boolGuaradar == true) {
        modoGuardando();
    } else {
        modoEditando();
    }
});



// Focus al input Buscar (Icono lupa)
$('#btn-BuscarBuscador').click(function () {
    $('#txtBusqueda').focus();
});

/////
//TABLA
/////
$("#btn-Actualizar").click(function () {
    ActualizarTabla()
});
$("#tablaId").click(function () {
    paginaTabla = 0
    $('#iconoDescripcion').removeClass('fa-sort-alpha-down').addClass('fa-sort');
    $('#iconoDescripcion').removeClass('fa-sort-alpha-up').addClass('fa-sort');

    query = (query == 'R_A') ? 'R_D' : 'R_A';
    var nuevoIconoClass = (query == 'R_A') ? 'fa-sort-numeric-down' : 'fa-sort-numeric-down-alt';
    $('#iconoID').removeClass('fa-sort-numeric-down fa-sort-numeric-down-alt').addClass(`${nuevoIconoClass} fa-1x m-1`);
    ActualizarTabla();
    return false;
});
$("#tablaDescripcion").click(function () {
    paginaTabla = 0
    $('#iconoID').removeClass('fa-sort-alpha-down').addClass('fa-sort');
    $('#iconoID').removeClass('fa-sort-alpha-up').addClass('fa-sort');

    query = (query == 'N_A') ? 'N_D' : 'N_A';
    var nuevoIconoClass = (query == 'N_A') ? 'fas fa-sort-alpha-down' : 'fas fa-sort-alpha-down-alt';
    $('#iconoDescripcion').removeClass('fas fa-sort-alpha-down fas fa-sort-alpha-down-alt').addClass(`${nuevoIconoClass} fa-1x m-1`);
    ActualizarTabla();
    return false;
});
$("#btn-buscarTabla").click(function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla()
    }
    else {
        $("#filtroslinea").focus();
    }
});
//BOTON EDITAR
$("#tabla tbody").on("click", '.btn-editar', function () {
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', false);
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', false);
    $('#cboRegistroActivo').focus();

    var rowIndex = $(this).closest("tr").index();
    var Cliente = dataTabla[rowIndex];
    JQueryAjax_Normal('/Cliente/listarPorIdCliente', { RFC: Cliente.rfc }, false,
        function (data) {
            DesplegarInformacionCliente(data);
        }, function () { });
});
//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var rowIndex = $(this).closest("tr").index();
    var Cliente = dataTabla[rowIndex];
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Cliente?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Cliente/EliminarCliente', { rfc: Cliente.rfc }, true, function (data) {

                if (data.resultado) {
                    dataTabla.splice(rowIndex, 1);
                    datosTablaPaginado(paginaTabla, $("#cboDivicion-tabla").val());
                    CargarUltimo();
                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });
        });
})
/*****************************************************/
//Cosas que tiene que cambiar
$('#txtBusquedaCliente').on('input', function () {
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var searchTerm = $('#txtBusquedaCliente').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscador(pagina, NumeroDivisiones);
    var division = CountPaginadoBuscador(searchTerm, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscador, "B", NumeroDivisiones)
});
$("#txtRegistroNombre").on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});
// Ocultar opcionesLista cuando se hace clic fuera de ella
$(document).on('click', function (event) {
    if (!$(event.target).closest('#buscador').length) {
        $('#botonesPaginado').hide();
        $('#opcionesLista').hide();
    }
});
/////////
// Filtro
/////////
$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtrosClientes').slideDown();
    } else {
        $('#filtrosClientes').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
});
//////////////
//Paginado Tabla
//////////////
function CountPaginadoTabla() {
    var resultado = 0;
    if ($("#filtrosClientes").is(':checked')) {
        var where = whereQuery();
        JQueryAjax_Normal('/Cliente/CountTablaClienteWhere', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Cliente/CountTablaCliente', {}, false, function (data) {
            resultado = data.registros;
        }, function () { });
    }
    return resultado;
}
function cargartabla() {
    datosTablaPaginado(paginaTabla, 10);
    var divisiona = Math.ceil(CountPaginadoTabla() / 10);
    configurarPaginacion(paginaTabla, divisiona, datosTablaPaginado, "T", 10)
}
function whereQuery() {
    var query = ""
    if ($("#checkFiltroId").prop('checked')) {
        query += "c.RFC LIKE '%" + $("#txtFiltroIDWhere").val() + "%'"
    }
    if ($("#checkFiltroRazonSocial").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "c.RazonSocial LIKE '%" + $("#txtFiltroRazonSocialWhere").val() + "%'"
    }
    if ($("#checkFiltroTelefono").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "c.telefono LIKE '%" + $("#txtFiltroTelefonoWhere").val() + "%'"
    }
    if ($("#checkFiltroCorreo").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "c.correo LIKE '%" + $("#txtFiltroCorreoWhere").val() + "%'"
    }
    return query
}
function eventoCheckFiltro(checkbox, element) {
    checkbox.change(function () {
        if ($(this).is(':checked')) {
            element.attr('data-requerido-filtro', checkbox.val());
        } else {
            element.removeAttr('data-requerido-filtro');
        }
        element.prop('disabled', !$(this).is(':checked'));
        element.val("");
    });
}
function ClientedorTabla() {
    ClienterCeldas(0, $("#txtFiltroIDWhere").val(), 'yellow');
    ClienterCeldas(1, $("#checkFiltroRazonSocial").val(), '#FCC208');

}
function ClienterCeldas(columna, filtro, color) {
    $('#tabla tbody tr').each(function () {
        var textoCelda = $(this).find('td:eq(' + columna + ')').text().trim().toLowerCase();
        var filtroMinusculas = filtro.toLowerCase();

        if (textoCelda.includes(filtroMinusculas)) {
            var spanResaltado = $('<span>').html(textoCelda.replace(new RegExp(filtroMinusculas, 'gi'), match => `<span style="background-color: ${color};">${match}</span>`));
            $(this).find('td:eq(' + columna + ')').html(spanResaltado);
        }
    });
}
eventoCheckFiltro($("#checkFiltroId"), $("#txtFiltroIDWhere"));
eventoCheckFiltro($("#checkFiltroRazonSocial"), $("#txtFiltroRazonSocialWhere"));
eventoCheckFiltro($("#checkFiltroTelefono"), $("#txtFiltroTelefonoWhere"));
eventoCheckFiltro($("#checkFiltroCorreo"), $("#txtFiltroCorreoWhere"));
/*BUSCADOR*/
/****************************/
function desplegarPaginacionBuscador(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusquedaCliente').val().toLowerCase();
    JQueryAjax_Normal('/Cliente/ElementosPaginacionBuscadorCliente', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.lista;
        $('#opcionesLista').empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    $("#txtBusquedaCliente").val(item);
                    desplegarClientePorNombre(item);
                });
                $('#opcionesLista').append(listItem);
            });
            $('#opcionesLista').show();
            $('#botonesPaginado').show();
        }
    }, function () { });
}
function CountPaginadoBuscador(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Cliente/CountBuscadorCliente', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

//-----------------------------------------//

// Conteo para Paginación de Buscador CFDI Uso por Descripción
function CountPaginadoBuscadorCFDIUso(condicion, texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Cliente/countCFDIUsoWhere', { where: condicion + " LIKE '%" + texto + "%'" }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

// Despliegue de Paginación y Lista para Buscador CFDI Uso por Descripción
function desplegarPaginacionBuscadorCFDIUsoDescripcion(descripcion, pagina, NumeroDivisiones) {
    JQueryAjax_Normal('/Cliente/BuscadorCFDIUsoDescripcionID', { descripcion: descripcion, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.descripcion);
                    listItem.on('click', function () {
                        $('#txtRegistroCFDIUsoDescripcion').attr('data-id', item.cfdiusoId);
                        $("#txtRegistroCFDIUsoDescripcion").val(item.descripcion);
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        }, function () { });
}
// Activación del Buscador por Descripción
$("#txtRegistroCFDIUsoDescripcion").on("input focus", function () {
    var NumeroDivisiones = 5;
    var descripcion = $(this).val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorCFDIUsoDescripcion(descripcion, pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorCFDIUso("Descripcion", descripcion, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorCFDIUsoDescripcion, NumeroDivisiones);
});

// Despliegue de Paginación y Lista para Buscador CFDI Uso por ID
function desplegarPaginacionBuscadorCFDIUsoID(descripcion, pagina, NumeroDivisiones) {
    JQueryAjax_Normal('/Cliente/BuscadorCFDIUsoIDDescripcion', { descripcion: descripcion, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.CFDIUsoId);
                    listItem.on('click', function () {
                        $('#txtRegistroCFDIUsoDescripcion').attr('data-id', item.CFDIUsoId);
                        $("#txtRegistroCFDIUsoDescripcion").val(item.Descripcion);
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        }, function () { });
}
$("#txtRegistroCFDIUsoId").on("input focus", function () {
    var NumeroDivisiones = 5; // Número de divisiones para la paginación
    var ID = $(this).val().toLowerCase();
    pagina = 0; // Inicializa la página a mostrar
    desplegarPaginacionBuscadorCFDIUsoID(ID, pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorCFDIUso("CFDIUsoId", ID, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorCFDIUsoID, NumeroDivisiones);
});
/*____________________________________________________________*/
// Conteo para Paginación de Buscador Método Pago por Descripción
function CountPaginadoBuscadorMetodoPago(condicion, texto, num) {
    debugger
    var division = 0;
    JQueryAjax_Normal('/Cliente/ContarMetodosPago', { where: condicion + " LIKE '%" + texto + "%'" }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

// Despliegue de Paginación y Lista para Buscador Método Pago por Descripción
function desplegarPaginacionBuscadorMetodoPagoDescripcion(descripcion, pagina, NumeroDivisiones) {
    JQueryAjax_Normal('/Cliente/BuscadorMetodoPagoDescripcion', { descripcion: descripcion, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            $('#opcionesLista').empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                    listItem.on('click', function () {
                        $('#txtRegistroCFDIMetodoPagoDescripcion').attr('data-id', item.MetodoPagoId);
                        $("#txtRegistroCFDIMetodoPagoDescripcion").val(item.Descripcion);
                        $('#opcionesLista').hide();
                    });
                    $('#opcionesLista').append(listItem);
                });
                $('#opcionesLista').show();
            }
        }, function () { });
}

// Despliegue de Paginación y Lista para Buscador Método Pago por ID
function desplegarPaginacionBuscadorMetodoPagoID(id, pagina, NumeroDivisiones) {
    JQueryAjax_Normal('/Cliente/BuscadorMetodoPagoID', { id: id, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            $('#opcionesLista').empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.CFDIMetodoPagoId);
                    listItem.on('click', function () {
                        $('#txtRegistroCFDIMetodoPagoDescripcion').attr('data-id', item.CFDIMetodoPagoId);
                        $("#txtRegistroCFDIMetodoPagoDescripcion").val(item.Descripcion);
                        $('#opcionesLista').hide();
                    });
                    $('#opcionesLista').append(listItem);
                });
                $('#opcionesLista').show();
            }
        }, function () { });
}

// Activación del Buscador por Descripción
$("#txtRegistroCFDIMetodoPagoDescripcion").on("input focus", function () {
    var NumeroDivisiones = 5; // Número de divisiones para la paginación
    var descripcion = $(this).val().toLowerCase();
    var num = 5; // Número de resultados por página
    var pagina = 0; // Página inicial
    desplegarPaginacionBuscadorMetodoPagoDescripcion(descripcion, pagina, num);
    var division = CountPaginadoBuscadorMetodoPago('Descripcion', descripcion, num);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorMetodoPagoDescripcion, NumeroDivisiones);
});

// Activación del Buscador por ID
$("#txtRegistroCFDIMetodoPagoId").on("input focus", function () {
    var NumeroDivisiones = 5; // Número de divisiones para la paginación
    var id = $(this).val().toLowerCase();
    var num = 5; // Número de resultados por página
    var pagina = 0; // Página inicial
    desplegarPaginacionBuscadorMetodoPagoID(id, pagina, num);
    var division = CountPaginadoBuscadorMetodoPago('CFDIMetodoPagoId', id, num);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorMetodoPagoID, NumeroDivisiones);
});

/*______________________________________________*/

function CountPaginadoBuscadorFormaPago(condicion, texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Cliente/ContarFormasPago', { where: condicion + " LIKE '%" + texto + "%'" }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

function desplegarPaginacionBuscadorFormaPagoDescripcion(descripcion, pagina, NumeroDivisiones) {
    JQueryAjax_Normal('/Cliente/BuscarFormasPagoDescripcion', { descripcion: descripcion, pagina: pagina, cantidadPorPagina: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            $('#opcionesLista').empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                    listItem.on('click', function () {
                        $('#txtRegistroCFDIFormaPagoDescripcion').attr('data-id', item.CFDIFormaPagoId);
                        $("#txtRegistroCFDIFormaPagoDescripcion").val(item.Descripcion);
                        $('#opcionesLista').hide();
                    });
                    $('#opcionesLista').append(listItem);
                });
                $('#opcionesLista').show();
            }
        }, function () { });
}

function desplegarPaginacionBuscadorFormaPagoID(id, pagina, NumeroDivisiones) {
    JQueryAjax_Normal('/Cliente/BuscarFormasPagoId', { id: id, pagina: pagina, cantidadPorPagina: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            $('#opcionesLista').empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.CFDIFormaPagoId);
                    listItem.on('click', function () {
                        $('#txtRegistroCFDIFormaPagoDescripcion').attr('data-id', item.CFDIFormaPagoId);
                        $("#txtRegistroCFDIFormaPagoDescripcion").val(item.Descripcion);
                        $('#opcionesLista').hide();
                    });
                    $('#opcionesLista').append(listItem);
                });
                $('#opcionesLista').show();
            }
        }, function () { });
}

$("#txtRegistroCFDIFormaPagoDescripcion").on("input focus", function () {
    var descripcion = $(this).val().toLowerCase();
    var num = 5; // Número de resultados por página
    var pagina = 0; // Página inicial
    desplegarPaginacionBuscadorFormaPagoDescripcion(descripcion, pagina, num);
    var division = CountPaginadoBuscadorFormaPago('Descripcion', descripcion, num);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorFormaPagoDescripcion, num);
});

$("#txtRegistroCFDIFormaPagoId").on("input focus", function () {
    var id = $(this).val().toLowerCase();
    var num = 5; // Número de resultados por página
    var pagina = 0; // Página inicial
    desplegarPaginacionBuscadorFormaPagoID(id, pagina, num);
    var division = CountPaginadoBuscadorFormaPago('CFDIFormaPagoId', id, num);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorFormaPagoID, num);
});


/*---------------------------------------*/
// Conteo para Paginación de Buscador de Regimen Fiscal por Descripción
function CountPaginadoBuscadorRegimenFiscal(condicion, texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Cliente/ContarRegimenesFiscales', { condicion: condicion + " LIKE '%" + texto + "%'" }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

function desplegarPaginacionBuscadorRegimenFiscalDescripcion(descripcion, pagina, NumeroDivisiones) {
    JQueryAjax_Normal('/Cliente/BuscadorRegimenFiscalDescripcion', { descripcion: descripcion, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            console.log(data)
            var lista = data.data;
            $('#opcionesLista').empty();
            if (lista.length > 0) {

                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                    listItem.on('click', function () {
                        $('#txtRegistroCFDIRegimenFiscalDescripcion').attr('data-id', item.CFDIRegimenFiscalId);
                        $("#txtRegistroCFDIRegimenFiscalDescripcion").val(item.Descripcion);
                        $('#opcionesLista').hide();
                    });
                    $('#opcionesLista').append(listItem);
                });
                $('#opcionesLista').show();
            }
        }, function () { });
}

function desplegarPaginacionBuscadorRegimenFiscalID(id, pagina, NumeroDivisiones) {
    JQueryAjax_Normal('/Cliente/BuscadorRegimenFiscalID', { id: id, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            $('#opcionesLista').empty();
            if (lista.length > 0) {
                console.log(lista)

                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.CFDIRegimenFiscalId);
                    listItem.on('click', function () {
                        $('#txtRegistroCFDIRegimenFiscalDescripcion').attr('data-id', item.CFDIRegimenFiscalId);
                        $("#txtRegistroCFDIRegimenFiscalDescripcion").val(item.Descripcion);
                        $('#opcionesLista').hide();
                    });
                    $('#opcionesLista').append(listItem);
                });
                $('#opcionesLista').show();
            }
        }, function () { });
}

$("#txtRegistroCFDIRegimenFiscalDescripcion").on("input focus", function () {
    var NumeroDivisiones = 5; // Número de divisiones para la paginación

    var descripcion = $(this).val().toLowerCase();
    var num = 5; // Número de resultados por página
    var pagina = 0; // Página inicial
    desplegarPaginacionBuscadorRegimenFiscalDescripcion(descripcion, pagina, num);
    var division = CountPaginadoBuscadorRegimenFiscal('Descripcion', descripcion, num);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorRegimenFiscalDescripcion, NumeroDivisiones);
});

$("#txtRegistroCFDIRegimenFiscalId").on("input focus", function () {
    var NumeroDivisiones = 5; // Número de divisiones para la paginación

    var id = $(this).val().toLowerCase();
    var num = 5; // Número de resultados por página
    var pagina = 0; // Página inicial
    desplegarPaginacionBuscadorRegimenFiscalID(id, pagina, num);
    var division = CountPaginadoBuscadorRegimenFiscal('CFDIRegimenFiscalId', id, num);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorRegimenFiscalID, NumeroDivisiones);
});

$("#txtRegistroCFDIUsoId").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorCFDIUso");
});

$("#txtRegistroCFDIMetodoPagoId").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorCFDIMetodoPago");
});

$("#txtRegistroCFDIFormaPagoId").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorCFDIFormaPago");
});

$("#txtRegistroCFDIRegimenFiscalId").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorCFDIRegimenFiscal");
});
$("#txtRegistroCFDIUsoDescripcion").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorCFDIUsoDescripcion");
});

$("#txtRegistroCFDIMetodoPagoDescripcion").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorCFDIMetodoPagoDescripcion");
});

$("#txtRegistroCFDIFormaPagoDescripcion").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorCFDIFormaPagoDescripcion");
});

$("#txtRegistroCFDIRegimenFiscalDescripcion").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorCFDIRegimenFiscalDescripcion");
});

///////////
//AL CARGAR


$(document).ready(function () {
    JQueryAjax_Normal('/Cliente/tamaño', {}, true, function (campos) {
        console.log(campos)
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                }
            }
        });
    }, function () { });

    JQueryAjax_Normal('/Cliente/tamañoCFDIUso', {}, true, function (campos) {
        console.log(campos)
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                } else if (item.Item2 > 1) {
                    input = document.querySelector('[name="Descripcion2"]');
                    input.setAttribute('maxlength', item.Item2);

                }
            }
        });
    }, function () { });

    JQueryAjax_Normal('/Cliente/tamañoCFDIFormaPago', {}, true, function (campos) {
        console.log(campos)
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                } else if (item.Item2 > 1) {
                    input = document.querySelector('[name="Descripcion4"]');
                    input.setAttribute('maxlength', item.Item2);

                }
            }
        });
    }, function () { });
    JQueryAjax_Normal('/Cliente/tamañoCFDIMetodoPago', {}, true, function (campos) {
        console.log(campos)
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                } else if (item.Item2 > 1) {
                    input = document.querySelector('[name="Descripcion3"]');
                    input.setAttribute('maxlength', item.Item2);

                }
            }
        });
    }, function () { });

    JQueryAjax_Normal('/Cliente/tamañoCFDIRegimenFiscal', {}, true, function (campos) {
        console.log(campos)
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                } else if (item.Item2 > 1) {
                    input = document.querySelector('[name="Descripcion5"]');
                    input.setAttribute('maxlength', item.Item2);

                }
            }
        });
    }, function () { });


    ///////////
    cargartabla();
    CargarUltimo();

});
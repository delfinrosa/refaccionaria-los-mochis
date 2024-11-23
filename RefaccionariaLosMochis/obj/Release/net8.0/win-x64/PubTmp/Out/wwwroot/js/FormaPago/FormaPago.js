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
var query = 'I_A';
$('#iconoDescripcion').hide();
var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');
var boolGuaradar = 0;


//MOSTRAR DATOS EN LA TABLA
function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/FormaPago/ListarTablaWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/FormaPago/ListarTabla', { strpagina: paginaTabla, tipoOrden: query, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }
    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.cfdiFormaPagoId;
        row.insertCell(1).textContent = item.descripcion.toUpperCase();
        var estadoCell = row.insertCell(2);

        if (item.estatus === "A") {
            estadoCell.innerHTML = '<h5 class="m-auto"><span class="badge rounded-pill bg-success">Activo</span></h5>';
        }  else {
            estadoCell.innerHTML = '<h5 class="m-auto"><span class="badge rounded-pill bg-danger">Desactivado</span></h5>';
        }

        var accionesCell = row.insertCell(3);
        accionesCell.innerHTML = '<span class="icono-texto-container">' +
            '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
            '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button></span>';
    });
    if ($("#checkFiltros").is(':checked')) {
        if (dataTabla.length != 0) {
            MarcadorTabla()
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
    debugger
    var formaPago = {
        CFDIFormaPagoId: $('#txtCFDIFormaPagoId').val(),
        Descripcion: $('#txtRegistroDescripcion').val().toUpperCase(),
        Estatus: $('#cboEstatus').val(),
        oUsuario: {
            IdUsuario: 0
        }
    };

    JQueryAjax_Normal('/FormaPago/GuardarFormaPago', { formaPago: formaPago, boolGuaradar: boolGuaradar }, true,
        function (data) {
            //FormaPago guardar
            if (boolGuaradar == 1) {
                if (data.resultado != 0) {
                    $('#txtRegistroid').val(data.resultado);
                    swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroDescripcion').val(), "success")
                    $('#mensajeError').hide();
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();
                }
                boolGuaradar = 0;
            }
            //FormaPago editar
            else {
                if (data.resultado) {
                    $('#mensajeError').hide();
                    swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroDescripcion').val(), "success")
                    $('#mensajeError').hide();
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();
                }
            }
        },
        function (errorThrown) {
            swal("Error al guardar", "Por favor, intente de nuevo. Error: " + errorThrown, "error");
        });
}

//Nuevo Editar
function DesplegarInformacionCampos(data) {
    if (data) {
        if (data.estatus == "A") {
            $('#cboEstatus').val("A");
        } else {
            $('#cboEstatus').val("D");
        }
        $('#txtCFDIFormaPagoId').val(data.cfdiFormaPagoId);
        $('#txtRegistroDescripcion').val(data.descripcion);
    }
}

function desplegarFormaPagoPorDescripcion(texto) {
    JQueryAjax_Normal('/FormaPago/BuscarFormaPagoPorDescripcion', { descripcion: texto }, true, function (data) {
        if (data.Descripcion != "") {
            $('#txtRegistroDescripcion').val(data.descripcion);
            $('#txtCFDIFormaPagoId').val(data.cfdiFormaPagoId);
            if (data.activo == "A") {
                $('#cboEstatus').val("A"); // Asumiendo que tienes un control para el estatus
            } else {
                $('#cboEstatus').val("D"); // Para otros casos, adaptar según tus necesidades
            }
            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();
        } else {
            $('#txtRegistroDescripcion').val('');
            $('#cboRegistroEstatus').val('A'); // Reset al estatus por defecto o según corresponda
        }
    }, function () { });
}

function limpiar() {
    $('#txtCFDIFormaPagoId').val("");
    $('#txtRegistroDescripcion').val("");
    $('#cboEstatus').val("A");
    $('#txtCFDIFormaPagoId').focus();
}
///////////////
///Carga el ultimo
///////////////
function CargarUltimo() {
    $('#cboEstatus').focus();
    JQueryAjax_Normal('/FormaPago/UltimoRegistro', {}, true, function (data) {
        DesplegarInformacionCampos(data.lista);
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
    document.getElementById('SpantabDatosGenerales').textContent = '2';
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
        modoEditando()

        $(".bg-edicion").removeClass("bg-edicion");
        $(".bg-guardar").removeClass("bg-guardar");
        Navegar()
    }
});
$("#btn-Eliminar").click(function () {
    $('#btn-Guardar').prop('disabled', false);
    var idBorrar = $("#txtCFDIFormaPagoId").val();
    if (idBorrar != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar esta forma de pago?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/FormaPago/EliminarFormaPago', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    //ActualizarTablaFormaPago();
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Forma de Pago Eliminada", "la Forma de Pago se elimino correctamente", "success")
                    }, 250);
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
    $('#iconoDescripcion').hide();
    $('#iconoID').show();
    query = (query == 'I_A') ? 'I_D' : 'I_A';
    var nuevoIconoClass = (query == 'I_A') ? 'fa-sort-numeric-down' : 'fa-sort-numeric-down-alt';
    $('#iconoID').removeClass('fa-sort-numeric-down fa-sort-numeric-down-alt').addClass(`${nuevoIconoClass} fa-1x m-1`);
    ActualizarTabla();
    return false;
});
$("#tablaDescripcion").click(function () {
    paginaTabla = 0
    $('#iconoID').hide();
    $('#iconoDescripcion').show();
    query = (query == 'D_A') ? 'D_D' : 'D_A';
    var nuevoIconoClass = (query == 'D_A') ? 'fas fa-sort-alpha-down' : 'fas fa-sort-alpha-down-alt';
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
    var FormaPago = dataTabla[rowIndex];
    JQueryAjax_Normal('/FormaPago/ListarPorIdFormaPago', { Id: FormaPago.cfdiFormaPagoId }, false,
        function (data) {
            DesplegarInformacionCampos(data.lista);
        }, function () { });
});
//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var rowIndex = $(this).closest("tr").index();
    var FormaPago = dataTabla[rowIndex];
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la forma de pago " + FormaPago.descripcion + " ?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/FormaPago/EliminarFormaPago', { id: FormaPago.cfdiFormaPagoId }, true, function (data) {

                if (data.resultado) {
                    dataTabla.splice(rowIndex, 1);
                    datosTablaPaginado(paginaTabla, $("#cboDivicion-tabla").val());
                    CargarUltimo();
                } else {
                    swal("No se pudo eliminar", data.mensaje, "error");
                }
            }, function () { });
        });
})
/*****************************************************/
//Cosas que tiene que cambiar
$('#txtBusqueda').on('input', function () {
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var searchTerm = $('#txtBusqueda').val().toLowerCase();
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
        opcionesLista.hide();
    }
});
/////////
// Filtro
/////////
$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtroslinea').slideDown();
    } else {
        $('#filtroslinea').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
});
//////////////
//Paginado Tabla
//////////////
function CountPaginadoTabla() {
    var resultado = 0;
    if ($("#checkFiltros").is(':checked')) {
        var where = whereQuery();
        JQueryAjax_Normal('/FormaPago/countTablaWhere', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/FormaPago/CountTabla', {}, false, function (data) {
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
        query += "f.CFDIFormaPagoId LIKE '%" + $("#txtFiltroIDWhere").val() + "%'"
    }
    if ($("#checkFiltroDescripcion").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "f.Descripcion LIKE '%" + $("#txtFiltroDescripcionWhere").val() + "%'"
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
function MarcadorTabla() {
    marcarCeldas(0, $("#txtFiltroIDWhere").val(), 'yellow');
    marcarCeldas(1, $("#txtFiltroDescripcionWhere").val(), '#FCC208');

}
function marcarCeldas(columna, filtro, color) {
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
eventoCheckFiltro($("#checkFiltroDescripcion"), $("#txtFiltroDescripcionWhere"));
/****************************/
/*BUSCADOR DE FORMA DE PAGO*/
/****************************/
function desplegarPaginacionBuscador(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusqueda').val().toLowerCase(); // Asegúrate de tener el input correcto
    JQueryAjax_Normal('/FormaPago/ElementosPaginacionBuscadorFormaPago', { descripcion: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.lista;
        $('#opcionesLista').empty(); // Ajusta al ID de tu lista de opciones
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    desplegarFormaPagoPorDescripcion(listItem.text());
                    $('#txtBusqueda').val(item); // Ajusta al input donde se muestra la selección
                });
                $('#opcionesLista').append(listItem); // Ajusta al ID de tu lista de opciones
            });
            $('#opcionesLista').show(); // Ajusta al ID de tu lista de opciones
            $('#botonesPaginado').show(); // Ajusta al ID de tus botones de paginado, si aplicase
        }
    }, function () { });
}

function CountPaginadoBuscador(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/FormaPago/CountBuscadorFormaPago', { descripcion: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}




$(document).ready(function () {
    JQueryAjax_Normal('/FormaPago/tamaño', {}, true, function (campos) {

        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                }
            }
        });
    }, function () { });



});



///////////
//AL CARGAR
///////////
CargarUltimo();
cargartabla();
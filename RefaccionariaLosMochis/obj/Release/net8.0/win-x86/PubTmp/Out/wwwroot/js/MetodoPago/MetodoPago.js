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
var boolGuardar = 0;


//MOSTRAR DATOS EN LA TABLA
function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/MetodoPago/ListarMetodoPagoTablaWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/MetodoPago/ListarMetodoPagoTabla', { strpagina: paginaTabla, tipoOrden: query, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }
    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.cfdiMetodoPagoId;
        row.insertCell(1).textContent = item.descripcion;
        var estadoCell = row.insertCell(2);

        if (item.estatus === "A") {
            estadoCell.innerHTML = '<h5 class="m-auto"><span class="badge rounded-pill bg-success">Activo</span></h5>';
        } else {
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
    var metodoPago = {
        CFDIMetodoPagoId: $('#txtCFDIMetodoPagoId').val(),
        Descripcion: $('#txtDescripcion').val().toUpperCase(),
        Estatus: $('#cboEstatus').val(),
        oUsuario: { IdUsuario: "0" } 
    };

    JQueryAjax_Normal('/MetodoPago/GuardarMetodoPago', { metodoPago: metodoPago, boolGuardar: boolGuardar }, true,
        function (data) {
            if (boolGuardar == 1) {
                if (data.resultado != 0) {
                    $('#txtRegistroid').val(data.resultado);
                    swal("Registro Guardado", "Se guardo el registro de " + $('#txtDescripcion').val(), "success")
                    $('#mensajeError').hide();
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();
                }
                boolGuardar = 0;
            }
            //metodoPago editar
            else {
                if (data.resultado) {
                    $('#mensajeError').hide();
                    swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtDescripcion').val(), "success")
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
        $('#txtCFDIMetodoPagoId').val(data.cfdiMetodoPagoId);
        $('#txtDescripcion').val(data.descripcion);
    }
}


function desplegarMarcaPorNombre(texto) {
    $('#cboRegistroActivo').focus();
    JQueryAjax_Normal('/MetodoPago/BuscarMetodoPagoPorNombre', { nombre: texto }, true, function (data) {
        if (data.descripcion != "") {
            if (data.estatus == "A") {
                $('#cboEstatus').val("A");
            } else {
                $('#cboEstatus').val("D");
            }
            $('#txtCFDIMetodoPagoId').val(data.cfdiMetodoPagoId);
            $('#txtDescripcion').val(data.descripcion);

            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();

        } else {
            // Si no se encontraron resultados, puedes limpiar los campos o mostrar un mensaje de error
            $('#txtRegistroNombre').val('');
            $('#txtRegistrodescripcion').val('');
            $('#cboRegistroActivo').val('A');
        }
    }, function () { }
    );
}
function limpiar() {
    $('#txtCFDIMetodoPagoId').val("");
    $('#txtDescripcion').val("");
    $('#cboEstatus').val("A");
    $('#txtCFDIMetodoPagoId').focus();
}
///////////////
///Carga el ultimo
///////////////
function CargarUltimo() {
    $('#cboEstatus').focus();
    JQueryAjax_Normal('/MetodoPago/UltimoRegistro', {}, true, function (data) {
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
    boolGuardar = 1;

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
            boolGuardar = 0;

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
    var idBorrar = $("#txtCFDIMetodoPagoId").val();
    if (idBorrar != "") {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar este método de pago?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/MetodoPago/EliminarMetodoPago', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    //ActualizarTablaMetodoPago(); // Función para actualizar la tabla, si tienes una.
                    CargarUltimo(); 
                    setTimeout(function () {
                        swal("Método de Pago Eliminado", "El método de pago se eliminó correctamente", "success");
                    }, 250);
                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });

        });
    } else {
        swal("No se ha seleccionado ningún método de pago para eliminar", "", "warning");
    }
});


$('[data-requerido]').on('change input', function () {
    $('#btn-Cancelar').prop('disabled', false);
    $('#btn-Guardar').prop('disabled', false);
    if (boolGuardar == true) {
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
    var metodoPago = dataTabla[rowIndex];
    JQueryAjax_Normal('/MetodoPago/ListarPorIdMetodoPago', { CFDIMetodoPagoId: metodoPago.cfdiMetodoPagoId }, false,
        function (data) {
            DesplegarInformacionCampos(data.lista);
        }, function () { });
});
//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var rowIndex = $(this).closest("tr").index();
    var metodoPago = dataTabla[rowIndex];
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar el Metodo de Pago " + metodoPago.descripcion + " ?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/MetodoPago/EliminarMetodoPago', { id: metodoPago.cfdiMetodoPagoId }, true, function (data) {

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
        JQueryAjax_Normal('/MetodoPago/CountTablaWhereMetodoPago', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/MetodoPago/CountTablaMetodoPago', {}, false, function (data) {
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
        query += "CFDIMetodoPagoId LIKE '%" + $("#txtFiltroIDWhere").val() + "%'"
    }
    if ($("#checkFiltroDescripcion").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "Descripcion LIKE '%" + $("#txtFiltroDescripcionWhere").val() + "%'"
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
/*BUSCADOR*/
/****************************/
function desplegarPaginacionBuscador(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusqueda').val().toLowerCase();
    JQueryAjax_Normal('/MetodoPago/elementosPaginacionBuscadorMetodoPago', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    input.val(item);
                    desplegarMarcaPorNombre(listItem.text());
                });
                opcionesLista.append(listItem);
            });
            opcionesLista.show();
            $('#botonesPaginado').show();
        }
    }, function () { });
}
function CountPaginadoBuscador(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/MetodoPago/countBuscadorMetodoPago', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}


$(document).ready(function () {
    JQueryAjax_Normal('/MetodoPago/tamaño', {}, true, function (campos) {
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
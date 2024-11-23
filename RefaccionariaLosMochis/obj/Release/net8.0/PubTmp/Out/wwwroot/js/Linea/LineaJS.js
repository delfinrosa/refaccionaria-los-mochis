/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />
/// <reference path="../general/jqueryajax.js" />

var rol = getCookie("tipo");

var dataTabla;
var filaSeleccionada;
var tablaInicializada = false;

var pagina = 0;
var paginaTabla = 0;

var query = 'I_A';
var activarWhere = false;

$('#iconoDescripcion').hide();
var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');

function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/Linea/ListarLineaWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Linea/ListarLinea', { strpagina: paginaTabla, tipoOrden: query, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }

    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';

    // Llena la tabla con los datos obtenidos
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.idLinea;
        row.insertCell(1).textContent = item.descripcion;
        var estadoCell = row.insertCell(2);

        // Añade badge según el estado
        if (item.activo === "A") {
            estadoCell.innerHTML = '<h5 class="m-auto"><span class="badge rounded-pill bg-success">Activo</span></h5>';
        } else if (item.activo === "O") {
            estadoCell.innerHTML = '<h5 class="m-auto"><span class="badge rounded-pill bg-secondary">Oculto</span></h5>';
        } else {
            estadoCell.innerHTML = '<h5 class="m-auto"><span class="badge rounded-pill bg-danger">Desactivado</span></h5>';
        }

        row.insertCell(3).textContent = item.deslc;

        var accionesCell = row.insertCell(4);
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
function Guardar() {
    var textActivo;
    if ($('#cboRegistroActivo').val() == "A") {
        textActivo = "A";
    } else if ($('#cboRegistroActivo').val() == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }
    debugger;
    var Linea = {
        IdLinea: $('#txtRegistroid').val(),
        Descripcion: $('#txtRegistroNombre').val().toUpperCase(),
        Activo: textActivo,
        Deslc: $('#txtRegistrodescripcion').val().toUpperCase(),
        oUsuario: {
            IdUsuario: "0"
        }
    }
    JQueryAjax_Normal('/Linea/GuardarLinea', { objeto: Linea }, false, function (data) {
        //Linea guardar
        if (Linea.IdLinea == 0) {
            if (data.resultado != 0) {
                $('#txtRegistroid').val(data.resultado);
                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
        }
        //Linea editar
        else {
            if (data.resultado) {
                $('#mensajeError').hide();
                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
        }
    }, function () { })
}
// Desplegar
function DesplegarInformacionCampos(json) {
    if (json != null) {
        console.log(json)
        $('#txtRegistroid').val(json.lista.idLinea);
        $('#txtRegistroNombre').val(json.lista.descripcion);
        if (json.lista.activo == "A") {
            $('#cboRegistroActivo').val("A");
        } else if (json.lista.activo == "O") {
            $('#cboRegistroActivo').val("O");
        } else {
            $('#cboRegistroActivo').val("D");
        }
        $('#txtRegistrodescripcion').val(json.lista.deslc);
    }
}
function desplegarLineaPorNombre(texto) {
    JQueryAjax_Normal('/Linea/bucarLineaPorNombre', { nombre: texto }, true, function (data) {
        if (data.Descripcion != "") {
            $('#txtRegistroNombre').val(data.Descripcion);
            $('#txtRegistrodescripcion').val(data.Deslc);
            $('#txtRegistroid').val(data.IdLinea);
            if (data.Activo == "A") {
                $('#cboRegistroActivo').val("A");
            } else if (data.Activo == "O") {
                $('#cboRegistroActivo').val("O");
            } else {
                $('#cboRegistroActivo').val("D");
            }
            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();
        } else {
            limpiar()
        }
    }, function () { }
    );
}
function limpiar() {
    $('#txtRegistroNombre').focus();

    $('#txtRegistroid').val("");
    $('#txtRegistroNombre').val("");
    $('#cboRegistroActivo').val("A");
    $('#txtRegistrodescripcion').val("");
}
///////////////
///Carga el ultimo
///////////////
function CargarUltimo() {
    $('#txtRegistroNombre').focus();

    JQueryAjax_Normal('/Linea/UltimoRegistro', {}, true, function (data) {
        DesplegarInformacionCampos(data);
    }, function () { });
}
////////////////
/// BUSCADOR
///////////////
function desplegarPaginacionBuscador(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusqueda').val().toLowerCase();
    JQueryAjax_Normal('/Linea/elementosPaginacionBuscador', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    input.val(item);
                    desplegarLineaPorNombre(listItem.text());
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
    JQueryAjax_Normal('/Linea/countBuscador', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}
//////////////
//Paginado Tabla
//////////////
function CountPaginadoTabla() {
    var resultado = 0;
    if ($("#checkFiltros").is(':checked')) {
        var where = whereQuery();
        JQueryAjax_Normal('/Linea/countTablaWhere', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Linea/countTabla', {}, false, function (data) {
            resultado = data.registros;
        }, function () { });
    }
    return resultado;
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

    debugger
    if (idBorrar != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar la Línea " + $("#txtRegistroNombre").val() + " ?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {

            JQueryAjax_Normal('/Linea/EliminarLinea', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Linea Eliminada", "la Linea se elimino correctamente", "success")
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
    if ($("#txtRegistroid").val() == 0) {
        modoGuardando();
    } else {
        modoEditando();
    }
});
$("#tablaId").click(function () {
    paginaTabla = 0;
    $('#iconoDescripcion').hide();
    $('#iconoID').show();
    query = (query == 'I_A') ? 'I_D' : 'I_A';
    var nuevoIconoClass = (query == 'I_A') ? 'fa-sort-numeric-down' : 'fa-sort-numeric-down-alt';
    $('#iconoID').removeClass('fa-sort-numeric-down fa-sort-numeric-down-alt').addClass(`${nuevoIconoClass} fa-2x m-2`);
    ActualizarTabla();
});
$("#tablaDescripcion").click(function () {
    paginaTabla = 0
    $('#iconoID').hide();
    $('#iconoDescripcion').show();
    query = (query == 'D_A') ? 'D_D' : 'D_A';
    var nuevoIconoClass = (query == 'D_A') ? 'fas fa-sort-alpha-down' : 'fas fa-sort-alpha-down-alt';
    $('#iconoDescripcion').removeClass('fas fa-sort-alpha-down fas fa-sort-alpha-down-alt').addClass(`${nuevoIconoClass} fa-2x m-2`);
    ActualizarTabla()
});


// Focus al input Buscar (Icono lupa)
$('#btn-BuscarBuscador').click(function () {
    $('#txtBusqueda').focus();
});

$("#btn-Actualizar").click(function () {
    ActualizarTabla();
});

//BOTON EDITAR
$("#tabla tbody").on("click", ".btn-editar", function () {
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', false);
    $('#btn-Eliminar').prop('disabled', false);
    $('#btn-Cancelar').prop('disabled', false);
    $('#txtRegistroNombre').focus();

    var rowIndex = $(this).closest("tr").index();
    var linea = dataTabla[rowIndex];
    JQueryAjax_Normal('/Linea/ListarPorIdLineas', { Id: linea.idLinea }, false,
        function (data) {
            DesplegarInformacionCampos(data);
        }, function () { });
});

//BOTON ELIMINAR
$("#tabla").on("click", ".btn-eliminar", function () {

    var rowIndex = $(this).closest("tr").index(); // Obtener el índice de la fila
    var linea = dataTabla[rowIndex]; // Obtener el objeto de la fila correspondiente
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Linea " + linea.Descripcion + " ?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {

            JQueryAjax_Normal('/Linea/EliminarLinea', { id: linea.IdLinea }, true, function (data) {
                if (data.resultado) {
                    dataTabla.splice(rowIndex, 1);
                    datosTablaPaginado(paginaTabla, $("#cboDivicion-tabla").val());
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Linea Eliminada", "la Linea  se elimino correctamente", "success")
                    }, 250);
                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });
        });
})

////////////
//INPUT
////////////

$("#txtRegistroNombre").on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});

$("#txtRegistrodescripcion").on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});

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

// Ocultar opcionesLista cuando se hace clic fuera de ella
$(document).on('click', function (event) {

    if (!$(event.target).closest('#buscador').length) {
        $('#botonesPaginado').hide();
        opcionesLista.hide();
    }
});

///////////
//AL CARGAR
///////////

function cargartabla() {
    datosTablaPaginado(paginaTabla, 10);
    var divisiona = Math.ceil(CountPaginadoTabla() / 10);
    configurarPaginacion(paginaTabla, divisiona, datosTablaPaginado, "T", 10);
}

$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtroslinea').slideDown();
    } else {
        $('#filtroslinea').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
});

function whereQuery() {
    var query = ""
    if ($("#checkFiltroId").prop('checked')) {
        query += "l.IdLinea LIKE '%" + $("#txtFiltroIDWhere").val() + "%'"
    }
    if ($("#checkFiltroDescripcion").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "l.Descripcion LIKE '%" + $("#txtFiltroDescripcionWhere").val() + "%'"
    } if ($("#checkFiltroDescripcionLinea").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "lc.Descripcion LIKE '%" + $("#txtFiltroDescripcionLineaWhere").val() + "%'"
    }
    return query
}

$("#btn-buscarTabla").click(function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla()
    }
    else {
        $("#filtroslinea").focus();
    }
});

$("#tabla tbody").on("click", '#masInformacion', function () {
    if ($("#checkFiltros").prop('checked')) {
        swal({
            title: "Filtro Activo",
            text: "Puede que el filtro este mal, ¿quiere quitarlo?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            cancelButtonClass: "btn-success",
            confirmButtonText: "Si",
            cancelButtonText: "No",
            closeOnConfirm: true,
            closeOnCancel: true
        },
            function (isConfirm) {
                if (isConfirm) {
                    desactivarFiltro();
                    ActualizarTabla();
                } else {
                    $("#checkFiltros").focus();
                }
            });
    } else {
        swal({
            title: "Fallo en la coneccion con la base de datos",
            text: "Espere un momento en lo que se intenta reconectar con la base de datos",
            type: "info",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true
        }, function () {
            setTimeout(function () {
                JQueryAjax_Normal('/Linea/ChecarConexion', {}, true, function (data) {
                    if (data.resultado) {
                        swal("Coneccion Exitosa", "La conceccion se realizo correctamente", "success");
                        desactivarFiltro();
                        ActualizarTabla();
                    } else {
                        swal("No se pudo conectar", "Se presento un fallo en la coneccion", "error");
                    }
                }, function () { });
            }, 2000);
        });
    }
});
function ActualizarTabla() {
    datosTablaPaginado(0, $("#cboDivicion-tabla").val());
    var division = Math.ceil(CountPaginadoTabla() / $("#cboDivicion-tabla").val());
    configurarPaginacion(0, division, datosTablaPaginado, "T", $("#cboDivicion-tabla").val())
}

function MarcadorTabla() {
    marcarCeldas(0, $("#txtFiltroIDWhere").val(), 'yellow');
    marcarCeldas(1, $("#txtFiltroDescripcionWhere").val(), '#FCC208');
    marcarCeldas(3, $("#txtFiltroDescripcionLineaWhere").val(), '#00943A');

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

function desactivarFiltro() {
    $("#checkFiltros").prop('checked', false);
    $("#checkFiltroId").prop('checked', false);
    $("#checkFiltroDescripcion").prop('checked', false);
    $("#checkFiltroDescripcionLinea").prop('checked', false);

    $("#checkFiltroId").removeAttr('data-requerido-filtro');
    $("#checkFiltroDescripcion").removeAttr('data-requerido-filtro');
    $("#checkFiltroDescripcionLinea").removeAttr('data-requerido-filtro');

    $("#txtFiltroIDWhere").prop('disabled', true);
    $("#txtFiltroDescripcionWhere").prop('disabled', true);
    $("#txtFiltroDescripcionLineaWhere").prop('disabled', true);

    $("#txtFiltroIDWhere").val("");
    $("#txtFiltroDescripcionWhere").val("");
    $("#txtFiltroDescripcionLineaWhere").val("");

    $('#filtroslinea').attr('hidden', true);


}

CargarUltimo();
cargartabla()
eventoCheckFiltro($("#checkFiltroId"), $("#txtFiltroIDWhere"));
eventoCheckFiltro($("#checkFiltroDescripcion"), $("#txtFiltroDescripcionWhere"));
eventoCheckFiltro($("#checkFiltroDescripcionLinea"), $("#txtFiltroDescripcionLineaWhere"));

$(document).ready(function () {
    JQueryAjax_Normal('/Linea/tamaño', {}, true, function (campos) {
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

$(document).ready(function () {
    JQueryAjax_Normal('/Linea/tamañoCaracteristicas', {}, true, function (campos) {
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="DescripcionLinea"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                }
            }
        });
    }, function () { });



});

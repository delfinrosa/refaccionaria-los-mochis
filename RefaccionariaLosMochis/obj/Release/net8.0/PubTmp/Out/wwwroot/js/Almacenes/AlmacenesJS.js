/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />
/// <reference path="../general/jqueryajax.js" />

var rol = getCookie("tipo");

var dataTabla;
var filaSeleccionada;
var tablaInicializada = false;

var boolGuardar = false;

var pagina = 0;
var paginaTabla = 0;

var query = 'I_A';
var activarWhere = false;

$('#iconoDescripcion').hide();
var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');
var boolGuardar = 0;

function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/Almacen/ListarAlmacenWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Almacen/ListarAlmacen', { strpagina: paginaTabla, tipoOrden: query, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }

    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';

    // Llena la tabla con los datos obtenidos
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.AlmacenId;
        row.insertCell(1).textContent = item.Descripcion;

        row.insertCell(2).textContent = item.Ubicacion;

        var accionesCell = row.insertCell(3);
        accionesCell.innerHTML = '<span class="icono-texto-container">' +
            '<a href="#cardRegistros"><button type="button" class="btn btn-success btn-sm btn-ver ms-2"><i class="fas fa-eye"></i></button></a>' +
            '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar ms-2"><i class="fas fa-pen"></i></button></a>' +
            '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button></span> ';
    });


    if ($("#checkFiltros").is(':checked')) {
        if (dataTabla.length != 0) {
            AlmacendorTabla()
        }
        else {
            swal("Filtro NO valido", "No se a encontrado ningun Registro con el Filtro puesto")
        }
    }
}
function datosTablaPaginadoRack(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/Almacen/ListarAlmacenWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Almacen/ListarRacks', { strpagina: paginaTabla, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }

    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';

    // Llena la tabla con los datos obtenidos
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.RackId;
        row.insertCell(1).textContent = item.Ubicacion;
        row.insertCell(2).textContent = item.Descripcion;


        var accionesCell = row.insertCell(3);
        accionesCell.innerHTML = '<span class="icono-texto-container">' +
            '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
            '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button></span>';
    });


    if ($("#checkFiltros").is(':checked')) {
        if (dataTabla.length != 0) {
            AlmacendorTabla()
        }
        else {
            swal("Filtro NO valido", "No se a encontrado ningun Registro con el Filtro puesto")
        }
    }
}

/////////
//METODOS
/////////
function GuardarAlmacen() {
    var Almacen = {
        AlmacenId: $('#txtRegistroidAlmacen').val(),
        Descripcion: $('#txtRegistroDescripcionAlmacen').val().toUpperCase(),
        Ubicacion: $('#txtRegistroUbicacionAlmacen').val().toUpperCase(),
        oUsuario: {
            UsuarioId: 0
        }
    }
    JQueryAjax_Normal('/Almacen/GuardarAlmacen', { objeto: Almacen }, true, function (data) {
        //Almacen guardar
        if (Almacen.AlmacenId == 0) {
            if (data.resultado != 0) {
                $('#txtRegistroid').val(data.resultado);
                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroUbicacionAlmacen').val(), "success")
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
            boolGuaradar = 0;
        }
        //Almacen editar
        else {
            if (data.resultado) {
                $('#mensajeError').hide();
                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroUbicacion').val(), "success")
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
        }
    }, function () { })
}
//GUARDAR RACK 
function GuardarAlmacenRack() {
    var AlmacenRack = {
        oAlmacen: {
            AlmacenId: $('#txtRegistroidAlmacen').val()
        },
        RackId: $('#txtRegistroidAlmacenRack').val(),
        Descripcion: $('#txtRegistroDescripcionAlmacenRack').val().toUpperCase(),
        Ubicacion: $('#txtRegistroUbicacionAlmacenRack').val().toUpperCase(),
        oUsuario: {
            UsuarioId: 0
        }
    };

    JQueryAjax_Normal('/Almacen/GuardarAlmacenRack', { objeto: AlmacenRack, boolGuardar: boolGuardar }, false, function (data) {
        //Almacen guardar
        console.log(boolGuardar)
        if (boolGuardar) {
            if (data.resultado) {
                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroUbicacionAlmacenRack').val(), "success")
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
            boolGuaradar = 0;

        }
        //Almacen editar
        else {
            if (data.resultado) {
                $('#mensajeError').hide();
                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroUbicacionAlmacenRack').val(), "success")
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
function DesplegarCamposaAlmacen(json) {
    if (json != null) {
        $('#txtRegistroidAlmacen').val(json.Lista.AlmacenId);
        $('#txtRegistroDescripcionAlmacen').val(json.Lista.Descripcion);
        $('#txtRegistroUbicacionAlmacen').val(json.Lista.Ubicacion);
    }
}
function desplegarAlmacenPorNombre(texto) {
    JQueryAjax_Normal('/Almacen/bucarAlmacenPorNombre', { nombre: texto }, true, function (data) {
        if (data.Lista.Descripcion != "") {
            DesplegarCamposaAlmacen(data)
            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();
        } else {
            limpiar()
        }
    }, function () { }
    );
}
function limpiarAlmacen() {
    $('#txtRegistroidAlmacen').val("");
    $('#txtRegistroUbicacionAlmacen').val("");
    $('#txtRegistroDescripcionAlmacen').val("");
    $('#txtRegistroUbicacionAlmacen').focus();
}
function limpiarRack() {
    $('#txtRegistroidAlmacenRack').val("");
    $('#txtRegistroUbicacionAlmacenRack').val("");
    $('#txtRegistroDescripcionAlmacenRack').val("");
    $('#txtRegistroUbicacionAlmacenRack').focus();
}
function limpiarSecciones() {
    $('#txtRegistroIdSeccion').val("");
    $('#txtRegistroUbicacionSeccion').val("");
    $('#txtRegistroDescripcionSeccion').val("");
    $('#txtRegistroIdSeccion').focus();
}

function DesplegarCamposaAlmacenRack(data) {

    $('#txtRegistroidAlmacenRack').val(data.Lista.RackId);
    $('#txtRegistroUbicacionAlmacenRack').val(data.Lista.Ubicacion);
    $('#txtRegistroDescripcionAlmacenRack').val(data.Lista.Descripcion);
}

///////////////
///Carga el ultimo
///////////////
function CargarUltimoAlmacen() {
    $('#txtRegistroUbicacion').focus();

    JQueryAjax_Normal('/Almacen/UltimoRegistro', {}, true, function (data) {
        DesplegarCamposaAlmacen(data);
    }, function () { });
}
function CargarUltimoRack(id) {
    $('#txtRegistroidAlmacenRack').focus();

    JQueryAjax_Normal('/Almacen/UltimoRegistroRack', { id: id }, false, function (data) {
        if (data.Lista.Ubicacion != null) {
            DesplegarCamposaAlmacenRack(data);
        } else {
            limpiarRack()
            $('#tabDatosAlmacenRack input, #tabDatosAlmacenRack textarea').prop('disabled', true);
            $('#btn-GuardarRack').prop('disabled', false);
            $('#btn-EliminarRack').prop('disabled', false);

        }
    }, function () { });
}
////////////////
/// BUSCADOR
///////////////
function desplegarPaginacionBuscador(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusqueda').val().toLowerCase();
    JQueryAjax_Normal('/Almacen/ElementosPaginacionBuscadorAlmacen', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    input.val(item);
                    desplegarAlmacenPorNombre(listItem.text());
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
    JQueryAjax_Normal('/Almacen/CountBuscadorAlmacenAsync', { nombre: texto }, false, function (data) {
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
        JQueryAjax_Normal('/Almacen/countTablaWhere', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Almacen/CountTablaAlmacen', {}, false, function (data) {
            resultado = data.registros;
        }, function () { });
    }
    return resultado;
}
//////////
//BOTONES
//////////

//Barra de heramientas 
$("#btn-NuevoAlmacen").click(function () {
    limpiarAlmacen();
    $('#btn-GuardarAlmacen').prop('disabled', false);
    $('#btn-NuevoAlmacen').prop('disabled', true);
    $('#btn-EliminarAlmacen').prop('disabled', true);
    $('#btn-CancelarAlmacen').prop('disabled', false);
    modoGuardando()
    document.getElementById('SpantabDatosAlmacen').textContent = '2';
    boolGuardar = 1;

});
$("#btn-NuevoRack").click(function () {
    $('#tabDatosAlmacenRack input, #tabDatosAlmacenRack textarea').prop('disabled', false);
    $('#btn-EliminarRack').prop('disabled', false);
    limpiarRack();
    $('#btn-NuevoRack').prop('disabled', true);
    $('#btn-CancelarRack').prop('disabled', false);
    modoGuardando()
    boolGuardar = true;
    $('#btn-GuardarRack').prop('disabled', false);
    $('#btn-EliminarRack').prop('disabled', false);
    document.getElementById('SpantabDatosRack').textContent = '3';
    boolGuardar = 1;
    $("#txtRegistroAlmacenRack").val($("#txtRegistroUbicacionAlmacen").val());

});
$("#btn-CancelarRack").click(function () {
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
            CargarUltimoRack($("#txtRegistroidAlmacen").val())
            $('#btn-EliminarRack').prop('disabled', false);
            $('#btn-CancelarRack').prop('disabled', true);
            $('#btn-GuardarRack').prop('disabled', true);
            $('#btn-NuevoRack').prop('disabled', false);
            $('#modo').hide();

            $(".bg-edicion").removeClass("bg-edicion");
            $(".bg-guardar").removeClass("bg-guardar");
            Navegar()
            document.getElementById('SpantabDatosRack').textContent = '';

            boolGuardar = 0;

        });



});
$("#btn-CancelarAlmacen").click(function () {
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
            CargarUltimoAlmacen();
            $('#btn-Eliminar').prop('disabled', false);
            $('#btn-Cancelar').prop('disabled', true);
            $('#btn-Guardar').prop('disabled', true);
            $('#btn-Nuevo').prop('disabled', false);
            $('#modo').hide();

            $(".bg-edicion").removeClass("bg-edicion");
            $(".bg-guardar").removeClass("bg-guardar");
            Navegar()
            document.getElementById('SpantabDatosAlmacen').textContent = '';

            boolGuardar = 0;

        });



});
$("#btn-GuardarAlmacen").click(function () {
    if (ValidarCampoVacio($('[data-requerido]'))) {
        $('#btn-Eliminar').prop('disabled', false);

        $('#btn-Guardar').prop('disabled', true);
        $('#btn-Cancelar').prop('disabled', true);
        $('#btn-Nuevo').prop('disabled', false);

        $('#tabDatosAlmacenRack-tab').prop('disabled', false);
        GuardarAlmacen();
        ActualizarTabla();
        CargarUltimoAlmacen();

        $(".bg-edicion").removeClass("bg-edicion");
        $(".bg-guardar").removeClass("bg-guardar");
        Navegar()

        $('#modo').hide();

    }
});
$("#btn-GuardarRack").click(function () {

    if (ValidarCampoVacio($('[data-requerido]'))) {
        $('#btn-EliminarRack').prop('disabled', false);

        $('#btn-GuardarRack').prop('disabled', true);
        $('#btn-CancelarRack').prop('disabled', true);
        $('#btn-NuevoRack').prop('disabled', false);

        $(".bg-edicion").removeClass("bg-edicion");
        $(".bg-guardar").removeClass("bg-guardar");
        Navegar()

        $('#modo').hide();
        GuardarAlmacenRack();
        CargarUltimoRack($("#txtRegistroidAlmacen").val())
        boolGuardar = false;


    }
});
$("#btn-EliminarAlmacen").click(function () {
    $('#btn-GuardarAlmacen').prop('disabled', false);
    var idBorrar = $("#txtRegistroidAlmacen").val();
    if (idBorrar != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar el Almacen de " + $("#txtRegistroUbicacionAlmacen").val() + " ?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/Almacen/EliminarAlmacen', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimoAlmacen();
                    setTimeout(function () {
                        swal("Almacen Eliminada", "la Almacen se elimino correctamente", "success")
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
$("#btn-EliminarRack").click(function () {
    $('#btn-GuardarRack').prop('disabled', false);
    var rackId = $("#txtRegistroidAlmacenRack").val();
    var almacenId = $("#txtRegistroidAlmacen").val();
    if (rackId != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar el Rack " + $("#txtRegistroUbicacionAlmacenRack").val() + " del Almacen " + $("#txtRegistroUbicacionAlmacen").val() + " ?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/Almacen/EliminarRack', { almacenId: almacenId, rackId: rackId }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimoRack($("#txtRegistroidAlmacen").val())
                    setTimeout(function () {
                        swal("Rack Eliminado", "la Rack del Almacen se elimino correctamente", "success")
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
    $('#btn-CancelarRack').prop('disabled', false);
    $('#btn-GuardarRack').prop('disabled', false);
    $('#btn-CancelarAlmacen').prop('disabled', false);
    $('#btn-GuardarAlmacen').prop('disabled', false);
    if (boolGuardar == true) {
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
            JQueryAjax_Normal('/Almacen/EliminarAlmacen', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimoAlmacen();
                    setTimeout(function () {
                        swal("Almacen Eliminada", "la Almacen se elimino correctamente", "success")
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
    $('#btn-Nuevo').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', true);
    $('#txtRegistroNombre').focus();

    $('[data-requerido]').removeAttr('data-requerido');
    $('#txtRegistroUbicacionAlmacen').attr('data-requerido', 'Ubicacion De Almacen');
    $('#txtRegistroDescripcionAlmacen').attr('data-requerido', 'Descripcion De Almacen');

    var rowIndex = $(this).closest("tr").index();
    var Almacen = dataTabla[rowIndex];
    JQueryAjax_Normal('/Almacen/BuscarAlmacenPorId', { Id: Almacen.AlmacenId }, false,
        function (data) {
            DesplegarCamposaAlmacen(data);
        }, function () { });
    $('#tabDatosAlmacen-tab').tab('show');  // Asegúrate de que el ID es correcto

});

//BOTON VER
$("#tabla tbody").on("click", ".btn-ver", function () {

    $('[data-requerido]').removeAttr('data-requerido');
    $('#txtRegistroidAlmacenRack').attr('data-requerido', 'ID Del Rack');
    $('#txtRegistroUbicacionAlmacenRack').attr('data-requerido', 'Ubicacio Del Rack');
    $('#txtRegistroAlmacenRack').attr('data-requerido', 'Almacen Del Rack');
    $('#txtRegistroDescripcionAlmacenRack').attr('data-requerido', 'Descripcion Del Rack');

    var rowIndex = $(this).closest("tr").index();
    var Almacen = dataTabla[rowIndex];

    JQueryAjax_Normal('/Almacen/BuscarAlmacenPorId', { Id: Almacen.AlmacenId }, false,
        function (data) {
            DesplegarCamposaAlmacen(data);
        }, function () { });


    $("#txtRegistroidAlmacen").val(Almacen.AlmacenId)
    CargarUltimoRack(Almacen.AlmacenId)
    $("#txtRegistroAlmacenRack").val($("#txtRegistroUbicacionAlmacen").val());
    $("#AlmacenSeleccionado").text("Almacen: " + $("#txtRegistroUbicacionAlmacen").val());

    var camposRequeridos = document.querySelectorAll('[data-requerido]');

    camposRequeridos.forEach(function (element) {
        function manejarCambioYClick() {
            var tabId = element.closest('.tab-pane').id;
            contarCamposVaciosEnTiempoReal(tabId);
        }

        element.addEventListener('change', manejarCambioYClick);
        element.addEventListener('click', manejarCambioYClick);

    });

    cargarRacksPorAlmacen($("#txtRegistroidAlmacen").val())

    $('#tabDatosRack-tab').tab('show');  // Asegúrate de que el ID es correcto

    $(".bg-edicion").removeClass("bg-edicion");
    $(".bg-guardar").removeClass("bg-guardar");
    Navegar()
});

//BOTON ELIMINAR
$("#tabla").on("click", ".btn-eliminar", function () {

    var rowIndex = $(this).closest("tr").index(); // Obtener el índice de la fila
    var Almacen = dataTabla[rowIndex]; // Obtener el objeto de la fila correspondiente
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Almacen?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Almacen/EliminarAlmacen', { id: Almacen.AlmacenId }, true, function (data) {
                if (data.resultado) {
                    dataTabla.splice(rowIndex, 1);
                    datosTablaPaginado(paginaTabla, $("#cboDivicion-tabla").val());
                    setTimeout(function () {
                        swal("Almacen Eliminada", "la Almacen se elimino correctamente", "success")
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

$('#txtBusqueda').on('input', async function () {



    var NumeroDivisiones = $("#cboDivicion-buscador").val() || 5;
    var pagina = 0;
    desplegarPaginacionBuscador(pagina, NumeroDivisiones);
    configurarPaginacion(pagina, desplegarPaginacionBuscador, "B", NumeroDivisiones);
    var division = await CountPaginadoBuscador($('#txtBusqueda').val().toLowerCase(), NumeroDivisiones);
    $('#numPaginadoBuscador').text(pagina + 1 + ' de ' + division);


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
        $('#filtrosAlmacen').slideDown();
    } else {
        $('#filtrosAlmacen').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
});

function whereQuery() {
    var query = ""
    if ($("#checkFiltroId").prop('checked')) {
        query += "l.IdAlmacen LIKE '%" + $("#txtFiltroIDWhere").val() + "%'"
    }
    if ($("#checkFiltroDescripcion").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "l.Descripcion LIKE '%" + $("#txtFiltroDescripcionWhere").val() + "%'"
    } if ($("#checkFiltroDescripcionAlmacen").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "lc.Descripcion LIKE '%" + $("#txtFiltroDescripcionAlmacenWhere").val() + "%'"
    }
    return query
}

$("#btn-buscarTabla").click(function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla()
    }
    else {
        $("#filtrosAlmacen").focus();
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
                JQueryAjax_Normal('/Almacen/ChecarConexion', {}, true, function (data) {
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

function AlmacendorTabla() {
    AlmacenrCeldas(0, $("#txtFiltroIDWhere").val(), 'yellow');
    AlmacenrCeldas(1, $("#txtFiltroDescripcionWhere").val(), '#FCC208');
    AlmacenrCeldas(3, $("#txtFiltroDescripcionAlmacenWhere").val(), '#00943A');

}

function AlmacenrCeldas(columna, filtro, color) {
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
    $("#checkFiltroDescripcionAlmacen").prop('checked', false);

    $("#checkFiltroId").removeAttr('data-requerido-filtro');
    $("#checkFiltroDescripcion").removeAttr('data-requerido-filtro');
    $("#checkFiltroDescripcionAlmacen").removeAttr('data-requerido-filtro');

    $("#txtFiltroIDWhere").prop('disabled', true);
    $("#txtFiltroDescripcionWhere").prop('disabled', true);
    $("#txtFiltroDescripcionAlmacenWhere").prop('disabled', true);

    $("#txtFiltroIDWhere").val("");
    $("#txtFiltroDescripcionWhere").val("");
    $("#txtFiltroDescripcionAlmacenWhere").val("");

    $('#filtrosAlmacen').attr('hidden', true);


}

CargarUltimoAlmacen();

eventoCheckFiltro($("#checkFiltroId"), $("#txtFiltroIDWhere"));
eventoCheckFiltro($("#checkFiltroDescripcion"), $("#txtFiltroDescripcionWhere"));
eventoCheckFiltro($("#checkFiltroDescripcionAlmacen"), $("#txtFiltroDescripcionAlmacenWhere"));

function cargarTodasLasSecciones() {

    $('#tSecciones').empty();
    JQueryAjax_Normal('/Almacen/ObtenerSeccionesPorAlmacenYRack', { almacenId: $("#txtRegistroidAlmacen").val(), rackId: $("#txtRegistroidAlmacenRack").val() }, true, function (data) {

        data.data.forEach(function (seccion) {
            var fila = `<tr>
        <td class="col-3 m-2" style="border-right: 2px solid #000;">
            <input class="form-control" value="${seccion.oAlmacen.Ubicacion}" disabled />
        </td>
        <td class="col-3 m-2" style="border-right: 2px solid #000;">
            <input class="form-control" value="${seccion.oRack.Ubicacion}" disabled />
        </td>
        <td class="col-5">
            <div class="row">
                <div class="col-4">
                    <input class="form-control" value="${seccion.SeccionId}" disabled placeholder="ID de la Sección" />
                </div>                                
                <div class="col-4">
                    <input class="form-control" value="${seccion.Ubicacion}" disabled placeholder="Ubicación de la Sección" />
                </div>
                <div class="col-4">
                    <input class="form-control" value="${seccion.Descripcion}" disabled placeholder="Descripción de la Sección" />
                </div>
            </div>
        </td>
        <td class="col-1 ">
            <button type="button" class="eliminar-seccion  btn btn-danger bg-danger text-white btn-sm ms-2 "
                data-almacenid="${seccion.oAlmacen.AlmacenId}" 
                data-rackid="${seccion.oRack.RackId}" 
                data-seccionid="${seccion.SeccionId}"><i class="fas fa-trash"></i> 
            </button>
            </td>
    </tr>`;
            $('#tSecciones').append(fila);
        });



    },
        function () {
        }
    );
}


$("#GuardarSeccion").click(function () {
    var boolGuardar = true;
    var AlmacenRackSeccion = {
        oAlmacen: {
            AlmacenId: $("#txtRegistroidAlmacen").val()
        },
        oRack: {
            RackId: $("#inputRack").data('requerido')
        },
        SeccionId: $('#txtRegistroIdSeccion').val().toUpperCase(),
        Descripcion: $('#txtRegistroDescripcionSeccion').val().toUpperCase(),
        Ubicacion: $('#txtRegistroUbicacionSeccion').val().toUpperCase(),
        oUsuario: {
            UsuarioId: 0
        }
    };

    JQueryAjax_Normal('/Almacen/GuardarAlmacenRackSeccion', { objeto: AlmacenRackSeccion, boolGuardar: boolGuardar }, false, function (data) {
        //Almacen guardar
        if (data.mensaje == "") {
            swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroUbicacionSeccion ').val(), "success")
            $('#mensajeError').hide();
            cargarTodasLasSecciones()
            limpiarSecciones();
        }
        else {
            $('#mensajeError').text(data.mensaje);
            $('#mensajeError').show();
        }

    }, function () { })
});



$("#inputAlmacen").on("input focus", function () {
    var valor = $(this).val().toLowerCase();
    JQueryAjax_Normal('/Almacen/SeleccionarAlmacen', { ParametroBusqueda: valor }, true, function (data) {
        $("#listaAlmacenes").show().empty();
        if (data.mensaje === "") {
            $.each(data.data, function (index, almacen) {
                if (almacen.Ubicacion.toLowerCase().includes(valor)) {
                    $("#listaAlmacenes").append('<div class="opcion-almacen" data-requerido="' + almacen.AlmacenId + '">' + almacen.Ubicacion + '</div>');
                }
            });
        } else {
            swal("Se ha producido un error", "Error: " + data.mensaje, "error");
        }
    }, function (errorThrown) {
        console.error("Error al obtener almacenes: " + errorThrown);
    });
});

$(document).on("click", ".opcion-almacen", function () {
    var texto = $(this).text();
    var almacenId = $(this).data('requerido');
    $("#inputAlmacen").val(texto).data('requerido', almacenId); // Actualiza el uso de data para guardar el ID
    $("#listaAlmacenes").hide();
    // Limpia el input de rack y la selección previa
    $("#inputRack").val('').removeData('requerido');
    $("#listaRacks").empty();
});

$("#inputRack").on("input focus", function () {
    var valor = $(this).val().toLowerCase();
    var almacenId = $("#inputAlmacen").data('requerido');
    if (almacenId) {
        JQueryAjax_Normal('/Almacen/SeleccionarAlmacenRack', { id: almacenId, ParametroBusqueda: valor }, true, function (data) {
            $("#listaRacks").show().empty();
            if (data.mensaje === "") {
                $.each(data.data, function (index, rack) {
                    if (rack.Ubicacion.toLowerCase().includes(valor)) {
                        $("#listaRacks").append('<div class="opcion-rack" data-requerido="' + rack.RackId + '">' + rack.Ubicacion + '</div>');


                    }
                });
            } else {
                swal("Se ha producido un error", "Error: " + data.mensaje, "error");
            }
        }, function (errorThrown) {
            console.error("Error al obtener racks: " + errorThrown);
        });
    } else {
        swal("Seleccione un almacén primero", "", "warning");
    }
});

$(document).on("click", ".opcion-rack", function () {
    var texto = $(this).text();
    var rackId = $(this).data('requerido');
    $("#inputRack").val(texto).data('requerido', rackId); // Guarda el ID del rack seleccionado
    $("#listaRacks").hide();
    $('#txtRegistroidAlmacenRack').val(rackId);
    cargarTodasLasSecciones()
});

$(document).on("click", function (e) {
    if (!$(e.target).closest("#inputAlmacen, #listaAlmacenes").length) {
        $("#listaAlmacenes").hide();
    }
    if (!$(e.target).closest("#inputRack, #listaRacks").length) {
        $("#listaRacks").hide();
    }
});

$(document).on("click", ".eliminar-seccion", function () {
    var $fila = $(this).closest('tr');
    var Objeto = {
        oAlmacen: {
            AlmacenId: $fila.find(".eliminar-seccion").data('almacenid')
        },
        oRack: {
            RackId: $fila.find(".eliminar-seccion").data('rackid')
        },
        SeccionId: $fila.find(".eliminar-seccion").data('seccionid')
    };
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la seccion?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Almacen/EliminarSeccion', { Objeto: Objeto }, true, function (data) {
                if (data.resultado) {
                    cargarTodasLasSecciones()
                    setTimeout(function () {
                        swal("Seccion Eliminada", "la Seccion se elimino correctamente", "success")
                    }, 250);
                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });
        });
});

function cargarRacksPorAlmacen(almacenId) {
    $('#tRacks tbody').empty(); // Asegúrate de tener un tbody con este ID en tu tabla HTML
    JQueryAjax_Normal('/Almacen/ObtenerRacksPorAlmacen', { almacenId: almacenId }, true, function (data) {
        data.data.forEach(function (rack) {
            var filas = `<tr>
    <td class="col-3" style="border-right: 2px solid #000;">
        <input class="form-control" value="${rack.RackId}" disabled />
    </td>
    <td class="col-3" style="border-right: 2px solid #000;">
        <input class="form-control" value="${rack.Ubicacion}" disabled />
    </td>
    <td class="col-3">
        <input class="form-control" value="${rack.Descripcion}" disabled placeholder="Descripción del Rack" />
    </td>
    <td class="col-1">
        <button type="button" class="btn btn-sm btn-danger btn-Rackeliminar"
            data-rackid="${rack.RackId}"><i class="fas fa-trash"></i></button>
    </td>
    <td class="col-1">
        <button type="button" class="btn btn-sm btn-primary btn-Rackeditar"
            data-rackid="${rack.RackId}"><i class="fas fa-edit"></i></button>
    </td>
    <td class="col-1">
        <button type="button" class="btn btn-sm btn-success btn-RackSeccion"
            data-rackid="${rack.RackId}"><i class="fas fa-eye"></i></button>
    </td>
</tr>`;

            $('#tRacks tbody').append(filas);

        });
    }, function () {
        console.log("Error al cargar los racks.");
    });
}
$(document).on('shown.bs.tab', '#tabDatosAlmacen-tab', function () {
    $('[data-requerido]').removeAttr('data-requerido');
    $('#txtRegistroUbicacionAlmacen').attr('data-requerido', 'Ubicacion De Almacen');
    $('#txtRegistroDescripcionAlmacen').attr('data-requerido', 'Descripcion De Almacen');
});
$(document).on('shown.bs.tab', '#tabDatosRack-tab', function () {

    $('[data-requerido]').removeAttr('data-requerido');
    $('#txtRegistroidAlmacenRack').attr('data-requerido', 'ID Del Rack');
    $('#txtRegistroUbicacionAlmacenRack').attr('data-requerido', 'Ubicacio Del Rack');
    $('#txtRegistroAlmacenRack').attr('data-requerido', 'Almacen Del Rack');
    $('#txtRegistroDescripcionAlmacenRack').attr('data-requerido', 'Descripcion Del Rack');

    CargarUltimoRack($("#txtRegistroidAlmacen").val())
    $("#txtRegistroAlmacenRack").val($("#txtRegistroUbicacionAlmacen").val());

    var camposRequeridos = document.querySelectorAll('[data-requerido]');

    camposRequeridos.forEach(function (element) {
        function manejarCambioYClick() {
            var tabId = element.closest('.tab-pane').id;
            contarCamposVaciosEnTiempoReal(tabId);
        }

        element.addEventListener('change', manejarCambioYClick);
        element.addEventListener('click', manejarCambioYClick);

    });

    cargarRacksPorAlmacen($("#txtRegistroidAlmacen").val())

    $("#AlmacenSeleccionado").text("Almacen: " + $("#txtRegistroUbicacionAlmacen").val());



    $(".bg-edicion").removeClass("bg-edicion");
    $(".bg-guardar").removeClass("bg-guardar");
    Navegar()
});
$(document).on('shown.bs.tab', '#tabDatosAlmacenSeccion-tab', function () {
    $('[data-requerido]').removeAttr('data-requerido');


    //$('#inputRack').attr('data-requerido', 'Rack De La Seccion');
    //$('#txtRegistroIdSeccion').attr('data-requerido', 'ID De La Seccion');
    //$('#txtRegistroUbicacionSeccion').attr('data-requerido', 'Ubicacion De La Seccion');
    //$('#txtRegistroDescripcionSeccion').attr('data-requerido', 'Descripcion De La Seccion');

    $("#inputAlmacen").val($("#txtRegistroUbicacionAlmacen").val());
    $('#inputAlmacen').attr('data-requerido', $("#txtRegistroidAlmacen").val());

    cargarTodasLasSecciones()

    var camposRequeridos = document.querySelectorAll('[data-requerido]');

    camposRequeridos.forEach(function (element) {
        function manejarCambioYClick() {
            var tabId = element.closest('.tab-pane').id;
            contarCamposVaciosEnTiempoReal(tabId);
        }

        element.addEventListener('change', manejarCambioYClick);
        element.addEventListener('click', manejarCambioYClick);

    });
});


$(document).ready(function () {
    JQueryAjax_Normal('/Almacen/tamaño', {}, true, function (campos) {
        console.log(campos)
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var inputs = document.querySelectorAll('[name="' + item.Item1 + '"]');
                inputs.forEach(function (input) {
                    if (input) {
                        input.setAttribute('maxlength', item.Item2);
                    }
                });
            }
        });

    }, function () { });



});


$("#tRacks tbody").on("click", ".btn-RackSeccion", function () {
    var $row = $(this).closest("tr");

    var rackId = $row.find("td:nth-child(1) input").val();  // Asume que el ID del Rack está en el primer td
    var descripcion = $row.find("td:nth-child(2) input").val();  // Asume que la descripción está en el segundo td
    var ubicacion = $row.find("td:nth-child(3) input").val();  // Asume que la ubicación está en el tercer td

    var objetoParaDesplegar = {
        Lista: {
            RackId: rackId,
            Descripcion: descripcion,
            Ubicacion: ubicacion
        }
    };
    DesplegarCamposaAlmacenRack(objetoParaDesplegar);

    $('#tabDatosAlmacenSeccion-tab').tab('show');  // Asegúrate de que el ID es correcto

});
$("#tRacks tbody").on("click", ".btn-Rackeditar", function () {
    var $row = $(this).closest("tr");

    var rackId = $row.find("td:nth-child(1) input").val();  // Asume que el ID del Rack está en el primer td
    var descripcion = $row.find("td:nth-child(2) input").val();  // Asume que la descripción está en el segundo td
    var ubicacion = $row.find("td:nth-child(3) input").val();  // Asume que la ubicación está en el tercer td

    var objetoParaDesplegar = {
        Lista: {
            RackId: rackId,
            Descripcion: descripcion,
            Ubicacion: ubicacion
        }
    };
    DesplegarCamposaAlmacenRack(objetoParaDesplegar);


});

$("#tRacks tbody").on("click", ".btn-Rackeliminar", function () {
    var $row = $(this).closest("tr");
    var rackId = $row.find("td:nth-child(1) input").val();
    var almacenId = $("#txtRegistroidAlmacen").val();
    if (rackId != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar el Rack " + $("#txtRegistroUbicacionAlmacenRack").val() + " del Almacen " + $("#txtRegistroUbicacionAlmacen").val() + " ?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/Almacen/EliminarRack', { almacenId: almacenId, rackId: rackId }, false, function (data) {
                if (data.resultado) {
                    CargarUltimoRack($("#txtRegistroidAlmacen").val())
                    setTimeout(function () {
                        swal("Rack Eliminado", "la Rack del Almacen se elimino correctamente", "success")
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




cargartabla()

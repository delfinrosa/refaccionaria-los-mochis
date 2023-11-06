/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />
/// <reference path="../general/jqueryajax.js" />

var rol = getCookie("tipo");

var tabladata;
var filaSeleccionada;
var tablaInicializada = false;

var TextNombre = true;
var TextDescripcion = true;

var UltimoID;

var pagina = 0;
var paginaTabla=0;


var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');
//MOSTRAR DATOS EN LA TABLA
tabladata = $("#tabla").DataTable({
    responsive: true,
    ordering: false,
    ajax: {
        url: '/Mantenedor/ListarMarca',
        type: "GET",
        dataType: "json"
    },
    columns: [
        { data: "Descripcion" },
        {
            data: "Activo", render: function (valor) {
                if (valor == "A") {
                    return '<h5 class="m-auto"><span class="badge rounded-pill bg-success">Activo</span></h5>'
                } else if (valor == "O") {
                    return '<h5 class="m-auto"><span class="badge rounded-pill bg-secondary ">Oculto</span></h5>'
                } else {
                    return '<h5 class="m-auto"><span class="badge rounded-pill bg-danger">Desacticvado</span></h5>';
                }
            }
        },
        {

            "defaultContent": '<button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button>' +
                '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button>',
            "orderable": false,
            "searchable": false,
            "width": "90px"
        }
    ],
    "language": {
        url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
    }
});


/////////
//METODOS
/////////

//BOTON GUARDAR 
function Guardar() {
    var textActivo;

    if ($('#cboRegistroActivo').val() == "A") {
        textActivo = "A";
    } else if ($('#cboRegistroActivo').val() == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }

    var Marca = {
        IdMarca: $('#txtRegistroid').val(),
        Descripcion: $('#txtRegistroNombre').val(),
        Activo: textActivo
    }

    JQueryAjax_Normal('/Marca/GuardarMarca', { objeto: Marca }, true, function (data) {
        //Marca guardar
        if (Marca.IdMarca == 0) {
            if (data.resultado != 0) {
                UltimoID = data.resultado;
                $('#txtRegistroid').val(data.resultado);
                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")
                limpiar();
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
        }
        //Marca editar
        else {
            if (data.resultado) {
                UltimoID = Marca.IdMarca;
                $('#mensajeError').hide();
                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
                limpiar();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }

        }
    }, function () { })
}

//Nuevo Editar
function abrirModal2(json) {
    if (json != null) {
        $('#txtRegistroid').val(json.Lista.IdMarca);
        $('#txtRegistroNombre').val(json.Lista.Descripcion);
        if (json.Lista.Activo == "A") {
            $('#cboRegistroActivo').val("A");
        } else if (json.Lista.Activo == "O") {
            $('#cboRegistroActivo').val("O");
        } else {
            $('#cboRegistroActivo').val("D");
        }
    }
}

function FiltroPorMarca2(texto) {
    JQueryAjax_Normal('/Marca/BusquedaFiltroMarca', { nombre: texto }, true, function (data) {
        if (data.Descripcion != "") {
            $('#txtRegistroNombre').val(data.Descripcion);
            $('#txtRegistrodescripcion').val(data.Deslc);
            $('#txtRegistroid').val(data.IdMarca);
            /*ACTIVO USUARIO*/
            if (data.Activo == "A") {
                $('#cboRegistroActivo').val("A");
            } else if (data.Activo == "O") {
                $('#cboRegistroActivo').val("O");
            } else {
                $('#cboRegistroActivo').val("D");
            }
            $('#opcionesLista').hide();
            $('#opcionesListaAbajo').hide();

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
    $('#txtRegistroid').val("");
    $('#txtRegistroNombre').val("");
    $('#cboRegistroActivo').val("A");
}

function VerificacionLleno() {
    if ($('#txtRegistroNombre').val().trim() == "") {
        swal("Campo Vacio", " el campo de Nombre se encuentra vacio ", "error");
        return false
    } else if ($('#cboRegistroActivo').val().trim() == "") {
        swal("Campo Vacio", " el campo de Activo se encuentra vacio ", "error");
        return false
    }
    return true

}

///////////////
///Carga el ultimo
///////////////

function CargarUltimo() {
    JQueryAjax_Normal('/Marca/UltimoRegistroMarca', {}, true, function (data) {
        UltimoID = data.Lista.IdMarca
        abrirModal2(data);
    }, function () { });
}
////////////////
///COUNT AUTO COMPLETADO
///////////////

function desplegarPaginacion(searchTerm, pagina) {
    JQueryAjax_Normal('/Marca/PaginacionPRUEBA', { nombre: searchTerm, pagina: pagina }, true, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    input.val(item);
                    FiltroPorMarca2(listItem.text());
                });
                opcionesLista.append(listItem);
            });
            opcionesLista.show();
            $('#opcionesListaAbajo').show();
        }
    }, function () { });
}


function obtenerDivision(texto) {
    var division = 0;
    JQueryAjax_Normal('/Marca/COUNT_PruebasAutoCompletadoMarca', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / 5);
    }, function () { });
    return division;
}


function cargarOpcionesLista() {
    $('#opcionesListaAbajo').empty();
    var texto = $('#txtBusquedaNuevo').val().toLowerCase()
    var division = obtenerDivision(texto);
    var listItem = $("<div>");
    listItem.append($('<span id="numPaginadoBuscador">').text(pagina + 1 + ' de ' + division));


    var opcionesLista = $('<div class="d-flex align-items-center">' +
        '<button id="btn-paginado-anterior" class="btn btn-sm btn-primary mx-1 justify-content-between"><i class="fas fa-angle-left"></i></button>' +
        '<button id="btn-paginado-primero" class="btn btn-sm btn-primary mx-1 justify-content-between"><i class="fas fa-angle-double-left"></i></button>' +
        listItem.html() +
        '<button id="btn-paginado-ultimo" class="btn btn-sm btn-primary mx-1 justify-content-end"><i class="fas fa-angle-double-right"></i></button>' +
        '<button id="btn-paginado-siguiente" class="btn btn-sm btn-primary mx-1 justify-content-end"><i class="fas fa-angle-right"></i></button><hr /></div>');

    $('#opcionesListaAbajo').append(opcionesLista);




    // Asignar el evento click al botón "Anterior"
    $('#btn-paginado-anterior').click(function () {
        if (pagina != 0) {
            pagina--;
            var searchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
            desplegarPaginacion(searchTerm, pagina);
            $('#numPaginadoBuscador').text(pagina + 1 + ' de ' + division);

        }
    });

    // Asignar el evento click al botón "Primero"
    $('#btn-paginado-primero').click(function () {
        pagina = 0;
        var searchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
        desplegarPaginacion(searchTerm, pagina);
        $('#numPaginadoBuscador').text(pagina + 1 + ' de ' + division);

    });

    // Asignar el evento click al botón "Último"
    $('#btn-paginado-ultimo').click(function () {
        pagina = division - 1
        var searchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
        desplegarPaginacion(searchTerm, pagina);
        $('#numPaginadoBuscador').text(pagina + 1 + ' de ' + division);


    });

    // Asignar el evento click al botón "Siguiente"
    $('#btn-paginado-siguiente').click(function () {
        if (pagina != division - 1) {

            pagina++;
            var searchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
            desplegarPaginacion(searchTerm, pagina);
            $('#numPaginadoBuscador').text(pagina + 1 + ' de ' + division);



        }
    });

}

//////////////
//Paginado Tabla
//////////////

function CountPaginadoTabla() {
    var resultado = 0;
    JQueryAjax_Normal('/Marca/COUNT_TablaMarca', {}, false, function (data) {
        resultado = data.registros;
    }, function () { });
    return resultado;
}



function cargarOpcionesTabla() {
    $('#paginadoTabla').empty();
    var division = Math.ceil(CountPaginadoTabla() / 10);
    var listItem = $("<div>");

    listItem.append($('<span id="numPaginadoTabla">').text(paginaTabla + 1 + ' de ' + division));


    var opcionesLista = $('<div class="d-flex align-items-center">' +
        '<button id="btn-paginado-anterior-tabla" class="btn btn-sm btn-primary mx-1 justify-content-between"><i class="fas fa-angle-left"></i></button>' +
        '<button id="btn-paginado-primero-tabla" class="btn btn-sm btn-primary mx-1 justify-content-between"><i class="fas fa-angle-double-left"></i></button>' +
        listItem.html() +
        '<button id="btn-paginado-ultimo-tabla" class="btn btn-sm btn-primary mx-1 justify-content-end"><i class="fas fa-angle-double-right"></i></button>' +
        '<button id="btn-paginado-siguiente-tabla" class="btn btn-sm btn-primary mx-1 justify-content-end"><i class="fas fa-angle-right"></i></button><hr /></div>');

    $('#paginadoTabla').append(opcionesLista);




    // Asignar el evento click al botón "Anterior"
    $('#btn-paginado-anterior-tabla').click(function () {
        if (paginaTabla != 0) {
            paginaTabla--;
            datosTablaPaginado();
            $('#numPaginadoTabla').text(paginaTabla + 1 + ' de ' + division);

        }
    });

    // Asignar el evento click al botón "Primero"
    $('#btn-paginado-primero-tabla').click(function () {
        paginaTabla = 0;
        datosTablaPaginado();
        $('#numPaginadoTabla').text(paginaTabla + 1 + ' de ' + division);

    });

    // Asignar el evento click al botón "Último"
    $('#btn-paginado-ultimo-tabla').click(function () {
        paginaTabla = division - 1
        datosTablaPaginado();
        $('#numPaginadoTabla').text(paginaTabla + 1 + ' de ' + division);


    });

    // Asignar el evento click al botón "Siguiente"
    $('#btn-paginado-siguiente-tabla').click(function () {
        if (paginaTabla != division - 1) {

            paginaTabla++;
            datosTablaPaginado();
            $('#numPaginadoTabla').text(paginaTabla + 1 + ' de ' + division);



        }
    });

}

//////////
//BOTONES
//////////

//Barra de heramientas 

$("#btn-Nuevo").click(function () {
    limpiar();
    TextNombre = false;
    TextDescripcion = false;
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    swal("Crea un Nuevo Registro", "Introduce los datos del registro llena todo los campos ")
});

$("#btn-Cancelar").click(function () {
    CargarUltimo();
});

$("#btn-Guardar").click(function () {
    if (VerificacionLleno()) {
        $('#btn-Eliminar').prop('disabled', false);
        Guardar();

    }
});

$("#btn-Eliminar").click(function () {

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
            JQueryAjax_Normal('/Marca/EliminarMarca', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    datosTablaPaginado();
                    CargarUltimo();
                } else {
                    swal("No se pudo eliminar", data.mensaje, "error");
                }
            }, function () { });
        });
    }
});

// Focus al input Buscar (Icono lupa)
$('#botonBuscar').click(function () {
    $('#txtBusquedaNuevo').focus();
});

/////
//TABLA
/////
$("#btn-Actualizar").click(function () {
    datosTablaPaginado();
});

//BOTON EDITAR
$("#tabla tbody").on("click", '.btn-editar', function () {
    // Obtener datos del registro de la tabla
    var data = tabladata.row($(this).closest("tr")).data();

    JQueryAjax_Normal('/Marca/ListarPorIdMarcas', { Id: data.IdMarca }, false,
        function (data) {
            abrirModal2(data);
        }, function () { });

    TextNombre = true;
    TextDescripcion = true;

});



//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var Marcaseleccionado = $(this).closest("tr");
    var data = tabladata.row(Marcaseleccionado).data();
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Marca?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Marca/EliminarMarca', { id: data.IdMarca }, true, function (data) {

                if (data.resultado) {
                    datosTablaPaginado();
                    CargarUltimo();
                } else {
                    swal("No se pudo eliminar", data.mensaje, "error");
                }
            }, function () { });

        });

})

////////////
//INPUT
////////////

$("#txtRegistroNombre").on('input', function () {
    TextNombre = true;
    if (TextNombre ) {
        $('#btn-Guardar').prop('disabled', false);

    }
});




$('#txtBusquedaNuevo').on('input', function () {
    var searchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
    pagina = 0;
    // Realizar el autocompletado y manejar los resultados
    desplegarPaginacion(searchTerm, pagina);
    cargarOpcionesLista();


});

// Ocultar opcionesLista cuando se hace clic fuera de ella
$(document).on('click', function (event) {

    if (!$(event.target).closest('#buscador').length) {
        $('#opcionesListaAbajo').hide();
        opcionesLista.hide();

    }
});

///////////
//AL CARGAR
///////////
CargarUltimo();
cargarOpcionesTabla()
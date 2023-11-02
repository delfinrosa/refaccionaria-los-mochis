/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />

var rol = getCookie("tipo");

var tabladata;
var filaSeleccionada;
var tablaInicializada = false;

var TextNombre = true;
var TextDescripcion = true;

var UltimoID;

var pagina = 0;



//Filtro por ID
function FiltrarPorID(id, callback) {
    var Linea;
    jQuery.ajax({
        url: '/Mantenedor/ListarPorIdLineas',
        type: "POST",
        data: JSON.stringify({ Id: id }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            Linea = data;
            console.log(Linea);
            // Llamar a la callback con los datos
            callback(null, Linea);
        },
        error: function (error) {
            console.log(error);
            // Manejar el error de la llamada AJAX
        }
    });
}




//BOTON EDITAR

$("#tabla tbody").on("click", '.btn-editar', function () {
    // Obtener datos del registro de la tabla
    var data = tabladata.row($(this).closest("tr")).data();

    // Llamar a la función para obtener datos adicionales del servidor
    FiltrarPorID(data.IdLinea, function (error, datosEditar) {
        if (error) {
            console.error("Error al obtener datos:", error);
            // Manejar errores si es necesario
        } else {

            // Llamar a la función para abrir el modal y pasar los datos
            abrirModal2(datosEditar);
        }
    });
    TextNombre = true;
    TextDescripcion = true;

});



//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var Lineaseleccionado = $(this).closest("tr");
    var data = tabladata.row(Lineaseleccionado).data();
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Linea?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {

            jQuery.ajax({
                url: '/Mantenedor/EliminarLinea',
                type: "POST",
                data: JSON.stringify({ id: data.IdLinea }),
                dataType: "json",
                contentType: "application/json; chartset=utf-8",
                success: function (data) {
                    if (data.resultado) {
                        datosTablaPaginado();
                        CargarUltimo();
                    } else {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }
                },
                error: function (error) {
                    console.log(error)
                }
            });


        });

})

//BOTON GUARDAR (MODAL)
function Guardar() {
    var textActivo;
    var comp = $('#cboRegistroActivo').val();

    if (comp == "A") {
        textActivo = "A";
    } else if (comp == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }

    var Linea = {
        IdLinea: $('#txtRegistroid').val(),
        Descripcion: $('#txtRegistroNombre').val(),
        Activo: textActivo,
        Deslc: $('#txtRegistrodescripcion').val()

    }

    jQuery.ajax({
        url: '/Mantenedor/GuardarLinea',
        type: "POST",
        data: JSON.stringify({ objeto: Linea }),
        dataType: "json",
        contentType: "application/json; chartset=utf-8",
        success: function (data) {
            $('.modal-body').LoadingOverlay("hide");
            //Linea nuevo
            if (Linea.IdLinea == 0) {
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
            //Linea editar
            else {
                if (data.resultado) {
                    //$("#FormModal").modal("hide");
                    //tabladata.row(filaSeleccionada).data(data.objDevolucion).draw(false);
                    //filaSeleccionada = null;
                    UltimoID = Linea.IdLinea;

                    $('#mensajeError').hide();
                    swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
                    limpiar();
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();

                }

            }

        },
        error: function (data) {
            $('.modal-body').LoadingOverlay("hide");
            $('#mensajeError').text("Error AJAX");
            $('#mensajeError').show();
        },
        beforeSend: function (data) {
            $('.modal-body').LoadingOverlay("show", {
                imageResizeFactor: 2,
                text: "Cargando.....",
                size: 14
            });
        }
    });
    datosTablaPaginado();


}
//Nuevo Editar
function abrirModal2(json) {
    if (json != null) {
        $('#txtRegistroid').val(json.Lista.IdLinea);
        $('#txtRegistroNombre').val(json.Lista.Descripcion);
        if (json.Lista.Activo == "A") {
            $('#cboRegistroActivo').val("A");
        } else if (json.Lista.Activo == "O") {
            $('#cboRegistroActivo').val("O");
        } else {
            $('#cboRegistroActivo').val("D");
        }
        $('#txtRegistrodescripcion').val(json.Lista.Deslc);

    }
}
///////////////////////////////////
//pruebas de lo nuevo
/////////////////////////////////
// Método para realizar la búsqueda de líneas
function ListarNombreDeLineas(texto, callback) {
    jQuery.ajax({
        url: '/Mantenedor/ListarNombreDeLineas',
        type: "POST",
        data: JSON.stringify({ nombre: texto }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            callback(data);
        },
        error: function (error) {
            console.log(error);
            // Manejar el error de la llamada AJAX
        }
    });
}

// Método para realizar el autocompletado de líneas
function FiltroPorLinea() {
    var input = $('#txtBusqueda');

    var texto = input.val().toLowerCase();
    if (texto != "") {

        jQuery.ajax({
            url: '/Mantenedor/BusquedaFiltroLinea',
            type: "POST",
            data: JSON.stringify({ nombre: texto }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.Descripcion != null) {

                    $('#txtRegistroNombre').val(data.Descripcion);
                    $('#txtRegistrodescripcion').val(data.Deslc);
                    $('#txtRegistroid').val(data.IdLinea);
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
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}
function FiltroPorLinea2(texto) {
    jQuery.ajax({
        url: '/Mantenedor/BusquedaFiltroLinea',
        type: "POST",
        data: JSON.stringify({ nombre: texto }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.Descripcion != "") {
                $('#txtRegistroNombre').val(data.Descripcion);
                $('#txtRegistrodescripcion').val(data.Deslc);
                $('#txtRegistroid').val(data.IdLinea);
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
        },
        error: function (error) {
            console.log(error);
        }
    });
}
var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');

input.on('input', function () {
    var searchTerm = input.val().toLowerCase();

    // Realizar el autocompletado y manejar los resultados
    ListarNombreDeLineas(searchTerm, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    input.val(item);
                    FiltroPorLinea2(listItem.text());
                });
                opcionesLista.append(listItem);
            });
            opcionesLista.show();


        }






    });
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
            jQuery.ajax({
                url: '/Mantenedor/EliminarLinea',
                type: "POST",
                data: JSON.stringify({ id: idBorrar }),
                dataType: "json",
                contentType: "application/json; chartset=utf-8",
                success: function (data) {
                    if (data.resultado) {
                        datosTablaPaginado();
                        CargarUltimo();
                    } else {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        });
    }
});



$("#txtBusqueda").on('input', FiltroPorLinea);

// Ocultar opcionesLista cuando se hace clic fuera de ella
$(document).on('click', function (event) {

    if (!$(event.target).closest('#buscador').length) {
        $('#opcionesListaAbajo').hide();
        opcionesLista.hide();

    }
});

//Evento de el boton 'Nuevo' 
//Limpia los campos
$("#btn-Nuevo").click(function () {
    limpiar();
    TextNombre = false;
    TextDescripcion = false;
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    swal("Crea un Nuevo Registro", "Introduce los datos del registro llena todo los campos ")
});


$("#btn-Actualizar").click(function () {
    datosTablaPaginado();
});


function limpiar() {
    $('#txtRegistroid').val("");
    $('#txtRegistroNombre').val("");
    $('#cboRegistroActivo').val("A");
    $('#txtRegistrodescripcion').val("");
}

function VerificacionLleno() {
    if ($('#txtRegistroNombre').val().trim() == "") {
        swal("Campo Vacio", " el campo de Nombre se encuentra vacio ", "error");
        return false
    } else if ($('#cboRegistroActivo').val().trim() == "") {
        swal("Campo Vacio", " el campo de Activo se encuentra vacio ", "error");
        return false
    } else if ($('#txtRegistrodescripcion').val().trim() == "") {
        swal("Campo Vacio", " el campo de Descripcion se encuentra vacio ", "error");
        return false
    }
    return true

}

$("#txtRegistroNombre").on('input', function () {
    TextNombre = true;
    if (TextNombre && TextDescripcion) {
        $('#btn-Guardar').prop('disabled', false);

    }
});
$("#txtRegistrodescripcion").on('input', function () {
    TextDescripcion = true;
    if (TextNombre && TextDescripcion) {
        $('#btn-Guardar').prop('disabled', false);

    }
});

///////////////
///Carga el ultimo
///////////////

function CargarUltimo() {
    jQuery.ajax({
        url: '/Mantenedor/UltimoRegistro',
        type: "POST",
        data: JSON.stringify({}),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            UltimoID = data.Lista.IdLinea
            abrirModal2(data);
        },
        error: function (error) {
            console.log(error);
            // Manejar el error de la llamada AJAX
        }
    });

}
CargarUltimo();

$("#btn-Cancelar").click(function () {

    FiltrarPorID(UltimoID, function (error, datosEditar) {
        if (error) {
            console.error("Error al obtener datos:", error);
            // Manejar errores si es necesario
        } else {

            // Llamar a la función para abrir el modal y pasar los datos
            abrirModal2(datosEditar);
        }
    });
});

////////////////
///AutoCompletad
///////////////


////////////////
///COUNT AUTO COMPLETADO
///////////////

function desplegarPaginacion(searchTerm, pagina) {
    jQuery.ajax({
        url: '/Mantenedor/PaginacionPRUEBA',
        type: "POST",
        data: JSON.stringify({ nombre: searchTerm, pagina: pagina }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var lista = data.Lista;
            opcionesLista.empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item);
                    listItem.on('click', function () {
                        input.val(item);
                        FiltroPorLinea2(listItem.text());
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#opcionesListaAbajo').show();
            }
        },
        error: function (error) {
            console.log(error);
            // Manejar el error de la llamada AJAX
        }
    });
}



$('#txtBusquedaNuevo').on('input', function () {
    var searchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
    pagina = 0;
    // Realizar el autocompletado y manejar los resultados
    desplegarPaginacion(searchTerm, pagina);
    cargarOpcionesLista();


});


function obtenerDivision(texto) {
    var division = 0;
    jQuery.ajax({
        url: '/Mantenedor/COUNT_PruebasAutoCompletado',
        type: "POST",
        data: JSON.stringify({ nombre: texto }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,  // Espera la respuesta del AJAX antes de continuar
        success: function (data) {
            division = Math.ceil(data.registros / 5);
        },
        error: function (error) {
            console.log(error);
            // Manejar el error de la llamada AJAX
        }
    });
    return division;
}
// Definir la función handleButtonClick
function handleButtonClick() {
    // Obtener el valor del botón clickeado
    var opagina = $(this).val();
    var osearchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
    console.log(opagina);
    console.log(osearchTerm);
    // Llamar a la función desplegarPaginacion con searchTerm y pagina como parámetros
    desplegarPaginacion(osearchTerm, opagina);
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




// Focus al input Buscar
$('#botonBuscar').click(function () {
    $('#txtBusquedaNuevo').focus();
});


var paginaTabla;

function realizarLlamadaAjax() {
    var result; // Variable para almacenar los datos devueltos por la llamada AJAX

    jQuery.ajax({
        url: '/Mantenedor/ListarPrueba',
        type: "POST",
        data: JSON.stringify({ strpagina: paginaTabla }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            result = data; // Almacena los datos devueltos en la variable result
        },
        error: function (error) {
            console.log(error);
        },
        async: false
    });

    return result; // Devuelve los datos almacenados en la variable result
}

//MOSTRAR DATOS EN LA TABLA
function datosTablaPaginado() {
    $('#tabla').DataTable().clear().destroy();
    // Limpiar el encabezado
    var data = realizarLlamadaAjax();

    var headers = '';
    if (!tablaInicializada) {
        paginaTabla = 0

        // Marcar la tabla como inicializada
        tablaInicializada = true;

        var headers = '';

        if (rol == "A") {
            headers = '<th>Persona que modifico</th>' +
                '<th>Fecha de Registro</th>' +
                '<th>Fecha de Actualizacion</th>' +
                '<th></th>';
        } else {
            headers = '<th></th>';
        }
        $("#trowTabla").append(headers);
    }
    $('#tabla').DataTable().clear().destroy();

    // Limpiar el encabezado
    console.log("Carga Tabla")
    var headers = '';
    if (!tablaInicializada) {

        // Marcar la tabla como inicializada
        tablaInicializada = true;

        var headers = '';

        if (rol == "A") {
            headers = '<th>Persona que modifico</th>' +
                '<th>Fecha de Registro</th>' +
                '<th>Fecha de Actualizacion</th>' +
                '<th></th>';
        } else {
            headers = '<th></th>';
        }
        $("#trowTabla").append(headers);
    }

    //MOSTRAR DATOS EN LA TABLA
    if (rol == "A") {

        console.log(data.data)
        tabladata = $("#tabla").DataTable({
            responsive: true,
            ordering: false,
            data: data.data,
            columns: [
                { data: "IdLinea" },
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
                { data: "Deslc" },
                { data: "IdUsuario" },
                { data: "fechaCreaccion" },
                { data: "fechaActualizacion" },
                {

                    "defaultContent": '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
                        '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button>',
                    "orderable": false,
                    "searchable": false,
                    "width": "90px"
                }
            ],
            "language": {
                url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
            }

            // Verificar el rol y mostrar las columnas adicionales si es "A" (administrador)


        });
    } else {

        tabladata = $("#tabla").dataTable({
            responsive: true,
            ordering: false,
            bServerSide: true,
            data: data,
            columns: [
                { data: "IdLinea" },
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
                { data: "Deslc" },
                {

                    "defaultContent": '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
                        '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button>',
                    "orderable": false,
                    "searchable": false,
                    "width": "90px"
                }
            ],
            "language": {
                url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
            }
            // Verificar el rol y mostrar las columnas adicionales si es "A" (administrador)


        });
    }

}
datosTablaPaginado();

//////////////
//Paginado Tabla
//////////////

function CountPaginadoTabla() {
    var resultado = 0;
    jQuery.ajax({
        url: '/Mantenedor/COUNT_Tabla',
        type: "POST",
        data: JSON.stringify({}),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,  // Espera la respuesta del AJAX antes de continuar
        success: function (data) {
            resultado = data.registros;
        },
        error: function (error) {
            console.log(error);
            // Manejar el error de la llamada AJAX
        }
    });
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
cargarOpcionesTabla()


/// <reference path="../jquery-3.4.1.js" />


var tabladata;
var filaSeleccionada;
//MOSTRAR DATOS EN LA TABLA
tabladata = $("#tabla").DataTable({
    responsive: true,
    ordering: false,
    ajax: {
        url: '/Mantenedor/ListarLinea',
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
});
//ABRIR MODAL
function abrirModal(json) {
    $('#txtid').val(0);
    $('#txtdescripcion').val("");
    $('#cboactivo').val("A");
    $('#textareadescripcion').val("");
    $('#mensajeError').hide();

    if (json != null) {
        $('#txtid').val(json.IdLinea);
        $('#txtdescripcion').val(json.Descripcion);
        if (json.Activo == "A") {
            $('#cboactivo').val("A");
        } else if (json.Activo == "O") {
            $('#cboactivo').val("O");
        } else {
            $('#cboactivo').val("D");
        }
        $('#textareadescripcion').val(json.Deslc);

    }
    $("#FormModal").modal("show");
}
//BOTON EDITAR

$("#tabla tbody").on("click", '.btn-editar', function () {
    filaSeleccionada = $(this).closest("tr");
    var data = tabladata.row(filaSeleccionada).data();
    abrirModal2(data);
})
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
                        tabladata.row(Lineaseleccionado).remove().draw()
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
                    Linea.IdLinea = data.resultado;
                    tabladata.row.add(Linea).draw(false);
                    $("#FormModal").modal("hide");
                    $('#txtRegistroid').val(data.resultado);

                    limpiar();
                    $('#mensajeError').hide();
                    swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();

                }

            }
            //Linea editar
            else {
                if (data.resultado) {
                    $("#FormModal").modal("hide");
                    tabladata.row(filaSeleccionada).data(Linea).draw(false);
                    filaSeleccionada = null;
                    $('#mensajeError').hide();
                    limpiar();
                    swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
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

}
//Nuevo Editar
function abrirModal2(json) {
    if (json != null) {
        $('#txtRegistroid').val(json.IdLinea);
        $('#txtRegistroNombre').val(json.Descripcion);
        if (json.Activo == "A") {
            $('#cboRegistroActivo').val("A");
        } else if (json.Activo == "O") {
            $('#cboRegistroActivo').val("O");
        } else {
            $('#cboRegistroActivo').val("D");
        }
        $('#txtRegistrodescripcion').val(json.Deslc);

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





//METODO DE GUARDAR
//function Guardar() {
//    var textActivo;
//    var comp = $('#cboRegistroActivo').val();

//    if (comp == "A") {
//        textActivo = "A";
//    } else if (comp == "O") {
//        textActivo = "O";
//    } else {
//        textActivo = "D";
//    }

//    var Linea = {
//        IdLinea: $('#txtRegistroid').val(),
//        Descripcion: $('#txtRegistroNombre').val(),
//        Activo: textActivo,
//        Deslc: $('#txtRegistrodescripcion').val()

//    }

//    jQuery.ajax({
//        url: '/Mantenedor/GuardarLinea',
//        type: "POST",
//        data: JSON.stringify({ objeto: Linea }),
//        dataType: "json",
//        contentType: "application/json; chartset=utf-8",
//        success: function (data) {

//            //Linea nuevo
//            if (Linea.IdLinea == 0) {
//                if (data.resultado != 0) {
//                    Linea.IdLinea = data.resultado;
//                    tabladata.row.add(Linea).draw(false);
//                    $('#txtRegistroid').val(data.resultado);

//                    limpiar();
//                    $('#mensajeError').hide();
//                    swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")


//                }
//                else {
//                    swal("Error en el Registro", data.mensaje, "warning")

//                    $('#mensajeError').text(data.mensaje);
//                    $('#mensajeError').show();

//                }

//            }
//            //Linea editar
//            else {
//                if (data.resultado) {
//                    $("#FormModal").modal("hide");
//                    tabladata.row(data.resultado).data(Linea).draw(false);
//                    filaSeleccionada = null;
//                    $('#mensajeError').hide();
//                    limpiar();
//                    swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")

//                }
//                else {
//                    swal("Error en el Registro", data.mensaje, "warning")


//                    $('#mensajeError').text(data.mensaje);
//                    $('#mensajeError').show();

//                }

//            }



//        },
//        error: function (data) {
//            $('.modal-body').LoadingOverlay("hide");
//            $('#mensajeError').text("Error AJAX");
//            $('#mensajeError').show();
//        }
//    });
//}
$("#btn-Guardar").click(Guardar);

$("#btn-Eliminar").click(function () {
    var idBorrar = $("#txtRegistroid").val();
    console.log(idBorrar)
    if (idBorrar != 0) {

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
            function() {

                jQuery.ajax({
                    url: '/Mantenedor/EliminarLinea',
                    type: "POST",
                    data: JSON.stringify({ id: idBorrar }),
                    dataType: "json",
                    contentType: "application/json; chartset=utf-8",
                    success: function (data) {

                        if (data.resultado) {
                            tabladata.row(idBorrar).remove().draw()
                        } else {
                            swal("No se pudo eliminar", data.mensaje, "error");

                        }
                    },
                    error: function (error) {
                        console.log(error)
                    }
                });



            

        })
    }
})

    $("#txtBusqueda").on('input', FiltroPorLinea);

    // Ocultar opcionesLista cuando se hace clic fuera de ella
    $(document).on('click', function (event) {
        if (!$(event.target).closest('#opcionesLista').length && !$(event.target).is('#txtBusqueda')) {
            opcionesLista.hide();

        }
    });

    //Evento de el boton 'Nuevo' 
    //Limpia los campos
    $("#btn-Nuevo").click(limpiar);
    $("#btn-Nuevo").click(function () {
        swal("Crea un Nuevo Registro", "Introduce los datos del registro llena todo los campos ")
    });


    function limpiar() {
        $('#txtRegistroid').val("");
        $('#txtRegistroNombre').val("");
        $('#cboRegistroActivo').val("A");
        $('#txtRegistrodescripcion').val("");
    }
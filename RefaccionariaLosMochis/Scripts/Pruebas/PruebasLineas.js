//// Método para realizar la búsqueda de líneas
//function ListarNombreDeLineas(texto, callback) {
//    jQuery.ajax({
//        url: '/Mantenedor/ListarNombreDeLineas',
//        type: "POST",
//        data: JSON.stringify({ nombre: texto }),
//        dataType: "json",
//        contentType: "application/json; charset=utf-8",
//        success: function (data) {
//            callback(data);
//        },
//        error: function (error) {
//            console.log(error);
//            // Manejar el error de la llamada AJAX
//        }
//    });
//}

//// Método para realizar el autocompletado de líneas
//function FiltroPorLinea() {
//    var input = $('#txtBusqueda');

//    var texto = input.val().toLowerCase();
//    if (texto != "") {

//        jQuery.ajax({
//            url: '/Mantenedor/BusquedaFiltroLinea',
//            type: "POST",
//            data: JSON.stringify({ nombre: texto }),
//            dataType: "json",
//            contentType: "application/json; charset=utf-8",
//            success: function (data) {
//                if (data.Descripcion != null) {

//                    $('#txtRegistroNombre').val(data.Descripcion);
//                    $('#txtRegistrodescripcion').val(data.Deslc);
//                    $('#txtRegistroid').val(data.IdLinea);
//                    /*ACTIVO USUARIO*/
//                    if (data.Activo == "A") {
//                        $('#cboRegistroActivo').val("A");
//                    } else if (data.Activo == "O") {
//                        $('#cboRegistroActivo').val("O");
//                    } else {
//                        $('#cboRegistroActivo').val("D");
//                    }
//                    $('#opcionesLista').hide();


//                } else {
//                    // Si no se encontraron resultados, puedes limpiar los campos o mostrar un mensaje de error
//                    $('#txtRegistroNombre').val('');
//                    $('#txtRegistrodescripcion').val('');
//                    $('#cboRegistroActivo').val('A');
//                }
//            },
//            error: function (error) {
//                console.log(error);
//            }
//        });
//    } 
//}
//function FiltroPorLinea2(texto) {
//    jQuery.ajax({
//        url: '/Mantenedor/BusquedaFiltroLinea',
//        type: "POST",
//        data: JSON.stringify({ nombre: texto }),
//        dataType: "json",
//        contentType: "application/json; charset=utf-8",
//        success: function (data) {
//            if (data.Descripcion != "") {
//                $('#txtRegistroNombre').val(data.Descripcion);
//                $('#txtRegistrodescripcion').val(data.Deslc);
//                $('#txtRegistroid').val(data.IdLinea);
//                /*ACTIVO USUARIO*/
//                if (data.Activo == "A") {
//                    $('#cboRegistroActivo').val("A");
//                } else if (data.Activo == "O") {
//                    $('#cboRegistroActivo').val("O");
//                } else {
//                    $('#cboRegistroActivo').val("D");
//                }

//                $('#opcionesLista').hide();

//            } else {
//                // Si no se encontraron resultados, puedes limpiar los campos o mostrar un mensaje de error
//                $('#txtRegistroNombre').val('');
//                $('#txtRegistrodescripcion').val('');
//                $('#cboRegistroActivo').val('A');
//            }
//        },
//        error: function (error) {
//            console.log(error);
//        }
//    });
//}
//var opcionesLista = $('#opcionesLista');
//var input = $('#txtBusqueda');

//input.on('input', function () {
//    var searchTerm = input.val().toLowerCase();

//    // Realizar el autocompletado y manejar los resultados
//    ListarNombreDeLineas(searchTerm, function (data) {
//        var lista = data.Lista;
//        opcionesLista.empty();
//        if (lista.length > 0) {
//            lista.forEach(function (item) {
//                var listItem = $('<div class="autocomplete-option"></div>').text(item);
//                listItem.on('click', function () {
//                    input.val(item);
//                    FiltroPorLinea2(listItem.text());
//                });
//                opcionesLista.append(listItem);
//            });
//            opcionesLista.show();


//        } 





         
//    });
//});





////METODO DE GUARDAR
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
//$("#btn-Guardar").click(Guardar);

//$("#txtBusqueda").on('input', FiltroPorLinea);

//// Ocultar opcionesLista cuando se hace clic fuera de ella
//$(document).on('click', function (event) {
//    if (!$(event.target).closest('#opcionesLista').length && !$(event.target).is('#txtBusqueda')) {
//        opcionesLista.hide();

//    }
//});

////Evento de el boton 'Nuevo' 
////Limpia los campos
//$("#btn-Nuevo").click(limpiar);

//function limpiar () {
//    $('#txtRegistroid').val("");
//    $('#txtRegistroNombre').val("");
//    $('#cboRegistroActivo').val("A");
//    $('#txtRegistrodescripcion').val("");
//}
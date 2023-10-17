(function () {
    var tTime = 100;  // tiempo de transición para transformación desde #register en ms
    var wTime = 200;  // tiempo de transición para el ancho desde #register en ms

    var inputContainers = document.querySelectorAll('.inputContainer');
    var inputFields = document.querySelectorAll('.inputField');
    var inputLabels = document.querySelectorAll('.inputLabel');

    inputFields.forEach(function (inputField, index) {
        showCurrent(index);  
        inputField.addEventListener('focus', function () {
            showCurrent(index);
        });
    });

    function showCurrent(index, callback) {
        inputContainers[index].style.opacity = 1;
        inputContainers[index].style.transition = 'opacity .3s ease-in-out';
        inputLabels[index].style.transition = '.2s ease-in-out';

        inputLabels[index].classList.add('active');
        inputFields[index].classList.add('active');

        if (callback) {
            setTimeout(callback, wTime);
        }
    }
})();
///BUSCADOR 
$('#txtBusqueda').on('input', function () {
    var searchTerm = $(this).val();
    jQuery.ajax({
        url: '/Mantenedor/BusquedaFiltroLinea',
        type: "POST",
        data: JSON.stringify({ nombre: searchTerm }),
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
});

//METODO DE GUARDAR
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
                    swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")

                }
                else {
                    swal("Error en el Registro", data.mensaje, "warning")

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
                    swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")

                }
                else {
                    swal("Error en el Registro", data.mensaje, "warning")


                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();

                }

            }



        },
        error: function (data) {
            $('.modal-body').LoadingOverlay("hide");
            $('#mensajeError').text("Error AJAX");
            $('#mensajeError').show();
        }
    });
}
$("#btn-Guardar").click(Guardar);
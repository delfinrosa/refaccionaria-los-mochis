/// <reference path="../jquery-3.4.1.js" />

var tabladata;
var filaSeleccionada;
//MOSTRAR DATOS EN LA TABLA
tabladata = $("#tabla").DataTable({
    responsive: true,
    ordering: false,
    ajax: {
        url: '/Mantenedor/ListarUsuario',
        type: "GET",
        dataType: "json"
    },
    columns: [
        { data: "Nombre" },
        { data: "Correo" },
        {
            data: "Tipo", render: function (valor) {
                if (valor == "A") {
                    return '<h5 class="m-auto"><span class="badge bg-success"><i class="fas fa-cog me-1"></i>Admintardor</span></h5>'
                } else if (valor == "I") {
                    return '<h5 class="m-auto"><span class="badge bg-primary "><i class="fas fa-table me-1"></i>Inventario</span></h5>'
                } else if (valor == "C") {
                    return '<h5 class="m-auto"><span class="badge bg-secondary "><i class="fas fa-money-bill me-1"></i>Cajero</span></h5>'
                } else {
                    return '<h5 class="m-auto"><span class="badge bg-danger"><i class="fas fa-tag me-1"></i>Vendedor</span></h5>';
                }
            }
        }, {
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
//ABRIR MODAL
function abrirModal(json) {
    $('#txtid').val(0);
    $('#txtnombre').val("");
    $('#txtcorreo').val("");
    $('#txtcontraseña').val("");
    $('#cboactivo').val("A");
    $('#mensajeError').hide();

    if (json != null) {
        $('#txtid').val(json.IdUsuario);
        $('#txtnombre').val(json.Nombre);
        $('#txtcontraseña').val(json.Contraseña);
        $('#txtcorreo').val(json.Correo);

        /*ACTIVO USUARIO*/
        if (json.Activo == "A") {
            $('#cboactivo').val("A");
        } else if (json.Activo == "O") {
            $('#cboactivo').val("O");
        } else {
            $('#cboactivo').val("D");
        }

        /*TIPO USUARIO*/
        if (json.Tipo == "A") {
            $('#cbotipo').val("A");
        } else if (json.Tipo == "C") {
            $('#cbotipo').val("C");
        } else if (json.Tipo == "I") {
            $('#cbotipo').val("I");
        } else {
            $('#cbotipo').val("V");
        }
    }
    $("#FormModal").modal("show");
}
//BOTON EDITAR
$("#tabla tbody").on("click", '.btn-editar', function () {
    filaSeleccionada = $(this).closest("tr");
    var data = tabladata.row(filaSeleccionada).data();
    abrirModal(data);
})
//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var Marcaseleccionado = $(this).closest("tr");
    var data = tabladata.row(Marcaseleccionado).data();
    console.log(data);
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Usuario?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {

            jQuery.ajax({
                url: '/Mantenedor/EliminarUsuario',
                type: "POST",
                data: JSON.stringify({ id: data.IdUsuario }),
                dataType: "json",
                contentType: "application/json; chartset=utf-8",
                success: function (data) {

                    if (data.resultado) {
                        tabladata.row(Marcaseleccionado).remove().draw()
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
    var ValActivo = $('#cboactivo').val();
    if (ValActivo == "A") {
        textActivo = "A";
    } else if (ValActivo == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }
    var textTipo;
    var ValTipo = $('#cbotipo').val();
    if (ValTipo == "A") {
        textTipo = "A";
    } else if (ValTipo == "C") {
        textTipo = "C";
    } else if (ValTipo == "V") {
        textTipo = "V";
    } else {
        textTipo = "I";
    }

    var Usuario = {
        IdUsuario: $('#txtid').val(),
        Nombre: $('#txtnombre').val(),
        Contraseña: $('#txtcontraseña').val(),
        Correo: $('#txtcorreo').val(),
        Tipo: textTipo,
        Activo: textActivo
    }

    jQuery.ajax({
        url: '/Mantenedor/GuardarUsuario',
        type: "POST",
        data: JSON.stringify({ objeto: Usuario }),
        dataType: "json",
        contentType: "application/json; chartset=utf-8",
        success: function (data) {
            $('.modal-body').LoadingOverlay("hide");

            //Usuario nuevo
            if (Usuario.IdUsuario == 0) {
                if (data.resultado != 0) {
                    Usuario.IdUsuario = data.resultado;
                    tabladata.row.add(Usuario).draw(false);
                    $("#FormModal").modal("hide");
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();

                }

            }
            //Usuario editar
            else {
                if (data.resultado) {
                    $("#FormModal").modal("hide");
                    tabladata.row(filaSeleccionada).data(Usuario).draw(false);
                    filaSeleccionada = null;
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
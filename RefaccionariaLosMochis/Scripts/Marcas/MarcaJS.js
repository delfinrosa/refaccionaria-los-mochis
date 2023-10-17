/// <reference path="../jquery-3.4.1.js" />

var tabladata;
var filaSeleccionada;
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
//ABRIR MODAL
function abrirModal(json) {
    $('#txtid').val(0);
    $('#txtdescripcion').val("");
    $('#cboactivo').val("A");
    $('#mensajeError').hide();

    if (json != null) {
        $('#txtid').val(json.IdMarca);
        $('#txtdescripcion').val(json.Descripcion);
        if (json.Activo == "A") {
            $('#cboactivo').val("A");
        } else if (json.Activo == "O") {
            $('#cboactivo').val("O");
        } else {
            $('#cboactivo').val("D");
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
        text: "¿Desea eliminar la Marca?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {

            jQuery.ajax({
                url: '/Mantenedor/EliminarMarca',
                type: "POST",
                data: JSON.stringify({ id: data.IdMarca }),
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
    debugger;
    var textActivo;
    var comp = $('#cboactivo').val();
    if (comp == "A") {
        textActivo = "A";
    } else if (comp == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }
    var Marca = {
        IdMarca: $('#txtid').val(),
        Descripcion: $('#txtdescripcion').val(),
        Activo: textActivo
    }

    jQuery.ajax({
        url: '/Mantenedor/GuardarMarca',
        type: "POST",
        data: JSON.stringify({ objeto: Marca }),
        dataType: "json",
        contentType: "application/json; chartset=utf-8",
        success: function (data) {
            $('.modal-body').LoadingOverlay("hide");

            //Marca nuevo
            if (Marca.IdMarca == 0) {
                if (data.resultado != 0) {
                    Marca.IdMarca = data.resultado;
                    tabladata.row.add(Marca).draw(false);
                    $("#FormModal").modal("hide");
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();

                }

            }
            //Marca editar
            else {
                if (data.resultado) {
                    $("#FormModal").modal("hide");
                    tabladata.row(filaSeleccionada).data(Marca).draw(false);
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
var tabladata;
var filaSeleccionada;
/*CARGAR IMAGEN*/
function mostrarImagen(input) {
    if (input.files) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $("#img_producto").attr("src", e.target.result).width(200).height(197)

        };
        reader.readAsDataURL(input.files[0]);

    }
}
//MOSTRAR DATOS EN LA TABLA
tabladata = $("#tabla").DataTable({
    responsive: true,
    ordering: false,
    "ajax": {
        url: '/Mantenedor/ListarProducto',
        type: "GET",
        dataType: "json"
    },
    "columns": [
        { "data": "IdProducto" },
        { "data": "Descripcion" },
        { "data": "Precio" },
        { "data": "Minimo" },
        { "data": "Maximo" },
        {
            "data": "oMarca", "render": function (data) {
                return data.Descripcion
            }
        },
        {
            "data": "oLinea", "render": function (data) {
                return data.Descripcion
            }
        },
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
        { "data": "Valor" },

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

//ENLISTAR MARCAS COMBOS
jQuery.ajax({
    url: '/Mantenedor/ListarMarca',
    type: "GET",
    data: null,
    dataType: "json",
    contentType: "application/json; chartset=utf-8",
    success: function (data) {
        $("<option>").attr({ "value": "0", "disabled": "true" }).text("Seleccionar").appendTo("#cbomarca");

        $.each(data.data, function (index, valor) {
            $("<option>").attr({ "value": valor.IdMarca }).text(valor.Descripcion).appendTo("#cbomarca");
        })

    },
    error: function (error) {
        console.log(error)
    }
});
//ENLISTAR Linea COMBOS
jQuery.ajax({
    url: '/Mantenedor/ListarLinea',
    type: "GET",
    data: null,
    dataType: "json",
    contentType: "application/json; chartset=utf-8",
    success: function (data) {
        $("<option>").attr({ "value": 0, "disabled": "true" }).text("Seleccionar").appendTo("#cbolinea")

        $.each(data.data, function (index, valor) {
            $("<option>").attr({ "value": valor.IdLinea }).text(valor.Descripcion).appendTo("#cbolinea")
        })

    },
    error: function (error) {
        console.log(error)
    }
});



//VALIDAR CAMPOS Y FORMATOS
jQuery.validator.addMethod("preciodecimal", function (value, element) {
    return this.optional(element) || /^\d{0,5}(\.\d{0,2})?$/i.test(value);
}, "El formato correcto del precio es ##.##");

$("#contenedor").validate({
    rules: {
        /*SON LOS NAMES DE LOS INPUTS*/
        nombre: {
            required: true
        },
        descripcion: {
            required: true
        },
        precio: {
            required: true
        },
        minimo: {
            required: true,
            number: true
        },
        maximo: {
            required: true,
            number: true
        }
    },
    messages: {
        nombre: "- El campo nombre es obligatorio",
        descripcion: "- El campo descripcion es obligatorio",
        precio: { required: "- El campo precio es obligatorio", precio: "- El formato correcto del precio es ##.##" },
        minimo: { required: "- El campo minimo es obligatorio", minimo: "-Debe de ingresar solo numeros en el campo minimo" },
        maximo: { required: "- El campo maximo es obligatorio", maximo: "-Debe de ingresar solo numeros en el campo maximo" },
    },
    errorElement: "div",
    errorLabelContainer: ".alert-danger"

})


//ABRIR MODAL
function abrirModal(json) {
    $('#txtid').val(0);
    $('#img_producto').removeAttr("src");
    $('#fileProducto').val("");
    $('#txtnombre').val("");
    $('#txtdescripcion').val("");
    $('#cbomarca').val($('#cbomarca option:first').val());
    $('#cbolinea').val($('#cbolinea option:first').val());
    $('#txtprecio').val("");
    $('#txtmaximo').val("");
    $('#txtminimo').val("");
    $('#cboactivo').val("A");
    $('#mensajeError').hide();


    if (json != null) {

        var textActiv;
        if (json.Activo == "A") {
            textActiv = "A";
        } else if (json.Activo == "O") {
            textActiv = "O";
        } else {
            textActiv = "D";
        }

        $('#txtid').val(json.IdProducto);
        $('#txtnombre').val(json.Descripcion);
        $('#txtdescripcion').val(json.Valor);
        $('#cbomarca').val(json.oMarca.IdMarca);
        $('#cbolinea').val(json.oLinea.IdLinea);
        $('#cboactivo').val(textActiv);
        $('#txtprecio').val(json.Precio);
        $('#txtminimo').val(json.Minimo);
        $('#txtmaximo').val(json.Maximo);



    }
    $("#FormModal").modal("show");
}
//BOTON EDITAR
$("#tabla tbody").on("click", '.btn-editar', function () {
    filaSeleccionada = $(this).closest("tr");
    var data = tabladata.row(filaSeleccionada).data();
    console.log(tabladata.row(filaSeleccionada).data());

    abrirModal(data);
})
//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var Productoseleccionado = $(this).closest("tr");
    var data = tabladata.row(Productoseleccionado).data();
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Producto?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {

            jQuery.ajax({
                url: '/Mantenedor/EliminarProducto',
                type: "POST",
                data: JSON.stringify({ id: data.IdProducto }),
                dataType: "json",
                contentType: "application/json; chartset=utf-8",
                success: function (data) {

                    if (data.resultado) {
                        tabladata.row(Productoseleccionado).remove().draw()
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
//


//BOTON GUARDAR (MODAL)
function Guardar() {
    //VALIDAR LOS CAMPOS
    //if (!$("#contenedor").valid()) {
    //    return;
    //}

    var ImagenSeleccionada = $("#fileProducto")[0].files[0];
    var textActivo;
    var comp = $('#cboactivo').val();

    if (comp == "A") {
        textActivo = "A";
    } else if (comp == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }

    var Producto = {
        IdProducto: $('#txtid').val(),
        Descripcion: $('#txtnombre').val(),
        Precio: $("#txtprecio").val(),
        Minimo: $("#txtminimo").val(),
        Maximo: $("#txtmaximo").val(),
        oMarca: {
            IdMarca: $("#cbomarca option:selected").val(),
            Descripcion: $("#cbomarca option:selected").text()
        },
        oLinea: {
            IdLinea: $("#cbolinea option:selected").val(),
            Descripcion: $("#cbolinea option:selected").text()
        },
        Activo: textActivo,
        Valor: $('#txtdescripcion').val(),
        Img: "",
    }

    var request = new FormData();
    request.append("objeto", JSON.stringify(Producto));
    request.append("archivoImagen", ImagenSeleccionada);

    jQuery.ajax({
        url: '/Mantenedor/GuardarProducto',
        type: "POST",
        data: request,
        processData: false,
        contentType: false,
        success: function (data) {
            $('.modal-body').LoadingOverlay("hide");

            //Producto nuevo
            if (Producto.IdProducto == 0) {
                if (data.IdGenerado != 0) {
                    Producto.IdProducto = data.IdGenerado;
                    tabladata.row.add(Producto).draw(false);
                    $("#FormModal").modal("hide");
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();

                }

            }
            //Producto editar
            else {
                if (data.operacionExitosa) {
                    $("#FormModal").modal("hide");
                    tabladata.row(filaSeleccionada).data(Producto).draw(false);
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
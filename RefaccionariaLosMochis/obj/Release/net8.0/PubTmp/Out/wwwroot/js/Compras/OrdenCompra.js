// Evento para eliminar un detalle de compra
$("tbody").on("click", ".btn-eliminar", function () {
    var tr = $(this).closest("tr");

    var totalRows = $("tbody tr").length;
    if (totalRows == 1) {
        swal({
            title: "¿Esta Seguro?",
            text: "Si se elimina el detalla de compra se eliminara la compra tambien",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Si",
            cancelButtonText: "No",

        },
            function () {
            var dataId = $("#ID").data("id");
                JQueryAjax_Normal('/Compras/EliminarCompra', { compraId: dataId }, true, function (data) {
                    setTimeout(function () {
                        swal("Eliminada Compra", data.Mensaje, "success")
                    }, 250);
                    window.location.href = '/Compras/Administracion' ;

                }, function () { });

            }, function () { });



    } else {
        swal({
            title: "¿Esta Seguro?",
            text: "Si se elimina el detalla de compra se eliminara la compra tambien",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Si",
            cancelButtonText: "No",

        },
            function () {
                var dataId = tr.data("id");

                JQueryAjax_Normal('/Compras/EliminarCompraDTL', { compraId: dataId }, true, function (data) {
                    setTimeout(function () {
                        swal("Eliminada Compra", data.Mensaje, "success")
                    }, 250);
                    location.reload();

                }, function () { });
            }, function () { });
    }

});

// Evento para actualizar la cantidad al perder el foco
$("tbody").on("blur", "td.cantidad input", function () {
    var tr = $(this).closest("tr");
    var dataId = $(this).data("id");
    var idcambio = $(this).val();
    var CompraID = tr.data("id");
    CambioCantidades(dataId, idcambio, CompraID);
});

// Evento para actualizar la cantidad al presionar Enter
$("tbody").on("keypress", "td.cantidad input", function (event) {
    if (event.which === 13) { // 13 es el código de tecla Enter
        var tr = $(this).closest("tr");
        var dataId = $(this).data("id");
        var idcambio = $(this).val();
        var CompraID = tr.data("id");
        CambioCantidades(dataId, idcambio, CompraID);
    }
});

// Función para actualizar cantidades y recalcular subtotales
function CambioCantidades(dataId, idcambio, CompraID) {
    if (dataId != idcambio) {
        JQueryAjax_Normal('/Compras/ActualizarCantidadCompraDtl', { idCompraDtl: CompraID, nuevaCantidad: idcambio }, true, function (data) {
            if (data.Mensaje == "") {
                var tr = $(`tr[data-id="${CompraID}"]`);
                var cantidadInput = tr.find(".cantidad input");
                cantidadInput.attr('data-id', idcambio); // Actualiza el data-id del input

                var precioUnitario = parseFloat(tr.find('td:nth-child(5)').text());
                var nuevoSubtotal = idcambio * precioUnitario;
                tr.find('td:nth-child(6)').text(nuevoSubtotal.toFixed(2));

                // Llamar a la función para actualizar el total
                actualizarTotal();
            } else {
                swal("No se pudo actualizar", data.mensaje, "error");
            }
        }, function (error) {
            console.error("Error al listar compras:", error);
        });
    } else {
        console.log("IGUAL");
    }
}

// Función para recalcular y actualizar el total
function actualizarTotal() {
    var nuevoTotal = 0;
    $('tbody tr').each(function () {
        nuevoTotal += parseFloat($(this).find('td:nth-child(6)').text());
    });
    $('tfoot td:last-child').text(nuevoTotal.toFixed(2));
}

$("#btn-Aprobar").click(function () {
    swal({
        title: "¿Está Seguro?",
        text: "¿Desea aprobar la Compra?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Sí",
        cancelButtonText: "No",
    },
        function () {
            var CompraID = $("#ID").data("id");

            JQueryAjax_Normal('/Compras/AprobarCompra', { compraId: CompraID }, true, function (data) {
                setTimeout(function () {
                    swal("Estatus Compra", data.Mensaje, "success");
                }, 5000);
                    location.reload();
            }, function () { });
        });
});
$("#btn-Cancelar").click(function () {

    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea cancelar la Compra?",
        type: "input",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: false,
        inputPlaceholder: "Ingrese motivo de cancelacion"
    },
        function (inputValue) {
            if (inputValue === false) return false;
            if (inputValue === "") {
                swal.showInputError("Debe ingresar un comentario!");
                return false;
            }
            var CompraID = $("#ID").data("id");

            JQueryAjax_Normal('/Compras/CancelarCompra', { compraId: CompraID, comentario: inputValue }, true, function (data) {
                setTimeout(function () {
                    swal("Estatus Compra", data.Mensaje, "success")
                }, 250);
                location.reload();
                cargartabla()
            }, function () { });
        });
});

$("#btn-Descargar").click(function () {
    var CompraID = $("#ID").data("id");
    $.ajax({
        url: '/Compras/CompraPDF',
        type: 'GET',
        data: { idCompra: CompraID },
        xhrFields: {
            responseType: 'blob' // Para recibir datos como blob
        },
        success: function (blob) {
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = `Compra_${CompraID}.pdf`;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        },
        error: function (xhr, status, error) {
            console.error('Error al generar el PDF: ', error);
        }
    });

});
$("#btn-DescargarProveedor").click(function () {
    var idCompra = $("#ID").data("id");
    $.ajax({
        url: '/Compras/GenerarPdfConQrProveedor',
        type: 'GET',
        data: { idCompra: idCompra },
        xhrFields: {
            responseType: 'blob' // Para recibir datos como blob
        },
        success: function (blob) {
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = `Compra.pdf`;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        },
        error: function (xhr, status, error) {
            console.error('Error al generar el PDF: ', error);
        }
    });

});

$("#btn-Guardar").click(function () {

    var listaCompraDtl = [];

    var fechaEstimadaEntrega = $("#fecha").data("fecha");
    var idCompra = $("#ID").data("id");
    $(' tbody tr').each(function () {
        var idProductoProveedor = $(this).find('.cboProveedor').val();
        var cantidad = $(this).find('.txtCantidad').val();
        var precio = $(this).find('.txtPrecioUnitario').val(); // Notar la minúscula en 'precio'
        
        if (idProductoProveedor && cantidad) {
            var CompraDtl = {
                oProductoProveedor: {
                    IdProductoProveedor: parseInt(idProductoProveedor),
                },
                Cantidad: parseInt(cantidad),
                Precio: parseFloat(precio),
                FechaEstimadaEntrega: fechaEstimadaEntrega 
            }


            listaCompraDtl.push(CompraDtl);
        }
    });

    console.log(listaCompraDtl)

    JQueryAjax_Normal('/Compras/AgregarCompraDTL', { listaCompraDtl: listaCompraDtl, idCompra: idCompra  }, true, function (data) {
        if (data.bien) {
            window.location.href = '/Compras/OrdenCompra?idCompra=' + idCompra;
        } else {
            swal("Mal", "error: " + data.mensaje, "error");
        }
    }, function () {
        swal("Error", "Hubo un problema al obtener los detalles del producto.", "error");
    });

});


//////////////////////////////NUEVO
// Función para agregar una nueva fila
// Verificar que el script se carga

    $('#btn-Agregar').on('click', function () {
        console.log("Botón Agregar clickeado"); // Verificar que el evento click funciona

        var nuevaFila = '<tr>' +
            '<td><input type="text" class="form-control form-control-sm txtNoParte" placeholder="NoParte"></td>' +
            '<td><input type="text" disabled class="form-control form-control-sm txtDescripcion" placeholder="Descripción"></td>' +
            '<td><select class="form-control form-control-sm cboProveedor"></select></td>' +
            '<td><input type="number" class="form-control form-control-sm txtCantidad" placeholder="Cantidad"></td>' +
            '<td><input type="number" class="form-control form-control-sm txtPrecioUnitario" placeholder="Precio Unitario"></td>' +
            '<td><span class="subtotal">0</span></td>' +
            '<td><button type="button" class="btn-eliminar-tabla btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button></td>' +
            '</tr>';
        $('#tblDatosProductos').append(nuevaFila);
    });

    // Otros eventos y funciones...


// Evento para eliminar una fila
$('#tblDatosProductos').on('click', '.btn-eliminar-tabla', function () {
    $(this).closest('tr').remove();
    actualizarTotal();
});

// Evento para calcular el subtotal al cambiar cantidad o precio unitario
$('#tblDatosProductos').on('blur', '.txtCantidad, .txtPrecioUnitario', function () {
    var fila = $(this).closest('tr');
    var cantidad = parseFloat(fila.find('.txtCantidad').val()) || 0;
    var precioUnitario = parseFloat(fila.find('.txtPrecioUnitario').val()) || 0;
    var subtotal = cantidad * precioUnitario;
    fila.find('.subtotal').text(subtotal.toFixed(2));

    actualizarTotal();
});

// Función para actualizar el total
function actualizarTotal() {
    var total = 0;
    $('#tblDatosProductos .subtotal').each(function () {
        total += parseFloat($(this).text()) || 0;
    });
    $('#total').text(total.toFixed(2));
}

// Evento para obtener detalles del producto por número de parte
$('#tblDatosProductos').on('blur', '.txtNoParte', function () {
    var noParte = $(this).val().trim();
    if (noParte !== '') {
        var currentRow = $(this).closest('tr');

        JQueryAjax_Normal('/Compras/ObtenerCompraDtlPorNoParte', { noParte: noParte }, true, function (data) {
            if (data.data.length > 0) {
                var descripcionInput = currentRow.find('.txtDescripcion');
                var proveedorInput = currentRow.find('.cboProveedor');

                // Asigna la descripción del primer resultado
                descripcionInput.val(data.data[0].oProductoProveedor.oProducto.Descripcion);

                // Llenar la lista de proveedores
                proveedorInput.empty();
                data.data.forEach(function (CompraDtl) {
                    var proveedorRazonSocial = CompraDtl.oProductoProveedor.oProveedor.RazonSocial;
                    var idProductoProveedor = CompraDtl.oProductoProveedor.IdProductoProveedor;

                    if (!proveedorInput.find('option[value="' + idProductoProveedor + '"]').length) {
                        proveedorInput.append(new Option(proveedorRazonSocial, idProductoProveedor));
                    }
                });
            } else {
                swal("No se encontraron proveedores", "No se encontraron proveedores para el número de parte proporcionado.", "error");
            }
        }, function () {
            swal("Error", "Hubo un problema al obtener los detalles del producto.", "error");
        });
    }
});

// Evento para manejar el cambio de proveedor
$('#tblDatosProductos').on('change', '.cboProveedor', function () {
    var selectedOption = $(this).find('option:selected');
    var idProductoProveedor = selectedOption.val();
    var currentRow = $(this).closest('tr');
    // Aquí puedes manejar cualquier lógica adicional al cambiar el proveedor
});


$('#btn-Recivir').on('click', function () {

    var idCompra = $("#ID").data("id");
    window.location.href = '/Compras/RecibirMercancia?idCompra=' + idCompra;
});
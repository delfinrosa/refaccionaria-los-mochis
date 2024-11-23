

$('#BuscarCodigoBarras').focus();

$("#btn-BuscarCodigoBarras").on('click', function () {
    $('#BuscarCodigoBarras').focus();
})


$('#BuscarCodigoBarras').on('change', async function () {

    var codigoBarras = $(this).val();
    var data2;
    if (codigoBarras) {
        // Llamar a la función AJAX para buscar productos
        await new Promise((resolve, reject) => {
            JQueryAjax_Normal(
                '/Producto/BuscarProductosPorCodigoBarras', // URL del controlador
                { codigoBarras: codigoBarras }, // Parámetros a enviar
                false,
                function (data) {
                    data2 = data;
                    resolve(); // Resolver la promesa una vez que se asigna data2
                },
                function (error) {
                    console.error("Error al obtener productos:", error);
                    reject(error); // Rechazar la promesa en caso de error
                }
            );
        });

        if (data2 && data2.data) {
            if (data2.data.length === 1) {
                // Si solo hay un producto, agregarlo directamente al carrito
                var producto = data2.data[0];
                var idProducto = producto.idProducto;
                seleccionarProducto(idProducto);
                $('#BuscarCodigoBarras').val('');
            } else {
                for (const producto of data2.data) {
                    const isConfirm = await new Promise((resolve) => {
                        swal({
                            title: "Agregar producto",
                            text: "¿Quieres agregar el producto " + producto.descripcion + " de la marca " + producto.oMarca.descripcion + "?",
                            type: "warning",
                            showCancelButton: true,
                            confirmButtonClass: "btn-primary",
                            confirmButtonText: "Si",
                            cancelButtonText: "No",
                            closeOnConfirm: false // No cerrar automáticamente
                        }, function (isConfirm) {
                            resolve(isConfirm);
                            swal.close(); // Cerrar la alerta manualmente
                        });
                    });

                    if (isConfirm) {
                        var idProducto = producto.idProducto;
                        var noParte = producto.noParte;
                        seleccionarProducto(idProducto);

                    }

                    // Pausa breve antes de mostrar la siguiente alerta
                    await new Promise(resolve => setTimeout(resolve, 200));
                }

                $('#BuscarCodigoBarras').val('');
            }
        }
    }
});




$(document).on('input', '#inputNoParte', function () {
    var valorNoParte = $(this).val();
    if (valorNoParte.length > 2) {
        BusquedaProductoVenta();
    }
});

function BusquedaProductoVenta() {
    var valorNoParte = $('#inputNoParte').val();

    JQueryAjax_Normal('/Venta/BuscarProductos', { NoParte: valorNoParte }, true, function (data) {
        $('#resultadosBusqueda').empty();

        if (data.lista.length > 0) {
            data.lista.forEach(function (producto, index) {
                var resultadoHtml = `
                    <div class="row my-2 resultado-item" data-index="${index}" tabindex="0"  style="position: relative; z-index: 20;">
                        <div class="col-8">${producto.noParte}</div>
                        <div class="col-6"><span class="badge bg-primary">${producto.oMarca.descripcion}</span></div>
                    </div>
                `;
                $('#resultadosBusqueda').append(resultadoHtml);
            });
            $('.resultado-item').on('click', function () {
                seleccionarProducto(data.lista[$(this).data('index')].idProducto);
            });

            $('#resultadosBusqueda').on('keydown', '.resultado-item', function (e) {
                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    $(this).next().focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    $(this).prev().focus();
                } else if (e.key === 'Enter') {
                    e.preventDefault();
                    seleccionarProducto(data.lista[$(this).data('index')].idProducto);

                }
            });


            $('#inputNoParte').on('keydown', function (e) {
                if (e.key === 'Enter') {
                    $('.resultado-item').first().focus();
                }
                if (e.key === 'ArrowDown') {
                    $('.resultado-item').first().focus();
                }
            });
            // Coloca el foco en el primer elemento de la lista de resultados
        } else {
            $('#resultadosBusqueda').html('<p>No se encontraron productos.</p>');
        }

    });

    // Habilita o deshabilita el campo Marca según el valor de NoParte
    $('#inputMarca').prop('disabled', valorNoParte.trim() === '');


}



$('#btnAgregarProducto').on('click', function () {
    // Verificar si ya existen los campos de entrada
    if ($('#campos-agregar-producto').length === 0) {

        var camposHtml = `
                            <div id="campos-agregar-producto" class="row my-3">
                                <div class="col-6 text-center mx-auto" id="NoParteDiv">
                                    <input type="text" id="inputNoParte" class="form-control" placeholder="No. Parte" required autocomplete="off" name="no_parte" />
                                    <div id="NoParteBuscador" class="mt-2"></div>
                                </div>
                                <input type="number" id="inputIdProducto" hidden />
                            </div>
                                `;
        $('#carrito-items').append(camposHtml);


        $('#NoParteDiv').append('<div id="resultadosBusqueda" class="mt-2"></div>');

        $("#inputNoParte").focus();
    }
});


function seleccionarProducto(id) {
    JQueryAjax_Normal('/Venta/ObtenerVentasPorID', { id: id }, true, function (data) {
        if (data.error == "") {
            $("#ventas-container").empty();

            let ventasHtml = `
                <table class="table table-bordered table-striped">
                    <thead class="thead-dark">
                        <tr>
                            <th>ID Venta</th>
                            <th>RFC Cliente</th>
                            <th>Fecha de Venta</th>
                            <th>Vendedor</th>
                            <th>Cajero</th>
                            <th>Estatus</th>
                            <th>Tipo de Pago</th>
                            <th>Nombre Cliente</th>
                            <th>Cambio</th>
                            <th>Requiere Factura</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
            `;

            // Generar filas de ventas con jQuery
            $.each(data.venta, function (index, venta) {
                ventasHtml += `
                    <tr>
                        <td>${venta.idVenta}</td>
                        <td>${venta.rfcCliente}</td>
                        <td>${venta.fecha}</td>
                        <td>${venta.oVendedor ? venta.oVendedor.nombre : 'N/A'}</td>
                        <td>${venta.oCajero ? venta.oCajero.nombre : 'N/A'}</td>
                        <td>${venta.estatus}</td>
                        <td>${venta.tipoPago}</td>
                        <td>${venta.nombreCliente}</td>
                        <td>${venta.cambio}</td>
                        <td>${venta.requiereFactura ? 'Sí' : 'No'}</td>
                        <td>
                            <button class="btn btn-secondary btn-sm mx-1 btn-detalle" data-id="${venta.idVenta}">
                                <i class="fas fa-eye"></i>
                            </button>
                        </td>
                    </tr>
                `;
            });

            ventasHtml += `
                    </tbody>
                </table>
            `;

            // Insertar la tabla en el contenedor
            $("#ventas-container").html(ventasHtml);
            $('#resultadosBusqueda').empty();


        } else {
            swal("Error", data.error, "error");
        }
    });


}


$(document).on("click", ".btn-detalle", function () {
    var idVenta = $(this).data("id");
    console.log(idVenta)
    $.ajax({
        url: '/Venta/CargarTicketParcial', // URL del método del controlador
        type: 'GET',
        data: { idVenta: idVenta }, // Pasar el idVenta como parámetro
        success: function (result) {
            // Insertar la vista parcial en el contenedor especificado
            $('#contenedorTicket').html(result);

            // Mostrar el ticket después de cargarlo
            $('#DivTicket').removeClass('d-none');
            $('#ticketModal').modal('show');
            $.getScript('/js/General/imprimirTicket.js');


        },
        error: function () {
            swal("Error", "No se pudo cargar la información del ticket.", "error");
        }
    });
});
$(document).ready(function () {
    var fechaActual = new Date().toISOString().split('T')[0];
    $("#dateSelect").val(fechaActual);
    //function CortePrincipal() {
    //    $.ajax({
    //        url: '/Venta/ListarTotalDineroDiaSeleccionado', // URL del método del controlador
    //        type: 'GET',
    //        dataType: 'json',
    //        success: function (data) {
    //            if (data.lista && data.lista.length > 0) {
    //                $.each(data.lista, function (index, item) {
    //                    switch (item.tipoPago) {
    //                        case 'EFECTIVO':
    //                            $('#totalEfectivo').text(`$${item.totalDinero}`);
    //                            break;
    //                        case 'TARJETA':
    //                            $('#totalTarjeta').text(`$${item.totalDinero}`);
    //                            break;
    //                        case 'TRANSFERENCIA':
    //                            $('#totalTranseferencia').text(`$${item.totalDinero}`);
    //                            break;
    //                    }
    //                });
    //            } else {
    //                console.log("La lista está vacía en la respuesta.");
    //            }
    //        },
    //        error: function (jqXHR, textStatus, errorThrown) {
    //            var errorMsg = errorThrown || 'Ocurrió un error en la solicitud.';
    //            swal("Error", "Hubo un problema al procesar la venta: " + errorMsg, "error");
    //            console.log("Error en la solicitud: ", errorMsg);
    //        }
    //    });
    //}
    function CortePrincipal() {
        const selectedDate = $('#dateSelect').val(); // Obtener la fecha seleccionada
        if ($('.compra-info').length) {
            $('.compra-info').remove();
        }
        $.ajax({
            url: '/Venta/ListarTotalDineroDiaSeleccionado', // URL del método del controlador
            type: 'GET',
            dataType: 'json',
            data: { dia: selectedDate }, // Pasar la fecha seleccionada como parámetro
            success: function (data) {
                if (data.lista && data.lista.length > 0) {
                    $.each(data.lista, function (index, item) {
                        switch (item.tipoPago) {
                            case 'EFECTIVO':
                                $('#totalEfectivo').text(`$${item.totalDinero}`);
                                break;
                            case 'TARJETA':
                                $('#totalTarjeta').text(`$${item.totalDinero}`);
                                break;
                            case 'TRANSFERENCIA':
                                $('#totalTransferencia').text(`$${item.totalDinero}`);
                                break;
                        }
                    });
                } else {
                    console.log("La lista está vacía en la respuesta.");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var errorMsg = errorThrown || 'Ocurrió un error en la solicitud.';
                swal("Error", "Hubo un problema al procesar la venta: " + errorMsg, "error");
                console.log("Error en la solicitud: ", errorMsg);
            }
        });
        $('#dateSelect').change(function () {
            CortePrincipal(); // Llama a la función cada vez que cambia la fecha
        });
    }


    CortePrincipal();

    $('#footerEfectivo').click(function () {
        toggleInfo($(this), 'EFECTIVO');
    });

    $('#footerTarjeta').click(function () {
        toggleInfo($(this), 'TARJETA');
    });

    $('#footerTransferencia').click(function () {
        toggleInfo($(this), 'TRANSFERENCIA');
    });

    function toggleInfo(element, tipoPago) {
        const cardBody = element.prev('.card-body');
        const selectedDate = $('#dateSelect').val(); // Obtener la fecha seleccionada

        if (cardBody.find('.compra-info').length > 0) {
            cardBody.find('.compra-info').remove();
        } else {
            JQueryAjax_Normal(
                '/Venta/ListarComprasDiaSeleccionado',
                { estatus: tipoPago, dia: selectedDate },
                true,
                function (data) {
                    let html = '<div class="compra-info mt-3">';
                    data.lista.forEach(function (item) {
                        if (item.objVenta.cambio) {
                            html += `
                            <p><strong>Hora:</strong> ${item.horaMinuto} <strong class="text-warning" ></br> Total:</strong><span class="text-warning"> $${item.totalCompra} </span></br> <strong>Productos:</strong> ${item.cantidadProductos}</br> Este producto fue pagado con efectivo y tarjeta </br> <strong>Efectivo:</strong> ${item.objVenta.cambio}</br> <strong>Tarjeta:</strong> ${item.totalCompra -item.objVenta.cambio} </p>
                            <hr/>
                            `;
                        } else {
                            html += `
                            <p><strong>Hora:</strong> ${item.horaMinuto} </br> <strong >Total:</strong> $${item.totalCompra} </br><strong>Productos:</strong> ${item.cantidadProductos}</p>
                            <hr/>
                            `;
                        }
                    });

                    html += '</div>';

                    cardBody.append(html);
                },
                function (error) {
                    swal("Error", "Hubo un problema al obtener la información de las compras.", "error");
                }
            );
        }
    }
});

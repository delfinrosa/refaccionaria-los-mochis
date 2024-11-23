function CortePrincipal() {
    JQueryAjax_Normal(
        '/Venta/ListarTotalDineroHoy', {}, true,
        function (data) {
            // Asignar los valores a cada tarjeta usando jQuery
            $.each(data.lista, function (index, item) {
                switch (item.Item1) {
                    case 'EFECTIVO':
                        $('#totalEfectivo').text(`$${item.Item2}`);
                        break;
                    case 'TARJETA':
                        $('#totalTarjeta').text(`$${item.Item2}`);
                        break;
                    case 'TRANSFERENCIA':
                        $('#totalTranseferencia').text(`$${item.Item2}`);
                        break;
                }
            });
        },
        function (error) {
            swal("Error", "Hubo un problema al procesar la venta.", "error");
        }
    );
}

// Llamar a la función para cargar los datos al inicio
CortePrincipal();

$('#footerEfectivo').click(function () {
    toggleInfo($(this), 'EFECTIVO');
});

// Asignar el evento click al card-footer de Tarjeta
$('#footerTarjeta').click(function () {
    toggleInfo($(this), 'TARJETA');
});

// Asignar el evento click al card-footer de Transferencia
$('#footerTransferencia').click(function () {
    toggleInfo($(this), 'TRANSFERENCIA');
});

// Función que alterna la información en la tarjeta
function toggleInfo(element, tipoPago) {
    const cardBody = element.prev('.card-body');

    if (cardBody.find('.compra-info').length > 0) {
        // Si ya existe la información desplegada, la elimina
        cardBody.find('.compra-info').remove();
    } else {
        // Si no existe la información desplegada, realiza la solicitud AJAX
        JQueryAjax_Normal(
            '/Venta/ListarComprasHoyEfectivo',
            { estatus: tipoPago },
            true,
            function (data) {
                console.log(data);
                let html = '<div class="compra-info mt-3">';

                // Recorrer el listado de compras y añadir la información
                data.lista.forEach(function (item) {
                    html += `
                            <p><strong>Hora:</strong> ${item.Item2} <strong>Total:</strong> $${item.Item3} <strong>Productos:</strong> ${item.Item4}</p>
                            <hr/>
                        `;
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
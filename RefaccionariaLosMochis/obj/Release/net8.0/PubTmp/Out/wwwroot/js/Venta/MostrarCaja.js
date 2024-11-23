// Método para manejar el éxito de la solicitud AJAX


function manejarRespuestaObtenerVentas(response) {
    if (response.lista && response.lista.length > 0) {
        let html = `<table class="table table-striped">
            <thead class="thead-dark">
                <tr>
                    <th>Nombre Cliente</th>
                    <th>Nombre Vendedor</th>
                    <th>Cantidad Productos</th>
                    <th>Total a Pagar</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>`;

        response.lista.forEach(function (item) {
            let idVenta = item.Item5 || '';
            let nombreCliente = item.Item1 || 'N/A';
            let nombreVendedor = item.Item2 || 'N/A';
            let cantidadProductos = item.Item3 || 0;
            let totalPagar = (typeof item.Item4 === 'number' && !isNaN(item.Item4)) ? item.Item4.toFixed(2) : '0.00';

            html += `<tr>
                <td>${nombreCliente}</td>
                <td>${nombreVendedor}</td>
                <td>${cantidadProductos}</td>
                <td>${totalPagar}</td>
                <td>
                    <button class="btn btn-primary btn-sm mx-1 btn-detalle" data-id="${idVenta}">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="btn btn-secondary btn-sm mx-1 btn-cancelar" data-id="${idVenta}">
                        <i class="fas fa-window-close"></i>
                    </button>
                </td>
            </tr>`;
        });

        html += "</tbody></table>";
        $("#resultados").html(html);
    } else {
        $("#resultados").html(`<p>${response.mensaje || 'No se encontraron ventas.'}</p>`);
    }
}

function manejarErrorObtenerVentas(error) {
    $("#resultados").html(`<p>Error: ${error || 'Ocurrió un error inesperado.'}</p>`);
}

function obtenerVentasAgrupadas() {
    JQueryAjax_Normal("/Venta/ObtenerVentasAgrupadas", {}, true, manejarRespuestaObtenerVentas, manejarErrorObtenerVentas);
}

$("#btnObtenerVentas").click(function () {
    obtenerVentasAgrupadas();
});

$("#resultados").on('click', '.btn-detalle', function () {
    var idVenta = $(this).data('id');
    if (idVenta) {
        window.location.href = '/Venta/Ticket?idVenta=' + encodeURIComponent(idVenta);
    }
});

obtenerVentasAgrupadas();

setInterval(function () {
    obtenerVentasAgrupadas();
}, 5000); 



$("#resultados").on('click', '.btn-cancelar', function () {
    var idVenta = $(this).data('id');
    JQueryAjax_Normal("/Venta/CancelarVenta", { idVenta: idVenta }, true, 
        function () {swal("Correcto", "Se a Cancelado la Venta correctamente.", "sucess");
} 
        , 
        function () {
            swal("Error", "Se a ocacionado un error vuelva a cargar la ventas existentes.", "error");
        } 
    );

});

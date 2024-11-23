$(document).ready(function () {
    $('#btnGuardar').click(function () {
        var productosSinCantidad = [];

        $('tbody tr').each(function () {
            var noParte = $(this).find('td').eq(0).text().trim();
            var idCompraDtl = $(this).find('td').eq(1).text().trim();
            var cantidad = $(this).find('.cantidad-input').val();

            if (cantidad) {
                productosSinCantidad.push({
                    NoParte: noParte,
                    CompraDtlId: idCompraDtl,
                    CantidadEntrada: cantidad
                });
            }
        });
        JQueryAjax_Normal(
            '/Compras/ActualizarCompraDtlIncompleto',
            { productosSinCantidad: productosSinCantidad },
            true,
            function (response) {
                if (response.Exito) {
                    window.location.href = '/Compras/Administracion';

                } else {
                    alert("Error: " + response.Mensaje);
                }
            },
            function (error) {
                alert('Error: ' + error);
            }
        );
    });
});

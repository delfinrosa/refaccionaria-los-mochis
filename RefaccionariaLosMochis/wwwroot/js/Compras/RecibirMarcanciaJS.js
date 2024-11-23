// Función para recolectar los datos de la tabla y enviar la solicitud AJAX
$("#Actualizar").click(function () {
    // Crear un array para almacenar el estado de los productos
    var estadoProductos = [];

    // Recolectar el estado de cada fila en la tabla
    $("#table tbody tr").each(function () {
        var $row = $(this);

        // Obtener los valores de los campos ocultos y las casillas de verificación usando clases
        var compraDtlId = $row.find(".compraDtlId").val();
        var isPrecioChecked = $row.find(".precio-checkbox").is(":checked");
        var isCantidadChecked = $row.find(".cantidad-checkbox").is(":checked");

        // Agregar los datos al array estadoProductos
        estadoProductos.push({
            CompraDtlId: compraDtlId,
            IsPrecioChecked: isPrecioChecked,
            IsCantidadChecked: isCantidadChecked
        });
    });


    var idCompra = $("#idCompra").val();

    JQueryAjax_Normal(
        '/Compras/ActualizarCompraDtl',
        { estadoProductos: estadoProductos, idCompra: idCompra },
        true,
        function (data) {
            if (data.exito) {
                // Filtrar productos que no tienen cantidad marcada
                var productosSinCantidad = estadoProductos.filter(function (producto) {
                    return !producto.IsCantidadChecked;
                });

                // Redirigir según el estado de los productos
                if (productosSinCantidad.length === 0) {
                    window.location.href = '/Compras/Administracion';
                } else {
                    // Crear el array de productosSinCantidad con la estructura requerida
                    var productosSinCantidadDetalles = [];

                    $("#table tbody tr").each(function () {
                        var $row = $(this);
                        var isCantidadChecked = $row.find(".cantidad-checkbox").is(":checked");

                        // Si el checkbox de cantidad no está marcado
                        if (!isCantidadChecked) {
                            var noParte = $row.find("td:eq(0)").text(); // Obtener NoParte (columna 0)
                            var compraDtlId = $row.find(".compraDtlId").val(); // Obtener CompraDtlId

                            // Agregar el objeto con la estructura anidada correcta
                            productosSinCantidadDetalles.push({
                                oProductoProveedor: {
                                    oProducto: {
                                        NoParte: noParte.trim()
                                    }
                                },
                                CompraDtlId: compraDtlId
                            });
                        }
                    });

                    window.location.href = '/Compras/RecibirMercanciaIncompleto?idCompra=' + idCompra + '&productos=' + encodeURIComponent(JSON.stringify(productosSinCantidadDetalles));
                }

            } else {
                alert(data.mensaje);
            }
        },
        function (error) {
            // Manejar errores en la solicitud
            alert("Error en la solicitud: " + error);
        }
    );
});

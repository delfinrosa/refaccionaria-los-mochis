


    $('#BuscarCodigoBarras').focus();

    $("#btn-BuscarCodigoBarras").on('click', function () {
        $('#BuscarCodigoBarras').focus();
    })


    $('#BuscarCodigoBarras').on('change', function () {
        var codigoBarras = $(this).val();
        var encontrado = false;
        var filaEncontrada = null;

        $('table tbody tr').each(function () {
            var codigoBarrasFila = $(this).data('codigobarras');

            if (codigoBarrasFila === codigoBarras) {
                encontrado = true;
                filaEncontrada = $(this);
                $("#DevolucionCantidad").val(filaEncontrada.find('td:nth-child(3)').text());
                $("#productoDevolver").data('id', $(this).data('id'));
                $("#productoDevolver").text("¿Por que quiere devolver " + filaEncontrada.find('td:nth-child(1)').text() + " de la marca " + filaEncontrada.find('td:nth-child(2)').text() +" ?")
                $('#miModal').modal('show');
                return false;
            }
        });

        if (!encontrado) {
            swal("Error", "Código de barras no encontrado en el carrito.", "error");
        }
    });



    function aplicarColorFilasTabla() {
        $('table tbody tr').each(function () {
            var estatus = $(this).data('estatus'); // Obtener el valor del atributo data-Estatus

            // Aplicar la clase de color basada en el valor de estatus
            if (estatus === 'A') {
                $(this).addClass('bg-rojo');
            } else if (estatus === 'V') {
                $(this).addClass('bg-verde');
            } else if (estatus === 'D') {
                $(this).addClass('bg-gris');
            }
        });
    }


   

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
                    seleccionarProducto($(this).data('index'), data.lista);
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
                        seleccionarProducto($(this).data('index'), data.lista);
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


    function seleccionarProducto(index, lista) {
        var producto = lista[index];

        idseleccion = producto.idProducto;
        $('#inputNoParte').val("");

        $('table tbody tr').each(function () {
            var codigoBarrasFila = $(this).data('idproducto');

            if (codigoBarrasFila === idseleccion) {
                encontrado = true;
                filaEncontrada = $(this);

                var idVentasProducto = $(this).data('id');
                $("#DevolucionCantidad").val(filaEncontrada.find('td:nth-child(3)').text());
                $("#productoDevolver").data('id', $(this).data('id'));
                $("#productoDevolver").text("¿Por que quiere devolver " + filaEncontrada.find('td:nth-child(1)').text() + " de la marca " + filaEncontrada.find('td:nth-child(2)').text() + " ?")
                $('#miModal').modal('show');

                return false;
            }
            $('#resultadosBusqueda').empty();
        });

    }




$('input[name="razon"]').on('change', function () {
    if ($(this).val() === 'garantia') {
        $('#motivoTextarea').prop('disabled', false); // Habilitar el textarea
    } else {
        $('#motivoTextarea').val(''); // Vaciar el textarea
        $('#motivoTextarea').prop('disabled', true); // Deshabilitar el textarea
    }
});

$("#Delvolucion").click(function () {
    let motivoSeleccionado = $('input[name="razon"]:checked').val();

    if (motivoSeleccionado === 'garantia') {
        motivoSeleccionado = $('#motivoTextarea').val();
    }
    $("#Delvolucion").click(function () {
        let motivoSeleccionado = $('input[name="razon"]:checked').val();

        if (motivoSeleccionado === 'garantia') {
            motivoSeleccionado = $('#motivoTextarea').val();
        }

        JQueryAjax_Normal(
            '/Venta/DevolucionProducto',
            {
                garantia: motivoSeleccionado,
                idVentaProducto: $("#productoDevolver").data('id'),
                idVenta: $('#ID').data('id'),
                cantidad: $("#DevolucionCantidad").val()
            },
            true,
            function (data) {
                // Verificar si hubo un error
                if (data.exito === false) {
                    swal("Error", data.mensaje || "Error al procesar la devolución.", "error");

                } else {
                    swal("Éxito", data.mensaje || "La devolución se realizó correctamente.", "success");
                    console.log(data.registro)
                    $('#miModal').modal('hide');
                    setTimeout(function () {
                        location.reload();
                    }, 3000);

                }
            },
            function (error) {
                console.error("Error al actualizar el estatus:", error);
                swal("Error", "Hubo un problema con la solicitud de devolución.", "error");
            }
        );

    });

});
aplicarColorFilasTabla()
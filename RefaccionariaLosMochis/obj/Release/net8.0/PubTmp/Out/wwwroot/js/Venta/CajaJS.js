$(document).ready(function () {



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
                var idVentasProducto = $(this).data('id');

                JQueryAjax_Normal(
                    '/Venta/ActualizarEstatus',
                    { idVentasProducto: idVentasProducto },
                    true,
                    function (data) {
                        if (data.exito) {
                            filaEncontrada.removeClass('bg-rojo').addClass('bg-verde');
                        } else {
                            filaEncontrada.removeClass('bg-verde').addClass('bg-rojo');
                        }
                        $('#BuscarCodigoBarras').val('');
                    },
                    function (error) {
                        console.error("Error al actualizar el estatus:", error);
                    }
                );

                return false;
            }
        });

        if (!encontrado) {
            alert('Código de barras no encontrado en el carrito.');
        }
    });




    $('table tbody tr').each(function () {
        var estatus = $(this).data('estatus'); // Obtener el valor del atributo data-Estatus

        // Aplicar la clase de color basada en el valor de estatus
        if (estatus === 'A') {
            $(this).addClass('bg-rojo');
        } else if (estatus === 'V') {
            $(this).addClass('bg-verde');
        }
    });

    $('.btn-eliminar').on('click', function () {
        var idVentasProducto = $(this).data('id');
        var idVenta = $('#ID').data('id');

        $.ajax({
            url: '/Venta/EliminarProducto', // Cambia esta URL al endpoint correcto para eliminar el producto
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ idVentasProducto: idVentasProducto, idVenta :idVenta}),
            success: function (response) {
                if (response.exito) {
                    location.reload();
                    if (response.eliminar) {
                        window.location.href = '/Venta/Caja' ;
                    }
                } else {
                    alert(response.mensaje || 'Error al eliminar el producto.');
                }
            },
            error: function (xhr, status, error) {
                alert('Error: ' + (xhr.responseJSON.mensaje || 'Ocurrió un error inesperado.'));
            }
        });
    });


    function generarTicket(idVenta) {
        JQueryAjax_Normal(
            '/Venta/GenerarTicket',
            { idVenta: idVenta },
            true,
            function (response) {
                // Aquí ya no se necesita JSON.parse porque 'response' ya es un objeto
                if (response.Exito) {
                    console.log("Impresión exitosa:", response.Mensaje);
                } else {
                    console.error("Error en la impresión:", response.Error);
                }
            },
            function (error) {
                console.error("Error en la solicitud:", error); // Mensaje de error en consola
            }
        );
    }




    $('#Imprimir').on('click', function () {
        var idVenta = $('#ID').data('id');
        generarTicket(idVenta);
    });





    function procesarPago(idVenta, tipoPago, monto) {
        var totalVenta = parseFloat($('#total-precio').data('total'));
        var cambio = monto - totalVenta;

        if (cambio < 0 && tipoPago === "EFECTIVO") {
            swal("Error", "La cantidad ingresada no es suficiente para cubrir el total de la venta.", "error");
            return;
        }

        var ventaObj = {
            IdVenta: idVenta,
            TipoPago: tipoPago,
            Cambio: tipoPago === "EFECTIVO" ? cambio : 0, // Solo calcular cambio para efectivo
            oCajero: {
                IdUsuario: "10000"
            }
        };

        JQueryAjax_Normal(
            '/Venta/ActualizarVenta',
            ventaObj,
            true,
            function (data) {
                if (data.exito) {
                    swal("Bien", "El ticket se generó correctamente.", "success");
                } else {
                    swal("Error", "Error: " + data.mensaje, "error");
                }
            },
            function (error) {
                swal("Error", "Hubo un problema al procesar la venta.", "error");
            }
        );
    }

    $('#cartEfectivo').on('click', function () {
        var idVenta = $('#ID').data('id');
        var totalVenta = parseFloat($('#total-precio').data('total'));

        swal({
            title: "Final de la Venta",
            text: "¿Con cuánto va a pagar?",
            type: "input",
            inputType: "number",
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Enviar",
            closeOnConfirm: false,
            inputPlaceholder: "Ingrese el monto"
        },
            function (inputValue) {
                if (inputValue === false) return false;
                if (inputValue === "" || isNaN(inputValue)) {
                    swal.showInputError("Debe ingresar una cantidad válida!");
                    return false;
                }

                var cantidadPago = parseFloat(inputValue);
                procesarPago(idVenta, "EFECTIVO", cantidadPago);
            });
    });

    $('#cartTarjeta').on('click', function () {
        var idVenta = $('#ID').data('id');
        var totalVenta = parseFloat($('#total-precio').data('total'));
        procesarPago(idVenta, "TARJETA", totalVenta); // Usar totalVenta como monto para tarjeta
    });

    $('#cartTrasferencia').on('click', function () {
        var idVenta = $('#ID').data('id');
        var totalVenta = parseFloat($('#total-precio').data('total'));
        procesarPago(idVenta, "TRANSFERENCIA", totalVenta); // Usar totalVenta como monto para transferencia
    });




    $(document).on('input', '#inputNoParte', function () {
        var valorNoParte = $(this).val();
        if (valorNoParte.length > 3) {
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
                        <div class="col-8">${producto.NoParte}</div>
                        <div class="col-6"><span class="badge bg-primary">${producto.oMarca.Descripcion}</span></div>
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
                });
                // Coloca el foco en el primer elemento de la lista de resultados
            } else {
                $('#resultadosBusqueda').html('<p>No se encontraron productos.</p>');
            }

            console.log(data);
        });

        // Habilita o deshabilita el campo Marca según el valor de NoParte
        $('#inputMarca').prop('disabled', valorNoParte.trim() === '');


    }



    $('#btnAgregarProducto').on('click', function () {
        // Verificar si ya existen los campos de entrada
        if ($('#campos-agregar-producto').length === 0) {
            // Crear el contenedor para los campos de entrada y el botón de guardar
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

        idseleccion = producto.IdProducto;
        $('#inputNoParte').val("");

        $('table tbody tr').each(function () {
            var codigoBarrasFila = $(this).data('idproducto');

            if (codigoBarrasFila === idseleccion) {
                encontrado = true;
                filaEncontrada = $(this);
                var idVentasProducto = $(this).data('id');

                JQueryAjax_Normal(
                    '/Venta/ActualizarEstatus',
                    { idVentasProducto: idVentasProducto },
                    true,
                    function (data) {
                        if (data.exito) {
                            filaEncontrada.removeClass('bg-rojo').addClass('bg-verde');
                        } else {
                            filaEncontrada.removeClass('bg-verde').addClass('bg-rojo');
                        }
                        $('#BuscarCodigoBarras').val('');
                    },
                    function (error) {
                        console.error("Error al actualizar el estatus:", error);
                    }
                );

                return false;
            }
            $('#resultadosBusqueda').empty();
        });

    }



})




$('#checkFiltros').change(function () {
    var idVenta = $('#ID').data('id');
    factura = $(this).is(':checked')
    JQueryAjax_Normal(
        '/Venta/ActualizarRequiereFactura',
        { idVenta: idVenta, factura: factura },
        true,
        function (response) {
            location.reload(); // Recarga la página
        },
        function (error) {
            alert('Error: ' + error);
        }
    );

    if ($(this).is(':checked')) {
        $('#iconoEstado').removeClass('text-danger').addClass('text-success');
    } else {
        $('#iconoEstado').removeClass('text-success').addClass('text-danger');
    }
});


//////////////
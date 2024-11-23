var carrito = [];

var idseleccion = 0;


$('#BuscarCodigoBarras').focus();

$("#btn-BuscarCodigoBarras").on('click', function () {
    $('#BuscarCodigoBarras').focus();

})

$('#BuscarCodigoBarras').on('change', async function () {
    // Obtener el código de barras ingresado
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
                var idProducto = producto.IdProducto;
                var noParte = producto.NoParte;
                var marca = producto.oMarca.Descripcion;
                var precio = producto.Precio;
                var cantidad = 1; // Cantidad fija para agregar

                var itemEnCarrito = carrito.find(item => item.noParte === noParte);

                if (itemEnCarrito) {
                    itemEnCarrito.cantidad += cantidad;
                } else {
                    carrito.push({ noParte, marca, precio, cantidad, idProducto });
                }

                // Actualizar el carrito después de agregar los productos
                actualizarCarrito();
                $('#BuscarCodigoBarras').val('');
            } else {
                for (const producto of data2.data) {
                    const isConfirm = await new Promise((resolve) => {
                        swal({
                            title: "Agregar producto",
                            text: "¿Quieres agregar el producto " + producto.Descripcion + " de la marca " + producto.oMarca.Descripcion + "?",
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
                        var idProducto = producto.IdProducto;
                        var noParte = producto.NoParte;
                        var marca = producto.oMarca.Descripcion;
                        var precio = producto.Precio;
                        var cantidad = 1; // Cantidad fija para agregar

                        var itemEnCarrito = carrito.find(item => item.noParte === noParte);

                        if (itemEnCarrito) {
                            itemEnCarrito.cantidad += cantidad;
                        } else {
                            carrito.push({ noParte, marca, precio, cantidad, idProducto });
                        }
                        actualizarCarrito();
                    }

                    // Pausa breve antes de mostrar la siguiente alerta
                    await new Promise(resolve => setTimeout(resolve, 200));
                }

                $('#BuscarCodigoBarras').val('');
            }
        }
    }
});


function CargaDescripcion() {
    JQueryAjax_Normal('/Venta/TodosDescripcionProductos', {}, true, function (data) {
        if (data.lista) {
            var cmbDescripcion = $('#cmbDescripcion');
            cmbDescripcion.empty(); // Limpiar opciones anteriores
            data.lista.forEach(function (producto) {
                cmbDescripcion.append($('<option>', {
                    value: producto.Descripcion,
                    text: producto.Descripcion
                }));
            });
        }
    }, function () {
        swal("Error", "Hubo un problema al obtener los detalles del producto.", "error");
    });
}

CargaDescripcion();
$("#guardar").click(function () {
    var descripcion = $("#cmbDescripcion").val().toUpperCase();
    var añocompleto = $("#cmbyear option:selected").text().toUpperCase();
    var añoavalue = $("#cmbyear").val().toUpperCase();
    var marcacompleto = $("#cmbMarcas option:selected").text().toUpperCase();
    var marcaavalue = $("#cmbMarcas").val().toUpperCase();
    var modelo = $("#txtmodelo").val().toUpperCase();
    var motor = $("#txtmotor").val().toUpperCase();

    JQueryAjax_Normal('/Venta/BuscadorVenta', {
        descripcion: descripcion,
        anioCompleto: añocompleto,
        anioAbreviado: añoavalue,
        marcaCompleto: marcacompleto,
        marcaAbreviado: marcaavalue,
        modelo: modelo
    }, true, function (data) {
        console.log("Productos que contienen el año y el modelo específicos:");
        if (data.productosFiltrados) {
            data.productosFiltrados.forEach(function (item) {
                console.log(item.NoParte + ": " + item.Valor);
            });
        }

        console.log("--------");

        console.log("Otros productos:");
        if (data.otrosProductos) {
            data.otrosProductos.forEach(function (item) {
                console.log(item.NoParte + ": " + item.Valor);
            });
        }
        var resultadosHtml = "";
        if (data.productosFiltrados) {
            console.log(data.productosFiltrados)
            debugger;
            data.productosFiltrados.forEach(function (item) {
                resultadosHtml += "<h1>" + item.NoParte + "</h1>";
                var valor = item.Valor;
                // Subrayar y cambiar color del texto correspondiente a motor
                valor = valor.replace(new RegExp("\\b" + modelo + "\\b", 'gi'), '<span class="badge bg-success text-dark">$&</span>');

                // Subrayar y cambiar color del texto correspondiente a año completo
                valor = valor.replace(new RegExp("\\b" + añocompleto + "\\b", 'gi'), '<span class="badge bg-warning text-dark">$&</span>');

                // Reemplazar | por <br>
                valor = valor.replace(/\|/g, '<br><br>');

                resultadosHtml += valor + "<br>";
            });
        }
        $("#resultados").html(resultadosHtml);

    }, function () {
        swal("Error", "Hubo un problema al obtener los detalles del producto.", "error");
    });
});
//NUEVO


$("#btn-Buscar").click(function () {
    if ($("#txtBuscar").val() == "") {
        $("#txtBuscar").focus();
    } else {
        alert("Logica de buscar")
    }
})



//______________________________________

//$(document).ready(function () {
//    $('#btn-Buscar').on('click', function () {
//        const query = $('#txtBuscar').val();
//        if (query) {
//            search(query);
//        }
//    });

//    $('#checkFiltros').on('change', function () {
//        if ($(this).is(':checked')) {
//            $('#iconoEstado').removeClass('text-danger').addClass('text-success');
//        } else {
//            $('#iconoEstado').removeClass('text-success').addClass('text-danger');
//        }
//    });

//    function search(query) {
//        // Simula una búsqueda con resultados superficiales
//        const results = [
//            { id: 1, name: "Resultado 1", description: "Descripción corta del resultado 1" },
//            { id: 2, name: "Resultado 2", description: "Descripción corta del resultado 2" }
//        ];

//        $('#results').empty();
//        results.forEach(result => {
//            const resultItem = `
//                <a href="#" class="list-group-item list-group-item-action" data-id="${result.id}">
//                    <h5 class="mb-1">${result.name}</h5>
//                    <p class="mb-1">${result.description}</p>
//                    <small><button class="btn btn-link btn-sm show-detail">Ver más</button></small>
//                </a>
//            `;
//            $('#results').append(resultItem);
//        });

//        $('.show-detail').on('click', function (e) {
//            e.preventDefault();
//            const id = $(this).closest('a').data('id');
//            showDetail(id);
//        });
//    }

//    function showDetail(id) {
//        // Simula la obtención de detalles de un resultado
//        const details = {
//            1: "Detalles completos del resultado 1...",
//            2: "Detalles completos del resultado 2..."
//        };

//        $('#detailContent').text(details[id]);
//        $('#detailModal').modal('show');
//    }
//});



$(document).ready(function () {


    function buscarProductos() {
        var busqueda = $('#txtBuscar').val().toUpperCase();
        if (busqueda.trim() !== "") {
            JQueryAjax_Normal('/Venta/BuscadorVenta2', { busqueda: busqueda }, true, function (data) {
                $('#resultados').empty();
                if (data.lista.length > 0) {
                    data.lista.forEach(function (producto) {
                        $('#resultados').append(`
                        <div class="producto-item row text-center my-3" id="producto-${producto.IdProducto}">
                            <div class="col-12 d-flex align-items-center justify-content-between mb-2">
                                <span class="d-flex">
                                    <button class="btn btn-link btn-detalle me-2" data-idproducto="${producto.IdProducto}">
                                        <i class="fas fa-caret-square-down fa-2x"></i>
                                    </button>
                                    <button class="btn btn-link btn-imagenes" data-idproducto="${producto.IdProducto}">
                                        <i class="fas fa-camera fa-2x"></i>
                                    </button>
                                </span>

                                <span class="d-flex align-items-center">
                                    <span class="badge bg-danger text-white rounded-circle p-2 me-2" style="font-size: 1rem;">
                                        ${producto.Maximo}
                                    </span>
                                    <button class="btn btn-link me-2 btm-carrito" data-idproducto="${producto.IdProducto}" data-noparte="${producto.NoParte}" data-marca="${producto.oMarca.Descripcion}" data-precio="${producto.Precio}">
                                        <i class="fas fa-cart-plus fa-2x"></i>
                                    </button>
                                </span>
                            </div>
                            <div class="col-3 d-flex align-items-center justify-content-center">
                                <span class="h4">${producto.Descripcion}</span>
                            </div>
                            <div class="col-3 d-flex align-items-center justify-content-center">
                                <p class="h4"><strong>No. Parte:</strong> ${producto.NoParte}</p>
                            </div>
                            <div class="col-auto d-flex align-items-center justify-content-center">
                                <span class="badge bg-primary py-2 h5">${producto.oMarca.Descripcion}</span>
                            </div>
                            <div class="col-3 d-flex align-items-center justify-content-start">
                                <span class="badge bg-success py-2 h3">${producto.Precio} $$</span>
                            </div>
                        </div>
                        <div id="detalles-${producto.IdProducto}" class="detalles-producto"></div>
                        <hr />
                    `);
                    });
                } else {
                    $('#resultados').html('<p>No se encontraron productos.</p>');
                }
            }, function (error) {
                console.log("Error : ");
                console.log(error);
            });
        }
    }





    $('#btn-Buscar').on('click', buscarProductos);

    $('#txtBuscar').on('keypress', function (e) {
        if (e.which === 13) {
            var busqueda = $(this).val().toUpperCase();
            if (busqueda.trim() !== "") {
                buscarProductos();
            }
        }
    });

    // Usa delegación de eventos para manejar los clics en los botones generados dinámicamente
    $(document).on('click', '.btn-imagenes', function () {
        var idProducto = $(this).data('idproducto');
        desplegarIMG(idProducto);
    });

    $(document).on('click', '.btn-detalle', function () {

        var $detallesDiv = $(this).closest('.producto-item').next('.detalles-producto');
        var IdProducto = $(this).data('idproducto');

        // Alternar la visibilidad del contenedor de detalles
        $detallesDiv.toggle();

        // Si el contenedor de detalles está visible y vacío, cargar los datos
        if ($detallesDiv.is(':visible') && $detallesDiv.is(':empty')) {
            // Realizar la llamada AJAX para obtener las imágenes
            $.ajax({
                type: "POST",
                url: '/Producto/ListarImagenes',
                data: { Id: IdProducto },
                dataType: "json",
                success: function (data) {

                    console.log(data)
                    var carruselHtml;
                    if (data.imagenes.length > 0) {
                        var carruselId = `carrusel-${IdProducto}`;
                        var carruselIndicators = data.imagenes.map((img, index) => `
                        <button type="button" data-bs-target="#${carruselId}" data-bs-slide-to="${index}" ${index === 0 ? 'class="active" aria-current="true"' : ''}></button>
                    `).join('');
                        var carruselItems = data.imagenes.map((img, index) => `
                        <div class="carousel-item ${index === 0 ? 'active' : ''}">
                            <img src="data:image/png;base64,${img}" class="d-block w-100" alt="Imagen ${index + 1}">
                        </div>
                    `).join('');

                        carruselHtml = `
                        <div id="${carruselId}" class="carousel slide" data-bs-ride="carousel">
                            <div class="carousel-indicators">
                                ${carruselIndicators}
                            </div>
                            <div class="carousel-inner">
                                ${carruselItems}
                            </div>
                            <button class="carousel-control-prev" type="button" data-bs-target="#${carruselId}" data-bs-slide="prev">
                                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                                <span class="visually-hidden">Previous</span>
                            </button>
                            <button class="carousel-control-next" type="button" data-bs-target="#${carruselId}" data-bs-slide="next">
                                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                                <span class="visually-hidden">Next</span>
                            </button>
                            <div class="text-center mt-2">
                                <span id="${carruselId}-counter">1 de ${data.imagenes.length} imágenes</span>
                            </div>
                        </div>
                    `;

                        $detallesDiv.html(`
                        <div class="row mt-3">
                            <div class="col-6">
                                ${carruselHtml}
                            </div>
                            <div class="col-6">
                                <!-- Aquí se cargarán los detalles del producto -->
                            </div>
                        </div>
                    `);

                        $('#' + carruselId).on('slid.bs.carousel', function (e) {
                            var currentIndex = $(e.relatedTarget).index() + 1;
                            $('#' + carruselId + '-counter').text(currentIndex + ' de ' + data.imagenes.length + ' imágenes');
                        });
                    } else {
                        $detallesDiv.html(`
                                            <div class="row mt-3">
                                                <div class="col-6">
                                                    <div class="d-flex align-items-center justify-content-center bg-secondary text-white" style="height: 200px;">
                                                        No hay imágenes registradas.
                                                    </div>
                                                </div>
                                                <div class="col-6">
                                                    <!-- Aquí se cargarán los detalles del producto -->
                                                </div>
                                            </div>
                                        `);
                    }







                },
                error: function (error) {
                    console.log(error);
                }
            });                    // Realizar la llamada AJAX para obtener más detalles del producto

            JQueryAjax_Normal('/Venta/detalleProducto', { IdProducto: IdProducto }, true, function (data) {
                console.log(data)
                var detallesHtml = `
        <p class="h4"><strong>Aplicacion:</strong> ${data.lista.Valor}</p>
    `;
                $detallesDiv.find('.col-6:last').html(detallesHtml);
            });
        }
    });





    function desplegarIMG(IdProducto) {
        $("#miModal").modal("show");

        $.ajax({
            type: "POST",
            url: '/Producto/ListarImagenes',
            data: { Id: IdProducto },
            dataType: "json",
            success: function (data) {
                mostrarImagenes2(data.imagenes, data.id, "#contenedor-imagenes");
            },
            error: function (error) {
                console.log(error);
            },
            dataFilter: function (data, dataType) {
                return data; // No es necesario dividir la respuesta
            }
        });
    }

    function mostrarImagenes2(imagenes, ids, contenedor) {
        $(contenedor).empty();
        // Recorrer la lista de imágenes y agregarlas a divs redondeados

        $.each(imagenes, function (index, imagen) {
            var id = ids[index]; // Obtener el ID correspondiente al índice actual

            // Crear un div de columna para cada imagen
            var colElement = $('<div class="col-4 mb-3">');

            // Crear un div redondeado de 200px con la imagen dentro
            var imgElement = $('<div class="rounded d-flex justify-content-center align-items-center image-container ">')
                .css('width', '150px')
                .css('height', '100px')
                .attr('data-idimagen', id)
                .append($('<img class="d-block rounded ">')
                    .attr('src', 'data:image/png;base64,' + imagen)
                    .css('object-fit', 'cover')
                    .css('border-radius', '5px')
                    .css('width', '100%')
                    .css('height', '100%')
                );

            if (contenedor != "#imageContainerExistente") {
                imgElement.on('click', function () {
                    $('.border-img').removeClass('border-img');

                    $('#mostrarGrande').empty();
                    idImagenBorrar = $(this).data('idimagen');
                    mostrarImagenEnGrande('data:image/png;base64,' + imagen);

                    $(this).addClass('border-img');
                });
            }

            colElement.append(imgElement);

            $(contenedor).append(colElement);
        });
    }


    // Función para mostrar la imagen en grande
    function mostrarImagenEnGrande(imagenSrc) {
        // Limpiar el contenedor antes de agregar la imagen en grande
        $('#mostrarGrande').empty();


        // Crear un div con la imagen en grande
        var imgGrande = $('<img class="img-fluid zoom-img">').attr('src', imagenSrc).css('border-radius', '5px');

        // Agregar la imagen en grande al contenedor
        $('#mostrarGrande').append(imgGrande);

    }



});



function actualizarCarrito() {
    var total = 0;
    var carritoHtml = '';

    carrito.forEach(function (item, index) {
        var subtotal = item.precio * item.cantidad;
        carritoHtml += `
                <div class="row carrito-item my-2">
                    <div class="col-2">${item.noParte}</div>
                    <div class="col-2">${item.marca}</div>
                    <div class="col-2">
                        <input type="number" class="cantidad form-control" data-index="${index}" value="${item.cantidad}" min="1" />
                    </div>
                    <div class="col-2">$${item.precio.toFixed(2)}</div>
                    <div class="col-1">$${subtotal.toFixed(2)}</div>
                    <div class="col-1 text-end m-auto">
                        <button class="btn btn-danger btn-eliminar" data-index="${index}">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </div>
                </div>
            `;
        total += subtotal;
    });

    $('#carrito-items').html(carritoHtml);
    $('#total-precio').text(`$${total.toFixed(2)}`);
}

$(document).on('click', '.btm-carrito', function () {
    var idProducto = $(this).data('idproducto'); // Convertir idProducto a cadena
    var noParte = $(this).data('noparte');
    var marca = $(this).data('marca');
    var precio = parseFloat($(this).data('precio'));
    var cantidad = 1; // Aquí estamos añadiendo una cantidad fija para el clic en el botón

    var itemEnCarrito = carrito.find(item => item.noParte === noParte);

    if (itemEnCarrito) {
        itemEnCarrito.cantidad += cantidad;
    } else {
        carrito.push({ noParte, marca, precio, cantidad, idProducto });
    }

    actualizarCarrito();
});

$('#carrito-items').on('input', '.cantidad', function () {
    var index = $(this).data('index');
    var cantidad = parseInt($(this).val());

    if (cantidad > 0) {
        carrito[index].cantidad = cantidad;
        actualizarCarrito();
    }
});

$('#carrito-items').on('click', '.btn-eliminar', function () {
    var index = $(this).data('index');
    carrito.splice(index, 1);
    actualizarCarrito();
});

$('#btnAgregarProducto').on('click', function () {
    // Verificar si ya existen los campos de entrada
    if ($('#campos-agregar-producto').length === 0) {
        // Crear el contenedor para los campos de entrada y el botón de guardar
        var camposHtml = `
            <div id="campos-agregar-producto" class="row my-3">
                <div class="col-2" id="NoParteDiv">
                    <input type="text" id="inputNoParte" class="form-control" placeholder="No. Parte" required autocomplete="off" name="no_parte" />
                    <div id="NoParteBuscador" class="mt-2"></div>
                </div>
                <div class="col-2">
                    <input type="text" id="inputMarca" class="form-control" placeholder="Marca" disabled />
                </div>
                <div class="col-2">
                    <input type="number" id="inputCantidad" class="form-control" placeholder="Cantidad" min="1" disabled />
                </div>
                <div class="col-2">
                    <input type="number" id="inputPrecio" class="form-control" placeholder="Precio" step="0.01" disabled />
                </div>
                <input type="number" id="inputIdProducto" hidden />

            </div>
        `;
        $('#carrito-items').append(camposHtml);


        $('#NoParteDiv').append('<div id="resultadosBusqueda" class="mt-2"></div>');

        $("#inputNoParte").focus();
    }
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
                    <div class="row my-2 resultado-item" data-index="${index}" tabindex="0">
                        <div class="col-6">${producto.NoParte}</div>
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

function seleccionarProducto(index, lista) {
    var producto = lista[index];

    idseleccion = producto.IdProducto;
    console.log(idseleccion)
    $('#inputNoParte').val(producto.NoParte);
    $('#inputMarca').val(producto.oMarca.Descripcion).prop('disabled', true);
    $('#inputPrecio').val(producto.Precio).prop('disabled', true);
    $('#inputCantidad').prop('disabled', false);
    $('#inputCantidad').focus();
    $('#resultadosBusqueda').empty();
}
$(document).on('change', '#inputNoParte, #inputMarca, #inputPrecio, #inputCantidad', function () {
    verificarCamposYAgregarProducto();
});

function verificarCamposYAgregarProducto() {
    var noParte = $('#inputNoParte').val();
    var marca = $('#inputMarca').val();
    var precio = $('#inputPrecio').val();
    var cantidad = $('#inputCantidad').val();
    var IdProducto = $('#inputIdProducto').val();
    console.log(IdProducto)
    // Verifica si todos los campos están llenos
    if (noParte !== "" && marca !== "" && precio !== "" && cantidad !== "") {
        agregarProductoAlCarrito(noParte, marca, parseFloat(precio), parseInt(cantidad), parseInt(idseleccion));
    }
}

// Función para agregar producto al carrito
function agregarProductoAlCarrito(noParte, marca, precio, cantidad, idProducto) {
    var itemEnCarrito = carrito.find(item => item.noParte === noParte);

    if (itemEnCarrito) {
        itemEnCarrito.cantidad += cantidad;
    } else {
        carrito.push({ noParte, marca, precio, cantidad, idProducto });
    }
    $("#btnAgregarProducto").focus();
    actualizarCarrito();
}


$('#btnEnviar').on('click', function () {

    if (carrito.length === 0) {
        swal("Error", "El carrito está vacío, no se puede finalizar la venta.", "error");
        return;
    }
    console.log(carrito)
    swal({
        title: "Final de la Venta",
        text: "Nombre del cliente",
        type: "input",
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Enviar",
        closeOnConfirm: false,
        inputPlaceholder: "Nombre del cliente"
    },
        function (inputValue) {
            if (inputValue === false) return false;
            if (inputValue === "") {
                swal.showInputError("Debe ingresar el nombre del cliente!");
                return false;
            }


            var listaVentasProductos = [];
            console.log(carrito)
            debugger;
            carrito.forEach(function (item) {
                var ventasProducto = {
                    oProducto: {
                        IdProducto: parseInt(item.idProducto),
                    },
                    Cantidad: item.cantidad.toString(),
                    Precio: parseFloat(item.precio)
                };

                listaVentasProductos.push(ventasProducto);
            });

            JQueryAjax_Normal('/Venta/GuardarVenta', { listaVentasProductos: listaVentasProductos, nombre: inputValue.toUpperCase(), factura: $("#checkFiltros").prop('checked') }, true, function (data) {
                if (data.bien) {
                    swal("Bien", "La informacion fue enviada a la caja  \n ID: " + data.idVenta, "success");

                } else {
                    swal("Mal", "error: " + data.mensaje, "error");
                }
            }, function () {
                swal("Error", "Hubo un problema al obtener los detalles del producto.", "error");
            });

        });
});



$(document).ready(function () {
    $("#btnObtenerVentas").click(function () {
        $.ajax({
            url: "/Venta/ObtenerVentasAgrupadas",
            type: "POST",
            contentType: "application/json",
            success: function (response) {
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
                        // Asigna valores de las propiedades de los items
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
            },
            error: function (xhr, status, error) {
                $("#resultados").html(`<p>Error: ${xhr.responseJSON.mensaje || 'Ocurrió un error inesperado.'}</p>`);
            }
        });
    });


    $("#resultados").on('click', '.btn-detalle', function () {
        var idVenta = $(this).data('id');
        if (idVenta) {
            // Redirige a la vista Ticket con el ID de la venta
            window.location.href = '/Venta/Ticket?idVenta=' + encodeURIComponent(idVenta);
        }
    });



});


















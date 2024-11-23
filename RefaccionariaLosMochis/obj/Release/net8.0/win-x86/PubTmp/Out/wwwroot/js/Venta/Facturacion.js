let currentFocus = -1;
$("#DivClienteFactura").on('click', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivClienteFactura");

});

$("#DivClienteFactura").on('keydown', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivClienteFactura");

});

$("#DivClienteFactura").on('select', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivClienteFactura");

});

$(document).on('focus', '#DivClienteFactura', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivClienteFactura");

});
//METODO PAGO
$("#DivMetodoPagoFactura").on('click', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivMetodoPagoFactura");

});

$("#DivMetodoPagoFactura").on('keydown', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivMetodoPagoFactura");

});

$("#DivMetodoPagoFactura").on('select', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivMetodoPagoFactura");

});

$(document).on('focus', '#DivMetodoPagoFactura', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivMetodoPagoFactura");

});
//FORMA PAGO
$("#DivFormaPagoFactura").on('click', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivFormaPagoFactura");

});

$("#DivFormaPagoFactura").on('keydown', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivFormaPagoFactura");

});

$("#DivFormaPagoFactura").on('select', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivFormaPagoFactura");

});

$(document).on('focus', '#DivFormaPagoFactura', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivFormaPagoFactura");

});
//USO
$("#DivUso").on('click', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivUso");

});

$("#DivUso").on('keydown', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivUso");

});

$("#DivUso").on('select', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivUso");

});

$(document).on('focus', '#DivUso', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#DivUso");

});

async function handleInputLinea() {
    var NumeroDivisiones = $("#cboDivicion-buscador").val() || 5;
    var pagina = 0;
    desplegarPaginacionBuscadorLinea(pagina, NumeroDivisiones);

    var division = await CountPaginadoBuscadorLinea($('#ClienteFactura').val().toLowerCase(), NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorLinea, "B", NumeroDivisiones);

}

$('#ClienteFactura').on('input', debounce(handleInputLinea, 100));  // 300 m

async function desplegarPaginacionBuscadorLinea(pagina, NumeroDivisiones) {
    var searchTerm = $('#ClienteFactura').val().toLowerCase();
    var opcionesLista = $('#opcionesLista'); // Asegúrate de tener este elemento en el DOM

    JQueryAjax_Normal(
        '/Cliente/BuscarCliente',
        { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones },
        true,
        function (data) {
            var lista = data.Lista;
            opcionesLista.empty();
            currentFocus = -1; // Resetear el foco

            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.RazonSocial);
                    listItem.on('click', function () {
                        selectItem(item);
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        },
        function (error) {
            console.error("Error al obtener los datos:", error);
        }
    );
}

function selectItem(item) {
    $('#ClienteFactura').val(item.RazonSocial);
    $('#ClienteFactura').attr('data-RFCCliente', item.RFC);
    $('#opcionesLista').hide();
    $('#botonesPaginado').hide();

}
$('#ClienteFactura').on('keydown', function (e) {
    var opcionesLista = $('#opcionesLista');
    var opciones = opcionesLista.find('.autocomplete-option');

    if (e.key === "ArrowDown") {
        currentFocus++;
        addActive(opciones);
    } else if (e.key === "ArrowUp") {
        currentFocus--;
        addActive(opciones);
    } else if (e.key === "Enter") {
        e.preventDefault();
        if (currentFocus > -1) {
            opciones.eq(currentFocus).click();
            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();

        }
    }
});
async function CountPaginadoBuscadorLinea(texto, num) {
    var division = 0;
    await JQueryAjax_Normal('/Cliente/CountBuscadorCliente', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}
//$('#ClienteFactura').blur(function () {
//    setTimeout(function () {
//        if ($('#ClienteFactura').attr('data-RFCCliente') != 0) {
//            var searchTerm = $('#ClienteFactura').val().toLowerCase();
//            JQueryAjax_Normal(
//                '/Cliente/BuscarCliente',
//                { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones },
//                true,
//                function (data) {
//                    if (data.RazonSocial != undefined) {
//                        $('#ClienteFactura').val(data.RazonSocial);
//                        $('#ClienteFactura').attr('data-RFCCliente', data.RFC);
//                        $('#opcionesLista').hide();
//                        $('#botonesPaginado').hide();

//                    } else {
//                        swal("No se encontro la Linea", "Utilice el buscador y seleccione una opcion", "error")


//                    }
//                }, function () { }
//            );
//        }
//    }, 9000);
//});
//METODO PAGO
async function handleInputMetodoPago() {
    var NumeroDivisiones = $("#cboDivicion-buscador").val() || 5;
    var pagina = 0;
    desplegarPaginacionMetodoPago(pagina, NumeroDivisiones);

    var division = await CountPaginadoMetodoPago($('#MetodoPagoFactura').val().toLowerCase(), NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorLinea, "B", NumeroDivisiones);

}

$('#MetodoPagoFactura').on('input', debounce(handleInputMetodoPago, 100));  // 300 m

async function desplegarPaginacionMetodoPago(pagina, NumeroDivisiones) {
    var searchTerm = $('#MetodoPagoFactura').val().toLowerCase();
    var opcionesLista = $('#opcionesLista'); // Asegúrate de tener este elemento en el DOM

    JQueryAjax_Normal(
        '/Cliente/BuscadorMetodoPagoDescripcion',
        { descripcion: searchTerm, pagina: pagina, siguientes: NumeroDivisiones },
        true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            currentFocus = -1; // Resetear el foco

            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                    listItem.on('click', function () {
                        selectItemMetodoPago(item);
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        },
        function (error) {
            console.error("Error al obtener los datos:", error);
        }
    );
}

function selectItemMetodoPago(item) {
    $('#MetodoPagoFactura').val(item.Descripcion);
    $('#MetodoPagoFactura').attr('data-ID', item.CFDIMetodoPagoId);
    $('#opcionesLista').hide();
    $('#botonesPaginado').hide();

}
$('#MetodoPagoFactura').on('keydown', function (e) {
    var opcionesLista = $('#opcionesLista');
    var opciones = opcionesLista.find('.autocomplete-option');

    if (e.key === "ArrowDown") {
        currentFocus++;
        addActive(opciones);
    } else if (e.key === "ArrowUp") {
        currentFocus--;
        addActive(opciones);
    } else if (e.key === "Enter") {
        e.preventDefault();
        if (currentFocus > -1) {
            opciones.eq(currentFocus).click();
            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();

        }
    }
});
async function CountPaginadoMetodoPago(texto, num) {
    var division = 0;
    await JQueryAjax_Normal('/Cliente/ContarMetodosPago', { condicion: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

//FORMA PAGO
async function handleInputFormaPago() {
    var NumeroDivisiones = $("#cboDivicion-buscador").val() || 5;
    var pagina = 0;
    desplegarPaginacionFormaPago(pagina, NumeroDivisiones);

    var division = await CountPaginadoFormaPago($('#FormaPagoFactura').val().toLowerCase(), NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorLinea, "B", NumeroDivisiones);

}

$('#FormaPagoFactura').on('input', debounce(handleInputFormaPago, 100));  // 300 m

async function desplegarPaginacionFormaPago(pagina, NumeroDivisiones) {
    var searchTerm = $('#FormaPagoFactura').val().toLowerCase();
    var opcionesLista = $('#opcionesLista'); // Asegúrate de tener este elemento en el DOM

    JQueryAjax_Normal(
        '/Cliente/BuscarFormasPagoDescripcion',
        { descripcion: searchTerm, pagina: pagina, cantidadPorPagina: NumeroDivisiones },
        true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            currentFocus = -1; // Resetear el foco

            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                    listItem.on('click', function () {
                        selectItemFormaPago(item);
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        },
        function (error) {
            console.error("Error al obtener los datos:", error);
        }
    );
}

function selectItemFormaPago(item) {
    $('#FormaPagoFactura').val(item.Descripcion);
    $('#FormaPagoFactura').attr('data-ID', item.CFDIFormaPagoId);
    $('#opcionesLista').hide();
    $('#botonesPaginado').hide();

}
$('#FormaPagoFactura').on('keydown', function (e) {
    var opcionesLista = $('#opcionesLista');
    var opciones = opcionesLista.find('.autocomplete-option');

    if (e.key === "ArrowDown") {
        currentFocus++;
        addActive(opciones);
    } else if (e.key === "ArrowUp") {
        currentFocus--;
        addActive(opciones);
    } else if (e.key === "Enter") {
        e.preventDefault();
        if (currentFocus > -1) {
            opciones.eq(currentFocus).click();
            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();

        }
    }
});
async function CountPaginadoFormaPago(texto, num) {
    var division = 0;
    await JQueryAjax_Normal('/Cliente/ContarFormasPago', { where: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

//USO
async function handleInputUsoFactura() {
    var NumeroDivisiones = $("#cboDivicion-buscador").val() || 5;
    var pagina = 0;
    desplegarPaginacionUsoFactura(pagina, NumeroDivisiones);

    var division = await CountPaginadoUsoFactura($('#UsoFactura').val().toLowerCase(), NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionUsoFactura, "B", NumeroDivisiones);

}

$('#UsoFactura').on('input', debounce(handleInputUsoFactura, 200)); 

async function desplegarPaginacionUsoFactura(pagina, NumeroDivisiones) {
    var searchTerm = $('#UsoFactura').val().toLowerCase();
    var opcionesLista = $('#opcionesLista'); // Asegúrate de tener este elemento en el DOM

    JQueryAjax_Normal(
        '/Cliente/BuscadorCFDIUsoDescripcionID',
        { descripcion: searchTerm, pagina: pagina, siguientes: NumeroDivisiones },
        true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            currentFocus = -1; // Resetear el foco

            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                    listItem.on('click', function () {
                        selectItemUsoFactura(item);
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        },
        function (error) {
            console.error("Error al obtener los datos:", error);
        }
    );
}

function selectItemUsoFactura(item) {
    $('#UsoFactura').val(item.Descripcion);
    $('#UsoFactura').attr('data-ID', item.CFDIUsoId);
    $('#opcionesLista').hide();
    $('#botonesPaginado').hide();

}
$('#UsoFactura').on('keydown', function (e) {
    var opcionesLista = $('#opcionesLista');
    var opciones = opcionesLista.find('.autocomplete-option');

    if (e.key === "ArrowDown") {
        currentFocus++;
        addActive(opciones);
    } else if (e.key === "ArrowUp") {
        currentFocus--;
        addActive(opciones);
    } else if (e.key === "Enter") {
        e.preventDefault();
        if (currentFocus > -1) {
            opciones.eq(currentFocus).click();
            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();

        }
    }
});
async function CountPaginadoUsoFactura(texto, num) {
    var division = 0;
    await JQueryAjax_Normal('/Cliente/countCFDIUsoWhere', { where: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}










//METODOS COMPARTIDOS


function addActive(opciones) {
    if (!opciones) return false;
    removeActive(opciones);
    if (currentFocus >= opciones.length) currentFocus = 0;
    if (currentFocus < 0) currentFocus = opciones.length - 1;
    opciones.eq(currentFocus).addClass("autocomplete-active");
}

function removeActive(opciones) {
    opciones.removeClass("autocomplete-active");
}
function debounce(func, wait) {
    let timeout;
    return function () {
        const context = this, args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(context, args), wait);
    };
}





$(document).on('click', function (event) {

    if (!$(event.target).closest('#buscador').length) {
        $('#botonesPaginado').hide();
        $('#opcionesLista').hide();
    }
});
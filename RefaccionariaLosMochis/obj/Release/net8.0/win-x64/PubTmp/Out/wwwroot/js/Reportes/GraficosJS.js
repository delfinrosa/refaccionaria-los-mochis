var myAreaChart = null;
var myBarChart = null;
var myHourlyChart = null; // Nueva variable para la gráfica de horas
var myPriceEvolutionChart = null; // Nueva variable para la gráfica de evolución del precio
var myBarChartTopProductos = null;

var anioActual = new Date().getFullYear();
$("#anioSelect").val(anioActual);

var mesActual = new Date().getMonth() + 1;
$("#mesSelect").val(mesActual);

// Configura la fecha actual en el selector de fecha
var fechaActual = new Date().toISOString().split('T')[0];
$("#dateSelect").val(fechaActual);
$("#dateSelectGananciaPorHora").val(fechaActual);

function ObtenerGananciaDelMes() {
    var mes = $("#mesSelect").val();
    var anio = $("#anioSelect").val();

    JQueryAjax_Normal('/Reportes/ObtenerVentasPorDias', { mes: mes, anio: anio }, false, function (data) {

        if (myAreaChart) {
            myAreaChart.destroy();
        }
        if (data.data && data.data.length > 0) {
            var labels = data.data.map(item => `Día ${item.dia}`);
            var valores = data.data.map(item => item.totalVentas);

            var ctx = document.getElementById('myAreaChart').getContext('2d');

            myAreaChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Ganancia del Mes',
                        data: valores,
                        borderColor: 'rgba(75, 192, 192, 1)',
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        borderWidth: 2,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Día'
                            }
                        },
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Ganancia'
                            }
                        }
                    }
                }
            });
        } else {
            swal("No se encontraron datos", "No se encontraron datos para el mes seleccionado.", "info");
        }
    }, function (error) {
        swal("Error al obtener datos", error, "error");
    });
}

function ObtenerGananciaDelAño() {
    var anio = $("#anioSelect").val();

    JQueryAjax_Normal('/Reportes/ObtenerVentasPorMes', { anio: anio }, false, function (data) {
        if (myBarChart) {
            myBarChart.destroy();
        }
        if (data.data && data.data.length > 0) {
            var meses = data.data.map(item => `Mes ${item.mes}`);
            var totalProductosVendidos = data.data.map(item => item.totalProductosVendidos);
            var totalVentas = data.data.map(item => item.totalVentas);

            var ctx = document.getElementById('myBarChart').getContext('2d');

            myBarChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: meses,
                    datasets: [
                        {
                            label: 'Total Ventas',
                            data: totalVentas,
                            backgroundColor: 'rgba(153, 102, 255, 0.2)',
                            borderColor: 'rgba(153, 102, 255, 1)',
                            borderWidth: 1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Mes'
                            }
                        },
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Valor'
                            }
                        }
                    }
                }
            });
        } else {
            swal("No se encontraron datos", "No se encontraron datos para el año seleccionado.", "info");
        }
    }, function (error) {
        swal("Error al obtener datos", error, "error");
    });
}

function ObtenerGananciaPorHora() {
    var fecha = $("#dateSelectGananciaPorHora").val().toString();

    JQueryAjax_Normal('/Reportes/ObtenerVentasPorHora', { fecha: fecha }, false, function (response) {

        if (myHourlyChart) {
            myHourlyChart.destroy();
        }

        // Verificamos si la respuesta tiene datos
        if (response.data && response.data.length > 0) {
            // Extraemos las horas y las ventas de la respuesta
            var labels = response.data.map(item => `Hora ${item.hora}`);
            var valores = response.data.map(item => item.totalVentas);

            var ctx = document.getElementById('myHourlyChart').getContext('2d');

            // Creamos el gráfico con los datos obtenidos
            myHourlyChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Ganancia por Hora',
                        data: valores,
                        borderColor: 'rgba(255, 159, 64, 1)',
                        backgroundColor: 'rgba(255, 159, 64, 0.2)',
                        borderWidth: 2,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Hora'
                            }
                        },
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Ganancia'
                            }
                        }
                    }
                }
            });
        } else {
            swal("No se encontraron datos", "No se encontraron datos para el día seleccionado.", "info");
        }
    }, function (error) {
        swal("Error al obtener datos", error, "error");
    });
}

function ObtenerEvolucionDelPrecio(idProducto) {

    JQueryAjax_Normal('/Reportes/ObtenerEvolucionPrecios', { idProducto: idProducto }, false, function (response) {

        if (myPriceEvolutionChart) {
            myPriceEvolutionChart.destroy();
        }

        // Verificamos si la respuesta tiene datos
        if (response.data && response.data.length > 0) {
            // Extraemos las fechas y los precios de la respuesta
            var labels = response.data.map(item => item.fechaCompra);
            var precios = response.data.map(item => item.precioCompra);

            var ctx = document.getElementById('myPriceEvolutionChart').getContext('2d');

            // Creamos el gráfico con los datos obtenidos
            myPriceEvolutionChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Evolución del Precio',
                        data: precios,
                        borderColor: 'rgba(54, 162, 235, 1)',
                        backgroundColor: 'rgba(54, 162, 235, 0.2)',
                        borderWidth: 2,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Fecha'
                            }
                        },
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Precio'
                            }
                        }
                    }
                }
            });
        } else {
        //    swal("No se encontraron datos", "No se encontraron datos para el producto seleccionado.", "info");
        }
    }, function (error) {
        swal("Error al obtener datos", error, "error");
    });
}

// Configurar los eventos change para los selects
$(document).ready(function () {
    // Llamar a las funciones al cargar la página

    ObtenerGananciaDelMes();
    ObtenerGananciaDelAño();
    ObtenerGananciaPorHora(); // Llama la función para la gráfica de horas

    // Evento change para el select del mes
    $("#mesSelect").change(function () {
        ObtenerGananciaDelMes();
    });

    // Evento change para el select del año
    $("#anioSelect").change(function () {
        ObtenerGananciaDelAño();
    });

    // Evento change para el input de fecha
    $("#dateSelectGananciaPorHora").change(function () {
        ObtenerGananciaPorHora();
    });

    
});


$('#productIdSelect').on('input',  function () {
    var valorNoParte = $(this).val();
    var currentRow = $(this).closest('tr');

    if (valorNoParte.length > 2) {        // Realiza la búsqueda de productos
        BusquedaProductoVenta(valorNoParte, currentRow);
    } else {
        // Si el valor es menor o igual a 3 caracteres, elimina el div si existe
        $('#resultadosBusqueda').children().not('#productIdSelect').remove();
    }
});
function BusquedaProductoVenta(valorNoParte, currentRow) {
    JQueryAjax_Normal('/Venta/BuscarProductos', { NoParte: valorNoParte }, true, function (data) {
        var resultContainer = currentRow.find('#resultadosBusqueda');
        resultContainer.empty();
        if (data.lista.length > 0) {
            data.lista.forEach(function (producto, index) {
                var resultadoHtml = `
                  <div class="row my-2 resultado-item" data-index="${index}" tabindex="0"  style="position: relative; z-index: 20;  width: 160px;">
                        <div class="col-8">${producto.noParte}</div>
                        <div class="col-6"><span class="badge bg-primary">${producto.oMarca.descripcion}</span></div>
                    </div>
                `;
                $('#resultadosBusqueda').append(resultadoHtml);
            });

            // Añadir eventos de selección y navegación por teclado
            $('#resultadosBusqueda').on('click', '.resultado-item', function () {
                seleccionarProducto($(this).data('index'), data.lista, currentRow);

            });

            // Eventos de teclado en los elementos de resultado
            $('#resultadosBusqueda').on('keydown', '.resultado-item', function (e) {
                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    $(this).next('.resultado-item').focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    $(this).prev('.resultado-item').focus();
                } else if (e.key === 'Enter') {
                    e.preventDefault();
                    seleccionarProducto($(this).data('index'), data.lista, currentRow);
                }
            });

            // Coloca el foco en el primer elemento de la lista de resultados al presionar Enter en el campo de entrada
            $('#productIdSelect').on('keydown', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    $('#resultadosBusqueda .resultado-item').first().focus();
                }
                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    $('#resultadosBusqueda .resultado-item').first().focus();
                }
            });
            //$('#resultadosBusqueda').css('top', '%');

        } else {
            $('#resultadosBusqueda').html('<p>No se encontraron productos.</p>');
        }

    });
}

function seleccionarProducto(index, lista, currentRow) {

    //$('#resultadosBusqueda').css('top', '');
    var producto = lista[index];
    ObtenerEvolucionDelPrecio(producto.idProducto); 
    $('#productIdSelect').val(producto.noParte)
    $('#resultadosBusqueda').children().not('#productIdSelect').remove();

}

function ObtenerTopProductos() {
    const caso = $("#SelectCase").val();
    const anio = $("#anioSelectProductosvendidos").val();
    const mes = $("#mesSelectProductosvendidos").val();
    const fecha = $("#dateSelectProductosvendidos").val();

    JQueryAjax_Normal(
        '/Reportes/ObtenerTopProductos',
        { caso: caso, anio: anio || "", mes: mes || "", fecha: fecha || "" },
        false,
        function (data) {
            // Destruir gráfico existente si existe
            if (myBarChartTopProductos ) {
                myBarChartTopProductos.destroy();
            }
            if (data.data && data.data.length > 0) {

                const etiquetas = data.data.map(item => item.noParte); // Usar "noParte" tal como está en el JSON
                const valores = data.data.map(item => item.totalProductosVendidos); // Usar "totalProductosVendidos" tal como está en el JSON

                const ctx = document.getElementById('myBarChartTopProductos').getContext('2d');

                // Crear gráfico de barras
                myBarChartTopProductos = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: etiquetas,
                        datasets: [
                            {
                                label: 'Total Productos Vendidos',
                                data: valores,
                                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                                borderColor: 'rgba(75, 192, 192, 1)',
                                borderWidth: 1
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                display: true,
                                position: 'top'
                            },
                            tooltip: {
                                callbacks: {
                                    label: function (tooltipItem) {
                                        return `Total Vendido: ${tooltipItem.raw}`;
                                    }
                                }
                            }
                        },
                        scales: {
                            x: {
                                title: {
                                    display: true,
                                    text: 'No. Parte'
                                }
                            },
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Cantidad Vendida'
                                }
                            }
                        }
                    }
                });
            } else {
            //    swal("No se encontraron datos", "No se encontraron datos para los criterios seleccionados.", "info");
            }
        },
        function (error) {
            swal("Error al obtener datos", error, "error");
        }
    );
}


$("#SelectCase").change(function () {
    const selectedValue = $(this).val();

    // Ocultar todos los campos al inicio
    $("#anioSelectProductosvendidos").addClass("d-none");
    $("#mesSelectProductosvendidos").addClass("d-none");
    $("#dateSelectProductosvendidos").addClass("d-none");

    // Mostrar los campos según la selección
    if (selectedValue === "2") {
        $("#dateSelectProductosvendidos").removeClass("d-none");
        var fechaActual = new Date().toISOString().split('T')[0];
        $("#dateSelectProductosvendidos").val(fechaActual);
    } else if (selectedValue === "3") {
        $("#anioSelectProductosvendidos").removeClass("d-none");
        $("#mesSelectProductosvendidos").removeClass("d-none");
    } else if (selectedValue === "4") {
        $("#anioSelectProductosvendidos").removeClass("d-none");
    }
    ObtenerTopProductos();

});

$("#anioSelectProductosComprados, #mesSelectProductosComprados, #dateSelectProductosComprados").change(function () {
    ObtenerTopProductosComprado();
});
function ObtenerTopProductosComprado() {
    const caso = $("#SelectCaseComprados").val();
    const anio = $("#anioSelectProductosComprados").val();
    const mes = $("#mesSelectProductosComprados").val();
    const fecha = $("#dateSelectProductosComprados").val();

    JQueryAjax_Normal(
        '/Reportes/ObtenerTopProductosCompra',
        { caso: caso, anio: anio || "", mes: mes || "", fecha: fecha || "" },
        false,
        function (data) {
            console.log(data)
            // Destruir gráfico existente si existe
            if (myBarChartTopProductos ) {
                myBarChartTopProductos.destroy();
            }
            if (data.data && data.data.length > 0) {

                const etiquetas = data.data.map(item => item.noParte); // Usar "noParte" tal como está en el JSON
                const valores = data.data.map(item => item.totalProductosComprados); // Usar "totalProductosVendidos" tal como está en el JSON

                const ctx = document.getElementById('myBarChartTopProductosComprados').getContext('2d');

                // Crear gráfico de barras
                myBarChartTopProductos = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: etiquetas,
                        datasets: [
                            {
                                label: 'Total Productos Comprados',
                                data: valores,
                                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                                borderColor: 'rgba(75, 192, 192, 1)',
                                borderWidth: 1
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                display: true,
                                position: 'top'
                            },
                            tooltip: {
                                callbacks: {
                                    label: function (tooltipItem) {
                                        return `Total Comprado: ${tooltipItem.raw}`;
                                    }
                                }
                            }
                        },
                        scales: {
                            x: {
                                title: {
                                    display: true,
                                    text: 'No. Parte'
                                }
                            },
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Cantidad Comprada'
                                }
                            }
                        }
                    }
                });
            } else {
            //    swal("No se encontraron datos", "No se encontraron datos para los criterios seleccionados.", "info");
            }
        },
        function (error) {
            swal("Error al obtener datos", error, "error");
        }
    );
}


$("#SelectCaseComprados").change(function () {
    const selectedValue = $(this).val();

    // Ocultar todos los campos al inicio
    $("#anioSelectProductosComprados").addClass("d-none");
    $("#mesSelectProductosComprados").addClass("d-none");
    $("#dateSelectProductosComprados").addClass("d-none");

    // Mostrar los campos según la selección
    if (selectedValue === "2") {
        $("#dateSelectProductosComprados").removeClass("d-none");
        var fechaActual = new Date().toISOString().split('T')[0];
        $("#dateSelectProductosComprados").val(fechaActual);
    } else if (selectedValue === "3") {
        $("#anioSelectProductosComprados").removeClass("d-none");
        $("#mesSelectProductosComprados").removeClass("d-none");
    } else if (selectedValue === "4") {
        $("#anioSelectProductosComprados").removeClass("d-none");
    }
    ObtenerTopProductosComprado();

});
$("#anioSelectProductosvendidos, #mesSelectProductosvendidos, #dateSelectProductosvendidos").change(function () {
    ObtenerTopProductos();
});
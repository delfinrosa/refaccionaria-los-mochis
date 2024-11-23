var myAreaChart = null;
var myBarChart = null;
var myHourlyChart = null; // Nueva variable para la gráfica de horas
var myPriceEvolutionChart = null; // Nueva variable para la gráfica de evolución del precio

var anioActual = new Date().getFullYear();
$("#anioSelect").val(anioActual);

var mesActual = new Date().getMonth() + 1;
$("#mesSelect").val(mesActual);

// Configura la fecha actual en el selector de fecha
var fechaActual = new Date().toISOString().split('T')[0];
$("#dateSelect").val(fechaActual);

function ObtenerGananciaDelMes() {
    var mes = $("#mesSelect").val();
    var anio = $("#anioSelect").val();

    JQueryAjax_Normal('/Reportes/ObtenerVentasPorDias', { mes: mes, anio: anio }, false, function (data) {
        console.log(data);

        if (myAreaChart) {
            myAreaChart.destroy();
        }
        if (data.data && data.data.length > 0) {
            var labels = data.data.map(item => `Día ${item.Dia}`);
            var valores = data.data.map(item => item.TotalVentas);

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
        console.log(data);
        if (myBarChart) {
            myBarChart.destroy();
        }
        if (data.data && data.data.length > 0) {
            var meses = data.data.map(item => `Mes ${item.Mes}`);
            var totalProductosVendidos = data.data.map(item => item.TotalProductosVendidos);
            var totalVentas = data.data.map(item => item.TotalVentas);

            var ctx = document.getElementById('myBarChart').getContext('2d');

            myBarChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: meses,
                    datasets: [
                        {
                            label: 'Total Productos Vendidos',
                            data: totalProductosVendidos,
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            borderColor: 'rgba(75, 192, 192, 1)',
                            borderWidth: 1
                        },
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
    var fecha = $("#dateSelect").val().toString();

    JQueryAjax_Normal('/Reportes/ObtenerVentasPorHora', { fecha: fecha }, false, function (response) {

        if (myHourlyChart) {
            myHourlyChart.destroy();
        }

        // Verificamos si la respuesta tiene datos
        if (response.data && response.data.length > 0) {
            // Extraemos las horas y las ventas de la respuesta
            var labels = response.data.map(item => `Hora ${item.Hora}`);
            var valores = response.data.map(item => item.TotalVentas);

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

        console.log(response)

        if (myPriceEvolutionChart) {
            myPriceEvolutionChart.destroy();
        }

        // Verificamos si la respuesta tiene datos
        if (response.data && response.data.length > 0) {
            // Extraemos las fechas y los precios de la respuesta
            var labels = response.data.map(item => item.FechaCompra);
            var precios = response.data.map(item => item.PrecioCompra);

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
            swal("No se encontraron datos", "No se encontraron datos para el producto seleccionado.", "info");
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
    $("#dateSelect").change(function () {
        ObtenerGananciaPorHora();
    });

    $("#productIdSelect").change(function () {
        idProducto = $(this).val();
        ObtenerEvolucionDelPrecio(idProducto); // Actualiza la gráfica de evolución del precio
    });
});
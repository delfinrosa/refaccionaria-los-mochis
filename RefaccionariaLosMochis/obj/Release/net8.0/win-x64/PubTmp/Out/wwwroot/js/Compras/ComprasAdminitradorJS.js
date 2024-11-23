
/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />
/// <reference path="../general/jqueryajax.js" />

var paginaTabla = 0; // Variable para la página actual de la tabla
var dataTabla;

// Función para cargar los datos paginados de la tabla de compras
function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual;
    var siguiente = num;
    var dataTabla = [];
    JQueryAjax_Normal('/Compras/ListarCompra', { pagina: paginaActual, siguientes: num, estatus: $("#estatusSelect").val() }, true, function (response) {
        if (response.data.length > 0) {
            dataTabla = response.data;
            llenarTabla(dataTabla);
        } else {
            var tbody = $("#tabla tbody");
            tbody.empty();
            tbody.append("<tr>").append($("<td>").text("No se encontraron compras con el estatus proporcionado."));;
        }
    }, function (error) {
        console.error("Error al listar compras:", error);
    });
}

// Función para llenar la tabla con los datos recibidos
function llenarTabla(tabla) {
    var tbody = $("#tabla tbody");
    tbody.empty();
    dataTabla = tabla
    $.each(tabla, function (index, item) {
        var row = $("<tr>");
        row.append($("<td>").text(item.idCompra));
        row.append($("<td>").text(item.proveedor));
        row.append($("<td>").text(item.cantidad));
        row.append($("<td>").text(item.precio));
        row.append($("<td>").html('<button type="button" class="btn-aprobar btn btn-success btn-sm ms-2"><i class="fa fa-check-square"></i></button>'+
            '<button type="button" class="btn-ver btn btn-primary btn-sm ms-2"><i class="fas fa-eye"></i></button>' +
            '<button type="button" class="btn-cancelar btn btn-secondary btn-sm ms-2"><i class="fas fa-window-close"></i></button>' +
            '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button>'));

        tbody.append(row);
    });
}

function CountPaginadoTabla() {
    return new Promise((resolve, reject) => {
        JQueryAjax_Normal('/Compras/ContarComprasPorEstatus', { estatus: $("#estatusSelect").val() }, true, function (response) {
            resolve(response.totalRegistros);
        }, function (error) {
            console.error("Error al contar compras por estatus:", error);
            reject(error);
        });
    });
}


async function cargartabla() {
    try {
        datosTablaPaginado(paginaTabla, 10);
        let totalRegistros = await CountPaginadoTabla();
        var divisiona = Math.ceil(totalRegistros / 10);
        configurarPaginacion(paginaTabla, divisiona, datosTablaPaginado, "T", 10);
    } catch (error) {
        console.error("Error al cargar la tabla:", error);
    }
}

$('#estatusSelect').on('change', function () {
    cargartabla()
});

//Acciones tabla

$("#tabla tbody").on("click", '.btn-aprobar', function () {
    var rowIndex = $(this).closest("tr").index();
    var compra = dataTabla[rowIndex];
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea aprobar la Comrpa?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        
    },
        function () {
            JQueryAjax_Normal('/Compras/AprobarCompra', { compraId: compra.idCompra }, true, function (data) {
                setTimeout(function () {
                    swal("Estatus Compra", data.mensaje, "success")
                }, 250);
                cargartabla()

            }, function () { });
        });
})

$("#tabla tbody").on("click", '.btn-cancelar', function () {
    var rowIndex = $(this).closest("tr").index();
    var compra = dataTabla[rowIndex];
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea cancelar la Compra?",
        type: "input",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: false,
        inputPlaceholder: "Ingrese motivo de cancelacion"
    },
        function (inputValue) {
            if (inputValue === false) return false;
            if (inputValue === "") {
                swal.showInputError("Debe ingresar un comentario!");
                return false;
            }
            JQueryAjax_Normal('/Compras/CancelarCompra', { compraId: compra.idCompra, comentario: inputValue }, true, function (data) {
                setTimeout(function () {
                    swal("Compra Cancelada", data.mensaje, "success")
                }, 250);
                cargartabla()
            }, function () { });
        });
});


$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var rowIndex = $(this).closest("tr").index();
    var compra = dataTabla[rowIndex];
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Compra?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
    },
        function () {
            JQueryAjax_Normal('/Compras/EliminarCompra', { compraId: compra.idCompra }, true, function (data) {
                setTimeout(function () {
                    swal("Eliminada Compra", data.Mensaje, "success")
                }, 250);
                cargartabla()
            }, function () { });
        });
});
$("#tabla tbody").on("click", '.btn-ver', function () {
    var rowIndex = $(this).closest("tr").index();
    var idCompra = dataTabla[rowIndex].idCompra;
    window.location.href = '/Compras/OrdenCompra?idCompra=' + idCompra;
});

cargartabla()


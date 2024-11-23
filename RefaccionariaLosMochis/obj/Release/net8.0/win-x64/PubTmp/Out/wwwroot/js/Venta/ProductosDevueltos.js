var dataTabla;
var filaSeleccionada;
var tablaInicializada = false;

var pagina = 0;
var paginaTabla = 0;
var activarWhere = false;
$('#iconoDescripcion').hide();
var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');
function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/Venta/ListarDevolucionesWhere', { strpagina: paginaTabla,  siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Venta/ListarDevoluciones', { strpagina: paginaTabla,  siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }

    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';
    // Llena la tabla con los datos obtenidos
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.noParte;
        row.insertCell(1).textContent = item.marca;
        row.insertCell(2).textContent = item.motivo;
        row.insertCell(3).textContent = item.fechaRegistro;
        row.insertCell(4).textContent = item.cantidad;

    });


}
function ActualizarTabla() {

    datosTablaPaginado(0, $("#cboDivicion-tabla").val());
    var division = Math.ceil(CountPaginadoTabla() / $("#cboDivicion-tabla").val());
    configurarPaginacion(0, division, datosTablaPaginado, "T", $("#cboDivicion-tabla").val())
}
function CountPaginadoTabla() {
    var resultado = 0;
    if ($("#checkFiltros").is(':checked')) {
        var where = whereQuery();
        JQueryAjax_Normal('/Venta/countDevolucionesWhere', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Venta/countDevoluciones', {}, false, function (data) {
            resultado = data.registros;
        }, function () { });
    }
    return resultado;
}
function cargartabla() {
    datosTablaPaginado(paginaTabla, 10);
    var divisiona = Math.ceil(CountPaginadoTabla() / 10);
    configurarPaginacion(paginaTabla, divisiona, datosTablaPaginado, "T", 10);
}
$("#btn-buscarTabla").click(function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla()
    }
    else {
        $("#filtrosProductosDevolucion").focus();
    }
});

$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtrosProductosDevolucion').slideDown();
    } else {
        $('#filtrosProductosDevolucion').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
});
function eventoCheckFiltro(checkbox, element) {
    checkbox.change(function () {
        if ($(this).is(':checked')) {
            element.attr('data-requerido-filtro', checkbox.val());
        } else {
            element.removeAttr('data-requerido-filtro');
        }
        element.prop('disabled', !$(this).is(':checked'));
        element.val("");
    });
}
eventoCheckFiltro($("#checkFiltroNoParte"), $("#txtFiltroNoParteWhere"));
eventoCheckFiltro($("#checkFiltroFecha"), $("#txtFiltroFechaWhere"));

function whereQuery() {
    var query = ""
    if ($("#checkFiltroNoParte").prop('checked')) {
        query += " P.NoParte LIKE '%" + $("#txtFiltroNoParteWhere").val() + "%'"
    }
    if ($("#checkFiltroFecha").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        var fecha = $("#txtFiltroFechaWhere").val();
        query += "vd.FechaRegistro = '" + fecha + "'";    } 
    return query
}
cargartabla();
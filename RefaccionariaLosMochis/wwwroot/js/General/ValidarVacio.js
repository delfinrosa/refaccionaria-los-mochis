function ValidarCampoVacio(dataRequerido) {
    var isValid = true;
    dataRequerido.each(function () {
        var tipoInput = $(this).data('requerido');
        if ($(this).data('requerido-filtro')) {
            tipoInput = $(this).data('requerido-filtro');
        }

        if ($(this).val().trim() === '') {
            swal("Campo Vacío", "El campo de " + tipoInput + " se encuentra vacío.", "error");
            isValid = false;
            return false;
        }
    });

    return isValid;
}

function modoEditando() {
    noNavegar()

    $('#modo').show();

    $("#modo").removeClass("animacion-guardar").addClass("animacion-editar");
    $("#icono-modo").remove();
    $("#modo").text("Editando");
    $("#modo").append(' <i id="icono-modo" class="fas fa-edit"></i>');
    $("[data-requerido]").removeClass("bg-guardar").addClass("bg-edicion");
}
function modoGuardando() {
    noNavegar()

    $('#modo').show();

    $("#modo").removeClass("animacion-editar").addClass("animacion-guardar");
    $("#icono-modo").remove();
    $("#modo").text("Guardando");
    $("#modo").append(' <i id="icono-modo" class="fas fa-clipboard"></i>');
    $("[data-requerido]").removeClass("bg-edicion").addClass("bg-guardar");

}
function noNavegar() {

    $('.btn-editar').prop('disabled', true);
    $('.btn-eliminar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);

    $('#btn-Actualizar').prop('disabled', true);
    $('#btn-BuscarBuscador').prop('disabled', true);
    $('#txtBusqueda').prop('disabled', true);

    $('#checkFiltros').prop('disabled', true);

    //$('#btn-paginado-anterior-tabla').prop('disabled', true);
    //$('#btn-paginado-siguiente-tabla').prop('disabled', true);
    //$('#btn-paginado-primero-tabla').prop('disabled', true);
    //$('#btn-paginado-ultimo-tabla').prop('disabled', true);
    //$('#cboDivicion-tabla').prop('disabled', true);
    $('#filtroslinea').hide();

}

function Navegar() {
    $('#btn-Nuevo').prop('disabled', false);
    $('#btn-Eliminar').prop('disabled', false);
    $('.btn-editar').prop('disabled', false);
    $('.btn-eliminar').prop('disabled', false);
    $('#btn-Actualizar').prop('disabled', false);
    $('#btn-BuscarBuscador').prop('disabled', false);
    $('#txtBusqueda').prop('disabled', false);
    $('#checkFiltros').prop('disabled', false);
    //$('#btn-paginado-anterior-tabla').prop('disabled', false);
    //$('#btn-paginado-siguiente-tabla').prop('disabled', false);
    //$('#btn-paginado-primero-tabla').prop('disabled', false);
    //$('#btn-paginado-ultimo-tabla').prop('disabled', false);
    //$('#cboDivicion-tabla').prop('disabled', false);

}

var camposRequeridos = document.querySelectorAll('[data-requerido]');

camposRequeridos.forEach(function (element) {
    function manejarCambioYClick() {
        var tabId = element.closest('.tab-pane').id;
        contarCamposVaciosEnTiempoReal(tabId);
    }

    element.addEventListener('change', manejarCambioYClick);
    element.addEventListener('click', manejarCambioYClick);

});



function contarCamposVaciosEnTiempoReal(tabId) {
    var tabElement = document.getElementById(tabId);

    if (tabElement) {
        var camposVacios = 0;
        tabElement.querySelectorAll('[data-requerido]').forEach(function (element) {
            if (element.value.trim() == '') {
                camposVacios++;
            }
        });
        var spanId = 'Spantab' + tabId.substring(3);

        var spanElement = document.getElementById(spanId);
        if (camposVacios > 0) {
            spanElement.style.display = 'inline';
            spanElement.textContent = camposVacios;
        } else {
            spanElement.style.display = 'none';
        }
    }
}

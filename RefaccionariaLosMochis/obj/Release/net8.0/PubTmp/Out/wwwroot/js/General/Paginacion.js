/// <reference path="../jquery-3.4.1.js" />

async function configurarPaginacion(paginaActual, totalPaginas, onPaginaCambiada, tipo, numElementosPorPagina) {
    var lblPagina;
    var IdSpan;
    var btnPaginadoAnterior;
    var btnPaginadoPrimero;
    var btnPaginadoUltimo;
    var btnPaginadoSiguiente;
  
    if (tipo == "T") {
        lblPagina = $('#paginadoTabla');
        IdSpan = "numPaginadoTabla";
        btnPaginadoAnterior = "btn-paginado-anterior-tabla";
        btnPaginadoPrimero = "btn-paginado-primero-tabla";
        btnPaginadoUltimo = "btn-paginado-ultimo-tabla";
        btnPaginadoSiguiente = "btn-paginado-siguiente-tabla";
        cboElementos = "cboDivicion-tabla";

    } else {
        lblPagina = $('#botonesPaginado');
        IdSpan = "numPaginadoBuscador";
        btnPaginadoAnterior = "btn-paginado-anterior-buscador";
        btnPaginadoPrimero = "btn-paginado-primero-buscador";
        btnPaginadoUltimo = "btn-paginado-ultimo-buscador";
        btnPaginadoSiguiente = "btn-paginado-siguiente-buscador";
        cboElementos = "cboDivicion-buscador";
    }
    $(lblPagina).empty();
    var listItem = $("<div>");

    listItem.append($('<span id="' + IdSpan + '">').text(paginaActual + 1 + ' de ' + totalPaginas));

    var opcionesLista = $('<div class="d-flex align-items-center">' +
        '<button id="' + btnPaginadoAnterior + '" class="btn btn-sm btn-primary mx-1 justify-content-between"><i class="fas fa-angle-left"></i></button>' +
        '<button id="' + btnPaginadoPrimero + '" class="btn btn-sm btn-primary mx-1 justify-content-between"><i class="fas fa-angle-double-left"></i></button>' +
        listItem.html() +
        '<button id="' + btnPaginadoUltimo + '" class="btn btn-sm btn-primary mx-1 justify-content-end"><i class="fas fa-angle-double-right"></i></button>' +
        '<button id="' + btnPaginadoSiguiente + '" class="btn btn-sm btn-primary mx-1 justify-content-end"><i class="fas fa-angle-right"></i></button> ' +
        '<select class="form-select form-select-sm w-auto" id="' + cboElementos + '">' +
        '<option value ="5" >5</option >' +
        '<option value="10">10</option>' +
        '<option value="15">15</option>' +
        '</select > <hr /></div>');

    $(lblPagina).append(opcionesLista);

    $('#' + cboElementos + '').val(numElementosPorPagina);

    $('#' + btnPaginadoAnterior + '').click(function () {
        if (paginaActual > 0) {
            paginaActual--;
            onPaginaCambiada(paginaActual, numElementosPorPagina);
            $('#' + IdSpan + '').text(paginaActual + 1 + ' de ' + totalPaginas);
        }
    });

    $('#' + btnPaginadoPrimero + '').click(function () {
        if (totalPaginas != 0) {
            paginaActual = 0;
            onPaginaCambiada(paginaActual, numElementosPorPagina);
            $('#' + IdSpan + '').text(paginaActual + 1 + ' de ' + totalPaginas);
        }

    });

    $('#' + btnPaginadoUltimo + '').click(function () {
        if (totalPaginas != 0) {
            paginaActual = totalPaginas - 1;
            onPaginaCambiada(paginaActual, numElementosPorPagina);
            $('#' + IdSpan + '').text(paginaActual + 1 + ' de ' + totalPaginas);
        }
    });
    $('#' + btnPaginadoSiguiente + '').click(function () {
        if (paginaActual < totalPaginas - 1) {
            paginaActual++;
            onPaginaCambiada(paginaActual, numElementosPorPagina);
            $('#' + IdSpan + '').text(paginaActual + 1 + ' de ' + totalPaginas);
        }
    });


    $('#' + cboElementos + '').change(function () {
        onPaginaCambiada(0, $('#' + cboElementos + '').val());
        var division;
        if (tipo == "B") {
            division = CountPaginadoBuscador($('#txtBusqueda').val().toLowerCase(), $('#' + cboElementos + '').val())
        } else {
            division = Math.ceil(CountPaginadoTabla() / $('#' + cboElementos + '').val());
        }
        configurarPaginacion(0, division, onPaginaCambiada, tipo, $('#' + cboElementos + '').val())
    });
}

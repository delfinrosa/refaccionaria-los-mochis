/// <reference path="../jquery-3.4.1.js" />

function configurarPaginacion(paginaActual, totalPaginas, onPaginaCambiada, tipo) {
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
    } else {
        debugger;
        lblPagina = $('#opcionesListaAbajo');
        IdSpan = "numPaginadoBuscador";
        btnPaginadoAnterior = "btn-paginado-anterior-buscador";
        btnPaginadoPrimero = "btn-paginado-primero-buscador";
        btnPaginadoUltimo = "btn-paginado-ultimo-buscador";
        btnPaginadoSiguiente = "btn-paginado-siguiente-buscador";
    }
    $(lblPagina).empty();
    var listItem = $("<div>");

    listItem.append($('<span id="' + IdSpan + '">').text(paginaActual + 1 + ' de ' + totalPaginas));

    var opcionesLista = $('<div class="d-flex align-items-center">' +
        '<button id="' + btnPaginadoAnterior + '" class="btn btn-sm btn-primary mx-1 justify-content-between"><i class="fas fa-angle-left"></i></button>' +
        '<button id="' + btnPaginadoPrimero + '" class="btn btn-sm btn-primary mx-1 justify-content-between"><i class="fas fa-angle-double-left"></i></button>' +
        listItem.html() +
        '<button id="' + btnPaginadoUltimo + '" class="btn btn-sm btn-primary mx-1 justify-content-end"><i class="fas fa-angle-double-right"></i></button>' +
        '<button id="' + btnPaginadoSiguiente + '" class="btn btn-sm btn-primary mx-1 justify-content-end"><i class="fas fa-angle-right"></i></button><hr /></div>');

    $(lblPagina).append(opcionesLista);

    $('#' + btnPaginadoAnterior + '').click(function () {
        if (paginaActual > 0) {
             paginaActual--;
            onPaginaCambiada(paginaActual);
            $('#' + IdSpan + '').text(paginaActual +1 + ' de ' + totalPaginas);
        }
    });

    $('#' + btnPaginadoPrimero + '').click(function () {

        paginaActual = 0;
        onPaginaCambiada(paginaActual);
        $('#' + IdSpan + '').text(paginaActual +1+ ' de ' + totalPaginas);
    });

    $('#' + btnPaginadoUltimo + '').click(function () {
        paginaActual = totalPaginas - 1;
        onPaginaCambiada(paginaActual);
        $('#' + IdSpan + '').text(paginaActual +1+ ' de ' + totalPaginas);
    });

    $('#' + btnPaginadoSiguiente + '').click(function () {
        if (paginaActual < totalPaginas - 1) {
            paginaActual++;
            onPaginaCambiada(paginaActual);
            $('#' + IdSpan + '').text(paginaActual +1 + ' de ' + totalPaginas);
        }
    });
}

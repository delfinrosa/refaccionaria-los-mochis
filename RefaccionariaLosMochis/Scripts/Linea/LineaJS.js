/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />
/// <reference path="../general/jqueryajax.js" />

var rol = getCookie("tipo");

var tabladata;
var filaSeleccionada;
var tablaInicializada = false;

var TextNombre = true;
var TextDescripcion = true;

var UltimoID;

var pagina = 0;
var paginaTabla = 0;

var query = 'I_A';

var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');

//MOSTRAR DATOS EN LA TABLA
function datosTablaPaginado(paginaActual, num) {
    $('#tabla').DataTable().clear().destroy();
    paginaTabla = paginaActual

    var dataTabla; // Variable para almacenar los datos devueltos por la llamada AJAX
    if ($("#checkFiltros").is(':checked')) {
        var where = whereQuery();
        var preguntawhere = $("#txtFiltroWhere").val();
        JQueryAjax_Normal('/Linea/ListarPruebaWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where, preguntawhere: preguntawhere }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Linea/ListarPrueba', { strpagina: paginaTabla, tipoOrden: query, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }


    var headers = '';
    if (!tablaInicializada) {
        // Marcar la tabla como inicializada
        tablaInicializada = true;

        if (rol == "A") {
            headers = '<th>Persona que modifico</th>' +
                '<th>Fecha de Registro</th>' +
                '<th>Fecha de Actualizacion</th>' +
                '<th></th>';
        } else {
            headers = '<th></th>';
        }
        $("#trowTabla").append(headers);
    }



    //MOSTRAR DATOS EN LA TABLA
    if (rol == "A") {
        tabladata = $("#tabla").DataTable({
            responsive: true,
            ordering: false,
            ///
            searching: false,
            paging: false,
            info: false,
            ///
            data: dataTabla,
            columns: [
                { data: "IdLinea" },
                { data: "Descripcion" },
                {
                    data: "Activo", render: function (valor) {
                        if (valor == "A") {
                            return '<h5 class="m-auto"><span class="badge rounded-pill bg-success">Activo</span></h5>'
                        } else if (valor == "O") {
                            return '<h5 class="m-auto"><span class="badge rounded-pill bg-secondary ">Oculto</span></h5>'
                        } else {
                            return '<h5 class="m-auto"><span class="badge rounded-pill bg-danger">Desacticvado</span></h5>';
                        }
                    }
                },
                { data: "Deslc" },
                { data: "IdUsuario" },
                { data: "fechaCreaccion" },
                { data: "fechaActualizacion" },
                {
                    "defaultContent": '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
                        '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button>',
                    "orderable": false,
                    "searchable": false,
                    "width": "90px"
                }
            ]
        });

    } else {
        tabladata = $("#tabla").dataTable({
            responsive: true,
            ordering: false,
            ///
            searching: false,
            paging: false,
            info: false,
            ///
            data: dataTabla,
            columns: [
                { data: "IdLinea" },
                { data: "Descripcion" },
                {
                    data: "Activo", render: function (valor) {
                        if (valor == "A") {
                            return '<h5 class="m-auto"><span class="badge rounded-pill bg-success">Activo</span></h5>'
                        } else if (valor == "O") {
                            return '<h5 class="m-auto"><span class="badge rounded-pill bg-secondary ">Oculto</span></h5>'
                        } else {
                            return '<h5 class="m-auto"><span class="badge rounded-pill bg-danger">Desacticvado</span></h5>';
                        }
                    }
                },
                { data: "Deslc" },
                {

                    "defaultContent": '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
                        '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button>',
                    "orderable": false,
                    "searchable": false,
                    "width": "90px"
                }
            ]

        });

    }
    if ($("#checkFiltros").is(':checked')) {
        MarcadorTabla()
    } 

}

/////////
//METODOS
/////////

//BOTON GUARDAR 
function Guardar() {
    var textActivo;

    if ($('#cboRegistroActivo').val() == "A") {
        textActivo = "A";
    } else if ($('#cboRegistroActivo').val() == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }

    var Linea = {
        IdLinea: $('#txtRegistroid').val(),
        Descripcion: $('#txtRegistroNombre').val(),
        Activo: textActivo,
        Deslc: $('#txtRegistrodescripcion').val()
    }

    JQueryAjax_Normal('/Linea/GuardarLinea', { objeto: Linea }, true, function (data) {
        //Linea guardar
        if (Linea.IdLinea == 0) {
            if (data.resultado != 0) {
                UltimoID = data.resultado;
                $('#txtRegistroid').val(data.resultado);
                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")
                limpiar();
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
        }
        //Linea editar
        else {
            if (data.resultado) {
                UltimoID = Linea.IdLinea;
                $('#mensajeError').hide();
                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
                limpiar();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }

        }
    }, function () { })
}

//Nuevo Editar
function abrirModal2(json) {
    if (json != null) {
        $('#txtRegistroid').val(json.Lista.IdLinea);
        $('#txtRegistroNombre').val(json.Lista.Descripcion);
        if (json.Lista.Activo == "A") {
            $('#cboRegistroActivo').val("A");
        } else if (json.Lista.Activo == "O") {
            $('#cboRegistroActivo').val("O");
        } else {
            $('#cboRegistroActivo').val("D");
        }
        $('#txtRegistrodescripcion').val(json.Lista.Deslc);
    }
}

function FiltroPorLinea2(texto) {
    JQueryAjax_Normal('/Linea/BusquedaFiltroLinea', { nombre: texto }, true, function (data) {
        if (data.Descripcion != "") {
            $('#txtRegistroNombre').val(data.Descripcion);
            $('#txtRegistrodescripcion').val(data.Deslc);
            $('#txtRegistroid').val(data.IdLinea);
            /*ACTIVO USUARIO*/
            if (data.Activo == "A") {
                $('#cboRegistroActivo').val("A");
            } else if (data.Activo == "O") {
                $('#cboRegistroActivo').val("O");
            } else {
                $('#cboRegistroActivo').val("D");
            }
            $('#opcionesLista').hide();
            $('#opcionesListaAbajo').hide();

        } else {
            // Si no se encontraron resultados, puedes limpiar los campos o mostrar un mensaje de error
            $('#txtRegistroNombre').val('');
            $('#txtRegistrodescripcion').val('');
            $('#cboRegistroActivo').val('A');
        }
    }, function () { }
    );
}

function limpiar() {
    $('#txtRegistroid').val("");
    $('#txtRegistroNombre').val("");
    $('#cboRegistroActivo').val("A");
    $('#txtRegistrodescripcion').val("");
}

///////////////
///Carga el ultimo
///////////////

function CargarUltimo() {
    JQueryAjax_Normal('/Linea/UltimoRegistro', {}, true, function (data) {
        UltimoID = data.Lista.IdLinea
        abrirModal2(data);
    }, function () { });
}
////////////////
/// BUSCADOR
///////////////

function desplegarPaginacion(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
    JQueryAjax_Normal('/Linea/PaginacionPRUEBA', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    input.val(item);
                    FiltroPorLinea2(listItem.text());
                });
                opcionesLista.append(listItem);
            });
            opcionesLista.show();
            $('#opcionesListaAbajo').show();
        }
    }, function () { });
}


function obtenerDivision(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Linea/COUNT_PruebasAutoCompletado', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}



//////////////
//Paginado Tabla
//////////////

function CountPaginadoTabla() {
    var resultado = 0;
    if ($("#checkFiltros").is(':checked')) {
        var where = whereQuery();
        var preguntawhere = $("#txtFiltroWhere").val();
        JQueryAjax_Normal('/Linea/COUNT_TablaWhere', { where: where, preguntawhere: preguntawhere }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Linea/COUNT_Tabla', {}, false, function (data) {
            resultado = data.registros;
        }, function () { });
    }
    return resultado;




}



//////////
//BOTONES
//////////

//Barra de heramientas 

$("#btn-Nuevo").click(function () {
    limpiar();
    TextNombre = false;
    TextDescripcion = false;
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    swal("Crea un Nuevo Registro", "Introduce los datos del registro llena todo los campos ")
});

$("#btn-Cancelar").click(function () {
    CargarUltimo();
});

$("#btn-Guardar").click(function () {
    if (ValidarCampoVacio($('[data-requerido]'))) {
        $('#btn-Eliminar').prop('disabled', false);
        Guardar();
    }
});

$("#tablaId").click(function () {
    paginaTabla = 0
    if (query == 'I_A') {
        query = 'I_D';
    } else {
        query = 'I_A';
    }
    ActualizarTabla()

});

$("#tablaDescripcion").click(function () {
    paginaTabla = 0
    if (query == 'D_A') {
        query = 'D_D';
    } else {
        query = 'D_A';
    }
    ActualizarTabla()

});

$("#btn-Eliminar").click(function () {

    var idBorrar = $("#txtRegistroid").val();

    if (idBorrar != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar la línea?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/Linea/EliminarLinea', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    datosTablaPaginado(paginaTabla, $("#cboDivicion-tabla").val());
                    CargarUltimo();
                } else {
                    swal("No se pudo eliminar", data.mensaje, "error");
                }
            }, function () { });
        });
    }
});

// Focus al input Buscar (Icono lupa)
$('#botonBuscar').click(function () {
    $('#txtBusquedaNuevo').focus();
});

/////
//TABLA
/////
$("#btn-Actualizar").click(function () {
    ActualizarTabla()
});

//BOTON EDITAR
$("#tabla tbody").on("click", '.btn-editar', function () {
    // Obtener datos del registro de la tabla
    var data = tabladata.row($(this).closest("tr")).data();

    JQueryAjax_Normal('/Linea/ListarPorIdLineas', { Id: data.IdLinea }, false,
        function (data) {
            abrirModal2(data);
        }, function () { });

    TextNombre = true;
    TextDescripcion = true;

});



//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var Lineaseleccionado = $(this).closest("tr");
    var data = tabladata.row(Lineaseleccionado).data();
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Linea?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Linea/EliminarLinea', { id: data.IdLinea }, true, function (data) {

                if (data.resultado) {
                    datosTablaPaginado(paginaTabla, $("#cboDivicion-tabla").val());
                    CargarUltimo();
                } else {
                    swal("No se pudo eliminar", data.mensaje, "error");
                }
            }, function () { });

        });

})

////////////
//INPUT
////////////

$("#txtRegistroNombre").on('input', function () {
    TextNombre = true;
    if (TextNombre && TextDescripcion) {
        $('#btn-Guardar').prop('disabled', false);

    }
});

$("#txtRegistrodescripcion").on('input', function () {
    TextDescripcion = true;
    if (TextNombre && TextDescripcion) {
        $('#btn-Guardar').prop('disabled', false);

    }
});

$('#txtBusquedaNuevo').on('input', function () {
    var NumeroDivisiones = 5;

    if ($('#opcionesListaAbajo').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var searchTerm = $('#txtBusquedaNuevo').val().toLowerCase();
    pagina = 0;
    desplegarPaginacion(pagina, NumeroDivisiones);
    var division = obtenerDivision(searchTerm, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacion, "B", NumeroDivisiones)

});

// Ocultar opcionesLista cuando se hace clic fuera de ella
$(document).on('click', function (event) {

    if (!$(event.target).closest('#buscador').length) {
        $('#opcionesListaAbajo').hide();
        opcionesLista.hide();

    }
});

///////////
//AL CARGAR
///////////
CargarUltimo();

function cargartabla() {
    datosTablaPaginado(paginaTabla, 10);
    var divisiona = Math.ceil(CountPaginadoTabla() / 10);
    configurarPaginacion(paginaTabla, divisiona, datosTablaPaginado, "T", 10)

}

cargartabla()

$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtroslinea').removeAttr('hidden');
    } else {
        $("#txtFiltroWhere").val("")
        $('#filtroslinea').attr('hidden', true);
        ActualizarTabla()
    }
})

$('input[name=grupoFiltros]').change(function () {
    $("#buscadorFiltroTabla").html($(this).val());
});

function whereQuery() {
    switch ($('input[name=grupoFiltros]:checked').val()) {
        case "ID":
            return "l.IdLinea";
        case "Descripcion":
            return "l.Descripcion ";
        case "Descripcion Linea":
            return "lc.Descripcion ";
    }
}

$("#btn-buscarTabla").click(function () {
    ActualizarTabla()
});

function ActualizarTabla() {
    datosTablaPaginado(0, $("#cboDivicion-tabla").val());
    var division = Math.ceil(CountPaginadoTabla() / $("#cboDivicion-tabla").val());
    configurarPaginacion(0, division, datosTablaPaginado, "T", $("#cboDivicion-tabla").val())
}

function MarcadorTabla() {
    $('#tabla tbody td').each(function () {
        var textoCelda = $(this).text().trim();
        if (textoCelda.includes($("#txtFiltroWhere").val())) {
            var spanResaltado = $('<span>').html(textoCelda.replace(new RegExp($("#txtFiltroWhere").val(), 'gi'), match => `<span style="background-color: yellow;">${match}</span>`));
            $(this).html(spanResaltado);
        }
    });
}
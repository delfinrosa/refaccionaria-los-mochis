
var rol = getCookie("tipo");

var dataTabla;
var filaSeleccionada;
var tablaInicializada = false;

var pagina = 0;
var paginaTabla = 0;

var query = 'I_A';
var activarWhere = false;

$('#iconoDescripcion').hide();
var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');
var boolGuaradar = 0;

function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/Proveedor/ListarProveedorWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Proveedor/ListarProveedor', { strpagina: paginaTabla, tipoOrden: query, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }

    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';

    // Llena la tabla con los datos obtenidos
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.razonSocial;
        row.insertCell(1).textContent = item.rfc;
        row.insertCell(2).textContent = item.telefono;
        row.insertCell(3).textContent = item.correo;
        var accionesCell = row.insertCell(4);

        accionesCell.innerHTML = '<span class="icono-texto-container">' +
            '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
            '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button></span>';
    });


    if ($("#checkFiltros").is(':checked')) {
        if (dataTabla.length != 0) {
            MarcadorTabla()
        }
        else {
            swal("Filtro NO valido", "No se a encontrado ningun Registro con el Filtro puesto")
        }
    }
}
/////////
//METODOS
/////////
function Guardar() {
    var Proveedor = {
        RFC: $('#txtRegistroRFC').val().toUpperCase() || "",
        RazonSocial: $('#txtRegistroRazonSocial').val().toUpperCase() || "",
        Calle: $('#txtRegistroCalle').val().toUpperCase() || "",
        Estado: $('#txtRegistroEstado').val().toUpperCase() || "",
        Pais: $('#txtRegistroPais').val().toUpperCase() || "",
        CP: $('#txtRegistroCP').val().toUpperCase() || "",
        NumeroInt: $('#txtRegistroNumeroInt').val().toUpperCase() || "",
        NumeroExt: $('#txtRegistroNumeroExt').val().toUpperCase() || "",
        Telefono: $('#txtRegistroTelefono').val().toUpperCase() || "",
        Correo: $('#txtRegistroCorreo').val().toUpperCase() || "",
        Estatus: $('#txtRegistroEstatus').val().toUpperCase() || "",
        Colonia: $('#txtRegistroColonia').val().toUpperCase() || "",
        Ciudad: $('#txtRegistroCiudad').val().toUpperCase() || "",
        Comentario: $('#txtRegistroComentario').val().toUpperCase() || "NA",
        oUsuario: {
            IdUsuario: 0
        }
    }

    JQueryAjax_Normal('/Proveedor/GuardarProveedor', { objeto: Proveedor, boolGuaradar: boolGuaradar }, false, function (data) {
        //Proveedor guardar
        if (boolGuaradar == 1) {
            if (data.resultado != 0) {
                $('#txtRegistroid').val(data.resultado);
                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroRazonSocial').val(), "success")
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
            boolGuaradar = 0;
        }
        //Proveedor editar
        else {
            if (data.resultado) {
                $('#mensajeError').hide();
                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroRazonSocial').val(), "success")
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
        }
    }, function () { })
}
// Desplegar
function DesplegarInformacionCampos(json) {
    if (json != null) {
        $('#txtRegistroRFC').val(json.lista.rfc);
        $('#txtRegistroRazonSocial').val(json.lista.razonSocial);
        $('#txtRegistroCalle').val(json.lista.calle);
        $('#txtRegistroEstado').val(json.lista.estado);
        $('#txtRegistroPais').val(json.lista.pais);
        $('#txtRegistroCP').val(json.lista.cp);
        $('#txtRegistroNumeroInt').val(json.lista.numeroInt);
        $('#txtRegistroNumeroExt').val(json.lista.numeroExt);
        $('#txtRegistroTelefono').val(json.lista.telefono);
        $('#txtRegistroCorreo').val(json.lista.correo);
        $('#txtRegistroEstatus').val(json.lista.estatus);
        $('#txtRegistroColonia').val(json.lista.colonia);
        $('#txtRegistroCiudad').val(json.lista.ciudad);
        $('#txtRegistroComentario').val(json.lista.comentario);

    }
}
function desplegarProveedorPorNombre(texto) {
    JQueryAjax_Normal('/Proveedor/buscarProveedorPorNombre', { nombre: texto }, true, function (data) {
        if (data.RazonSocial != "") {
            $('#txtRegistroRFC').val(data.lista.rfc);
            $('#txtRegistroRazonSocial').val(data.lista.razonSocial);
            $('#txtRegistroCalle').val(data.lista.calle);
            $('#txtRegistroEstado').val(data.lista.estado);
            $('#txtRegistroPais').val(data.lista.pais);
            $('#txtRegistroCP').val(data.lista.cp);
            $('#txtRegistroNumeroInt').val(data.lista.numeroInt);
            $('#txtRegistroNumeroExt').val(data.lista.numeroExt);
            $('#txtRegistroTelefono').val(data.lista.telefono);
            $('#txtRegistroCorreo').val(data.lista.correo);
            $('#txtRegistroEstatus').val(data.lista.estatus);
            $('#txtRegistroColonia').val(data.lista.colonia);
            $('#txtRegistroCiudad').val(data.lista.ciudad);
            $('#txtRegistroComentario').val(data.lista.comentario);

            $('#opcionesLista').hide();
            $('#botonesPaginado').hide();
        } else {
            limpiar()
        }
    }, function () { }
    );
}
function limpiar() {
    $('#txtRegistroRFC').focus();

    $('#txtRegistroid').val("");
    $('#txtRegistroRFC').val("");
    $('#txtRegistroRazonSocial').val("");
    $('#txtRegistroCalle').val("");
    $('#txtRegistroEstado').val("");
    $('#txtRegistroPais').val("");
    $('#txtRegistroCP').val("");
    $('#txtRegistroNumeroInt').val("");
    $('#txtRegistroNumeroExt').val("");
    $('#txtRegistroTelefono').val("");
    $('#txtRegistroCorreo').val("");

    $('#txtRegistroEstatus').val("");
    $('#txtRegistroColonia').val("");
    $('#txtRegistroCiudad').val("");
    $('#txtRegistroComentario').val("");

}
///////////////
///Carga el ultimo
///////////////
function CargarUltimo() {
    $('#txtRegistroRFC').focus();
    JQueryAjax_Normal('/Proveedor/UltimoRegistro', {}, true, function (data) {
        DesplegarInformacionCampos(data);
    }, function () { });
}
////////////////
/// BUSCADOR
///////////////
function desplegarPaginacionBuscador(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusqueda').val().toLowerCase();
    JQueryAjax_Normal('/Proveedor/elementosPaginacionBuscador', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    input.val(item);
                    desplegarProveedorPorNombre(listItem.text());
                });
                opcionesLista.append(listItem);
            });
            opcionesLista.show();
            $('#botonesPaginado').show();
        }
    }, function () { });
}
function CountPaginadoBuscador(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Proveedor/countBuscador', { nombre: texto }, false, function (data) {
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
        JQueryAjax_Normal('/Proveedor/countTablaWhere', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Proveedor/countTabla', {}, false, function (data) {
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
    $('#btn-Guardar').prop('disabled', false);
    $('#btn-Nuevo').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', false);
    modoGuardando()
    document.getElementById('SpantabDatosGenerales').textContent = '13';
    boolGuaradar = 1;
});
$("#btn-Cancelar").click(function () {
    swal({
        title: "¿Esta Seguro?",
        text: "Si cancela no se realizara ningun cambio en el registro",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            CargarUltimo();
            $('#btn-Eliminar').prop('disabled', false);
            $('#btn-Cancelar').prop('disabled', true);
            $('#btn-Guardar').prop('disabled', true);
            $('#btn-Nuevo').prop('disabled', false);
            $('#modo').hide();

            $(".bg-edicion").removeClass("bg-edicion");
            $(".bg-guardar").removeClass("bg-guardar");
            Navegar()
            document.getElementById('SpantabDatosGenerales').textContent = '';
            boolGuaradar = 0;
1
        });



});
$("#btn-Guardar").click(function () {
    if (ValidarCampoVacio($('[data-requerido]'))) {
        $('#btn-Eliminar').prop('disabled', false);
        $('#btn-Guardar').prop('disabled', true);
        $('#btn-Cancelar').prop('disabled', true);
        $('#btn-Nuevo').prop('disabled', false);
        Guardar();

        ActualizarTabla();
        CargarUltimo();

        $(".bg-edicion").removeClass("bg-edicion");
        $(".bg-guardar").removeClass("bg-guardar");
        Navegar()

        $('#modo').hide();

    }
});
$("#btn-Eliminar").click(function () {
    $('#btn-Guardar').prop('disabled', false);
    var RFCBorrar = $("#txtRegistroRFC").val();
    if (RFCBorrar != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar el Proveedor " + $("#txtRegistroRazonSocial").val() + " ?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/Proveedor/EliminarProveedor', { rfc: RFCBorrar }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Proveedor Eliminado", "la Proveedor se elimino correctamente", "success")
                    }, 250);
                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });
        });
    }
});

$('[data-requerido]').on('change input', function () {
    $('#btn-Cancelar').prop('disabled', false);
    $('#btn-Guardar').prop('disabled', false);
    if (boolGuaradar == true) {
        modoGuardando();
    } else {
        modoEditando();
    }
});
//$("#tablaId").click(function () {
//    paginaTabla = 0;
//    $('#iconoDescripcion').hide();
//    $('#iconoID').show();
//    query = (query == 'I_A') ? 'I_D' : 'I_A';
//    var nuevoIconoClass = (query == 'I_A') ? 'fa-sort-numeric-down' : 'fa-sort-numeric-down-alt';
//    $('#iconoID').removeClass('fa-sort-numeric-down fa-sort-numeric-down-alt').addClass(`${nuevoIconoClass} fa-2x m-2`);
//    ActualizarTabla();
//});
$("#tablaRazonSocial").click(function () {
    paginaTabla = 0
    $('#iconoID').hide();
    $('#iconoDescripcion').show();
    query = (query == 'D_A') ? 'D_D' : 'D_A';
    var nuevoIconoClass = (query == 'D_A') ? 'fas fa-sort-alpha-down' : 'fas fa-sort-alpha-down-alt';
    $('#iconoDescripcion').removeClass('fas fa-sort-alpha-down fas fa-sort-alpha-down-alt').addClass(`${nuevoIconoClass} fa-2x m-2`);
    ActualizarTabla()
});


// Focus al input Buscar (Icono lupa)
$('#btn-BuscarBuscador').click(function () {
    $('#txtBusqueda').focus();
});

$("#btn-Actualizar").click(function () {
    ActualizarTabla();
});

//BOTON EDITAR
$("#tabla tbody").on("click", ".btn-editar", function () {
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', false);
    $('#btn-Eliminar').prop('disabled', false);
    $('#btn-Cancelar').prop('disabled', false);
    $('#txtRegistroNombre').focus();

    var rowIndex = $(this).closest("tr").index();
    var Proveedor = dataTabla[rowIndex];
    JQueryAjax_Normal('/Proveedor/BusquedaID', { Id: Proveedor.rfc }, false,
        function (data) {
            DesplegarInformacionCampos(data);
        }, function () { });
});

//BOTON ELIMINAR
$("#tabla").on("click", ".btn-eliminar", function () {

    var rowIndex = $(this).closest("tr").index(); // Obtener el índice de la fila
    var Proveedor = dataTabla[rowIndex]; // Obtener el objeto de la fila correspondiente
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Proveedor " + Proveedor.razonSocial + " ?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Proveedor/EliminarProveedor', { id: Proveedor.idProvedor }, true, function (data) {
                if (data.resultado) {
                    dataTabla.splice(rowIndex, 1);
                    datosTablaPaginado(paginaTabla, $("#cboDivicion-tabla").val());
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Proveedor Eliminada", "la Proveedor se elimino correctamente", "success")
                    }, 250);
                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });
        });
})

////////////
//INPUT
////////////

$("#txtRegistroNombre").on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});

$("#txtRegistrodescripcion").on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});

$('#txtBusqueda').on('input', function () {
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var searchTerm = $('#txtBusqueda').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscador(pagina, NumeroDivisiones);
    var division = CountPaginadoBuscador(searchTerm, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscador, "B", NumeroDivisiones)

});

// Ocultar opcionesLista cuando se hace clic fuera de ella
$(document).on('click', function (event) {

    if (!$(event.target).closest('#buscador').length) {
        $('#botonesPaginado').hide();
        opcionesLista.hide();
    }
});

///////////
//AL CARGAR
///////////

function cargartabla() {
    datosTablaPaginado(paginaTabla, 10);
    var divisiona = Math.ceil(CountPaginadoTabla() / 10);
    configurarPaginacion(paginaTabla, divisiona, datosTablaPaginado, "T", 10);
}

$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtrosProveedor').slideDown();
    } else {
        $('#filtrosProveedor').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
});

function whereQuery() {
    var query = ""
    if ($("#checkFiltroTelefono").prop('checked')) {
        query += "P.Telefono LIKE '%" + $("#txtFiltroTelefonoWhere").val() + "%'"
    }
    if ($("#checkFiltroRazonSocial").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "P.RazonSocial LIKE '%" + $("#txtFiltroRazonSocialWhere").val() + "%'"
    } 
    return query
}

$("#btn-buscarTabla").click(function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla()
    }
    else {
        $("#filtrosProveedor").focus();
    }
});

$("#tabla tbody").on("click", '#masInformacion', function () {
    if ($("#checkFiltros").prop('checked')) {
        swal({
            title: "Filtro Activo",
            text: "Puede que el filtro este mal, ¿quiere quitarlo?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            cancelButtonClass: "btn-success",
            confirmButtonText: "Si",
            cancelButtonText: "No",
            closeOnConfirm: true,
            closeOnCancel: true
        },
            function (isConfirm) {
                if (isConfirm) {
                    desactivarFiltro();
                    ActualizarTabla();
                } else {
                    $("#checkFiltros").focus();
                }
            });
    } else {
        swal({
            title: "Fallo en la coneccion con la base de datos",
            text: "Espere un momento en lo que se intenta reconectar con la base de datos",
            type: "info",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true
        }, function () {
            setTimeout(function () {
                JQueryAjax_Normal('/Proveedor/ChecarConexion', {}, true, function (data) {
                    if (data.resultado) {
                        swal("Coneccion Exitosa", "La conceccion se realizo correctamente", "success");
                        desactivarFiltro();
                        ActualizarTabla();
                    } else {
                        swal("No se pudo conectar", "Se presento un fallo en la coneccion", "error");
                    }
                }, function () { });
            }, 2000);
        });
    }
});
function ActualizarTabla() {
    datosTablaPaginado(0, $("#cboDivicion-tabla").val());
    var division = Math.ceil(CountPaginadoTabla() / $("#cboDivicion-tabla").val());
    configurarPaginacion(0, division, datosTablaPaginado, "T", $("#cboDivicion-tabla").val())
}

function MarcadorTabla() {
    marcarCeldas(2, $("#txtFiltroTelefonoWhere").val(), 'yellow');
    marcarCeldas(0, $("#txtFiltroRazonSocialWhere").val(), '#FCC208');

}

function marcarCeldas(columna, filtro, color) {
    $('#tabla tbody tr').each(function () {
        var textoCelda = $(this).find('td:eq(' + columna + ')').text().trim().toLowerCase();
        var filtroMinusculas = filtro.toLowerCase();
        if (textoCelda.includes(filtroMinusculas)) {
            var spanResaltado = $('<span>').html(textoCelda.replace(new RegExp(filtroMinusculas, 'gi'), match => `<span style="background-color: ${color};">${match}</span>`));
            $(this).find('td:eq(' + columna + ')').html(spanResaltado);
        }
    });
}

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

function desactivarFiltro() {
    $("#checkFiltros").prop('checked', false);
    $("#checkFiltroId").prop('checked', false);
    $("#checkFiltroRazonSocial").prop('checked', false);

    $("#checkFiltroId").removeAttr('data-requerido-filtro');
    $("#checkFiltroRazonSocial").removeAttr('data-requerido-filtro');

    $("#txtFiltroIDWhere").prop('disabled', true);
    $("#txtFiltroRazonSocialWhere").prop('disabled', true);

    $("#txtFiltroIDWhere").val("");
    $("#txtFiltroRazonSocialWhere").val("");

    $('#filtrosProveedor').attr('hidden', true);


}

CargarUltimo();
cargartabla()
eventoCheckFiltro($("#checkFiltroTelefono"), $("#txtFiltroTelefonoWhere"));
eventoCheckFiltro($("#checkFiltroRazonSocial"), $("#txtFiltroRazonSocialWhere"));

$(document).ready(function () {
    JQueryAjax_Normal('/Proveedor/tamaño', {}, true, function (campos) {

        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                }
            }
        });
    }, function () { });



});
/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />
/// <reference path="../general/jqueryajax.js" />
var rol = getCookie("tipo");
var tabladata;
var filaSeleccionada;
var tablaInicializada = false;
var pagina = 0;
var paginaTabla = 0;
var activarWhere = false;
var query = 'I_A';
$('#iconoDescripcion').hide();
var opcionesLista = $('#opcionesLista');
var input = $('#txtBusqueda');
    var dataTabla; 
//MOSTRAR DATOS EN LA TABLA
function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/Usuario/ListarUsuarioTablaWhere', { strpagina: paginaTabla, tipoOrden: query, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Usuario/ListarUsuarioTabla', { strpagina: paginaTabla, tipoOrden: query, siguientes: num }, false, function (data) {
            dataTabla = data.data;
        }, function () { });
    }


    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';

    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.IdUsuario;
        row.insertCell(1).textContent = item.Nombre;
        row.insertCell(2).textContent = item.Correo;
        var tipoCell = row.insertCell(3);

        tipoCell.innerHTML = (function () {
            var valor = item.Tipo;
            if (valor == "A") {
                return '<h5 class="m-auto"><span class="badge bg-success"><i class="fas fa-cog me-1"></i>Admintardor</span></h5>';
            } else if (valor == "I") {
                return '<h5 class="m-auto"><span class="badge bg-primary "><i class="fas fa-table me-1"></i>Inventario</span></h5>';
            } else if (valor == "C") {
                return '<h5 class="m-auto"><span class="badge bg-secondary "><i class="fas fa-money-bill me-1"></i>Cajero</span></h5>';
            } else {
                return '<h5 class="m-auto"><span class="badge bg-danger"><i class="fas fa-tag me-1"></i>Vendedor</span></h5>';
            }
        })();

        var estadoCell = row.insertCell(4);

        estadoCell.innerHTML = (function () {
            var valor = item.Activo;
            if (valor == "A") {
                return '<h5 class="m-auto"><span class="badge rounded-pill bg-success">Activo</span></h5>';
            } else if (valor == "O") {
                return '<h5 class="m-auto"><span class="badge rounded-pill bg-secondary">Oculto</span></h5>';
            } else {
                return '<h5 class="m-auto"><span class="badge rounded-pill bg-danger">Desactivado</span></h5>';
            }
        })();

        row.insertCell(5).textContent = '';  
        var accionesCell = row.insertCell(5);
        accionesCell.innerHTML = '<span class="icono-texto-container">' +
            '<a href="#cardRegistros"><button type="button" class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pen"></i></button></a>' +
            '<button type="button" class="btn-eliminar btn btn-danger btn-sm ms-2"><i class="fas fa-trash"></i></button></span>';
    });

    if ($("#checkFiltros").is(':checked')) {
        if (dataTabla.length != 0) {
            UsuariodorTabla()
        }
        else {
            swal("Filtro NO valido", "No se a encontrado ningun Registro con el Filtro puesto")

        }
    }
}


/////////
//METODOS
/////////

//BOTON GUARDAR 
function Guardar() {
    var textActivo;
    var ValActivo = $('#cboRegistroActivo').val();
    if (ValActivo == "A") {
        textActivo = "A";
    } else if (ValActivo == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }
    var textTipo;
    var ValTipo = $('#cbotipo').val();
    if (ValTipo == "A") {
        textTipo = "A";
    } else if (ValTipo == "C") {
        textTipo = "C";
    } else if (ValTipo == "V") {
        textTipo = "V";
    } else {
        textTipo = "I";
    }
    var Usuario = {
        IdUsuario: $('#txtRegistroid').val(),
        Nombre: $('#txtRegistroNombre').val(),
        Contraseña: $('#txtcontraseña').val(),
        Correo: $('#txtcorreo').val(),
        Tipo: textTipo,
        Activo: textActivo
    }
    JQueryAjax_Normal('/Usuario/GuardarUsuario', { objeto: Usuario }, false, function (data) {
        //Usuario guardar
        if (Usuario.IdUsuario == 0) {
            if (data.resultado != 0) {
                $('#txtRegistroid').val(data.resultado);
                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")
                ActualizarTabla();
                CargarUltimo();
                $('#mensajeError').hide();
            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }
        }
        //Usuario editar
        else {
            if (data.resultado) {
                $('#mensajeError').hide();
                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
                ActualizarTabla();
                CargarUltimo();
                $('#mensajeError').hide();            }
            else {
                $('#mensajeError').text(data.mensaje);
                $('#mensajeError').show();
            }

        }
    }, function () { })
}

//Nuevo Editar
function DesplegarInformacionCampos(json) {
    if (json != null) {
        $('#txtRegistroid').val(json.Lista.IdUsuario);
        $('#txtRegistroNombre').val(json.Lista.Nombre);
        $('#txtcontraseña').val(json.Lista.Contraseña);
        $('#txtcorreo').val(json.Lista.Correo);
        if (json.Lista.Activo == "A") {
            $('#cboactivo').val("A");
        } else if (json.Lista.Activo == "O") {
            $('#cboactivo').val("O");
        } else {
            $('#cboactivo').val("D");
        } 

        if (json.Lista.Tipo == "A") {
            $('#cbotipo').val("A");
        } else if (json.Lista.Tipo == "C") {
            $('#cbotipo').val("C");
        } else if (json.Lista.Tipo == "V") {
            $('#cbotipo').val("V");
        } else {
            $('#cbotipo').val("I");
        }
    }
}

function desplegarUsuarioPorNombre(texto) {
    JQueryAjax_Normal('/Usuario/bucarUsuarioPorNombre', { nombre: texto }, true, function (data) {
        if (data.Nombre != "") {
            DesplegarInformacionCampos(data)
        } else {
            limpiar();
        }
    }, function () { }
    );
}

function limpiar() {
    $('#txtRegistroNombre').focus();

    $('#txtRegistroid').val("");
    $('#txtRegistroNombre').val("");
    $('#txtcorreo').val("");
    $('#txtcontraseña').val("");
    $('#cboactivo').val("A");
    $('#cbotipo').val("A");
    $('#mensajeError').hide();

}



///////////////
///Carga el ultimo
///////////////

function CargarUltimo() {
    $('#txtRegistroNombre').focus();

    JQueryAjax_Normal('/Usuario/UltimoRegistroUsuario', {}, true, function (data) {
        DesplegarInformacionCampos(data);
    }, function () { });
}



function ActualizarTabla() {
    datosTablaPaginado(0, $("#cboDivicion-tabla").val());
    var division = Math.ceil(CountPaginadoTabla() / $("#cboDivicion-tabla").val());
    configurarPaginacion(0, division, datosTablaPaginado, "T", $("#cboDivicion-tabla").val())
}
//////////
//BOTONES
//////////

//Barra de heramientas 
$("#btn-Nuevo").click(function () {
    limpiar();
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', false);
    modoGuardando()
    document.getElementById('SpantabDatosGenerales').textContent = '3';

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
        $('#modo').hide();

        $(".bg-edicion").removeClass("bg-edicion");
        $(".bg-guardar").removeClass("bg-guardar");
        Navegar()
    }
});
$("#btn-Eliminar").click(function () {
    $('#btn-Guardar').prop('disabled', false);
    var idBorrar = $("#txtRegistroid").val();
    if (idBorrar != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar el Usuario " + $('#txtRegistroNombre').val() +" ?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/Usuario/EliminarUsuario', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Usuario Eliminada", "la Usuario se elimino correctamente", "success")
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
    if ($("#txtRegistroid").val() == 0) {
        modoGuardando();
    } else {
        modoEditando();
    }
});

// Focus al input Buscar (Icono lupa)
$('#btn-BuscarBuscador').click(function () {
    $('#txtBusqueda').focus();
});

/////
//TABLA
/////
$("#btn-Actualizar").click(function () {
    ActualizarTabla()
});

$("#tablaId").click(function () {
    paginaTabla = 0
    $('#iconoDescripcion').hide();
    $('#iconoID').show();
    query = (query == 'I_A') ? 'I_D' : 'I_A';
    var nuevoIconoClass = (query == 'I_A') ? 'fa-sort-numeric-down' : 'fa-sort-numeric-down-alt';
    $('#iconoID').removeClass('fa-sort-numeric-down fa-sort-numeric-down-alt').addClass(`${nuevoIconoClass} fa-1x m-1`);
    ActualizarTabla();
});

$("#tablaDescripcion").click(function () {
    paginaTabla = 0
    $('#iconoID').hide();
    $('#iconoDescripcion').show();
    query = (query == 'N_A') ? 'N_D' : 'N_A';
    var nuevoIconoClass = (query == 'N_A') ? 'fas fa-sort-alpha-down' : 'fas fa-sort-alpha-down-alt';
    $('#iconoDescripcion').removeClass('fas fa-sort-alpha-down fas fa-sort-alpha-down-alt').addClass(`${nuevoIconoClass} fa-1x m-1`);
    ActualizarTabla();
});

$("#btn-buscarTabla").click(function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla()
    }
    else {
        $("#filtroslinea").focus();
    }
});
//BOTON EDITAR
$("#tabla tbody").on("click", '.btn-editar', function () {
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', false);
    $('#btn-Eliminar').prop('disabled', false);
    $('#btn-Cancelar').prop('disabled', false);
    $('#txtRegistroNombre').focus();

    var rowIndex = $(this).closest("tr").index();
    var Usuario = dataTabla[rowIndex];
    JQueryAjax_Normal('/Usuario/ListarPorIdUsuario', { Id: Usuario.IdUsuario }, false,
        function (datos) {
            DesplegarInformacionCampos(datos);
        },
        function () {
        }
    );
});



//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var rowIndex = $(this).closest("tr").index();
    var Usuario = dataTabla[rowIndex];
    console.log(Usuario.IdUsuario)
    console.log(Usuario)
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Usuario " + Usuario.Nombre + " ?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Usuario/EliminarUsuario', { id: Usuario.IdUsuario }, true, function (data) {

                if (data.resultado) {
                    ActualizarTabla()
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Usuario Eliminada", "la Usuario se elimino correctamente", "success")
                    }, 250);
                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });

        });

})


/*****************************************************/
//Cosas que tiene que cambiar
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

$('#txtRegistroid').on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});

$('#txtRegistroNombre').on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});

$('#txtcontraseña').on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});

$('#txtcorreo').on('input', function () {
    $('#btn-Guardar').prop('disabled', false);
});




// Ocultar opcionesLista cuando se hace clic fuera de ella
$(document).on('click', function (event) {

    if (!$(event.target).closest('#buscador').length) {
        $('#botonesPaginado').hide();
        opcionesLista.hide();

    }
});


/////////
// Filtro
/////////
$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtroslinea').slideDown();
    } else {
        $('#filtroslinea').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
})

//////////////
//Paginado Tabla
//////////////

function CountPaginadoTabla() {
    var resultado = 0;
    if ($("#checkFiltros").is(':checked')) {
        var where = whereQuery();
        JQueryAjax_Normal('/Usuario/countTablaWhere', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Usuario/CountTabla', {}, false, function (data) {
            resultado = data.registros;
        }, function () { });
    }
    return resultado;
}


function cargartabla() {
    datosTablaPaginado(paginaTabla, 10);
    var divisiona = Math.ceil(CountPaginadoTabla() / 10);
    configurarPaginacion(paginaTabla, divisiona, datosTablaPaginado, "T", 10)
}


function whereQuery() {
    var query = ""
    if ($("#checkFiltroId").prop('checked')) {
        query += "u.IdUsuario LIKE '%" + $("#txtFiltroIDWhere").val() + "%'"
    }
    if ($("#checkFiltroNombre").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "u.Nombre LIKE '%" + $("#txtFiltroNombreWhere").val() + "%'"
    }
    if ($("#checkFiltroCorreo").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "u.Correo LIKE '%" + $("#txtFiltroCorreoWhere").val() + "%'"
    }
    return query
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

function UsuariodorTabla() {
    UsuariorCeldas(0, $("#txtFiltroIDWhere").val(), 'yellow');
    UsuariorCeldas(1, $("#txtFiltroNombreWhere").val(), '#FCC208');
    UsuariorCeldas(2, $("#txtFiltroCorreoWhere").val(), '#00943A');

}

function UsuariorCeldas(columna, filtro, color) {
    $('#tabla tbody tr').each(function () {
        var textoCelda = $(this).find('td:eq(' + columna + ')').text().trim().toLowerCase();
        var filtroMinusculas = filtro.toLowerCase();

        if (textoCelda.includes(filtroMinusculas)) {
            var spanResaltado = $('<span>').html(textoCelda.replace(new RegExp(filtroMinusculas, 'gi'), match => `<span style="background-color: ${color};">${match}</span>`));
            $(this).find('td:eq(' + columna + ')').html(spanResaltado);
        }
    });
}


eventoCheckFiltro($("#checkFiltroId"), $("#txtFiltroIDWhere"));
eventoCheckFiltro($("#checkFiltroNombre"), $("#txtFiltroNombreWhere"));
eventoCheckFiltro($("#checkFiltroCorreo"), $("#txtFiltroCorreoWhere"));


/*BUSCADOR*/
/****************************/
function desplegarPaginacionBuscador(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusqueda').val().toLowerCase();
    JQueryAjax_Normal('/Usuario/elementosPaginacionBuscador', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item);
                listItem.on('click', function () {
                    input.val(item);
                    desplegarUsuarioPorNombre(listItem.text());
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
    JQueryAjax_Normal('/Usuario/countBuscador', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}


///////////
//AL CARGAR
///////////
CargarUltimo();
cargartabla()






$(document).ready(function () {
    JQueryAjax_Normal('/Usuario/tamaño', {}, true, function (campos) {
        console.log(campos)
        debugger;
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
/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />
/// <reference path="../general/jqueryajax.js" />

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

function datosTablaPaginado(paginaActual, num) {
    paginaTabla = paginaActual;
    var siguiente = num;
    var dataTabla = [];
    JQueryAjax_Normal('/Compras/ListarCompra', { pagina: paginaActual, siguientes: num, estatus: $("#estatusSelect").val() }, false, function (data) {
        if (data.data.length > 0) {
            dataTabla = data.data;
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
function llenarTabla(dataTabla) {
    var tbody = $("#tabla tbody");
    tbody.empty();
    $.each(dataTabla, function (index, item) {
        var row = $("<tr>");
        row.append($("<td>").text(item.proveedor));
        row.append($("<td>").text(item.cantidad));
        row.append($("<td>").text(item.precio));
        row.append($("<td>").text(item.idCompra));

        tbody.append(row);
    });
}

// Función para contar el número de registros con un estatus específico
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

// Función para cargar la tabla
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
/////////
//METODOS
///////////
//function Guardar() {
//    var textActivo;
//    if ($('#cboRegistroActivo').val() == "A") {
//        textActivo = "A";
//    } else if ($('#cboRegistroActivo').val() == "O") {
//        textActivo = "O";
//    } else {
//        textActivo = "D";
//    }
//    debugger;
//    var Compra = {
//        IdCompra: $('#txtRegistroidCompra').val(), // Este valor es autonumérico, probablemente no necesites capturarlo al crear una nueva compra.
//        IdFactura: 5050, // Asumiendo que tienes un campo para el ID de factura.
//        Estatus: $('#cboRegistroEstatus').val(), // Valor seleccionado del estado de la compra.
//        Fecha: "06/06/2000", // Asumiendo que tienes un campo de fecha para cuando se realizó la compra.
//        Nota: $('#txtRegistroNota').val(), // Nota adicional de la compra.
//        Proveedor: {
//            RFC: $('#txtRegistroRFCProvedor').val()
//        },
//        Usuario: {
//            IdUsuario: "0" // Este valor debe ser sustituido por el mecanismo que utilices para obtener el ID de usuario actual.
//        }
//    };

//    console.log("--Compra--")
//    console.log(Compra)
//    console.log("--Compra Producto--")

//    JQueryAjax_Normal('/Compras/GuardarCompra', { objeto: Compra }, false, function (data) {
//        //Compra guardar
//        if (Compra.IdCompra == 0) {
//            if (data.resultado != 0) {

//                var CompraProducto = {
//                    IdCompraProducto: 0, // Este valor es autonumérico, probablemente no necesites capturarlo al crear un nuevo producto dentro de una compra.
//                    Cantidad: $('#txtRegistroCantidad').val(),
//                    CostoUnitario: $('#txtRegistroPrecio').val(),
//                    Compra: {
//                        IdCompra: data.resultado // Este campo puede no ser necesario dependiendo de cómo estés manejando la lógica de agregación de productos a compras.
//                    },
//                    Producto: {
//                        IdProducto: $('#txtRegistroidProducto').val()
//                    },
//                    Usuario: {
//                        IdUsuario: "0"
//                    }
//                };
//                JQueryAjax_Normal('/Compras/GuardarCompraProducto', { objeto: CompraProducto }, false, function (data) {
//                }, function () { })


//                /*-------*/
//                $('#txtRegistroid').val(data.resultado);
//                swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")
//                $('#mensajeError').hide();
//            }
//            else {
//                $('#mensajeError').text(data.mensaje);
//                $('#mensajeError').show();
//            }
//        }
//        //Compra editar
//        else {
//            if (data.resultado) {
//                $('#mensajeError').hide();
//                swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
//                $('#mensajeError').hide();
//            }
//            else {
//                $('#mensajeError').text(data.mensaje);
//                $('#mensajeError').show();
//            }
//        }
//    }, function () { })
//}
//// Desplegar
//function DesplegarInformacionCampos(json) {
//    if (json != null) {
//        $('#txtRegistroid').val(json.Lista.IdCompra);
//        $('#txtRegistroNombre').val(json.Lista.Descripcion);
//        if (json.Lista.Activo == "A") {
//            $('#cboRegistroActivo').val("A");
//        } else if (json.Lista.Activo == "O") {
//            $('#cboRegistroActivo').val("O");
//        } else {
//            $('#cboRegistroActivo').val("D");
//        }
//        $('#txtRegistrodescripcion').val(json.Lista.Deslc);
//    }
//}
//function desplegarCompraPorNombre(texto) {
//    JQueryAjax_Normal('/Compra/bucarCompraPorNombre', { nombre: texto }, true, function (data) {
//        if (data.Descripcion != "") {
//            $('#txtRegistroNombre').val(data.Descripcion);
//            $('#txtRegistrodescripcion').val(data.Deslc);
//            $('#txtRegistroid').val(data.IdCompra);
//            if (data.Activo == "A") {
//                $('#cboRegistroActivo').val("A");
//            } else if (data.Activo == "O") {
//                $('#cboRegistroActivo').val("O");
//            } else {
//                $('#cboRegistroActivo').val("D");
//            }
//            $('#opcionesLista').hide();
//            $('#botonesPaginado').hide();
//        } else {
//            limpiar()
//        }
//    }, function () { }
//    );
//}
/////////////////
/////Carga el ultimo
/////////////////
//function CargarUltimo() {
//    $('#txtRegistroNombre').focus();

//    JQueryAjax_Normal('/Compra/UltimoRegistro', {}, true, function (data) {
//        DesplegarInformacionCampos(data);
//    }, function () { });
//}
//////////////////
///// BUSCADOR
/////////////////
//function desplegarPaginacionBuscador(pagina, NumeroDivisiones) {
//    var searchTerm = $('#txtBusqueda').val().toLowerCase();
//    JQueryAjax_Normal('/Compra/elementosPaginacionBuscador', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
//        var lista = data.Lista;
//        opcionesLista.empty();
//        if (lista.length > 0) {
//            lista.forEach(function (item) {
//                var listItem = $('<div class="autocomplete-option"></div>').text(item);
//                listItem.on('click', function () {
//                    input.val(item);
//                    desplegarCompraPorNombre(listItem.text());
//                });
//                opcionesLista.append(listItem);
//            });
//            opcionesLista.show();
//            $('#botonesPaginado').show();
//        }
//    }, function () { });
//}
//function CountPaginadoBuscador(texto, num) {
//    var division = 0;
//    JQueryAjax_Normal('/Compra/countBuscador', { nombre: texto }, false, function (data) {
//        division = Math.ceil(data.registros / num);
//    }, function () { });
//    return division;
//}
//////////////
//Paginado Tabla
//////////////
//////////
//BOTONES
//////////

function limpiar() {
    $('#txtRegistroFechaEstimadaEntrega').val('');
    $('#tblDatosProductos .txtNoParte').val('');
    $('#tblDatosProductos .txtPrecio').val('');
    $('#tblDatosProductos .txtCantidad').val('');
    $('#tblDatosProductos .cboProveedor').val('');
    $('#mensajeError').hide();
}
//Barra de heramientas 
$("#btn-Nuevo").click(function () {
    limpiar();
    $('#btn-Guardar').prop('disabled', false);
    $('#btn-Nuevo').prop('disabled', true);
    //$('#btn-Eliminar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', false);
    modoGuardando()


    $('#txtRegistroFechaEstimadaEntrega').prop('disabled', false);
    $('#tblDatosProductos .txtNoParte').prop('disabled', false);
    $('#tblDatosProductos .cboProveedor').prop('disabled', false);
    $('#tblDatosProductos .txtPrecio').prop('disabled', false);
    $('#tblDatosProductos .txtCantidad').prop('disabled', false);
    $('#tblDatosProductos .eliminar-orden').removeClass('disabled'); 


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

        $('#btn-Guardar').prop('disabled', true);
        $('#btn-Cancelar').prop('disabled', true);
        $('#btn-Nuevo').prop('disabled', false);
        Guardar();
        ActualizarTabla();

        $(".bg-edicion").removeClass("bg-edicion");
        $(".bg-guardar").removeClass("bg-guardar");
        Navegar()

        $('#modo').hide();

    
});
//$("#btn-Eliminar").click(function () {
//    $('#btn-Guardar').prop('disabled', false);
//    var idBorrar = $("#txtRegistroid").val();
//    if (idBorrar != 0) {
//        swal({
//            title: "¿Está seguro?",
//            text: "¿Desea eliminar la línea?",
//            type: "warning",
//            showCancelButton: true,
//            confirmButtonClass: "btn-primary",
//            confirmButtonText: "Sí",
//            cancelButtonText: "No",
//            closeOnConfirm: true
//        }, function () {
//            JQueryAjax_Normal('/Marca/EliminarMarca', { id: idBorrar }, false, function (data) {
//                if (data.resultado) {
//                    ActualizarTabla();
//                    CargarUltimo();
//                    modoEditando()
//                } else {
//                    swal("No se pudo eliminar", data.mensaje, "error");
//                }
//            }, function () { });
//        });
//    }
//});

//$('[data-requerido]').on('change input', function () {
//    $('#btn-Cancelar').prop('disabled', false);
//    $('#btn-Guardar').prop('disabled', false);
//    if ($("#txtRegistroid").val() == 0) {
//        modoGuardando();
//    } else {
//        modoEditando();
//    }
//});
$("#tablaId").click(function () {
    paginaTabla = 0;
    $('#iconoDescripcion').hide();
    $('#iconoID').show();
    query = (query == 'I_A') ? 'I_D' : 'I_A';
    var nuevoIconoClass = (query == 'I_A') ? 'fa-sort-numeric-down' : 'fa-sort-numeric-down-alt';
    $('#iconoID').removeClass('fa-sort-numeric-down fa-sort-numeric-down-alt').addClass(`${nuevoIconoClass} fa-2x m-2`);
    ActualizarTabla();
});
$("#tablaDescripcion").click(function () {
    paginaTabla = 0
    $('#iconoID').hide();
    $('#iconoDescripcion').show();
    query = (query == 'D_A') ? 'D_D' : 'D_A';
    var nuevoIconoClass = (query == 'D_A') ? 'fas fa-sort-alpha-down' : 'fas fa-sort-alpha-down-alt';
    $('#iconoDescripcion').removeClass('fas fa-sort-alpha-down fas fa-sort-alpha-down-alt').addClass(`${nuevoIconoClass} fa-2x m-2`);
    ActualizarTabla()
});
$("#btn-Eliminar").click(function () {
    $('#btn-Guardar').prop('disabled', false);
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
            JQueryAjax_Normal('/Compra/EliminarCompra', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimo();
                    modoEditando()

                } else {
                    swal("No se pudo eliminar", data.mensaje, "error");
                }
            }, function () { });
        });
    }
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
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', false);
    modoEditando()
    $('#txtRegistroNombre').focus();

    var rowIndex = $(this).closest("tr").index();
    var Compra = dataTabla[rowIndex];
    JQueryAjax_Normal('/Compra/ListarPorIdCompras', { Id: Compra.idCompra }, false,
        function (data) {
            DesplegarInformacionCampos(data);
        }, function () { });
});

//BOTON ELIMINAR
$("#tabla").on("click", ".btn-eliminar", function () {

    var rowIndex = $(this).closest("tr").index(); // Obtener el índice de la fila
    var Compra = dataTabla[rowIndex]; // Obtener el objeto de la fila correspondiente
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Compra?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Compra/EliminarCompra', { id: Compra.idCompra }, true, function (data) {
                if (data.resultado) {
                    dataTabla.splice(rowIndex, 1);
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

async function ActualizarTabla() {
    try {
        datosTablaPaginado(paginaTabla, 10);
        let totalRegistros = await CountPaginadoTabla();
        var divisiona = Math.ceil(totalRegistros / 10);
        configurarPaginacion(paginaTabla, divisiona, datosTablaPaginado, "T", 10);
    } catch (error) {
        console.error("Error al cargar la tabla:", error);
    }
}

$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtrosCompra').slideDown();
    } else {
        $('#filtrosCompra').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
});

function whereQuery() {
    var query = ""
    if ($("#checkFiltroId").prop('checked')) {
        query += "l.IdCompra LIKE '%" + $("#txtFiltroIDWhere").val() + "%'"
    }
    if ($("#checkFiltroDescripcion").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "l.Descripcion LIKE '%" + $("#txtFiltroDescripcionWhere").val() + "%'"
    } if ($("#checkFiltroDescripcionCompra").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "lc.Descripcion LIKE '%" + $("#txtFiltroDescripcionCompraWhere").val() + "%'"
    }
    return query
}

$("#btn-buscarTabla").click(function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla()
    }
    else {
        $("#filtrosCompra").focus();
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
                JQueryAjax_Normal('/Compra/ChecarConexion', {}, true, function (data) {
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



//___________
//BUSCADOR RAZON SOCIAL PROVEEDOR
function CountPaginadoBuscadorProveedor(texto, num) {
    debugger;
    var division = 0;
    JQueryAjax_Normal('/Proveedor/countBuscador', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}
function desplegarPaginacionBuscadorProveedor(RFC, pagina, NumeroDivisiones) {
    debugger;

    JQueryAjax_Normal('/Compras/elementosPaginacionBuscadorRFC', { RFC: RFC, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.rfc);
                    listItem.on('click', function () {
                        $("#txtRegistroRFCProvedor").val(item.rfc)
                        $("#txtRegistroNombreProvedor").val(item.razonSocial)
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        }, function () { });
}
$("#txtRegistroRFCProvedor").on("input focus", function () {
    debugger;
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var RFC = $('#txtRegistroRFCProvedor').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorProveedor(RFC, pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorProveedor(RFC, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorProveedor, "B", NumeroDivisiones)

});
//___________
//BUSCADOR RAZON SOCIAL PROVEEDOR
function CountPaginadoBuscadorProveedorRazonSocial(texto, num) {
    debugger;
    var division = 0;
    JQueryAjax_Normal('/Compras/countBuscadorRazonSocial', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}
function desplegarPaginacionBuscadorProveedorRazonSocial(RazonSocial, pagina, NumeroDivisiones) {
    debugger;

    JQueryAjax_Normal('/Compras/elementosPaginacionBuscadorRazonSocial', { RazonSocial: RazonSocial, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.razonSocial);
                    listItem.on('click', function () {
                        $("#txtRegistroRFCProvedor").val(item.rfc)
                        $("#txtRegistroNombreProvedor").val(item.razonSocial)
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        }, function () { });
}
$("#txtRegistroNombreProvedor").on("input focus", function () {
    debugger;
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var RFC = $('#txtRegistroNombreProvedor').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorProveedorRazonSocial(RFC, pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorProveedorRazonSocial(RFC, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorProveedorRazonSocial, "B", NumeroDivisiones)

});
//___________
//BUSCADOR ID PROVEEDOR
function CountPaginadoBuscadorProductoID(texto, num) {
    debugger;
    var division = 0;
    JQueryAjax_Normal('/Compras/countProductosID', { where: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}
function desplegarPaginacionBuscadorProductoID(ID, pagina, NumeroDivisiones) {

    JQueryAjax_Normal('/Compras/elementosBuscadorProductosID', { ID: ID, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            console.log(data)
            var lista = data.data;
            opcionesLista.empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.idProducto);
                    listItem.on('click', function () {
                        $("#txtRegistroidProducto").val(item.idProducto)
                        $("#txtRegistroNombreProducto").val(item.descripcion)
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        }, function () { });
}
$("#txtRegistroidProducto").on("input focus", function () {
    debugger;
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var RFC = $('#txtRegistroidProducto').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorProductoID(RFC, pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorProductoID(RFC, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorProductoID, "B", NumeroDivisiones)

});
//___________
//BUSCADOR Descripcion PROVEEDOR
function CountPaginadoBuscadorProductoDescripcion(texto, num) {
    debugger;
    var division = 0;
    JQueryAjax_Normal('/Compras/countProductosDescripcion', { where: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}
function desplegarPaginacionBuscadorProductoDescripcion(ID, pagina, NumeroDivisiones) {
    debugger;

    JQueryAjax_Normal('/Compras/elementosBuscadorProductosDescripcion', { ID: ID, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.descripcion);
                    listItem.on('click', function () {
                        $("#txtRegistroidProducto").val(item.idProducto)
                        $("#txtRegistroNombreProducto").val(item.descripcion)
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        }, function () { });
}
$("#txtRegistroNombreProducto").on("input focus", function () {
    debugger;
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var RFC = $('#txtRegistroNombreProducto').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorProductoDescripcion(RFC, pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorProductoDescripcion(RFC, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorProductoDescripcion, "B", NumeroDivisiones)

});
$("#txtRegistroRFCProvedor").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorRFCProvedor");
});

$("#txtRegistroNombreProvedor").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorRazonSocialProvedor");
});
$("#txtRegistroidProducto").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorIDProducto");
});

$("#txtRegistroNombreProducto").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorNombreProducto");
});



//CargarUltimo();
cargartabla()
//eventoCheckFiltro($("#checkFiltroId"), $("#txtFiltroIDWhere"));
//eventoCheckFiltro($("#checkFiltroDescripcion"), $("#txtFiltroDescripcionWhere"));
//eventoCheckFiltro($("#checkFiltroDescripcionCompra"), $("#txtFiltroDescripcionCompraWhere"));




function agregarNuevaFila() {
    var ultimaFila = $('#tblDatosProductos tbody tr:last');
    var inputsVacios = ultimaFila.find('.txtNoParte, .cboProveedor, .txtCantidad, .txtPrecio').filter(function () {
        return $(this).val().trim() === '';
    });

    if (inputsVacios.length > 0) {
        return;
    }

    var nuevaFila = ultimaFila.clone();
    nuevaFila.find('.txtNoParte, .cboProveedor, .txtCantidad, .txtPrecio').val('');

    $('#tblDatosProductos tbody').append(nuevaFila);
    nuevaFila.find('.txtNoParte').focus();
}

$('#tblDatosProductos').on('blur', '.txtCantidad, .txtPrecio', function () {
    if ($(this).val().trim() !== '') {
        agregarNuevaFila();
    }
});

$('#tblDatosProductos').on('keypress', '.txtCantidad', function (event) {
    if (event.which === 13 && $(this).val().trim() !== '') {
        agregarNuevaFila();
    }
});

$('#tblDatosProductos').on('blur', '.cboProveedor', function () {
    if ($(this).val().trim() !== '') {
        agregarNuevaFila();
    }
});

//$('#tblDatosProductos').on('blur', '.txtNoParte', function () {
//    var noParte = $(this).val().trim();
//    if (noParte !== '') {
//        var currentRow = $(this).closest('tr');

//        JQueryAjax_Normal('/Compras/ObtenerCompraDtlPorNoParte', { noParte: noParte }, true, function (data) {
//            if (data.data.length > 0) {
//                var selectProveedor = currentRow.find('.cboProveedor');
//                selectProveedor.empty();

//                data.data.forEach(function (CompraDtl) {
//                    var proveedorRazonSocial = CompraDtl.oProductoProveedor.oProveedor.RazonSocial;
//                    var idProductoProveedor = CompraDtl.oProductoProveedor.IdProductoProveedor;

//                    if (!selectProveedor.find('option[value="' + idProductoProveedor + '"]').length) {
//                        selectProveedor.append(new Option(proveedorRazonSocial, idProductoProveedor));
//                    }
//                });
//            } else {
//                swal("No se encontraron proveedores", "No se encontraron proveedores para el número de parte proporcionado.", "error");
//            }
//        }, function () {
//            swal("Error", "Hubo un problema al obtener los detalles del producto.", "error");
//        });
//    }
//});


$('#tblDatosProductos').on('input', '.txtNoParte', function () {
    var valorNoParte = $(this).val();
    var currentRow = $(this).closest('tr');

    if (valorNoParte.length > 2) {
        // Verifica si ya existe el div 'resultadosBusqueda' en la fila actual
        if (currentRow.find('#resultadosBusqueda').length === 0) {
            // Inserta el div 'resultadosBusqueda' justo debajo del campo de entrada
            $('<div id="resultadosBusqueda"></div>').insertAfter($(this));
        }

        // Realiza la búsqueda de productos
        BusquedaProductoVenta(valorNoParte, currentRow);
    } else {
        // Si el valor es menor o igual a 3 caracteres, elimina el div si existe
        currentRow.find('#resultadosBusqueda').remove();
    }
});
function BusquedaProductoVenta(valorNoParte, currentRow) {
    JQueryAjax_Normal('/Venta/BuscarProductos', { NoParte: valorNoParte }, true, function (data) {
        var resultContainer = currentRow.find('#resultadosBusqueda');
        resultContainer.empty();
        if (data.lista.length > 0) {
            data.lista.forEach(function (producto, index) {
                var resultadoHtml = `
                    <div class="row my-2 resultado-item" data-index="${index}" tabindex="0">
                        <div class="col-6">${producto.noParte}</div>
                        <div class="col-6"><span class="badge bg-primary">${producto.oMarca.descripcion}</span></div>
                    </div>
                `;
                resultContainer.append(resultadoHtml);
            });

            // Añadir eventos de selección y navegación por teclado
            resultContainer.on('click', '.resultado-item', function () {
                seleccionarProducto($(this).data('index'), data.lista, currentRow);

            });

            resultContainer.on('keydown', '.resultado-item', function (e) {
                if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    $(this).next().focus();
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    $(this).prev().focus();
                } else if (e.key === 'Enter') {
                    e.preventDefault();
                    seleccionarProducto($(this).data('index'), data.lista, currentRow);
                }
            });

            // Coloca el foco en el primer elemento de la lista de resultados al presionar Enter en el campo
            currentRow.find('.txtNoParte').on('keydown', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    resultContainer.find('.resultado-item').first().focus();
                }
            });
        } else {
            resultContainer.html('<p>No se encontraron productos.</p>');
        }

    });
}

function seleccionarProducto(index, lista, currentRow) {
    var producto = lista[index];
    currentRow.find('.txtNoParte').val(producto.noParte);
    currentRow.find('#resultadosBusqueda').remove(); 

    JQueryAjax_Normal('/Compras/ObtenerCompraDtlPorNoParte', { noParte: producto.noParte }, true, function (data) {
        if (data.data.length > 0) {

            var selectProveedor = currentRow.find('.cboProveedor');
            selectProveedor.empty();

            data.data.forEach(function (CompraDtl) {
                currentRow.find('.txtPrecio').val(CompraDtl.oProductoProveedor.precio);;
                var proveedorRazonSocial = CompraDtl.oProductoProveedor.oProveedor.razonSocial;
                var idProductoProveedor = CompraDtl.oProductoProveedor.idProductoProveedor;

                if (!selectProveedor.find('option[value="' + idProductoProveedor + '"]').length) {
                    selectProveedor.append(new Option(proveedorRazonSocial, idProductoProveedor));
                }
                selectProveedor.find('option:last').attr('data-precio', CompraDtl.oProductoProveedor.precio);

            });
                selectProveedor.find('option:last').prop('selected', true);
        } else {
            swal("No se encontraron proveedores", "No se encontraron proveedores para el número de parte proporcionado.", "error");
        }
    }, function () {
        swal("Error", "Hubo un problema al obtener los detalles del producto.", "error");
    });
}




$('#tblDatosProductos').on('change', '.cboProveedor', function () {
    var selectedOption = $(this).find('option:selected');
    var precio = selectedOption.data('precio'); // Obtener el valor de data-precio
    var currentRow = $(this).closest('tr');

    currentRow.find('.txtPrecio').val(precio); // Asignar el valor de data-precio al campo de texto
});


 $('#tblDatosProductos').on('click', '.eliminar-orden', function () {
    var rowCount = $('#tblDatosProductos tbody tr').length;
    if (rowCount > 1) {
        $(this).closest('tr').remove();
    } else {
        swal("No se puede eliminar", "Debe haber al menos una fila en la tabla.", "warning");
    }
});




function Guardar() {
    var listaCompraDtl = [];
    var fechaEstimadaEntrega = $('#txtRegistroFechaEstimadaEntrega').val();
    // Validar la fecha estimada de entrega
    if (!fechaEstimadaEntrega) {
        swal("Mal", "La Fecha Estimada de Entrega es Requerida", "error");
        return; // Salir de la función si hay un error
    }

    var hasError = false; // Variable para controlar si hay errores

    $('#tblDatosProductos tbody tr').each(function () {
        var idProductoProveedor = $(this).find('.cboProveedor').val();
        var cantidad = $(this).find('.txtCantidad').val();
        var precio = $(this).find('.txtPrecio').val();

        // Verificar si todos los campos están vacíos
        var todosVacios = !idProductoProveedor && !cantidad && !precio;

        // Si todos los campos están vacíos, no crear el objeto y pasar a la siguiente fila
        if (todosVacios) {
            return; // Continuar con la siguiente iteración
        }

        // Si alguno de los campos está vacío, mostrar un error
        if (!idProductoProveedor || !cantidad || !precio) {
            swal("Mal", "Todos los campos (Proveedor, Cantidad, Precio) son requeridos si alguno está lleno.", "error");
            hasError = true;
            return false; // Salir del each
        }

        // Validar que cantidad y precio sean números válidos
        if (isNaN(cantidad) || isNaN(precio)) {
            swal("Mal", "Cantidad y Precio deben ser números válidos.", "error");
            hasError = true;
            return false; // Salir del each
        }

        var CompraDtl = {
            oProductoProveedor: {
                IdProductoProveedor: parseInt(idProductoProveedor),
            },
            Cantidad: parseInt(cantidad),
            Precio: parseFloat(precio),
            FechaEstimadaEntrega: fechaEstimadaEntrega
        };

        listaCompraDtl.push(CompraDtl);
    });

    // Si hubo un error, no continuar con la llamada AJAX
    if (hasError) {
        return;
    }
    JQueryAjax_Normal('/Compras/GuardarCompra', { listaCompraDtl: listaCompraDtl }, true, function (data) {
        if (data.idcompra) {
            swal("Bien", "ID: " + data.idcompra, "success");

            limpiar();
            Navegar();
            setTimeout(function () {
                location.reload();
            }, 4000); 


        } else {
            swal("Mal", "Error: " + data.mensaje, "error");
        }
    }, function () {
        swal("Error", "Hubo un problema al obtener los detalles del producto.", "error");
    });
}




$("#estatusSelect").on("change", function () {
    ActualizarTabla();
});

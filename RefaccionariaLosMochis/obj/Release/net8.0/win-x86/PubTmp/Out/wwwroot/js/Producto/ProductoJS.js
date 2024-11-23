
var tabladata;
var filaSeleccionada;
var opcionesLista = $('#opcionesLista');
var input = $('#txtBusquedaLinea');
var activarWhere = false;

var paginaTabla = 0;
var order = 'I_A';
var idImagenBorrar = 0;
$("#iconoI, #iconoN, #iconoL, #iconoM").removeClass().addClass("fas fa-sort fa-1x m-1");

var dataTabla;

var inputProveedores

//MOSTRAR DATOS EN LA TABLA
function datosTablaPaginadoTabla(paginaActual, num) {
    paginaTabla = paginaActual
    orden = ponerOrden(order);
    if (activarWhere) {
        var where = whereQuery();
        JQueryAjax_Normal('/Producto/elementosPaginacionTablaProductoWhere', { pagina: paginaTabla, orden: orden, siguientes: num, where: where }, false, function (data) {
            dataTabla = data.lista;
            console.log(data)
        }, function () { });
    } else {
        JQueryAjax_Normal('/Producto/elementosPaginacionTablaProducto', { pagina: paginaTabla, siguientes: num, orden: orden }, false, function (data) {
            dataTabla = data.lista;
            console.log(data)

        }, function () { });

    }
    // Limpia la tabla antes de agregar nuevos datos
    var tbody = document.querySelector("#tabla tbody");
    tbody.innerHTML = '';
    dataTabla.forEach(function (item) {
        var row = tbody.insertRow();
        row.insertCell(0).textContent = item.noParte;
        row.insertCell(1).textContent = item.descripcion;
        row.insertCell(2).textContent = item.oLinea.descripcion;
        row.insertCell(3).textContent = item.oMarca.descripcion;

        row.insertCell(4).textContent = item.precio;

        var imagenCell = row.insertCell(5);
        imagenCell.innerHTML = '<button type="button" class="btn-imagenes btn-sm btn btn-primary "><i class="fas fa-images"></i> Imagenes</button>';


        var accionesCell = row.insertCell(6);
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
//////////////
//Paginado Tabla
//////////////
function CountPaginadoTabla() {
    var resultado = 0;
    if ($("#checkFiltros").is(':checked')) {
        var where = whereQuery();
        JQueryAjax_Normal('/Producto/countTablaWhere', { where: where }, false, function (data) {
            resultado = data.registros;
        }, function () { });
    } else {
        JQueryAjax_Normal('/Producto/countTabla', {}, false, function (data) {
            resultado = data.registros;
        }, function () { });
    }
    return resultado;
}

$("#BuscadorLinea").on('click', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorLinea");
});

$("#BuscadorLinea").on('keydown', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorLinea");
});

$("#BuscadorLinea").on('select', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorLinea");
});
$(document).on('focus', '#BuscadorLinea', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorLinea");
});

$("#BuscadorMarca").on('click', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorMarca");
});

$("#BuscadorMarca").on('keydown', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorMarca");
});

$("#BuscadorMarca").on('select', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorMarca");
});

$(document).on('focus', '#BuscadorMarca', function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BuscadorMarca");
});


$("#txtBusquedaProducto").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#BusquedaProducto");
});
$(".BuscadorProveedor").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo($(this).parent());
});


function cargartabla() {
    datosTablaPaginadoTabla(paginaTabla, 10);
    var divisiona = Math.ceil(CountPaginadoTabla() / 10);
    configurarPaginacion(paginaTabla, divisiona, datosTablaPaginadoTabla, "T", 10);
}

function ActualizarTabla() {
    datosTablaPaginadoTabla(0, $("#cboDivicion-tabla").val());
    var division = Math.ceil(CountPaginadoTabla() / $("#cboDivicion-tabla").val());
    configurarPaginacion(0, division, datosTablaPaginadoTabla, "T", $("#cboDivicion-tabla").val())
}


/*CARGAR IMAGEN*/
function mostrarImagen(input) {
    var container = document.getElementById('imageContainer');

    if (input.files) {
        for (var i = 0; i < input.files.length; i++) {
            var reader = new FileReader();

            reader.onload = function (e) {
                // Crear elemento de imagen y agregar al contenedor
                var img = document.createElement('img');
                img.src = e.target.result;
                img.width = 200;
                img.height = 197;
                img.classList.add('border', 'redorder', 'mx-2', 'my-2', 'img-fluid');

                img.onclick = function () {

                    swal({
                        title: "¿Esta Seguro?",
                        text: "¿Desea eliminar la Imagen?",
                        type: "warning",
                        showCancelButton: true,
                        confirmButtonClass: "btn-primary",
                        confirmButtonText: "Si",
                        cancelButtonText: "No",
                        closeOnConfirm: true
                    },
                        function () {
                            container.removeChild(img);

                        });

                };

                container.appendChild(img);
            };
            reader.readAsDataURL(input.files[i]);
        }
    }
}

function desplegarIMG(IdProducto) {
    $("#miModal").modal("show");

    $.ajax({
        type: "POST",
        url: '/Producto/ListarImagenes',
        data: { Id: IdProducto },
        dataType: "json",
        success: function (data) {
            console.log(data.ids)
            mostrarImagenes2(data.imagenes, data.ids, "#contenedor-imagenes");
        },
        error: function (error) {
            console.log(error);
        },
        dataFilter: function (data, dataType) {
            return data; // No es necesario dividir la respuesta
        }
    });
}
$("#tabla tbody").on("click", '.btn-imagenes', function () {
    var rowIndex = $(this).closest("tr").index();
    var producto = {
        NoParte: dataTabla[rowIndex].NoParte,
        oMarca: {
            Descripcion: dataTabla[rowIndex].oMarca.Descripcion
        }
    };

    let id;
    JQueryAjax_Normal('/Producto/ObtenerIdProductoConNoparteMarca', { producto: producto }, false, function (data) {
        id = data.IdProducto;
        $("#miModal").attr('data-idproducto', id);
        desplegarIMG(id);
    }, function (error) {
        console.log(error);
    });
});


$("#btn-Nuevo").click(function () {
    limpiar();
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', true);
    $('#btn-Eliminar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', false);
    modoGuardando()
    document.getElementById('SpantabDatosGenerales').textContent = '4';
    document.getElementById('SpantabDatosInventario').textContent = '5';
    document.getElementById('SpantabDatosUbicacion').textContent = '3';
    $("#fileProducto").val('');
    $("#imageContainer").empty();
    $("#imageContainerExistente").empty();


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
            document.getElementById('SpantabDatosInventario').textContent = '';

        });



});
$("#btn-Guardar").click(function () {
    if (ValidarCampoVacio($('[data-requerido]'))) {
        if ($('#txtBusquedaLinea').attr('data-idlinea') != 0) {
            JQueryAjax_Normal('/Linea/bucarLineaPorNombre', { nombre: $('#txtBusquedaLinea').val().toLowerCase() }, true, function (data) {
                if (data.Descripcion != undefined) {
                    $('#txtBusquedaLinea').attr('data-idlinea', data.IdLinea);

                } else {
                    swal("No se encontro la Linea", "Utilice el buscador y seleccione una opcion", "error")


                }
            }, function () { }
            );
        }

        if ($('#txtBusquedaMarca').attr('data-idmarca') != 0) {
            JQueryAjax_Normal('/Marca/bucarMarcaPorNombre', { nombre: $('#txtBusquedaMarca').val().toLowerCase() }, true, function (data) {
                if (data.Descripcion != undefined) {
                    $('#txtBusquedaMarca').attr('data-idmarca', data.IdMarca);

                } else {
                    swal("No se encontro la Marca", "Utilice el buscador y seleccione una opcion", "error")


                }
            }, function () { }
            );
        }

        if ($('#txtBusquedaMarca').attr('data-idmarca') != 0) {
            if ($('#txtBusquedaLinea').attr('data-idlinea') != 0) {
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
            else {
                swal("Faltan campos", "Seleccione una linea", "info");
                $('#txtBusquedaLinea').focus();
            }
        }
        else {
            swal("Faltan campos", "Seleccione una marca", "info");
            $('#txtBusquedaMarca').focus();

        }

    }
});
$("#btn-Eliminar").click(function () {
    $('#btn-Guardar').prop('disabled', false);
    var idBorrar = $("#txtRegistroid").val();
    if (idBorrar != 0) {
        swal({
            title: "¿Está seguro?",
            text: "¿Desea eliminar el Producto " + $("#txtRegistroNombre").val() + " ?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Sí",
            cancelButtonText: "No",
            closeOnConfirm: true
        }, function () {
            JQueryAjax_Normal('/Producto/EliminarProducto', { id: idBorrar }, false, function (data) {
                if (data.resultado) {
                    ActualizarTabla();
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Producto Eliminada", "la Producto se elimino correctamente", "success")
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



//ABRIR MODAL
function abrirModal(json) {
    $('#txtid').val(0);
    $('#img_producto').removeAttr("src");
    $("#fileProducto").val('');
    $("#imageContainer").empty();
    $('#txtnombre').val("");
    $('#txtdescripcion').val("");
    $('#cbomarca').val($('#cbomarca option:first').val());
    $('#cbolinea').val($('#cbolinea option:first').val());
    $('#txtprecio').val("");
    $('#txtmaximo').val("");
    $('#txtminimo').val("");
    $('#cboactivo').val("A");
    $('#mensajeError').hide();


    if (json != null) {

        var textActiv;
        if (json.Activo == "A") {
            textActiv = "A";
        } else if (json.Activo == "O") {
            textActiv = "O";
        } else {
            textActiv = "D";
        }

        $('#txtid').val(json.IdProducto);
        $('#txtnombre').val(json.Descripcion);
        $('#txtdescripcion').val(json.Valor);
        $('#cbomarca').val(json.oMarca.IdMarca);
        $('#cbolinea').val(json.oLinea.IdLinea);
        $('#cboactivo').val(textActiv);
        $('#txtprecio').val(json.Precio);
        $('#txtminimo').val(json.Minimo);
        $('#txtmaximo').val(json.Maximo);



    }
    $("#FormModal").modal("show");
}
//BOTON EDITAR
$("#tabla tbody").on("click", '.btn-editar', function () {
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', false);
    $('#btn-Eliminar').prop('disabled', false);
    $('#btn-Cancelar').prop('disabled', false);
    $('#txtRegistroNombre').focus();

    var rowIndex = $(this).closest("tr").index();
    var data = dataTabla[rowIndex];
    BuscarProductoPorNoParteYMarcaDescripcion(data.noParte, data.oMarca.descripcion)
    $("#fileProducto").val('');
    $("#imageContainer").empty();
})
//BOTON ELIMINAR
$("#tabla tbody").on("click", '.btn-eliminar', function () {
    var rowIndex = $(this).closest("tr").index();
    var Producto = dataTabla[rowIndex];
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Producto " + Producto.Descripcion + " ?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Producto/EliminarProducto', { id: Producto.IdProducto }, true, function (data) {

                if (data.resultado) {
                    dataTabla.splice(rowIndex, 1);
                    ActualizarTabla();
                    CargarUltimo();
                    setTimeout(function () {
                        swal("Producto Eliminada", "la Producto se elimino correctamente", "success")
                    }, 250);
                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });
        });


})
//


function Guardar() {
    var textActivo;
    var comp = $('#cboactivo').val();

    if (comp == "A") {
        textActivo = "A";
    } else if (comp == "O") {
        textActivo = "O";
    } else {
        textActivo = "D";
    }
    var Producto = {
        IdProducto: $('#txtRegistroid').val(),
        Descripcion: $('#txtRegistroNombre').val().toUpperCase(),
        Precio: $("#txtRegistroPrecio").val(),
        Minimo: $("#txtRegistroMinimo").val(),
        Maximo: $("#txtRegistroMaximo").val(),
        NoParte: $("#txtRegistroNoParte").val().toUpperCase(),
        CodigoBarras: $("#txtRegistroCodigoBarra").val().toUpperCase(),
        oMarca: {
            IdMarca: $('#txtBusquedaMarca').attr('data-idmarca'),
            Descripcion: $("#txtBusquedaMarca").val().toUpperCase()
        },
        oLinea: {
            IdLinea: $('#txtBusquedaLinea').attr('data-idlinea'),
            Descripcion: $("#txtBusquedaLinea").val().toUpperCase()
        },
        Activo: textActivo,
        Valor: $('#txtRegistroDescripcion').val().toUpperCase(),
        oUsuario: {
            IdUsuario: "10000"
        },
        oAlmacen: {
            AlmacenId: $("#inputAlmacen").data('id')
        },
        oRack: {
            RackId: $("#inputRack").data('id')
        },
        oSeccion: {
            SeccionId: $("#inputSeccion").data('id')
        }
    };

    JQueryAjax_Normal('/Producto/GuardarProducto', { objeto: Producto }, false,
        function (data) {

            if (Producto.IdProducto == 0) {
                if (data.IdGenerado != 0) {
                    $('#txtRegistroid').val(data.IdGenerado);
                    swal("Registro Guardado", "Se guardo el registro de " + $('#txtRegistroNombre').val(), "success")
                    $('#mensajeError').hide();
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();
                }
            }
            //Producto editar
            else {
                if (data.resultado) {
                    $('#mensajeError').hide();
                    swal("Cambios Guardados", "Se guardaron los cambios de " + $('#txtRegistroNombre').val(), "success")
                }
                else {
                    $('#mensajeError').text(data.mensaje);
                    $('#mensajeError').show();
                    var NumeroDivisiones = 5;
                    if ($('#botonesPaginado').html().trim().length === 0) {
                        NumeroDivisiones = 5
                    } else {
                        NumeroDivisiones = $("#cboDivicion-buscador").val()
                    }
                    var searchTerm = $('#txtFiltroMarcaWhere').val().toLowerCase();
                    pagina = 0;
                    desplegarPaginacionBuscadorMarcaFiltro(pagina, NumeroDivisiones);
                    var division = CountPaginadoBuscadorMarcaFiltro(searchTerm, NumeroDivisiones);
                    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorMarcaFiltro, "B", NumeroDivisiones)
                }

            }



        }, function () {
            alert("Error")
        });

    if ($("#txtRegistroid").val() != 0) {

        var formData = new FormData();

        var files = $("#fileProducto")[0].files;
        if (files) {

            for (var i = 0; i < files.length; i++) {
                formData.append("imagenes", files[i]);
            }
            $.ajax({
                url: "/Producto/GuardarProductoImagenes?id=" + $("#txtRegistroid").val(),
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                async: false,
                success: function (data) {
                },
                error: function (error) {
                    // Manejar errores
                }
            });
            $("#fileProducto").val('');
            $("#imageContainer").empty();
            $.ajax({
                type: "POST",
                url: '/Producto/ListarImagenes',
                data: { Id: $("#txtRegistroid").val() },
                dataType: "json",
                async: false,

                success: function (data) {
                    mostrarImagenes2(data.imagenes, data.id, "#galeriaIMGExistente");
                },
                error: function (error) {
                    console.log(error);
                },
                dataFilter: function (data, dataType) {
                    return data;
                }
            });
        }
        else {
            swal("No se pudo guardar imagenes", "no hay imagenes guardadas", "error");

        }
    } else {
        swal("No se pudo guardar imagenes", "Primero antes de guardar las imagenes guarde un producto", "error");

    }

    //$.ajax({
    //    url: "/Producto/GuardarProducto",
    //    type: "POST",
    //    data: { formData },
    //    processData: false,
    //    contentType: false,
    //    success: function (data) {
    //        // Verificar si se recibió la imagen base64

    //    },
    //    error: function (error) {
    //        // Manejar errores

    //    }
    //});



}
//////////////////////
//SELECCIONAR LINEA

////////////////
/// BUSCADOR DE LINEAS
///////////////

//function CountPaginadoBuscadorLinea(texto, num) {
//    var division = 0;
//    JQueryAjax_Normal('/Linea/countBuscador', { nombre: texto }, false, function (data) {
//        division = Math.ceil(data.registros / num);
//    }, function () { });
//    return division;
//}
//function desplegarPaginacionBuscadorLinea(pagina, NumeroDivisiones) {
//    var searchTerm = $('#txtBusquedaLinea').val().toLowerCase();
//    JQueryAjax_Normal('/Producto/LineaElementosPaginacionBuscadorDescripcionID', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true,
//        function (data) {
//            var lista = data.Lista;
//            opcionesLista.empty();
//            if (lista.length > 0) {
//                lista.forEach(function (item) {
//                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
//                    listItem.on('click', function () {
//                        $('#txtBusquedaLinea').attr('data-idlinea', item.IdLinea);
//                        input.val(item.Descripcion);
//                    });
//                    opcionesLista.append(listItem);
//                });
//                opcionesLista.show();
//                $('#botonesPaginado').show();
//            }
//        }, function () { });
//}
//$('#txtBusquedaLinea').on('input focus', async function () {
//    var NumeroDivisiones = 5;
//    if ($('#botonesPaginado').html().trim().length === 0) {
//        NumeroDivisiones = 5
//    } else {
//        NumeroDivisiones = $("#cboDivicion-buscador").val()
//    }
//    var searchTerm = $('#txtBusquedaLinea').val().toLowerCase();
//    pagina = 0;
//    desplegarPaginacionBuscadorLinea(pagina, NumeroDivisiones);
//    configurarPaginacion(pagina, 100, desplegarPaginacionBuscadorLinea, "B", NumeroDivisiones)

//    var division = await CountPaginadoBuscadorLinea($('#txtBusquedaLinea').val().toLowerCase(), NumeroDivisiones);
//    $('#numPaginadoBuscador').text(pagina + 1 + ' de ' + division);

//});



//Función asincrónica para manejar la entrada de búsqueda
async function handleInputLinea() {
    var NumeroDivisiones = $("#cboDivicion-buscador").val() || 5;
    var pagina = 0;
    desplegarPaginacionBuscadorLinea(pagina, NumeroDivisiones);

    var division = await CountPaginadoBuscadorLinea($('#txtBusquedaLinea').val().toLowerCase(), NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorLinea, "B", NumeroDivisiones);

}

// Aplicando debouncing al evento input
$('#txtBusquedaLinea').on('input', debounce(handleInputLinea, 100));  // 300 ms de espera

// Función para mostrar opciones de paginación de búsqueda de líneas
//async function desplegarPaginacionBuscadorLinea(pagina, NumeroDivisiones) {
//    console.log("BUSCADOR")
//    var searchTerm = $('#txtBusquedaLinea').val().toLowerCase();
//    JQueryAjax_Normal('/Producto/LineaElementosPaginacionBuscadorDescripcionID', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true,
//        function (data) {
//            var lista = data.Lista;
//            opcionesLista.empty();
//            if (lista.length > 0) {
//                lista.forEach(function (item) {
//                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
//                    listItem.on('click', function () {
//                        $('#txtBusquedaLinea').val(item.Descripcion);
//                        $('#txtBusquedaLinea').attr('data-idlinea', item.IdLinea);

//                    });
//                    opcionesLista.append(listItem);
//                });
//                opcionesLista.show();
//                $('#botonesPaginado').show();
//            }
//        }, function () { });
//}


let currentFocus = -1;

async function desplegarPaginacionBuscadorLinea(pagina, NumeroDivisiones) {
    console.log("BUSCADOR");
    var searchTerm = $('#txtBusquedaLinea').val().toLowerCase();
    var opcionesLista = $('#opcionesLista'); // Asegúrate de tener este elemento en el DOM

    JQueryAjax_Normal(
        '/Producto/LineaElementosPaginacionBuscadorDescripcionID',
        { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones },
        true,
        function (data) {
            var lista = data.Lista;
            console.log(lista);
            opcionesLista.empty();
            currentFocus = -1; // Resetear el foco

            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                    listItem.on('click', function () {
                        selectItem(item);
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        },
        function (error) {
            console.error("Error al obtener los datos:", error);
        }
    );
}

function selectItem(item) {
    $('#txtBusquedaLinea').val(item.Descripcion);
    $('#txtBusquedaLinea').attr('data-idlinea', item.IdLinea);
    $('#opcionesLista').hide();
}

// Manejador de eventos para el input de búsqueda
$('#txtBusquedaLinea').on('keydown', function (e) {
    var opcionesLista = $('#opcionesLista');
    var opciones = opcionesLista.find('.autocomplete-option');

    if (e.key === "ArrowDown") {
        currentFocus++;
        addActive(opciones);
    } else if (e.key === "ArrowUp") {
        currentFocus--;
        addActive(opciones);
    } else if (e.key === "Enter") {
        e.preventDefault();
        if (currentFocus > -1) {
            opciones.eq(currentFocus).click();
        }
    }
});

function addActive(opciones) {
    if (!opciones) return false;
    removeActive(opciones);
    if (currentFocus >= opciones.length) currentFocus = 0;
    if (currentFocus < 0) currentFocus = opciones.length - 1;
    opciones.eq(currentFocus).addClass("autocomplete-active");
}

function removeActive(opciones) {
    opciones.removeClass("autocomplete-active");
}







// Función para contar la paginación según la búsqueda de línea
async function CountPaginadoBuscadorLinea(texto, num) {
    var division = 0;
    await JQueryAjax_Normal('/Linea/countBuscador', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}



// Ocultar opcionesLista cuando se hace clic fuera de ella
$(document).on('click', function (event) {

    if (!$(event.target).closest('#buscador').length) {
        $('#botonesPaginado').hide();
        opcionesLista.hide();
    }
});

// Definir una variable para almacenar el temporizador del debounce

//$('#txtBusquedaMarca').on('input focus', async function () {
//    var NumeroDivisiones = $("#cboDivicion-buscador").val() || 5;
//    var pagina = 0;
//    var searchTerm = $('#txtBusquedaMarca').val().toLowerCase();
//    pagina = 0;
//    desplegarPaginacionBuscadorMarca(pagina, NumeroDivisiones);
//    var division = CountPaginadoBuscadorMarca(searchTerm, NumeroDivisiones);
//    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorMarca, "B", NumeroDivisiones)

//    var division = await CountPaginadoBuscadorMarca($('#txtBusquedaMarca').val().toLowerCase(), NumeroDivisiones);
//    $('#numPaginadoBuscador').text(pagina + 1 + ' de ' + division);
//});

//function desplegarPaginacionBuscadorMarca(pagina, NumeroDivisiones) {
//    var searchTerm = $('#txtBusquedaMarca').val().toLowerCase();
//    JQueryAjax_Normal('/Producto/MarcaElementosPaginacionBuscadorDescripcionID', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
//        var lista = data.Lista;
//        opcionesLista.empty();
//        if (lista.length > 0) {
//            lista.forEach(function (item) {
//                var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
//                listItem.on('click', function () {
//                    $('#txtBusquedaMarca').val(item.Descripcion);
//                    $('#txtBusquedaMarca').attr('data-idmarca', item.IdMarca);

//                });
//                opcionesLista.append(listItem);
//            });
//            opcionesLista.show();
//            $('#botonesPaginado').show();
//        }
//    }, function () { });
//}
//function CountPaginadoBuscadorMarca(texto, num) {
//    var division = 0;
//    JQueryAjax_Normal('/Marca/CountBuscador', { nombre: texto }, false, function (data) {
//        division = Math.ceil(data.registros / num);
//    }, function () { });
//    return division;
//}

function debounce(func, wait) {
    let timeout;
    return function () {
        const context = this, args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(context, args), wait);
    };
}

//// Función que se ejecuta después de que el usuario deja de escribir
async function handleInput() {



    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var searchTerm = $('#txtBusquedaMarca').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorMarca(pagina, NumeroDivisiones);
    var division = await CountPaginadoBuscadorMarca(searchTerm, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorMarca, "B", NumeroDivisiones)


}

$('#txtBusquedaMarca').on('input', debounce(handleInput, 200)); // 300 ms de espera

let currentFocusMarca = -1;

async function desplegarPaginacionBuscadorMarca(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusquedaMarca').val().toLowerCase();

    JQueryAjax_Normal(
        '/Producto/MarcaElementosPaginacionBuscadorDescripcionID',
        { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones },
        true,
        function (data) {
            console.log()
            var lista = data.Lista;
            opcionesLista.empty();
            currentFocusMarca = -1; // Resetear el foco

            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                    listItem.on('click', function () {
                        selectItemMarca(item);
                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        },
        function (error) {
            console.error("Error al obtener los datos:", error);
        }
    );
}

function selectItemMarca(item) {
    $('#txtBusquedaMarca').val(item.Descripcion);
    $('#txtBusquedaMarca').attr('data-idmarca', item.IdMarca);
    $('#opcionesLista').hide();
}

// Manejador de eventos para el input de búsqueda de marca
$('#txtBusquedaMarca').on('keydown', function (e) {
    var opcionesLista = $('#opcionesLista');
    var opciones = opcionesLista.find('.autocomplete-option');

    if (e.key === "ArrowDown") {
        currentFocusMarca++;
        addActiveMarca(opciones);
    } else if (e.key === "ArrowUp") {
        currentFocusMarca--;
        addActiveMarca(opciones);
    } else if (e.key === "Enter") {
        e.preventDefault();
        if (currentFocusMarca > -1) {
            opciones.eq(currentFocusMarca).click();
        }
    }
});

function addActiveMarca(opciones) {
    if (!opciones) return false;
    removeActiveMarca(opciones);
    if (currentFocusMarca >= opciones.length) currentFocusMarca = 0;
    if (currentFocusMarca < 0) currentFocusMarca = opciones.length - 1;
    opciones.eq(currentFocusMarca).addClass("autocomplete-active");
}

function removeActiveMarca(opciones) {
    opciones.removeClass("autocomplete-active");
}

async function CountPaginadoBuscadorMarca(texto, num) {

    var division = 0;
    await JQueryAjax_Normal('/Marca/CountBuscador', { nombre: texto }, false, function (data) {

        division = Math.ceil(data.registros / num);
    }, function () { });


    return division;
}



$('#txtBusquedaMarca').blur(function () {
    setTimeout(function () {  // Inicia el temporizador para esperar 40 segundos
        if ($('#txtBusquedaMarca').attr('data-idmarca') != 0) {
            JQueryAjax_Normal('/Marca/bucarMarcaPorNombre', { nombre: $('#txtBusquedaMarca').val().toLowerCase() }, true, function (data) {
                if (data.Descripcion != undefined) {
                    $('#txtBusquedaMarca').attr('data-idmarca', data.IdMarca);

                } else {
                    swal("No se encontro la Marca", "Utilice el buscador y seleccione una opcion", "error")


                }
            }, function () { }
            );
        }
    }, 9000);
});
$('#txtBusquedaLinea').blur(function () {
    setTimeout(function () {  // Inicia el temporizador para esperar 40 segundos
        if ($('#txtBusquedaLinea').attr('data-idlinea') != 0) {
            JQueryAjax_Normal('/Linea/bucarLineaPorNombre', { nombre: $('#txtBusquedaLinea').val().toLowerCase() }, true, function (data) {
                if (data.Descripcion != undefined) {
                    $('#txtBusquedaLinea').attr('data-idlinea', data.IdLinea);

                } else {
                    swal("No se encontro la Linea", "Utilice el buscador y seleccione una opcion", "error")


                }
            }, function () { }
            );
        }
    }, 9000);
});


$('#inputAlmacen').blur(function () {
    setTimeout(function () {  // Inicia el temporizador para esperar 40 segundos
        if ($('#inputAlmacen').attr('data-id') != 0) {
            JQueryAjax_Normal('/Almacen/bucarAlmacenPorUbicacion', { Ubicacion: $('#inputAlmacen').val().toLowerCase() }, true, function (data) {
                console.log(data)
                if (data.Lista.Descripcion != undefined) {
                    $('#inputAlmacen').attr('data-id', data.Lista.AlmacenId);
                } else {
                    $('#inputAlmacen').attr('data-id', -1);
                    swal("No se encontro la Almacen", "Utilice el buscador y seleccione una opcion", "error")


                }
            }, function () { }
            );
        }
    }, 4000);
});

$('#inputRack').blur(function () {
    setTimeout(function () {  // Inicia el temporizador para esperar 40 segundos
        if ($('#inputRack').attr('data-id') != 0) {
            JQueryAjax_Normal('/Almacen/bucarRackPorUbicacion', { Ubicacion: $('#inputRack').val().toLowerCase() }, true, function (data) {
                console.log(data)
                if (data.Lista.Descripcion != undefined) {
                    $('#inputRack').attr('data-id', data.Lista.RackId);
                } else {
                    $('#inputRack').attr('data-id', -1);
                    swal("No se encontro la Rack", "Utilice el buscador y seleccione una opcion", "error")


                }
            }, function () { }
            );
        }
    }, 4000);
});
$('#inputSeccion').blur(function () {
    setTimeout(function () {  // Inicia el temporizador para esperar 40 segundos
        if ($('#inputSeccion').attr('data-id') != 0) {
            JQueryAjax_Normal('/Almacen/bucarSeccionPorUbicacion', { Ubicacion: $('#inputSeccion').val().toLowerCase() }, true, function (data) {
                console.log(data)
                if (data.Lista.Descripcion != undefined) {
                    $('#inputSeccion').attr('data-id', data.Lista.SeccionId);
                } else {
                    $('#inputSeccion').attr('data-id', -1);
                    swal("No se encontro la Seccion", "Utilice el buscador y seleccione una opcion", "error")


                }
            }, function () { }
            );
        }
    }, 4000);
});

$(".ubicacion").change(function () {
    modoEditando();
    $('#btn-Cancelar').prop('disabled', false);
    $('#btn-Guardar').prop('disabled', false);
})

//// BUSCADOR PROVEEDOR 
function CountPaginadoBuscadorProveedor(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Compras/countBuscadorRazonSocial', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}
function desplegarPaginacionBuscadorProveedor(RazonSocial, pagina, NumeroDivisiones) {

    JQueryAjax_Normal('/Compras/elementosPaginacionBuscadorRazonSocial', { RazonSocial: RazonSocial, pagina: pagina, siguientes: NumeroDivisiones }, true,
        function (data) {
            var lista = data.data;
            opcionesLista.empty();
            if (lista.length > 0) {
                lista.forEach(function (item) {
                    var listItem = $('<div class="autocomplete-option"></div>').text(item.RazonSocial);
                    listItem.on('click', function () {
                        $(inputProveedores).val(item.RazonSocial);
                        $(inputProveedores).attr("data-idproveedor", item.RFC);

                    });
                    opcionesLista.append(listItem);
                });
                opcionesLista.show();
                $('#botonesPaginado').show();
            }
        }, function () { });
}
function BuscadorDeProveedores() {
    inputProveedores = this;
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var RFC = $("#txtRegistroProveedor").val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorProveedor(RFC, pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorProveedor(RFC, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorProveedor, "B", NumeroDivisiones)

}
$("#txtRegistroProveedor").on("input focus", BuscadorDeProveedores);



$('#btnMostrarImagenes').on('click', function () {
    // Hacer la solicitud Ajax para obtener la lista de imágenes
    $.ajax({
        url: '/Producto/ObtenerImagenesDesdeBaseDeDatos?idProducto=' + $("#txtprueba").val(), // Ajusta la URL según tu configuración
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data)
            mostrarImagenes(data.Imagenes);
        },
        error: function () {
            // Manejar errores
            alert('Error al obtener las imágenes.');
        }
    });
});


function mostrarImagenes(imagenes) {
    // Limpiar el contenedor antes de agregar nuevas imágenes
    $('.carousel-inner').empty();
    $('.carousel-indicators').empty();

    var isFirst = true; // Booleano para rastrear el primer elemento

    // Recorrer la lista de imágenes y agregarlas al carrusel
    $.each(imagenes, function (index, imagen) {
        var imgElement = $('<div class="carousel-item">').append($('<img class="d-block w-100">').attr('src', 'data:image/png;base64,' + imagen));

        // Agregar la clase 'active' solo al primer elemento
        if (isFirst) {
            imgElement.addClass('active');
        }

        // Agregar la imagen al carrusel
        $('.carousel-inner').append(imgElement);
        // Agregar un indicador (bullet) para cada imagen
        var indicator = $('<li data-target="#contenedor-imagenes" data-slide-to="' + index + '">' + index + '</li>');
        if (isFirst) {
            indicator.addClass('active');
            isFirst = false; // Cambiar el booleano después de agregar la clase 'active' una vez

        }
        $('.carousel-indicators').append(indicator);
    });

    // Mostrar el modal con el carrusel
    $("#miModal").modal("show");
}



function mostrarImagenes2(imagenes, ids, contenedor) {
    $(contenedor).empty();
    // Recorrer la lista de imágenes y agregarlas a divs redondeados
    console.log(ids)
    $.each(imagenes, function (index, imagen) {
        var id = ids[index]; // Obtener el ID correspondiente al índice actual

        // Crear un div de columna para cada imagen
        var colElement = $('<div class="col-4 mb-3">');

        // Crear un div redondeado de 200px con la imagen dentro
        var imgElement = $('<div class="rounded d-flex justify-content-center align-items-center image-container ">')
            .css('width', '150px')
            .css('height', '100px')
            .attr('data-idimagen', id)
            .append($('<img class="d-block rounded ">')
                .attr('src', 'data:image/png;base64,' + imagen)
                .css('object-fit', 'cover')
                .css('border-radius', '5px')
                .css('width', '100%')
                .css('height', '100%')
            );

        if (contenedor != "#imageContainerExistente") {
            imgElement.on('click', function () {
                $('.border-img').removeClass('border-img');

                $('#mostrarGrande').empty();
                idImagenBorrar = $(this).data('idimagen');
                mostrarImagenEnGrande('data:image/png;base64,' + imagen);

                $(this).addClass('border-img');
            });
        }

        colElement.append(imgElement);

        $(contenedor).append(colElement);
    });
}



// Función para mostrar la imagen en grande
function mostrarImagenEnGrande(imagenSrc) {
    // Limpiar el contenedor antes de agregar la imagen en grande
    $('#mostrarGrande').empty();


    // Crear un div con la imagen en grande
    var imgGrande = $('<img class="img-fluid zoom-img">').attr('src', imagenSrc).css('border-radius', '5px');

    // Agregar la imagen en grande al contenedor
    $('#mostrarGrande').append(imgGrande);

}

///////////////
///Carga el ultimo
///////////////
// Desplegar Información de Productos
function desplegarProductoPorNombre(texto) {

    JQueryAjax_Normal('/Producto/BuscarProductoPorNombre', { nombre: texto }, true, function (data) {

        DesplegarInformacionCampos(data.data);
        cargarImagenesExistente(data.data.IdProducto, "#imageContainerExistente")
    }, function () { }
    );
}
function desplegarProductoPorNoParteYMarca(texto, idmarca) {

    JQueryAjax_Normal('/Producto/BuscarProductoPorNoParteYMarca', { nombre: texto, id: idmarca }, true, function (data) {

        DesplegarInformacionCampos(data.data);
        cargarImagenesExistente(data.data.IdProducto, "#imageContainerExistente")
    }, function () { }
    );
}
function BuscarProductoPorNoParteYMarcaDescripcion(texto, marca) {


    JQueryAjax_Normal('/Producto/dsds', { nombre: texto, marca: marca }, false, function (data) {

        DesplegarInformacionCampos(data.data);
        cargarImagenesExistente(data.data.idProducto, "#imageContainerExistente")
    }, function () { }
    );
}

function DesplegarInformacionCampos(json) {

    if (json != null) {
        $('#txtRegistroid').val(json.idProducto);
        $('#txtRegistroNombre').val(json.descripcion);
        $("#txtRegistroPrecio").val(json.precio);
        $("#txtRegistroMinimo").val(json.minimo);
        $("#txtRegistroMaximo").val(json.maximo);
        $("#txtRegistroNoParte").val(json.noParte);
        $("#txtRegistroCodigoBarra").val(json.codigoBarras);
        $("#txtBusquedaMarca").val(json.oMarca.descripcion);
        $("#txtBusquedaLinea").val(json.oLinea.descripcion);
        $('#cboRegistroActivo').val(json.activo);
        $('#txtRegistroDescripcion').val(json.valor);
        $("#inputAlmacen").val(json.oAlmacen.ubicacion).attr("data-id", json.oAlmacen.almacenId); 
        $("#inputRack").val(json.oRack.ubicacion).attr("data-id", json.oRack.rackId); 
        $("#inputSeccion").val(json.oSeccion.ubicacion).attr("data-id", json.oSeccion.seccionId); 
        $('#txtBusquedaMarca').attr('data-idmarca', json.oMarca.idMarca);
        $('#txtBusquedaLinea').attr('data-idlinea', json.oLinea.idLinea);
    }
}




function CargarUltimo() {
    $('#txtRegistroNombre').focus();
    $("#fileProducto").val('');
    $("#imageContainer").empty();
    $("#imageContainerExistente").empty();

    JQueryAjax_Normal('/Producto/UltimoRegistro', {}, false, function (data) {
        DesplegarInformacionCampos(data.data);
        cargarImagenesExistente(data.data.idProducto)
    }, function () { });
}


function cargarImagenesExistente(id, container) {
    $.ajax({
        type: "POST",
        url: '/Producto/ListarImagenes',
        data: { Id: id },
        dataType: "json",
        success: function (data) {
            mostrarImagenes2(data.imagenes, data.id, imageContainerExistente);
        },
        error: function (error) {
            console.log(error);
            // Manejar errores si es necesario
        },
        dataFilter: function (data, dataType) {
            return data; // No es necesario dividir la respuesta
        }
    });
}

$("#GuardarIMG").click(function () {

    if ($("#txtRegistroid").val() != 0) {

        var formData = new FormData();

        var files = $("#fileProducto")[0].files;
        console.log("Afuera")
        if (files.length >= 1) {
            console.log("Adentro")

            for (var i = 0; i < files.length; i++) {
                formData.append("files", files[i]);
            }
            console.log(formData)
            console.log(files)
            $.ajax({
                url: "/Producto/GuardarProductoImagenes?id=" + $("#txtRegistroid").val(),
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (data) {
                },
                error: function (error) {
                    // Manejar errores
                }
            });
            $("#fileProducto").val('');
            $("#imageContainer").empty();
            $.ajax({
                type: "POST",
                url: '/Producto/ListarImagenes',
                data: { Id: $("#txtRegistroid").val() },
                dataType: "json",
                async: false,

                success: function (data) {
                    console.log(data)
                    mostrarImagenes2(data.imagenes, data.id, "#galeriaIMGExistente");
                },
                error: function (error) {
                    console.log(error);
                },
                dataFilter: function (data, dataType) {
                    return data;
                }
            });
        }
        else {
            swal("No se pudo guardar imagenes", "no hay imagenes guardadas", "error");

        }
    } else {
        swal("No se pudo guardar imagenes", "Primero antes de guardar las imagenes guarde un producto", "error");

    }
});




$('#txtBusquedaProducto').on('input', function () {
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var searchTerm = $('#txtBusquedaProducto').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorProducto(pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorProducto(searchTerm, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorProducto, "B", NumeroDivisiones)


});
function desplegarPaginacionBuscadorProducto(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtBusquedaProducto').val().toLowerCase();
    JQueryAjax_Normal('/Producto/elementosPaginacionBuscadorProducto', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option d-flex justify-content-between">' +
                    '<span>' + item.NoParte + '</span>' +
                    '<span class="badge rounded-pill m-1 bg-success">' + item.oMarca.Descripcion + '</span>' +
                    '</div>');

                listItem.on('click', function () {
                    $('#txtBusquedaProducto').val(item.NoParte);
                    desplegarProductoPorNoParteYMarca(item.NoParte, item.oMarca.IdMarca)
                    $('#botonesPaginado').hide();
                    opcionesLista.hide();
                });
                opcionesLista.append(listItem);
            });
            opcionesLista.show();
            $('#botonesPaginado').show();
        }
    }, function () { });
}
function CountPaginadoBuscadorProducto(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Producto/countBuscadorProducto', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

// Variables globales para el orden y la página

// Función para manejar el clic en el encabezado de ID
$("#tablaId").click(function () {
    actualizarOrden('I');
});

// Función para manejar el clic en el encabezado de Nombre
$("#tablaNombre").click(function () {
    actualizarOrden('N');
});

// Función para manejar el clic en el encabezado de Línea
$("#tablaLinea").click(function () {
    actualizarOrden('L');
});

// Función para manejar el clic en el encabezado de Marca
$("#tablaMarca").click(function () {
    actualizarOrden('M');
});

// Función para actualizar el orden y la tabla
function actualizarOrden(columna) {
    paginaTabla = 0;
    order = (order.includes(columna)) ? invertirOrden(order) : columna + '_A';
    actualizarIconosOrden(columna);
    ActualizarTabla();
}

// Función para invertir el orden (Ascendente a Descendente y viceversa)
function invertirOrden(ordenActual) {
    return ordenActual.endsWith('_A') ? ordenActual.replace('_A', '_D') : ordenActual.replace('_D', '_A');
}

// Función para actualizar los iconos de orden en todos los encabezados
function actualizarIconosOrden(columnaSeleccionada) {
    // Restablecer todos los iconos
    $("#iconoI, #iconoN, #iconoL, #iconoM").removeClass().addClass("fas fa-sort fa-1x m-1");

    // Establecer el icono correcto para la columna seleccionada
    var icono = (order.endsWith('_A')) ? 'fa-sort-alpha-down' : 'fa-sort-alpha-down-alt';
    $(`#icono${columnaSeleccionada}`).removeClass().addClass(`fas ${icono} fa-1x m-1`);
}

function ponerOrden(orden) {
    switch (orden) {
        case 'I_A':
            return "p.NoParte ASC";
        case 'I_D':
            return "p.NoParte DESC";
        case 'N_A':
            return "p.Descripcion ASC";
        case 'N_D':
            return "p.Descripcion DESC";
        case 'M_A':
            return "m.Descripcion ASC";
        case 'M_D':
            return "m.Descripcion DESC";
        case 'L_A':
            return "l.Descripcion ASC";
        case 'L_D':
            return "l.Descripcion DESC";
        default:
            return "p.IdProducto ASC";
    }

}

$("#checkFiltros").click(function () {
    if ($("#checkFiltros").is(':checked')) {
        $('#filtrosProductos').slideDown();
    } else {
        $('#filtrosProductos').slideUp();
        activarWhere = false;
        ActualizarTabla()
    }
});

function whereQuery() {
    var query = ""
    if ($("#checkFiltroId").prop('checked')) {
        query += "p.NoParte LIKE '%" + $("#txtFiltroIDWhere").val() + "%'"
    }
    if ($("#checkFiltroDescripcion").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "p.Descripcion LIKE '%" + $("#txtFiltroDescripcionWhere").val() + "%'"
    }
    if ($("#checkFiltroMarca").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "m.Descripcion LIKE '%" + $("#txtFiltroMarcaWhere").val() + "%'"
    }
    if ($("#checkFiltroLinea").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "l.Descripcion LIKE '%" + $("#txtFiltroLineaWhere").val() + "%'"
    }
    if ($("#checkFiltroCodigoBarras").prop('checked')) {
        if (query.length != 0) {
            query += " OR "
        }
        query += "p.CodigoBarras LIKE '%" + $("#txtFiltroCodigoBarrasWhere").val() + "%'"
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

















$('#txtFiltroMarcaWhere').on('input', function () {
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var searchTerm = $('#txtFiltroMarcaWhere').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorMarcaFiltro(pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorMarcaFiltro(searchTerm, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorMarcaFiltro, "B", NumeroDivisiones)


});
function desplegarPaginacionBuscadorMarcaFiltro(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtFiltroMarcaWhere').val().toLowerCase();
    JQueryAjax_Normal('/Producto/MarcaElementosPaginacionBuscadorDescripcionID', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                listItem.on('click', function () {
                    $('#txtFiltroMarcaWhere').val(item.Descripcion);
                });
                opcionesLista.append(listItem);
            });
            opcionesLista.show();
            $('#botonesPaginado').show();
        }
    }, function () { });
}
function CountPaginadoBuscadorMarcaFiltro(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Marca/countBuscador', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

$("#contenedorFiltroMarca").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#contenedorFiltroMarca");
});


$('#txtFiltroLineaWhere').on('input', function () {
    var NumeroDivisiones = 5;
    if ($('#botonesPaginado').html().trim().length === 0) {
        NumeroDivisiones = 5
    } else {
        NumeroDivisiones = $("#cboDivicion-buscador").val()
    }
    var searchTerm = $('#txtFiltroLineaWhere').val().toLowerCase();
    pagina = 0;
    desplegarPaginacionBuscadorLineaFiltro(pagina, NumeroDivisiones);
    var division = CountPaginadoBuscadorLineaFiltro(searchTerm, NumeroDivisiones);
    configurarPaginacion(pagina, division, desplegarPaginacionBuscadorLineaFiltro, "B", NumeroDivisiones)


});
function desplegarPaginacionBuscadorLineaFiltro(pagina, NumeroDivisiones) {
    var searchTerm = $('#txtFiltroLineaWhere').val().toLowerCase();
    JQueryAjax_Normal('/Producto/LineaElementosPaginacionBuscadorDescripcionID', { nombre: searchTerm, pagina: pagina, siguientes: NumeroDivisiones }, true, function (data) {
        var lista = data.Lista;
        opcionesLista.empty();
        if (lista.length > 0) {
            lista.forEach(function (item) {
                var listItem = $('<div class="autocomplete-option"></div>').text(item.Descripcion);
                listItem.on('click', function () {
                    $('#txtFiltroLineaWhere').val(item.Descripcion);
                });
                opcionesLista.append(listItem);
            });
            opcionesLista.show();
            $('#botonesPaginado').show();
        }
    }, function () { });
}
function CountPaginadoBuscadorLineaFiltro(texto, num) {
    var division = 0;
    JQueryAjax_Normal('/Linea/countBuscador', { nombre: texto }, false, function (data) {
        division = Math.ceil(data.registros / num);
    }, function () { });
    return division;
}

$("#contenedorFiltroLinea").click(function () {
    $("#botonesPaginado, #opcionesLista").appendTo("#contenedorFiltroLinea");
});

$("#btn-buscarTabla").click(function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla()
    }
    else {
        $("#filtrosproducto").focus();
    }
});

function limpiar() {
    $('#txtRegistroNombre').focus();

    $('#txtRegistroid').val("");
    $('#txtRegistroNombre').val("");
    $('#txtRegistroMinimo').val("");
    $('#txtRegistroPrecio').val("");
    $('#txtRegistroMaximo').val("");
    $('#txtBusquedaMarca').val("");
    $('#txtBusquedaLinea').val("");
    $('#cboactivo').val("A");
    $('#txtRegistroDescripcion').val("");
    $('#txtRegistroCodigoBarra').val("");
    $('#txtRegistroNoParte').val("");
    $('#inputAlmacen').val("");
    $('#inputRack').val("");
    $('#inputSeccion').val("");

    $('#txtBusquedaMarca').data('idmarca', '0');
    $('#txtBusquedaLinea').data('idlinea', '0');

}
function limpiarProductosProvedores() {
    $('#txtRegistroProveedor').val("");
    $('#txtRegistroReferencia').val(""); // Este campo asumo que es para detalles de proveedor o producto
    $('#txtRegistroProveedorPrecio').val("");
}
$('#miModal').on('hidden.bs.modal', function (e) {
    idImagenBorrar = 0;
    $('#mostrarGrande').empty();
    $("#miModal").attr('data-idproducto', 0);

    if ($('#SeleccionarImagen').length === 0) {
        // Crear el elemento SeleccionarImagen
        var seleccionarImagen = $('<div>').attr('id', 'SeleccionarImagen')
            .addClass('w-auto rounded-3 d-flex justify-content-center align-items-center');
        // Crear el contenido interno
        var texto = $('<span>').addClass('h3 text-white').text('Seleccione una imagen ');
        var icono = $('<i>').addClass('fas fa-images fa-1x m-1');

        // Agregar el icono al elemento span
        texto.append(icono);

        // Agregar el texto con el icono al elemento SeleccionarImagen
        seleccionarImagen.append(texto);

        // Agregar SeleccionarImagen al elemento mostrarGrande
        $('#mostrarGrande').append(seleccionarImagen);
    }
});
$("#btn-Actualizar").click(function () {
    ActualizarTabla()
});

$("#btn-borrarIMG").click(function () {
    swal({
        title: "¿Esta Seguro?",
        text: "¿Desea eliminar la Imagen?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: true
    },
        function () {
            JQueryAjax_Normal('/Producto/EliminarIMGProducto', { id: idImagenBorrar }, true, function (data) {

                if (data.resultado) {
                    desplegarIMG($("#miModal").data('idproducto'))

                } else {
                    setTimeout(function () {
                        swal("No se pudo eliminar", data.mensaje, "error");
                    }, 250);
                }
            }, function () { });
        }
    );
});

function MarcadorTabla() {
    marcarCeldas(0, $("#txtFiltroIDWhere").val(), 'yellow');
    marcarCeldas(1, $("#txtFiltroDescripcionWhere").val(), '#FCC208');
    marcarCeldas(2, $("#txtFiltroLineaWhere").val(), 'yellow');
    marcarCeldas(3, $("#txtFiltroMarcaWhere").val(), '#FCC208');
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

// Manejo del input de Almacén
$("#inputAlmacen").on("input focus", function () {
    var valor = $(this).val().toLowerCase();
    JQueryAjax_Normal('/Almacen/SeleccionarAlmacen', { ParametroBusqueda: valor }, true, function (data) {
        $("#listaAlmacenes").show().empty();
        if (data.mensaje === "") {
            $.each(data.data, function (index, almacen) {
                if (almacen.Ubicacion.toLowerCase().includes(valor)) {
                    $("#listaAlmacenes").append('<div class="opcion-almacen" data-id="' + almacen.AlmacenId + '">' + almacen.Ubicacion + '</div>');
                }
            });
        } else {
            swal("Se ha producido un error", "Error: " + data.mensaje, "error");
        }
    }, function (errorThrown) {
        console.error("Error al obtener almacenes: " + errorThrown);
    });

});

// Selección de un Almacén
$(document).on("click", ".opcion-almacen", function () {
    var texto = $(this).text();
    var almacenId = $(this).data('id');
    $("#inputAlmacen").val(texto).data('id', almacenId);
    $("#listaAlmacenes").hide();
    $("#inputRack").val('').prop('disabled', false); // Habilita y limpia el input de Rack
    $("#inputSeccion").val('').prop('disabled', true); // Deshabilita y limpia el input de Sección
    $("#inputAlmacen").attr('data-id', almacenId);
});

// Manejo del input de Rack
$("#inputRack").on("input focus", function () {
    var valor = $(this).val().toLowerCase();
    var almacenId = $("#inputAlmacen").data('id');
    if (almacenId) {
        JQueryAjax_Normal('/Almacen/SeleccionarAlmacenRack', { id: almacenId, ParametroBusqueda: valor }, true, function (data) {
            $("#listaRacks").show().empty();
            if (data.mensaje === "") {
                $.each(data.data, function (index, rack) {
                    if (rack.Ubicacion.toLowerCase().includes(valor)) {
                        $("#listaRacks").append('<div class="opcion-rack" data-id="' + rack.RackId + '">' + rack.Ubicacion + '</div>');
                    }
                });
            } else {
                swal("Se ha producido un error", "Error: " + data.mensaje, "error");
            }
        }, function (errorThrown) {
            console.error("Error al obtener racks: " + errorThrown);
        });
    } else {
        swal("Seleccione un almacén primero", "", "warning");
    }
});

// Selección de un Rack
$(document).on("click", ".opcion-rack", function () {
    var texto = $(this).text();
    var rackId = $(this).data('id');
    $("#inputRack").val(texto).data('id', rackId);
    $("#listaRacks").hide();
    $("#inputSeccion").val('').prop('disabled', false); // Habilita el input de Sección
    $("#inputRack").attr('data-id', rackId);

});
// Manejo del input de Sección
$("#inputSeccion").on("input focus", function () {
    var rackId = $("#inputRack").data('id'); // Asegúrate de que este método obtiene correctamente el ID del rack seleccionado
    var inputSeccion = $("#inputSeccion"); // Asegúrate de que este es el ID correcto para el input de Sección
    var listaSecciones = $("#listaSecciones"); // Asume que hay un contenedor para listar las secciones
    // Limpia la lista de secciones y muestra un mensaje de carga
    listaSecciones.show().empty().append('<div>Cargando secciones...</div>');

    if (rackId != 0) {
        console.log(rackId)
        JQueryAjax_Normal('/Almacen/SeleccionarAlmacenRackSeccion', { rackId: rackId }, true, function (data) {
            listaSecciones.empty(); // Limpia la lista para los nuevos datos
            if (data.mensaje === "") {
                // Si hay secciones, añade las opciones a la lista
                console.log(data)

                $.each(data.data, function (index, seccion) {
                    listaSecciones.append('<div class="opcion-seccion" data-id="' + seccion.SeccionId + '">' + seccion.Ubicacion + '</div>');
                });
            } else {
                // Si hay un error, muestra un mensaje en la lista
                listaSecciones.append('<div>No se pudieron cargar las secciones</div>');
                swal("Se ha producido un error", "Error: " + data.mensaje, "error");
            }
        }, function (errorThrown) {
            // Si ocurre un error en la llamada AJAX, muestra un mensaje en la lista
            listaSecciones.empty().append('<div>Error al cargar las secciones</div>');
            console.error("Error al obtener secciones: " + errorThrown);
        });
    } else {
        // Si no hay un ID de rack válido, limpia la lista
        listaSecciones.empty().hide();
    }
});

// Selección de una Sección
$(document).on("click", ".opcion-seccion", function () {
    var texto = $(this).text();
    var seccionId = $(this).data('id');
    $("#inputSeccion").val(texto).data('id', seccionId);
    $("#listaSecciones").hide();
    $("#inputSeccion").attr('data-id', seccionId);

});

// Ocultar lista cuando se hace clic fuera de la sección
$(document).on("click", function (e) {
    if (!$(e.target).closest("#inputSeccion, #listaSecciones").length) {
        $("#listaSecciones").hide();
    }
});

$(document).on("click", function (e) {
    if (!$(e.target).closest("#inputAlmacen, #listaAlmacenes").length) {
        $("#listaAlmacenes").hide();
    }
    if (!$(e.target).closest("#inputRack, #listaRacks").length) {
        $("#listaRacks").hide();
    }
    if (!$(e.target).closest("#inputSeccion, #listaSecciones").length) {
        $("#listaSecciones").hide();
    }
});




$('#AgregarProveedores').click(function () {
    var contador = $('#ControlesProveedores .ControlesDeProveedores').length + 1; // Contamos cuántas filas hay para generar un ID único

    var nuevoControl = $(`
        <div class="row ControlesDeProveedores">
            <div class="col-sm-4 ">
                <div class="row p-1 d-flex align-items-center m-auto" >
                    <label for="inputProveedores${contador}" class="col-sm-2 col-form-label col-form-label-sm">Proveedor</label>
                    <div class="col-sm-10">
                        <input type="text" id="inputProveedores${contador}" class="form-control form-control-sm BuscadorProveedor" placeholder="Seleccione un Proveedor" data-requerido="Proveedor De la Fila ${contador}">
                    </div>
                </div>
            </div>
            <div class="col-sm-4 ">
                <div class="row p-1 m-auto">
                    <label for="inputPrecio${contador}" class="col-sm-2 col-form-label col-form-label-sm">Precio</label>
                    <div class="col-sm-10">
                        <input type="text" id="inputPrecio${contador}" class="form-control form-control-sm" placeholder="Precio $$$$" data-requerido="Precio De la Fila ${contador}">
                    </div>
                </div>
            </div>
            <div class="col-sm-4 ">
                <div class="row p-1 m-auto">
                    <label for="txtRegistroDescripcion${contador}" class="col-sm-2 col-form-label col-form-label-sm">Rack</label>
                    <div class="col-sm-10">
                        <textarea id="txtRegistroDescripcion${contador}" class="form-control form-control-sm" style="height: 80px;" data-requerido="Descripcion"></textarea>
                    </div>
                </div>
            </div>
        </div>`);

    $('#ControlesProveedores').append(nuevoControl);

    nuevoControl.find('.BuscadorProveedor').on("input focus", BuscadorDeProveedores);
    nuevoControl.find('.BuscadorProveedor').on("click", function () {
        $("#botonesPaginado, #opcionesLista").appendTo($(this).parent());
    });
});




eventoCheckFiltro($("#checkFiltroId"), $("#txtFiltroIDWhere"));
eventoCheckFiltro($("#checkFiltroDescripcion"), $("#txtFiltroDescripcionWhere"));
eventoCheckFiltro($("#checkFiltroMarca"), $("#txtFiltroMarcaWhere"));
eventoCheckFiltro($("#checkFiltroLinea"), $("#txtFiltroLineaWhere"));
eventoCheckFiltro($("#checkFiltroCodigoBarras"), $("#txtFiltroCodigoBarrasWhere"));

CargarUltimo()
cargartabla()

$(document).ready(function () {
    JQueryAjax_Normal('/Producto/tamaño', {}, true, function (campos) {
        console.log(campos)
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                }
            }
        });
    }, function () { });
    JQueryAjax_Normal('/Producto/tamañoValor', {}, true, function (campos) {
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                }
            }
        });
    }, function () { });
    JQueryAjax_Normal('/Producto/tamañoProvedor', {}, true, function (campos) {
        campos.campos.forEach(function (item) {
            if (item.Item2 !== -1) {
                var input = document.querySelector('[name="' + item.Item1 + '"]');
                if (input) {
                    input.setAttribute('maxlength', item.Item2);
                }
            }
        });
    }, function () { });

    obtenerYMostrarProductosProveedores()

});




function GuardarProductoProveedor() {
    debugger
    objeto = {
        IdProductoProveedor: 0,
        oProducto: {
            IdProducto: $("#txtRegistroid").val()
        },
        oProveedor: {
            RFC: $("#txtRegistroProveedor").data("idproveedor")
        },
        Precio: $("#txtRegistroProveedorPrecio").val(),
        Referencia: $("#txtRegistroReferencia").val(),
        oUsuario: {
            IdUsuario: 0
        }
    };
    console.log(objeto)



    JQueryAjax_Normal('/Producto/GuardarProductoProveedor', { objeto: objeto }, false, function (data) {
        console.log(data)
        swal("Registro Guardado", "Se guardo el registro  Correctamente", "success")
    }, function () {
        alert("Error")
    });

}
$("#btn-GuardarProductoProveedor").click(function () {
    $('#btn-Eliminar').prop('disabled', false);
    $('#btn-Guardar').prop('disabled', true);
    $('#btn-Cancelar').prop('disabled', true);
    $('#btn-Nuevo').prop('disabled', false);

    GuardarProductoProveedor();
    $('#modo').hide();

    $(".bg-edicion").removeClass("bg-edicion");
    $(".bg-guardar").removeClass("bg-guardar");
    Navegar()
    limpiarProductosProvedores()

    obtenerYMostrarProductosProveedores()

});
// Función para generar el HTML con la información de los productos proveedores en columnas con un ícono de bote de basura para eliminar
function generarHTMLProductosProveedores(listaProductosProveedores) {

    var html = '';
    listaProductosProveedores.forEach(function (productoProveedor) {
        html += '<div class="col-md-3 mb-3">';
        html += '<div class="card">';
        html += '<div class="card-body">';
        html += '<h5 class="card-title">' + productoProveedor.oProveedor.RFC + '</h5>';
        html += '<p class="card-text">Precio: ' + productoProveedor.Precio + '</p>';
        html += '<p class="card-text">Referencia: ' + productoProveedor.Referencia + '</p>';
        html += '<a href="#" class="btn btn-danger eliminarProductoProveedor" data-id="' + productoProveedor.IdProductoProveedor + '"><i class="fas fa-trash-alt"></i> Eliminar</a>';
        html += '</div></div></div>';
    });
    return html;
}

// Suponiendo que tienes una función obtenerProductosProveedores() que obtiene la lista de productos proveedores
// y una función mostrarProductosProveedores() que muestra la lista en el div
function obtenerYMostrarProductosProveedores() {
    $('#productosProveedoresContainer').empty();
    // Lógica para obtener la lista de productos proveedores (puede ser una llamada AJAX, por ejemplo)
    id = $("#txtRegistroid").val()
    console.log(id)

    JQueryAjax_Normal('/Producto/SeleccionarProductoProveedor', { id: id }, true,
        function (data) {
            var htmlProductosProveedores = generarHTMLProductosProveedores(data.data);
            $('#productosProveedoresContainer').html(htmlProductosProveedores);

        }, function () {
            alert("Error")
        });

}

$(document).on("click", ".eliminarProductoProveedor", function () {

    var idProductoProveedor = $(this).data('id');
    JQueryAjax_Normal('/Producto/EliminarProductoProveedor', { id: idProductoProveedor }, false, function (data) {

        if (data.resultado) {
            setTimeout(function () {
                swal("Proveedor Eliminado", "Esta relacion de Productos Proveedores se elimno correctamente", "success")
            }, 250);
        } else {
            setTimeout(function () {
                swal("No se pudo eliminar", data.mensaje, "error");
            }, 250);
        }
    }, function () { });
    obtenerYMostrarProductosProveedores()
});


$("#tabDatosGenerales-tab, #tabDatosInventario-tab, #tabDatosUbicacion-tab, #tabImagenes-tab").click(function () {
    $('[data-requerido]').removeAttr('data-requerido'); // Limpia todos los campos primero

    // Datos Generales
    $('#txtRegistroNombre').attr('data-requerido', 'Nombre del producto');
    $('#txtRegistroNombre').attr('data-requerido', 'Nombre del producto');
    $('#txtRegistroDescripcion').attr('data-requerido', 'Descripción del producto');
    $('#txtBusquedaLinea').attr('data-requerido', 'Línea del producto');
    $('#txtBusquedaMarca').attr('data-requerido', 'Marca del producto');

    // Datos Inventario
    $('#txtRegistroPrecio').attr('data-requerido', 'Precio del producto');
    $('#txtRegistroMinimo').attr('data-requerido', 'Mínimo en stock');
    $('#txtRegistroMaximo').attr('data-requerido', 'Máximo en stock');
    $('#cboactivo').attr('data-requerido', 'Estado activo del producto');
    $('#txtRegistroCodigoBarra').attr('data-requerido', 'Código de barras');
    $('#txtRegistroNoParte').attr('data-requerido', 'Número de parte');

    // Datos Ubicación
    $('#inputAlmacen').attr('data-requerido', 'Almacén del producto');
    $('#inputRack').attr('data-requerido', 'Rack del producto');
    $('#inputSeccion').attr('data-requerido', 'Sección del producto');

    var camposRequeridos = document.querySelectorAll('[data-requerido]');

    camposRequeridos.forEach(function (element) {
        function manejarCambioYClick() {
            var tabId = element.closest('.tab-pane').id;
            contarCamposVaciosEnTiempoReal(tabId);
        }

        element.addEventListener('change', manejarCambioYClick);
        element.addEventListener('click', manejarCambioYClick);

    });
});

// Evento para el tab de Datos Proveedores
$("#tabDatosProveedores-tab").click(function () {
    $('[data-requerido]').removeAttr('data-requerido');
    $('#txtRegistroProveedor').attr('data-requerido', 'Proveedor del producto');
    $('#txtRegistroProveedorPrecio').attr('data-requerido', 'Precio por el proveedor');
    $('#txtRegistroReferencia').attr('data-requerido', 'Referencia del proveedor');
    var camposRequeridos = document.querySelectorAll('[data-requerido]');
    obtenerYMostrarProductosProveedores()

    camposRequeridos.forEach(function (element) {
        function manejarCambioYClick() {
            var tabId = element.closest('.tab-pane').id;
            contarCamposVaciosEnTiempoReal(tabId);
        }

        element.addEventListener('change', manejarCambioYClick);
        element.addEventListener('click', manejarCambioYClick);

    });
});
$('#mostrarGrande').on('click', function () {
    $(this).find('img').toggleClass('zoomed');
    $(this).toggleClass('zoomed-container');
});

$("#filtrosProductos").on("blur", ".busqueda", function () {
    if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
        activarWhere = true;
        ActualizarTabla();
    } else {
        $("#filtrosProductos").focus();
    }
});

$("#filtrosProductos").on("keypress", ".busqueda", function (event) {
    if (event.which === 13) { // 13 es el código de tecla Enter
        if (ValidarCampoVacio($('[data-requerido-filtro]'))) {
            activarWhere = true;
            ActualizarTabla();
        } else {
            $("#filtrosProductos").focus();
        }
    }
});



//$('input').keydown(function (e) {
//    // Verificar si se presionó la tecla Tab
//    if (e.keyCode == 9) {
//        // Obtener el tab actual
//        var currentTab = $('.nav-tabs .active');
//        // Obtener el siguiente tab
//        var nextTab = currentTab.next('li').find('button');
//        // Mostrar el siguiente tab
//        if (nextTab.length > 0) {
//            nextTab.tab('show');
//        }
//    }
//});
$('#ExcelDescripcion').change(function () {
    $('#txtExcelDescripcion').prop('disabled', !$('#ExcelDescripcion').is(':checked'));
});

$('#ExcelMarca').change(function () {
    $('#txtExcelMarca').prop('disabled', !$('#ExcelMarca').is(':checked'));
});

$('#ExcelLinea').change(function () {
    $('#txtExcelLinea').prop('disabled', !$('#ExcelLinea').is(':checked'));
});

$('#ExcelRack').change(function () {
    $('#txtExcelRack').prop('disabled', !$('#ExcelRack').is(':checked'));
});

$('#exportBtn').on('click', function () {
    // Obtener los campos seleccionados
    const camposSeleccionados = $('#campoForm input:checked').map(function () {
        return $(this).val();
    }).get();

    // Obtener las condiciones de los filtros
    const condiciones = [];
    const joins = [];
    const camposSeleccionadosFinales = [];
    const groupByCampos = [];

    // Revisa los checkboxes y agrega las condiciones a la lista si están marcados
    if ($('#ExcelDescripcion').is(':checked')) {
        const descripcion = $('#txtExcelDescripcion').val();
        if (descripcion) {
            condiciones.push(`P.Descripcion LIKE '%${descripcion}%'`);
        }
    }

    if ($('#ExcelMarca').is(':checked')) {
        const marca = $('#txtExcelMarca').val();
        if (marca) {
            condiciones.push(`m.Descripcion LIKE '%${marca}%'`);
            if (!joins.includes('INNER JOIN tMarcas m ON P.IdMarca = m.IdMarca')) {
                joins.push('INNER JOIN tMarcas m ON P.IdMarca = m.IdMarca');
            }
        }
    }

    if ($('#ExcelLinea').is(':checked')) {
        const linea = $('#txtExcelLinea').val();
        if (linea) {
            condiciones.push(`l.Descripcion LIKE '%${linea}%'`);
            if (!joins.includes('INNER JOIN tLineas l ON P.IdLinea = l.IdLinea')) {
                joins.push('INNER JOIN tLineas l ON P.IdLinea = l.IdLinea');
            }
        }
    }

    if ($('#ExcelRack').is(':checked')) {
        const rack = $('#txtExcelRack').val();
        if (rack) {
            condiciones.push(`r.Descripcion LIKE '%${rack}%'`);
            if (!joins.includes('INNER JOIN tAlmacenRacks r ON P.RackId = r.RackId')) {
                joins.push('INNER JOIN tAlmacenRacks r ON P.RackId = r.RackId');
            }
        }
    }

    // Definir los campos disponibles y los equivalentes para descripción
    const camposDisponibles = [
        'P.IdProducto', 'P.Descripcion', 'P.Precio', 'P.Minimo', 'P.Maximo', 'P.NoParte',
        'P.CodigoBarras', 'M.Descripcion', 'L.Descripcion', 'P.Activo',
        'A.Descripcion', 'R.Descripcion', 'S.Descripcion'
    ];

    // Inicializar la consulta SQL
    let consulta = `SELECT `;

    // Iterar sobre los campos seleccionados para agregar alias y JOINs
    camposSeleccionados.forEach(campo => {
        switch (campo) {
            case 'M.Descripcion':
                camposSeleccionadosFinales.push('m.Descripcion AS MarcaDescripcion');
                if (!joins.includes('INNER JOIN tMarcas m ON P.IdMarca = m.IdMarca')) {
                    joins.push('INNER JOIN tMarcas m ON P.IdMarca = m.IdMarca');
                }
                break;
            case 'P.Descripcion':
                camposSeleccionadosFinales.push('P.Descripcion AS ProductoDescripcion');
                break;
            case 'L.Descripcion':
                camposSeleccionadosFinales.push('l.Descripcion AS LineaDescripcion');
                if (!joins.includes('INNER JOIN tLineas l ON P.IdLinea = l.IdLinea')) {
                    joins.push('INNER JOIN tLineas l ON P.IdLinea = l.IdLinea');
                }
                break;
            case 'A.Ubicacion':
                camposSeleccionadosFinales.push('a.Ubicacion AS AlmacenDescripcion');
                if (!joins.includes('INNER JOIN tAlmacenes a ON P.AlmacenId = a.AlmacenId')) {
                    joins.push('INNER JOIN tAlmacenes a ON P.AlmacenId = a.AlmacenId');
                }
                break;
            case 'R.Ubicacion':
                camposSeleccionadosFinales.push('r.Ubicacion AS RackDescripcion');
                if (!joins.includes('INNER JOIN tAlmacenRacks r ON P.RackId = r.RackId')) {
                    joins.push('INNER JOIN tAlmacenRacks r ON P.RackId = r.RackId');
                }
                break;
            case 'S.Ubicacion':
                camposSeleccionadosFinales.push('s.Ubicacion AS SeccionDescripcion');
                if (!joins.includes('INNER JOIN tAlmacenesRacksSecciones s ON P.SeccionId = s.SeccionId')) {
                    joins.push('INNER JOIN tAlmacenesRacksSecciones s ON P.SeccionId = s.SeccionId');
                }
                break;
            case 'SP.CantidadDisponible':
                camposSeleccionadosFinales.push('SUM(SP.CantidadDisponible) AS CantidadDisponible');
                if (!joins.includes('INNER JOIN StockProductos SP ON P.IdProducto = SP.IdProducto')) {
                    joins.push('INNER JOIN StockProductos SP ON P.IdProducto = SP.IdProducto');
                }
                break;
            default:
                camposSeleccionadosFinales.push(campo);
        }
    });

    // Construir la consulta SQL final
    consulta += camposSeleccionadosFinales.join(', ');
    consulta += ` FROM tProductos P`;

    // Agregar los INNER JOINs
    if (joins.length > 0) {
        consulta += ' ' + joins.join(' ');
    }

    // Agregar la cláusula WHERE si es necesario
    if (condiciones.length > 0) {
        consulta += ' WHERE ' + condiciones.join(' AND ');
    }

    // Agregar la cláusula GROUP BY si se incluye SUM(SP.CantidadDisponible)
    if (camposSeleccionados.includes('SP.CantidadDisponible')) {
        consulta += ` GROUP BY ${camposSeleccionados.join(', ')}`;
    }

    // Mostrar la consulta generada
    console.log(consulta);

    // Llamada AJAX para exportar a Excel
    $.ajax({
        url: '/Producto/ExportarExcel', // URL de tu método en el controlador
        type: 'POST',
        data: { query: consulta }, // Enviamos el query como parámetro
        xhrFields: {
            responseType: 'blob' // La respuesta será un blob para manejar el archivo
        },
        success: function (data, status, xhr) {
            // Obtenemos el nombre del archivo del header si está disponible
            var filename = "";
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                var matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) {
                    filename = matches[1].replace(/['"]/g, '');
                }
            }

            var link = document.createElement('a');
            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var url = window.URL.createObjectURL(blob);
            link.href = url;
            link.download = filename || 'archivo.xlsx'; // Nombre por defecto si no se encuentra en el header
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url); // Liberamos la URL para optimización
        },
        error: function () {
            alert('Error al exportar el archivo.');
        }
    });
});

// Función para exportar datos a Excel
function exportarAExcel(lista) {
    // Crear un libro de trabajo nuevo
    const wb = XLSX.utils.book_new();

    // Convertir la lista a formato de hoja de trabajo
    const ws = XLSX.utils.json_to_sheet(lista);

    // Agregar la hoja de trabajo al libro
    XLSX.utils.book_append_sheet(wb, ws, 'Productos');

    // Generar archivo Excel y forzar descarga
    XLSX.writeFile(wb, 'productos.xlsx');
}


$("#BtnExportarExcel").click(function () {
    // Llamada Ajax para obtener los nombres de las columnas
    JQueryAjax_Normal('/Producto/NombreColumnasExcel', {}, false, function (data) {
        // Limpiar el contenedor antes de agregar los nuevos elementos
        $("#ColumnasTablaProductos").empty();

        // Iterar sobre la lista de nombres de columnas recibidas
        data.Lista.forEach(function (columna) {
            // Crear un nuevo div para el checkbox y el label
            var nuevoControl = `
                <div class="form-check">
                    <input class="form-check-input" type="checkbox" value="P.${columna}" id="${columna}">
                    <label class="form-check-label" for="${columna}">${columna}</label>
                </div>
            `;

            // Agregar el nuevo control al contenedor
            $("#ColumnasTablaProductos").append(nuevoControl);
        });
    }, function () {
        console.log("Error al obtener las columnas.");
    });
});

$('#CantidadDisponible.form-check-input').change(function() {
    var cantidadDisponible = $(this).prop('checked'); 
    $('#IdProducto').prop('disabled', cantidadDisponible);
    $('#IdProducto').prop('checked', true);
});
$('#ExcelDescripcion.form-check-input').change(function() {
    var cantidadDisponible = $(this).prop('checked'); 
    $('#Descripcion').prop('disabled', cantidadDisponible);
    $('#Descripcion').prop('checked', true);
});
$('#ExcelMarca.form-check-input').change(function() {
    var cantidadDisponible = $(this).prop('checked'); 
    $('#IdMarca').prop('disabled', cantidadDisponible);
    $('#IdMarca').prop('checked', true);
});
$('#ExcelLinea.form-check-input').change(function() {
    var cantidadDisponible = $(this).prop('checked'); 
    $('#IdLinea').prop('disabled', cantidadDisponible);
    $('#IdLinea').prop('checked', true);
});
$('#ExcelRack.form-check-input').change(function() {
    var cantidadDisponible = $(this).prop('checked'); 
    $('#RackId').prop('disabled', cantidadDisponible);
    $('#RackId').prop('checked', true);
});
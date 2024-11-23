
function buscarPendientes() {
    var tableBody = $('#productosTable tbody');
    tableBody.empty();

    JQueryAjax_Normal('/Producto/BuscarPendientes', {}, true,
        function (data) {
            $.each(data.Lista, function (index, producto) {
                var row = `<tr>
                            <td>${producto.NoParte}</td>
                            <td>${producto.Descripcion}</td>
                            <td>${producto.oMarca.Descripcion}</td>
                            <td><textarea class="form-control descripcion"></textarea></td>
                            <td>
                                <button class="btn btn-success play-button" data-productoid="${producto.IdProducto}">
                                    <i class="fas fa-play"></i>
                                </button>
                            </td>
                        </tr>`;
                tableBody.append(row);
            });
        }, function () {
            alert("Error al obtener los productos pendientes.");
        });
}

buscarPendientes();


$('#productosTable').on('click', '.play-button', function () {
    var id = $(this).data('productoid');
    var valor = $(this).closest('tr').find('textarea.form-control').val();

    JQueryAjax_Normal('/Producto/GuardarPendientes', { id: id, valor: valor }, true,
        function (data) {
            if (data.resultado) {
                swal("Descripción Guardada", "La descripción se guardó correctamente", "success");
                buscarPendientes()
            } else {
                swal("Error", "error: " + data.mensaje, "error");
            }
        }, function () {
            alert("Error al intentar guardar la descripción.");
        });
});

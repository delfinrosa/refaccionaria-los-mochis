function ValidarCampoVacio() {
    $('[data-requerido]').each(function () {
        var tipoInput = $(this).data('requerido');
        if ($(this).val().trim() === '') {
            swal("Campo Vacio", " el campo de " + tipoInput + " se encuentra vacio ", "error");
            return false
        }
    });
        return true
}
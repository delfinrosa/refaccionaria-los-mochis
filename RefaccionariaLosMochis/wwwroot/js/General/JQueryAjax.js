/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />

function JQueryAjax_Normal(strUrl, objParametros, boolAsync, fnSuccess, fnError) {
    $.ajax({
        type: "POST",
        url: strUrl,
        data: objParametros,  // Enviar los parámetros como un objeto simple
        contentType: "application/x-www-form-urlencoded; charset=UTF-8", // Codificación como un formulario
        dataType: "json",
        async: boolAsync,
        success: function (data) {
            fnSuccess(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (errorThrown !== "") {
                fnError(errorThrown);
                console.log("Error: ", errorThrown);
            }
        }
    });
}

function JQueryAjax_WithProgress(url, objParametros, fnSuccess, fnError, fnProgress) {
    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(objParametros),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        xhr: function () {
            var xhr = new window.XMLHttpRequest();
            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    fnProgress(percentComplete);
                }
            }, false);
            return xhr;
        },
        success: fnSuccess,
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (errorThrown !== "") {
                fnError(errorThrown);
                console.log("Error : ", errorThrown);
            }
        }
    });
}

$("#pruebaname").click(function () {
    const elementosConMaxLength = document.querySelectorAll('[maxlength]');

    elementosConMaxLength.forEach((elemento) => {
        console.log(`ID: ${elemento.id}, Name: ${elemento.name}, MaxLength: ${elemento.getAttribute('maxlength')}`);
    });
})
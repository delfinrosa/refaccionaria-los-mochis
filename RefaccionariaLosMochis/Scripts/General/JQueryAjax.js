/// <reference path="../jquery-3.4.1.js" />
/// <reference path="../index/permisosnav.js" />

function JQueryAjax_Normal(strUrl, objParametros, boolAsync, fnSuccess, fnError) {
    $.ajax({
        type: "POST",
        url: strUrl,
        data: JSON.stringify(objParametros),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: boolAsync,
        success: function (data) {
            fnSuccess(data)
            
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (errorThrown !== "") {
                fnError(errorThrown);
                console.log("Error : ");
                console.log(errorThrown);
            }
        }
    });
}
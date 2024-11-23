/// <reference path="../jquery-3.4.1.js" />
/*alert($("#RolPermiso").data("value"));*/
var rol = getCookie("tipo");

function getCookie(nombre) {
    var nombreCookie = nombre + "=";
    var cookies = document.cookie.split(';');
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i].trim();
        if (cookie.indexOf(nombreCookie) == 0) {
            return cookie.substring(nombreCookie.length, cookie.length);
        }
    }
    return "";
}
//MARCA
//Se busca en la cookie el rol y se es la necesaria de despliega el elemento
if (rol === "A" || rol === "I") {
    $("#navMarca").removeAttr("hidden");
} else {
    $("#navMarca").attr("hidden", true);
}
//LINEA
//Se busca en la cookie el rol y se es la necesaria de despliega el elemento
if (rol === "A" || rol === "I") {
    $("#navLinea").removeAttr("hidden");
} else {
    $("#navLinea").attr("hidden", true);
}
//PRODUCTOS
//Se busca en la cookie el rol y se es la necesaria de despliega el elemento
if (rol === "A" || rol === "I") {
    $("#navProductos").removeAttr("hidden");
} else {
    $("#navProductos").attr("hidden", true);
}
//USUARIO
//Se busca en la cookie el rol y se es la necesaria de despliega el elemento
if (rol === "A") {
    $("#navUsuario").removeAttr("hidden");
} else {
    $("#navUsuario").attr("hidden", true);
}
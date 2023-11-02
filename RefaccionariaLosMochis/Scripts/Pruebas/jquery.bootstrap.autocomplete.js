
(function ($) {

    $.fn.wbAutocomplete = function (options, accion) {

        // This is the easiest way to have default options.
        var settings = $.extend({
            // These are the defaults.
            minwidth: $(this).width(),
            ajaxurl: "test",
            fnParametrosAjax: undefined,
            multiselect: false,
            multikey: false,
            setArr: [],
            fnSelect: undefined,
            fnUnselect: undefined,
            showSelected: true,
            noSelect: false,
            selectInactivo: false
        }, options);

        var strControl = $(this).attr("id");

        if (settings.selectInactivo == undefined) {
            settings.selectInactivo = false;

        }

        if (accion !== undefined) {

            switch (accion) {

                case "getSelected":
                    //Autocomplete NO requeridos
                    if ($("#ulSelectedItems_" + strControl).data("selecteditems").length == 0) {

                        if ($("#" + strControl).data("tipo") !== undefined) {

                            if ($("#" + strControl).data("tipo") === "I") { //Tipo de dato Int (identity)
                                return [{ strValor: 0, strDescripcion: "Sin Definir", strEstatus: "A" }];
                            }
                            else { //Tipo de dato varchar

                                return [{ strValor: "", strDescripcion: "Sin Definir", strEstatus: "A" }];
                            }
                        }
                    }
                    return $("#ulSelectedItems_" + strControl).data("selecteditems");
                    break;

                case "setSelected":
                    $("#ulSelectedItems_" + strControl).empty();
                    if ($("#ulSelectedItems_" + strControl).data("selecteditems").length !== undefined)
                        $("#ulSelectedItems_" + strControl).data("selecteditems").length = 0;
                    else
                        $("#ulSelectedItems_" + strControl).data("selecteditems") = [];

                    for (var i = 0; i < settings.setArr.length; i++) {

                        $("#ulSelectedItems_" + strControl).append('<li class="active" style="margin-top:5px"><a>' + settings.setArr[i].strDescripcion + '<button type="button" class="close glyphicon glyphicon-remove" data-seleccion="item" data-valor="' + settings.setArr[i].strValor + '" style="font-size:15px;color:red;margin-right:10px;"></button></a></li>');
                        $("#ulSelectedItems_" + strControl).data("selecteditems").push({
                            strValor: settings.setArr[i].strValor,
                            strDescripcion: settings.setArr[i].strDescripcion,
                            strEstatus: settings.setArr[i].strEstatus
                        });

                    }

                    if (settings.setArr.length > 0)
                        $("#ulSelectedItems_" + strControl).append('<li class="divider"></li>');

                    $("#ulSelectedItems_" + strControl + " .close").click(function (e) {

                        var arrSelectedItems = $("#ulSelectedItems_" + strControl).data("selecteditems");
                        var intIndex = -1;
                        //Valor que se remueve
                        var objItem;
                        for (var i = 0; i < arrSelectedItems.length; i++) {
                            objItem = arrSelectedItems[i]
                            if ($(this).data("valor").toString() === objItem.strValor) {
                                intIndex = i;
                                break;
                            }

                        }

                        if (intIndex >= 0) {
                            arrSelectedItems.splice(intIndex, 1);
                        }


                        if (settings.fnUnselect !== undefined) {
                            //alert(strId);
                            settings.fnUnselect($(this).data("valor"));

                        }

                        if (arrSelectedItems.length > 0)
                            $(this).parent().parent().remove();
                        else
                            $("#ulSelectedItems_" + strControl).empty();

                        e.stopPropagation();
                        e.preventDefault();
                        return false;

                    });

                    $("#" + strControl).val("");
                    $("#ulSelectedItems_" + strControl + " a").each(function () {

                        if (($("#" + strControl).val() + $(this).text()).length > 150) {

                            $("#" + strControl).val(($("#" + strControl).val() + +$(this).text()).substring(0, 147) + "...");
                            return false;
                        }
                        else {
                            if (settings.noSelect == false) {
                                $("#" + strControl).val($("#" + strControl).val() + $(this).text().replace("×", "") + "; ");
                            }
                            else { //Es autocomplete /Textbox
                                $("#" + strControl).val($("#" + strControl).val() + $(this).find("button").eq(0).data("valor"));
                            }
                        }
                    });
                    return false;
                    break;
                case "clearSelected":
                    $("#ulSelectedItems_" + strControl).empty().data("selecteditems", []);
                    $(this).val("");
                    return false;
                    break;
            }
        }
        if (settings.noSelect == false) {
            $(this).attr("data-wbAutocomplete", "true");
        }

        var strLista = '<div id="ulauto_' + $(this).attr("id") + '" class="dropdown">' +
            '<a id = "aDropDown_' + $(this).attr("id") + '" style="display:none" class="btn btn-default dropdown-toggle" type="button" data-toggle="dropdown" aria-expanded="false"></a>' +
            '<ul id="menuResult_' + $(this).attr("id") + '" class="dropdown-menu" aria-labelledby="aDropDown_' + $(this).attr("id") + '" style ="background-color:white; min-width:' + settings.minwidth + 'px">' +
            '<li>' +
            '<ul id="ulSelectedItems_' + strControl + '" data-multi="' + settings.multiselect + '" data-multikey="' + settings.multikey + '" data-selecteditems="[]" class="list-unstyled">' +
            '</ul>' +
            '</li>' +
            '<li>' +
            '<ul id="lstResult_' + $(this).attr("id") + '" class="nav nav-pills nav-stacked" style ="background-color:white;">' +
            '</ul>' +
            '</li>' +
            '<li class="divider"></li>' +
            '<li>' +
            '<div id="divpaginacion_' + $(this).attr("id") + '" style="padding-left: 15px">' +
            '<a id="first_' + $(this).attr("id") + '" tabindex="0" data-btn = "first" class="btn glyphicon glyphicon-fast-backward btn-sm" role="button" style="padding: 5px 5px 10px 5px;"></a>' +
            '<a id="back_' + $(this).attr("id") + '" tabindex="0" data-btn = "back" class="btn glyphicon glyphicon-step-backward btn-sm" role="button" style="padding: 5px 5px 10px 5px;"></a>' +
            '<label id="lblcurrent_' + $(this).attr("id") + '" style="padding: 5px 5px 5px 5px;">1</label>' +
            '<label style="padding: 5px 5px 5px 5px;">de</label>' +
            '<label id="lbltotal_' + $(this).attr("id") + '" style="padding: 5px 5px 5px 5px;">10</label>' +
            '<a id="next_' + $(this).attr("id") + '" tabindex="0" data-btn = "next" class="btn glyphicon glyphicon-step-forward btn-sm" role="button" style="padding: 5px 5px 10px 5px;"></a>' +
            '<a id="last_' + $(this).attr("id") + '" tabindex="0" data-btn = "last" class="btn glyphicon glyphicon-fast-forward btn-sm" role="button" style="padding: 5px 5px 10px 5px;"></a>' +
            '</div>' +
            '</li>' +
            '</ul>' +
            '</div>';


        //var strLista = '<ul id="ulauto_' + $(this).attr("id") + '" class="nav navbar-nav">' +
        //                    '<li id = "liDropDown_' + $(this).attr("id") + '" class="dropdown">' +
        //                        '<a id = "aDropDown_' + $(this).attr("id") + '" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false" style="display:none"></a>' +
        //                        '<ul id="menuResult_' + $(this).attr("id") + '" class="dropdown-menu" role="menu" style ="background-color:white; min-width:' + settings.minwidth + 'px;margin-top:30px">' +
        //                            '<ul id="ulSelectedItems_' + strControl + '" data-multi="' + settings.multiselect + '" data-multikey="' + settings.multikey + '" data-selecteditems="[]" class="list-unstyled">' +
        //                            '</ul>' +
        //                            '<li>' +
        //                                '<ul id="lstResult_' + $(this).attr("id") + '" class="nav nav-pills nav-stacked" style ="background-color:white;">' +
        //                                '</ul>' +
        //                            '</li>' +
        //                            '<li class="divider"></li>' +
        //                            '<li>' +
        //								'<div id="divpaginacion_' + $(this).attr("id") + '" style="padding-left: 15px">' +
        //									'<a id="first_' + $(this).attr("id") + '" tabindex="0" data-btn = "first" class="btn glyphicon glyphicon-fast-backward btn-sm" role="button" style="padding: 5px 5px 10px 5px;"></a>' +
        //									'<a id="back_' + $(this).attr("id") + '" tabindex="0" data-btn = "back" class="btn glyphicon glyphicon-step-backward btn-sm" role="button" style="padding: 5px 5px 10px 5px;"></a>' +
        //									'<label id="lblcurrent_' + $(this).attr("id") + '" style="padding: 5px 5px 5px 5px;">1</label>' +
        //                                    '<label style="padding: 5px 5px 5px 5px;">de</label>' +
        //									'<label id="lbltotal_' + $(this).attr("id") + '" style="padding: 5px 5px 5px 5px;">10</label>' +
        //									'<a id="next_' + $(this).attr("id") + '" tabindex="0" data-btn = "next" class="btn glyphicon glyphicon-step-forward btn-sm" role="button" style="padding: 5px 5px 10px 5px;"></a>' +
        //									'<a id="last_' + $(this).attr("id") + '" tabindex="0" data-btn = "last" class="btn glyphicon glyphicon-fast-forward btn-sm" role="button" style="padding: 5px 5px 10px 5px;"></a>' +
        //								'</div>' +
        //							'</li>' +
        //                        '</ul>' +
        //                    '</li>' +
        //                '</ul>';

        $(this).after(strLista);

        //Evento antes de cerrar lista
        $("#ulauto_" + $(this).attr("id")).on('hide.bs.dropdown', function (e) {
            //Si el input tiene focus no cerrar la lista
            if ($(document.activeElement).attr("id") === strControl) {
                e.stopPropagation();
                e.preventDefault();
            }

            if ($(e.relatedTarget).attr("data-seleccion") === "item") {
                e.stopPropagation();
                e.preventDefault();
            }

        });

        $("#ulauto_" + $(this).attr("id")).on('hidden.bs.dropdown', function (e) {


            if (settings.noSelect == false) {  //Es Autocomplete Normal
                $("#" + strControl).val("");
            }

            if (settings.showSelected === true) {

                $("#ulSelectedItems_" + strControl + " a").each(function () {

                    if (($("#" + strControl).val() + $(this).text()).length > 150) {

                        $("#" + strControl).val(($("#" + strControl).val() + +$(this).text()).substring(0, 147) + "...");
                        return false;
                    }
                    else {

                        if (settings.noSelect == false) {
                            $("#" + strControl).val($("#" + strControl).val() + $(this).text().replace("×", "") + "; ");
                        }
                        else { //Es autocomplete / Textbox

                            $("#" + strControl).val($("#" + strControl).val() + $(this).find("button").eq(0).data("valor"));
                        }
                    }

                });

            }

        });

        //$("#ulauto_" + $(this).attr("id")).on('show.bs.dropdown', function (e) {
        //    // si esta read only o disabled no mostrar lista
        //    if ($(this).attr("disabled") === "disabled" || $(this).attr("readonly") === "readonly") {
        //        e.stopPropagation();
        //        e.preventDefault();
        //    }
        //});

        $(this).keyup(function (e) {
            CargarListaAutocomplete(strControl, $(this).val(), settings.ajaxurl, 1, settings);
        });

        $(this).focus(function () {

            if ($(this).attr("disabled") === "disabled" || $(this).attr("readonly") === "readonly") {
                return false;
            }

            if ($("#ulauto_" + $(this).attr("id")).parent().hasClass("open") === false) {
                $("#menuResult_" + $(this).attr("id")).dropdown("toggle");
                if (settings.noSelect == false) {
                    $(this).val("");
                }

                CargarListaAutocomplete(strControl, $(this).val(), settings.ajaxurl, 1, settings);
            }
        });

        $('#divpaginacion_' + $(this).attr("id") + ' .btn').on("click", function () {

            var intPaginaActual = parseInt("lblcurrent_" + $(this).attr("id"));
            var strPagina;
            //var strControl = "#" + $(this).attr("data-control");
            switch ($(this).attr('data-btn')) {

                case "first":
                    CargarListaAutocomplete(strControl, $("#" + strControl).val(), settings.ajaxurl, 1, settings);
                    break;
                case "next":
                    CargarListaAutocomplete(strControl, $("#" + strControl).val(), settings.ajaxurl, parseInt($("#lblcurrent_" + strControl).html()) + 1, settings);
                    break;
                case "back":
                    CargarListaAutocomplete(strControl, $("#" + strControl).val(), settings.ajaxurl, parseInt($("#lblcurrent_" + strControl).html()) - 1, settings);
                    break;
                case "last":
                    CargarListaAutocomplete(strControl, $("#" + strControl).val(), settings.ajaxurl, parseInt($("#lbltotal_" + strControl).html()), settings);
                    break;

                default:

            }

            return false;

        }).attr("tabindex", "0").keypress(function (e) {
            if (e.which == 13 && $(this).hasClass("disabled") == false) {
                $(this).click();
            }
        });

        return this;

    };

}(jQuery));

function AjaxPostAutocomplete(strUrl, strParametros, bolAsync, fnSuccess) {

    //var fnAjax = function () {

    $.ajax({
        type: "POST",
        url: strUrl,
        data: "{" + strParametros + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: bolAsync,
        success: function (result) {

            fnSuccess(result.d);

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            if (errorThrown !== "") {

                //MostrarNotificacion(strMSGTituloErrorEnAjax, errorThrown, "danger", true, false);

                //alert("Error en Post: " + errorThrown);

            }
        }
    });

    //};

    return false;

}

function CargarListaAutocomplete(strId, strPrefix, strUrl, intPagina, settings) {
    strId=1
    console.log("strId "+strId)
    console.log("strPrefix " + strPrefix)
    console.log("strUrl " + strUrl)
    console.log("intPagina " + intPagina)
    console.log("settings " + settings)
    console.log( settings)
    var strParametros = "";
    if (settings.fnParametrosAjax !== undefined) {

        strParametros = "," + settings.fnParametrosAjax();

    }

    AjaxPostAutocomplete(strUrl, "'intPaginaActual':" + intPagina + ",'strPrefix':'" + strPrefix + "'" + strParametros, true, function (objAutoComplete) {
        //prueba
        //$("#lblcurrent_" + strId).html(objAutoComplete.intPaginaActual);
        //$("#lbltotal_" + strId).html(objAutoComplete.intTotalPaginas);

        console.log("lstItems " + objAutoComplete.lstItems.length)

        $("#lstResult_" + strId).empty();
        var strDisabled = 'class="disabled"';

        var objItem;
        var strGrupo = "";
        var strGrupoActual = "";
        for (var i = 0; i < objAutoComplete.lstItems.length; i++) {
            objItem = objAutoComplete.lstItems[i];

            if (objItem.strGrupo !== undefined) {
                //Inicializar grupo por primera vez
                if (objItem.strGrupo !== "" && strGrupoActual === "") {

                    strGrupoActual = objItem.strGrupo;
                    strGrupo = objItem.strGrupo;
                    $("#lstResult_" + strId).append('<li class="active" ><a href="#" disabled="disabled">' + objItem.strGrupo + '</a></li>');

                }

                //Identificar el cambio de grupo

                if (strGrupoActual !== objItem.strGrupo) {

                    $("#lstResult_" + strId).append('<li class="active"><a href="#" disabled="disabled">' + objItem.strGrupo + '</a></li>');
                    //Igualar grupos
                    strGrupoActual = objItem.strGrupo;

                }

            }

            //Agregar item a html
            //No selecciona Inactivos (Solo para controles en formularios)
            if (settings.selectInactivo == false) {
                $("#lstResult_" + strId).append('<li' + (objItem.strEstatus === "I" ? "" : " class='liautocomplete' ") + ' ' + (objItem.strEstatus === "I" ? strDisabled : "") + '><a data-valor="' + objItem.strValor + '" href="#" ' + (objItem.strEstatus === "I" ? "disabled='disabled'" : "") + '>' + objItem.strDescripcion + '</a></li>');
            }
            else {
                $("#lstResult_" + strId).append('<li' + (objItem.strEstatus === "I" ? "" : " class='liautocomplete' ") + '><a data-valor="' + objItem.strValor + '" href="#">' + objItem.strDescripcion + '</a></li>');
            }

            //Agregar el objeto completo al item.
            $("#lstResult_" + strId + " li:last").find("a").data("objitem", objItem);

        }

        $("#lstResult_" + strId + " a").click(function (objItem) {

            //Cancelar el click si esta disabled
            if ($(this).attr("disabled") === "disabled") {

                return false;

            }

            if ($("#ulSelectedItems_" + strId).data("multi") === false) {
                $("#ulSelectedItems_" + strId).empty();
                $("#ulSelectedItems_" + strId).data("selecteditems").length = 0;
            }
            else {
                if ($("#ulSelectedItems_" + strId).data("multikey") === false) {
                    var objItem;
                    for (var i = 0; i < $("#ulSelectedItems_" + strId).data("selecteditems").length; i++) {
                        objItem = $("#ulSelectedItems_" + strId).data("selecteditems")[i];
                        if (objItem.strValor === $(this).data("valor")) {
                            return false;
                        }
                    }
                }
            }

            if (settings !== undefined) {

                if (settings.showSelected === true) {

                    var objItem = $(this).data("objitem");
                    //$("#ulSelectedItems_" + strControl).append('<li class="active" style="margin-top:5px"><a>' + settings.setArr[i].strDescripcion + '<button type="button" class="close glyphicon glyphicon-remove" data-seleccion="item" data-valor="' + settings.setArr[i].strValor + '" style="font-size:15px;color:red;margin-right:10px;"></button></a></li>');
                    $("#ulSelectedItems_" + strId).append('<li class="active" style="margin-top:5px"><a>' + $(this).html() + '<button type="button" class="close glyphicon glyphicon-remove" data-seleccion="item" data-valor="' + $(this).data("valor") + '" style="font-size:15px;color:red;margin-right:10px;"></button></a></li>');
                    $("#ulSelectedItems_" + strId).append('<li class="divider"></li>');


                    //$("#ulSelectedItems_" + strId).append('<li class="active"><a>' + $(this).html() + '<button type="button" class="close" data-seleccion="item" data-valor="' + $(this).data("valor") + '">&times;</button></a></li>');
                    $("#ulSelectedItems_" + strId).data("selecteditems").push({
                        strValor: objItem.strValor,
                        strDescripcion: objItem.strDescripcion,
                        strEstatus: objItem.strEstatus,
                        objItem: objItem
                    });

                }

                if (settings.fnSelect !== undefined) {

                    settings.fnSelect($(this).data("valor"), this);

                }
            }


            //return false;
        });
        //Remover item seleccionado
        $("#ulSelectedItems_" + strId + " .close").click(function (e) {
            var arrSelectedItems = $("#ulSelectedItems_" + strId).data("selecteditems");
            var intIndex = -1;// $.inArray($(this).data("valor"), $("#ulSelectedItems_" + strId).data("selecteditems"));
            //Valor que se remueve
            var objItem;
            for (var i = 0; i < arrSelectedItems.length; i++) {
                objItem = arrSelectedItems[i]
                if ($(this).data("valor").toString() === objItem.strValor) {
                    intIndex = i;
                    break;
                }

            }

            if (intIndex >= 0) {
                arrSelectedItems.splice(intIndex, 1);
            }


            if (settings.fnUnselect !== undefined) {
                //alert(strId);
                settings.fnUnselect($(this).data("valor"));

            }

            if (arrSelectedItems.length > 0)
                $(this).parent().parent().remove();
            else
                $("#ulSelectedItems_" + strId).empty();

            e.stopPropagation();
            e.preventDefault();
            return false;
        });

        $("#first_" + strId).addClass("disabled");
        $("#next_" + strId).addClass("disabled");
        $("#back_" + strId).addClass("disabled");
        $("#last_" + strId).addClass("disabled");

        if (objAutoComplete.intPaginaActual > 1) {
            $("#first_" + strId).removeClass("disabled");
        }

        if (objAutoComplete.intPaginaActual >= 2) {
            $("#back_" + strId).removeClass("disabled");
        }

        if ((objAutoComplete.intTotalPaginas > 1) && (objAutoComplete.intPaginaActual < objAutoComplete.intTotalPaginas)) {
            $("#next_" + strId).removeClass("disabled");
        }

        if (objAutoComplete.intPaginaActual < objAutoComplete.intTotalPaginas) {
            $("#last_" + strId).removeClass("disabled");
        }

        return false;

    });

    return false;

}
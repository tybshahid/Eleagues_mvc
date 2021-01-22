
$(document).ajaxStart(function () {
    //    $(".loader-wrap").toggle();
    blockUI();
});

$(document).ajaxStop(function () {
    //    $(".loader-wrap").toggle();
    unblockUI();
});

$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
    options.async = true;
});

$.ajaxSetup({
    cache: false
    //,error: function (XMLHttpRequest, textStatus, errorThrown) {
    //    window.location.href = $("#baseUrl").val() + 'Home/Error';
    //}
});

function showAlert(type, msg) {
    if ($('#MainTableDiv').find('.alert').length > 0) {

        $('#MainTableDiv').find('.alert').remove();
    }

    var errorDiv = '<div class="info-alert fade in"> ' +
        ' <div class="alert alert-' + type + ' alert-dismissible" role="alert">' +
        '<button type="button" class="alert-close close" data-dismiss="alert" aria-label="Close">' +
        '<span aria-hidden="true">&times;</span></button>' +
        //                    '<span class="alert-icon"><i class="fa fa-exclamation"></i></span>' +
        '<span class="alert-icon"></span>' +
        '<strong class="alert-text">' + msg + '</strong>' +
        '</div>' +
        '</div>';


    $('#MainTableDiv').prepend(errorDiv);


    $('.close').on('click', function () {
        $('.info-alert').remove();
    });

    setTimeout(function () {
        $('.info-alert').remove();
    }, 20000);
}

function ajaxSuccessCall(result, callBack, errFunc) {
    if (result.indexOf("Application Error") !== -1) {
        if (errFunc !== null) {
            errFunc();
        }
        showAlert('danger', result);
    }
    else {
        callBack();
    } //else condition
}

function blockUI() {

    var ringURL = $("#baseUrl").val() + 'Content/images/ring.gif';

    $.blockUI({
        message: '<div style="font-size:14px; font-family:Calibri !important;background-color: none;border: none;z-index: 0"><img src=' + ringURL + ' /></div>',
        border: 'none',
        padding: '15px',
        backgroundColor: '#000',
        '-webkit-border-radius': '10px',
        '-moz-border-radius': '10px',
        opacity: .5,
        color: '#fff'
    });
    //$.blockUI({ message: '<div style="font-size:13px; font-family:Calibri !important;background-color: none;border: none;z-index: 0;">Processing<img src="@Url.RouteUrl("ajax_loading")" /></div>' });
}

function unblockUI() {
    $.unblockUI();
}
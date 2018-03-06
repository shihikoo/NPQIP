$(function () {
    $('#loading').hide();
    $(document).ready(function () {
        $.ajaxSetup({
            'beforeSend': function () {
                $("#contacForm :input").prop("disabled", true);
                $('#formSubmitNetworkError').hide();
                $('#loading').show();
            },
            'complete': function () {
                $("#contacForm :input").prop("disabled", false);
                $('#loading').hide();
            }
        });
    });

    var contactForm = $('#contactForm');
   
    contactForm.on('submit', function(e) {
        e.preventDefault();
        $('body').scrollTop(0);

        $.ajax({
            url: 'https://script.google.com/macros/s/AKfycbxOdnt1ftiKveSDGV6h5OUu5x5nlomfTpdno1lzvy433l7MK2cn/exec',
            type: 'POST',
            dataType: 'jsonp',
            timeout: 3.5 * 1000,
            data: contactForm.serialize(),
            success: function (msg) {
                $('.form-group').hide();
                $('#nameSpan').append( msg.name);
                $('#formSubmitSuccess').show();
            },
            error: function (jqXhr, textStatus, errorThrown) {
                if (textStatus == "timeout") {
                    $("#contacForm :input").prop("disabled", false);
                    $('#formSubmitNetworkError').show();
                } else {
                    $("#contacForm :input").prop("disabled", true);
                    $('.form-group').hide();
                    $('#formSubmitError').show();
                }
            }
        });
    });
});

function htmlString(text) {
    return text.replace(/\r\n?|\n/g, '<br />');
}
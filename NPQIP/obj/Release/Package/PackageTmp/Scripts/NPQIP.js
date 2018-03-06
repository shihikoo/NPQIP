$(function () {
    $('#loading').hide();
    //$('#formSubmitSuccess').show();
    $(document).ready(function () {

        $("#submit").click(function() {
            // To Display progress bar
            $("#loading").show();
            var name = $("#name").val();
            var email = $("#email").val();
            var mobile = $("#mobile").val();
            var address = $("#address").val();
            // Transfering form information to different page without page refresh
            $.post("processing.php", {
                name: name,
                email: email,
                mobile: mobile,
                address: address
            }, function(status) {
                $("#loading").hide(); // To Hide progress bar
                alert(status);
            });
        });
    });



        $('#LostPasswordForm').submit(function () {
            $("#registrationForm :input").prop("disabled", false);
            $('#loading').hide();
            $('.form-group').hide();
            $('#formSubmitSuccess').show();
        });
})
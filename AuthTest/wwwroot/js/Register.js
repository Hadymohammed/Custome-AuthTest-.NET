$(document).ready(function () {
    // Handle form submission
    $('#RegisterForm').submit(function (e) {
        e.preventDefault();

        var form = $(this);

        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: form.serialize(),
            success: function (response) {
                // Replace the registration view with the rendered partial view
                $('#otpVerificationContainer').html(response);
            }
        });
    });
});
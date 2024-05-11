
document.getElementById('closeAccountBtn').addEventListener('click', closeAccount);

function HandleCloseAccountForm() {
    $.ajax({
        url: '/Account/CloseAccount',
        type: 'POST',

        success: function () {
            //Log that account closing was successful
            console.log('Account was closed successfully');

            //redirect to register View
            window.location.href = '/Account/Login';
        },

        error: function (xhr, status, error) {
            //Display errors
            console.error('Error Changing password', error);
            console.log(xhr.responseText);

            //Display errors in form
            displayValidationErrors(xhr.responseJSON);
            window.location.href = '/Account/Login';
        }
    });
}

function closeAccount() {
    $('#CloseAccountModal').modal('show');
}
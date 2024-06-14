document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('changeUsernameBtn').addEventListener('click', changeUsername);
    document.getElementById('changeEmailBtn').addEventListener('click', changeEmail);
    document.getElementById('changeNameBtn').addEventListener('click', changeName);
    document.getElementById('changePasswordBtn').addEventListener('click', changePassword);
    document.getElementById('logoutBtn').addEventListener('click', logout);
    document.getElementById('closeAccountBtn').addEventListener('click', closeAccount);
    document.getElementById('ConfirmCloseAccount').addEventListener('click', HandleCloseAccountForm);
    document.getElementById('changeProfilePictureBtn').addEventListener('click', changeProfilePicture);

    document.getElementById('ProfilePictureUpdateModal').addEventListener('hidden.bs.modal', clearProfilePictureDataOnClose);
    document.getElementById('UsernameUpdateModal').addEventListener('hidden.bs.modal', clearUsernameDataOnClose);
    document.getElementById('EmailUpdateModal').addEventListener('hidden.bs.modal', clearEmailDataOnClose);
    document.getElementById('NameUpdateModal').addEventListener('hidden.bs.modal', clearNameDataOnClose);
    document.getElementById('PasswordUpdateModal').addEventListener('hidden.bs.modal', clearPasswordDataOnClose);



    document.getElementById('ProfilePictureUpdateForm').addEventListener('submit', handleUpdateProfilePictureSubmit);
    document.getElementById('UsernameUpdateForm').addEventListener('submit', HandleUsernameUpdateSubmit);
    document.getElementById('EmailUpdateForm').addEventListener('submit', HandleEmailUpdateSubmit);
    document.getElementById('NameUpdateForm').addEventListener('submit', HandleNameUpdateSubmit);
    document.getElementById('PasswordUpdateForm').addEventListener('submit', HandlePasswordUpdateSubmit);

});

function displayValidationErrors(errors) {
    // Loop through the errors object and display validation messages above corresponding fields
    hideAllErrorMessages();
    for (var key in errors) {
        if (errors.hasOwnProperty(key)) {
            var errorMessage = errors[key];
            $('#' + key + '-UpdateValidation').text(errorMessage).show();
        }
    }
}

function hideAllErrorMessages() {
    $('.text-danger').hide();
}
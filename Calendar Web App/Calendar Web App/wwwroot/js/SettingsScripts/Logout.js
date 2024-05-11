function logout() {
    $.ajax({
        url: '/Account/Logout',
        type: 'POST',

        success: function () {
            //log that logging out was successful
            console.log('Logging out was succesful');

            //redirect to login view
            window.location.href = '/Account/Login';
        },

        error: function (xhr, status, error) {
            console.error('Logging out failed', error);
            console.log(xhr.responseJSON);
        }

    });
 
}
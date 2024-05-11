//Show form to update username
function changeUsername() {
    //Show the form to change username
    $('#UsernameUpdateModal').modal('show');

}



//send ajax to update username in db
function UpdateUsername(formData) {
    $.ajax({
        url: '/Account/UpdateUsername',
        type: 'POST',
        data: formData,

        success: function () {
            //Log that updating was successful
            console.log("Username updated succesfully");

            //retrieve new username from form
            var newUsername = $('#Username').val();

            //set username field to newUsername
            $('#username').text(newUsername);

            //hide update username form
            $('#UsernameUpdateModal').modal('hide');

        },

        error: function (xhr, status, error) {
            //Display errors
            console.error('Error Changing password', error);
            console.log(xhr.responseText);

            //Display errors in form
            displayValidationErrors(xhr.responseJSON);
        }
    });
}


//Retrieve data from form and send it to be updated in db
function HandleUsernameUpdateSubmit(event) {
    //prevent default form submission
    event.preventDefault();


    //retrieve data from form
    var formData = $(this).serialize();

    //Send data to be updated in db
    UpdateUsername(formData);
}



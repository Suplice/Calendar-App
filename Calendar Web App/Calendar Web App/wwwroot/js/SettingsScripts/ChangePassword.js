//Show update password form
function changePassword() {
    $('#PasswordUpdateModal').modal('show');
}


//Send ajax to update password in db
function UpdatePassword(formData) {
    $.ajax({
        url: "/Account/UpdatePassword",
        type: "POST",
        data: formData,

        success: function () {
            //Log that updating was succesful
            console.log("successfully changed password");

            //hide update form
            $('#PasswordUpdateForm').modal('hide');
        },

        error: function (xhr, status, error) {
            //Display errors
            console.error("Error Changing Password", error);
            console.log(xhr.responseTEXT);

            //Display validation errors in form
            displayValidationErrors(xhr.responseJSON);
        }
    })
}

//Retrieve data from form and send it to be updated
function HandlePasswordUpdateSubmit(event) {
    //prevent default form submission
    event.preventDefault();


    //retireve data from form
    var formData = $(this).serialize();


    //send data to be updated in db
    UpdatePassword(formData);
}
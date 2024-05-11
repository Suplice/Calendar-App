//Function to show update email form
function changeEmail() {
    $('#EmailUpdateModal').modal('show');
}


//Send ajax to update email in db
function UpdateEmail(formData){
    $.ajax({
        url: '/Account/UpdateEmail',
        type: 'POST',
        data: formData,

        success: function () {
            //Log that updating was successful
            console.log("Email Updated succesfully");

            //retrieve email from site
            var newEmail = $('#Email').val();

            //update email on site with new value
            $('#email').text(newEmail);


            //hide update email form
            $('#EmailUpdateModal').modal('hide');
        },

        error: function (xhr, status, error) {
            //Display errors
            console.error('Error Changing email', error);
            console.log(xhr.responseText);

            //Display errors in form
            displayValidationErrors(xhr.responseJSON);
        }
    });
}

//retrieve data from form, and sent it further
function HandleEmailUpdateSubmit(event) {
    //prevent default form submission
    event.preventDefault();


    //retrieve data from form
    formData = $(this).serialize();


    //send data to update in db
    UpdateEmail(formData);
}
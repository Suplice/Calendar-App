//Show form for changing Name
function changeName() {
    $('#NameUpdateModal').modal('show');
}


//Send ajax to update Name in db
function UpdateName(formData) {
    $.ajax({
        url: "/Account/UpdateName",
        type: "POST",
        data: formData,

        success: function () {
            //Log that updating was succesful
            console.log("succesfully Updated Name");


            //retrieve name from site
            var Name = $('#Name').val();
            $('#name').text(Name);


            //hide update form
            $('#NameUpdateModal').modal('hide');
        },

        error: function (xhr, status, error) {
            //Display Errors
            console.error('Error Changing Name', error);
            console.log(xhr.responseText);

            //Display validation errors on site
            displayValidationErrors(xhr.responseJSON);
        }

    });
}

//Retrieve data from form and send it to update in db
function HandleNameUpdateSubmit(event) {
    //prevent default form submission
    event.preventDefault();

    //retrieve data from form
    formData = $(this).serialize();

    //send data to be updated in db
    UpdateName(formData);

}
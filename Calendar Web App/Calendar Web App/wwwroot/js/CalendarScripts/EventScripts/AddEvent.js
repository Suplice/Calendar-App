///Adding Event to Database
//Send an ajax POST to server
function addEvent(formData) {
    $.ajax({
        url: '/Calendar/AddEvent',
        type: 'POST',
        data: formData,
        success: function (response) {
            console.log('Event added successfully');
            // Close the modal after adding the event
            $('#eventFormModal').modal('hide');
            
            
        },
        error: function (xhr, status, error) {
            //display errors in console
            console.error('Error adding event:', error); 
            console.log(xhr.responseText); 

            //display errors in form
            displayAddEventValidationErrors(xhr.responseJSON);
        }
    });
}

function displayAddEventValidationErrors(errors) {
    // Loop through the errors object and display validation messages above corresponding fields
    hideAllErrorMessages();
    for (var key in errors) {
        if (errors.hasOwnProperty(key)) {
            var errorMessage = errors[key];
            $('#' + key + '-AddValidation').text(errorMessage).show();
        }
    }
}


function handleFormSubmission(event) {
    //Prevent the default form submission behavior
    event.preventDefault();

    //Get the form data

    var formData = $(this).serialize();
=

    //Call the function to add the event
    addEvent(formData);
}

//Hide all error messages
function hideAllErrorMessages() {
    $('.text-danger').hide(); 
}


function HandleAddEvent() {

  
    $('#eventFormModal').modal('show');


    $('#eventForm').on('submit', handleFormSubmission);

}

function ValidateOnServer(formData) {

    debugger
    
    $.ajax({
        url: 'TestCalendar/AddEvent',
        type: 'POST',
        data: formData,

        success: function () {
            console.log('event added successfully');
        },


        error: function (xhr, status, event) {
            console.log('there was an error while trying to add event', event);
            
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

function hideAllErrorMessages() {
    $('.text-danger').hide();
}

function TestHandleFormSubmission(event) {
 
    
    event.preventDefault();

    
    var formData = $(this).serialize();

    ValidateOnServer(formData);

}


function TestHandleAddEvent() {


    $('#TestEventFormModal').modal('show');


    $('#TestEventForm').on('submit', TestHandleFormSubmission);

}
function ValidateOnServer(formData, calendar) {


    
    $.ajax({
        url: '/TestCalendar/AddEvent',
        type: 'POST',
        data: formData,

        success: function (response) {
            console.log('event added successfully');

            var event = {
                description: response.description,
                title: response.title,
                start: response.startDate,
                end: response.endDate
            };
            debugger
            $('#TestEventFormModal').modal('hide');

            calendar.addEvent(event);

           
        },


        error: function (xhr, status, event) {
            console.log('there was an error while trying to add event', event);
            console.log(xhr.responseText);
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

function TestHandleFormSubmission(event, calendar) {
 
    
    event.preventDefault();


    var formData = $(event.target).serialize();
   

    ValidateOnServer(formData, calendar);

}


function TestHandleAddEvent(calendar) {


    $('#TestEventFormModal').modal('show');


    $('#TestEventForm').on('submit', function (event) {
        TestHandleFormSubmission(event, calendar);
    });

}
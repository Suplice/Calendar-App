


function ValidateOnServer(formData, calendar) {



    $.ajax({
        url: '/TestCalendar/AddEvent',
        type: 'POST',
        data: formData,

        success: function (response) {
            console.log('event added successfully');

            var eventId = 'event-' + Date.now() + '-' + Math.floor(Math.random() * 1000);

            var event = {
                id: eventId,
                description: response.description,
                title: response.title,
                start: response.startDate,
                end: response.endDate
            };

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


    $('#TestEventForm').off('submit').on('submit', function (event) {
        TestHandleFormSubmission(event, calendar);
    });

}

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('TestEventFormModal').addEventListener('hidden.bs.modal', clearAddEventDataOnClose);
})

function clearAddEventDataOnClose() {
    document.getElementById('TestEventForm').reset();
    document.getElementById('Title-AddValidation').innerText = '';
    document.getElementById('Description-AddValidation').innerText = '';
    document.getElementById('StartDate-AddValidation').innerText = '';
    document.getElementById('EndDate-AddValidation').innerText = '';
}


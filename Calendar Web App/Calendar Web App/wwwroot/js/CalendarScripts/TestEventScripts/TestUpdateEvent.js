///Update event 
//Send ajax POST to server
function UpdateEvent(formData, info, calendar) {
    $.ajax({
        url: "/TestCalendar/UpdateEvent",
        type: "POST",
        data: formData,

        success: function (response) {
            console.log('Event updated successfully');
            //close the modal after adding event
            updateCalendar(response, info, calendar);



            $('#TestEventUpdateModal').modal('hide');

        },
        error: function (xhr, status, error) {
            //Display errors
            console.error('Error updating event', error);
            console.log(xhr.responseText);

            //Display errors in form
            displayUpdateEventValidationErrors(xhr.responseJSON);
        }

    });
}

function updateCalendar(response, info, calendar) {
    var eventId = info.event.id;

    var event = calendar.getEventById(eventId);


    event.setProp('description', response.description)
    event.setProp('description', response.description)
    event.setStart(response.startDate);
    event.setEnd(response.endDate)

    calendar.refetchEvents();

}

function displayUpdateEventValidationErrors(errors) {
    // Loop through the errors object and display validation messages above corresponding fields
    hideAllErrorMessages();
    for (var key in errors) {
        if (errors.hasOwnProperty(key)) {
            var errorMessage = errors[key];
            $('#' + key + '-UpdateValidation').text(errorMessage).show();
        }
    }
}

function handleUpdateFormSubmission(event, info, calendar) {
    //Prevent the default form submission
    event.preventDefault();

    //Get the form data
    var formData = $(event.target).serialize();
    debugger
    UpdateEvent(formData, info,  calendar);

}
function TestHandleEditEvent(info, calendar) {
    var form = document.getElementById('TestEventUpdateForm');
    form.addEventListener('submit', function (event) {
        handleUpdateFormSubmission(event, info, calendar);
    })

    document.getElementById('eventId').value = info.event.id;
}
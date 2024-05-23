///Update event 
//Send ajax POST to server
function UpdateEvent(formData) {
    $.ajax({
        url: "/Calendar/UpdateEvent",
        type: "POST",
        data: formData,

        success: function (response) {
            console.log('Event updated successfully');
            //close the modal after adding event
            $('#eventUpdateModal').modal('hide');

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

function handleUpdateFormSubmission(event) {
    //Prevent the default form submission
    event.preventDefault();

    //Get the form data
    var formData = $(event.target).serialize();

    UpdateEvent(formData)

}

function bindUpdateFormHandler(form) {
    function wrappedHandler(event) {
        handleUpdateFormSubmission(event);
    }

    // Unbind any previous event listener
    form.removeEventListener('submit', form._submitHandler);

    // Bind the new event listener and store it in the form's property
    form._submitHandler = wrappedHandler;
    form.addEventListener('submit', wrappedHandler);
}

function HandleEditEvent(info) {
    var form = document.getElementById('eventUpdateForm');

    bindUpdateFormHandler(form); 

    document.getElementById('eventId').value = info.event.id;
}


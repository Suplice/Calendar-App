function TestHandleFormSubmission(event) {
    $('eventForm').validate();

    event.preventDefault();

    if ($(this).valid()) {
        console.log('validation succeeded');
    }
    else {
        console.log('Validation was unsuccessful');
    }


}


function TestHandleAddEvent() {
    $('#eventFormModal').modal('show');

    document.addEventListener('DOMContentLoaded', function () {



        //Select the form element
        var form = document.getElementById('eventForm');
        form.addEventListener('submit', TestHandleFormSubmission);

    })
}
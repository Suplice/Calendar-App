function TestHandleEventClick(info, calendar) {
    // Populate event details in the pop-up window
    document.getElementById('eventTitle').textContent = info.event.title;
    document.getElementById('eventDescription').textContent = info.event.extendedProps.description;
    document.getElementById('eventStart').textContent = 'Start: ' + formatDate(info.event.start);
    document.getElementById('eventEnd').textContent = 'End: ' + formatDate(info.event.end);

    // Show the pop-up window
    $('#TestEventModal').modal('show');

    //Close the popup window
    document.getElementById('closeButton').addEventListener('click', function () {
        //Hide edit window
        $('#TestEventModal').modal('hide');
    });

    // Event handlers for edit and delete buttons
    document.getElementById('editEventBtn').addEventListener('click', function () {

        //Hide edit window
        $('#TestEventModal').modal('hide');

        //Show update event Form
        $('#TestEventUpdateModal').modal('show');


        //Edit event in calendar (via ajax)
        HandleEditEvent(info, calendar);


    });

    document.getElementById('deleteEventBtn').addEventListener('click', function () {

        //hide edit window
        $('#TestEventModal').modal('hide');

        //Delete event from calendar (via ajax)
        TestHandleDeleteEvent(info, calendar);

    });
}


function formatDate(date) {
    var year = date.getFullYear();
    var month = ('0' + (date.getMonth() + 1)).slice(-2);
    var day = ('0' + date.getDate()).slice(-2);
    var hour = ('0' + date.getHours()) - 2;
    var minute = ('0' + date.getMinutes()).slice(-2);

    return year + '-' + month + '-' + day + ' ' + hour + ':' + minute;
}
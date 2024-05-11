function HandleEventResize(info) {
    var event = info.event;
    var newEnd = event.end;
    var start = event.start


    $.ajax({
        url: '/Calendar/UpdateEvent',
        type: 'POST',
        data: {
            eventId: event.id,
            Title: event.title,
            Description: event.extendedProps.description,
            StartDate: start.toISOString(),
            EndDate: newEnd.toISOString(),
        },
        success: function () {
            console.log('Event Updated to database');
        },

        error: function () {
            console.error('Error updating event to databse');
        }
    })
}
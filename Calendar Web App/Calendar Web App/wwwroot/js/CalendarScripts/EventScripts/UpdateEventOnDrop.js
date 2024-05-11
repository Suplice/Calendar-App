function HandleEventDrop(info) {

    //Get new date
    var event = info.event;
    var newStart = event.start;
    var newEnd = event.end;


    event.setStart(newStart);
    event.setEnd(newEnd);



    //Send ajax to change event date in db
    $.ajax({
        url: '/Calendar/UpdateEvent',
        type: 'POST',
        data: {
            eventId: event.id,
            Title: event.title,
            Description: event.extendedProps.description,
            StartDate: newStart.toISOString(),
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
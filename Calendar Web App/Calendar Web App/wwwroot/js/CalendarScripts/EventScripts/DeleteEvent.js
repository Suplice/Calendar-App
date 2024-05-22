///Delete event
//send ajax POST to server
function HandleDeleteEvent(info) {
    var eventId = info.event.id;

    $.ajax({
        url: '/Calendar/RemoveEvent',
        type: 'POST',
        data: {eventId: eventId},

        success: function () {
            console.log('Event deleted succesfully');
        },

        error: function (xhr, status, eroor) {
            console.error('Erorr removing event', error);
            console.log(xhr.responseJSON);
        }
    })
}
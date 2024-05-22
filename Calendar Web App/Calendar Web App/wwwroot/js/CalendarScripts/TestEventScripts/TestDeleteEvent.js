function TestHandleDeleteEvent(info, calendar) {
    //retrieve eventId
    var eventId = info.event.id;


    //retrieve event via Id
    var event = calendar.getEventById(eventId);


    //remove event from calendar
    event.remove();
}
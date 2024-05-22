function TestHandleDeleteEvent(info, calendar) {
    var eventId = info.event.id;

    var event = calendar.getEventById(eventId);

    event.remove();

    calendar.refetchEvents();
}
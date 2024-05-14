export function InitializeCalendar() {
    let calendarEl = document.getElementById('calendar');
    let calendar = new FullCalendar.Calendar(calendarEl, {
        headerToolbar: {
            left: 'prev,next today',
            center: 'addEventButton',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth'
        },
        timeZone: 'Europe/Warsaw',
        locale: 'pl',
        editable: true,
        selectable: true,
        dayMaxEvents: true,
        events: '/Calendar/GetEvents',
        eventDrop: function (info) {
            HandleEventDrop(info);
        },
        eventResize: function (info) {
            HandleEventResize(info);
        },
        eventClick: function (info) {
            HandleEventClick(info);
        },
        customButtons: {
            addEventButton: {
                text: 'Add Event',
                click: function () {
                    HandleAddEvent();
                },
            }
        },
        initialView: 'dayGridMonth'
    });

    calendar.render();


    $('#eventFormModal').on('hidden.bs.modal', function () {
        //Refresh calendar after adding event
        calendar.refetchEvents();
    });
    $('#eventUpdateModal').on('hidden.bs.modal', function () {
        //Refresh calendar after updating event
        calendar.refetchEvents();
    })

    $('#eventModal').on('hidden.bs.modal', function () {
        //Refresh calendar after updating event
        calendar.refetchEvents();
    })
   

    return calendar;
}


export function InitializeTestCalendar() {
    let calendarEl = document.getElementById('calendar');
    let calendar = new FullCalendar.Calendar(calendarEl, {
        headerToolbar: {
            left: 'prev,next today',
            center: 'addEventButton',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth'
        },
        events: [
            {
                title: 'Event 1',
                start: new Date(),
                end: new Date(new Date().getTime() + 2 * 60 * 60 * 1000) // Add 2 hours
            },
            {
                title: 'Event 2',
                start: new Date(new Date().getTime() + 2 * 60 * 60 * 1000), // Start 2 hours after the current time
                end: new Date(new Date().getTime() + 4 * 60 * 60 * 1000) // Add 2 more hours
            }
        ],
        customButtons: {
            addEventButton: {
                text: 'Add Event',
                click: function () {
                    TestHandleAddEvent()
                },
            }
        },
        editable: true,
        selectable: true,
        dayMaxEvents: true,
        initialView: 'dayGridMonth'
    });

    calendar.render();

    return calendar;
}
using Calendar_Web_App.Data;
using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Models;
using Calendar_Web_App.ViewModels.EventViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Linq;
using System.Security.Claims;

namespace Calendar_Web_App.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<EventRepository> _logger;

        public EventRepository(IApplicationDbContext context, ILogger<EventRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        private IEnumerable<Event> ExpandReccuringEvents(IEnumerable<Event> events) { 
        
            List<Event> resultEvents = new List<Event>();

            var recurrenceMaxDate = DateTime.Now.AddYears(1);

            foreach(var ev in events)
            {
                if (ev.RecurrencePattern == RecurrencePattern.none)
                {
                    resultEvents.Add(ev);
                    continue;
                }
                else 
                {
                    var StartDate = ev.start;
                    var EndDate = ev.end;

                    while (StartDate <= recurrenceMaxDate && (!ev.RecurrenceEndDate.HasValue || StartDate <= ev.RecurrenceEndDate.Value))
                    {
                        if(StartDate >= DateTime.Now)
                        {
                            var recurringEvent = new Event
                            {
                                Id = ev.Id,
                                UserId = ev.UserId,
                                title = ev.title,
                                description = ev.description,
                                start = StartDate,
                                end = EndDate,
                                RecurrencePattern = ev.RecurrencePattern,
                                RecurrenceEndDate = ev.RecurrenceEndDate
                            };

                            resultEvents.Add(recurringEvent);
                        };

                        switch(ev.RecurrencePattern)
                        {
                            case RecurrencePattern.daily:
                                StartDate = StartDate.AddDays(1);
                                EndDate = EndDate.AddDays(1);
                                break;
                            case RecurrencePattern.weekly:
	                            StartDate = StartDate.AddDays(7);
	                            EndDate = EndDate.AddDays(7);
	                            break;
                            case RecurrencePattern.monthly:
                                StartDate = StartDate.AddMonths(1);
								EndDate = EndDate.AddMonths(1);
                                break;
						}

                    }
                }
            }

            return resultEvents;

        }

        public IEnumerable<Event> GetAllEvents(string UserId)
        {
            try
            {
                _logger.LogInformation("Executing GetAllEvents operation in Event Repository");

                var events = _context.Events.Where(e => e.UserId == UserId).ToList();
                
                if(!events.Any())
                {
                    _logger.LogWarning("user {UserId} does have any events", UserId);
                    return Enumerable.Empty<Event>();
                }
                else
                {
                    events = (List<Event>) ExpandReccuringEvents(events);


                    _logger.LogInformation("Events for {UserId} were successfully retrieved", UserId);
                    return events;
                }
            }
            catch(InvalidOperationException ex)
            {
	            _logger.LogError(ex, "An InvalidOperationException occurred while trying to retrieve events for user {UserId}",UserId);
                return Enumerable.Empty<Event>();
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "an unexpected error occured while trying to retrieve user {userId} events", UserId);
				return Enumerable.Empty<Event>();
			}
        }

        public Event GetEventById(string eventId)
        {
            try
            {
                _logger.LogInformation("Executing GetEventById operation in Event Repository");

                var Event = _context.Events.FirstOrDefault(e => e.Id == eventId);

                if(Event == null)
                {
                    _logger.LogWarning("Event {eventId} does not exist", eventId);
				}
                else
                {
                    _logger.LogInformation("event {eventId} was successfully found", eventId); 
				}
                return Event;
			}
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex, "An InvalidOperationException occurred while trying to retrieve event using {eventId}", eventId);
                return null;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "an error occured while trying to retrieve event {eventId} from database", eventId);
                return null;
            }


        }

        public void AddEvent(AddEventViewModel newEventModel)
        {
            try
            {
                _logger.LogInformation("Executing AddEvent operation in Event Repository");

                //Check if User exists
                if (newEventModel.UserId != null)
                {
                    //Create new Event
                    var EventToAdd = new Event
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = newEventModel.UserId,
                        title = newEventModel.Title,
                        description = newEventModel.Description,
                        start = newEventModel.StartDate,
                        end = newEventModel.EndDate,
                        RecurrencePattern = newEventModel.RecurrencePattern,
                        RecurrenceEndDate = newEventModel.RecurrenceEndDate
                    };
                    //Add event to Db
                    _context.Events.Add(EventToAdd);
                    _context.SaveChanges();

                    _logger.LogInformation("New Event was successfully added");
                }
                else
                {
                    _logger.LogWarning("User does not exist");
                }
            }
            catch (InvalidOperationException ex)
            {
	            _logger.LogError(ex, "An InvalidOperationException occurred while trying to add event");
                return;
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to add event");
                return;
            }
        }

        public void UpdateEvent(UpdateEventViewModel UpdateEventModel)
        {
            try
            {
                _logger.LogInformation("Executing UpdateEvent method in Event Respository");

                //Find event to update
                Event UpdatedEvent = _context.Events.FirstOrDefault(e => e.Id == UpdateEventModel.EventId);

                if(UpdatedEvent == null)
                {
                    _logger.LogWarning("Event with Id {eventId} does not exist", UpdateEventModel.EventId);
                    return;
                }
                else {
                    //Change event details
                    UpdatedEvent.title = UpdateEventModel.Title;
                    UpdatedEvent.description = UpdateEventModel.Description;
                    UpdatedEvent.start = UpdateEventModel.StartDate;
                    UpdatedEvent.end = UpdateEventModel.EndDate;
                    UpdatedEvent.RecurrencePattern = UpdateEventModel.RecurrencePattern;
                    UpdatedEvent.RecurrenceEndDate = UpdateEventModel.RecurrenceEndDate;
                    _context.SaveChanges();
                    _logger.LogInformation("Event {eventId} was successfully updated", UpdateEventModel.EventId);
                }
            }
            catch(InvalidOperationException ex)
            {
	            _logger.LogError(ex, "An InvalidOperationException occurred while trying to update event");
                return;
			}
            catch(Exception ex)
            {
                _logger.LogError(ex, "an unexpected error occured while trying to update Event {eventId}", UpdateEventModel.EventId);
                return;
            }
            
	    }

        public void RemoveEvent(string eventId)
        {
			try
			{
                _logger.LogInformation("Executing RemoveEvent method in EventRepository");
				// Find event to remove
				var eventToDelete = _context.Events.FirstOrDefault(e => e.Id == eventId);

				// Check whether event exists
				if (eventToDelete != null)
				{
					_context.Events.Remove(eventToDelete);
					_context.SaveChanges();

                    _logger.LogInformation("Event with Id {eventId} was removed successfully", eventId);
				}
                else
                {
                    _logger.LogWarning("Event with Id {eventId} not found", eventId);
                }
			}
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "An InvalidOperationException occurred while trying to update event");
                return;
            }
			catch (Exception ex)
			{

                _logger.LogError(ex, "an unexpected error occured while removing the event with Id {EventId}", eventId);
                return;
			}
		}

   
    }
    
}

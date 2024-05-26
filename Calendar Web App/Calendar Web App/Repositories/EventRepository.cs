using Calendar_Web_App.Data;
using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Models;
using Calendar_Web_App.ViewModels.EventViewModels;
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
                    _logger.LogInformation("Events for {UserId} were successfully retrieved", UserId);
                    return events;
                }
            }
            catch(InvalidOperationException ex)
            {
	            _logger.LogError(ex, "An InvalidOperationException occurred while trying to retrieve events for user {UserId}",UserId);
				throw;
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "an unexpected error occured while trying to retrieve user {userId} events", UserId);
                throw;
            }
        }

        public Event GetEventById(string eventId)
        {
            try
            {
                _logger.LogInformation("Executing GetEventById operation in Event Repository");

                var Event = _context.Events.Find(eventId);

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
            catch (Exception ex) 
            {
                _logger.LogError(ex, "an error occured while trying to retrieve event {eventId} from database", eventId);
                throw;
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
                        end = newEventModel.EndDate
                    };
                    //Add event to Db
                    _context.Events.Add(EventToAdd);
                    _context.SaveChanges();

                    _logger.LogInformation("New Event {eventId} was successfully added", EventToAdd.Id);
                }
                else
                {
                    _logger.LogWarning("User with Id {UserId} does not exist", newEventModel.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to add event");
                throw;
            }
        }

        public void UpdateEvent(UpdateEventViewModel UpdateEventModel)
        {
            try
            {
                _logger.LogInformation("Executing UpdateEvent method in Event Respository");

                //Find event to update
                Event UpdatedEvent = _context.Events.FirstOrDefault(e => e.Id == UpdateEventModel.EventId);

                //Check whether event is connected to actual User
                if (UpdatedEvent.UserId != UpdateEventModel.UserId)
                {
                    _logger.LogWarning("The event with Id {eventId} does not belong to User {toUpdate.UserId}", UpdateEventModel.EventId, UpdateEventModel.UserId);
                }
                else if(UpdatedEvent == null)
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
                    _context.SaveChanges();
                    _logger.LogInformation("Event {eventId} was successfully updated", UpdateEventModel.EventId);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "an unexpected error occured while trying to update Event {eventId}", UpdateEventModel.EventId);
                throw;
            }
            
	    }

        public void RemoveEvent(string eventId)
        {
			try
			{
                _logger.LogInformation("Executing RemoveEvent method in EventRepository");
				// Find event to remove
				var eventToDelete = _context.Events.Find(eventId);

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
			catch (Exception ex)
			{

                _logger.LogError(ex, "an unexpected error occured while removing the event with Id {EventId}", eventId);
                throw;
			}
		}

   
    }
    
}

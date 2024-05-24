using Calendar_Web_App.Data;
using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Models;
using Calendar_Web_App.ViewModels.EventViewModels;
using System.Security.Claims;

namespace Calendar_Web_App.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventRepository> _logger;

        public EventRepository(ApplicationDbContext context, ILogger<EventRepository> logger)
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

                var data = _context.Events.Find(eventId);

                if(data == null)
                {
                    _logger.LogWarning("Event {eventId} does not exist", eventId);
				}
                else
                {
                    _logger.LogInformation("event {eventId} was successfully found", eventId); 
				}
                return data;
			}
            catch (Exception ex) 
            {
                _logger.LogError(ex, "an error occured while trying to retrieve event {eventId} from database", eventId);
                throw;
            }


        }

        public void AddEvent(AddEventViewModel newEvent)
        {
            try
            {
                _logger.LogInformation("Executing AddEvent operation in Event Repository");

                //Check if User exists
                if (newEvent.UserId != null)
                {
                    //Create new Event
                    var EventToAdd = new Event
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = newEvent.UserId,
                        title = newEvent.Title,
                        description = newEvent.Description,
                        start = newEvent.StartDate,
                        end = newEvent.EndDate
                    };
                    //Add event to Db
                    _context.Events.Add(EventToAdd);
                    _context.SaveChanges();

                    _logger.LogInformation("New Event {eventId} was successfully added", EventToAdd.Id);
                }
                else
                {
                    _logger.LogWarning("User with Id {UserId} does not exist", newEvent.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured while trying to add event");
                throw;
            }
        }

        public void UpdateEvent(UpdateEventViewModel toUpdate)
        {
            try
            {
                _logger.LogInformation("Executing UpdateEvent method in Event Respository");

                //Find event to update
                Event UpdatedEvent = _context.Events.FirstOrDefault(e => e.Id == toUpdate.EventId);

                //Check whether event is connected to actual User
                if (UpdatedEvent.UserId != toUpdate.UserId)
                {
                    _logger.LogWarning("The event with Id {eventId} does not belong to User {toUpdate.UserId}", toUpdate.EventId, toUpdate.UserId);
                }
                else if(UpdatedEvent == null)
                {
                    _logger.LogWarning("Event with Id {eventId} does not exist", toUpdate.EventId);
                    return;
                }
                else {
                    //Change event details
                    UpdatedEvent.title = toUpdate.Title;
                    UpdatedEvent.description = toUpdate.Description;
                    UpdatedEvent.start = toUpdate.StartDate;
                    UpdatedEvent.end = toUpdate.EndDate;
                    _context.SaveChanges();
                    _logger.LogInformation("Event {eventId} was successfully updated", toUpdate.EventId);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "an unexpected error occured while trying to update Event {eventId}", toUpdate.EventId);
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

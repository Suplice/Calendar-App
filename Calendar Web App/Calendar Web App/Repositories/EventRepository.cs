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

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Event> GetAllEvents(string UserId)
        {
            return _context.Events.Where(e => e.UserId == UserId).ToList();
        }

        public Event GetEventById(int eventId)
        {
            return _context.Events.Find(eventId);
        }

        public void AddEvent(AddEventViewModel newEvent)
        {
         

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
			}	
        }

        public void UpdateEvent(UpdateEventViewModel toUpdate)
        {
            //Find event to update
            Event Updated = _context.Events.FirstOrDefault(e => e.Id == toUpdate.EventId);

            //Check whether event is connected to actual User
            if(Updated.UserId != toUpdate.UserId)
            {
                return;
            }

            //Change event details
            Updated.title = toUpdate.Title;
            Updated.description = toUpdate.Description;
            Updated.start = toUpdate.StartDate;
            Updated.end = toUpdate.EndDate;
            _context.SaveChanges();
	    }

        public void RemoveEvent(string eventId)
        {
			try
			{
				// Find event to remove
				var eventToDelete = _context.Events.Find(eventId);

				// Check whether event exists
				if (eventToDelete != null)
				{
					_context.Events.Remove(eventToDelete);
					_context.SaveChanges();
				}
			}
			catch (InvalidOperationException ex)
			{
				// Handle the case where the event was not found
				// Log the exception or take other appropriate actions
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
			catch (Exception ex)
			{
				// Handle other potential exceptions
				Console.WriteLine($"An unexpected error occurred: {ex.Message}");
			}
		}

   
    }
    
}

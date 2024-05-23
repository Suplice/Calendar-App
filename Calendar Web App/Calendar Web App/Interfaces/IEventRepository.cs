using Calendar_Web_App.Models;
using Calendar_Web_App.ViewModels.EventViewModels;

namespace Calendar_Web_App.Interfaces
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAllEvents(String UserId);
        Event GetEventById(int id);
        void AddEvent(AddEventViewModel newEvent);
        void RemoveEvent(string id);
        void UpdateEvent(UpdateEventViewModel toUpdate);

    }
}

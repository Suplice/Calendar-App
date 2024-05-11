using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.Models
{
    public class Calendar
    {
        [Key]
        public int CalendarId { get; set; }

        public List<Event>? Events { get; set; }
    }
}

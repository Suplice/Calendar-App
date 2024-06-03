using Calendar_Web_App.Attributes;
using Calendar_Web_App.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calendar_Web_App.ViewModels.EventViewModels
{
    public class AddEventViewModel
    {
        [Required(ErrorMessage = "The Title field is required")]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "The Start Date field is required")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "The End Date field is required")]
        [DataType(DataType.DateTime)]
        [CompareDates("StartDate", "EndDate")]
        public DateTime EndDate { get; set; }
        public string eventId { get; set; }
        public string? UserId { get; set; }

        public RecurrencePattern? RecurrencePattern { get; set; }

        public DateTime? RecurrenceEndDate { get; set; }


    }
}
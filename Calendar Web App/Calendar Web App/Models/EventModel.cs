using Calendar_Web_App.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calendar_Web_App.Models
{
	public class Event
	{
		[Key]
		public string Id { get; set; }

		[Required]
		public string title { get; set; }

		public string? description { get; set; }

		[Required]
		public DateTime start { get; set; }

		[Required]
		public DateTime end { get; set; }

		[ForeignKey("User")]
		public string UserId { get; set; }

		public User User { get; set; }

		public RecurrencePattern? RecurrencePattern { get; set; }

		public DateTime RecurrenceEndDate { get; set; }
	}
}

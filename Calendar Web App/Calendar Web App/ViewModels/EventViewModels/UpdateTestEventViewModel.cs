using Calendar_Web_App.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.EventViewModels
{
	public class UpdateTestEventViewModel
	{
		[Required(ErrorMessage = "The Title field is required")]
		public string Title { get; set; }

		public string? Description { get; set; }

		[Required(ErrorMessage = "The Start Date field is required")]
		[DataType(DataType.DateTime)]
		public DateTime StartDate { get; set; }


		[DataType(DataType.DateTime)]
		[CompareDates("StartDate", "EndDate")]
		[Required(ErrorMessage = "The End Date field is required")]
		public DateTime EndDate { get; set; }

		public string EventId { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountSettingsViewModels
{
    public class ChangeNameViewModel
    {
	    [StringLength(15, MinimumLength = 3, ErrorMessage = "The Username field must contain between 3 and 15 characters.")]
	    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The Name field can only contain letters.")]
		[Required(ErrorMessage = "Name is required")]
		public required string Name { get; set; }
    }
}

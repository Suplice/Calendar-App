using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountSettingsViewModels
{
    public class ChangeNameViewModel
    {
	    [StringLength(15, MinimumLength = 3, ErrorMessage = "The Username field must contain between 3 and 15 characters.")]
	    [Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }
    }
}

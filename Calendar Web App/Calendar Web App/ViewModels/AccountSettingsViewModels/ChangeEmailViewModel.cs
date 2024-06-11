using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountSettingsViewModels
{
    public class ChangeEmailViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
	        ErrorMessage = "Please enter a valid email address.")]
        [Required(ErrorMessage = "This Field is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public required string Email { get; set; }
    }
}

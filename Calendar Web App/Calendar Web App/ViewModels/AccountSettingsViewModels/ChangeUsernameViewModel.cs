using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountSettingsViewModels
{
    public class ChangeUsernameViewModel
    {
        [StringLength(15, MinimumLength = 3, ErrorMessage = "The Username field must contain between 3 and 15 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "The Username field can only contain letters and numbers.")]
		[Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }
    }
}

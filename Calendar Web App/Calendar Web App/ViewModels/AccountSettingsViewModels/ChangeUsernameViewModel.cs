using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountSettingsViewModels
{
    public class ChangeUsernameViewModel
    {
        [StringLength(15, MinimumLength = 3, ErrorMessage = "The Username field must contain between 3 and 15 characters.")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Calendar_Web_App.ViewModels.AccountAccessViewModels
{
    public class LoginViewModel
    {
        [StringLength(15, MinimumLength = 3, ErrorMessage = "The Login field must contain between 3 and 15 characters.")]
        [Required(ErrorMessage = "Login is required")]
        [Display(Name = "Login")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }

    }
}

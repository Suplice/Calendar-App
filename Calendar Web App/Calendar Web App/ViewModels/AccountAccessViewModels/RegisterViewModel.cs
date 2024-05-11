using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountAccessViewModels
{
    public class RegisterViewModel
    {
        [StringLength(15, MinimumLength = 3, ErrorMessage = "The Name field must contain between 3 and 15 characters.")]
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(15, MinimumLength = 3, ErrorMessage = "The Login field must contain between 3 and 15 characters.")]
        [Required(ErrorMessage = "Login is required")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }



        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords are not the same")]
        public string ConfirmPassword { get; set; }
    }
}

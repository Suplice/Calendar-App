using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountAccessViewModels
{
    public class RegisterViewModel
    {
        [StringLength(15, MinimumLength = 3, ErrorMessage = "The Name field must contain between 3 and 15 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The Name field can only contain letters.")]
		[Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(15, MinimumLength = 3, ErrorMessage = "The Login field must contain between 3 and 15 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "The Username field can only contain letters and numbers.")]
		[Required(ErrorMessage = "Login is required")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
	        ErrorMessage = "Please enter a valid email address.")]
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

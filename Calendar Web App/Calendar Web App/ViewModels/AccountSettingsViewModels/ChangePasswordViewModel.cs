using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountSettingsViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Old password is required")]
        public required string OldPassword { get; set; }


        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }


        [Required(ErrorMessage = "Confirm new password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords are not the same")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }

    }
}

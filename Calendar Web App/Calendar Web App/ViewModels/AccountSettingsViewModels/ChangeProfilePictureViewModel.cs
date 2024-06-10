using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.ViewModels.AccountSettingsViewModels
{
	public class ChangeProfilePictureViewModel
	{
		[Required(ErrorMessage = "This field is required")]
		public IFormFile ProfilePicture { get; set; }
	}
}

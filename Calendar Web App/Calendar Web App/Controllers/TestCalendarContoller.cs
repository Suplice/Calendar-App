using Calendar_Web_App.ViewModels.EventViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar_Web_App.Controllers
{
	public class TestCalendarContoller : Controller
	{

		[AllowAnonymous]
		[HttpPost]
		public IActionResult AddEvent(AddEventViewModel model)
		{

			//validate Model 
			if (!ModelState.IsValid)
			{
				var errors = ModelState.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
				);
				return BadRequest(errors);
			}

			return Ok();
		}

		public IActionResult UpdateEvent()
		{

			//Validate Model
			if (!ModelState.IsValid) {
				var errors = ModelState.ToDictionary(
						kvp => kvp.Key,
						kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
					);

				return BadRequest(errors);
			}

			return Ok();
		}
	}
}

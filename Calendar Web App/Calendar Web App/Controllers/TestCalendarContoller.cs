using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Repositories;
using Calendar_Web_App.ViewModels.EventViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar_Web_App.Controllers
{
	public class TestCalendarController : Controller
	{


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

			return Ok(model);
		}

		public IActionResult UpdateEvent(UpdateEventViewModel model)
		{

			//Validate Model
			if (!ModelState.IsValid) {
				var errors = ModelState.ToDictionary(
						kvp => kvp.Key,
						kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
					);

				return BadRequest(errors);
			}

			return Ok(model);
		}
	}
}

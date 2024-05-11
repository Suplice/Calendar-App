using Calendar_Web_App.Interfaces;
using Calendar_Web_App.ViewModels.EventViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Calendar_Web_App.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IEventRepository _eventRepository;

        public CalendarController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpGet]
        [AllowAnonymous] // Allow access to everyone, regardless of authentication status
        public IActionResult TestCalendar()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // If the user is authenticated, redirect them to another page
                return RedirectToAction("Calendar", "Calendar");
            }
            return View();
            
        }

        [Authorize]
        [HttpGet]
        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = _eventRepository.GetAllEvents(userId);
            return Json(events);
        }

        [HttpPost]
        [Authorize]
		public IActionResult AddEvent(AddEventViewModel newEvent)
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            newEvent.UserId = userId;
			//Remove User, UserId from HttpPost Input

			if (!ModelState.IsValid)
			{

				var errors = ModelState.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
				);
                return BadRequest(errors);
			}

            _eventRepository.AddEvent(newEvent);

            return Ok();
		
		}
        [HttpPost]
		[Authorize]
        public IActionResult RemoveEvent(int eventId)
        {
            _eventRepository.RemoveEvent(eventId);
            return Ok();
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateEvent(UpdateEventViewModel updatedEvent)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            updatedEvent.UserId = userId;

			if (!ModelState.IsValid)
			{

				var errors = ModelState.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
				);
				return BadRequest(errors);
			}

			_eventRepository.UpdateEvent(updatedEvent);

			return Ok();
		}
    
    }
}

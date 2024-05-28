using Calendar_Web_App.Data;
using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Models;
using Calendar_Web_App.Repositories;
using Calendar_Web_App.ViewModels.EventViewModels;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using Xunit.Sdk;

namespace Calendar_App_Tests
{
	public class EventRepositoryTests
	{
		private readonly EventRepository _repository;
		private readonly Mock<ILogger<EventRepository>> _mockLogger;
		private readonly Mock<IApplicationDbContext> _mockContext;
		private readonly Mock<DbSet<Event>> _MockEventsDbSet;


		public EventRepositoryTests() {
			_mockLogger = new Mock<ILogger<EventRepository>>();
			_mockContext = new Mock<IApplicationDbContext>();
			_repository = new EventRepository(_mockContext.Object, _mockLogger.Object);
			_MockEventsDbSet = new Mock<DbSet<Event>>();
		}

		[Theory]
		[InlineData("user1", 2)]
		[InlineData("user2", 0)]
		public void EventRepository_GetAllEvents_ReturnEventsOrNoEvents(string userId, int expectedEventCount)
		{
			//Arrange

			var events = new List<Event>{
				new Event {Id = "event1", UserId = "user1"},
				new Event {Id = "event2", UserId = "user1"}
			}.AsQueryable();

			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(events.Provider);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(events.Expression);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(events.ElementType);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator());


			_MockEventsDbSet.Setup(m => m.Find(It.IsAny<Event>()))
				.Returns((string UserId) =>
				{
					return events.FirstOrDefault(e => e.UserId == UserId);
				});

			_mockContext.Setup(c => c.Events).Returns(_MockEventsDbSet.Object);

			// Act
			var result = _repository.GetAllEvents(userId);

			// Assert
			Assert.Equal(expectedEventCount, result.Count());

			if (expectedEventCount > 0)
			{
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Events for {userId} were successfully retrieved")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()),
					Times.Once);
			}
			else
			{
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"user {userId} does have any events")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()),
					Times.Once);
			}


		}


		[Fact]
		public void EventRepository_GetAllEvents_ThrowsInvalidOperationException()
		{
			//Arrange 
			var userId = "user1";
			var exceptionMessage = "Exception Message";

			_mockContext.Setup(c => c.Events)
				.Throws(new InvalidOperationException(exceptionMessage));

			//Act

			var exception = Assert.Throws<InvalidOperationException>(() => _repository.GetAllEvents(userId));

			//Assert
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An InvalidOperationException occurred while trying to retrieve events for user {userId}")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

			Assert.Equal(exceptionMessage, exception.Message);
		}

		[Theory]
		[InlineData("event1", true)]
		[InlineData("event2", true)] 
		[InlineData("event3", false)] 
		public void EventRepository_GetEventById_ReturnsEventOrNullWhenDoesNotExist(string eventId, bool EventExists)
		{
			//Arrange
			var events = new List<Event>
			{
				new Event { Id = "event1", UserId = "userId1", title = "Event1Title" },
				new Event { Id = "event2", UserId = "userId2", title = "Event2Title" }
			}.AsQueryable();


			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(events.Provider);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(events.Expression);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(events.ElementType);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator());

			_MockEventsDbSet.Setup(m => m.Find(It.IsAny<string>()))
				.Returns((string EventId) =>
				{
					return events.FirstOrDefault(e => e.Id == EventId);
				});
			
			
			_mockContext.Setup(c => c.Events).Returns(_MockEventsDbSet.Object);


			// Act
			var result = _repository.GetEventById(eventId);

			//Assert
			if (EventExists)
			{
				Assert.NotNull(result);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"event {eventId} was successfully found")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
			else
			{
				Assert.Null(result);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event {eventId} does not exist")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}

		}

		[Fact]
		public void EventRepository_GetEventById_ThrowsInvalidOperationException()
		{
			//Arrange
			var eventId = "user1";
			var exceptionMessage = "Exception Message";

			_mockContext.Setup(e => e.Events)
				.Throws(new InvalidOperationException(exceptionMessage));

			//Act
			var exception = Assert.Throws<InvalidOperationException>(() => _repository.GetEventById(eventId));

			//Assert
			Assert.Equal(exception.Message, exceptionMessage);
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An InvalidOperationException occurred while trying to retrieve event using {eventId}")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
		}

		[Theory]
		[InlineData("user1", true)]
		[InlineData(null, false)]
		public void EventRepository_AddEvent_AddsEventToDatabaseIfUserIdNotNullElseDoesNothing(string userId, bool AddedEvent) 
		{
			//Arrange
			var EventToAddModel = new AddEventViewModel 
			{ 
			UserId = userId,
			Description = "testDescription",
			Title = "Test Title",
			StartDate = DateTime.Now,
			EndDate = DateTime.Now.AddHours(1),
			};

			_mockContext.Setup(c => c.Events).Returns(_MockEventsDbSet.Object);

			_MockEventsDbSet.Setup(m => m.Add(It.IsAny<Event>())).Verifiable();


			//Act
			_repository.AddEvent(EventToAddModel);

			//Assert
			if (AddedEvent)
			{
				_mockContext.Verify(m => m.SaveChanges(), Times.Once);
				_mockContext.Verify(m => m.Events.Add(It.IsAny<Event>()), Times.Once);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"New Event was successfully added")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()),
					Times.Once);
						
			}
			else
			{
				_mockContext.Verify(m => m.SaveChanges(), Times.Never);
				_mockContext.Verify(m => m.Events.Add(It.IsAny<Event>()), Times.Never);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"User does not exist")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()),
					Times.Once);
			}

		}

		[Fact]
		public void EventRepository_AddEvent_ThrowsInvalidOperationException()
		{
			//Arrange
			var exceptionMessage = "exceptionMessage";

			_mockContext.Setup(e => e.Events).Throws(new InvalidOperationException(exceptionMessage));

			var EventToAddModel = new AddEventViewModel
			{
				UserId = "user1",
				Description = "testDescription",
				Title = "Test Title",
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddHours(1),
			};

			//Act
			var exception = Assert.Throws<InvalidOperationException>(() => _repository.AddEvent(EventToAddModel));

			//Assert
			Assert.Equal(exception.Message, exceptionMessage);
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occurred while trying to add event")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
		}

		[Theory]
		[InlineData("event1", true)]
		[InlineData("event2", false)]
		public void EventRepository_UpdateEvent_UpdatesEventIfFoundElseDoesNothing(string eventId, bool UpdatedEventSuccessfully)
		{
			//Arrange
			var events = new List<Event>
			{
				new Event {Id = "event1", UserId = "user1", title = "testevent1", start = DateTime.Now, end = DateTime.Now.AddHours(1)}
			}.AsQueryable();


			var eventToUpdate = new UpdateEventViewModel
			{
				EventId = eventId,
			};

			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(events.Provider);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(events.Expression);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(events.ElementType);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator());

			_MockEventsDbSet.Setup(m => m.Find(It.IsAny<string>()))
				.Returns((string EventId) =>
				{
					return events.FirstOrDefault(e => e.Id == EventId);
				}); 

			_mockContext.Setup(c => c.Events).Returns(_MockEventsDbSet.Object);

			//Act
			_repository.UpdateEvent(eventToUpdate);

			//Assert
			if (UpdatedEventSuccessfully)
			{
				_mockContext.Verify(m => m.SaveChanges(), Times.Once);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event {eventId} was successfully updated")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()),
						Times.Once);

			}
			else
			{
				_mockContext.Verify(m => m.SaveChanges(), Times.Never);
				_mockContext.Verify(m => m.Events.Update(It.IsAny<Event>()), Times.Never);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event with Id {eventId} does not exist")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()),
					Times.Once);
			}
		}

		[Fact]
		public void EventRepository_UpdateEvent_ThrowsInvalidOperationException()
		{
			//Arrange
			var exceptionMessage = "exceptionMessage";

			_mockContext.Setup(e => e.Events).Throws(new InvalidOperationException(exceptionMessage));

			var eventToUpdate = new UpdateEventViewModel
			{
				EventId = "event1"
			};

			//Act
			var exception = Assert.Throws<InvalidOperationException>(() => _repository.UpdateEvent(eventToUpdate));

			//Assert
			Assert.Equal(exception.Message, exceptionMessage);
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occurred while trying to update event")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Theory]
		[InlineData("event1", true)]
		[InlineData("event2", false)]
		public void EventRepository_RemoveEvent_RemovesEventIfExistsElseDoesNothing(string eventId, bool RemovedEventSuccessfully)
		{
			//Arrange
			var events = new List<Event>
			{
				new Event {Id = "event1", UserId = "user1", title = "testevent1", start = DateTime.Now, end = DateTime.Now.AddHours(1)}
			}.AsQueryable();


			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(events.Provider);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(events.Expression);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(events.ElementType);
			_MockEventsDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator());

			_MockEventsDbSet.Setup(m => m.Find(It.IsAny<string>()))
				.Returns((string EventId) =>
				{
					return events.FirstOrDefault(e => e.Id == EventId);
				});

			_mockContext.Setup(c => c.Events).Returns(_MockEventsDbSet.Object);

			//Act
			_repository.RemoveEvent(eventId);

			//Assert
			if (RemovedEventSuccessfully)
			{
				_mockContext.Verify(m => m.SaveChanges(), Times.Once);
				_mockContext.Verify(m => m.Events.Remove(It.IsAny<Event>()), Times.Once);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event with Id {eventId} was removed successfully")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
			else
			{
				_mockContext.Verify(m => m.SaveChanges(), Times.Never);
				_mockContext.Verify(m => m.Events.Remove(It.IsAny<Event>()), Times.Never);
				_mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event with Id {eventId} not found")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
		}

		[Fact]
		public void EventRepository_RemoveEvent_ThrowInvalidOperationException()
		{
			//Arrange
			var exceptionMessage = "exceptionMessage";

			_mockContext.Setup(c => c.Events).Throws(new InvalidOperationException(exceptionMessage));

			var eventId = "event1";

			//Act
			var exception = Assert.Throws<InvalidOperationException>(() => _repository.RemoveEvent(eventId));

			//Assert
			Assert.Equal(exception.Message, exceptionMessage);
			_mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occurred while trying to update event")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

		}

	}
}

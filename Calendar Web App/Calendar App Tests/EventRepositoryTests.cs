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

		[Theory]
		[InlineData("user1", 2)]
		[InlineData("user2", 0)]
		public void EventRepository_GetAllEvents_ReturnEventsOrNoEvents(string userId, int expectedEventCount)
		{
			//Arrange

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "InMemoryEventRepositoryTest")
				.Options;

			var mockLogger = new Mock<ILogger<EventRepository>>();

			var context = new ApplicationDbContext(options);

			context.Events.AddRange(
				new Event { Id = "1", UserId = "user1", title = "Event 1" },
				new Event { Id = "2", UserId = "user1", title = "Event 2" }
			);

			context.SaveChanges();


			var repository = new EventRepository(context, mockLogger.Object);

			// Act
			var result = repository.GetAllEvents(userId);

			// Assert
			Assert.Equal(expectedEventCount, result.Count());

			if (expectedEventCount > 0)
			{
				mockLogger.Verify(
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
				mockLogger.Verify(
					x => x.Log(
						LogLevel.Warning,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"user {userId} does have any events")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()),
					Times.Once);
			}

			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
			context.Dispose();

		}


		[Fact]
		public void EventRepository_GetAllEvents_ThrowsInvalidOperationException()
		{
			//Arrange 


			var userId = "user1";
			var exceptionMessage = "Exception Message";
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();
			var repository = new EventRepository(mockContext.Object, mockLogger.Object);

			mockContext.Setup(c => c.Events)
				.Throws(new InvalidOperationException(exceptionMessage));

			//Act

			var exception = Assert.Throws<InvalidOperationException>(() => repository.GetAllEvents(userId));

			//Assert
			mockLogger.Verify(
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
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();

			var events = new List<Event>
			{
				new Event { Id = "event1", UserId = "userId1", title = "Event1Title" },
				new Event { Id = "event2", UserId = "userId2", title = "Event2Title" }
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Event>>();

			mockSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(events.Provider);
			mockSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(events.Expression);
			mockSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(events.ElementType);
			mockSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator());

			mockSet.Setup(m => m.Find(It.IsAny<string>()))
				.Returns((string EventId) =>
				{
					return events.FirstOrDefault(e => e.Id == EventId);
				});
			
			mockContext.Setup(c => c.Events).Returns(mockSet.Object);

			var eventRepository = new EventRepository(mockContext.Object, mockLogger.Object);

			// Act
			var result = eventRepository.GetEventById(eventId);

			//Assert
			if (EventExists)
			{
				Assert.NotNull(result);
				mockLogger.Verify(
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
				mockLogger.Verify(
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
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();
			var repository = new EventRepository(mockContext.Object, mockLogger.Object);


			mockContext.Setup(e => e.Events)
				.Throws(new InvalidOperationException(exceptionMessage));

			//Act
			var exception = Assert.Throws<InvalidOperationException>(() => repository.GetEventById(eventId));

			//Assert
			Assert.Equal(exception.Message, exceptionMessage);
			mockLogger.Verify(
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
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();
			var repository = new EventRepository(mockContext.Object, mockLogger.Object);
			var mockSet = new Mock<DbSet<Event>>();

			var EventToAddModel = new AddEventViewModel 
			{ 
			UserId = userId,
			Description = "testDescription",
			Title = "Test Title",
			StartDate = DateTime.Now,
			EndDate = DateTime.Now.AddHours(1),
			};

			mockContext.Setup(c => c.Events).Returns(mockSet.Object);

			mockSet.Setup(m => m.Add(It.IsAny<Event>())).Verifiable();


			//Act
			repository.AddEvent(EventToAddModel);

			//Assert
			if (AddedEvent)
			{
				mockContext.Verify(m => m.SaveChanges(), Times.Once);
				mockContext.Verify(m => m.Events.Add(It.IsAny<Event>()), Times.Once);
				mockLogger.Verify(
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
				mockContext.Verify(m => m.SaveChanges(), Times.Never);
				mockContext.Verify(m => m.Events.Add(It.IsAny<Event>()), Times.Never);
				mockLogger.Verify(
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
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();
			var repository = new EventRepository(mockContext.Object, mockLogger.Object);
			var exceptionMessage = "exceptionMessage";

			mockContext.Setup(e => e.Events).Throws(new InvalidOperationException(exceptionMessage));

			var EventToAddModel = new AddEventViewModel
			{
				UserId = "user1",
				Description = "testDescription",
				Title = "Test Title",
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddHours(1),
			};

			//Act
			var exception = Assert.Throws<InvalidOperationException>(() => repository.AddEvent(EventToAddModel));

			//Assert
			Assert.Equal(exception.Message, exceptionMessage);
			mockLogger.Verify(
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
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();
			var repository = new EventRepository(mockContext.Object, mockLogger.Object);

			var events = new List<Event>
			{
				new Event {Id = "event1", UserId = "user1", title = "testevent1", start = DateTime.Now, end = DateTime.Now.AddHours(1)}
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Event>>();

			var eventToUpdate = new UpdateEventViewModel
			{
				EventId = eventId,
			};

			mockSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(events.Provider);
			mockSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(events.Expression);
			mockSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(events.ElementType);
			mockSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator());

			mockSet.Setup(m => m.Find(It.IsAny<string>()))
				.Returns((string EventId) =>
				{
					return events.FirstOrDefault(e => e.Id == EventId);
				}); 

			mockContext.Setup(c => c.Events).Returns(mockSet.Object);

			//Act
			repository.UpdateEvent(eventToUpdate);

			//Assert
			if (UpdatedEventSuccessfully)
			{
				mockContext.Verify(m => m.SaveChanges(), Times.Once);
				mockLogger.Verify(
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
				mockContext.Verify(m => m.SaveChanges(), Times.Never);
				mockContext.Verify(m => m.Events.Update(It.IsAny<Event>()), Times.Never);
				mockLogger.Verify(
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
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();
			var repository = new EventRepository(mockContext.Object, mockLogger.Object);
			var exceptionMessage = "exceptionMessage";

			mockContext.Setup(e => e.Events).Throws(new InvalidOperationException(exceptionMessage));

			var eventToUpdate = new UpdateEventViewModel
			{
				EventId = "event1"
			};

			//Act
			var exception = Assert.Throws<InvalidOperationException>(() => repository.UpdateEvent(eventToUpdate));

			//Assert
			Assert.Equal(exception.Message, exceptionMessage);
			mockLogger.Verify(
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
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();
			var repository = new EventRepository(mockContext.Object, mockLogger.Object);

			var events = new List<Event>
			{
				new Event {Id = "event1", UserId = "user1", title = "testevent1", start = DateTime.Now, end = DateTime.Now.AddHours(1)}
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Event>>();


			mockSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(events.Provider);
			mockSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(events.Expression);
			mockSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(events.ElementType);
			mockSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator());

			mockSet.Setup(m => m.Find(It.IsAny<string>()))
				.Returns((string EventId) =>
				{
					return events.FirstOrDefault(e => e.Id == EventId);
				});

			mockContext.Setup(c => c.Events).Returns(mockSet.Object);

			//Act
			repository.RemoveEvent(eventId);

			//Assert
			if (RemovedEventSuccessfully)
			{
				mockContext.Verify(m => m.SaveChanges(), Times.Once);
				mockContext.Verify(m => m.Events.Remove(It.IsAny<Event>()), Times.Once);
				mockLogger.Verify(
					x => x.Log(
						LogLevel.Information,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Event with Id {eventId} was removed successfully")),
						null,
						It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			}
			else
			{
				mockContext.Verify(m => m.SaveChanges(), Times.Never);
				mockContext.Verify(m => m.Events.Remove(It.IsAny<Event>()), Times.Never);
				mockLogger.Verify(
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
			var mockLogger = new Mock<ILogger<EventRepository>>();
			var mockContext = new Mock<IApplicationDbContext>();
			var repository = new EventRepository(mockContext.Object, mockLogger.Object);
			var exceptionMessage = "exceptionMessage";

			mockContext.Setup(c => c.Events).Throws(new InvalidOperationException(exceptionMessage));

			var eventId = "event1";

			//Act
			var exception = Assert.Throws<InvalidOperationException>(() => repository.RemoveEvent(eventId));

			//Assert
			Assert.Equal(exception.Message, exceptionMessage);
			mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An InvalidOperationException occurred while trying to update event")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

		}

	}
}

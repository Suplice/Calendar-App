using Calendar_Web_App.Data;
using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Models;
using Calendar_Web_App.Repositories;
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
		public void EventRepository_GetAllEvents_ThrowsException()
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

			mockLogger.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An InvalidOperationException occurred while trying to retrieve events for user {userId}")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

			Assert.Equal(exceptionMessage, exception.Message);
		}


		public void EventRepository_GetEventById_ReturnsEvent()
		{

		}
	}
}
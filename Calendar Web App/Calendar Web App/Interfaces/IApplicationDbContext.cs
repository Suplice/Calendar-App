using Calendar_Web_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendar_Web_App.Interfaces
{
	public interface IApplicationDbContext
	{
		DbSet<Event> Events { get; set; }
		DbSet<User> Users { get; set; }

		int SaveChanges();

	}
}

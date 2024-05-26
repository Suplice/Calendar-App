using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Calendar_Web_App.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

		public new DbSet<Event> Events { get; set; }
        public new DbSet<User> Users { get; set; }

       public new int SaveChanges() {
            return base.SaveChanges();
        }

	}
}

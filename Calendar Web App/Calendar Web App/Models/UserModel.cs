using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Calendar_Web_App.Models
{
    public class User : IdentityUser
    {
        [Key]
        public override string Id { get; set; }


        public required string Name { get; set; }

        public override required string Email { get; set; }

        public override required string UserName { get; set; }    

        public ICollection<Event> Events { get; set; }

    }
}

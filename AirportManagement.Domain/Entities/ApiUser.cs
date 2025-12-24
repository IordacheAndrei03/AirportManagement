using Microsoft.AspNetCore.Identity;

namespace AirportManagement.Domain.Entities
{
    public class ApiUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }
}

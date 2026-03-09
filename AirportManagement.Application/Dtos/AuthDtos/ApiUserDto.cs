using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.AuthDtos
{
    public class ApiUserDto:LoginDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }
    }
}

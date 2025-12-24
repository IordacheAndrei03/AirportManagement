using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.Flights
{
    public class FlightCreateDto
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string AirlineIata { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string FlightNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string OriginIata { get; set; } = string.Empty;

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string DestinationIata { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DefaultAircraftTail { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}

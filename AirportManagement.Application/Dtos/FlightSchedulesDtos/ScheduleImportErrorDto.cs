
namespace AirportManagement.Application.Dtos.FlightSchedulesDtos
{
    public class ScheduleImportErrorDto
    {
        public int Row { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}

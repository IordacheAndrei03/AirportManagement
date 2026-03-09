using AirportManagement.Application.Enums;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IFlightStatusRepository
    {
        Task<int> GetStatusIdByNameAsync(FlightScheduleStatus status);
    }
}

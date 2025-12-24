using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using DomainFlightSchedule = AirportManagement.Domain.Entities.FlightSchedule;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IFlightScheduleRepository : IGenericRepository<DomainFlightSchedule>
    {
        Task<DomainFlightSchedule?> GetByIdWithDetailsAsync(int id);

        Task<IReadOnlyList<DomainFlightSchedule>> SearchUpcomingByRouteAndDateAsync(string originIata, string destinationIata, DateTime departureDateUtc, int page, int pageSize);

        Task<bool> HasGateOverlapAsync(int gateId, DateTime fromUtc);

        Task<bool> AnyByFlightIdAsync(int flightId);

        Task<IReadOnlyList<UpcomingSchedulesDto>> GetUpcomingStatsAsync(int days);

        Task<DomainFlightSchedule?> FindByFlightAndDepartureAsync(int flightId, DateTime departureUtc);

        Task<int> GetSeatCapacityAsync(int flightScheduleId);

        Task<int> GetActiveBookedSeatsAsync(int flightScheduleId);
    }
}


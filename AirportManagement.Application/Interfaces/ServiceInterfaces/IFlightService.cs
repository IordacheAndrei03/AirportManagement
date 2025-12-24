using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Results;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface IFlightService
    {
        Task<ResultObject<FlightDetailsDto>> GetByIdAsync(int id);

        Task<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>> SearchByRouteAndDateAsync(string originIata, string destinationIata, DateOnly departureDate, int page, int pageSize);

        Task<ResultObject<int>> CreateFlightAsync(FlightCreateDto flightCreateDto);

        Task<Result> UpdateAsync(int id, FlightCreateDto flightCreateDto);

        Task<Result> DeleteAsync(int id);
    }
}

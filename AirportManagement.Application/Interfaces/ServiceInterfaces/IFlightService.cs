using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface IFlightService
    {
        Task<ResultObject<FlightDetailsDto>> GetByIdAsync(int id);

        Task<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>> SearchByRouteAndDateAsync(
           string originIata,
           string destinationIata,
           DateOnly departureDate,
           int page,
           int pageSize);

        Task<int> CreateFlightAsync(FlightCreateDto flightCreateDto);

        Task UpdateAsync(int id, FlightCreateDto flightCreateDto);

        Task DeleteAsync(int id);
    }
}

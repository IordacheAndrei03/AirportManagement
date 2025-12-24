using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
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

        Task<ResultObject<int>> CreateFlightAsync(FlightCreateDto flightCreateDto);

        Task<Result> UpdateAsync(int id, FlightCreateDto flightCreateDto);

        Task<Result> DeleteAsync(int id);
    }
}

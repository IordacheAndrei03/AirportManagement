using AirportManagement.Application.Dtos;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Results;
using Microsoft.AspNetCore.Http;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface IFlightScheduleService
    {
        Task<ResultObject<FlightScheduleDetailsDto>> GetByIdAsync(int id);

        Task<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>> GetUpcomingStatsAsync(int days);

        Task<ResultObject<int>> CreateAsync(FlightScheduleCreateDto dto);

        Task<ResultObject<ScheduleImportResultDto>> ImportAsync(IFormFile file);
    }
}

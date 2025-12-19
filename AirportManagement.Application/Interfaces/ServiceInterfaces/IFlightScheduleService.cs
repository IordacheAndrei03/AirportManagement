using AirportManagement.Application.Dtos;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface IFlightScheduleService
    {
        Task<ResultObject<FlightScheduleDetailsDto>> GetByIdAsync(int id);

        Task<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>> GetUpcomingStatsAsync(int days);

        Task<int> CreateAsync(FlightScheduleCreateDto dto);

        Task<ScheduleImportResultDto> ImportAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}

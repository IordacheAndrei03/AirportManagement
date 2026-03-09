using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using Microsoft.AspNetCore.Http;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces.ImportInterfaces
{
    public interface IScheduleImportParser
    {
        Task<ResultObject<List<ScheduleImportRowDto>>> ParseAsync(IFormFile file);
    }
}

using AirportManagement.Application.Dtos.FlightSchedulesDtos;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces.ImportInterfaces
{
    public interface IScheduleImportRowProcessor
    {
        Task ProcessRowAsync(ScheduleImportRowDto row, int plannedStatusId, ScheduleImportResultDto result);
    }
}

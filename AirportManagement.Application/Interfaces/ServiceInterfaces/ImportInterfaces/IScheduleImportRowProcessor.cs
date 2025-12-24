using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces.ImportInterfaces
{
    public interface IScheduleImportRowProcessor
    {
        Task ProcessRowAsync(ScheduleImportRowDto row, int plannedStatusId, ScheduleImportResultDto result);
    }
}

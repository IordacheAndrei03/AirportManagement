using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces.ImportInterfaces
{
    public interface IScheduleImportParser
    {
        Task<ResultObject<List<ScheduleImportRowDto>>> ParseAsync(IFormFile file);
    }
}

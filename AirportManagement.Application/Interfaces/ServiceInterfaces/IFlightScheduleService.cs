using AirportManagement.Application.Dtos.FlightSchedulesDtos;
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
    }
}

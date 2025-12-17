using AirportManagement.Application.Dtos;
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
    }
}

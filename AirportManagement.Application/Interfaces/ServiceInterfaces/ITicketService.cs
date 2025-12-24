using AirportManagement.Application.Dtos.TicketDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface ITicketService
    {
        Task<ResultObject<TicketCreateResponseDto>> CreateAsync(TicketCreateRequestDto dto);

        Task<IReadOnlyList<TicketByFlightScheduleDto>> GetByFlightScheduleAsync(int flightScheduleId);

        Task DeleteAsync(int id);

        Task<ResultObject<TicketSeatUpdateDto>> UpdateSeatNumberAsync(int ticketId, string seatNumber);
    }
}

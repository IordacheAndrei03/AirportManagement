using AirportManagement.Application.Dtos.TicketDtos;
using AirportManagement.Application.Results;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface ITicketService
    {
        Task<ResultObject<TicketCreateResponseDto>> CreateAsync(TicketCreateRequestDto dto);

        Task<ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>> GetByFlightScheduleAsync(int flightScheduleId);

        Task<Result> DeleteAsync(int id);

        Task<ResultObject<TicketSeatUpdateDto>> UpdateSeatNumberAsync(int ticketId, string seatNumber);
    }
}

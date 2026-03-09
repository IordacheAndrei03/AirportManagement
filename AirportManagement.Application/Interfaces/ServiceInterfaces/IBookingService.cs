using AirportManagement.Application.Dtos.BookingDtos;
using AirportManagement.Application.Results;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingService
    {
        Task<ResultObject<BookingCreateResponseDto>> CreateAsync();

        Task<ResultObject<BookingDetailsDto>> GetByCodeAsync(string code);

        Task<Result> CancelAsync(string code);
    }
}

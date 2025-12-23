using AirportManagement.Application.Dtos.BookingDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingService
    {
        Task<BookingCreateResponseDto> CreateAsync();
        Task<BookingDetailsDto?> GetByCodeAsync(string code);
        Task CancelAsync(string code);
    }
}

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
    }
}

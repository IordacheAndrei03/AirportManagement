using AirportManagement.Api.Controllers;
using AirportManagement.Application.Dtos.TicketDtos;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Api.UnitTests.TicketsControllerTest
{
    public abstract class TicketsControllerTestBase
    {
        protected readonly Mock<ITicketService> _ticketService;
        protected readonly TicketsController _controller;

        protected TicketsControllerTestBase()
        {
            _ticketService = new Mock<ITicketService>(MockBehavior.Loose);
            _controller = new TicketsController(_ticketService.Object);
        }

        protected TicketCreateRequestDto CreateValidTicketCreateRequestDto()
        {
            return new TicketCreateRequestDto
            {
                BookingId = 10,
                FlightScheduleId = 20,
                FareClass = "Economy",
                BasePrice = 100,
                Taxes = 20,
                IsRefundable = false,
                SeatNumber = "12A",
                PassengerFullName = "John Doe",
                PassengerEmail = "john@example.com"
            };
        }
    }
}

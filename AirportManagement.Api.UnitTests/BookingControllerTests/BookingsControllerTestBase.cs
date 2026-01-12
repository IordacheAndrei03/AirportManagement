using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportManagement.Api.Controllers;
using AirportManagement.Application.Dtos.BookingDtos;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using Moq;

namespace AirportManagement.Api.UnitTests.BookingControllerTests
{

    public abstract class BookingsControllerTestBase
    {
        protected readonly Mock<IBookingService> _bookingService;
        protected readonly BookingsController _controller;

        protected BookingsControllerTestBase()
        {
            _bookingService = new Mock<IBookingService>(MockBehavior.Loose);
            _controller = new BookingsController(_bookingService.Object);
        }

        protected BookingCreateRequestDto CreateValidBookingCreateRequestDto()
        {
            return new BookingCreateRequestDto();
        }
    }
}

using AirportManagement.Api.Controllers;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Api.UnitTests.FlightScheduleControllerTests
{
    public abstract class FlightScheduleControllerTestBase
    {
        protected readonly Mock<IFlightScheduleService> _scheduleService;
        protected readonly FlightScheduleController _controller;

        protected FlightScheduleControllerTestBase()
        {
            _scheduleService = new Mock<IFlightScheduleService>(MockBehavior.Loose);
            _controller = new FlightScheduleController(_scheduleService.Object);
        }

        protected FlightScheduleCreateDto CreateValidFlightScheduleCreateDto()
        {
            return new FlightScheduleCreateDto
            {
                FlightId = 1,
                GateId = 2,
                AssignedAircraftId = 3,
                ScheduledDepartureUtc = DateTime.UtcNow.AddHours(2),
                ScheduledArrivalUtc = DateTime.UtcNow.AddHours(5)
            };
        }
    }
}

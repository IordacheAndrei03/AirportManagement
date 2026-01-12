using AirportManagement.Api.Controllers;
using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Api.UnitTests.FlightsControllerTest
{
    public abstract class FlightsControllerTestBase
    {
        protected readonly Mock<IFlightService> _flightService;
        protected readonly FlightsController _controller;

        protected FlightsControllerTestBase()
        {
            _flightService = new Mock<IFlightService>(MockBehavior.Loose);
            _controller = new FlightsController(_flightService.Object);
        }

        protected FlightCreateDto CreateValidFlightCreateDto()
        {
            return new FlightCreateDto
            {
                AirlineIata = "AA",
                FlightNumber = "100",
                OriginIata = "OTP",
                DestinationIata = "LHR",
                DefaultAircraftTail = "YR-ABC",
                IsActive = true
            };
        }
    }
}

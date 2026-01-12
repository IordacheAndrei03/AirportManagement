using AirportManagement.Application;
using AirportManagement.Application.Dtos.TicketDtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Api.UnitTests.TicketsControllerTest
{
    public class TicketsControllerCreateTests : TicketsControllerTestBase
    {
        [Fact]
        public async Task Create_WhenOk_Returns201_WithResultObjectInBody()
        {
            var dto = CreateValidTicketCreateRequestDto();

            var responseDto = new TicketCreateResponseDto
            {
                FareClass = "Economy",
                TotalPrice = 120,
                IsRefundable = false,
                SeatNumber = "12A",
                PassengerFullName = "John Doe"
            };

            var serviceResult = ResultObject<TicketCreateResponseDto>.Success(responseDto);

            _ticketService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(serviceResult);

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<TicketCreateResponseDto>>(objectResult.Value);
            Assert.True(returned.IsSuccess);
            Assert.Equal(120, returned.Value!.TotalPrice);

            _ticketService.Verify(s => s.CreateAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Create_WhenInvalid_Returns400()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _ticketService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<TicketCreateResponseDto>.Invalid("invalid"));

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<ResultObject<TicketCreateResponseDto>>(badRequest.Value);
        }

        [Fact]
        public async Task Create_WhenNotFound_Returns404()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _ticketService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<TicketCreateResponseDto>.NotFound("not found"));

            var result = await _controller.Create(dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.IsType<ResultObject<TicketCreateResponseDto>>(notFound.Value);
        }

        [Fact]
        public async Task Create_WhenConflict_Returns409()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _ticketService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<TicketCreateResponseDto>.Conflict("conflict"));

            var result = await _controller.Create(dto);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<TicketCreateResponseDto>>(conflict.Value);
        }

        [Fact]
        public async Task Create_WhenForbidden_Returns403()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _ticketService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<TicketCreateResponseDto>.Forbidden("forbidden"));

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<TicketCreateResponseDto>>(objectResult.Value);
        }
    }
}

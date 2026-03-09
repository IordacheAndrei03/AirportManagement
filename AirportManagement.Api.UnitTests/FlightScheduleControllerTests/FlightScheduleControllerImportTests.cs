using AirportManagement.Application;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Api.UnitTests.FlightScheduleControllerTests
{
    public class FlightScheduleControllerImportTests : FlightScheduleControllerTestBase
    {
        [Fact]
        public async Task Import_WhenOk_AndNoErrors_Returns201()
        {
            var file = new Mock<IFormFile>().Object;
            var request = new ScheduleImportRequest { File = file };

            var importDto = new ScheduleImportResultDto
            {
                Total = 2,
                Created = 2,
                Updated = 0,
                Errors = new List<ScheduleImportErrorDto>() // empty
            };

            var serviceResult = ResultObject<ScheduleImportResultDto>.Success(importDto);

            _scheduleService.Setup(s => s.ImportAsync(file))
                .ReturnsAsync(serviceResult);

            var result = await _controller.Import(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<ScheduleImportResultDto>>(objectResult.Value);
            Assert.True(returned.IsSuccess);
            Assert.Empty(returned.Value!.Errors);

            _scheduleService.Verify(s => s.ImportAsync(file), Times.Once);
        }

        [Fact]
        public async Task Import_WhenOk_ButHasErrors_Returns207()
        {
            var file = new Mock<IFormFile>().Object;
            var request = new ScheduleImportRequest { File = file };

            var importDto = new ScheduleImportResultDto
            {
                Total = 2,
                Created = 1,
                Updated = 0,
                Errors = new List<ScheduleImportErrorDto>
            {
                new ScheduleImportErrorDto { Row = 1, Message = "Bad row" }
            }
            };

            var serviceResult = ResultObject<ScheduleImportResultDto>.Success(importDto);

            _scheduleService.Setup(s => s.ImportAsync(file))
                .ReturnsAsync(serviceResult);

            var result = await _controller.Import(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(207, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<ScheduleImportResultDto>>(objectResult.Value);
            Assert.True(returned.IsSuccess);
            Assert.NotEmpty(returned.Value!.Errors);
        }

        [Fact]
        public async Task Import_WhenInvalid_Returns207()
        {
            var file = new Mock<IFormFile>().Object;
            var request = new ScheduleImportRequest { File = file };

            _scheduleService.Setup(s => s.ImportAsync(file))
                .ReturnsAsync(ResultObject<ScheduleImportResultDto>.Invalid("File is empty."));

            var result = await _controller.Import(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(207, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<ScheduleImportResultDto>>(objectResult.Value);
            Assert.False(returned.IsSuccess);
            Assert.Equal(ResultStatus.Invalid, returned.Status);
        }

        [Fact]
        public async Task Import_WhenNotFound_Returns207()
        {
            var file = new Mock<IFormFile>().Object;
            var request = new ScheduleImportRequest { File = file };

            _scheduleService.Setup(s => s.ImportAsync(file))
                .ReturnsAsync(ResultObject<ScheduleImportResultDto>.NotFound("not found"));

            var result = await _controller.Import(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(207, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<ScheduleImportResultDto>>(objectResult.Value);
            Assert.False(returned.IsSuccess);
            Assert.Equal(ResultStatus.NotFound, returned.Status);
        }

        [Fact]
        public async Task Import_WhenConflict_Returns207()
        {
            var file = new Mock<IFormFile>().Object;
            var request = new ScheduleImportRequest { File = file };

            _scheduleService.Setup(s => s.ImportAsync(file))
                .ReturnsAsync(ResultObject<ScheduleImportResultDto>.Conflict("conflict"));

            var result = await _controller.Import(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(207, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<ScheduleImportResultDto>>(objectResult.Value);
            Assert.False(returned.IsSuccess);
            Assert.Equal(ResultStatus.Conflict, returned.Status);
        }

        [Fact]
        public async Task Import_WhenForbidden_Returns207()
        {
            var file = new Mock<IFormFile>().Object;
            var request = new ScheduleImportRequest { File = file };

            _scheduleService.Setup(s => s.ImportAsync(file))
                .ReturnsAsync(ResultObject<ScheduleImportResultDto>.Forbidden("forbidden"));

            var result = await _controller.Import(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(207, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<ScheduleImportResultDto>>(objectResult.Value);
            Assert.False(returned.IsSuccess);
            Assert.Equal(ResultStatus.Forbidden, returned.Status);
        }
    }
}

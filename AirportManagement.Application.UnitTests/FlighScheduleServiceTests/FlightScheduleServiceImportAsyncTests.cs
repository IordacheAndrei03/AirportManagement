using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Application.UnitTests.FlighScheduleServiceTests
{
    public class FlightScheduleServiceImportAsyncTests : FlightScheduleServiceTestBase
    {
        [Fact]
        public async Task ImportAsync_WhenParserInvalid_ReturnsInvalid()
        {
            var file = new Mock<IFormFile>().Object;

            _scheduleImportParser
                .Setup(x => x.ParseAsync(file))
                .ReturnsAsync(ResultObject<List<ScheduleImportRowDto>>.Invalid("bad file"));

            var result = await _flightScheduleService.ImportAsync(file);

            Assert.False(result.IsSuccess);
            Assert.Equal("bad file", result.Error);
        }

        [Fact]
        public async Task ImportAsync_WhenParserSuccess_ProcessesAllRows_AndReturnsSuccess()
        {
            var file = new Mock<IFormFile>().Object;

            var rows = new List<ScheduleImportRowDto>
             {
                 new ScheduleImportRowDto(),
                 new ScheduleImportRowDto(),
             };

            _scheduleImportParser
                .Setup(x => x.ParseAsync(file))
                .ReturnsAsync(ResultObject<List<ScheduleImportRowDto>>.Success(rows));
            ArrangeScheduledStatusId(12);

            _scheduleImportRowProcessor
                .Setup(x => x.ProcessRowAsync(rows[0], 12, It.IsAny<ScheduleImportResultDto>()))
                .Returns(Task.CompletedTask);

            _scheduleImportRowProcessor
                .Setup(x => x.ProcessRowAsync(rows[1], 12, It.IsAny<ScheduleImportResultDto>()))
                .Returns(Task.CompletedTask);

            var result = await _flightScheduleService.ImportAsync(file);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value!.Total);
            Assert.Empty(result.Value.Errors);
        }

        [Fact]
        public async Task ImportAsync_WhenOneRowThrows_AddsErrorAndContinues()
        {
            var file = new Mock<IFormFile>().Object;
            var rows = new List<ScheduleImportRowDto>
            {
                new ScheduleImportRowDto(),
                new ScheduleImportRowDto(), 
                new ScheduleImportRowDto()  
            };
          
            _scheduleImportParser
                .Setup(x => x.ParseAsync(file))
                .ReturnsAsync(ResultObject<List<ScheduleImportRowDto>>.Success(rows));
            ArrangeScheduledStatusId(12);

            _scheduleImportRowProcessor
                .Setup(x => x.ProcessRowAsync(rows[0], 12, It.IsAny<ScheduleImportResultDto>()))
                .Returns(Task.CompletedTask);

            _scheduleImportRowProcessor
                .Setup(x => x.ProcessRowAsync(rows[1], 12, It.IsAny<ScheduleImportResultDto>()))
                .ThrowsAsync(new Exception("boom"));

            _scheduleImportRowProcessor
                .Setup(x => x.ProcessRowAsync(rows[2], 12, It.IsAny<ScheduleImportResultDto>()))
                .Returns(Task.CompletedTask);

            var result = await _flightScheduleService.ImportAsync(file);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value!.Total);
            Assert.Single(result.Value.Errors);
            Assert.Equal(2, result.Value.Errors[0].Row); 
            Assert.Contains("boom", result.Value.Errors[0].Message);
        }
    }
}

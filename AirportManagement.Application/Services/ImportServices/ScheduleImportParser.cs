using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Interfaces.ServiceInterfaces.ImportInterfaces;
using AirportManagement.Application.Results;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AirportManagement.Application.Services.ImportServices
{
    public sealed class ScheduleImportParser : IScheduleImportParser
    {
        public async Task<ResultObject<List<ScheduleImportRowDto>>> ParseAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ResultObject<List<ScheduleImportRowDto>>.Invalid("File is empty.");
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                return ResultObject<List<ScheduleImportRowDto>>.Invalid("File is too large. Max 2 MB.");
            }

            List<ScheduleImportRowDto>? rows;

            using (var stream = file.OpenReadStream())
            {
                rows = await JsonSerializer.DeserializeAsync<List<ScheduleImportRowDto>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (rows == null || rows.Count == 0)
            {
                return ResultObject<List<ScheduleImportRowDto>>.Invalid("File does not contain any schedules.");
            }

            if (rows.Count > 1000)
            {
                return ResultObject<List<ScheduleImportRowDto>>.Invalid("File contains more than 1000 rows (limit 1000).");
            }

            return ResultObject<List<ScheduleImportRowDto>>.Success(rows);
        }
    }
}

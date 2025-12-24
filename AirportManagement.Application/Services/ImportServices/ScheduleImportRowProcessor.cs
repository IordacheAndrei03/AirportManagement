using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces.ImportInterfaces;
using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Services.ImportServices
{
    public sealed class ScheduleImportRowProcessor : IScheduleImportRowProcessor
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleImportRowProcessor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ProcessRowAsync(ScheduleImportRowDto row, int plannedStatusId, ScheduleImportResultDto result)
        {
            if (row.ScheduledArrivalUtc <= row.ScheduledDepartureUtc)
                throw new Exception("Arrival must be after departure.");

            var airline = await _unitOfWork.AirlineRepository.GetByIataCodeAsync(row.AirlineIata)
                ?? throw new Exception($"Unknown airline IATA code '{row.AirlineIata}'.");

            var originAirport = await _unitOfWork.AirportRepository.GetByIataCodeAsync(row.OriginIata)
                ?? throw new Exception($"Unknown origin airport IATA code '{row.OriginIata}'.");

            var destAirport = await _unitOfWork.AirportRepository.GetByIataCodeAsync(row.DestinationIata)
                ?? throw new Exception($"Unknown destination airport IATA code '{row.DestinationIata}'.");

            var aircraft = await _unitOfWork.AircraftRepository.GetByTailNoAsync(row.AssignedAircraftTail)
                ?? throw new Exception($"Unknown aircraft tail number '{row.AssignedAircraftTail}'.");

            var flight = await _unitOfWork.FlightRepository.FindByAirlineNumberAndRouteAsync(
                airline.Id, row.FlightNumber, originAirport.Id, destAirport.Id);

            if (flight == null)
            {
                var newFlight = new Flight
                {
                    AirlineId = airline.Id,
                    FlightNumber = row.FlightNumber,
                    OriginAirport = originAirport.Id,
                    DestinationAirport = destAirport.Id,
                    DefaultAircraftId = aircraft.Id,
                    IsActive = true
                };

                await _unitOfWork.FlightRepository.AddAsync(newFlight);
                await _unitOfWork.SaveChangesAsync();
                flight = newFlight;
            }

            var gate = await _unitOfWork.GateRepository.GetByAirportIdAndCodeAsync(originAirport.Id, row.GateCode);
            if (gate == null)
                throw new Exception("Gate does not exist.");

            var hasOverlap = await _unitOfWork.FlightScheduleRepository.HasGateOverlapAsync(gate.Id, row.ScheduledDepartureUtc);
            if (hasOverlap)
                throw new Exception($"Gate overlap at {row.OriginIata}:{row.GateCode} {row.ScheduledDepartureUtc:O}–{row.ScheduledArrivalUtc:O}");

            var existing = await _unitOfWork.FlightScheduleRepository.FindByFlightAndDepartureAsync(flight.Id, row.ScheduledDepartureUtc);

            if (existing == null)
            {
                var schedule = new FlightSchedule
                {
                    FlightId = flight.Id,
                    ScheduledDepartureUtc = row.ScheduledDepartureUtc,
                    ScheduleArrivalUtc = row.ScheduledArrivalUtc,
                    GateId = gate.Id,
                    AssignedAircraftId = aircraft.Id,
                    FlightStatusId = plannedStatusId
                };

                await _unitOfWork.FlightScheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
                result.Created++;
            }
            else
            {
                existing.ScheduleArrivalUtc = row.ScheduledArrivalUtc;
                existing.GateId = gate.Id;
                existing.AssignedAircraftId = aircraft.Id;
                existing.FlightStatusId = plannedStatusId;

                await _unitOfWork.SaveChangesAsync();
                result.Updated++;
            }
        }
    }
}

using AirportManagement.Application.Dtos.TicketDtos;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Services
{
    public sealed class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public TicketService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultObject<TicketCreateResponseDto>> CreateAsync(TicketCreateRequestDto dto)
        {
            if (dto.BasePrice <= 0)
                throw new ArgumentException("BasePrice must be positive.");

            if (dto.Taxes < 0)
                throw new ArgumentException("Taxes must be non-negative.");

            var booking = await _unitOfWork.BookingRepository.GetByIdWithStatusAsync(dto.BookingId);
            if (booking is null)
                throw new KeyNotFoundException($"Booking {dto.BookingId} not found.");

            var userId = _currentUserService.UserId;

            if (!string.Equals(booking.UserId, userId, StringComparison.Ordinal))
                throw new InvalidOperationException("Cannot add ticket to another user's booking.");


            if (!string.Equals(booking.BookingStatus?.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot add tickets to a non-active booking.");

            var schedule = await _unitOfWork.FlightScheduleRepository.GetByIdAsync(dto.FlightScheduleId);
            if (schedule is null)
                throw new KeyNotFoundException($"FlightSchedule {dto.FlightScheduleId} not found.");

            var capacity = await _unitOfWork.FlightScheduleRepository.GetSeatCapacityAsync(dto.FlightScheduleId);
            if (capacity <= 0)
                throw new InvalidOperationException("Aircraft capacity invalid.");

            var soldSeats = await _unitOfWork.FlightScheduleRepository.GetActiveBookedSeatsAsync(dto.FlightScheduleId);
            if (soldSeats + 1 > capacity)
                throw new InvalidOperationException("Not enough seats available for this flight schedule.");

            var ticket = new Ticket
            {
                BookingId = booking.Id,
                FlightScheduleId = dto.FlightScheduleId,
                FareClass = dto.FareClass,
                BasePrice = dto.BasePrice,
                Taxes = dto.Taxes,
                TotalPrice = dto.BasePrice + dto.Taxes,
                Currency = "EUR",
                IsRefundable = dto.IsRefundable,
                SeatNumber = dto.SeatNumber,
                PassangerFullName = dto.PassengerFullName,
                PassangerEmail = dto.PassengerEmail,
            };

            await _unitOfWork.TicketRepository.AddAsync(ticket);

            booking.Quantity += 1;

            await _unitOfWork.SaveChangesAsync();

            var result =  new TicketCreateResponseDto
            {
                FareClass = ticket.FareClass,
                TotalPrice = ticket.TotalPrice,
                IsRefundable = ticket.IsRefundable,
                SeatNumber = ticket.SeatNumber,
                PassengerFullName = ticket.PassangerFullName
            };

            return ResultObject<TicketCreateResponseDto>.Success(result);
        }

        public async Task<IReadOnlyList<TicketByFlightScheduleDto>> GetByFlightScheduleAsync(int flightScheduleId)
        {
            if (flightScheduleId <= 0)
                throw new ArgumentException("FlightScheduleId must be positive.", nameof(flightScheduleId));

            var tickets = await _unitOfWork.TicketRepository.GetByFlightScheduleAsync(flightScheduleId);

            return tickets.Select(t => _mapper.Map<TicketByFlightScheduleDto>(t)).ToList();
        }
    }
}

using AirportManagement.Application.Dtos.BookingDtos;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper,ICurrentUserService CurentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = CurentUserService;
        }

        public async Task<BookingCreateResponseDto> CreateAsync(BookingCreateRequestDto dto)
        {
            var userId = _currentUserService.UserId;
            // 1) validări simple
            if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be positive.");

            // 2) verificăm că Ticket există și aparține schedule-ului cerut
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(dto.TicketId);

            if (ticket is null)
                throw new ArgumentException($"Ticket {dto.TicketId} not found.");

            if (ticket.FlightScheduleId != dto.FlightScheduleId)
                throw new ArgumentException("Ticket does not belong to the given flightScheduleId.");

            // 3) capacity check (prevent overbooking)
            var capacity = await _unitOfWork.FlightScheduleRepository.GetSeatCapacityAsync(dto.FlightScheduleId);
            if (capacity <= 0)
                throw new ArgumentException("Flight schedule not found or aircraft capacity invalid.");

            var booked = await _unitOfWork.FlightScheduleRepository.GetActiveBookedSeatsAsync(dto.FlightScheduleId);

            if (booked + dto.Quantity > capacity)
                throw new InvalidOperationException("Not enough seats available for this schedule.");

            // 4) găsim BookingStatus 'Active'
            var activeStatus = await _unitOfWork.BookingRepository.GetByStatusAsync("Active");

            if (activeStatus is null)
                throw new InvalidOperationException("Booking status 'Active' is not configured.");

            // 5) creăm Booking + Ticket update (Ticket are BookingId, deci îl legăm)
            var confirmation = GenerateConfirmationCode();

            var booking = new Booking
            {
                UserId = userId, // Identity user id (string) - adaptează dacă la tine e int
                BookingStatusId = activeStatus.Id,
                CreatedUtc = DateTime.UtcNow,
                ConfirmationCode = confirmation,
                Quantity = dto.Quantity
            };

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Ticket are coloane passenger/seat etc. - le completăm
            // Observație: schema ta obligă SeatNumber NOT NULL -> trebuie să pui ceva.
            // Pentru simplu, punem "AUTO" (sau poți implementa alocare reală mai târziu).
            var ticketToUpdate = await _unitOfWork.TicketRepository.GetByIdAsync(dto.TicketId);

            ticketToUpdate.BookingId = booking.Id;
            ticketToUpdate.PassangerFullName = dto.PassengerFullName;
            ticketToUpdate.PassangerEmail = dto.PassengerEmail;
            ticketToUpdate.SeatNumber = string.IsNullOrWhiteSpace(ticketToUpdate.SeatNumber) ? "AUTO" : ticketToUpdate.SeatNumber;

            await _unitOfWork.SaveChangesAsync();

            var totalAmount = ticket.TotalPrice * dto.Quantity;


            return new BookingCreateResponseDto
            {
                ConfirmationCode = confirmation,
                Status = "Active",
                TotalAmount = totalAmount
            };
        }

        public async Task<BookingDetailsDto?> GetByCodeAsync(string code)
        {
            var booking = await _unitOfWork.BookingRepository.GetByConfirmationCodeAsync(code);
            if (booking is null) return null;

            // luăm și ticketul asociat (în schema ta ticket are BookingId)
            var ticket = await _unitOfWork.TicketRepository.GetByBookingIdAsync(booking.Id);

            if (ticket is null)
            {
                // booking fără ticket e inconsistent, dar nu dăm 500; returnăm minimal
                return new BookingDetailsDto
                {
                    ConfirmationCode = booking.ConfirmationCode,
                    Status = booking.BookingStatus?.Status ?? "Unknown",
                    CreatedUtc = booking.CreatedUtc,
                    Quantity = booking.Quantity,
                    TotalAmount = 0
                };
            }

            return new BookingDetailsDto
            {
                ConfirmationCode = booking.ConfirmationCode,
                Status = booking.BookingStatus?.Status ?? "Unknown",
                CreatedUtc = booking.CreatedUtc,
                Quantity = booking.Quantity,
                FlightScheduleId = ticket.FlightScheduleId,
                TicketId = ticket.Id,
                PassengerFullName = ticket.PassangerFullName,
                PassengerEmail = ticket.PassangerEmail,
                TotalAmount = ticket.TotalPrice * booking.Quantity,
                Currency = ticket.Currency
            };
        }

        public async Task CancelAsync(string code)
        {
            var booking = await _unitOfWork.BookingRepository.GetByConfirmationCodeAsync(code);

            if (booking is null)
                throw new KeyNotFoundException($"Booking with code '{code}' not found.");

            var cancelledStatus = await _unitOfWork.BookingRepository.GetByStatusAsync("Cancelled");

            if (cancelledStatus is null)
                throw new InvalidOperationException("Booking status 'Cancelled' is not configured.");

            // dacă e deja cancelled, o facem idempotent (204)
            if (booking.BookingStatusId == cancelledStatus.Id)
                return;

            booking.BookingStatusId = cancelledStatus.Id;

            // “inventory restored”: prin faptul că status devine Cancelled,
            // calculele de capacity nu mai includ booking-ul
            await _unitOfWork.SaveChangesAsync();

        }

        private static string GenerateConfirmationCode()
        {
            // simplu: 6 caractere alfanumerice
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = Random.Shared;

            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[rng.Next(chars.Length)])
                .ToArray());
        }
    }
}


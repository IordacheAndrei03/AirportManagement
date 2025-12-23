using AirportManagement.Application.Dtos.BookingDtos;
using AirportManagement.Application.Exceptions;
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

        public async Task<BookingCreateResponseDto> CreateAsync()
        {
            var userId = _currentUserService.UserId;

            var activeStatus = await _unitOfWork.BookingRepository.GetByStatusAsync("Active");

            if (activeStatus is null)
                throw new BadRequestException("Booking status 'Active' is not configured.");

            var confirmationCode = GenerateConfirmationCode();

            var booking = new Booking
            {
                UserId = userId, 
                BookingStatusId = activeStatus.Id,
                CreatedUtc = DateTime.UtcNow,
                ConfirmationCode = confirmationCode,
                Quantity = 1
            };

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();


            return new BookingCreateResponseDto
            {
                ConfirmationCode = confirmationCode,
                Status = "Active",
                Quantity = 1
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


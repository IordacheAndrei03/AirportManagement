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

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService CurentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = CurentUserService;
        }

        public async Task<ResultObject<BookingCreateResponseDto>> CreateAsync()
        {
            var userId = _currentUserService.UserId;

            var activeStatus = await _unitOfWork.BookingRepository.GetByStatusAsync("Active");

            if (activeStatus is null)
            {
                return ResultObject<BookingCreateResponseDto>.Invalid("Booking status 'Active' is not configured.");
            }

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


            var response = new BookingCreateResponseDto
            {
                ConfirmationCode = confirmationCode,
                Status = "Active",
                Quantity = 1
            };

            return ResultObject<BookingCreateResponseDto>.Success(response);
        }

        public async Task<ResultObject<BookingDetailsDto>> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return ResultObject<BookingDetailsDto>.Invalid("Booking code is required.");
            }

            var booking = await _unitOfWork.BookingRepository.GetByConfirmationCodeAsync(code);
            if (booking is null)
            {
                return ResultObject<BookingDetailsDto>.NotFound($"Booking with code '{code}' not found.");
            }

            var ticket = await _unitOfWork.TicketRepository.GetByBookingIdAsync(booking.Id);

            BookingDetailsDto dto;

            if (ticket is null)
            {
                dto = new BookingDetailsDto
                {
                    ConfirmationCode = booking.ConfirmationCode,
                    Status = booking.BookingStatus?.Status ?? "Unknown",
                    CreatedUtc = booking.CreatedUtc,
                    Quantity = booking.Quantity,
                    TotalAmount = 0
                };
            }
            else
            {
                dto = new BookingDetailsDto
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

            return ResultObject<BookingDetailsDto>.Success(dto);
        }

        public async Task<Result> CancelAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Result.Invalid("Booking code is required.");
            }

            var booking = await _unitOfWork.BookingRepository.GetByConfirmationCodeAsync(code);
            if (booking is null)
            {
                return Result.NotFound($"Booking with code '{code}' not found.");
            }

            var cancelledStatus = await _unitOfWork.BookingRepository.GetByStatusAsync("Cancelled");
            if (cancelledStatus is null)
            {
                return Result.Invalid("Booking status 'Cancelled' is not configured.");
            }

            if (booking.BookingStatusId == cancelledStatus.Id)
            {
                return Result.Success();
            }

            booking.BookingStatusId = cancelledStatus.Id;

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();

        }

        private static string GenerateConfirmationCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = Random.Shared;

            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[rng.Next(chars.Length)])
                .ToArray());
        }
    }
}


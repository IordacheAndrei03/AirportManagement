using AirportManagement.Application.Dtos.TicketDtos;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Application.Results;
using AirportManagement.Domain.Entities;
using AutoMapper;

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
            {
                return ResultObject<TicketCreateResponseDto>.Invalid("BasePrice must be positive.");
            }

            if (dto.Taxes < 0)
            {
                return ResultObject<TicketCreateResponseDto>.Invalid("Taxes must be non-negative.");
            }

            var booking = await _unitOfWork.BookingRepository.GetByIdWithStatusAsync(dto.BookingId);
            if (booking is null)
            {
                return ResultObject<TicketCreateResponseDto>.NotFound($"Booking {dto.BookingId} not found.");
            }

            var userId = _currentUserService.UserId;
            if (!string.Equals(booking.UserId, userId, StringComparison.Ordinal))
            {
                return ResultObject<TicketCreateResponseDto>.Forbidden("Cannot add ticket to another user's booking.");
            }

            if (!string.Equals(booking.BookingStatus?.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                return ResultObject<TicketCreateResponseDto>.Conflict("Cannot add tickets to a non-active booking.");
            }

            var schedule = await _unitOfWork.FlightScheduleRepository.GetByIdAsync(dto.FlightScheduleId);
            if (schedule is null)
            {
                return ResultObject<TicketCreateResponseDto>.NotFound($"FlightSchedule {dto.FlightScheduleId} not found.");
            }

            var capacity = await _unitOfWork.FlightScheduleRepository.GetSeatCapacityAsync(dto.FlightScheduleId);
            if (capacity <= 0)
            {
                return ResultObject<TicketCreateResponseDto>.Invalid("Aircraft capacity invalid.");
            }

            var soldSeats = await _unitOfWork.FlightScheduleRepository.GetActiveBookedSeatsAsync(dto.FlightScheduleId);
            if (soldSeats + 1 > capacity)
            {
                return ResultObject<TicketCreateResponseDto>.Conflict("Not enough seats available for this flight schedule.");
            }

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

            await _unitOfWork.BookingRepository.IncrementQuantityAsync(booking.Id);

            await _unitOfWork.SaveChangesAsync();

            var result = new TicketCreateResponseDto
            {
                FareClass = ticket.FareClass,
                TotalPrice = ticket.TotalPrice,
                IsRefundable = ticket.IsRefundable,
                SeatNumber = ticket.SeatNumber,
                PassengerFullName = ticket.PassangerFullName
            };

            return ResultObject<TicketCreateResponseDto>.Success(result);
        }

        public async Task<ResultObject<TicketSeatUpdateDto>> UpdateSeatNumberAsync(int ticketId, string seatNumber)
        {
            if (ticketId <= 0)
            {
                return ResultObject<TicketSeatUpdateDto>.Invalid("TicketId must be positive.");
            }

            if (string.IsNullOrWhiteSpace(seatNumber))
            {
                return ResultObject<TicketSeatUpdateDto>.Invalid("SeatNumber is required.");
            }

            seatNumber = seatNumber.Trim();

            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);
            if (ticket is null)
            {
                return ResultObject<TicketSeatUpdateDto>.NotFound($"Ticket {ticketId} not found.");
            }

            ticket.SeatNumber = seatNumber;

            await _unitOfWork.SaveChangesAsync();

            var dto = new TicketSeatUpdateDto
            {
                TicketId = ticket.Id,
                SeatNumber = ticket.SeatNumber
            };

            return ResultObject<TicketSeatUpdateDto>.Success(dto);
        }

        public async Task<ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>> GetByFlightScheduleAsync(int flightScheduleId)
        {
            if (flightScheduleId <= 0)
            {
                return ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>.Invalid("FlightScheduleId must be positive.");
            }

            var tickets = await _unitOfWork.TicketRepository.GetByFlightScheduleAsync(flightScheduleId);

            var dtoList = tickets.Select(t => _mapper.Map<TicketByFlightScheduleDto>(t)).ToList();

            return ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>.Success(dtoList);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var ticketRepo = _unitOfWork.TicketRepository;
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);

            if (ticket is null)
            {
                return Result.NotFound($"Ticket with Id={id} not found.");
            }

            await _unitOfWork.TicketRepository.DeleteByIdAsync(id);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}

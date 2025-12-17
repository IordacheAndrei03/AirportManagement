using AirportManagement.Application.Interfaces;
using AirprotManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirportManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AirportManagementContext _context;
        private IMapper _mapper;

        private IFlightRepository? _flightRepository;
        private IFlightScheduleRepository? _flightScheduleRepository;
        private ITicketRepository? _ticketRepository;
        private IBookingRepository? _bookingRepository;

        public UnitOfWork(AirportManagementContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IFlightRepository FlightRepository => 
            _flightRepository ??= new FlightRepository(_context,_mapper);

        public IFlightScheduleRepository FlightScheduleRepository =>
            _flightScheduleRepository ??= new FlightScheduleRepository(_context,_mapper);

        public ITicketRepository TicketRepository =>
            _ticketRepository ??= new TicketRepository(_context,_mapper);

        public IBookingRepository BookingRepository =>
            _bookingRepository ??= new BookingRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

using AirportManagement.Application.Interfaces.RepositoryInterfaces;
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
        private IAirlineRepository? _airlineRepository;
        private IAirportRepository? _airportRepository;
        private IAircraftRepository? _aircraftRepository;
        private IFlightStatusRepository? _flightStatusRepository;
        private IGateRepository? _gateRepository;
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
            _bookingRepository ??= new BookingRepository(_context,_mapper);

        public IAirlineRepository AirlineRepository =>
            _airlineRepository ??= new AirlineRepository(_context,_mapper);

        public IAirportRepository AirportRepository =>
            _airportRepository ??= new AirportRepository(_context,_mapper);

        public IAircraftRepository AircraftRepository =>
            _aircraftRepository ??= new AircraftRepository(_context,_mapper);

        public IFlightStatusRepository FlightStatusRepository =>
            _flightStatusRepository ??= new FlightStatusRepository(_context,_mapper);

        public IGateRepository GateRepository =>
            _gateRepository ??= new GateRepository(_context,_mapper);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

using AutoMapper;

using DomainFlight = AirportManagement.Domain.Entities.Flight;
using EfFlight = AirportManagement.Infrastructure.ScaffoldDb.Entities.Flight;

using DomainAircraft = AirportManagement.Domain.Entities.Aircraft;
using EfAircraft = AirportManagement.Infrastructure.ScaffoldDb.Entities.Aircraft;

using DomainAirport = AirportManagement.Domain.Entities.Airport;
using EfAirport = AirportManagement.Infrastructure.ScaffoldDb.Entities.Airport;

using DomainBooking = AirportManagement.Domain.Entities.Booking;
using EfBooking = AirportManagement.Infrastructure.ScaffoldDb.Entities.Booking;

using DomainTicket = AirportManagement.Domain.Entities.Ticket;
using EfTicket = AirportManagement.Infrastructure.ScaffoldDb.Entities.Ticket;

using DomainAirline = AirportManagement.Domain.Entities.Airline;
using EfAirline = AirportManagement.Infrastructure.ScaffoldDb.Entities.Airline;

namespace AirportManagement.Infrastructure.Mappers
{
    public class EfToDomainMapper:Profile
    {
        public EfToDomainMapper()
        {
            CreateMap<EfFlight, DomainFlight>().ReverseMap();
            CreateMap<EfAircraft, DomainAircraft>().ReverseMap();
            CreateMap<EfAirport, DomainAirport>().ReverseMap();
            CreateMap<EfBooking, DomainBooking>().ReverseMap();
            CreateMap<EfTicket, DomainTicket>().ReverseMap();
            CreateMap<EfAirline, DomainAirline>().ReverseMap();
        }
    }
}

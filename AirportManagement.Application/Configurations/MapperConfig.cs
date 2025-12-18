using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Flight, FlightDetailsDto>()
            .ForMember(dest => dest.AirlineName,
                opt => opt.MapFrom(src => src.Airline.Name))
            .ForMember(dest => dest.OriginIata,
                opt => opt.MapFrom(src => src.OriginAirportNavigation.Iatacode))
            .ForMember(dest => dest.DestinationIata,
                opt => opt.MapFrom(src => src.DestinationAirportNavigation.Iatacode))
            .ForMember(dest => dest.DefaultAircraftModel,
                opt => opt.MapFrom(src => src.DefaultAircraft.Model))
            .ReverseMap();

            CreateMap<FlightSchedule, FlightSearchScheduleDto>()
                .ForMember(dest => dest.AirlineIata,
                    opt => opt.MapFrom(src => src.Flight.Airline.Iatacode))
                .ForMember(dest => dest.FlightNumber,
                    opt => opt.MapFrom(src => src.Flight.FlightNumber))
                .ForMember(dest => dest.OriginIata,
                    opt => opt.MapFrom(src => src.Flight.OriginAirportNavigation.Iatacode))
                .ForMember(dest => dest.DestinationIata,
                    opt => opt.MapFrom(src => src.Flight.DestinationAirportNavigation.Iatacode))
                .ForMember(dest => dest.ScheduledDepartureUtc,
                    opt => opt.MapFrom(src => src.ScheduledDepartureUtc))
                .ForMember(dest => dest.ScheduledArrivalUtc,
                    opt => opt.MapFrom(src => src.ScheduleArrivalUtc));

            CreateMap<FlightCreateDto, Flight>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.AirlineId, opt => opt.Ignore())
                .ForMember(dest => dest.OriginAirport, opt => opt.Ignore())
                .ForMember(dest => dest.DestinationAirport, opt => opt.Ignore())
                .ForMember(dest => dest.DefaultAircraftId, opt => opt.Ignore())
                .ForMember(dest => dest.FlightNumber,
                    opt => opt.MapFrom(src => src.FlightNumber))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.IsActive));
        }
    }
}

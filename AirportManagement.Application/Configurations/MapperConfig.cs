using AirportManagement.Application.Dtos;
using AirportManagement.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Configurations
{
    public class MapperConfig:Profile
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
        }
    }
}

using AirportManagement.Application.Dtos;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Services
{
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FlightService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultObject<FlightDetailsDto>> GetByIdAsync(int id)
        {
            var flight = await _unitOfWork.FlightRepository.GetByIdWithDetailsAsync(id);
            var flightDto = _mapper.Map<FlightDetailsDto>(flight);

            return ResultObject<FlightDetailsDto>.Success(flightDto);
        }
    }
}

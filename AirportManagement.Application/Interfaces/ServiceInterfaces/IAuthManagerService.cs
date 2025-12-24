using AirportManagement.Application.Dtos;
using AirportManagement.Application.Dtos.AuthDtos;
using Microsoft.AspNetCore.Identity;

namespace AirportManagement.Application.Interfaces.ServiceInterfaces
{
    public interface IAuthManagerService
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);

        Task<AuthResponseDto> Login(LoginDto loginDto);
    }
}

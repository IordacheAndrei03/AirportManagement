using AirportManagement.Application.Dtos;
using AirportManagement.Application.Dtos.AuthDtos;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AirportManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManagerService _authManager;

        public AccountController(IAuthManagerService authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] ApiUserDto userDto)
        {
            var result = await _authManager.Register(userDto);
            if (result.Any())
            {
                foreach(var error in result)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok(userDto);
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authResponse = await _authManager.Login(loginDto);
            if (authResponse == null)
            {
                ModelState.AddModelError("Login", "Invalid login attempt.");
                return Unauthorized();
            }
            return Ok(authResponse);
        }
    }
}   


using AirportManagement.Application.Interfaces.ServiceInterfaces;
using System.Security.Claims;

namespace AirportManagement.Api.WeServices
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user is null)
                {
                    return null;
                }

                var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(id))
                {
                    return id;
                }

                id = user.FindFirstValue("sub");
                if (!string.IsNullOrWhiteSpace(id))
                {
                    return id;
                }

                id = user.FindFirstValue("uid");
                if (!string.IsNullOrWhiteSpace(id))
                {
                    return id;
                }

                return null;
            }
        }

    }
}

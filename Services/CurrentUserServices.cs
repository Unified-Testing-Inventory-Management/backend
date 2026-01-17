using System;
using System.Security.Claims;

namespace Server.Services;

public class CurrentUserServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserServices(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public Guid GetLoggedInUser()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User is not authenticated");

        return Guid.Parse(userIdClaim.Value);
    }
}

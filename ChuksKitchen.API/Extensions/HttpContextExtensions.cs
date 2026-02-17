using System.Security.Claims;

namespace ChuksKitchen.API.Extensions;

public static class HttpContextExtensions
{
    public static int GetUserId(this HttpContext context)
    {
        var userIdClaim = context.User?.FindFirst("userId")?.Value
                       ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to retrieve user ID from token");
        }

        return userId;
    }
}

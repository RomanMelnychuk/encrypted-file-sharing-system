using System.IdentityModel.Tokens.Jwt;
using System.Net;
using FileGuard.Shared;
using FileGuard.Shared.Exceptions;

namespace FileGuard.API.Providers;

public interface IUserProvider
{
    string GetCurrentUserId();
}

public class UserProvider(IHttpContextAccessor httpContextAccessor) : IUserProvider
{
    public string GetCurrentUserId()
    {
        var httpContext = httpContextAccessor.HttpContext
                          ?? throw new InvalidOperationException("HTTP context is not available.");

        var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            throw new StatusCodeBaseException((int)HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized, "No Bearer token found in the request headers.");
        }

        var token = authHeader["Bearer ".Length..];
        var handler = new JwtSecurityTokenHandler();

        var jsonToken = handler.ReadToken(token) as JwtSecurityToken
                        ?? throw new StatusCodeBaseException((int)HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized, "Invalid token");

        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new StatusCodeBaseException((int)HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized, "Token not contain userId");

        return userId;
    }
}

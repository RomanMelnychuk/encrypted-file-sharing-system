namespace FileGuard.API.Extensions;

public static class HttpContextExtensions
{
    private const string AccessTokenKey = "fg_access_token";

    public static void SetAuthCookies(this HttpContext context, string token)
    {
        var accessTokenCookieOptions = new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(1),
        };

        context.Response.Cookies.Append(AccessTokenKey, token, accessTokenCookieOptions);
    }
}
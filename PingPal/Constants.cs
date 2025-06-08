using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PingPal;

public static class Constants
{
    public const string DescSuffix = "Desc";

    public const int FirstPage = 1;
    public const int PageSize = 20;

    public const string MultiAuthScheme = "MultiAuthScheme";

    public static readonly TimeSpan CookieLifetime = TimeSpan.FromMinutes(60);

    public const string JwtIssuer = "HttpServerAuthorizeExample";
    public const string JwtAudience = "HttpServerAuthorizeExampleClient";
    public static readonly TimeSpan JwtLifetime = TimeSpan.FromMinutes(60);
    private const string JwtKey = "AnySecretKeyForHttpServerAuthorizeExample!123456";

    public static SymmetricSecurityKey GetJwtSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
    }
}
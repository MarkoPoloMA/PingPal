using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PingPal;

public static class Constants
{
	public const string JwtIssuer = "PingPal";
    public const string JwtAudience = "PingPalClient";
    public static readonly TimeSpan JwtLifetime = TimeSpan.FromMinutes(60);
    private const string JwtKey = "AnySecretKeyForHttpServerAuthorizeExample!123456";

    public static SymmetricSecurityKey GetJwtSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
    }
}
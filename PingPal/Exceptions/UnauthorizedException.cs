using System.Net;

namespace PingPal.Exceptions;

public class UnauthorizedException : ApiExceptionBase
{
    public override HttpStatusCode Code => HttpStatusCode.Unauthorized;

    public UnauthorizedException(string? message = null, Exception? exception = null)
        : base(message, exception)
    {
    }
}
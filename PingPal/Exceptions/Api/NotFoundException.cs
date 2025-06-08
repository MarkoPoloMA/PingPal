using System.Net;

namespace PingPal.Exceptions.Api;

public class NotFoundException : ApiExceptionBase
{
    public override HttpStatusCode Code => HttpStatusCode.NotFound;

    public NotFoundException(string? message = null, Exception? exception = null)
        : base(message, exception)
    {
    }
}
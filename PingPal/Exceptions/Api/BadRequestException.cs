using System.Net;

namespace PingPal.Exceptions.Api;

public class BadRequestException : ApiExceptionBase
{
    public override HttpStatusCode Code => HttpStatusCode.BadRequest;

    public BadRequestException(string? message = null, Exception? exception = null)
        : base(message, exception)
    {
    }
}
using System.Net;

namespace PingPal.Exceptions;

public class BadRequestException : ApiExceptionBase
{
    public override HttpStatusCode Code => HttpStatusCode.BadRequest;

    public BadRequestException(string? message = null, Exception? exception = null)
        : base(message, exception)
    {
    }
}
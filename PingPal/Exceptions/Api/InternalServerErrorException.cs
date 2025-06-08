using System.Net;

namespace PingPal.Exceptions.Api;

public class InternalServerErrorException : ApiExceptionBase
{
    public override HttpStatusCode Code => HttpStatusCode.InternalServerError;

    public InternalServerErrorException(string? message = null, Exception? exception = null)
        : base(message, exception)
    {
    }
}
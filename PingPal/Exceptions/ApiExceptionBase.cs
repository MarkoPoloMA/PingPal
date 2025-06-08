using System.Net;

namespace PingPal.Exceptions;

public abstract class ApiExceptionBase : Exception
{
    public abstract HttpStatusCode Code { get; }

    protected ApiExceptionBase(string? message, Exception? exception)
        : base(message ?? string.Empty, exception)
    {
    }
}
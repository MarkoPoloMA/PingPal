using System.Net;
using System.Net.Mime;
using PingPal.Exceptions;

namespace PingPal.Middlewares;

public class ApiExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ApiExceptionHandlerMiddleware> _logger;

    public ApiExceptionHandlerMiddleware(
        ILogger<ApiExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next.Invoke(httpContext);
        }
        catch (Exception exception)
        {
            if (!httpContext.Request.Path.StartsWithSegments("/api"))
                throw;

            var (code, message) = exception is ApiExceptionBase apiException
                ? (apiException.Code, apiException.Message)
                : (HttpStatusCode.InternalServerError, "Неизвестная ошибка");

            _logger.LogDebug(exception, $"Handled exception with code {code}");

            httpContext.Response.StatusCode = (int) code;
            httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
            await httpContext.Response.WriteAsync(message);
        }
    }
}
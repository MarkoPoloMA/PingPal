
using System.Net;

namespace PingPal.Middlewares;
public class SwaggerAuthorizedMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            var user = context.User;
            if (user.Identity.IsAuthenticated)
            {
                var role = user.Claims.Any(x => x.Type == "Admin");

                if (role)
                    await next.Invoke(context);
                else
                {
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest; 
                    await context.Response.WriteAsync("Вы не админ");
                }
            }
            else
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest; 
                await context.Response.WriteAsync("Не авторизован");
            }
        }
        else
            await next.Invoke(context);
    }
}


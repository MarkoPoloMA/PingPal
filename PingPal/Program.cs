using AspNetExample.Store;
using PingPal.Database.Context.Factory;
using PingPal.Domain.Entities;
using PingPal.Middlewares;
using PingPal.Store;
using Microsoft.AspNetCore.Builder;

namespace PingPal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IApplicationContextFactory, ApplicationContextFactory>(); 
            builder.Services.AddTransient<ApiExceptionHandlerMiddleware>();
            // Add services to the container.

            builder.Services.AddIdentity<User, Role>()
                .AddUserStore<ApplicationContextUserStore>()
                .AddRoleStore<ApplicationContextRoleStore>();

            builder.Services.AddControllersWithViews();

			builder.Services.AddControllers()
				.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            var app = builder.Build();
          
            if (!app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
            }

            app.UseMiddleware<ApiExceptionHandlerMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Account/Login");
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}

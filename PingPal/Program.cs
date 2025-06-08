using Microsoft.AspNetCore.Authentication.JwtBearer;
using PingPal.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using PingPal.Helpers;
using PingPal.Middlewares;
using PingPal.Service.Migrations;
using PingPal.Service.Startup;
using NLog.Extensions.Logging;
using NLog;
using NSwag;
using NSwag.Generation.Processors.Security;
using PingPal.NSwag;

namespace PingPal
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            ComponentModelResourceManagerHelper.OverrideResourceManager();

            var builder = WebApplication.CreateBuilder(args);

            LogManager.Configuration = new NLogLoggingConfiguration(
                builder.Configuration.GetSection("NLog"));

            Module.RegisterDependencies(builder.Services, builder.Configuration);

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = Constants.MultiAuthScheme;
                    options.DefaultChallengeScheme = Constants.MultiAuthScheme;
                    options.DefaultAuthenticateScheme = Constants.MultiAuthScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Constants.JwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = Constants.JwtAudience,
                        ValidateLifetime = true,
                        IssuerSigningKey = Constants.GetJwtSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddPolicyScheme(
                    Constants.MultiAuthScheme,
                    Constants.MultiAuthScheme,
                    options =>
                    {
                        options.ForwardDefaultSelector = context =>
                            context.Request.Path.StartsWithSegments("/api")
                                ? JwtBearerDefaults.AuthenticationScheme
                                : IdentityConstants.ApplicationScheme;
                    });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = Constants.CookieLifetime;
            });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;
            });

            builder.Services
                .AddControllersWithViews(options =>
                {
                    options.AllowEmptyInputInBodyModelBinding = true;
                    ComponentModelResourceManagerHelper.SetAccessorMessages(
                        options.ModelBindingMessageProvider);
                })
                .ConfigureApiBehaviorOptions(options =>
                    options.SuppressModelStateInvalidFilter = true);
            builder.Services
                .AddOpenApiDocument(settings =>
                {
                    settings.DocumentName = "PingPal";
                    settings.Title = "PingPal";
                    settings.Description = "API documentation";
                    settings.OperationProcessors.Insert(0, new OnlyApiOperationProcessor());

					settings.AddSecurity(
						JwtBearerDefaults.AuthenticationScheme,
						new string[] { },
						new global::NSwag.OpenApiSecurityScheme
						{
							Name = "Authorization",
							Type = OpenApiSecuritySchemeType.Http,
							In = OpenApiSecurityApiKeyLocation.Header,
							Description = "Введите JWT токен (префикс 'Bearer' добавится автоматически)",
							BearerFormat = "JWT",
							Scheme = JwtBearerDefaults.AuthenticationScheme,
							ExtensionData = new Dictionary<string, object?> { ["x-bearer-prefix"] = true }
						});

                    settings.OperationProcessors.Add(new OperationSecurityScopeProcessor(JwtBearerDefaults.AuthenticationScheme));

                    settings.PostProcess = document =>
                    {
                        if (document.Components.SecuritySchemes.TryGetValue(JwtBearerDefaults.AuthenticationScheme, out var securityScheme))
                            securityScheme.Scheme = JwtBearerDefaults.AuthenticationScheme;
                    };
                });

            builder.Services.AddSingleton<SwaggerAuthorizedMiddleware>();
            builder.Services.AddSingleton<ApiExceptionHandlerMiddleware>();
            builder.Services.AddSingleton<DatabaseCheckUserRolesMiddleware>();

            builder.Host.UseNLog();

            var app = builder.Build();

            await ApplyMigrationsAsync(app);
            InitializeUsersAndRolesAsync(app)
                .FireAndForgetSafeAsync();

            if (!app.Environment.IsDevelopment())
                app.UseExceptionHandler("/Home/Error");

            app.UseMiddleware<ApiExceptionHandlerMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<DatabaseCheckUserRolesMiddleware>();

            app.UseMiddleware<SwaggerAuthorizedMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
            }

            app.MapControllerRoute("default", "{controller=Home}/{action=Index}");

            await app.RunAsync();
        }

        private static async Task ApplyMigrationsAsync(IHost app)
        {
            using var scope = app.Services.CreateScope();

            var applicationContextMigrationsService = scope.ServiceProvider
                .GetRequiredService<IApplicationContextMigrationsService>();

            await applicationContextMigrationsService.ApplyMigrationsAsync();
        }

        private static async Task InitializeUsersAndRolesAsync(IHost app)
        {
            using var scope = app.Services.CreateScope();

            var applicationContextStartupService = scope.ServiceProvider
                .GetRequiredService<IApplicationContextStartupService>();

            await applicationContextStartupService.InitializeUsersAndRolesAsync();
        }
    }
}
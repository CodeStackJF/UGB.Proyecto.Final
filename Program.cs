using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using UGB.Proyecto.Final.Entities;
using UGB.Proyecto.Final.Helper;
using UGB.Proyecto.Final.Middleware;
using UGB.Proyecto.Final.Policies;
using UGB.Proyecto.Final.Repositories;
using UGB.Proyecto.Final.Validations.UsersValidation;
using UGB.Proyecto.FinalHelper;
using UGB.Proyecto.FinalInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddValidationInjection();

builder.Services.AddRepositoryInjection();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Disables the automatic 400 response for invalid models
    options.SuppressModelStateInvalidFilter = true;
});

//inyección de servicio de correo
builder.Services.AddSingleton<IConfigureOptions<SettingsBase>, MailSettings>();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddDbContext<StoreCTX>(options =>
    options.UseSqlite("Data Source=store.db"));

builder.Services.AddHttpContextAccessor();

//inyectamos la politica
builder.Services.AddSingleton<IAuthorizationHandler, ApiKeyPolicyHandler>();
builder.Services.AddAuthorization(options =>
{
    //aca se define el nombre de la politica que es necesaria para establecer en el controlador
    options.AddPolicy("API-KEY-POLICY", policy =>
        policy.Requirements.Add(new ApiKeyPolicyRequirement()));
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Optional: still print logs to console
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day) // Writes to logs/app-yyyyMMdd.txt
    .CreateLogger();

// 2. Clear default providers and inject Serilog into the ILogger pipeline
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login/index";
        options.LogoutPath = "/login/Logout";
        //options.AccessDeniedPath = "/login/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;

        // Medidas de seguridad para proteger la cookie de sesión.
        options.Cookie.Name = "Proyecto.Final.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.Use(async (context, next) =>
{
     context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] =
        "strict-origin-when-cross-origin";

    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self'; " +
        "img-src 'self' https: data:; " +
        "font-src 'self' https: data:; " +
        "connect-src 'self'; " +
        "object-src 'none'; " +
        "base-uri 'self'; " +
        "frame-ancestors 'self'; " +
        "form-action 'self';";

    await next();
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"status\":429,\"title\":\"Demasiadas peticiones\"," +
            "\"detail\":\"Ha alcanzado el límite de 1000 peticiones diarias para este token de API.\"}",
            cancellationToken);
    };

    options.AddPolicy("ApiUsersPolicy", httpContext =>
    {
        string userId = httpContext.User.GetProperty("UserId");        

        return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 2,
            Window = TimeSpan.FromDays(1),
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });
});

app.UseHttpsRedirection();
//app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

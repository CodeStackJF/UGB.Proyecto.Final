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

// Configura el sistema de autenticación de la aplicación.
builder.Services

    // Agrega autenticación utilizando las opciones especificadas.
    .AddAuthentication(options =>
    {
        // Define las cookies como esquema de autenticación predeterminado.
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        // Define las cookies como esquema utilizado para autenticar al usuario.
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        // Define las cookies como esquema utilizado cuando se requiere autenticación.
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })

    // Configura la autenticación basada en cookies.
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        // Indica la página a la que se enviará al usuario cuando no esté autenticado.
        options.LoginPath = "/login/index";

        // Indica la ruta utilizada para cerrar la sesión.
        options.LogoutPath = "/login/Logout";

        // Indica la página mostrada cuando el usuario no tiene permisos.
        options.AccessDeniedPath = "/login/AccessDenied";

        // Define que la cookie tendrá una duración de 2 horas.
        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        // Renueva la duración de la cookie mientras el usuario permanezca activo.
        options.SlidingExpiration = true;

        // Define un nombre personalizado para la cookie de autenticación.
        options.Cookie.Name = "Proyecto.Final.Auth";

        // Impide que JavaScript pueda acceder directamente a la cookie.
        options.Cookie.HttpOnly = true;

        // Evita que la cookie se envíe en solicitudes provenientes de otros sitios.
        options.Cookie.SameSite = SameSiteMode.Strict;

        // Obliga a enviar la cookie únicamente mediante conexiones HTTPS.
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


//esta configuracion permite evitar XSS, clickjacking, MIME sniffing y filtración innecesaria del Referer.
app.Use(async (context, next) =>
{
    //indica al navegador que debe respetar el content type enviado
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    //impide cargar el sitio en un iframe
    context.Response.Headers["X-Frame-Options"] = "DENY";
    //Controla qué información se envía mediante el encabezado Referer cuando el usuario navega desde tu sitio hacia otro
    context.Response.Headers["Referrer-Policy"] =
        "strict-origin-when-cross-origin";

    context.Response.Headers["Content-Security-Policy"] =
    //CSP le dice al navegador desde dónde puede cargar y ejecutar diferentes tipos de recursos, los recursos
    // no pueden venir de otro dominio
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
    //mensaje a mostrar cuando se alcance la cuota de peticiones
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"status\":429,\"title\":\"Demasiadas peticiones\"," +
            "\"detail\":\"Ha alcanzado el límite de 1000 peticiones diarias para este token de API.\"}",
            cancellationToken);
    };

    //politica para limitar peticiones
    options.AddPolicy("ApiUsersPolicy", httpContext =>
    {
        string userId = httpContext.User.GetProperty("UserId");        
        //se define que la clave de particion es el id del usuario, es decir, cada usuario tendra su propio contador
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
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

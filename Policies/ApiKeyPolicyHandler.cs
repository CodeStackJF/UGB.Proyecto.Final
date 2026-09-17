using Microsoft.AspNetCore.Authorization;

namespace UGB.Proyecto.Final.Policies
{
    //para toda politica se requiere un AuthorizationHandler que llame el IAuthorizationRequirement
    public class ApiKeyPolicyHandler : AuthorizationHandler<ApiKeyPolicyRequirement>
    {
        //llamamos las dependencias para leer el appsettings.json
        private readonly IConfiguration config;
        //este permite leer las cabeceras http
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApiKeyPolicyHandler(IConfiguration _config, IHttpContextAccessor _httpContextAccessor)
        {
            config = _config;
            httpContextAccessor = _httpContextAccessor;
        }

    protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context, 
    ApiKeyPolicyRequirement requirement)
    {
        //leemos el api key en el appsettings
        string API_KEY = config.GetValue<string>("API_KEY")!;
        //buscamos la cabecera en la petición
        string API_KEY_HEADER = httpContextAccessor.HttpContext!.Request.Headers["X-API-Key"]!;

        //Si no existe la cabecera impedimos el acceso
        if(string.IsNullOrWhiteSpace(API_KEY_HEADER))
        {
            return Task.CompletedTask;
        }
        
        //si existe y es igual al del appsettings entonces permitimos el acceso
        if(API_KEY.Equals(API_KEY_HEADER))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
        
    }
    }
}
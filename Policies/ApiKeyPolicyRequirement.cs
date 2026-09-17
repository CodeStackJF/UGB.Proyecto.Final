using Microsoft.AspNetCore.Authorization;

namespace UGB.Proyecto.Final.Policies
{
    //para toda politica se necesita un IAuthorizationRequirement
    public class ApiKeyPolicyRequirement  : IAuthorizationRequirement
    {
        
    }
}
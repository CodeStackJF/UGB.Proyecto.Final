using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGB.Proyecto.Final.Interfaces;

namespace UGB.Proyecto.Final.Repositories
{
    public static class RepositoryInjection
    {
        public static IServiceCollection AddRepositoryInjection(this IServiceCollection services)
        {
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<IUsersRolesRepository, UsersRolesRepository>();
            services.AddScoped<IOtpService, OtpRepository>();
            return services;
        }
    }
}
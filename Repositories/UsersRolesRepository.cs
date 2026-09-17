using UGB.Proyecto.Final.Entities;
using UGB.Proyecto.Final.Interfaces;

namespace UGB.Proyecto.Final.Repositories
{
    public class UsersRolesRepository(StoreCTX ctx) : IUsersRolesRepository
    {
        public async Task<users_roles> Insert(users_roles usersRoles)
        {
            usersRoles.role = null!;
            ctx.users_roles.Add(usersRoles);
            await ctx.SaveChangesAsync();
            return usersRoles;
        }

    }
}
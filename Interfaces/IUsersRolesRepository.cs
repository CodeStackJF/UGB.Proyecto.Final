using UGB.Proyecto.Final.Entities;

namespace UGB.Proyecto.Final.Interfaces
{
    public interface IUsersRolesRepository
    {
        public Task<users_roles> Insert(users_roles usersRoles);
    }
}
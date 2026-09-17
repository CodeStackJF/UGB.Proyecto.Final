using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.Proyecto.Final.Entities;

namespace UGB.Proyecto.Final.DatabaseConfiguration
{
    public class UsersRolesConfiguration : IEntityTypeConfiguration<users_roles>
    {
        public void Configure(EntityTypeBuilder<users_roles> builder)
        {
            //se define la llave compuesta de la tabla users_roles
            builder.HasKey(x=>new {x.role_id, x.user_id});

            //se define la relación entre la tabla users_roles y la tabla users
            builder.HasOne(x=>x.role).WithMany().HasForeignKey(x=>x.role_id);
        }
    }
}
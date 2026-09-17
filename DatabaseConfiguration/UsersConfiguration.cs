using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.Proyecto.Final.Entities;

namespace UGB.Proyecto.Final.DatabaseConfiguration
{
    public class UsersConfiguration : IEntityTypeConfiguration<users>
    {
        public void Configure(EntityTypeBuilder<users> builder)
        {
            //se define la llave primaria de la tabla roles
            builder.HasKey(x=>x.id);
            //se define la relación entre la tabla users y la tabla users_roles
            builder.HasMany(x=>x.users_roles).WithOne().HasForeignKey(x=>x.user_id);
        }

    }
}
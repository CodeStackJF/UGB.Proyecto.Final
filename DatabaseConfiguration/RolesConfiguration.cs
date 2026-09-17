using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.Proyecto.Final.Entities;
namespace UGB.Proyecto.Final.DatabaseConfiguration
{
    public class RolesConfiguration : IEntityTypeConfiguration<roles>
    {
        public void Configure(EntityTypeBuilder<roles> builder)
        {
            //se define la llave primaria de la tabla roles
            builder.HasKey(x=>x.id);
        }
    }
}
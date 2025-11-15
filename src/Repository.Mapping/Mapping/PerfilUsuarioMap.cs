using Repository.Mapping.Abstractions;
using Domain.Core.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;

public class PerfilUsuarioMap : BaseMap<PerfilUsuario, int>
{
    public PerfilUsuarioMap(DatabaseProvider provider) : base(provider) { }

    protected override void ConfigureEntity(EntityTypeBuilder<PerfilUsuario> builder)
    {
        builder.ToTable("PerfilUsuario");
        builder.HasKey(pu => pu.Id);
        builder.Property(pu => pu.Name).IsRequired();

        builder.HasData(
            new PerfilUsuario(PerfilUsuario.Perfil.Admin),
            new PerfilUsuario(PerfilUsuario.Perfil.User)
        );
    }
}
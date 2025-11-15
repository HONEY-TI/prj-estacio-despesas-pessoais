using Repository.Mapping.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;
public class ImagemPerfilUsuarioMap : BaseMap<ImagemPerfilUsuario, Guid>
{
    public ImagemPerfilUsuarioMap(DatabaseProvider provider) : base(provider) { }

    protected override void ConfigureEntity(EntityTypeBuilder<ImagemPerfilUsuario> builder)
    {
        builder.ToTable(nameof(ImagemPerfilUsuario));
        builder.HasKey(i => i.Id);
        builder.Property(i => i.UsuarioId).IsRequired();
        builder.HasIndex(i => i.UsuarioId).IsUnique();
        builder.Property(i => i.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(i => i.Name).IsUnique();

        builder.Property(i => i.Url).IsRequired();
        builder.HasIndex(i => i.Url).IsUnique();

        builder.Property(i => i.ContentType).IsRequired().HasMaxLength(20);
    }
}

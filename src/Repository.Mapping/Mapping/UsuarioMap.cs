using Repository.Mapping.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;

public class UsuarioMap : BaseMap<Usuario, Guid>
{
    public UsuarioMap(DatabaseProvider provider) : base(provider) { }

    protected override void ConfigureEntity(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable(nameof(Usuario));

        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique(true);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(50);

        builder.Property(u => u.Nome).HasMaxLength(50).IsRequired();
        builder.Property(u => u.SobreNome).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Telefone).HasMaxLength(15).IsRequired(false);

        builder.HasOne(u => u.PerfilUsuario).WithMany();

        builder.Property(u => u.Profile)
               .IsRequired(false)
               .HasColumnType(Provider switch
               {
                   DatabaseProvider.Oracle => "CLOB",
                   DatabaseProvider.MySql => "LONGTEXT",
                   DatabaseProvider.SqlServer => "NVARCHAR(MAX)",
                   _ => "TEXT"
               });
    }
}

using Repository.Mapping.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;

public class AcessoMap : BaseMap<Acesso, Guid>
{
    public AcessoMap(DatabaseProvider provider) : base(provider) { }

    protected override void ConfigureEntity(EntityTypeBuilder<Acesso> builder)
    {
        builder.ToTable("ControleAcesso");

        // Login
        builder.Property(ca => ca.Login)
               .IsRequired()
               .HasMaxLength(100);
        builder.HasIndex(ca => ca.Login).IsUnique();

        // Relacionamento com Usuario
        builder.Property(ca => ca.UsuarioId).IsRequired();

        // Tokens
        builder.Property(ca => ca.RefreshToken)
               .HasDefaultValue(null)
               .IsRequired(false);

        builder.Property(ca => ca.RefreshTokenExpiry)
               .HasDefaultValue(null)
               .IsRequired(false);

        // External Provider / Id
        builder.Property(ca => ca.ExternalProvider);
        builder.HasIndex(ca => ca.ExternalProvider).IsUnique(false);

        builder.Property(ca => ca.ExternalId);
        builder.HasIndex(ca => ca.ExternalId).IsUnique();
    }
}

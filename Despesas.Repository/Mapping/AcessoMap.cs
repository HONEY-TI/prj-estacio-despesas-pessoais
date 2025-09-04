using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;
public class AcessoMap : IEntityTypeConfiguration<Acesso>
{
    public void Configure(EntityTypeBuilder<Acesso> builder)
    {
        builder.ToTable("ControleAcesso");
        builder.Property(ca => ca.Id)
            .HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();
        builder.HasKey(ca => ca.Id);
        builder.HasIndex(ca => ca.Login).IsUnique(true);
        builder.Property(ca => ca.UsuarioId)
            .HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();
        builder.Property(ca => ca.Login).IsRequired().HasMaxLength(100);
        //builder.Property(ca => ca.Senha).IsRequired().HasColumnType("TEXT").HasDefaultValueSql("''");
        builder.Property(ca => ca.RefreshToken).HasDefaultValue(null).IsRequired(false);
        builder.Property(ca => ca.RefreshTokenExpiry).HasDefaultValue(null).IsRequired(false);

        builder.HasIndex(ca => ca.ExternalProvider).IsUnique(false);
        builder.Property(ca => ca.ExternalProvider);
        builder.HasIndex(ca => ca.ExternalId).IsUnique(true);
        builder.Property(ca => ca.ExternalId);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;
public class CategoriaMap : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable(nameof(Categoria));

        builder.Property(c => c.Id)
            .HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Descricao)
            .IsRequired(false)
            .HasMaxLength(100);
        builder.Property(ca => ca.UsuarioId)
            .HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();
        builder.Property(ca => ca.TipoCategoriaId)            
            .IsRequired();


        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Categorias)
            .HasForeignKey(c => c.UsuarioId);

        builder.HasOne(c => c.TipoCategoria)
            .WithMany()
            .HasForeignKey(c => c.TipoCategoriaId);

    }
}
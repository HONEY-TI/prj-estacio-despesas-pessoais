using Repository.Mapping.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;
public class CategoriaMap : BaseMap<Categoria, Guid>
{
    public CategoriaMap(DatabaseProvider provider) : base(provider) { }

    protected override void ConfigureEntity(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable(nameof(Categoria));

        builder.Property(c => c.Descricao)
               .IsRequired(false)
               .HasMaxLength(100);

        builder.Property(c => c.UsuarioId)
               .IsRequired();

        builder.Property(c => c.TipoCategoriaId)
               .IsRequired();

        // Relacionamentos
        builder.HasOne(c => c.Usuario)
               .WithMany(u => u.Categorias)
               .HasForeignKey(c => c.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.TipoCategoria)
               .WithMany()
               .HasForeignKey(c => c.TipoCategoriaId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

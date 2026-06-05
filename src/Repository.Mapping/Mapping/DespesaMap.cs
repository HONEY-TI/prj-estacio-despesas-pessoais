using Repository.Mapping.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;
public class DespesaMap : BaseMap<Despesa, Guid>
{
    public DespesaMap(DatabaseProvider provider) : base(provider) { }

    protected override void ConfigureEntity(EntityTypeBuilder<Despesa> builder)
    {
        builder.ToTable(nameof(Despesa));

        builder.Property(d => d.Descricao).IsRequired(false).HasMaxLength(100);
        builder.Property(d => d.UsuarioId).IsRequired();
        builder.Property(d => d.CategoriaId).IsRequired();

        builder.Property(d => d.Valor).HasColumnType("decimal(10, 2)").HasDefaultValue(0);

        builder.HasOne(d => d.Categoria)
               .WithMany(c => c.Despesas)
               .HasForeignKey(d => d.CategoriaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Usuario)
               .WithMany()
               .HasForeignKey(d => d.UsuarioId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}

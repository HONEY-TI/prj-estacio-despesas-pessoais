using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Repository.Mapping.Abstractions;

namespace Repository.Mapping;

public class LancamentoMap : BaseMap<Lancamento, Guid>
{
    public LancamentoMap(DatabaseProvider provider) : base(provider) { }

    protected override void ConfigureEntity(EntityTypeBuilder<Lancamento> builder)
    {
        builder.ToTable(nameof(Lancamento));
        builder.Property(l => l.Valor).HasColumnType("decimal(10, 2)");
        builder.Property(l => l.Descricao).HasMaxLength(100);
        builder.HasOne(l => l.Usuario)
               .WithMany()
               .HasForeignKey(l => l.UsuarioId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(l => l.Despesa)
               .WithMany()
               .HasForeignKey(l => l.DespesaId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(l => l.Receita)
               .WithMany()
               .HasForeignKey(l => l.ReceitaId)
               .OnDelete(DeleteBehavior.NoAction);

        ConfigureNullableGuid(builder, nameof(Lancamento.DespesaId));
        ConfigureNullableGuid(builder, nameof(Lancamento.ReceitaId));
    }
}

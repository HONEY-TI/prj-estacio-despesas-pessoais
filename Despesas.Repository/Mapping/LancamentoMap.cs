using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Repository.Mapping;
public class LancamentoMap : IEntityTypeConfiguration<Lancamento>
{
    public void Configure(EntityTypeBuilder<Lancamento> builder)
    {
        builder.ToTable(nameof(Lancamento));
        builder.Property(l => l.Id).HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();
        builder.HasKey(l => l.Id);
        builder.Property(l => l.UsuarioId)
            .HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();
        builder.Property(l => l.DespesaId)
                .HasColumnType("varchar(36)")
            .HasConversion(
            v => v.HasValue ? v.Value.ToString() : null,
            v => v != null ? new Guid(v) : (Guid?)null);

        builder.Property(l => l.ReceitaId)
            .HasColumnType("varchar(36)")
            .HasConversion(
            v => v.HasValue ? v.Value.ToString() : null,
            v => v != null ? new Guid(v) : (Guid?)null);

        //MySqlServer
        builder.Property(m => m.Data).HasColumnType("datetime").IsRequired();
        builder.Property(m => m.DataCriacao).HasColumnType("datetime").HasDefaultValueSql<DateTime>("CURRENT_TIMESTAMP");

        // MsSqlServer
        //builder.Property(l => l.Data).HasColumnType("datetime").IsRequired();
        //builder.Property(l => l.DataCriacao).HasColumnType("datetime").HasDefaultValueSql<DateTime>("GetDate()");            

        builder.Property(l => l.Valor).HasColumnType("decimal(10, 2)");
        builder.Property(l => l.Descricao).HasMaxLength(100);
        builder.HasOne(l => l.Usuario).WithMany().HasForeignKey(l => l.UsuarioId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(l => l.Despesa).WithMany().HasForeignKey(l => l.DespesaId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(l => l.Receita).WithMany().HasForeignKey(l => l.ReceitaId).OnDelete(DeleteBehavior.NoAction);
    }
}
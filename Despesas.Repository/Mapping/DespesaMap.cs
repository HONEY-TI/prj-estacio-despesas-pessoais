using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;
public class DespesaMap : IEntityTypeConfiguration<Despesa>
{
    public void Configure(EntityTypeBuilder<Despesa> builder)
    {
        builder.ToTable(nameof(Despesa));
        builder.Property(d => d.Id).HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Descricao).IsRequired(false).HasMaxLength(100);
        builder.Property(d => d.UsuarioId).HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();
        builder.Property(d => d.CategoriaId).HasColumnType("varchar(36)")
            .HasConversion(v => v.ToString(), v => new Guid(v))
            .IsRequired();

        // MySqlServer 
        builder.Property(m => m.Data).HasColumnType("datetime").HasDefaultValueSql<DateTime>("CURRENT_TIMESTAMP").IsRequired();
        builder.Property(m => m.DataVencimento).HasColumnType("datetime").HasDefaultValueSql(null);

        // MsSqlServer
        //builder.Property(d => d.Data).HasColumnType("datetime").HasDefaultValueSql<DateTime>("GetDate()").IsRequired();
        //builder.Property(d => d.DataVencimento).HasColumnType("datetime").HasDefaultValueSql(null);

        builder.HasOne(d => d.Categoria)   // Despesa -> Categoria
               .WithMany(c => c.Despesas)  // Categoria -> Despesas
               .HasForeignKey(d => d.CategoriaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(d => d.Valor).HasColumnType("decimal(10, 2)").HasDefaultValue(0);
        builder.HasOne(d => d.Usuario).WithMany().HasForeignKey(d => d.UsuarioId).OnDelete(DeleteBehavior.NoAction);
    }
}
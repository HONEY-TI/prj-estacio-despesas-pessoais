using Repository.Mapping.Abstractions;
using Domain.Core.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping;

public class TipoCategoriaMap : BaseMap<TipoCategoria, int>
{
    public TipoCategoriaMap(DatabaseProvider provider) : base(provider) { }

    protected override void ConfigureEntity(EntityTypeBuilder<TipoCategoria> builder)
    {
        builder.ToTable("TipoCategoria");
        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.Name).IsRequired();

        builder.HasData(
            new TipoCategoria(TipoCategoria.CategoriaType.Despesa),
            new TipoCategoria(TipoCategoria.CategoriaType.Receita)
        );
    }
}

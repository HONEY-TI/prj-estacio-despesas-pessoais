using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping.Abstractions;

public abstract class BaseMap<TEntity, TKey> : IEntityTypeConfiguration<TEntity> where TEntity : class
{
    private readonly DatabaseProvider _provider;

    protected BaseMap(DatabaseProvider provider)
    {
        _provider = provider;
    }

    // Expor provider para classes derivadas
    protected DatabaseProvider Provider => _provider;

    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureEntity(builder);
        ApplyProviderSpecifics(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);

    protected virtual void ApplyProviderSpecifics(EntityTypeBuilder<TEntity> builder)
    {
        var idProperty = typeof(TEntity).GetProperty("Id");
        if (idProperty != null)
        {
            if (typeof(TKey) == typeof(Guid))
            {
                switch (_provider)
                {
                    case DatabaseProvider.SqlServer:
                        builder.Property<Guid>("Id")
                               .IsRequired();
                        break;

                    case DatabaseProvider.MySql:
                        builder.Property<Guid>("Id")
                               .HasColumnType("varchar(36)")
                               .HasConversion(
                                   (Guid guid) => guid.ToString(),   // Guid → string
                                   (string str) => Guid.Parse(str)    // string → Guid
                               )
                               .IsRequired();
                        break;

                    case DatabaseProvider.Oracle:
                        builder.Property<Guid>("Id")
                               .HasColumnType("varchar(36)")
                               .HasConversion(
                                   (Guid guid) => guid.ToString(),
                                   (string str) => Guid.Parse(str)
                               )
                               .IsRequired();
                        break;
                }
            }
            else if (typeof(TKey) == typeof(int))
            {
                switch (_provider)
                {
                    case DatabaseProvider.SqlServer:
                    case DatabaseProvider.MySql:
                        builder.Property<int>("Id")
                               .HasColumnType("int")
                               .IsRequired();
                        break;

                    case DatabaseProvider.Oracle:
                        builder.Property<int>("Id")
                               .HasColumnType("NUMBER(10)")
                               .IsRequired();
                        break;
                }
            }
            builder.HasKey("Id");
        }

        foreach (var prop in typeof(TEntity).GetProperties())
        {
            if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                string dateType = _provider switch
                {
                    DatabaseProvider.SqlServer => "datetime",
                    DatabaseProvider.MySql => "datetime",
                    DatabaseProvider.Oracle => "TIMESTAMP",
                    _ => "datetime"
                };

                var propertyBuilder = builder.Property(prop.Name);
                propertyBuilder.HasColumnType(dateType);

                if (prop.PropertyType == typeof(DateTime))
                {
                    string defaultSql = _provider switch
                    {
                        DatabaseProvider.SqlServer => "GetDate()",
                        DatabaseProvider.MySql => "CURRENT_TIMESTAMP",
                        DatabaseProvider.Oracle => "CURRENT_TIMESTAMP",
                        _ => "CURRENT_TIMESTAMP"
                    };
                    propertyBuilder.HasDefaultValueSql(defaultSql).IsRequired();
                }
            }
        }
    }

    protected PropertyBuilder<Guid?> ConfigureNullableGuid<TEntityType>(EntityTypeBuilder<TEntityType> builder, string propertyName)
    where TEntityType : class
    {
        var propertyBuilder = builder.Property<Guid?>(propertyName);

        switch (_provider)
        {
            case DatabaseProvider.SqlServer:
                // SQL Server já lida com Guid? nativamente
                break;

            case DatabaseProvider.MySql:
            case DatabaseProvider.Oracle:
                propertyBuilder.HasColumnType("varchar(36)")
                               .HasConversion(
                                   v => v.HasValue ? v.Value.ToString() : null,  // Guid? → string
                                   v => v != null ? new Guid(v) : (Guid?)null   // string → Guid?
                               );
                break;
        }

        return propertyBuilder;
    }

}


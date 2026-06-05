using Repository.Mapping;
using Repository.Mapping.Abstractions;
using Domain.Core.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DatabaseContexts.Abstractions;

public abstract class BaseContext<TContext> : DbContext where TContext : DbContext
{
    protected readonly DatabaseProvider Provider;
    private readonly ILoggerFactory _loggerFactory;

    protected BaseContext(DbContextOptions<TContext> options, DatabaseProvider provider, ILoggerFactory loggerFactory = null)
        : base(options)
    {
        Provider = provider;
        _loggerFactory = loggerFactory;
    }

    // Entidades base
    public DbSet<TipoCategoria> TipoCategoria { get; set; }
    public DbSet<PerfilUsuario> PerfilUsuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapas base
        modelBuilder.ApplyConfiguration(new TipoCategoriaMap(Provider));
        modelBuilder.ApplyConfiguration(new PerfilUsuarioMap(Provider));
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_loggerFactory != null)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}
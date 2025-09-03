using Domain.Core.ValueObject;
using Microsoft.EntityFrameworkCore;
using Repository.Mapping;

namespace Repository.Abastractions;

public abstract class BaseContext<TContext> : DbContext where TContext : DbContext
{    
    public DbSet<TipoCategoria> TipoCategoria { get; set; }
    public DbSet<PerfilUsuario> PerfilUsuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new TipoCategoriaMap());
        modelBuilder.ApplyConfiguration(new PerfilUsuarioMap());
        base.OnModelCreating(modelBuilder);
    }

    public BaseContext(DbContextOptions<TContext> options) : base(options) {}
}
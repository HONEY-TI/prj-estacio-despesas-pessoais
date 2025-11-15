using Infrastructure.DatabaseContexts.Abstractions;
using Repository.Mapping;
using Repository.Mapping.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DatabaseContexts;

public class RegisterContext : BaseContext<RegisterContext>
{
    public RegisterContext(
        DbContextOptions<RegisterContext> options, 
        DatabaseProvider provider, 
        ILoggerFactory loggerFactory) : 
        base(options, provider, loggerFactory)   { }

    // DbSets específicos da aplicação
    public DbSet<Acesso> Acesso { get; set; }
    public DbSet<Categoria> Categoria { get; set; }
    public DbSet<Despesa> Despesa { get; set; }
    public DbSet<Receita> Receita { get; set; }
    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<ImagemPerfilUsuario> ImagemPerfilUsuario { get; set; }
    public DbSet<Lancamento> Lancamento { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapas específicos
        modelBuilder.ApplyConfiguration(new CategoriaMap(Provider));
        modelBuilder.ApplyConfiguration(new AcessoMap(Provider));
        modelBuilder.ApplyConfiguration(new DespesaMap(Provider));
        modelBuilder.ApplyConfiguration(new ReceitaMap(Provider));
        modelBuilder.ApplyConfiguration(new UsuarioMap(Provider));
        modelBuilder.ApplyConfiguration(new ImagemPerfilUsuarioMap(Provider));
        modelBuilder.ApplyConfiguration(new LancamentoMap(Provider));
    }
}

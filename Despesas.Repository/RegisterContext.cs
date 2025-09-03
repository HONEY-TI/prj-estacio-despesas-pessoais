using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Abastractions;
using Repository.Mapping;

namespace Repository;

public sealed class RegisterContext : BaseContext<RegisterContext>
{
    private readonly ILoggerFactory _loggerFactory;

    public DbSet<Acesso> Acesso { get; set; }
    public DbSet<Categoria> Categoria { get; set; }
    public DbSet<Despesa> Despesa { get; set; }
    public DbSet<Receita> Receita { get; set; }
    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<ImagemPerfilUsuario> ImagemPerfilUsuario { get; set; }
    public DbSet<Lancamento> Lancamento { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoriaMap());
        modelBuilder.ApplyConfiguration(new AcessoMap());
        modelBuilder.ApplyConfiguration(new DespesaMap());
        modelBuilder.ApplyConfiguration(new ReceitaMap());
        modelBuilder.ApplyConfiguration(new UsuarioMap());
        modelBuilder.ApplyConfiguration(new ImagemPerfilUsuarioMap());
        modelBuilder.ApplyConfiguration(new LancamentoMap());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies().UseLoggerFactory(_loggerFactory);
        base.OnConfiguring(optionsBuilder);
    }

    public RegisterContext(DbContextOptions<RegisterContext> options, ILoggerFactory loggerFactory) : base(options)
    {
        _loggerFactory = loggerFactory;
    }
}
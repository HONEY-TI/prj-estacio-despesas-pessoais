using Domain.Core.ValueObject;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Abastractions;
using Repository.Mapping;


namespace Repository;
public class RegisterContext : BaseContext<RegisterContext>
{
    public RegisterContext(DbContextOptions<RegisterContext> options) : base(options) { }

    public DbSet<Acesso> Acessos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Despesa> Despesas { get; set; }
    public DbSet<Receita> Receitas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<PerfilUsuario> PerfilUsuarios { get; set; }
    public DbSet<TipoCategoria> TipoCategorias { get; set; }
    public DbSet<ImagemPerfilUsuario> ImagemPerfilUsuarios { get; set; }
    public DbSet<Lancamento> Lancamentos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AcessoMap());
        modelBuilder.ApplyConfiguration(new CategoriaMap());
        modelBuilder.ApplyConfiguration(new DespesaMap());
        modelBuilder.ApplyConfiguration(new ReceitaMap());
        modelBuilder.ApplyConfiguration(new UsuarioMap());
        //modelBuilder.ApplyConfiguration(new PerfilUsuarioMap());
        //modelBuilder.ApplyConfiguration(new TipoCategoriaMap());
        modelBuilder.ApplyConfiguration(new ImagemPerfilUsuarioMap());
        modelBuilder.ApplyConfiguration(new LancamentoMap());

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies()
            .UseLoggerFactory(LoggerFactory.Create(x => x.AddConsole()));
        base.OnConfiguring(optionsBuilder);
    }
}

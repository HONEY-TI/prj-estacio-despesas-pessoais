using __mock__.Repository;
using Repository.Mapping.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Repository.Persistency.Implementations.Fixtures;

public sealed class DatabaseFixture : IDisposable
{
    public RegisterContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "UsuarioRepositorioImplTestDatabaseInMemory").Options;
        Context = new RegisterContext(options, DatabaseProvider.MySql,  Usings.GetLogerFactory());
        Context.Database.EnsureCreated();

        var lstAcesso = MockAcesso.Instance.GetAcessos();
        lstAcesso.ForEach(c => c.Usuario = c.Usuario.CreateUsuario(c.Usuario));
        lstAcesso.ForEach(c => c.Usuario.PerfilUsuario = Context.PerfilUsuario.First(tc => tc.Id == c.Usuario.PerfilUsuario.Id));
        lstAcesso.Select(c => c.Usuario).ToList()
            .SelectMany(u => u.Categorias).ToList()
            .ForEach(c => c.TipoCategoria = Context.TipoCategoria.First(tc => tc.Id == c.TipoCategoria.Id));
        Context.AddRange(lstAcesso);

        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

using Repository.Persistency.Abstractions;
using Microsoft.EntityFrameworkCore;
using __mock__.Repository;
using Domain.Core.ValueObject;
using Repository.Mapping.Abstractions;

namespace Repository.Persistency.Implementations.Fixtures;

public sealed class AcessoRepositorioFixture : IDisposable
{
    public RegisterContext Context { get; private set; }
    public Mock<AcessoRepositorioImpl> Repository { get; private set; }
    public Mock<IAcessoRepositorioImpl> MockRepository { get; private set; }

    public AcessoRepositorioFixture()
    {
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "AcessoRepositorioImpl_Test").Options;
        Context = new RegisterContext(options, DatabaseProvider.MySql,  Usings.GetLogerFactory());
        Context.PerfilUsuario.Add(new PerfilUsuario(PerfilUsuario.Perfil.Admin));
        Context.PerfilUsuario.Add(new PerfilUsuario(PerfilUsuario.Perfil.User));
        Context.TipoCategoria.Add(new TipoCategoria(1));
        Context.TipoCategoria.Add(new TipoCategoria(2));
        Context.SaveChanges();

        var lstAcesso = MockAcesso.Instance.GetAcessos(2);
        lstAcesso.ForEach(c => c.Usuario.PerfilUsuario = Context.PerfilUsuario.First(tc => tc.Id == c.Usuario.PerfilUsuario.Id));
        lstAcesso.Select(c => c.Usuario).ToList()
            .SelectMany(u => u.Categorias).ToList()
            .ForEach(c => c.TipoCategoria = Context.TipoCategoria.First(tc => tc.Id == c.TipoCategoria.Id));

        Context.AddRange(lstAcesso);
        Context.SaveChanges();

        Repository = new Mock<AcessoRepositorioImpl>(Context);
        MockRepository = Mock.Get<IAcessoRepositorioImpl>(Repository.Object);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
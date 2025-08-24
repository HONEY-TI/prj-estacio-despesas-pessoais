using Repository.Persistency.Abstractions;
using Microsoft.EntityFrameworkCore;
using __mock__.Repository;
using Domain.Core.ValueObject;

namespace Repository.Persistency.Implementations.Fixtures;

public sealed class AcessoRepositorioFixture : IDisposable
{
    public RegisterContext Context { get; private set; }
    public Mock<AcessoRepositorioImpl> Repository { get; set; }
    public Mock<IAcessoRepositorioImpl> MockRepository { get; private set; }

    public AcessoRepositorioFixture()
    {
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "AcessoRepositorioImpl").Options;
        Context = new RegisterContext(options);
        Context.PerfilUsuario.Add(new PerfilUsuario(PerfilUsuario.Perfil.Admin));
        Context.PerfilUsuario.Add(new PerfilUsuario(PerfilUsuario.Perfil.User));
        Context.SaveChanges();

        var lstAcesso = MockAcesso.Instance.GetAcessos();
        lstAcesso.ForEach(c => c.Usuario.PerfilUsuario = Context.PerfilUsuario.First(tc => tc.Id == c.Usuario.PerfilUsuario.Id));
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
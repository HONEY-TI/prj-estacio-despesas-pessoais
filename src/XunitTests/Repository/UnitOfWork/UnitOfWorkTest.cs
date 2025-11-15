using __mock__.Entities;
using Repository.Mapping.Abstractions;
using Repository.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Repository.UnitOfWork;

public sealed class UnitOfWorkTest
{
    private RegisterContext GetContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<RegisterContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new RegisterContext(options, DatabaseProvider.MySql, Usings.GetLogerFactory());
    }

    [Fact]
    public async Task Repository_Should_Return_BaseUnitOfWork_Instance()
    {
        using var context = GetContext(nameof(Repository_Should_Return_BaseUnitOfWork_Instance));
        var uow = new Repository.UnitOfWork.UnitOfWork<Usuario>(context);

        var repo = await Task.FromResult(uow.Repository);

        Assert.NotNull(repo);
        Assert.IsAssignableFrom<BaseUnitOfWork<Usuario>>(repo);
    }

    [Fact]
    public async Task CommitAsync_Should_Persist_Changes()
    {
        using var context = GetContext(nameof(CommitAsync_Should_Persist_Changes));
        var uow = new Repository.UnitOfWork.UnitOfWork<Usuario>(context);

        var usuario = UsuarioFaker.Instance.GetNewFaker();
        await uow.Repository.Insert(usuario);
        await uow.CommitAsync();

        var result = await context.Usuario.FindAsync(usuario.Id);

        Assert.NotNull(result);
        Assert.NotNull(result?.Id);
        Assert.Equal(usuario.Nome, result.Nome);
    }
}

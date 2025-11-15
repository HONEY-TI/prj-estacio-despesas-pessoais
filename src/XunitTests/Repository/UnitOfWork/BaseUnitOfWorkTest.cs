using Repository.Mapping.Abstractions;
using Repository.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Repository.UnitOfWork;

public sealed class BaseUnitOfWorkTest
{
    private RegisterContext GetContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: dbName).Options;
        return new RegisterContext(options, DatabaseProvider.MySql, Usings.GetLogerFactory());
    }

    [Fact]
    public async Task Insert_Should_Add_Entity()
    {
        using var context = GetContext(nameof(Insert_Should_Add_Entity));
        var repo = new BaseUnitOfWork<Usuario>(context);

        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Teste" };

        await repo.Insert(usuario);
        await context.SaveChangesAsync();

        var result = await context.Usuario.FindAsync(usuario.Id);

        Assert.NotNull(result);
        Assert.Equal("Teste", result.Nome);
    }

    [Fact]
    public async Task Get_Should_Return_Entity()
    {
        using var context = GetContext(nameof(Get_Should_Return_Entity));
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Teste" };
        context.Usuario.Add(usuario);
        await context.SaveChangesAsync();

        var repo = new BaseUnitOfWork<Usuario>(context);
        var result = await repo.Get(usuario.Id);

        Assert.NotNull(result);
        Assert.Equal(usuario.Id, result!.Id);
    }

    [Fact]
    public async Task GetAll_Should_Return_All()
    {
        using var context = GetContext(nameof(GetAll_Should_Return_All));
        context.Usuario.AddRange(
            new Usuario { Id = Guid.NewGuid(), Nome = "U1" },
            new Usuario { Id = Guid.NewGuid(), Nome = "U2" }
        );
        await context.SaveChangesAsync();

        var repo = new BaseUnitOfWork<Usuario>(context);
        var result = await repo.GetAll();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Update_Should_Modify_Entity()
    {
        using var context = GetContext(nameof(Update_Should_Modify_Entity));
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Old" };
        context.Usuario.Add(usuario);
        await context.SaveChangesAsync();

        usuario.Nome = "New";
        var repo = new BaseUnitOfWork<Usuario>(context);
        await repo.Update(usuario);
        await context.SaveChangesAsync();

        var updated = await context.Usuario.FindAsync(usuario.Id);

        Assert.Equal("New", updated.Nome);
    }

    [Fact]
    public async Task Delete_Should_Remove_Entity()
    {
        using var context = GetContext(nameof(Delete_Should_Remove_Entity));
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Delete" };
        context.Usuario.Add(usuario);
        await context.SaveChangesAsync();

        var repo = new BaseUnitOfWork<Usuario>(context);
        await repo.Delete(usuario.Id);
        await context.SaveChangesAsync();

        var deleted = await context.Usuario.FindAsync(usuario.Id);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task Find_Should_Return_Filtered_Entities()
    {
        using var context = GetContext(nameof(Find_Should_Return_Filtered_Entities));
        context.Usuario.AddRange(
            new Usuario { Id = Guid.NewGuid(), Nome = "Maria" },
            new Usuario { Id = Guid.NewGuid(), Nome = "João" }
        );
        await context.SaveChangesAsync();

        var repo = new BaseUnitOfWork<Usuario>(context);
        var result = await repo.Find(u => u.Nome.Contains("Maria"));

        Assert.Single(result);
        Assert.Equal("Maria", result.First().Nome);
    }
}

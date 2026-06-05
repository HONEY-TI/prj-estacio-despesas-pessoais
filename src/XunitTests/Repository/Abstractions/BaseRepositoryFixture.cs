using __mock__.Repository;
using Repository.Mapping.Abstractions;
using Domain.Core.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Repository.Abstractions;

public sealed class BaseRepositoryFixture : IDisposable
{
    public RegisterContext Context { get; private set; }

    public BaseRepositoryFixture()
    {
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "BaseRepositoryFixturetDatabaseInMemory").Options;
        Context = new RegisterContext(options, DatabaseProvider.MySql,  Usings.GetLogerFactory());
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        var acesso = MockAcesso.Instance.GetAcesso();
        acesso.Usuario.CreateUsuario(acesso.Usuario);
        acesso.Usuario.PerfilUsuario = Context.PerfilUsuario.First(tc => tc.Id == acesso.Usuario.PerfilUsuario.Id);
        acesso.Usuario.Categorias.ToList()
            .ForEach(c => c.TipoCategoria = Context.TipoCategoria.First(tc => tc.Id == c.TipoCategoria.Id));
        Context.Add(acesso);
        Context.SaveChanges();


        var usuario = Context.Usuario.First();
        var receitas = MockReceita.Instance.GetReceitas();
        foreach (var receita in receitas)
        {
            receita.Usuario = usuario;
            receita.UsuarioId = usuario.Id;
            receita.Categoria = Context.Categoria.FirstOrDefault(c => c.Usuario.Id == usuario.Id && c.TipoCategoria == (int)TipoCategoria.CategoriaType.Receita);
            receita.CategoriaId = receita.Categoria.Id;
        }
        Context.Receita.AddRange(receitas);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

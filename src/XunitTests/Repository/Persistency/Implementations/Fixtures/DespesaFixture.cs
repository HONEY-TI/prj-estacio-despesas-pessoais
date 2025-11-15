using __mock__.Repository;
using Repository.Mapping.Abstractions;
using Domain.Core.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Repository.Persistency.Implementations.Fixtures;

public sealed class DespesaFixture : IDisposable
{
    public RegisterContext Context { get; private set; }

    public DespesaFixture()
    {
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "DespesaRepositorioImplTestDatabaseInMemory").Options;
        Context = new RegisterContext(options, DatabaseProvider.MySql, Usings.GetLogerFactory());
        Context.Database.EnsureCreated();

        var acesso = MockAcesso.Instance.GetAcesso();
        acesso.Usuario.CreateUsuario(acesso.Usuario);
        acesso.Usuario.PerfilUsuario = Context.PerfilUsuario.First(tc => tc.Id == acesso.Usuario.PerfilUsuario.Id);
        acesso.Usuario.Categorias.ToList()
            .ForEach(c => c.TipoCategoria = Context.TipoCategoria.First(tc => tc.Id == c.TipoCategoria.Id));
        Context.Add(acesso);
        Context.SaveChanges();

        var usuario = Context.Usuario.First();
        var despesas = MockDespesa.Instance.GetDespesas();
        foreach (var despesa in despesas)
        {
            despesa.Usuario = usuario;
            despesa.UsuarioId = usuario.Id;
            despesa.Categoria = Context.Categoria.FirstOrDefault(c => c.Usuario.Id == usuario.Id && c.TipoCategoria == (int)TipoCategoria.CategoriaType.Despesa);
            despesa.CategoriaId = despesa.Categoria.Id;
        }
        Context.AddRange(despesas);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

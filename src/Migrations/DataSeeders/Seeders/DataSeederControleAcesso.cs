using Infrastructure.DatabaseContexts;
using Domain.Entities;
using EasyCryptoSalt;
using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.Seeders;

public class DataSeederAcesso : ISeeder
{
    private readonly ICrypto _crypto;
    private readonly RegisterContext _context;

    public DataSeederAcesso(RegisterContext context, ICrypto crypto)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
    }

    public void Seed()
    {
        this.Insert();
    }

    private void Insert()
    {
        CreateAndSaveUser(
            nome: "Alex",
            sobrenome: "Ribeiro de Faria",
            telefone: "(21) 99287-9319",
            email: "alexfariakof@yahoo.com.br",
            senha: "toor",
            perfilId: 1
        );

        CreateAndSaveUser(
            nome: "Teste",
            sobrenome: "Teste",
            telefone: "(21) 9999-9999",
            email: "user@example.com",
            senha: "toor",
            perfilId: 2
        );
    }

    private void CreateAndSaveUser(
        string nome,
        string sobrenome,
        string telefone,
        string email,
        string senha,
        int perfilId)
    {
        var usuario = new Usuario
        {
            Nome = nome,
            SobreNome = sobrenome,
            Telefone = telefone,
            Email = email,
            StatusUsuario = StatusUsuario.Ativo
        };

        var acesso = new Acesso();
        acesso.CreateAccount(usuario, _crypto.Encrypt(senha));

        acesso.Usuario.PerfilUsuario = _context.PerfilUsuario.First(p => p.Id == perfilId);
        acesso.Usuario.Categorias.ToList()
            .ForEach(c => c.TipoCategoria = _context.TipoCategoria
                .First(tc => tc.Id == c.TipoCategoria.Id));

        _context.Add(acesso);
        _context.SaveChanges();
    }
}

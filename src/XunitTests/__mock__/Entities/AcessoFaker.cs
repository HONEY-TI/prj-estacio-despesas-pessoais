using Bogus;
using Application.Dtos;

namespace __mock__.Entities;
public sealed class AcessoFaker
{
    static int counter = 1;
    private static AcessoFaker? _instance;
    private static readonly object LockObject = new object();

    public static AcessoFaker Instance
    {
        get
        {
            lock (LockObject)
            {
                return _instance ??= new AcessoFaker();
            }
        }
    }

    public Acesso GetNewFaker(Usuario? usuario = null)
    {
        lock (LockObject)
        {
            if (usuario == null) usuario = UsuarioFaker.Instance.GetNewFaker();

            var acessoFaker = new Faker<Acesso>()
                .RuleFor(ca => ca.Id, Guid.NewGuid())
                .RuleFor(ca => ca.Login, usuario.Email)
                .RuleFor(ca => ca.Senha, f => f.Internet.Password(8, false, "", "!12345"))
                .RuleFor(ca => ca.UsuarioId, usuario.Id)
                .RuleFor(ca => ca.Usuario, usuario)
                .Generate();
            counter++;
            return acessoFaker;
        }
    }

    public AcessoDto GetNewFakerVM(Usuario? usuario = null)
    {
        lock (LockObject)
        {
            if (usuario == null) usuario = UsuarioFaker.Instance.GetNewFaker();

            var acessoDtoFaker = new Faker<AcessoDto>()
                .RuleFor(ca => ca.Nome, usuario.Nome)
                .RuleFor(ca => ca.SobreNome, usuario.SobreNome)
                .RuleFor(ca => ca.Email, usuario.Email)
                .RuleFor(ca => ca.Telefone, usuario.Telefone)
                .RuleFor(ca => ca.Senha, f => f.Internet.Password(8, false, "", "!12345"))
                .Generate();
            acessoDtoFaker.ConfirmaSenha = acessoDtoFaker.Senha;
            return acessoDtoFaker;
        }
    }

    public List<AcessoDto> AcessoDtos(int count = 3)
    {
        var listAcessoDto = new List<AcessoDto>();
        for (int i = 0; i < count; i++)
        {
            var usuario = UsuarioFaker.Instance.GetNewFaker();
            var acessoDto = GetNewFakerVM(usuario);
            listAcessoDto.Add(acessoDto);
        }

        return listAcessoDto;
    }

    public List<Acesso> Acessos(int count = 3)
    {
        lock (LockObject)
        {

            var listAcesso = new List<Acesso>();
            for (int i = 0; i < count; i++)
            {
                var usuario = UsuarioFaker.Instance.GetNewFaker();
                var acesso = GetNewFaker(usuario);
                listAcesso.Add(acesso);
            }
            return listAcesso;
        }
    }
}

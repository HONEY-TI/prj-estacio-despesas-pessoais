using Bogus;

namespace __mock__.Repository;
public sealed class MockAcesso
{
    private static MockAcesso? _instance;
    private static readonly object LockObject = new object();

    public static MockAcesso Instance
    {
        get
        {
            lock (LockObject)
            {
                return _instance ??= new MockAcesso();
            }
        }
    }

    public Acesso GetAcesso(Usuario? usuario = null)
    {
        lock (LockObject)
        {
            if (usuario == null) usuario = MockUsuario.Instance.GetUsuario();

            var mockAcesso = new Faker<Acesso>()
            .RuleFor(ca => ca.Id, Guid.NewGuid())
            .RuleFor(ca => ca.Login, usuario.Email)
            .RuleFor(ca => ca.Senha, "!12345")
            .RuleFor(ca => ca.UsuarioId, usuario.Id)
            .RuleFor(ca => ca.Usuario, usuario)
            .RuleFor(ca => ca.RefreshTokenExpiry, DateTime.UtcNow);

            return mockAcesso.Generate();
        }
    }

    public List<Acesso> GetAcessos(int count = 3)
    {
        lock (LockObject)
        {
            var listAcesso = new List<Acesso>();
            for (int i = 0; i < count; i++)
            {
                var usuario = MockUsuario.Instance.GetUsuario();
                var acesso = GetAcesso(usuario);
                acesso.UsuarioId = usuario.Id;
                listAcesso.Add(acesso);
            }
            return listAcesso;
        }
    }
}

